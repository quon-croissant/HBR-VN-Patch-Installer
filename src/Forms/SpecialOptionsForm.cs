using System.Runtime.InteropServices;

public sealed class SpecialOptionsForm : Form
{
    private static readonly Color VsWindow = Color.FromArgb(43, 39, 52);
    private static readonly Color VsPanel = Color.FromArgb(55, 49, 66);
    private static readonly Color VsPanelAlt = Color.FromArgb(68, 57, 82);
    private static readonly Color VsBorder = Color.FromArgb(178, 158, 190);
    private static readonly Color VsAccent = Color.FromArgb(214, 81, 151);
    private static readonly Color VsAccentHover = Color.FromArgb(247, 111, 183);
    private static readonly Color VsText = Color.FromArgb(250, 245, 252);
    private static readonly Color VsSubtleText = Color.FromArgb(211, 174, 187);

    private readonly PatchManager _patchManager;
    private readonly string _gamePath;
    private readonly CheckBox _borderless = new();
    private readonly CheckBox _collaborationEvents = new();
    private readonly ComboBox _resolution = new();
    private readonly Panel _displayPanel = new();
    private readonly Panel _experimentalPanel = new();
    private readonly Button _save = new();

    public SpecialOptionsForm(PatchManager patchManager, string gamePath)
    {
        _patchManager = patchManager;
        _gamePath = gamePath;

        Text = "Tuỳ chỉnh đặc biệt";
        ClientSize = new Size(470, 420);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowIcon = false;
        StartPosition = FormStartPosition.CenterParent;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = VsWindow;
        ForeColor = VsText;
        Font = new Font("Bahnschrift", 9.75F);

        _displayPanel.BackColor = VsPanel;
        _displayPanel.Location = new Point(18, 18);
        _displayPanel.Size = new Size(434, 160);
        _displayPanel.Paint += Panel_Paint;

        var title = new Label
        {
            AutoSize = true,
            Font = new Font("Bahnschrift", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(214, 81, 151),
            Location = new Point(14, 12),
            Text = "HIỂN THỊ"
        };
        _borderless.AutoSize = true;
        _borderless.BackColor = VsPanel;
        _borderless.ForeColor = VsText;
        _borderless.Location = new Point(16, 42);
        _borderless.Text = "Borderless fullscreen";
        _borderless.UseVisualStyleBackColor = false;
        _borderless.Checked = _patchManager.GetBorderlessWindowEnabled(_gamePath);

        var resolutionLabel = new Label
        {
            AutoSize = true,
            ForeColor = VsSubtleText,
            Location = new Point(16, 79),
            Text = "Độ phân giải:"
        };
        _resolution.BackColor = VsPanelAlt;
        _resolution.ForeColor = VsText;
        _resolution.DropDownStyle = ComboBoxStyle.DropDownList;
        _resolution.FlatStyle = FlatStyle.Flat;
        _resolution.Location = new Point(118, 75);
        _resolution.Size = new Size(298, 25);
        LoadResolutions();
        _resolution.Enabled = _borderless.Checked;
        _borderless.CheckedChanged += (_, _) => _resolution.Enabled = _borderless.Checked;

        var description = new Label
        {
            AutoSize = false,
            ForeColor = VsSubtleText,
            Location = new Point(16, 112),
            Size = new Size(400, 38),
            Text = "Bao gồm các preset 21:9 và chế độ màn hình Windows hỗ trợ.\nThay đổi có hiệu lực ở lần khởi động game tiếp theo."
        };

        _experimentalPanel.BackColor = VsPanel;
        _experimentalPanel.Location = new Point(18, 190);
        _experimentalPanel.Size = new Size(434, 160);
        _experimentalPanel.Paint += Panel_Paint;

        var experimentalTitle = new Label
        {
            AutoSize = true,
            Font = new Font("Bahnschrift", 11F, FontStyle.Bold),
            ForeColor = VsAccent,
            Location = new Point(14, 12),
            Text = "NỘI DUNG THỬ NGHIỆM"
        };
        _collaborationEvents.AutoSize = true;
        _collaborationEvents.BackColor = VsPanel;
        _collaborationEvents.ForeColor = VsText;
        _collaborationEvents.Location = new Point(16, 42);
        _collaborationEvents.Text = "Mở lại Collaboration Event đã kết thúc";
        _collaborationEvents.UseVisualStyleBackColor = false;
        _collaborationEvents.Checked = _patchManager.GetArchivedCollaborationEventsEnabled(_gamePath);

        var warning = new Label
        {
            AutoSize = false,
            ForeColor = Color.FromArgb(255, 190, 205),
            Location = new Point(16, 72),
            Size = new Size(400, 72),
            Text = "LƯU Ý: Tính năng này chỉ nhằm truy cập phần cốt truyện của event cũ. " +
                   "Nhiệm vụ, cửa hàng và Prism Event sẽ bị khóa hoặc ẩn. " +
                   "Thay đổi có hiệu lực ở lần khởi động game tiếp theo."
        };
        _save.BackColor = VsAccent;
        _save.FlatStyle = FlatStyle.Flat;
        _save.ForeColor = Color.White;
        _save.Location = new Point(332, 370);
        _save.Size = new Size(120, 32);
        _save.Text = "Lưu";
        _save.FlatAppearance.BorderColor = Color.FromArgb(255, 183, 229);
        _save.FlatAppearance.MouseOverBackColor = VsAccentHover;
        _save.FlatAppearance.MouseDownBackColor = Color.FromArgb(169, 56, 103);
        _save.Click += Save_Click;

        _displayPanel.Controls.AddRange(new Control[] { title, _borderless, resolutionLabel, _resolution, description });
        _experimentalPanel.Controls.AddRange(new Control[] { experimentalTitle, _collaborationEvents, warning });
        Controls.AddRange(new Control[] { _displayPanel, _experimentalPanel, _save });
        AcceptButton = _save;
        ApplyRoundedSaveButton();
    }

    private void Save_Click(object? sender, EventArgs e)
    {
        try
        {
            var selected = (ResolutionOption)_resolution.SelectedItem!;
            _patchManager.SetBorderlessSettings(_gamePath, _borderless.Checked, selected.Width, selected.Height);
            _patchManager.SetArchivedCollaborationEventsEnabled(_gamePath, _collaborationEvents.Checked);
            MessageBox.Show("Đã lưu tuỳ chỉnh.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể lưu tuỳ chỉnh:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadResolutions()
    {
        var current = _patchManager.GetBorderlessResolution(_gamePath);
        var options = GetDisplayModes();
        var screen = Screen.PrimaryScreen?.Bounds ?? SystemInformation.VirtualScreen;
        options.Insert(0, new ResolutionOption(
            $"Tự động ({screen.Width} × {screen.Height})  •  {GetAspectRatioLabel(screen.Width, screen.Height)}",
            0,
            0));
        if (current.Width > 0 && current.Height > 0
            && current.Width <= screen.Width && current.Height <= screen.Height
            && options.All(option => option.Width != current.Width || option.Height != current.Height))
            options.Add(new ResolutionOption(
                $"{current.Width} × {current.Height}  •  {GetAspectRatioLabel(current.Width, current.Height)}",
                current.Width,
                current.Height));

        _resolution.Items.AddRange(options.Cast<object>().ToArray());
        _resolution.SelectedItem = options.FirstOrDefault(option =>
            option.Width == current.Width && option.Height == current.Height) ?? options[0];
    }

    private static List<ResolutionOption> GetDisplayModes()
    {
        var screen = Screen.PrimaryScreen?.Bounds ?? SystemInformation.VirtualScreen;
        var modes = new HashSet<(int Width, int Height)>();
        foreach (var preset in new[] { (1920, 810), (2560, 1080), (3440, 1440), (3840, 1600), (5120, 2160) })
            if (preset.Item1 <= screen.Width && preset.Item2 <= screen.Height)
                modes.Add(preset);

        for (var index = 0; ; index++)
        {
            var mode = new DevMode { dmSize = (short)Marshal.SizeOf<DevMode>() };
            if (!EnumDisplaySettings(null, index, ref mode))
                break;
            if (mode.dmPelsWidth >= 800 && mode.dmPelsHeight >= 600
                && mode.dmPelsWidth <= screen.Width && mode.dmPelsHeight <= screen.Height)
                modes.Add((mode.dmPelsWidth, mode.dmPelsHeight));
        }

        return modes
            .OrderByDescending(mode => mode.Width * mode.Height)
            .ThenByDescending(mode => mode.Width)
            .Select(mode => new ResolutionOption(
                $"{mode.Width} × {mode.Height}  •  {GetAspectRatioLabel(mode.Width, mode.Height)}",
                mode.Width,
                mode.Height))
            .ToList();
    }

    private static string GetAspectRatioLabel(int width, int height)
    {
        var ratio = (double)width / height;
        var presets = new[]
        {
            (Ratio: 64d / 27d, Label: "21:9"),
            (Ratio: 16d / 9d, Label: "16:9"),
            (Ratio: 16d / 10d, Label: "16:10"),
            (Ratio: 5d / 4d, Label: "5:4"),
            (Ratio: 4d / 3d, Label: "4:3")
        };
        var preset = presets.MinBy(item => Math.Abs(item.Ratio - ratio));
        if (Math.Abs(preset.Ratio - ratio) < 0.035)
            return preset.Label;

        var divisor = GreatestCommonDivisor(width, height);
        return $"{width / divisor}:{height / divisor}";
    }

    private static int GreatestCommonDivisor(int left, int right)
    {
        while (right != 0)
            (left, right) = (right, left % right);
        return left;
    }

    private void Panel_Paint(object? sender, PaintEventArgs e)
    {
        using var border = new Pen(VsBorder, 1f);
        if (sender is Panel panel)
            e.Graphics.DrawRectangle(border, 0, 0, panel.Width - 1, panel.Height - 1);
    }

    private void ApplyRoundedSaveButton()
    {
        _save.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(BackColor);
            var rect = new RectangleF(2.5f, 2.5f, _save.Width - 5f, _save.Height - 5f);
            using var path = CreateRoundedPath(rect, rect.Height);
            using var fill = new SolidBrush(_save.BackColor);
            using var border = new Pen(_save.FlatAppearance.BorderColor, 2f);
            e.Graphics.FillPath(fill, path);
            e.Graphics.DrawPath(border, path);
            TextRenderer.DrawText(e.Graphics, _save.Text, _save.Font, _save.ClientRectangle, _save.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        };
        _save.MouseEnter += (_, _) => { _save.BackColor = _save.FlatAppearance.MouseOverBackColor; _save.Invalidate(); };
        _save.MouseLeave += (_, _) => { _save.BackColor = VsAccent; _save.Invalidate(); };
        _save.MouseDown += (_, _) => { _save.BackColor = _save.FlatAppearance.MouseDownBackColor; _save.Invalidate(); };
        _save.MouseUp += (_, _) => { _save.BackColor = VsAccentHover; _save.Invalidate(); };
    }

    private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedPath(RectangleF rect, float radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        var diameter = Math.Min(radius, Math.Min(rect.Width, rect.Height));
        var arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
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

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ShowIcon = false;
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
        {
            var darkMode = 1;
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.UseImmersiveDarkMode, ref darkMode, sizeof(int));
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            var caption = ColorTranslator.ToWin32(VsWindow);
            var border = ColorTranslator.ToWin32(VsBorder);
            var text = ColorTranslator.ToWin32(VsText);
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.CaptionColor, ref caption, sizeof(int));
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.BorderColor, ref border, sizeof(int));
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.TextColor, ref text, sizeof(int));
        }
    }

    private sealed record ResolutionOption(string Label, int Width, int Height)
    {
        public override string ToString() => Label;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DevMode
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool EnumDisplaySettings(string? deviceName, int modeNum, ref DevMode devMode);

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
