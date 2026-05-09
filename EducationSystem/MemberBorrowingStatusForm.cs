using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class MemberBorrowingStatusForm : Form
    {
        private readonly Color SidebarBack = ColorTranslator.FromHtml("#2B3234");
        private readonly Color SidebarHover = Color.FromArgb(58, 66, 68);
        private readonly Color SidebarActive = Color.FromArgb(18, 0, 184, 148);
        private readonly Color AccentMint = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color TopBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color CardSoft = ColorTranslator.FromHtml("#EAF3F5");
        private readonly Color CardDark = ColorTranslator.FromHtml("#2B3234");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#64748B");
        private readonly Color MutedText = ColorTranslator.FromHtml("#BBCAC3");
        private readonly Color BorderSoft = ColorTranslator.FromHtml("#DDE7EA");
        private readonly Color ErrorText = ColorTranslator.FromHtml("#BA1A1A");

        private readonly System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        private readonly List<MemberBorrowRecord> records = new List<MemberBorrowRecord>();

        private Panel sidebar = null!;
        private Panel topbar = null!;
        private Panel footer = null!;
        private Panel contentHost = null!;
        private Panel canvas = null!;

        private Label lblClock = null!;
        private PictureBox logoBox = null!;
        private Label lblBrand = null!;
        private Label lblBrandSub = null!;

        private Panel userHeaderHost = null!;
        private FlowLayoutPanel userHeaderPanel = null!;
        private PictureBox profileImage = null!;
        private Label lblUsername = null!;
        private Label lblDropdown = null!;
        private ContextMenuStrip profileMenu = null!;

        private Button navDashboard = null!;
        private Button navBorrowing = null!;
        private Button navLogout = null!;
        private Button? activeNav;
        private TextBox searchBox = null!;
        private string currentSearchText = "";

        private sealed class MemberBorrowRecord
        {
            public string BookId { get; set; } = "";
            public string BookTitle { get; set; } = "";
            public string Author { get; set; } = "";
            public string ISBN { get; set; } = "";
            public string Category { get; set; } = "General";
            public DateTime BorrowDate { get; set; }
            public DateTime DueDate { get; set; }
            public string Status { get; set; } = "BORROWED";
            public decimal Fine { get; set; }
        }

        private sealed class PortalSettings
        {
            public string HoursMonFri { get; set; } = "08:00 - 22:00";
            public string HoursSaturday { get; set; } = "10:00 - 18:00";
            public string HoursSunday { get; set; } = "Closed";
            public string BranchName { get; set; } = "Main Library Branch";
            public string BranchLocation { get; set; } = "Davao City, Davao del Sur, Philippines";
            public decimal DailyLateFee { get; set; } = 2.50m;
            public decimal MaximumTotalFee { get; set; } = 100.00m;
            public int DailyLimit { get; set; } = 3;
            public int LoanPeriodDays { get; set; } = 7;
        }

        private PortalSettings portalSettings = new PortalSettings();

        private sealed class RoundedPanel : Panel
        {
            public int Radius { get; set; } = 14;
            public Color BorderColor { get; set; } = Color.Transparent;
            public int BorderSize { get; set; }

            public RoundedPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Width < 2 || Height < 2) return;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using GraphicsPath path = RoundPath(ClientRectangle, Radius);
                using SolidBrush fill = new SolidBrush(BackColor);
                e.Graphics.FillPath(fill, path);

                if (BorderSize > 0)
                {
                    using Pen border = new Pen(BorderColor, BorderSize);
                    e.Graphics.DrawPath(border, path);
                }

                base.OnPaint(e);
            }

            private static GraphicsPath RoundPath(Rectangle bounds, int radius)
            {
                int size = Math.Max(2, radius * 2);
                Rectangle rect = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                GraphicsPath path = new GraphicsPath();
                path.AddArc(rect.Left, rect.Top, size, size, 180, 90);
                path.AddArc(rect.Right - size, rect.Top, size, size, 270, 90);
                path.AddArc(rect.Right - size, rect.Bottom - size, size, size, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - size, size, size, 90, 90);
                path.CloseFigure();
                return path;
            }
        }

        public MemberBorrowingStatusForm()
        {
            InitializeComponent();

            Text = "Borrowing Status";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 640);
            BackColor = FormBack;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            BuildShell();
            BuildSidebar();
            BuildHeader();
            BuildFooter();

            Load += (s, e) =>
            {
                LoadPortalSettings();
                LoadBorrowRecords();
                LoadUserInfo();
                StartClock();
                ShowBorrowingStatus();
                ApplyResponsiveLayout();
            };

            Resize += (s, e) => ApplyResponsiveLayout();
        }

        private void BuildShell()
        {
            sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 255,
                BackColor = SidebarBack
            };

            topbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = TopBack
            };

            footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = FormBack
            };

            contentHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = FormBack,
                AutoScroll = true
            };
            contentHost.HorizontalScroll.Enabled = false;
            contentHost.HorizontalScroll.Visible = false;

            Controls.Add(contentHost);
            Controls.Add(footer);
            Controls.Add(topbar);
            Controls.Add(sidebar);
        }

        private void BuildSidebar()
        {
            Panel brandPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 104,
                BackColor = SidebarBack
            };

            logoBox = new PictureBox
            {
                Size = new Size(38, 38),
                Location = new Point(18, 22),
                BackColor = SidebarBack,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            LoadLogo();

            lblBrand = new Label
            {
                Text = "LibraFlow ERP",
                AutoSize = true,
                Location = new Point(64, 24),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = AccentMint,
                BackColor = SidebarBack
            };

            lblBrandSub = new Label
            {
                Text = "Member Portal",
                AutoSize = true,
                Location = new Point(64, 50),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(190, 203, 210),
                BackColor = SidebarBack
            };

            brandPanel.Controls.Add(logoBox);
            brandPanel.Controls.Add(lblBrand);
            brandPanel.Controls.Add(lblBrandSub);

            FlowLayoutPanel menu = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                Padding = new Padding(14, 10, 14, 10),
                BackColor = SidebarBack
            };

            navDashboard = CreateNavButton("▦", "Dashboard");
            navBorrowing = CreateNavButton("▤", "Borrowing Status");

            menu.Controls.Add(navDashboard);
            menu.Controls.Add(navBorrowing);

            Panel bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                Padding = new Padding(14, 14, 14, 14),
                BackColor = SidebarBack
            };

            navLogout = CreateNavButton("⏻", "Sign Out");
            navLogout.Dock = DockStyle.Fill;
            navLogout.ForeColor = Color.FromArgb(255, 138, 128);
            navLogout.BackColor = Color.FromArgb(18, 255, 255, 255);
            bottom.Controls.Add(navLogout);

            sidebar.Controls.Add(menu);
            sidebar.Controls.Add(bottom);
            sidebar.Controls.Add(brandPanel);

            navDashboard.Click += (s, e) => OpenDashboardWindow();

            navBorrowing.Click += (s, e) => ShowBorrowingStatus();

            navLogout.Click += (s, e) =>
            {
                LoginForm login = new LoginForm();
                login.Show();
                Close();
            };
        }

        private void OpenDashboardWindow()
        {
            Cursor = Cursors.WaitCursor;

            MemberDashboardForm dashboard = new MemberDashboardForm
            {
                StartPosition = FormStartPosition.Manual,
                Bounds = Bounds,
                WindowState = WindowState,
                Opacity = 0
            };

            dashboard.Shown += (sender, args) =>
            {
                dashboard.BeginInvoke(new Action(() =>
                {
                    dashboard.Opacity = 1;
                    dashboard.BringToFront();
                    Close();
                    Cursor = Cursors.Default;
                }));
            };

            dashboard.FormClosed += (sender, args) =>
            {
                Cursor = Cursors.Default;
            };

            dashboard.Show();
        }

        private Button CreateNavButton(string icon, string text)
        {
            Button btn = new Button
            {
                Width = 220,
                Height = 52,
                FlatStyle = FlatStyle.Flat,
                BackColor = SidebarBack,
                ForeColor = Color.FromArgb(214, 222, 228),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 16, 0),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 6),
                Text = $"{icon}  {text}"
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = SidebarHover;
            btn.FlatAppearance.MouseDownBackColor = SidebarHover;

            return btn;
        }

        private void BuildHeader()
        {
            profileMenu = new ContextMenuStrip();
            profileMenu.Items.Add("Profile Settings", null, (s, e) => MessageBox.Show("Profile settings next."));
            profileMenu.Items.Add("Sign Out", null, (s, e) =>
            {
                LoginForm login = new LoginForm();
                login.Show();
                Close();
            });

            userHeaderHost = new Panel
            {
                BackColor = TopBack,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Height = 52
            };

            userHeaderPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = TopBack
            };

            profileImage = new PictureBox
            {
                Size = new Size(38, 38),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(0, 7, 10, 0),
                BackColor = TopBack,
                Cursor = Cursors.Hand
            };

            lblUsername = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(220, 0),
                AutoEllipsis = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = TopBack,
                Margin = new Padding(0, 14, 8, 0)
            };

            lblDropdown = new Label
            {
                Text = "▾",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = TopBack,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 14, 0, 0)
            };

            profileImage.Click += ShowProfileMenu;
            lblDropdown.Click += ShowProfileMenu;
            lblUsername.Click += ShowProfileMenu;

            userHeaderPanel.Controls.Add(profileImage);
            userHeaderPanel.Controls.Add(lblUsername);
            userHeaderPanel.Controls.Add(lblDropdown);

            userHeaderHost.Controls.Add(userHeaderPanel);
            topbar.Controls.Add(userHeaderHost);

            topbar.Resize += (s, e) => PositionHeader();
        }

        private void ShowProfileMenu(object? sender, EventArgs e)
        {
            profileMenu.Show(userHeaderHost, 0, userHeaderHost.Height);
        }

        private void PositionHeader()
        {
            if (userHeaderHost == null) return;

            userHeaderPanel.PerformLayout();
            userHeaderHost.Width = userHeaderPanel.PreferredSize.Width;
            userHeaderHost.Height = Math.Max(50, userHeaderPanel.PreferredSize.Height);

            userHeaderHost.Location = new Point(
                topbar.ClientSize.Width - userHeaderHost.Width - 20,
                Math.Max(0, (topbar.ClientSize.Height - userHeaderHost.Height) / 2)
            );

            userHeaderPanel.Location = new Point(0, 0);
            userHeaderHost.BringToFront();
        }

        private void BuildFooter()
        {
            lblClock = new Label
            {
                AutoSize = true,
                Location = new Point(10, 11),
                Font = new Font("Segoe UI", 8.8F),
                ForeColor = SecondaryText
            };

            footer.Controls.Add(lblClock);
        }

        private void LoadLogo()
        {
            try
            {
                string path = FindAssetPath("logo.png");

                if (File.Exists(path))
                {
                    using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    logoBox.Image = Image.FromStream(fs);
                }
            }
            catch
            {
                logoBox.Image = null;
            }
        }

        private string FindAssetPath(string fileName)
        {
            string[] paths =
            {
                Path.Combine(Application.StartupPath, "Assets", fileName),
                Path.Combine(Application.StartupPath, "..", "..", "..", "Assets", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", fileName)
            };

            foreach (string path in paths)
            {
                string full = Path.GetFullPath(path);
                if (File.Exists(full))
                    return full;
            }

            return "";
        }

        private void LoadUserInfo()
        {
            lblUsername.Text = string.IsNullOrWhiteSpace(ClientSession.Username)
                ? "Member"
                : ClientSession.Username;

            try
            {
                if (!string.IsNullOrWhiteSpace(ClientSession.ImagePath) && File.Exists(ClientSession.ImagePath))
                {
                    using FileStream fs = new FileStream(ClientSession.ImagePath, FileMode.Open, FileAccess.Read);
                    profileImage.Image = Image.FromStream(fs);
                }
            }
            catch
            {
                profileImage.Image = null;
            }

            profileImage.SizeMode = PictureBoxSizeMode.Zoom;
            PositionHeader();
        }

        private void LoadBorrowRecords()
        {
            records.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"SELECT 
    b.BorrowID,
    b.BookID,
    COALESCE(NULLIF(b.BookTitle, ''), 'Untitled Book') AS BookTitle,
    b.IssueDate,
    b.DueDate,
    b.Status,
    'Library Collection' AS Author,
    '' AS ISBN,
    'General' AS Category,
    ISNULL(fr.Amount, 0) AS FineAmount
FROM dbo.BorrowingRecords b
LEFT JOIN dbo.FineRecords fr
    ON fr.BorrowID = b.BorrowID
   AND fr.ClientID = b.ClientID
   AND ISNULL(fr.IsArchived, 0) = 0
WHERE b.MemberID = @MemberID
  AND b.ClientID = @ClientID
  AND ISNULL(b.IsArchived, 0) = 0
ORDER BY b.IssueDate DESC, b.BorrowID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MemberID", GetCurrentMemberId());
                cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime dueDate = Convert.ToDateTime(reader["DueDate"]);
                    string status = Convert.ToString(reader["Status"]) ?? "BORROWED";
                    bool returned = status.Equals("RETURNED", StringComparison.OrdinalIgnoreCase);
                    bool overdue = dueDate.Date < DateTime.Today && !returned;

                    decimal storedFine = reader["FineAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["FineAmount"]);
                    decimal computedFine = overdue ? CalculateOverdueFine(dueDate) : 0m;
                    decimal fine = Math.Max(storedFine, computedFine);

                    string isbn = Convert.ToString(reader["ISBN"]) ?? "";

                    records.Add(new MemberBorrowRecord
                    {
                        BookId = Convert.ToString(reader["BookID"]) ?? "",
                        BookTitle = Convert.ToString(reader["BookTitle"]) ?? "Untitled Book",
                        Author = Convert.ToString(reader["Author"]) ?? "Library Collection",
                        ISBN = string.IsNullOrWhiteSpace(isbn) ? "ISBN: Pending" : "ISBN: " + isbn,
                        Category = Convert.ToString(reader["Category"]) ?? "General",
                        BorrowDate = Convert.ToDateTime(reader["IssueDate"]),
                        DueDate = dueDate,
                        Status = overdue ? "OVERDUE" : status.ToUpperInvariant(),
                        Fine = fine
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load borrowing records.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadPortalSettings()
        {
            portalSettings = new PortalSettings();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"
SELECT TOP 1
    DailyLateFee,
    MaximumTotalFee,
    LibraryHoursMonFri,
    LibraryHoursSaturday,
    LibraryHoursSunday,
    LibraryBranchName,
    LibraryBranchLocation
FROM dbo.SystemConfigurations
WHERE ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    portalSettings.DailyLateFee = ReadDecimal(reader["DailyLateFee"], 2.50m);
                    portalSettings.MaximumTotalFee = ReadDecimal(reader["MaximumTotalFee"], 100.00m);
                    portalSettings.HoursMonFri = ReadText(reader["LibraryHoursMonFri"], "08:00 - 22:00");
                    portalSettings.HoursSaturday = ReadText(reader["LibraryHoursSaturday"], "10:00 - 18:00");
                    portalSettings.HoursSunday = ReadText(reader["LibraryHoursSunday"], "Closed");
                    portalSettings.BranchName = ReadText(reader["LibraryBranchName"], "Main Library Branch");
                    portalSettings.BranchLocation = ReadText(reader["LibraryBranchLocation"], "Davao City");
                }

                reader.Close();

                // LOAD LATE RETURN POLICY FROM ADMIN
                string fineQuery = @"
SELECT TOP 1 DefaultAmount
FROM dbo.FinePolicies
WHERE ClientID = @ClientID
AND FineType = 'LATE RETURN'
AND ISNULL(IsArchived,0)=0";

                using SqlCommand fineCmd = new SqlCommand(fineQuery, conn);
                fineCmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

                object result = fineCmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    portalSettings.DailyLateFee = Convert.ToDecimal(result);
                }
            }
            catch
            {
                // keep defaults
            }
        }
        private int GetCurrentClientId()
        {
            object? value =
                typeof(ClientSession).GetProperty("ClientId")?.GetValue(null) ??
                typeof(ClientSession).GetProperty("ClientID")?.GetValue(null);

            if (int.TryParse(Convert.ToString(value), out int clientId) && clientId > 0)
                return clientId;

            return 1;
        }

        private int GetCurrentMemberId()
        {
            object? value =
                typeof(ClientSession).GetProperty("UserID")?.GetValue(null) ??
                typeof(ClientSession).GetProperty("UserId")?.GetValue(null) ??
                typeof(ClientSession).GetProperty("MemberID")?.GetValue(null) ??
                typeof(ClientSession).GetProperty("MemberId")?.GetValue(null);

            if (int.TryParse(Convert.ToString(value), out int memberId) && memberId > 0)
                return memberId;

            return 0;
        }

        private static string ReadText(object value, string fallback)
        {
            if (value == DBNull.Value || value == null)
                return fallback;

            string text = Convert.ToString(value) ?? "";
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }

        private static decimal ReadDecimal(object value, decimal fallback)
        {
            if (value == DBNull.Value || value == null)
                return fallback;

            return decimal.TryParse(Convert.ToString(value), out decimal result) ? result : fallback;
        }

        private decimal CalculateOverdueFine(DateTime dueDate)
        {
            int daysLate = Math.Max(0, (DateTime.Today - dueDate.Date).Days);

            decimal policyRate = portalSettings.DailyLateFee;

            decimal fine = daysLate * policyRate;

            if (portalSettings.MaximumTotalFee > 0)
                fine = Math.Min(fine, portalSettings.MaximumTotalFee);

            return fine;
        }

        private string FormatPeso(decimal amount)
        {
            return "₱" + amount.ToString("0.00");
        }


        private void ApplyResponsiveLayout()
        {
            bool compact = ClientSize.Width < 1100;
            bool narrow = ClientSize.Width < 980;

            sidebar.Width = narrow ? 76 : compact ? 210 : 255;
            topbar.Height = compact ? 62 : 68;
            lblUsername.MaximumSize = new Size(narrow ? 110 : compact ? 150 : 220, 0);

            lblBrand.Visible = !narrow;
            lblBrandSub.Visible = !narrow;

            if (logoBox != null)
            {
                logoBox.Size = narrow ? new Size(34, 34) : new Size(38, 38);
                logoBox.Location = narrow ? new Point(21, 22) : new Point(18, 22);
            }

            foreach (Control c in sidebar.Controls)
            {
                if (c is FlowLayoutPanel flow)
                {
                    flow.Padding = narrow ? new Padding(10, 10, 10, 10) : new Padding(14, 10, 14, 10);

                    foreach (Control item in flow.Controls)
                    {
                        if (item is Button btn)
                        {
                            btn.Width = sidebar.Width - (narrow ? 20 : 28);
                            btn.TextAlign = narrow ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
                            btn.Padding = narrow ? new Padding(0) : new Padding(16, 0, 16, 0);

                            if (btn == navDashboard)
                                btn.Text = narrow ? "▦" : "▦  Dashboard";
                            else if (btn == navBorrowing)
                                btn.Text = narrow ? "▤" : "▤  Borrowing Status";
                        }
                    }
                }
            }

            if (navLogout != null)
            {
                navLogout.Text = narrow ? "⏻" : "⏻  Sign Out";
                navLogout.TextAlign = narrow ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
                navLogout.Padding = narrow ? new Padding(0) : new Padding(16, 0, 16, 0);
                navLogout.Width = sidebar.Width - (narrow ? 20 : 28);
            }

            PositionHeader();

            if (canvas != null)
                LayoutBorrowingStatusCanvas();
        }

        private void SetActive(Button btn)
        {
            activeNav = btn;

            Button[] buttons = { navDashboard, navBorrowing };

            foreach (Button b in buttons)
            {
                b.BackColor = SidebarBack;
                b.ForeColor = Color.FromArgb(214, 222, 228);
                b.Font = new Font("Segoe UI", 10F);
            }

            btn.BackColor = SidebarActive;
            btn.ForeColor = AccentMint;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void ShowBorrowingStatus()
        {
            SetActive(navBorrowing);
            contentHost.Controls.Clear();

            // IMPORTANT:
            // This form uses the modern custom-card UI below.
            // Do not add a DataGridView here, otherwise it will look like the old style again.
            canvas = new Panel
            {
                BackColor = FormBack,
                Location = new Point(0, 0)
            };

            contentHost.Controls.Add(canvas);

            Label title = CreateLabel("Borrowing Status", 26F, FontStyle.Bold, PrimaryText);
            title.Name = "Title";

            Label subtitle = CreateLabel("Monitor your current loans, overdue returns, and library records.", 11F, FontStyle.Regular, ColorTranslator.FromHtml("#3C4A44"));
            subtitle.Name = "Subtitle";

            Panel overview = BuildBorrowingOverviewCard();
            overview.Name = "BorrowingOverview";

            Panel fines = BuildFineDetailsCard();
            fines.Name = "FineDetails";

            Panel rules = BuildBorrowingRulesCard();
            rules.Name = "BorrowingRules";

            canvas.Controls.Add(title);
            canvas.Controls.Add(subtitle);
            canvas.Controls.Add(overview);
            canvas.Controls.Add(fines);
            canvas.Controls.Add(rules);

            LayoutBorrowingStatusCanvas();
        }


        private void LayoutBorrowingStatusCanvas()
        {
            if (canvas.Controls.Count == 0) return;

            bool compact = contentHost.ClientSize.Width < 1050;
            bool narrow = contentHost.ClientSize.Width < 760;

            int left = narrow ? 18 : compact ? 24 : 40;
            int top = compact ? 28 : 36;
            int width = Math.Max(360, contentHost.ClientSize.Width - left * 2 - 18);
            int gap = compact ? 16 : 24;

            Control title = canvas.Controls["Title"];
            Control subtitle = canvas.Controls["Subtitle"];
            Panel overview = (Panel)canvas.Controls["BorrowingOverview"];
            Panel fines = (Panel)canvas.Controls["FineDetails"];
            Panel rules = (Panel)canvas.Controls["BorrowingRules"];

            title.Location = new Point(left, top);
            title.Font = new Font("Segoe UI", narrow ? 22F : 26F, FontStyle.Bold);

            subtitle.Location = new Point(left + 2, title.Bottom + 8);
            subtitle.MaximumSize = new Size(width, 0);

            int bodyTop = subtitle.Bottom + (compact ? 24 : 34);

            if (compact)
            {
                int overviewHeight = Math.Max(560, 178 + (GetFilteredBorrowRecords().Count * (narrow ? 120 : 104)));
                overview.Bounds = new Rectangle(left, bodyTop, width, overviewHeight);
                LayoutBorrowingOverviewCard(overview);

                fines.Bounds = new Rectangle(left, overview.Bottom + gap, width, 245);
                LayoutFineDetailsCard(fines);

                rules.Bounds = new Rectangle(left, fines.Bottom + gap, width, 350);
                LayoutBorrowingRulesCard(rules);
            }
            else
            {
                int sideW = Math.Max(330, (int)(width * 0.31));
                int mainW = width - sideW - gap;

                int overviewHeight = Math.Max(620, 178 + (GetFilteredBorrowRecords().Count * 104));
                overview.Bounds = new Rectangle(left, bodyTop, mainW, overviewHeight);
                LayoutBorrowingOverviewCard(overview);

                fines.Bounds = new Rectangle(left + mainW + gap, bodyTop, sideW, 270);
                LayoutFineDetailsCard(fines);

                rules.Bounds = new Rectangle(fines.Left, fines.Bottom + gap, sideW, 350);
                LayoutBorrowingRulesCard(rules);
            }

            int canvasHeight = Math.Max(overview.Bottom, rules.Bottom) + 70;
            canvas.Size = new Size(Math.Max(1, contentHost.ClientSize.Width - 18), canvasHeight);
            contentHost.AutoScrollMinSize = new Size(0, canvasHeight);
        }

        private Panel BuildBorrowingOverviewCard()
        {
            Panel card = new RoundedPanel
            {
                BackColor = CardBack,
                BorderColor = BorderSoft,
                BorderSize = 1,
                Radius = 14
            };

            Label icon = CreateLabel("▤", 17F, FontStyle.Bold, AccentDeep);
            icon.Name = "OverviewIcon";

            Label title = CreateLabel("My Borrowing Overview", 18F, FontStyle.Bold, PrimaryText);
            title.Name = "OverviewTitle";

            Panel searchWrap = new RoundedPanel
            {
                Name = "SearchWrap",
                BackColor = ColorTranslator.FromHtml("#EEF5F7"),
                BorderColor = BorderSoft,
                BorderSize = 1,
                Radius = 8
            };

            Label searchIcon = new Label
            {
                Name = "SearchIcon",
                Text = "⌕",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = AccentDeep,
                BackColor = Color.Transparent
            };

            searchBox = new TextBox
            {
                Name = "SearchBox",
                BorderStyle = BorderStyle.None,
                BackColor = ColorTranslator.FromHtml("#EEF5F7"),
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 10F),
                Text = currentSearchText
            };

            searchBox.TextChanged += (s, e) =>
            {
                currentSearchText = searchBox.Text.Trim();
                RebuildBorrowingRows(card);
                LayoutBorrowingStatusCanvas();
            };

            searchWrap.Controls.Add(searchIcon);
            searchWrap.Controls.Add(searchBox);

            Panel header = new RoundedPanel
            {
                Name = "TableHeader",
                BackColor = ColorTranslator.FromHtml("#EEF5F7"),
                Radius = 8
            };

            string[] headers = { "BOOK TITLE", "BORROW\nDATE", "DUE\nDATE", "STATUS", "FINE" };
            foreach (string text in headers)
            {
                header.Controls.Add(new Label
                {
                    Text = text,
                    AutoSize = false,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml("#2C3A36"),
                    BackColor = Color.Transparent,
                    TextAlign = text == "FINE" ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft
                });
            }

            card.Controls.Add(icon);
            card.Controls.Add(title);
            card.Controls.Add(searchWrap);
            card.Controls.Add(header);

            RebuildBorrowingRows(card);

            card.Resize += (s, e) => LayoutBorrowingOverviewCard(card);
            return card;
        }

        private void RebuildBorrowingRows(Panel card)
        {
            List<Control> oldRows = new List<Control>();
            foreach (Control ctrl in card.Controls)
            {
                if (Convert.ToString(ctrl.Tag) == "BorrowRow")
                    oldRows.Add(ctrl);
            }

            foreach (Control row in oldRows)
            {
                card.Controls.Remove(row);
                row.Dispose();
            }

            int index = 0;
            foreach (MemberBorrowRecord record in GetFilteredBorrowRecords())
            {
                AddBorrowingRow(card, record, index);
                index++;
            }

            LayoutBorrowingOverviewCard(card);
        }

        private List<MemberBorrowRecord> GetFilteredBorrowRecords()
        {
            if (string.IsNullOrWhiteSpace(currentSearchText))
                return new List<MemberBorrowRecord>(records);

            string q = currentSearchText.Trim().ToLowerInvariant();
            List<MemberBorrowRecord> filtered = new List<MemberBorrowRecord>();

            foreach (MemberBorrowRecord record in records)
            {
                string haystack = string.Join(" ",
                    record.BookTitle,
                    record.Author,
                    record.ISBN,
                    record.Status,
                    record.BorrowDate.ToString("MMM dd yyyy"),
                    record.DueDate.ToString("MMM dd yyyy")
                ).ToLowerInvariant();

                if (haystack.Contains(q))
                    filtered.Add(record);
            }

            return filtered;
        }

        private void AddBorrowingRow(Panel parent, MemberBorrowRecord record, int index)
        {
            Panel row = new Panel
            {
                Name = "BorrowRow" + index,
                BackColor = CardBack,
                Tag = "BorrowRow"
            };

            Panel cover = new RoundedPanel
            {
                Name = "Cover",
                BackColor = GetBookCoverColor(record),
                Radius = 4,
                BorderColor = Color.FromArgb(30, 0, 0, 0),
                BorderSize = 1
            };

            Label coverText = new Label
            {
                Text = GetBookCoverText(record),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 6.3F, FontStyle.Bold),
                ForeColor = GetBookCoverTextColor(record),
                BackColor = Color.Transparent
            };
            cover.Controls.Add(coverText);

            Label title = CreateLabel(record.BookTitle, 12F, FontStyle.Bold, PrimaryText);
            title.Name = "BookTitle";

            Label sub = CreateLabel(string.IsNullOrWhiteSpace(record.ISBN) ? "ISBN: Pending" : record.ISBN, 8.5F, FontStyle.Regular, ColorTranslator.FromHtml("#3C4A44"));
            sub.Name = "ISBN";

            Label borrow = CreateLabel(record.BorrowDate.ToString("MMM dd,\nyyyy"), 11F, FontStyle.Regular, PrimaryText);
            borrow.Name = "BorrowDate";

            bool overdue = record.Status.Equals("OVERDUE", StringComparison.OrdinalIgnoreCase);
            Label due = CreateLabel(record.DueDate.ToString("MMM dd,\nyyyy"), 11F, FontStyle.Bold, overdue ? ErrorText : PrimaryText);
            due.Name = "DueDate";

            string status = string.IsNullOrWhiteSpace(record.Status) ? "BORROWED" : record.Status.ToUpperInvariant();
            Label statusPill = new Label
            {
                Name = "StatusPill",
                Text = status,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = status == "OVERDUE" ? ColorTranslator.FromHtml("#6E1B0F") : status == "RETURNED" ? ColorTranslator.FromHtml("#3C4A44") : ColorTranslator.FromHtml("#1E4F41"),
                BackColor = status == "OVERDUE" ? ColorTranslator.FromHtml("#F7816D") : status == "RETURNED" ? ColorTranslator.FromHtml("#DDE4E6") : ColorTranslator.FromHtml("#B7EBD7"),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label fineLabel = CreateLabel(record.Fine > 0 ? FormatPeso(record.Fine) : "—", 11F, FontStyle.Bold, record.Fine > 0 ? ErrorText : PrimaryText);
            fineLabel.Name = "Fine";
            fineLabel.TextAlign = ContentAlignment.MiddleRight;
            fineLabel.AutoSize = false;

            row.Controls.Add(cover);
            row.Controls.Add(title);
            row.Controls.Add(sub);
            row.Controls.Add(borrow);
            row.Controls.Add(due);
            row.Controls.Add(statusPill);
            row.Controls.Add(fineLabel);

            parent.Controls.Add(row);
        }



        private string GetBookCoverText(MemberBorrowRecord record)
        {
            string source = !string.IsNullOrWhiteSpace(record.Category) && record.Category != "General"
                ? record.Category
                : record.BookTitle;

            string[] words = source.Split(new[] { ' ', '-', '_', '/', ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length >= 2)
                return (words[0][0].ToString() + words[1][0]).ToUpperInvariant();

            if (words.Length == 1)
                return words[0].Length <= 4 ? words[0].ToUpperInvariant() : words[0].Substring(0, 4).ToUpperInvariant();

            return "BOOK";
        }

        private Color GetBookCoverColor(MemberBorrowRecord record)
        {
            string key = (record.Category + " " + record.BookTitle).ToLowerInvariant();

            if (key.Contains("law"))
                return ColorTranslator.FromHtml("#3A261A");
            if (key.Contains("technology") || key.Contains("computer") || key.Contains("data") || key.Contains("information"))
                return ColorTranslator.FromHtml("#0F766E");
            if (key.Contains("science"))
                return ColorTranslator.FromHtml("#172554");
            if (key.Contains("history"))
                return ColorTranslator.FromHtml("#6B3F1D");
            if (key.Contains("math") || key.Contains("calculus"))
                return ColorTranslator.FromHtml("#1E3A8A");
            if (key.Contains("fiction") || key.Contains("story"))
                return ColorTranslator.FromHtml("#4C1D95");

            return ColorTranslator.FromHtml("#E8EFF1");
        }

        private Color GetBookCoverTextColor(MemberBorrowRecord record)
        {
            Color back = GetBookCoverColor(record);
            return back.GetBrightness() < 0.55F ? Color.White : PrimaryText;
        }


        private void LayoutBorrowingOverviewCard(Panel card)
        {
            if (!card.Controls.ContainsKey("OverviewIcon") ||
                !card.Controls.ContainsKey("OverviewTitle") ||
                !card.Controls.ContainsKey("SearchWrap") ||
                !card.Controls.ContainsKey("TableHeader"))
                return;

            bool narrow = card.Width < 620;

            Control icon = card.Controls["OverviewIcon"];
            Control title = card.Controls["OverviewTitle"];
            Panel searchWrap = (Panel)card.Controls["SearchWrap"];
            Panel header = (Panel)card.Controls["TableHeader"];

            icon.Location = new Point(28, 29);
            title.Location = new Point(64, 28);

            if (narrow)
            {
                title.MaximumSize = new Size(card.Width - 96, 0);
                searchWrap.Bounds = new Rectangle(26, 70, card.Width - 52, 36);
            }
            else
            {
                int searchW = Math.Min(280, Math.Max(210, card.Width / 3));
                searchWrap.Bounds = new Rectangle(card.Width - searchW - 26, 24, searchW, 36);
                title.MaximumSize = new Size(Math.Max(240, searchWrap.Left - title.Left - 20), 0);
            }

            searchWrap.Controls["SearchIcon"].Bounds = new Rectangle(8, 0, 30, searchWrap.Height);
            searchWrap.Controls["SearchBox"].Bounds = new Rectangle(42, 9, searchWrap.Width - 54, 20);

            int left = 26;
            int tableW = card.Width - 52;
            int y = narrow ? searchWrap.Bottom + 18 : 84;

            header.Bounds = new Rectangle(left, y, tableW, 64);

            int[] col;
            if (narrow)
            {
                col = new[]
                {
                    (int)(tableW * 0.46),
                    (int)(tableW * 0.18),
                    (int)(tableW * 0.18),
                    (int)(tableW * 0.18),
                    0
                };
                header.Controls[4].Visible = false;
            }
            else
            {
                col = new[]
                {
                    (int)(tableW * 0.38),
                    (int)(tableW * 0.17),
                    (int)(tableW * 0.15),
                    (int)(tableW * 0.16),
                    (int)(tableW * 0.14)
                };
                header.Controls[4].Visible = true;
            }

            col[0] += tableW - (col[0] + col[1] + col[2] + col[3] + col[4]);

            int x = 0;
            for (int i = 0; i < header.Controls.Count; i++)
            {
                if (i == 4 && narrow) continue;
                header.Controls[i].Bounds = new Rectangle(x + 16, 0, Math.Max(20, col[i] - 24), 64);
                x += col[i];
            }

            y = header.Bottom + 14;
            foreach (Control ctrl in card.Controls)
            {
                if (Convert.ToString(ctrl.Tag) != "BorrowRow")
                    continue;

                ctrl.Bounds = new Rectangle(left, y, tableW, narrow ? 112 : 96);
                LayoutBorrowingRow((Panel)ctrl, col);
                y += narrow ? 120 : 104;
            }
        }


        private void LayoutBorrowingRow(Panel row, int[] col)
        {
            bool narrow = col.Length >= 5 && col[4] == 0;
            int x = 0;

            Control cover = row.Controls["Cover"];
            Control title = row.Controls["BookTitle"];
            Control isbn = row.Controls["ISBN"];
            Control borrow = row.Controls["BorrowDate"];
            Control due = row.Controls["DueDate"];
            Control status = row.Controls["StatusPill"];
            Control fine = row.Controls["Fine"];

            cover.Bounds = new Rectangle(16, narrow ? 27 : 20, 44, 58);
            title.Location = new Point(74, narrow ? 24 : 22);
            title.MaximumSize = new Size(Math.Max(120, col[0] - 96), 0);
            isbn.Location = new Point(75, narrow ? 64 : 60);
            x += col[0];

            borrow.Bounds = new Rectangle(x + 6, narrow ? 30 : 22, Math.Max(42, col[1] - 12), 58);
            x += col[1];

            due.Bounds = new Rectangle(x + 6, narrow ? 30 : 22, Math.Max(42, col[2] - 12), 58);
            x += col[2];

            int pillW = narrow ? Math.Max(76, col[3] - 12) : 84;
            status.Bounds = new Rectangle(x + 6, narrow ? 42 : 34, pillW, 26);
            x += col[3];

            fine.Visible = !narrow;
            if (!narrow)
                fine.Bounds = new Rectangle(x + 10, 32, col[4] - 20, 28);
        }

        private Panel BuildFineDetailsCard()
        {
            Panel card = new RoundedPanel
            {
                BackColor = CardDark,
                BorderColor = Color.FromArgb(35, 0, 0, 0),
                BorderSize = 1,
                Radius = 14
            };

            decimal totalFine = GetTotalFine();
            decimal overdueFine = Math.Min(totalFine, portalSettings.DailyLateFee * 5m);
            decimal surcharge = totalFine > 0 ? Math.Max(0, totalFine - overdueFine) : 0m;

            Label icon = CreateLabel("▣", 17F, FontStyle.Bold, AccentMint);
            icon.Name = "FineIcon";

            Label title = CreateLabel("Fine Details", 18F, FontStyle.Bold, Color.White);
            title.Name = "FineTitle";

            Label badge = new Label
            {
                Name = "PendingBadge",
                Text = totalFine > 0 ? "Pending" : "Clear",
                Font = new Font("Segoe UI", 8F),
                ForeColor = AccentMint,
                BackColor = ColorTranslator.FromHtml("#006B55"),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label row1 = CreateLabel("Overdue Fine", 11F, FontStyle.Regular, MutedText);
            row1.Name = "FineRow1";
            Label row1Amount = CreateLabel(FormatPeso(overdueFine), 11F, FontStyle.Bold, Color.White);
            row1Amount.Name = "FineRow1Amount";

            Label row2 = CreateLabel("Late Fee Surcharge", 11F, FontStyle.Regular, MutedText);
            row2.Name = "FineRow2";
            Label row2Amount = CreateLabel(FormatPeso(surcharge), 11F, FontStyle.Bold, Color.White);
            row2Amount.Name = "FineRow2Amount";

            Label totalLabel = CreateLabel("TOTAL\nOUTSTANDING", 9F, FontStyle.Regular, MutedText);
            totalLabel.Name = "TotalLabel";

            Label total = CreateLabel(FormatPeso(totalFine), 27F, FontStyle.Bold, AccentMint);
            total.Name = "TotalAmount";

            card.Controls.Add(icon);
            card.Controls.Add(title);
            card.Controls.Add(badge);
            card.Controls.Add(row1);
            card.Controls.Add(row1Amount);
            card.Controls.Add(row2);
            card.Controls.Add(row2Amount);
            card.Controls.Add(totalLabel);
            card.Controls.Add(total);

            card.Resize += (s, e) => LayoutFineDetailsCard(card);
            return card;
        }

        private void LayoutFineDetailsCard(Panel card)
        {
            if (!card.Controls.ContainsKey("FineIcon") ||
                !card.Controls.ContainsKey("FineTitle") ||
                !card.Controls.ContainsKey("PendingBadge") ||
                !card.Controls.ContainsKey("FineRow1") ||
                !card.Controls.ContainsKey("FineRow1Amount") ||
                !card.Controls.ContainsKey("FineRow2") ||
                !card.Controls.ContainsKey("FineRow2Amount") ||
                !card.Controls.ContainsKey("TotalLabel") ||
                !card.Controls.ContainsKey("TotalAmount"))
                return;

            card.Controls["FineIcon"].Location = new Point(24, 28);
            card.Controls["FineTitle"].Location = new Point(62, 24);
            card.Controls["PendingBadge"].Bounds = new Rectangle(card.Width - 88, 30, 62, 24);

            card.Controls["FineRow1"].Location = new Point(24, 82);
            card.Controls["FineRow1Amount"].Location = new Point(card.Width - 86, 82);

            card.Controls["FineRow2"].Location = new Point(24, 132);
            card.Controls["FineRow2Amount"].Location = new Point(card.Width - 86, 132);

            card.Controls["TotalLabel"].Location = new Point(24, 190);
            card.Controls["TotalAmount"].Location = new Point(24, 220);
        }

        private Panel BuildBorrowingRulesCard()
        {
            Panel card = new RoundedPanel
            {
                BackColor = CardBack,
                BorderColor = BorderSoft,
                BorderSize = 1,
                Radius = 14
            };

            Panel accent = new Panel
            {
                Name = "Accent",
                BackColor = AccentDeep
            };

            Label icon = CreateLabel("▦", 17F, FontStyle.Bold, AccentDeep);
            icon.Name = "RulesIcon";

            Label title = CreateLabel("Borrowing Rules", 18F, FontStyle.Bold, PrimaryText);
            title.Name = "RulesTitle";

            Panel daily = CreateRuleItem("Daily Limit", $"Limit: {portalSettings.DailyLimit} books per day", "▤", ColorTranslator.FromHtml("#B7EBD7"), AccentDeep);
            daily.Name = "RuleDaily";

            Panel period = CreateRuleItem("Loan Period", $"Duration: {portalSettings.LoanPeriodDays} days per item", "▣", ColorTranslator.FromHtml("#B7EBD7"), AccentDeep);
            period.Name = "RulePeriod";

            Panel penalty = CreateRuleItem(
    "Overdue Penalty",
    $"{FormatPeso(portalSettings.DailyLateFee)} fine policy from admin settings", "!", ColorTranslator.FromHtml("#FFDDD8"), ColorTranslator.FromHtml("#A03F30"));
            penalty.Name = "RulePenalty";

            card.Controls.Add(accent);
            card.Controls.Add(icon);
            card.Controls.Add(title);
            card.Controls.Add(daily);
            card.Controls.Add(period);
            card.Controls.Add(penalty);

            card.Resize += (s, e) => LayoutBorrowingRulesCard(card);
            return card;
        }

        private Panel CreateRuleItem(string title, string subtitle, string icon, Color iconBack, Color iconColor)
        {
            Panel item = new RoundedPanel
            {
                BackColor = CardSoft,
                Radius = 9
            };

            Panel iconBox = new RoundedPanel
            {
                Name = "IconBox",
                BackColor = iconBack,
                Radius = 9
            };

            Label iconLabel = CreateLabel(icon, 16F, FontStyle.Bold, iconColor);
            iconLabel.Dock = DockStyle.Fill;
            iconLabel.TextAlign = ContentAlignment.MiddleCenter;
            iconLabel.AutoSize = false;
            iconBox.Controls.Add(iconLabel);

            Label titleLabel = CreateLabel(title, 11F, FontStyle.Bold, PrimaryText);
            titleLabel.Name = "RuleTitle";

            Label subLabel = CreateLabel(subtitle, 9.5F, FontStyle.Regular, ColorTranslator.FromHtml("#3C4A44"));
            subLabel.Name = "RuleSubtitle";

            item.Controls.Add(iconBox);
            item.Controls.Add(titleLabel);
            item.Controls.Add(subLabel);

            item.Resize += (s, e) =>
            {
                iconBox.Bounds = new Rectangle(14, 12, 46, 46);
                titleLabel.Location = new Point(76, 13);
                titleLabel.MaximumSize = new Size(item.Width - 92, 0);
                subLabel.Location = new Point(76, 38);
                subLabel.MaximumSize = new Size(item.Width - 92, 0);
            };

            return item;
        }

        private void LayoutBorrowingRulesCard(Panel card)
        {
            card.Controls["Accent"].Bounds = new Rectangle(0, 0, 4, card.Height);
            card.Controls["RulesIcon"].Location = new Point(28, 28);
            card.Controls["RulesTitle"].Location = new Point(64, 25);

            int itemX = 28;
            int itemW = card.Width - 56;

            card.Controls["RuleDaily"].Bounds = new Rectangle(itemX, 84, itemW, 74);
            card.Controls["RulePeriod"].Bounds = new Rectangle(itemX, 174, itemW, 74);
            card.Controls["RulePenalty"].Bounds = new Rectangle(itemX, 264, itemW, 74);
        }

        private decimal GetTotalFine()
        {
            decimal total = 0m;
            foreach (MemberBorrowRecord record in records)
                total += record.Fine;
            return total;
        }

        private Label CreateLabel(string text, float size, FontStyle style, Color color)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private void StartClock()
        {
            clockTimer.Interval = 1000;
            clockTimer.Tick += (s, e) => UpdateClock();
            clockTimer.Start();
            UpdateClock();
        }

        private void UpdateClock()
        {
            lblClock.Text = DateTime.Now.ToString("MMMM dd, yyyy - hh:mm:ss tt");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clockTimer.Stop();
            clockTimer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
