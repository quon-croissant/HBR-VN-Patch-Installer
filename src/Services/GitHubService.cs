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
        var latest = releases.FirstOrDefault(release =>
            release["draft"]?.Value<bool>() != true
            && release["assets"]?.Any(asset =>
                asset["name"]?.ToString().EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == true) == true);

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

        foreach (var entry in feed.Descendants(atom + "entry"))
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
            return (version, downloadUrl, fileName);
        }

        return null;
    }

    public async Task DownloadFileAsync(string url, string destPath,
        IProgress<int>? progress = null)
    {
        using var response = await Http.GetAsync(url,
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var total = response.Content.Headers.ContentLength ?? -1;

        await using var stream = await response.Content.ReadAsStreamAsync();
        await using var file = File.Create(destPath);

        var buffer = new byte[81920];
        long downloaded = 0;
        int read;

        while ((read = await stream.ReadAsync(buffer)) > 0)
        {
            await file.WriteAsync(buffer.AsMemory(0, read));
            downloaded += read;
            if (total > 0)
                progress?.Report((int)(downloaded * 100 / total));
        }
    }

    private static HttpClient CreateHttpClient()
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", "HBRPatchInstaller");
        return http;
    }
}
