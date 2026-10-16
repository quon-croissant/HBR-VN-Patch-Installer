using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

public class GitHubService
{
    private static readonly HttpClient Http = CreateHttpClient();
    private readonly string _repoOwner;
    private readonly string _repoName;

    public GitHubService(string repoOwner, string repoName)
    {
        _repoOwner = repoOwner;
        _repoName = repoName;
    }

    public async Task<(string version, string downloadUrl, string fileName)?> GetLatestReleaseAsync()
    {
        var url = $"https://api.github.com/repos/{_repoOwner}/{_repoName}/releases?per_page=20";
        using var response = await Http.GetAsync(url);
        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
            return await GetLatestReleaseFromFeedAsync();

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var releases = JArray.Parse(json);
        var candidates = releases.Where(release =>
            release["draft"]?.Value<bool>() != true
            && release["assets"]?.Any(asset =>
                asset["name"]?.ToString().EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == true) == true)
            .ToList();

        // Chọn bản có phiên bản cao nhất trong các bản phân tích được; nếu không có bản nào thì giữ thứ tự của API.
        JToken? latest = null;
        foreach (var candidate in candidates)
        {
            var tag = candidate["tag_name"]?.ToString();
            if (!PatchVersion.IsParseable(tag)) continue;
            if (latest == null || PatchVersion.Compare(tag, latest["tag_name"]?.ToString()) > 0)
                latest = candidate;
        }
        latest ??= candidates.FirstOrDefault();

        if (latest == null) return null;

        var version = latest["tag_name"]?.ToString();
        var asset = latest["assets"]?.FirstOrDefault(a =>
            a["name"]?.ToString().EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == true);
        var downloadUrl = asset?["browser_download_url"]?.ToString();
        var fileName = asset?["name"]?.ToString();

        if (version == null || downloadUrl == null || fileName == null) return null;
        return (version, downloadUrl, fileName);
    }

    private async Task<(string version, string downloadUrl, string fileName)?> GetLatestReleaseFromFeedAsync()
    {
        var feed = XDocument.Parse(await Http.GetStringAsync(
            $"https://github.com/{_repoOwner}/{_repoName}/releases.atom"));
        XNamespace atom = "http://www.w3.org/2005/Atom";
        const string tagMarker = "/releases/tag/";

        (string version, string downloadUrl, string fileName)? best = null;

        // Chỉ xét tối đa 5 bản mới nhất để tránh gửi quá nhiều yêu cầu HTTP.
        foreach (var entry in feed.Descendants(atom + "entry").Take(5))
        {
            var releaseUrl = entry.Elements(atom + "link")
                .FirstOrDefault(link => link.Attribute("rel")?.Value == "alternate")?
                .Attribute("href")?.Value;
            var tagIndex = releaseUrl?.IndexOf(tagMarker, StringComparison.OrdinalIgnoreCase) ?? -1;
            if (tagIndex < 0) continue;

            var version = Uri.UnescapeDataString(releaseUrl![(tagIndex + tagMarker.Length)..]);
            var html = await Http.GetStringAsync(
                $"https://github.com/{_repoOwner}/{_repoName}/releases/expanded_assets/{Uri.EscapeDataString(version)}");
            var match = Regex.Match(
                html,
                $"href=\"(?<path>/{Regex.Escape(_repoOwner)}/{Regex.Escape(_repoName)}/releases/download/[^\"]+\\.zip)\"",
                RegexOptions.IgnoreCase);
            if (!match.Success) continue;

            var downloadUrl = new Uri(
                new Uri("https://github.com"),
                WebUtility.HtmlDecode(match.Groups["path"].Value)).AbsoluteUri;
            var fileName = Uri.UnescapeDataString(Path.GetFileName(new Uri(downloadUrl).AbsolutePath));
            if (best == null
                || PatchVersion.Compare(version, best.Value.version) > 0
                || (!PatchVersion.IsParseable(best.Value.version) && PatchVersion.IsParseable(version)))
                best = (version, downloadUrl, fileName);
        }

        return best;
    }

    public async Task DownloadFileAsync(string url, string destPath,
        IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        var stallTimeout = TimeSpan.FromSeconds(60);
        const string stallMessage =
            "Tải về bị gián đoạn: không nhận được dữ liệu trong 60 giây. Hãy kiểm tra kết nối mạng và thử lại.";

        try
        {
            // Giới hạn thời gian chờ phản hồi đầu tiên (header) bằng cùng ngưỡng 60 giây
            using var headerCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            headerCts.CancelAfter(stallTimeout);
            using var response = await Http.GetAsync(url,
                HttpCompletionOption.ResponseHeadersRead, headerCts.Token);
            response.EnsureSuccessStatusCode();
            var total = response.Content.Headers.ContentLength ?? -1;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var file = File.Create(destPath);

            var buffer = new byte[81920];
            long downloaded = 0;
            int read;

            while (true)
            {
                using var readCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                readCts.CancelAfter(stallTimeout);
                read = await stream.ReadAsync(buffer.AsMemory(), readCts.Token);
                if (read <= 0) break;

                await file.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                downloaded += read;
                if (total > 0)
                    progress?.Report((int)(downloaded * 100 / total));
                else
                    // Không biết dung lượng: báo số MB đã tải (tối đa 95) để thanh tiến trình vẫn nhúc nhích
                    progress?.Report((int)Math.Min(95, downloaded / (1024 * 1024)));
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Không phải do người dùng hủy: nghĩa là quá thời gian chờ dữ liệu
            throw new TimeoutException(stallMessage);
        }
    }

    private static HttpClient CreateHttpClient()
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", "HBRPatchInstaller");
        return http;
    }
}
