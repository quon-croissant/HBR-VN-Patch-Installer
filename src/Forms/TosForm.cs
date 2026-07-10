using System.Diagnostics;
using System.Runtime.InteropServices;

public sealed partial class TosForm : Form
{
    private static readonly Color VsWindow = Color.FromArgb(43, 39, 52);
    private static readonly Color VsPanelAlt = Color.FromArgb(68, 57, 82);
    private static readonly Color VsBorder = Color.FromArgb(178, 158, 190);
    private static readonly Color VsAccent = Color.FromArgb(214, 81, 151);
    private static readonly Color VsAccentHover = Color.FromArgb(247, 111, 183);
    private static readonly Color VsAccentBorder = Color.FromArgb(255, 183, 229);
    private static readonly Color VsDisabled = Color.FromArgb(54, 47, 64);
    private static readonly Color VsText = Color.FromArgb(250, 245, 252);
    private static readonly Color VsSubtleText = Color.FromArgb(211, 174, 187);
    private static readonly Color VsEditor = Color.FromArgb(34, 30, 42);
    private Panel _termsBorder = null!;
    private PictureBox _picLogo = null!;
    private RichTextBox _txtTerms = null!;
    private Button _btnClose = null!;
    private RadioButton? _optAgree;
    private RadioButton? _optDisagree;
    private readonly bool _requireAcceptance;

    public bool AcceptedTerms { get; private set; }

    public TosForm(bool requireAcceptance = false)
    {
        _requireAcceptance = requireAcceptance;
        InitializeComponent();
        BuildUi();
        LoadLogo();
        WireEventHandlers();
    }

