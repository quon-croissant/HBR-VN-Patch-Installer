using System.IO.Compression;
using Newtonsoft.Json;

public class PatchManager
{
    private const string ActiveBepInExDirectory = "BepInEx";
    private const string DisabledBepInExDirectory = "BepInEx_";
    private const string ManifestFile = "hbr-vn_manifest.json";
    private const string LegacyManifestFile = "hbr-vn_manifest.txt";

    private static readonly string[] BepInExDirectories =
    {
        ActiveBepInExDirectory,
        DisabledBepInExDirectory
    };

    public bool IsPatchEnabled(string gamePath) =>
        Directory.Exists(GetBepInExPath(gamePath, ActiveBepInExDirectory));

    public bool IsPatchDisabled(string gamePath) =>
        Directory.Exists(GetBepInExPath(gamePath, DisabledBepInExDirectory));

    public void SetPatchEnabled(string gamePath, bool enabled)
    {
        var sourceDirectory = enabled ? DisabledBepInExDirectory : ActiveBepInExDirectory;
        var destinationDirectory = enabled ? ActiveBepInExDirectory : DisabledBepInExDirectory;
        var sourcePath = GetBepInExPath(gamePath, sourceDirectory);
        var destinationPath = GetBepInExPath(gamePath, destinationDirectory);

        if (!Directory.Exists(sourcePath))
            throw new DirectoryNotFoundException($"Không tìm thấy thư mục {sourceDirectory}.");

        if (Directory.Exists(destinationPath))
            throw new IOException($"Không thể đổi tên vì thư mục {destinationDirectory} đã tồn tại.");

        Directory.Move(sourcePath, destinationPath);
    }

    public bool GetBorderlessWindowEnabled(string gamePath)
    {
        var configPath = GetGraphicsConfigPath(gamePath);
        if (!File.Exists(configPath))
            return true;

        foreach (var line in File.ReadLines(configPath))
        {
            var parts = line.Split('=', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2
                && parts[0].Equals("EnableBorderlessWindow", StringComparison.OrdinalIgnoreCase)
                && bool.TryParse(parts[1], out var enabled))
                return enabled;
        }

        return true;
    }

