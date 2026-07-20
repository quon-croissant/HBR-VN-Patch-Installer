using System.Diagnostics;
using System.Runtime.InteropServices;

public partial class MainForm : Form
{
    private static readonly Color VsWindow = Color.FromArgb(43, 39, 52);
    private static readonly Color VsPanel = Color.FromArgb(55, 49, 66);
    private static readonly Color VsPanelAlt = Color.FromArgb(68, 57, 82);
    private static readonly Color VsBorder = Color.FromArgb(178, 158, 190);
    private static readonly Color VsAccent = Color.FromArgb(214, 81, 151);
    private static readonly Color VsAccentHover = Color.FromArgb(247, 111, 183);
    private static readonly Color VsAccentBorder = Color.FromArgb(255, 183, 229);
    private static readonly Color VsText = Color.FromArgb(250, 245, 252);
    private static readonly Color VsSubtleText = Color.FromArgb(211, 174, 187);
    private static readonly Color VsSuccess = Color.FromArgb(124, 233, 207);
    private static readonly Color VsDanger = Color.FromArgb(255, 118, 137);
    private static readonly Color VsWarning = Color.FromArgb(255, 184, 142);

    private readonly GitHubService _github = new("vnkeyfc", "HBR-EN_VN-Patch");
    private readonly PatchManager _patchManager = new();
    private readonly ToolTip _toolTip = new();
    private readonly HashSet<Button> _roundedButtons = new();
    private readonly Dictionary<Button, Color> _buttonNormalBackColors = new();
    private readonly Panel _gamePathPill = new();
    private string _latestVersion = "—";
    private string? _latestReleaseVersion;

    public MainForm()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        SuspendLayout();
        InitializeComponent();
        WireRuntimePaintHandlers();
        ApplyVisualStudioTheme();
        CreateGamePathPill();
        LoadHeaderAssets();
        ApplyDoubleBuffering(this);
        ResumeLayout(false);

        if (IsInDesigner())
            return;

        _toolTip.AutoPopDelay = 10000;
        _toolTip.InitialDelay = 400;
        _toolTip.ReshowDelay = 100;
        SendMessage(_txtGamePath.Handle, EM_SETMARGINS, EC_LEFTMARGIN, 10);
        _txtGamePath.TextChanged += (_, _) => RefreshStatus();

        var detectedPath = GameDetector.DetectGamePath();
        _txtGamePath.Text = detectedPath ?? "";
        RefreshStatus();
        _ = FetchLatestVersionAsync();