    private void InitializeComponent()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
    }

    private void BuildUi()
    {
        Text = "Terms of Service";
        ShowIcon = false;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = _requireAcceptance ? new Size(560, 600) : new Size(560, 560);
        BackColor = VsWindow;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);

        var title = new Label
        {
            AutoSize = false,
            Text = "Điều khoản sử dụng bản patch",
            Font = new Font("Bahnschrift", 13F, FontStyle.Bold, GraphicsUnit.Point, 163),
            ForeColor = VsText,
            Location = new Point(246, 24),
            Size = new Size(296, 28),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var subtitle = new Label
        {
            AutoSize = false,
            Text = "Hãy đọc các điều khoản bên dưới. Bạn phải chấp nhận các điều khoản dưới đây của chúng tôi trước khi tiếp tục.",
            Font = new Font("Bahnschrift SemiCondensed", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 163),
            ForeColor = VsSubtleText,
            Location = new Point(247, 54),
            Size = new Size(270, 50),
            TextAlign = ContentAlignment.MiddleLeft
        };

        _picLogo = new PictureBox
        {
            BackColor = Color.Transparent,
            Location = new Point(18, 14),
            Size = new Size(210, 92),
            SizeMode = PictureBoxSizeMode.Zoom,
            TabStop = false
        };
        _termsBorder = new Panel
        {
            BackColor = VsEditor,
            Location = new Point(18, 122),
            Size = _requireAcceptance ? new Size(524, 328) : new Size(524, 368)
        };
        _txtTerms = new RichTextBox
        {
            BackColor = VsEditor,
            BorderStyle = BorderStyle.None,
            ForeColor = Color.FromArgb(238, 226, 241),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 163),
            Location = new Point(8, 8),
            ReadOnly = true,
            ShortcutsEnabled = false,
            ScrollBars = RichTextBoxScrollBars.Vertical,
            Size = _requireAcceptance ? new Size(508, 312) : new Size(508, 352),
            TabStop = false
        };
        _termsBorder.Controls.Add(_txtTerms);

        if (_requireAcceptance)
            CreateAcceptanceOptions();

        _btnClose = new Button
        {
            BackColor = VsPanelAlt,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Bahnschrift", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163),
            ForeColor = VsText,
            Location = _requireAcceptance ? new Point(412, 550) : new Point(412, 510),
            Size = new Size(130, 32),
            Text = _requireAcceptance ? "Tiếp tục" : "Đóng",
            UseVisualStyleBackColor = false
        };
        _btnClose.Enabled = !_requireAcceptance;
        _btnClose.FlatAppearance.BorderColor = _requireAcceptance ? Color.FromArgb(92, 78, 101) : VsBorder;
        _btnClose.FlatAppearance.BorderSize = 0;
        _btnClose.FlatAppearance.MouseOverBackColor = _requireAcceptance ? VsAccentHover : Color.FromArgb(84, 68, 99);
        _btnClose.FlatAppearance.MouseDownBackColor = _requireAcceptance ? Color.FromArgb(169, 56, 103) : Color.FromArgb(48, 39, 59);
        Controls.Add(title);
        Controls.Add(subtitle);
        Controls.Add(_picLogo);
        Controls.Add(_termsBorder);
        if (_requireAcceptance)
        {
            Controls.Add(_optAgree!);
            Controls.Add(_optDisagree!);
        }
        Controls.Add(_btnClose);
    }

    private void WireEventHandlers()
    {
        _termsBorder.Paint += TermsBorder_Paint;
        ApplyRoundedCloseButton();
        _btnClose.Click += BtnClose_Click;
        if (_requireAcceptance)
        {
            _optAgree!.CheckedChanged += AcceptanceOption_CheckedChanged;
            _optDisagree!.CheckedChanged += AcceptanceOption_CheckedChanged;
            ApplyThemedRadio(_optAgree);
            ApplyThemedRadio(_optDisagree);
            UpdateContinueButtonState();
        }
        if (!IsInDesigner())
        {
            Shown += (_, _) =>
            {
                BeginInvoke(() =>
                {
                    LoadTermsText();
                    ApplyTermsTextFormatting();
                    _btnClose.Focus();
                });
            };
        }
    }

    private void CreateAcceptanceOptions()
    {
        _optAgree = new RadioButton
        {
            AutoSize = true,
            BackColor = VsWindow,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 163),
            ForeColor = VsText,
            Location = new Point(28, 468),
            Text = "Tôi chấp nhận điều khoản",
            UseVisualStyleBackColor = false
        };

        _optDisagree = new RadioButton
        {
            AutoSize = true,
            BackColor = VsWindow,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 163),
            ForeColor = VsSubtleText,
            Location = new Point(28, 494),
            Text = "Tôi không chấp nhận điều khoản",
            UseVisualStyleBackColor = false
        };
    }

    private void AcceptanceOption_CheckedChanged(object? sender, EventArgs e)
    {
        _optAgree?.Invalidate();
        _optDisagree?.Invalidate();
        UpdateContinueButtonState();
    }

    private void ApplyThemedRadio(RadioButton radio)
    {
        radio.Appearance = Appearance.Button;
        radio.FlatStyle = FlatStyle.Flat;
        radio.FlatAppearance.BorderSize = 0;
        radio.TextAlign = ContentAlignment.MiddleLeft;
        radio.Padding = new Padding(24, 0, 0, 0);
        radio.Paint += ThemedRadio_Paint;
    }

    private void ThemedRadio_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not RadioButton radio)
            return;

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.Clear(VsWindow);

        var circleRect = new Rectangle(1, (radio.Height - 14) / 2, 14, 14);
        using var border = new Pen(radio.Checked ? VsAccentBorder : VsBorder, 1.5f);
        using var fill = new SolidBrush(radio.Checked ? VsAccent : VsWindow);
        e.Graphics.FillEllipse(fill, circleRect);
        e.Graphics.DrawEllipse(border, circleRect);

        if (radio.Checked)
        {
            using var inner = new SolidBrush(Color.White);
            var innerRect = new Rectangle(circleRect.X + 4, circleRect.Y + 4, 6, 6);
            e.Graphics.FillEllipse(inner, innerRect);
        }

        var textRect = new Rectangle(24, 0, radio.Width - 24, radio.Height);
        TextRenderer.DrawText(
            e.Graphics,
            radio.Text,
            radio.Font,
            textRect,
            radio.ForeColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPadding);
    }

    private void UpdateContinueButtonState()
    {
        var canContinue = _optAgree?.Checked == true;
        _btnClose.Enabled = !_requireAcceptance || canContinue;
        _btnClose.BackColor = _requireAcceptance
            ? canContinue ? VsAccent : VsDisabled
            : VsPanelAlt;
        _btnClose.ForeColor = _requireAcceptance && !canContinue
            ? Color.FromArgb(132, 119, 145)
            : VsText;
        _btnClose.FlatAppearance.BorderColor = _requireAcceptance && !canContinue
            ? Color.FromArgb(92, 78, 101)
            : _requireAcceptance ? VsAccentBorder : VsBorder;
        _btnClose.Invalidate();
    }

    private void BtnClose_Click(object? sender, EventArgs e)
    {
        if (_requireAcceptance)
        {
            AcceptedTerms = _optAgree?.Checked == true;
            DialogResult = AcceptedTerms ? DialogResult.OK : DialogResult.Cancel;
        }

        Close();
    }

    private void TermsBorder_Paint(object? sender, PaintEventArgs e)
    {
        using var border = new Pen(VsBorder, 1f);
        e.Graphics.DrawRectangle(border, 0, 0, _termsBorder.Width - 1, _termsBorder.Height - 1);
    }

    private void ApplyRoundedCloseButton()
    {
        UpdateRoundedCloseButtonRegion();
        _btnClose.Paint += RoundedCloseButton_Paint;
        _btnClose.MouseEnter += (_, _) =>
        {
            if (!_btnClose.Enabled)
                return;

            _btnClose.BackColor = _btnClose.FlatAppearance.MouseOverBackColor;
            _btnClose.Invalidate();
        };
        _btnClose.MouseLeave += (_, _) =>
        {
            _btnClose.BackColor = _requireAcceptance && _optAgree?.Checked == true ? VsAccent : VsPanelAlt;
            _btnClose.Invalidate();
        };
        _btnClose.MouseDown += (_, _) =>
        {
            if (!_btnClose.Enabled)
                return;

            _btnClose.BackColor = _btnClose.FlatAppearance.MouseDownBackColor;
            _btnClose.Invalidate();
        };
        _btnClose.MouseUp += (_, _) =>
        {
            if (!_btnClose.Enabled)
                return;

            _btnClose.BackColor = _btnClose.ClientRectangle.Contains(_btnClose.PointToClient(Cursor.Position))
                ? _btnClose.FlatAppearance.MouseOverBackColor
                : _requireAcceptance && _optAgree?.Checked == true ? VsAccent : VsPanelAlt;
            _btnClose.Invalidate();
        };
        _btnClose.Resize += (_, _) =>
        {
            UpdateRoundedCloseButtonRegion();
            _btnClose.Invalidate();
        };
    }

    private void RoundedCloseButton_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Button button)
            return;

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.Clear(VsWindow);
        var borderWidth = _requireAcceptance && button.Enabled ? 2f : 1f;
        var inset = borderWidth + 0.5f;
        var rect = new RectangleF(inset, inset, button.Width - inset * 2, button.Height - inset * 2);
        using var path = CreateRoundedPath(rect, rect.Height);
        using var fill = new SolidBrush(button.BackColor);
        using var border = new Pen(button.FlatAppearance.BorderColor, borderWidth);

        e.Graphics.FillPath(fill, path);
        e.Graphics.DrawPath(border, path);
        TextRenderer.DrawText(
            e.Graphics,
            button.Text,
            button.Font,
            button.ClientRectangle,
            button.ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    private void UpdateRoundedCloseButtonRegion()
    {
        if (_btnClose.Width <= 0 || _btnClose.Height <= 0)
            return;

        using var path = CreateRoundedPath(
            new RectangleF(0, 0, _btnClose.Width, _btnClose.Height),
            _btnClose.Height);
        _btnClose.Region?.Dispose();
        _btnClose.Region = new Region(path);
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
        arc.X = rect.X;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void ApplyTermsTextFormatting()
    {
        if (_txtTerms.IsDisposed)
            return;

        ApplyTermsTextInset();
        _txtTerms.SelectAll();
        var format = new ParaFormat2
        {
            cbSize = Marshal.SizeOf<ParaFormat2>(),
            dwMask = PfmAlignment,
            wAlignment = PfaJustify,
            rgxTabs = new int[32]
        };
        SendMessage(_txtTerms.Handle, EmSetParaFormat, IntPtr.Zero, ref format);
        _txtTerms.SelectionStart = 0;
        _txtTerms.SelectionLength = 0;
    }

    private void LoadTermsText()
    {
        var regularFont = _txtTerms.Font;
        var boldFont = new Font(regularFont, FontStyle.Bold);

        _txtTerms.SuspendLayout();
        _txtTerms.Clear();
        foreach (var (text, isBold) in ParseBoldMarkup(TermsText))
        {
            if (string.IsNullOrEmpty(text))
                continue;

            _txtTerms.SelectionStart = _txtTerms.TextLength;
            _txtTerms.SelectionLength = 0;
            _txtTerms.SelectionFont = isBold ? boldFont : regularFont;
            _txtTerms.AppendText(text);
        }

        _txtTerms.SelectionStart = 0;
        _txtTerms.SelectionLength = 0;
        _txtTerms.ResumeLayout();
    }

    private static List<(string Text, bool IsBold)> ParseBoldMarkup(string source)
    {
        var chunks = new List<(string Text, bool IsBold)>();
        var current = new System.Text.StringBuilder();
        var boldDepth = 0;

        for (var i = 0; i < source.Length;)
        {
            if (source.AsSpan(i).StartsWith("<b>", StringComparison.OrdinalIgnoreCase))
            {
                FlushCurrentChunk();
                boldDepth++;
                i += 3;
                continue;
            }

            if (source.AsSpan(i).StartsWith("</b>", StringComparison.OrdinalIgnoreCase))
            {
                FlushCurrentChunk();
                if (boldDepth > 0)
                    boldDepth--;
                i += 4;
                continue;
            }

            current.Append(source[i]);
            i++;
        }

        FlushCurrentChunk();
        return chunks;

        void FlushCurrentChunk()
        {
            if (current.Length == 0)
                return;

            chunks.Add((current.ToString(), boldDepth > 0));
            current.Clear();
        }
    }

    private void ApplyTermsTextInset()
    {
        var rect = new RichEditRect
        {
            Left = 10,
            Top = 3,
            Right = Math.Max(1, _txtTerms.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 3),
            Bottom = _txtTerms.ClientSize.Height
        };
        SendMessage(_txtTerms.Handle, EmSetRect, IntPtr.Zero, ref rect);
    }

    private void LoadLogo()
    {
        using var logoStream = typeof(TosForm).Assembly.GetManifestResourceStream("HBR_EN_VNPatchInstaller.logo.png");
        if (logoStream != null)
            _picLogo.Image = Image.FromStream(logoStream);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (IsInDesigner())
            return;

        ShowIcon = false;
        LoadTermsText();
        ApplyTermsTextFormatting();
        ApplyWindowsTitleBarTheme();
        ApplyNativeScrollbarTheme();
    }

    private void ApplyNativeScrollbarTheme()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
            return;

        SetWindowTheme(_txtTerms.Handle, "DarkMode_Explorer", null);
        _txtTerms.Invalidate();
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

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hwnd, string? pszSubAppName, string? pszSubIdList);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref ParaFormat2 lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref RichEditRect lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct RichEditRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ParaFormat2
    {
        public int cbSize;
        public uint dwMask;
        public short wNumbering;
        public short wEffects;
        public int dxStartIndent;
        public int dxRightIndent;
        public int dxOffset;
        public short wAlignment;
        public short cTabCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] rgxTabs;
        public int dySpaceBefore;
        public int dySpaceAfter;
        public int dyLineSpacing;
        public short sStyle;
        public byte bLineSpacingRule;
        public byte bOutlineLevel;
        public short wShadingWeight;
        public short wShadingStyle;
        public short wNumberingStart;
        public short wNumberingStyle;
        public short wNumberingTab;
        public short wBorderSpace;
        public short wBorderWidth;
        public short wBorders;
    }

    private const int EmSetParaFormat = 0x447;
    private const int EmSetRect = 0xB3;
    private const uint PfmAlignment = 0x8;
    private const short PfaJustify = 4;

    private const string TermsText =
