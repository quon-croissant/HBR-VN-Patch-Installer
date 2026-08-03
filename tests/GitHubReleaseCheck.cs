#:property TargetFramework=net10.0-windows
#:project ../src/HBR-EN-VNPatchInstaller.csproj

using System.Reflection;

var service = new GitHubService("vnkeyfc", "HBR-EN_VN-Patch");
var method = typeof(GitHubService).GetMethod(
    "GetLatestReleaseFromFeedAsync",
    BindingFlags.Instance | BindingFlags.NonPublic)!;
var release = await (Task<(string version, string downloadUrl, string fileName)?>)method.Invoke(service, null)!;
if (release is not { fileName: var fileName } || !fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
    throw new Exception("Không tìm được file ZIP trong release mới nhất.");

Console.WriteLine($"{release.Value.version} -> {release.Value.fileName}");
