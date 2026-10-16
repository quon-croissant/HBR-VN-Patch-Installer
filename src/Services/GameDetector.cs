using Microsoft.Win32;
using Newtonsoft.Json.Linq;

public static class GameDetector
{
    public static string? GetGameVersion(string gamePath)
    {
        try
        {
            var manifestPath = Path.Combine(gamePath, "manifest.json");
            if (!File.Exists(manifestPath)) return null;
            var json = File.ReadAllText(manifestPath);
            var obj = JObject.Parse(json);
            return obj["version"]?.ToString();
        }
        catch { return null; }
    }

    public static string? DetectGamePath()
    {
        var uninstallPaths = new[]
        {
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        };

        foreach (var hive in new[] { Registry.LocalMachine, Registry.CurrentUser })
        {
            foreach (var regPath in uninstallPaths)
            {
                try
                {
                    var found = DetectFromUninstallKey(hive, regPath);
                    if (found != null) return found;
                }
                catch (Exception ex)
                {
                    // Không đọc được key này (thiếu quyền, v.v.) thì bỏ qua và thử key khác
                    System.Diagnostics.Debug.WriteLine($"Bỏ qua {regPath}: {ex.Message}");
                }
            }
        }

        var defaultPaths = new List<string> { @"C:\YostarGames\HeavenBurnsRed" };
        try
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (drive.DriveType != DriveType.Fixed || !drive.IsReady) continue;
                    var root = drive.RootDirectory.FullName;
                    defaultPaths.Add(Path.Combine(root, "YostarGames", "HeavenBurnsRed"));
                    defaultPaths.Add(Path.Combine(root, "Games", "YostarGames", "HeavenBurnsRed"));
                }
                catch { }
            }
        }
        catch { }

        foreach (var defaultPath in defaultPaths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                if (Directory.Exists(defaultPath) && ValidateGamePath(defaultPath))
                    return defaultPath;
            }
            catch { }
        }

        return null;
    }

    private static string? DetectFromUninstallKey(RegistryKey hive, string regPath)
    {
        using var key = hive.OpenSubKey(regPath);
        if (key == null) return null;

        foreach (var subKeyName in key.GetSubKeyNames())
        {
            try
            {
                using var subKey = key.OpenSubKey(subKeyName);
                var displayName = subKey?.GetValue("DisplayName") as string;
                if (displayName?.Contains("Heaven Burns Red", StringComparison.OrdinalIgnoreCase) != true) continue;

                if (subKey?.GetValue("InstallLocation") is string loc
                    && Directory.Exists(loc)
                    && ValidateGamePath(loc))
                    return loc;

                if (subKey?.GetValue("UninstallString") is string uninstall)
                {
                    // Lấy phần trong ngoặc kép hoặc phần đến hết ".exe" đầu tiên
                    uninstall = uninstall.Trim();
                    string exePath;
                    if (uninstall.StartsWith("\""))
                    {
                        var endQuote = uninstall.IndexOf('"', 1);
                        exePath = endQuote > 0 ? uninstall[1..endQuote] : uninstall.Trim('"');
                    }
                    else
                    {
                        var exeIndex = uninstall.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
                        exePath = exeIndex >= 0 ? uninstall[..(exeIndex + 4)] : uninstall;
                    }

                    var launcherDir = Path.GetDirectoryName(exePath);
                    if (launcherDir == null) continue;

                    var yostarDir = Path.GetDirectoryName(launcherDir);
                    if (yostarDir == null) continue;

                    var candidate = Path.Combine(yostarDir, "HeavenBurnsRed");
                    System.Diagnostics.Debug.WriteLine($"Candidate: {candidate}");
                    if (Directory.Exists(candidate) && ValidateGamePath(candidate))
                        return candidate;
                }
            }
            catch
            {
                // Bỏ qua subkey lỗi (thiếu quyền, đường dẫn không hợp lệ, v.v.)
            }
        }

        return null;
    }

    public static bool ValidateGamePath(string path) =>
    File.Exists(Path.Combine(path, "HeavenBurnsRed.exe"));
}