@"<b>1. CÁC GIỚI HẠN</b>

Bản dịch này được phát hành với thiện chí rằng nó sẽ không bị lạm dụng. Mọi hành vi liên quan đến thương mại hóa bản dịch đều bị nghiêm cấm. Và mọi sự chỉnh sửa hay làm sai lệch nội dung của bản dịch đều không được hoan nghênh.

Khi sử dụng bản dịch là bạn đồng ý không quảng bá khuyến khích sử dụng bản sao bất hợp pháp của trò chơi kèm với bản dịch hoặc bất kỳ phần nào trong đó. Chỉ mua và sử dụng phiên bản game hợp pháp theo chỉ định của nhà sản xuất.

Vietnam Key FanClub, đơn vị thực hiện bản dịch, chỉ cung cấp hỗ trợ về các lỗi phát sinh khi bản dịch được tải về từ trang web chính thức của đơn vị, tại địa chỉ: https://vnkeyfc.com/, hoặc từ các trang mạng xã hội chính thức của đơn vị như được liệt kê tại trang web chính thức. Những bản sao chép lại từ các nơi khác trên Internet sẽ không nhận được hỗ trợ hay giải đáp kỹ thuật từ chúng tôi.


<b>2. MIỄN TRỪ TRÁCH NHIỆM NỘI DUNG</b>