    public (int Width, int Height) GetBorderlessResolution(string gamePath)
    {
        var configPath = GetGraphicsConfigPath(gamePath);
        if (!File.Exists(configPath))
            return (0, 0);

        var values = File.ReadLines(configPath)
            .Select(line => line.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1], StringComparer.OrdinalIgnoreCase);
        return (
            values.TryGetValue("ResolutionWidth", out var width) && int.TryParse(width, out var parsedWidth) ? parsedWidth : 0,
            values.TryGetValue("ResolutionHeight", out var height) && int.TryParse(height, out var parsedHeight) ? parsedHeight : 0);
    }

    public void SetBorderlessSettings(string gamePath, bool enabled, int width, int height)
    {
        var configPath = GetGraphicsConfigPath(gamePath);
        Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
        var lines = File.Exists(configPath) ? File.ReadAllLines(configPath).ToList() : new List<string>();
        if (!lines.Any(line => line.Trim().Equals("[Graphics]", StringComparison.OrdinalIgnoreCase)))
            lines.AddRange(new[] { "", "[Graphics]" });

        SetValue("EnableBorderlessWindow", enabled.ToString().ToLowerInvariant());
        SetValue("ResolutionWidth", width.ToString());
        SetValue("ResolutionHeight", height.ToString());

        File.WriteAllLines(configPath, lines);

        void SetValue(string key, string value)
        {
            var index = lines.FindIndex(line =>
                line.Split('=', 2, StringSplitOptions.TrimEntries)[0].Equals(key, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
                lines[index] = $"{key} = {value}";
            else
                lines.Add($"{key} = {value}");
        }
    }

    public bool GetArchivedCollaborationEventsEnabled(string gamePath)
    {
        var configPath = GetMainPluginConfigPath(gamePath);
        if (!File.Exists(configPath))
            return false;

        foreach (var line in File.ReadLines(configPath))
        {
            var parts = line.Split('=', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2
                && parts[0].Equals("EnableArchivedCollaborationEvents", StringComparison.OrdinalIgnoreCase)
                && bool.TryParse(parts[1], out var enabled))
                return enabled;
        }

        return false;
    }

    public void SetArchivedCollaborationEventsEnabled(string gamePath, bool enabled)
    {
        var configPath = GetMainPluginConfigPath(gamePath);
        Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
        var lines = File.Exists(configPath) ? File.ReadAllLines(configPath).ToList() : new List<string>();
        const string key = "EnableArchivedCollaborationEvents";
        var index = lines.FindIndex(line =>
            line.Split('=', 2, StringSplitOptions.TrimEntries)[0].Equals(key, StringComparison.OrdinalIgnoreCase));

        if (index >= 0)
            lines[index] = $"{key}={enabled}";
        else
        {
            lines.Add("");
            lines.Add("# TÍNH NĂNG THỬ NGHIỆM: hiện lại event Collaboration đã kết thúc.");
            lines.Add("# Chỉ hỗ trợ cốt truyện; nhiệm vụ, cửa hàng và Prism Event bị vô hiệu hóa.");
            lines.Add($"{key}={enabled}");
        }

        File.WriteAllLines(configPath, lines);
    }

    public bool IsInstalled(string gamePath)
    {
        foreach (var bepInExDirectory in BepInExDirectories)
        {
            if (File.Exists(GetManifestPath(gamePath, bepInExDirectory))
                || File.Exists(GetLegacyJsonManifestPath(gamePath, bepInExDirectory)))
                return true;

            var pluginsPath = Path.Combine(gamePath, bepInExDirectory, "plugins", "HBR-VN");
            if (File.Exists(Path.Combine(pluginsPath, "HBRTLFixUp.dll")))
                return true;
        }

        return false;
    }

    public string GetInstalledVersion(string gamePath)
    {
        foreach (var bepInExDirectory in BepInExDirectories)
        {
            var manifestPath = GetManifestPath(gamePath, bepInExDirectory);
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

            var legacyJsonManifestPath = GetLegacyJsonManifestPath(gamePath, bepInExDirectory);
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

            var legacyManifestPath = GetLegacyManifestPath(gamePath, bepInExDirectory);
            if (File.Exists(legacyManifestPath))
                return File.ReadLines(legacyManifestPath).FirstOrDefault() ?? "Không rõ";
        }

        return "Không rõ";
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

    private static string GetBepInExPath(string gamePath, string bepInExDirectory) =>
        Path.Combine(gamePath, bepInExDirectory);

    private static string GetGraphicsConfigPath(string gamePath)
    {
        var bepInExDirectory = Directory.Exists(GetBepInExPath(gamePath, ActiveBepInExDirectory))
            ? ActiveBepInExDirectory
            : Directory.Exists(GetBepInExPath(gamePath, DisabledBepInExDirectory))
                ? DisabledBepInExDirectory
                : throw new DirectoryNotFoundException("Không tìm thấy BepInEx để lưu tuỳ chỉnh.");
        return Path.Combine(gamePath, bepInExDirectory, "config", "com.hbrvipatch.graphics.cfg");
    }

    private static string GetMainPluginConfigPath(string gamePath)
    {
        var bepInExDirectory = Directory.Exists(GetBepInExPath(gamePath, ActiveBepInExDirectory))
            ? ActiveBepInExDirectory
            : Directory.Exists(GetBepInExPath(gamePath, DisabledBepInExDirectory))
                ? DisabledBepInExDirectory
                : throw new DirectoryNotFoundException("Không tìm thấy BepInEx để lưu tuỳ chỉnh.");
        return Path.Combine(gamePath, bepInExDirectory, "config", "HBRTLFixUp.cfg");
    }

    private static string GetManifestPath(
        string gamePath,
        string bepInExDirectory = ActiveBepInExDirectory) =>
        Path.Combine(gamePath, bepInExDirectory, "plugins", "HBR-VN", ManifestFile);

    private static string GetLegacyJsonManifestPath(
        string gamePath,
        string bepInExDirectory = ActiveBepInExDirectory) =>
        Path.Combine(gamePath, bepInExDirectory, "Translation", ManifestFile);

    private static string GetLegacyManifestPath(
        string gamePath,
        string bepInExDirectory = ActiveBepInExDirectory) =>
        Path.Combine(gamePath, bepInExDirectory, "Translation", LegacyManifestFile);

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
        ActiveBepInExDirectory,
        DisabledBepInExDirectory,
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
        foreach (var manifestPath in BepInExDirectories.SelectMany(bepInExDirectory => new[]
        {
            GetManifestPath(gamePath, bepInExDirectory),
            GetLegacyJsonManifestPath(gamePath, bepInExDirectory),
            GetLegacyManifestPath(gamePath, bepInExDirectory)
        }))
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
