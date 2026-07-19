#nullable disable

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    // Header
    private Panel _pnlHeader;
    private PictureBox _picIcon;
    private Label _lblTitle;
    private Label _lblSubtitle;

    // Đường dẫn
    private TextBox _txtGamePath;
    private Button _btnBrowse;

    // Card: Thông tin game
    private Panel _pnlGameInfo;
    private Label _lblGameInfoTitle;
    private Label _lblGamePathKey;
    private TextBox _lblGamePathValue;
    private Label _lblGameVersionKey;
    private Label _lblGameVersionValue;
    private Label _lblBepInExKey;
    private Label _lblBepInExValue;

    // Card: Thông tin bản dịch
    private Panel _pnlPatchInfo;
    private Label _lblPatchInfoTitle;
    private Label _lblInstalledKey;
    private Label _lblInstalledVersion;
    private Label _lblLatestKey;
    private Label _lblLatestVersion;
    private Label _lblPatchStatusKey;
    private Label _lblPatchStatus;

    // Card: Thao tác
    private Panel _pnlOps;
    private Label _lblOpsTitle;
    private Button _btnInstall;
    private Button _btnReinstall;
    private Button _btnUninstall;

    // Card: Tùy chọn
    private Panel _pnlOptions;
    private Label _lblOptionsTitle;
    private Button _btnLaunchGame;
    private Button _btnTos;

    // Progress
    private Label _lblProgressStatusTitle;
    private Label _lblProgressStatus;
    private CustomProgressBar _progressBar;

    // Log
    private Panel _pnlLog;
    private RichTextBox _rtbLog;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        _pnlHeader = new Panel();
        _picIcon = new PictureBox();
        _lblTitle = new Label();
        _lblSubtitle = new Label();
        _txtGamePath = new TextBox();
        _btnBrowse = new Button();
        _pnlGameInfo = new Panel();
        _lblGameInfoTitle = new Label();
        _lblGamePathKey = new Label();
        _lblGamePathValue = new TextBox();
        _lblGameVersionKey = new Label();
        _lblGameVersionValue = new Label();
        _lblBepInExKey = new Label();
        _lblBepInExValue = new Label();
        _pnlPatchInfo = new Panel();
        _lblPatchInfoTitle = new Label();
        _lblInstalledKey = new Label();
        _lblInstalledVersion = new Label();
        _lblLatestKey = new Label();
        _lblLatestVersion = new Label();
        _lblPatchStatusKey = new Label();
        _lblPatchStatus = new Label();
        _pnlOps = new Panel();
        _lblOpsTitle = new Label();
        _btnInstall = new Button();
        _btnReinstall = new Button();
        _btnUninstall = new Button();
        _pnlOptions = new Panel();
        _lblOptionsTitle = new Label();
        _btnLaunchGame = new Button();
        _btnTos = new Button();
        _lblProgressStatusTitle = new Label();
        _lblProgressStatus = new Label();
        _progressBar = new CustomProgressBar();
        _pnlLog = new Panel();
        _rtbLog = new RichTextBox();
        _pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_picIcon).BeginInit();
        _pnlGameInfo.SuspendLayout();
        _pnlPatchInfo.SuspendLayout();
        _pnlOps.SuspendLayout();
        _pnlOptions.SuspendLayout();
        _pnlLog.SuspendLayout();
        SuspendLayout();
        // 
        // _pnlHeader
        // 
        _pnlHeader.BackColor = Color.FromArgb(55, 49, 66);
        _pnlHeader.Controls.Add(_picIcon);
        _pnlHeader.Controls.Add(_lblTitle);
        _pnlHeader.Controls.Add(_lblSubtitle);
        _pnlHeader.Location = new Point(19, 19);
        _pnlHeader.Name = "_pnlHeader";
        _pnlHeader.Size = new Size(636, 60);
        _pnlHeader.TabIndex = 0;
        // 
        // _picIcon
        // 
        _picIcon.BackColor = Color.Transparent;
        _picIcon.Location = new Point(10, 8);
        _picIcon.Name = "_picIcon";
        _picIcon.Size = new Size(44, 44);
        _picIcon.SizeMode = PictureBoxSizeMode.Zoom;
        _picIcon.TabIndex = 0;
        _picIcon.TabStop = false;
        // 
        // _lblTitle
        // 
        _lblTitle.AutoSize = true;
        _lblTitle.BackColor = Color.Transparent;
        _lblTitle.Font = new Font("A-OTF Soft Gothic Std DB", 12.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
        _lblTitle.ForeColor = Color.FromArgb(250, 245, 252);
        _lblTitle.Location = new Point(65, 10);
        _lblTitle.Name = "_lblTitle";
        _lblTitle.Size = new Size(456, 21);
        _lblTitle.TabIndex = 1;
        _lblTitle.Text = "HEAVEN BURNS RED GLOBAL - VIETNAMESE PATCH";
        // 
        // _lblSubtitle
        // 
        _lblSubtitle.AutoSize = true;
        _lblSubtitle.BackColor = Color.Transparent;
        _lblSubtitle.Font = new Font("Bahnschrift", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 163);
        _lblSubtitle.ForeColor = Color.FromArgb(211, 174, 187);
        _lblSubtitle.Location = new Point(66, 34);
        _lblSubtitle.Name = "_lblSubtitle";
        _lblSubtitle.Size = new Size(358, 18);
        _lblSubtitle.TabIndex = 2;
        _lblSubtitle.Text = "Trình cài đặt bản vá tiếng Việt cho Heaven Burns Red";
        // 
        // _txtGamePath
        // 
        _txtGamePath.BackColor = Color.FromArgb(68, 57, 82);
        _txtGamePath.BorderStyle = BorderStyle.FixedSingle;
        _txtGamePath.Font = new Font("Bahnschrift", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 163);
        _txtGamePath.ForeColor = Color.FromArgb(250, 245, 252);
        _txtGamePath.Location = new Point(19, 90);
        _txtGamePath.Name = "_txtGamePath";
        _txtGamePath.PlaceholderText = "Chưa chọn thư mục...";
        _txtGamePath.Size = new Size(535, 29);
        _txtGamePath.TabIndex = 1;
        // 
        // _btnBrowse
        // 
        _btnBrowse.BackColor = Color.FromArgb(68, 57, 82);
        _btnBrowse.FlatAppearance.BorderColor = Color.FromArgb(178, 158, 190);
        _btnBrowse.FlatAppearance.MouseDownBackColor = Color.FromArgb(48, 39, 59);
        _btnBrowse.FlatAppearance.MouseOverBackColor = Color.FromArgb(84, 68, 99);
        _btnBrowse.FlatStyle = FlatStyle.Flat;
        _btnBrowse.Font = new Font("Bahnschrift", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        _btnBrowse.ForeColor = Color.FromArgb(250, 245, 252);
        _btnBrowse.Location = new Point(560, 90);
        _btnBrowse.Name = "_btnBrowse";
        _btnBrowse.Size = new Size(95, 29);
        _btnBrowse.TabIndex = 2;
        _btnBrowse.Text = "Duyệt...";
        _btnBrowse.UseVisualStyleBackColor = false;
        _btnBrowse.Click += BtnBrowse_Click;
        // 
        // _pnlGameInfo
        // 
        _pnlGameInfo.BackColor = Color.FromArgb(55, 49, 66);
        _pnlGameInfo.Controls.Add(_lblGameInfoTitle);
        _pnlGameInfo.Controls.Add(_lblGamePathKey);
        _pnlGameInfo.Controls.Add(_lblGamePathValue);
        _pnlGameInfo.Controls.Add(_lblGameVersionKey);
        _pnlGameInfo.Controls.Add(_lblGameVersionValue);
        _pnlGameInfo.Controls.Add(_lblBepInExKey);
        _pnlGameInfo.Controls.Add(_lblBepInExValue);
        _pnlGameInfo.Location = new Point(19, 131);
        _pnlGameInfo.Name = "_pnlGameInfo";
        _pnlGameInfo.Size = new Size(312, 110);
        _pnlGameInfo.TabIndex = 3;
        // 
        // _lblGameInfoTitle
        // 
        _lblGameInfoTitle.AutoSize = true;
        _lblGameInfoTitle.BackColor = Color.Transparent;
        _lblGameInfoTitle.Font = new Font("Bahnschrift", 8.25F, FontStyle.Bold);
        _lblGameInfoTitle.ForeColor = Color.FromArgb(214, 81, 151);
        _lblGameInfoTitle.Location = new Point(10, 10);
        _lblGameInfoTitle.Name = "_lblGameInfoTitle";
        _lblGameInfoTitle.Size = new Size(93, 13);
        _lblGameInfoTitle.TabIndex = 0;
        _lblGameInfoTitle.Text = "THÔNG TIN GAME";
        // 
        // _lblGamePathKey
        // 
        _lblGamePathKey.AutoSize = true;
        _lblGamePathKey.BackColor = Color.Transparent;
        _lblGamePathKey.Font = new Font("Bahnschrift", 9F);
        _lblGamePathKey.ForeColor = Color.FromArgb(211, 174, 187);
        _lblGamePathKey.Location = new Point(10, 34);
        _lblGamePathKey.Name = "_lblGamePathKey";
        _lblGamePathKey.Size = new Size(67, 14);
        _lblGamePathKey.TabIndex = 1;
        _lblGamePathKey.Text = "Đường dẫn:";
        // 
        // _lblGamePathValue
        // 
        _lblGamePathValue.BackColor = Color.FromArgb(55, 49, 66);
        _lblGamePathValue.BorderStyle = BorderStyle.None;
        _lblGamePathValue.Font = new Font("Bahnschrift SemiCondensed", 9F, FontStyle.Bold);
        _lblGamePathValue.ForeColor = Color.FromArgb(250, 245, 252);
        _lblGamePathValue.Location = new Point(87, 33);
        _lblGamePathValue.Name = "_lblGamePathValue";
        _lblGamePathValue.ReadOnly = true;
        _lblGamePathValue.Size = new Size(213, 15);
        _lblGamePathValue.TabIndex = 2;
        _lblGamePathValue.Text = "—";
        // 
        // _lblGameVersionKey
        // 
        _lblGameVersionKey.AutoSize = true;
        _lblGameVersionKey.BackColor = Color.Transparent;
        _lblGameVersionKey.Font = new Font("Bahnschrift", 9F);
        _lblGameVersionKey.ForeColor = Color.FromArgb(211, 174, 187);
        _lblGameVersionKey.Location = new Point(10, 60);
        _lblGameVersionKey.Name = "_lblGameVersionKey";
        _lblGameVersionKey.Size = new Size(63, 14);
        _lblGameVersionKey.TabIndex = 3;
        _lblGameVersionKey.Text = "Phiên bản:";
        // 
        // _lblGameVersionValue
        // 
        _lblGameVersionValue.AutoSize = true;
        _lblGameVersionValue.BackColor = Color.Transparent;
        _lblGameVersionValue.Font = new Font("Bahnschrift SemiCondensed", 9F, FontStyle.Bold);
        _lblGameVersionValue.ForeColor = Color.FromArgb(250, 245, 252);
        _lblGameVersionValue.Location = new Point(84, 60);
        _lblGameVersionValue.Name = "_lblGameVersionValue";
        _lblGameVersionValue.Size = new Size(16, 14);
        _lblGameVersionValue.TabIndex = 4;
        _lblGameVersionValue.Text = "—";
        // 
        // _lblBepInExKey
        // 
        _lblBepInExKey.AutoSize = true;
        _lblBepInExKey.BackColor = Color.Transparent;
        _lblBepInExKey.Font = new Font("Bahnschrift", 9F);
        _lblBepInExKey.ForeColor = Color.FromArgb(211, 174, 187);
        _lblBepInExKey.Location = new Point(10, 86);
        _lblBepInExKey.Name = "_lblBepInExKey";
        _lblBepInExKey.Size = new Size(54, 14);
        _lblBepInExKey.TabIndex = 5;
        _lblBepInExKey.Text = "BepInEx:";
        // 
        // _lblBepInExValue
        // 
        _lblBepInExValue.AutoSize = true;
        _lblBepInExValue.BackColor = Color.Transparent;
        _lblBepInExValue.Font = new Font("Bahnschrift SemiCondensed", 9F, FontStyle.Bold);
        _lblBepInExValue.Location = new Point(84, 86);
        _lblBepInExValue.Name = "_lblBepInExValue";
        _lblBepInExValue.Size = new Size(16, 14);
        _lblBepInExValue.TabIndex = 6;
        _lblBepInExValue.Text = "—";
        // 
        // _pnlPatchInfo
        // 
        _pnlPatchInfo.BackColor = Color.FromArgb(55, 49, 66);
        _pnlPatchInfo.Controls.Add(_lblPatchInfoTitle);
        _pnlPatchInfo.Controls.Add(_lblInstalledKey);
        _pnlPatchInfo.Controls.Add(_lblInstalledVersion);
        _pnlPatchInfo.Controls.Add(_lblLatestKey);
        _pnlPatchInfo.Controls.Add(_lblLatestVersion);
        _pnlPatchInfo.Controls.Add(_lblPatchStatusKey);
        _pnlPatchInfo.Controls.Add(_lblPatchStatus);
        _pnlPatchInfo.Location = new Point(343, 131);
        _pnlPatchInfo.Name = "_pnlPatchInfo";
        _pnlPatchInfo.Size = new Size(312, 110);
        _pnlPatchInfo.TabIndex = 4;
        // 
        // _lblPatchInfoTitle
        // 
        _lblPatchInfoTitle.AutoSize = true;
        _lblPatchInfoTitle.BackColor = Color.Transparent;
        _lblPatchInfoTitle.Font = new Font("Bahnschrift", 8.25F, FontStyle.Bold);
        _lblPatchInfoTitle.ForeColor = Color.FromArgb(214, 81, 151);
        _lblPatchInfoTitle.Location = new Point(10, 10);
        _lblPatchInfoTitle.Name = "_lblPatchInfoTitle";
        _lblPatchInfoTitle.Size = new Size(112, 13);
        _lblPatchInfoTitle.TabIndex = 0;
        _lblPatchInfoTitle.Text = "THÔNG TIN BẢN DỊCH";
        // 
        // _lblInstalledKey
        // 
        _lblInstalledKey.AutoSize = true;
        _lblInstalledKey.BackColor = Color.Transparent;
        _lblInstalledKey.Font = new Font("Bahnschrift", 9F);
        _lblInstalledKey.ForeColor = Color.FromArgb(211, 174, 187);
        _lblInstalledKey.Location = new Point(10, 34);
        _lblInstalledKey.Name = "_lblInstalledKey";
        _lblInstalledKey.Size = new Size(61, 14);
        _lblInstalledKey.TabIndex = 1;
        _lblInstalledKey.Text = "Đã cài đặt:";
        // 
        // _lblInstalledVersion
        // 
        _lblInstalledVersion.AutoSize = true;
        _lblInstalledVersion.BackColor = Color.Transparent;
        _lblInstalledVersion.Font = new Font("Bahnschrift SemiCondensed", 9F, FontStyle.Bold);
        _lblInstalledVersion.ForeColor = Color.FromArgb(250, 245, 252);
        _lblInstalledVersion.Location = new Point(84, 34);
        _lblInstalledVersion.Name = "_lblInstalledVersion";
        _lblInstalledVersion.Size = new Size(16, 14);
        _lblInstalledVersion.TabIndex = 2;
        _lblInstalledVersion.Text = "—";
        // 
        // _lblLatestKey
        // 
        _lblLatestKey.AutoSize = true;
        _lblLatestKey.BackColor = Color.Transparent;
        _lblLatestKey.Font = new Font("Bahnschrift", 9F);
        _lblLatestKey.ForeColor = Color.FromArgb(211, 174, 187);
        _lblLatestKey.Location = new Point(10, 60);
        _lblLatestKey.Name = "_lblLatestKey";
        _lblLatestKey.Size = new Size(56, 14);
        _lblLatestKey.TabIndex = 3;
        _lblLatestKey.Text = "Mới nhất:";
        // 
        // _lblLatestVersion
        // 
        _lblLatestVersion.AutoSize = true;
        _lblLatestVersion.BackColor = Color.Transparent;
        _lblLatestVersion.Font = new Font("Bahnschrift SemiCondensed", 9F, FontStyle.Bold);
        _lblLatestVersion.ForeColor = Color.FromArgb(250, 245, 252);
        _lblLatestVersion.Location = new Point(84, 60);
        _lblLatestVersion.Name = "_lblLatestVersion";
        _lblLatestVersion.Size = new Size(16, 14);
        _lblLatestVersion.TabIndex = 4;
        _lblLatestVersion.Text = "—";
        // 
        // _lblPatchStatusKey
        // 
        _lblPatchStatusKey.AutoSize = true;
        _lblPatchStatusKey.BackColor = Color.Transparent;
        _lblPatchStatusKey.Font = new Font("Bahnschrift", 9F);
        _lblPatchStatusKey.ForeColor = Color.FromArgb(211, 174, 187);
        _lblPatchStatusKey.Location = new Point(10, 86);
        _lblPatchStatusKey.Name = "_lblPatchStatusKey";
        _lblPatchStatusKey.Size = new Size(63, 14);
        _lblPatchStatusKey.TabIndex = 5;
        _lblPatchStatusKey.Text = "Trạng thái:";
        // 
        // _lblPatchStatus
        // 
        _lblPatchStatus.AutoSize = true;
        _lblPatchStatus.BackColor = Color.Transparent;
        _lblPatchStatus.Font = new Font("Bahnschrift SemiCondensed", 9F, FontStyle.Bold);
        _lblPatchStatus.Location = new Point(84, 86);
        _lblPatchStatus.Name = "_lblPatchStatus";
        _lblPatchStatus.Size = new Size(16, 14);
        _lblPatchStatus.TabIndex = 6;
        _lblPatchStatus.Text = "—";
        // 
        // _pnlOps
        // 
        _pnlOps.BackColor = Color.FromArgb(55, 49, 66);
        _pnlOps.Controls.Add(_lblOpsTitle);
        _pnlOps.Controls.Add(_btnInstall);
        _pnlOps.Controls.Add(_btnReinstall);
        _pnlOps.Controls.Add(_btnUninstall);
        _pnlOps.Location = new Point(19, 253);
        _pnlOps.Name = "_pnlOps";
        _pnlOps.Size = new Size(312, 150);
        _pnlOps.TabIndex = 5;
        // 
        // _lblOpsTitle
        // 
        _lblOpsTitle.AutoSize = true;
        _lblOpsTitle.BackColor = Color.Transparent;
        _lblOpsTitle.Font = new Font("Bahnschrift", 8.25F, FontStyle.Bold);
        _lblOpsTitle.ForeColor = Color.FromArgb(214, 81, 151);
        _lblOpsTitle.Location = new Point(10, 10);
        _lblOpsTitle.Name = "_lblOpsTitle";
        _lblOpsTitle.Size = new Size(55, 13);
        _lblOpsTitle.TabIndex = 0;
        _lblOpsTitle.Text = "THAO TÁC";
        // 
        // _btnInstall
        // 
        _btnInstall.BackColor = Color.FromArgb(214, 81, 151);
        _btnInstall.Cursor = Cursors.Hand;
        _btnInstall.FlatAppearance.BorderColor = Color.FromArgb(247, 111, 183);
        _btnInstall.FlatAppearance.MouseDownBackColor = Color.FromArgb(169, 56, 103);
        _btnInstall.FlatAppearance.MouseOverBackColor = Color.FromArgb(247, 111, 183);
        _btnInstall.FlatStyle = FlatStyle.Flat;
        _btnInstall.Font = new Font("Bahnschrift", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        _btnInstall.ForeColor = Color.White;
        _btnInstall.Location = new Point(10, 30);
        _btnInstall.Name = "_btnInstall";
        _btnInstall.Padding = new Padding(6, 0, 0, 0);
        _btnInstall.Size = new Size(290, 32);
        _btnInstall.TabIndex = 1;
        _btnInstall.Text = "Cài đặt";
        _btnInstall.TextAlign = ContentAlignment.MiddleLeft;
        _btnInstall.UseVisualStyleBackColor = false;
        _btnInstall.Click += BtnInstall_Click;
        // 
        // _btnReinstall
        // 
        _btnReinstall.BackColor = Color.FromArgb(68, 57, 82);
        _btnReinstall.Cursor = Cursors.Hand;
        _btnReinstall.FlatAppearance.BorderColor = Color.FromArgb(178, 158, 190);
        _btnReinstall.FlatAppearance.MouseDownBackColor = Color.FromArgb(48, 39, 59);
        _btnReinstall.FlatAppearance.MouseOverBackColor = Color.FromArgb(84, 68, 99);
        _btnReinstall.FlatStyle = FlatStyle.Flat;
        _btnReinstall.Font = new Font("Bahnschrift", 9.75F);
        _btnReinstall.ForeColor = Color.FromArgb(250, 245, 252);
        _btnReinstall.Location = new Point(10, 68);
        _btnReinstall.Name = "_btnReinstall";
        _btnReinstall.Padding = new Padding(6, 0, 0, 0);
        _btnReinstall.Size = new Size(290, 32);
        _btnReinstall.TabIndex = 2;
        _btnReinstall.Text = "Cập nhật";
        _btnReinstall.TextAlign = ContentAlignment.MiddleLeft;
        _btnReinstall.UseVisualStyleBackColor = false;
        _btnReinstall.Click += BtnReinstall_Click;
        // 
        // _btnUninstall
        // 
        _btnUninstall.BackColor = Color.FromArgb(95, 38, 52);
        _btnUninstall.Cursor = Cursors.Hand;
        _btnUninstall.FlatAppearance.BorderColor = Color.FromArgb(255, 118, 137);
        _btnUninstall.FlatAppearance.MouseDownBackColor = Color.FromArgb(73, 29, 40);
        _btnUninstall.FlatAppearance.MouseOverBackColor = Color.FromArgb(123, 48, 65);
        _btnUninstall.FlatStyle = FlatStyle.Flat;
        _btnUninstall.Font = new Font("Bahnschrift", 9.75F);
        _btnUninstall.ForeColor = Color.FromArgb(255, 214, 221);
        _btnUninstall.Location = new Point(10, 106);
        _btnUninstall.Name = "_btnUninstall";
        _btnUninstall.Padding = new Padding(6, 0, 0, 0);
        _btnUninstall.Size = new Size(290, 32);
        _btnUninstall.TabIndex = 3;
        _btnUninstall.Text = "Gỡ cài đặt";
        _btnUninstall.TextAlign = ContentAlignment.MiddleLeft;
        _btnUninstall.UseVisualStyleBackColor = false;
        _btnUninstall.Click += BtnUninstall_Click;
        // 
        // _pnlOptions
        // 
        _pnlOptions.BackColor = Color.FromArgb(55, 49, 66);
        _pnlOptions.Controls.Add(_lblOptionsTitle);
        _pnlOptions.Controls.Add(_btnLaunchGame);
        _pnlOptions.Controls.Add(_btnTos);
        _pnlOptions.Location = new Point(343, 253);
        _pnlOptions.Name = "_pnlOptions";
        _pnlOptions.Size = new Size(312, 150);
        _pnlOptions.TabIndex = 6;
        // 
        // _lblOptionsTitle
        // 
        _lblOptionsTitle.AutoSize = true;
        _lblOptionsTitle.BackColor = Color.Transparent;
        _lblOptionsTitle.Font = new Font("Bahnschrift", 8.25F, FontStyle.Bold);
        _lblOptionsTitle.ForeColor = Color.FromArgb(214, 81, 151);
        _lblOptionsTitle.Location = new Point(10, 10);
        _lblOptionsTitle.Name = "_lblOptionsTitle";
        _lblOptionsTitle.Size = new Size(57, 13);
        _lblOptionsTitle.TabIndex = 0;
        _lblOptionsTitle.Text = "TÙY CHỌN";
        // 
        // _btnLaunchGame
        // 
        _btnLaunchGame.BackColor = Color.FromArgb(80, 61, 95);
        _btnLaunchGame.Cursor = Cursors.Hand;
        _btnLaunchGame.FlatAppearance.BorderColor = Color.FromArgb(232, 186, 244);
        _btnLaunchGame.FlatAppearance.MouseDownBackColor = Color.FromArgb(54, 42, 67);
        _btnLaunchGame.FlatAppearance.MouseOverBackColor = Color.FromArgb(102, 72, 116);
        _btnLaunchGame.FlatStyle = FlatStyle.Flat;
        _btnLaunchGame.Font = new Font("Bahnschrift", 9.75F);
        _btnLaunchGame.ForeColor = Color.FromArgb(255, 241, 248);
        _btnLaunchGame.Location = new Point(10, 30);
        _btnLaunchGame.Name = "_btnLaunchGame";
        _btnLaunchGame.Padding = new Padding(6, 0, 0, 0);
        _btnLaunchGame.Size = new Size(290, 32);
        _btnLaunchGame.TabIndex = 2;
        _btnLaunchGame.Text = "Khởi động game";
        _btnLaunchGame.TextAlign = ContentAlignment.MiddleLeft;
        _btnLaunchGame.UseVisualStyleBackColor = false;
        _btnLaunchGame.Click += BtnLaunchGame_Click;
        // 
        // _btnTos
        // 
        _btnTos.BackColor = Color.FromArgb(68, 57, 82);
        _btnTos.Cursor = Cursors.Hand;
        _btnTos.FlatAppearance.BorderColor = Color.FromArgb(178, 158, 190);
        _btnTos.FlatAppearance.MouseDownBackColor = Color.FromArgb(48, 39, 59);
        _btnTos.FlatAppearance.MouseOverBackColor = Color.FromArgb(84, 68, 99);
        _btnTos.FlatStyle = FlatStyle.Flat;
        _btnTos.Font = new Font("Bahnschrift", 9.75F);
        _btnTos.ForeColor = Color.FromArgb(250, 245, 252);
        _btnTos.Location = new Point(10, 68);
        _btnTos.Name = "_btnTos";
        _btnTos.Padding = new Padding(6, 0, 0, 0);
        _btnTos.Size = new Size(290, 32);
        _btnTos.TabIndex = 3;
        _btnTos.Text = "Điều khoản sử dụng";
        _btnTos.TextAlign = ContentAlignment.MiddleLeft;
        _btnTos.UseVisualStyleBackColor = false;
        _btnTos.Click += BtnTos_Click;
        // 
        // _lblProgressStatusTitle
        // 
        _lblProgressStatusTitle.AutoSize = true;
        _lblProgressStatusTitle.BackColor = Color.Transparent;
        _lblProgressStatusTitle.Font = new Font("Bahnschrift", 9F, FontStyle.Bold, GraphicsUnit.Point, 163);
        _lblProgressStatusTitle.ForeColor = Color.FromArgb(211, 174, 187);
        _lblProgressStatusTitle.Location = new Point(19, 572);
        _lblProgressStatusTitle.Name = "_lblProgressStatusTitle";
        _lblProgressStatusTitle.Size = new Size(63, 14);
        _lblProgressStatusTitle.TabIndex = 7;
        _lblProgressStatusTitle.Text = "Trạng thái:";
        // 
        // _lblProgressStatus
        // 
        _lblProgressStatus.AutoEllipsis = true;
        _lblProgressStatus.BackColor = Color.Transparent;
        _lblProgressStatus.Font = new Font("Bahnschrift", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
        _lblProgressStatus.ForeColor = Color.FromArgb(211, 174, 187);
        _lblProgressStatus.Location = new Point(82, 570);
        _lblProgressStatus.Name = "_lblProgressStatus";
        _lblProgressStatus.Size = new Size(391, 18);
        _lblProgressStatus.TabIndex = 8;
        _lblProgressStatus.Text = "Sẵn sàng";
        _lblProgressStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _progressBar
        // 
        _progressBar.BackColor = Color.FromArgb(118, 96, 136);
        _progressBar.ForeColor = Color.FromArgb(214, 81, 151);
        _progressBar.Location = new Point(495, 568);
        _progressBar.Name = "_progressBar";
        _progressBar.Size = new Size(160, 23);
        _progressBar.Style = ProgressBarStyle.Continuous;
        _progressBar.TabIndex = 9;
        _progressBar.TrackColor = Color.FromArgb(118, 96, 136);
        // 
        // _pnlLog
        // 
        _pnlLog.BackColor = Color.FromArgb(34, 30, 42);
        _pnlLog.Controls.Add(_rtbLog);
        _pnlLog.Location = new Point(19, 416);
        _pnlLog.Name = "_pnlLog";
        _pnlLog.Size = new Size(636, 143);
        _pnlLog.TabIndex = 11;
        // 
        // _rtbLog
        // 
        _rtbLog.BackColor = Color.FromArgb(34, 30, 42);
        _rtbLog.BorderStyle = BorderStyle.None;
        _rtbLog.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        _rtbLog.ForeColor = Color.FromArgb(238, 226, 241);
        _rtbLog.Location = new Point(10, 10);
        _rtbLog.Name = "_rtbLog";
        _rtbLog.ReadOnly = true;
        _rtbLog.ScrollBars = RichTextBoxScrollBars.None;
        _rtbLog.Size = new Size(614, 123);
        _rtbLog.TabIndex = 0;
        _rtbLog.Text = "";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(43, 39, 52);
        BackgroundImageLayout = ImageLayout.Stretch;
        ClientSize = new Size(676, 606);
        Controls.Add(_pnlHeader);
        Controls.Add(_txtGamePath);
        Controls.Add(_btnBrowse);
        Controls.Add(_pnlGameInfo);
        Controls.Add(_pnlPatchInfo);
        Controls.Add(_pnlOps);
        Controls.Add(_pnlOptions);
        Controls.Add(_lblProgressStatusTitle);
        Controls.Add(_lblProgressStatus);
        Controls.Add(_progressBar);
        Controls.Add(_pnlLog);
        Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimumSize = new Size(692, 645);
        Name = "MainForm";
        ShowIcon = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Heaven Burns Red Global - Vietnamese Patch by VNKeyFC";
        _pnlHeader.ResumeLayout(false);
        _pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_picIcon).EndInit();
        _pnlGameInfo.ResumeLayout(false);
        _pnlGameInfo.PerformLayout();
        _pnlPatchInfo.ResumeLayout(false);
        _pnlPatchInfo.PerformLayout();
        _pnlOps.ResumeLayout(false);
        _pnlOps.PerformLayout();
        _pnlOptions.ResumeLayout(false);
        _pnlOptions.PerformLayout();
        _pnlLog.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

}
