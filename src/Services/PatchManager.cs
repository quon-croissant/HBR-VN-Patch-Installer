using System.IO.Compression;
using Newtonsoft.Json;

public class PatchManager
{
    private const string ManifestFile = "hbr-vn_manifest.json";
    private const string LegacyManifestFile = "hbr-vn_manifest.txt";

    public bool IsInstalled(string gamePath)
    {
        if (File.Exists(GetManifestPath(gamePath)) || File.Exists(GetLegacyJsonManifestPath(gamePath)))
            return true;

        var pluginsPath = Path.Combine(gamePath, "BepInEx", "plugins", "HBR-VN");
        return File.Exists(Path.Combine(pluginsPath, "HBRTLFixUp.dll"));
    }

    public string GetInstalledVersion(string gamePath)
    {
        var manifestPath = GetManifestPath(gamePath);
        if (File.Exists(manifestPath))
        {
            try
            {
                var manifest = JsonConvert.DeserializeObject<PatchManifest>(File.ReadAllText(manifestPath));
                return string.IsNullOrWhiteSpace(manifest?.Version) ? "Không rõ" : manifest.Version;
            }
            catch
            {
                return "Không rõ";
            }
        }

        var legacyJsonManifestPath = GetLegacyJsonManifestPath(gamePath);
        if (File.Exists(legacyJsonManifestPath))
        {
            try
            {
                var manifest = JsonConvert.DeserializeObject<PatchManifest>(File.ReadAllText(legacyJsonManifestPath));
                return string.IsNullOrWhiteSpace(manifest?.Version) ? "Không rõ" : manifest.Version;
            }
            catch
            {
                return "Không rõ";
            }
        }

        var legacyManifestPath = GetLegacyManifestPath(gamePath);
        if (!File.Exists(legacyManifestPath)) return "Không rõ";
        return File.ReadLines(legacyManifestPath).FirstOrDefault() ?? "Không rõ";
    }

    public async Task InstallAsync(string zipPath, string gamePath,
    string version, IProgress<string>? status = null,
    IProgress<int>? progress = null)
    {
        var installedPaths = new List<string>();
        status?.Report("Đang giải nén...");

        await Task.Run(() =>
        {
            using var archive = ZipFile.OpenRead(zipPath);
            var entries = archive.Entries.Where(e => !string.IsNullOrEmpty(e.Name)).ToList();
            var newFiles = new List<string>();
            var overwrittenBackups = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var backupRoot = Path.Combine(Path.GetTempPath(), "HBRPatchInstaller", "backup-" + Guid.NewGuid().ToString("N"));
            int total = entries.Count;
            int current = 0;
            var gameRoot = Path.GetFullPath(gamePath);
            if (!gameRoot.EndsWith(Path.DirectorySeparatorChar))
                gameRoot += Path.DirectorySeparatorChar;

            try
            {
                foreach (var entry in entries)
                {
                    var destPath = Path.GetFullPath(Path.Combine(gamePath, entry.FullName));
                    if (!destPath.StartsWith(gameRoot, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException($"File trong gói cài đặt có đường dẫn không hợp lệ: {entry.FullName}");

                    var destDir = Path.GetDirectoryName(destPath)!;
                    Directory.CreateDirectory(destDir);

                    var wasExisting = File.Exists(destPath);
                    if (wasExisting && !overwrittenBackups.ContainsKey(destPath))
                    {
                        Directory.CreateDirectory(backupRoot);
                        var backupPath = Path.Combine(backupRoot, Guid.NewGuid().ToString("N"));
                        File.Copy(destPath, backupPath, overwrite: false);
                        overwrittenBackups.Add(destPath, backupPath);
                    }

                    entry.ExtractToFile(destPath, overwrite: true);
                    installedPaths.Add(entry.FullName);
                    current++;

                    status?.Report($"Copy: {entry.FullName}");
                    progress?.Report(50 + (int)(current * 50.0 / total)); // 50-100%

                    if (!wasExisting)
                        newFiles.Add(destPath);
                }
            }
            catch
            {
                RollBackChangedFiles(overwrittenBackups, newFiles, gameRoot, status);
                throw;
            }
            finally
            {
                if (Directory.Exists(backupRoot))
                    Directory.Delete(backupRoot, recursive: true);
            }
        });

        var manifestPath = GetManifestPath(gamePath);
        Directory.CreateDirectory(Path.GetDirectoryName(manifestPath)!);

        var manifest = new PatchManifest
        {
            Version = version,
            InstalledAt = DateTimeOffset.Now,
            Files = installedPaths
        };
        var manifestJson = JsonConvert.SerializeObject(manifest, Formatting.Indented);
        await File.WriteAllTextAsync(manifestPath, manifestJson);

        status?.Report("Cài đặt hoàn tất!");
        progress?.Report(100);
    }

    private static string GetManifestPath(string gamePath) =>
        Path.Combine(gamePath, "BepInEx", "plugins", "HBR-VN", ManifestFile);

    private static string GetLegacyJsonManifestPath(string gamePath) =>
        Path.Combine(gamePath, "BepInEx", "Translation", ManifestFile);

    private static string GetLegacyManifestPath(string gamePath) =>
        Path.Combine(gamePath, "BepInEx", "Translation", LegacyManifestFile);

    private static void RollBackChangedFiles(
        IReadOnlyDictionary<string, string> overwrittenBackups,
        IEnumerable<string> newFiles,
        string gameRoot,
        IProgress<string>? status)
    {
        foreach (var path in newFiles.Reverse())
        {
            if (!path.StartsWith(gameRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
                continue;

            File.Delete(path);
            status?.Report($"Rollback: {Path.GetRelativePath(gameRoot, path)}");
        }

        foreach (var backup in overwrittenBackups.Reverse())
        {
            if (!backup.Key.StartsWith(gameRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(backup.Value))
                continue;

            File.Copy(backup.Value, backup.Key, overwrite: true);
            status?.Report($"Khôi phục: {Path.GetRelativePath(gameRoot, backup.Key)}");
        }
    }

    public async Task UninstallAsync(string gamePath, IProgress<string>? status = null)
    {
        var filesToDelete = new[]
        {
        "winhttp.dll",
        ".doorstop_version",
        "doorstop_config.ini",
        "changelog.txt",
    };

        var foldersToDelete = new[]
        {
        "BepInEx",
        "dotnet",
    };
        await Task.Run(() =>
        {
            foreach (var file in filesToDelete)
            {
                var full = Path.Combine(gamePath, file);
                if (File.Exists(full))
                {
                    File.Delete(full);
                    status?.Report($"Đã xóa: {file}");
                }
            }

            foreach (var folder in foldersToDelete)
            {
                var full = Path.Combine(gamePath, folder);
                if (Directory.Exists(full))
                {
                    Directory.Delete(full, recursive: true);
                    status?.Report($"Đã xóa thư mục: {folder}");
                }
            }
        });

        // Xóa manifest nếu có, kể cả định dạng txt cũ.
        foreach (var manifestPath in new[] { GetManifestPath(gamePath), GetLegacyJsonManifestPath(gamePath), GetLegacyManifestPath(gamePath) })
        {
            if (File.Exists(manifestPath))
                File.Delete(manifestPath);
        }

        status?.Report("Gỡ cài đặt hoàn tất!");
    }

    private sealed class PatchManifest
    {
        [JsonProperty("version")]
        public string Version { get; set; } = "";

        [JsonProperty("installedAt")]
        public DateTimeOffset InstalledAt { get; set; }

        [JsonProperty("files")]
        public List<string> Files { get; set; } = new();
    }
}