Bạn nhận thức và đồng ý việc sử dụng bản dịch này là do bạn tự chịu mọi rủi ro. Những dự án dịch của Vietnam Key FanClub hoàn toàn tự phát, và nội dung dịch hoàn toàn do các tình nguyện viên đóng góp, không qua các kiểm tra chọn lọc mang tính chuyên môn. Tuy đã cố gắng hết sức nhằm cố gắng truyền tải trọn vẹn nội dung và văn cảnh nhằm thể hiện sự trân trọng ở mức tối đa các tác phẩm, Vietnam Key FanClub không thể đảm bảo các văn bản dịch là hoàn toàn chính xác và đầy đủ. Xin hãy cân nhắc trước khi bạn quyết định sử dụng các nội dung dịch này.


<b>3. PHỦ NHẬN TRÁCH NHIỆM PHÁP LÝ</b>

Vietnam Key FanClub sẽ không chịu trách nhiệm với bạn về bất kỳ thiệt hại trực tiếp, gián tiếp, ngẫu nhiên, thiệt hại đặc biệt, thiệt hại là hậu quả của hành vi hoặc điều nào đó, hay thiệt hại mang tính làm gương, chịu trách nhiệm nhưng không giới hạn đối với những thiệt hại do mất mát lợi nhuận, ủy thác tín kinh doanh, quyền hoa lợi, dữ liệu hoặc những mất mát không xác thực khác, bất kể Chúng tôi đã thông báo về khả năng thiệt hại đó hay chưa. Trong trường hợp mà luật áp dụng không cho phép đưa ra hạn chế hay loại trừ đối với trách nhiệm pháp lý hay thiệt hại – dù đó là thiệt hại ngẫu nhiên hay là thiệt hại do một hậu quả tất yếu, khi đó, những hạn chế hoặc loại trừ trên có thể không áp dụng đối với bạn, mặc dù trách nhiệm của Chúng tôi sẽ được hạn chế đến mức tối đa mà quy định của luật pháp áp dụng cho phép.


<b>4. SỰ ĐỒNG Ý CỦA BẠN</b>

Với việc cài đặt bản dịch, bạn đã đồng ý với bản Điều khoản sử dụng này.";
}
