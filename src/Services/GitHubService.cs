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
        var url = $"https://api.github.com/repos/{_repoOwner}/{_repoName}/releases/latest";
        var json = await Http.GetStringAsync(url);
        var latest = JObject.Parse(json);

        var version = latest["tag_name"]?.ToString();
        var asset = latest["assets"]?.FirstOrDefault(a =>
            a["name"]?.ToString().EndsWith(".zip") == true);
        var downloadUrl = asset?["browser_download_url"]?.ToString();
        var fileName = asset?["name"]?.ToString();

        if (version == null || downloadUrl == null || fileName == null) return null;
        return (version, downloadUrl, fileName);
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