        if (!GameDetector.ValidateGamePath(_txtGamePath.Text.Trim()))
        {
            MessageBox.Show(
                "Không tìm thấy thư mục cài đặt Heaven Burns Red.\nVui lòng chọn thư mục game thủ công bằng nút \"Duyệt...\".",
                "Không tìm thấy game",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void LoadHeaderAssets()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        using var iconStream = assembly.GetManifestResourceStream("HBR_EN_VNPatchInstaller.icon.png");
        if (iconStream != null)
        {
            using var iconImage = Image.FromStream(iconStream);
            _picIcon.Image = RenderHeaderIcon(iconImage, _picIcon.Size);
        }

        // Load font từ embedded resource
        using var fontStream = assembly.GetManifestResourceStream("HBR_EN_VNPatchInstaller.A-OTF-SOFTGOSTD-DEBOLD-BjPr71hu.otf");
        if (fontStream != null)
        {
            byte[] fontData = new byte[fontStream.Length];
            fontStream.ReadExactly(fontData);
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            var fontCollection = new System.Drawing.Text.PrivateFontCollection();
            fontCollection.AddMemoryFont(fontPtr, fontData.Length);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            if (fontCollection.Families.Length > 0)
                _lblTitle.Font = new Font(fontCollection.Families[0], 13f, FontStyle.Bold);
        }
    }

    private static bool IsInDesigner()
    {
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            return true;

        var processName = Process.GetCurrentProcess().ProcessName;
        var designerHosts = new[]
        {
            "devenv",
            "DesignToolsServer",
            "XDesProc",
            "WinFormsSurface",
            "ServiceHub.Host.DesignTools"
        };

        if (designerHosts.Any(host => processName.Contains(host, StringComparison.OrdinalIgnoreCase)))
            return true;

        var domainName = AppDomain.CurrentDomain.FriendlyName;
        return domainName.Contains("Design", StringComparison.OrdinalIgnoreCase) ||
               domainName.Contains("Designer", StringComparison.OrdinalIgnoreCase);
    }

    private void WireRuntimePaintHandlers()
    {
        foreach (var panel in new[] { _pnlHeader, _pnlGameInfo, _pnlPatchInfo, _pnlOps, _pnlOptions })
            panel.Paint += PanelBorder_Paint;

        _pnlLog.Paint += PnlLog_Paint;
    }

    private async Task FetchLatestVersionAsync()
    {
        try
        {
            var release = await _github.GetLatestReleaseAsync();
            _latestReleaseVersion = release?.version;
            _latestVersion = _latestReleaseVersion ?? "Không lấy được";
        }
        catch
        {
            _latestReleaseVersion = null;
            _latestVersion = "Lỗi kết nối";
        }

        RefreshStatus();
    }

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Chọn thư mục cài đặt Heaven Burns Red",
            UseDescriptionForTitle = true,
        };
        if (dialog.ShowDialog() == DialogResult.OK)
            _txtGamePath.Text = dialog.SelectedPath;
    }
    private void RefreshStatus()
    {
        var path = _txtGamePath.Text.Trim();

        if (string.IsNullOrEmpty(path) || !GameDetector.ValidateGamePath(path))
        {
            _lblGamePathValue.Text = "Không tìm thấy";
            _toolTip.SetToolTip(_lblGamePathValue, null);
            _toolTip.SetToolTip(_txtGamePath, null);
            _lblGamePathValue.ForeColor = VsDanger;
            _lblGameVersionValue.Text = "—";
            _lblBepInExValue.Text = "—";
            _lblInstalledVersion.Text = "—";
            _lblLatestVersion.Text = _latestVersion;
            _lblPatchStatus.Text = "Đường dẫn không hợp lệ";
            UpdateProgressStatus("Chọn thư mục game hợp lệ để tiếp tục", VsWarning);
            _btnInstall.Enabled = false;
            _btnReinstall.Enabled = false;
            _btnUninstall.Enabled = false;
            _btnLaunchGame.Enabled = false;
            return;
        }

        // Đường dẫn hợp lệ
        var shortPath = CompactPath(path, 38);
        _lblGamePathValue.Text = shortPath;
        _lblGamePathValue.ForeColor = VsText;
        _lblGamePathValue.SelectionStart = 0;
        _toolTip.SetToolTip(_lblGamePathValue, path);
        _toolTip.SetToolTip(_txtGamePath, path);
        _lblGameVersionValue.Text = GameDetector.GetGameVersion(path) ?? "—";
        _lblBepInExValue.Text = Directory.Exists(Path.Combine(path, "BepInEx"))
            ? "Đã cài đặt ✔" : "Chưa cài đặt";
        _lblBepInExValue.ForeColor = Directory.Exists(Path.Combine(path, "BepInEx"))
            ? VsAccentHover : VsWarning;

        if (_patchManager.IsInstalled(path))
        {
            var installedVersion = _patchManager.GetInstalledVersion(path);
            var updateAvailable = IsUpdateAvailable(installedVersion);
            _lblInstalledVersion.Text = installedVersion;
            _lblLatestVersion.Text = _latestVersion;
            _lblPatchStatus.Text = "Đã cài đặt ✔";
            _lblPatchStatus.ForeColor = VsAccentHover;
            UpdateProgressStatus("Bản Việt hóa đã sẵn sàng", VsSubtleText);
            _lblBepInExValue.ForeColor = VsAccentHover;

            _btnInstall.Text = "Cài đặt";
            _btnInstall.Enabled = false;
            StyleCommandButton(_btnInstall, ButtonKind.Disabled);

            _btnReinstall.Enabled = updateAvailable;
            StyleCommandButton(_btnReinstall, updateAvailable ? ButtonKind.Primary : ButtonKind.Disabled);
            _toolTip.SetToolTip(_btnReinstall, GetUpdateButtonToolTip(installedVersion, updateAvailable));

            _btnUninstall.Enabled = true;
            StyleCommandButton(_btnUninstall, ButtonKind.Danger);
            _btnLaunchGame.Enabled = true;
        }
        else
        {
            _lblInstalledVersion.Text = "Chưa cài";
            _lblLatestVersion.Text = _latestVersion;
            _lblPatchStatus.Text = "Chưa cài đặt";
            _lblPatchStatus.ForeColor = VsDanger;
            UpdateProgressStatus("Sẵn sàng cài đặt", VsSubtleText);

            _btnInstall.Text = "Cài đặt";
            _btnInstall.Enabled = true;
            StyleCommandButton(_btnInstall, ButtonKind.Primary);

            _btnReinstall.Enabled = false;
            StyleCommandButton(_btnReinstall, ButtonKind.Disabled);
            _toolTip.SetToolTip(_btnReinstall, "Cài đặt bản Việt hóa trước khi cập nhật");

            _btnUninstall.Enabled = false;
            StyleCommandButton(_btnUninstall, ButtonKind.Disabled);
            _btnLaunchGame.Enabled = true;
        }
    }

    private bool IsUpdateAvailable(string installedVersion) =>
        !string.IsNullOrWhiteSpace(_latestReleaseVersion)
        && !string.Equals(
            NormalizeVersion(installedVersion),
            NormalizeVersion(_latestReleaseVersion),
            StringComparison.OrdinalIgnoreCase);

    private string GetUpdateButtonToolTip(string installedVersion, bool updateAvailable)
    {
        if (string.IsNullOrWhiteSpace(_latestReleaseVersion))
            return "Chưa thể kiểm tra bản cập nhật từ GitHub";

        return updateAvailable
            ? $"Cập nhật từ {installedVersion} lên {_latestReleaseVersion}"
            : "Bạn đang sử dụng phiên bản mới nhất";
    }

    private static string NormalizeVersion(string version)
    {
        var normalized = version.Trim();
        if (normalized.StartsWith("v.", StringComparison.OrdinalIgnoreCase))
            return normalized[2..];
        if (normalized.StartsWith('v') || normalized.StartsWith('V'))
            return normalized[1..];
        return normalized;
    }

    private async void BtnInstall_Click(object? sender, EventArgs e) => await RunInstall(false);
    private async void BtnReinstall_Click(object? sender, EventArgs e) => await RunInstall(true);

    private async Task RunInstall(bool isReinstall = false)
    {
        if (!ShowTermsAcceptance())
            return;

        var confirm = MessageBox.Show(
            isReinstall ? "Bạn có chắc muốn cập nhật bản Việt hóa không?" : "Bạn có chắc muốn cài đặt bản Việt hóa không?",
            isReinstall ? "Xác nhận cập nhật" : "Xác nhận cài đặt",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        var gamePath = _txtGamePath.Text.Trim();
        SetBusyState(true);
        ShowProgress(0);
        UpdateProgressStatus("Đang kiểm tra phiên bản mới nhất...", VsSubtleText);
        string? tempDir = null;

        try
        {
            Log("Đang kiểm tra phiên bản mới nhất...");
            var release = await _github.GetLatestReleaseAsync();
            if (release == null)
            {
                Log("✘ Không lấy được thông tin release.");
                _progressBar.Value = 0;
                UpdateProgressStatus("Không lấy được thông tin release", VsDanger);
                return;
            }

            _latestReleaseVersion = release.Value.version;
            _latestVersion = release.Value.version;
            _lblLatestVersion.Text = _latestVersion;
            Log($"Phiên bản mới nhất: {release.Value.version}");

            // Dùng tên file thật từ GitHub thay vì hardcode
            tempDir = Path.Combine(Path.GetTempPath(), "HBRPatchInstaller", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var tempZip = Path.Combine(tempDir, release.Value.fileName);
            System.Diagnostics.Debug.WriteLine($"Temp zip path: {tempZip}");
            Log($"Đang tải về: {release.Value.fileName}");
            UpdateProgressStatus("Đang tải bản vá...", VsSubtleText);

            var dlProgress = new Progress<int>(v =>
            {
                _progressBar.Value = v / 2;
                _progressBar.Refresh();
            });
            await _github.DownloadFileAsync(release.Value.downloadUrl, tempZip, dlProgress);
            Log("Tải về hoàn tất. Đang cài đặt...");
            UpdateProgressStatus("Đang cài đặt bản vá...", VsSubtleText);

            var installProgress = new Progress<string>(msg =>
            {
                Log(msg);
            });

            var installProgressPct = new Progress<int>(v =>
            {
                _progressBar.Value = v;
                _progressBar.Refresh();
            });

            await _patchManager.InstallAsync(tempZip, gamePath, release.Value.version, installProgress, installProgressPct);

            ShowProgress(100);
            UpdateProgressStatus("Cài đặt hoàn tất", VsSuccess);
        }
        catch (Exception ex)
        {
            Log($"✘ Lỗi: {ex.Message}");
            _progressBar.Value = 0;
            UpdateProgressStatus("Cài đặt thất bại", VsDanger);
            MessageBox.Show($"Cài đặt thất bại:\n{ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (tempDir != null && Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
            await Task.Delay(1500);
            SetBusyState(false);
            RefreshStatus();
        }
    }

    private bool ShowTermsAcceptance()
    {
        using var form = new TosForm(requireAcceptance: true);
        return form.ShowDialog(this) == DialogResult.OK && form.AcceptedTerms;
    }

    private async void BtnUninstall_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "Thao tác này sẽ xóa bản Việt hóa và môi trường mod/plugin BepInEx khỏi thư mục game.\nBạn có chắc muốn tiếp tục không?",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        SetBusyState(true);
        UpdateProgressStatus("Đang gỡ cài đặt...", VsSubtleText);
        try
        {
            var progress = new Progress<string>(Log);
            await _patchManager.UninstallAsync(_txtGamePath.Text.Trim(), progress);
            UpdateProgressStatus("Gỡ cài đặt hoàn tất", VsSuccess);
            //Log("✔ Gỡ cài đặt thành công!");
        }
        catch (Exception ex) { Log($"✘ Lỗi: {ex.Message}"); UpdateProgressStatus("Gỡ cài đặt thất bại", VsDanger); }
        finally { SetBusyState(false); RefreshStatus(); }
    }

    private void BtnLaunchGame_Click(object? sender, EventArgs e)
    {
        var exe = Path.Combine(_txtGamePath.Text.Trim(), "HeavenBurnsRed.exe");
        if (!File.Exists(exe)) { Log("✘ Không tìm thấy HeavenBurnsRed.exe"); return; }
        Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true });
        Log("Đang khởi động game...");
    }

    private void BtnTos_Click(object? sender, EventArgs e)
    {
        using var form = new TosForm();
        form.ShowDialog(this);
    }

    // ── Helpers ──────────────────────────────────────────────
    private enum ButtonKind
    {
        Primary,
        Secondary,
        Success,
        Danger,
        Disabled
    }

    private void ApplyVisualStudioTheme()
    {
        BackColor = VsWindow;

        foreach (var panel in new[] { _pnlHeader, _pnlGameInfo, _pnlPatchInfo, _pnlOps, _pnlOptions })
            panel.BackColor = VsPanel;

        foreach (var label in new[]
        {
            _lblSubtitle, _lblGamePathKey, _lblGameVersionKey, _lblBepInExKey,
            _lblInstalledKey, _lblLatestKey, _lblPatchStatusKey,
            _lblProgressStatusTitle
        })
        {
            label.ForeColor = VsSubtleText;
        }

        foreach (var label in new[]
        {
            _lblTitle, _lblGameVersionValue, _lblInstalledVersion,
            _lblLatestVersion, _lblPatchStatus, _lblBepInExValue
        })
        {
            label.ForeColor = VsText;
        }

        foreach (var label in new[] { _lblGameInfoTitle, _lblPatchInfoTitle, _lblOpsTitle, _lblOptionsTitle })
        {
            label.ForeColor = VsAccent;
            label.Font = new Font(label.Font, FontStyle.Bold);
        }

        foreach (var textBox in new[] { _txtGamePath, _lblGamePathValue })
        {
            textBox.BackColor = VsPanel;
            textBox.ForeColor = VsText;
        }

        _txtGamePath.BackColor = VsPanelAlt;

        _rtbLog.BackColor = Color.FromArgb(34, 30, 42);
        _rtbLog.ForeColor = Color.FromArgb(238, 226, 241);
        _pnlLog.BackColor = _rtbLog.BackColor;

        _progressBar.BackColor = Color.FromArgb(118, 96, 136);
        _progressBar.TrackColor = _progressBar.BackColor;
        _progressBar.ForeColor = VsAccent;
        _lblProgressStatus.ForeColor = VsSubtleText;

        StyleCommandButton(_btnBrowse, ButtonKind.Secondary);
        StyleCommandButton(_btnInstall, ButtonKind.Primary);
        StyleCommandButton(_btnReinstall, ButtonKind.Secondary);
        StyleCommandButton(_btnUninstall, ButtonKind.Danger);
        StyleCommandButton(_btnLaunchGame, ButtonKind.Success);
        StyleCommandButton(_btnTos, ButtonKind.Secondary);
        _gamePathPill.BackColor = VsPanelAlt;
        _gamePathPill.Invalidate();
    }

    private void CreateGamePathPill()
    {
        var originalBounds = _txtGamePath.Bounds;
        _txtGamePath.BorderStyle = BorderStyle.None;

        _gamePathPill.Location = originalBounds.Location;
        _gamePathPill.Size = new Size(originalBounds.Width, _btnBrowse.Height);
        CenterGamePathTextBox();
        _txtGamePath.Width = originalBounds.Width - 28;
        _gamePathPill.BackColor = VsPanelAlt;
        _gamePathPill.Paint += GamePathPill_Paint;
        _gamePathPill.Resize += (_, _) =>
        {
            CenterGamePathTextBox();
            _txtGamePath.Width = _gamePathPill.Width - 28;
            _gamePathPill.Invalidate();
        };
        _gamePathPill.Click += (_, _) => _txtGamePath.Focus();

        Controls.Remove(_txtGamePath);
        _gamePathPill.Controls.Add(_txtGamePath);
        Controls.Add(_gamePathPill);
        _gamePathPill.BringToFront();
        _btnBrowse.BringToFront();
    }

    private void CenterGamePathTextBox()
    {
        var visualOffset = 2;
        var y = Math.Max(3, (_gamePathPill.Height - _txtGamePath.Height) / 2 + visualOffset);
        _txtGamePath.Location = new Point(14, y);
    }

    private void StyleCommandButton(Button button, ButtonKind kind)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.UseVisualStyleBackColor = false;
        button.FlatAppearance.BorderSize = 0;

        switch (kind)
        {
            case ButtonKind.Primary:
                button.BackColor = VsAccent;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = VsAccentBorder;
                button.FlatAppearance.MouseOverBackColor = VsAccentHover;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(169, 56, 103);
                break;
            case ButtonKind.Success:
                button.BackColor = Color.FromArgb(80, 61, 95);
                button.ForeColor = Color.FromArgb(255, 241, 248);
                button.FlatAppearance.BorderColor = Color.FromArgb(232, 186, 244);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(102, 72, 116);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(54, 42, 67);
                break;
            case ButtonKind.Danger:
                button.BackColor = Color.FromArgb(95, 38, 52);
                button.ForeColor = Color.FromArgb(255, 214, 221);
                button.FlatAppearance.BorderColor = VsDanger;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(123, 48, 65);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(73, 29, 40);
                break;
            case ButtonKind.Disabled:
                button.BackColor = Color.FromArgb(48, 43, 57);
                button.ForeColor = Color.FromArgb(128, 112, 132);
                button.FlatAppearance.BorderColor = Color.FromArgb(92, 78, 101);
                button.FlatAppearance.MouseOverBackColor = button.BackColor;
                button.FlatAppearance.MouseDownBackColor = button.BackColor;
                break;
            default:
                button.BackColor = VsPanelAlt;
                button.ForeColor = VsText;
                button.FlatAppearance.BorderColor = VsBorder;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(84, 68, 99);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(48, 39, 59);
                break;
        }

        ApplyRoundedButton(button);
        _buttonNormalBackColors[button] = button.BackColor;
    }

    private void ApplyRoundedButton(Button button)
    {
        if (_roundedButtons.Add(button))
        {
            button.Paint += RoundedCommandButton_Paint;
            button.Resize += (_, _) => button.Invalidate();
            button.MouseEnter += RoundedCommandButton_MouseEnter;
            button.MouseLeave += RoundedCommandButton_MouseLeave;
            button.MouseDown += RoundedCommandButton_MouseDown;
            button.MouseUp += RoundedCommandButton_MouseUp;
        }

        button.Region?.Dispose();
        button.Region = null;
        button.Invalidate();
    }

    private void RoundedCommandButton_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button button)
            return;

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        e.Graphics.Clear(button.Parent?.BackColor ?? VsWindow);

        const float borderWidth = 2f;
        const float inset = borderWidth + 0.5f;
        var rect = new RectangleF(inset, inset, button.Width - inset * 2, button.Height - inset * 2);
        using var path = CreateRoundedPath(rect, rect.Height);
        using var fill = new SolidBrush(button.BackColor);
        using var border = new Pen(button.FlatAppearance.BorderColor, borderWidth);

        e.Graphics.FillPath(fill, path);
        e.Graphics.DrawPath(border, path);

        var textRect = ReferenceEquals(button, _btnBrowse)
            ? new Rectangle(0, 0, button.Width, button.Height)
            : new Rectangle(button.Padding.Left + 8, 0, button.Width - button.Padding.Left - 16, button.Height);
        var textFlags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
        textFlags |= ReferenceEquals(button, _btnBrowse)
            ? TextFormatFlags.HorizontalCenter
            : TextFormatFlags.Left;

        TextRenderer.DrawText(
            e.Graphics,
            button.Text,
            button.Font,
            textRect,
            button.ForeColor,
            textFlags);
    }

    private void RoundedCommandButton_MouseEnter(object? sender, EventArgs e)
    {
        if (sender is not Button button || !button.Enabled)
            return;

        button.BackColor = button.FlatAppearance.MouseOverBackColor;
        button.Invalidate();
    }

    private void RoundedCommandButton_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        RestoreRoundedButtonBackColor(button);
    }

    private void RoundedCommandButton_MouseDown(object? sender, MouseEventArgs e)
    {
        if (sender is not Button button || !button.Enabled || e.Button != MouseButtons.Left)
            return;

        button.BackColor = button.FlatAppearance.MouseDownBackColor;
        button.Invalidate();
    }

    private void RoundedCommandButton_MouseUp(object? sender, MouseEventArgs e)
    {
        if (sender is not Button button || !button.Enabled)
            return;

        button.BackColor = button.ClientRectangle.Contains(e.Location)
            ? button.FlatAppearance.MouseOverBackColor
            : _buttonNormalBackColors.GetValueOrDefault(button, button.BackColor);
        button.Invalidate();
    }

    private void RestoreRoundedButtonBackColor(Button button)
    {
        button.BackColor = _buttonNormalBackColors.GetValueOrDefault(button, button.BackColor);
        button.Invalidate();
    }

    private void GamePathPill_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        e.Graphics.Clear(BackColor);

        var rect = new RectangleF(2f, 2f, _gamePathPill.Width - 4f, _gamePathPill.Height - 4f);
        using var path = CreateRoundedPath(rect, rect.Height);
        using var fill = new SolidBrush(VsPanelAlt);
        using var border = new Pen(VsBorder, 2f);
        e.Graphics.FillPath(fill, path);
        e.Graphics.DrawPath(border, path);
    }

    private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedPath(RectangleF rect, float radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        var diameter = Math.Min(radius, Math.Min(rect.Width, rect.Height));
        if (diameter <= 1)
        {
            path.AddRectangle(rect);
            return path;
        }

        var arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));
        path.AddArc(arc, 180, 90);
        arc.X = rect.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = rect.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = rect.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void ShowProgress(int value)
    {
        _progressBar.Value = value;
    }

    private static Bitmap RenderHeaderIcon(Image source, Size targetSize)
    {
        var bitmap = new Bitmap(targetSize.Width, targetSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        var scale = Math.Min(targetSize.Width / (float)source.Width, targetSize.Height / (float)source.Height);
        var width = (int)Math.Round(source.Width * scale);
        var height = (int)Math.Round(source.Height * scale);
        var x = (targetSize.Width - width) / 2;
        var y = (targetSize.Height - height) / 2;
        graphics.DrawImage(source, new Rectangle(x, y, width, height));
        return bitmap;
    }

    private static string CompactPath(string path, int maxLength)
    {
        if (path.Length <= maxLength)
            return path;

        var root = Path.GetPathRoot(path) ?? "";
        var fileName = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (!string.IsNullOrEmpty(root) && !string.IsNullOrEmpty(fileName))
        {
            var compact = root + "..." + Path.DirectorySeparatorChar + fileName;
            if (compact.Length <= maxLength)
                return compact;
        }

        return "..." + path[^Math.Min(path.Length, maxLength - 3)..];
    }

    private void UpdateProgressStatus(string text, Color color)
    {
        _lblProgressStatus.Text = text;
        _lblProgressStatus.ForeColor = color;
    }

    private void SetBusyState(bool isBusy)
    {
        _btnInstall.Enabled = !isBusy;
        _btnReinstall.Enabled = !isBusy;
        _btnUninstall.Enabled = !isBusy;
        _btnBrowse.Enabled = !isBusy;
        _txtGamePath.Enabled = !isBusy;
        _btnLaunchGame.Enabled = !isBusy;
        _btnTos.Enabled = !isBusy;
    }
    private void Log(string message)
    {
        if (InvokeRequired) { Invoke(() => Log(message)); return; }
        _rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
        _rtbLog.ScrollToCaret();
    }
    private void PanelBorder_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel)
            return;

        using var pen = new Pen(VsBorder, 1f);
        e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
    }

    private void PnlLog_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel)
            return;

        using var pen = new Pen(VsBorder, 1f);
        e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
    }

    private static void ApplyDoubleBuffering(Control control)
    {
        control.GetType()
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(control, true, null);

        foreach (Control child in control.Controls)
            ApplyDoubleBuffering(child);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            if (!IsInDesigner())
                cp.ExStyle |= WS_EX_COMPOSITED;

            return cp;
        }
    }

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    private const int EM_SETMARGINS = 0xD3;
    private const int EC_LEFTMARGIN = 0x1;
    private const int WS_EX_COMPOSITED = 0x02000000;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ShowIcon = false;
        ApplyWindowsTitleBarTheme();
    }

    private void ApplyWindowsTitleBarTheme()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
        {
            var useDarkMode = 1;
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.UseImmersiveDarkMode, ref useDarkMode, sizeof(int));
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            var captionColor = ColorTranslator.ToWin32(VsWindow);
            var borderColor = ColorTranslator.ToWin32(VsBorder);
            var textColor = ColorTranslator.ToWin32(VsText);

            DwmSetWindowAttribute(Handle, DwmWindowAttribute.CaptionColor, ref captionColor, sizeof(int));
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.BorderColor, ref borderColor, sizeof(int));
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.TextColor, ref textColor, sizeof(int));
        }
    }

    private enum DwmWindowAttribute
    {
        UseImmersiveDarkMode = 20,
        BorderColor = 34,
        CaptionColor = 35,
        TextColor = 36
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        DwmWindowAttribute attribute,
        ref int attributeValue,
        int attributeSize);
}

