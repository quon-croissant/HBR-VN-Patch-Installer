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

        foreach (var regPath in uninstallPaths)
        {
            using var key = Registry.LocalMachine.OpenSubKey(regPath);
            if (key == null) continue;

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                using var subKey = key.OpenSubKey(subKeyName);
                var displayName = subKey?.GetValue("DisplayName") as string;
                if (displayName?.Contains("Heaven Burns Red") != true) continue;

                if (subKey?.GetValue("InstallLocation") is string loc
                    && Directory.Exists(loc)
                    && ValidateGamePath(loc))
                    return loc;

                if (subKey?.GetValue("UninstallString") is string uninstall)
                {
                    // Lấy phần trong ngoặc kép hoặc trước space đầu tiên
                    string exePath;
                    if (uninstall.StartsWith("\""))
                    {
                        var endQuote = uninstall.IndexOf('"', 1);
                        exePath = endQuote > 0 ? uninstall[1..endQuote] : uninstall.Trim('"');
                    }
                    else
                    {
                        exePath = uninstall.Split(' ')[0];
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
        }

        var defaultPath = @"C:\YostarGames\HeavenBurnsRed";
        if (Directory.Exists(defaultPath) && ValidateGamePath(defaultPath))
            return defaultPath;

        return null;
    }

    public static bool ValidateGamePath(string path) =>
    File.Exists(Path.Combine(path, "HeavenBurnsRed.exe"));
}