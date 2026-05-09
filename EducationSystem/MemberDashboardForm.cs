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
    public partial class MemberDashboardForm : Form
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
        private readonly Color WarningSoft = ColorTranslator.FromHtml("#FFE8A3");
        private readonly Color WarningText = ColorTranslator.FromHtml("#8A2D00");
        private readonly Color ErrorText = ColorTranslator.FromHtml("#BA1A1A");

        private readonly List<MemberLoan> loans = new List<MemberLoan>();

        private readonly System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();

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

        private sealed class MemberLoan
        {
            public string BookId { get; set; } = "";
            public string BookTitle { get; set; } = "";
            public string Author { get; set; } = "";
            public string ISBN { get; set; } = "";
            public DateTime IssueDate { get; set; }
            public DateTime DueDate { get; set; }
            public string Status { get; set; } = "ACTIVE";
            public string Category { get; set; } = "General";
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
                int size = radius * 2;
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

        public MemberDashboardForm()
        {
            InitializeComponent();

            Text = "Member Dashboard";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 640);
            BackColor = FormBack;

            BuildShell();
            BuildSidebar();
            BuildHeader();
            BuildFooter();

            Load += (s, e) =>
            {
                LoadPortalSettings();
                LoadMemberLoans();
                LoadUserInfo();
                StartClock();
                ShowDashboard();
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
                AutoScroll = true,
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

            navDashboard.Click += (s, e) => ShowDashboard();
            navBorrowing.Click += (s, e) => OpenBorrowingStatusWindow();
            navLogout.Click += (s, e) =>
            {
                LoginForm login = new LoginForm();
                login.Show();
                Close();
            };
        }


        private void OpenBorrowingStatusWindow()
        {
            Cursor = Cursors.WaitCursor;

            MemberBorrowingStatusForm borrowingStatus = new MemberBorrowingStatusForm
            {
                StartPosition = FormStartPosition.Manual,
                Bounds = Bounds,
                WindowState = WindowState,
                Opacity = 0
            };

            borrowingStatus.Shown += (sender, args) =>
            {
                borrowingStatus.BeginInvoke(new Action(() =>
                {
                    borrowingStatus.Opacity = 1;
                    borrowingStatus.BringToFront();
                    Hide();
                    Cursor = Cursors.Default;
                }));
            };

            borrowingStatus.FormClosed += (sender, args) =>
            {
                Cursor = Cursors.Default;
            };

            borrowingStatus.Show();
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

        private void LoadMemberLoans()
        {
            loans.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"SELECT 
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
                    string rawStatus = Convert.ToString(reader["Status"]) ?? "ACTIVE";
                    bool returned = rawStatus.Equals("RETURNED", StringComparison.OrdinalIgnoreCase);
                    bool overdue = dueDate.Date < DateTime.Today && !returned;

                    decimal storedFine = reader["FineAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["FineAmount"]);
                    decimal computedFine = overdue ? CalculateOverdueFine(dueDate) : 0m;
                    decimal fine = Math.Max(storedFine, computedFine);

                    loans.Add(new MemberLoan
                    {
                        BookId = Convert.ToString(reader["BookID"]) ?? "",
                        BookTitle = Convert.ToString(reader["BookTitle"]) ?? "Untitled Book",
                        Author = Convert.ToString(reader["Author"]) ?? "Library Collection",
                        ISBN = Convert.ToString(reader["ISBN"]) ?? "",
                        Category = Convert.ToString(reader["Category"]) ?? "General",
                        IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                        DueDate = dueDate,
                        Status = overdue ? "OVERDUE" : rawStatus.ToUpperInvariant(),
                        Fine = fine
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load member dashboard records.\n\n" + ex.Message,
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
                    portalSettings.BranchLocation = ReadText(reader["LibraryBranchLocation"], "Davao City, Davao del Sur, Philippines");
                }

                reader.Close();

                string fineQuery = @"
SELECT TOP 1 DefaultAmount
FROM dbo.FinePolicies
WHERE ClientID = @ClientID
AND UPPER(FineType) IN ('LATE RETURN', 'OVERDUE')
AND ISNULL(IsArchived, 0) = 0
ORDER BY PolicyID DESC;";

                using SqlCommand fineCmd = new SqlCommand(fineQuery, conn);
                fineCmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

                object? result = fineCmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    portalSettings.DailyLateFee = Convert.ToDecimal(result);
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
            decimal fine = daysLate * portalSettings.DailyLateFee;

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
                LayoutDashboardCanvas();
        }

        private void PrepareCanvas(Button activeButton)
        {
            SetActive(activeButton);
            contentHost.Controls.Clear();

            canvas = new Panel
            {
                BackColor = FormBack,
                Location = new Point(0, 0)
            };

            contentHost.Controls.Add(canvas);
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

        private void ShowDashboard()
        {
            PrepareCanvas(navDashboard);

            string firstName = GetFirstName();
            int activeCount = loans.Count;
            int dueSoonCount = CountDueSoon();
            decimal fines = GetTotalOutstandingFine();

            Label title = CreateLabel("Academic Dashboard", 28F, FontStyle.Bold, PrimaryText);
            title.Name = "Title";

            Label subtitle = CreateLabel($"Welcome back, {firstName}. You have {dueSoonCount} book due for return soon.", 11F, FontStyle.Regular, ColorTranslator.FromHtml("#3C4A44"));
            subtitle.Name = "Subtitle";

            Panel cardBooks = CreateMetricCard("📖", "ACTIVE", activeCount + " Books", "Currently Borrowed", AccentDeep, ColorTranslator.FromHtml("#B7EBD7"));
            cardBooks.Name = "CardBooks";

            Panel cardDue = CreateMetricCard("▣", "PRIORITY", dueSoonCount + " Due", "Return within 48 hours", ColorTranslator.FromHtml("#A03F30"), ColorTranslator.FromHtml("#FFDDD8"));
            cardDue.Name = "CardDue";

            Panel cardFine = CreateMetricCard("💵", "OUTSTANDING", FormatPeso(fines), "Accumulated Fines", AccentDeep, ColorTranslator.FromHtml("#DDF4EE"));
            cardFine.Name = "CardFine";

            Panel activeLoans = BuildActiveLoansCard();
            activeLoans.Name = "ActiveLoans";

            Panel hours = BuildHoursCard();
            hours.Name = "Hours";

            Panel map = BuildLocationCard();
            map.Name = "Location";

            canvas.Controls.Add(title);
            canvas.Controls.Add(subtitle);
            canvas.Controls.Add(cardBooks);
            canvas.Controls.Add(cardDue);
            canvas.Controls.Add(cardFine);
            canvas.Controls.Add(activeLoans);
            canvas.Controls.Add(hours);
            canvas.Controls.Add(map);

            LayoutDashboardCanvas();
        }



        private void LayoutDashboardCanvas()
        {
            if (canvas.Controls.Count == 0) return;

            bool compact = contentHost.ClientSize.Width < 1000;
            bool narrow = contentHost.ClientSize.Width < 760;

            int left = narrow ? 18 : compact ? 24 : 40;
            int top = compact ? 28 : 36;
            int width = Math.Max(360, contentHost.ClientSize.Width - left * 2 - 18);
            int gap = compact ? 16 : 24;

            Control title = canvas.Controls["Title"];
            Control subtitle = canvas.Controls["Subtitle"];
            Control cardBooks = canvas.Controls["CardBooks"];
            Control cardDue = canvas.Controls["CardDue"];
            Control cardFine = canvas.Controls["CardFine"];
            Panel activeLoans = (Panel)canvas.Controls["ActiveLoans"];
            Control hours = canvas.Controls["Hours"];
            Control map = canvas.Controls["Location"];

            title.Location = new Point(left, top);
            title.Font = new Font("Segoe UI", narrow ? 22F : 28F, FontStyle.Bold);

            subtitle.Location = new Point(left + 2, title.Bottom + 6);
            subtitle.MaximumSize = new Size(width, 0);

            int cardTop = subtitle.Bottom + (compact ? 22 : 34);
            int cardHeight = compact ? 162 : 182;

            if (narrow)
            {
                cardBooks.Bounds = new Rectangle(left, cardTop, width, cardHeight);
                cardDue.Bounds = new Rectangle(left, cardBooks.Bottom + gap, width, cardHeight);
                cardFine.Bounds = new Rectangle(left, cardDue.Bottom + gap, width, cardHeight);
            }
            else
            {
                int cardWidth = (width - gap * 2) / 3;
                cardBooks.Bounds = new Rectangle(left, cardTop, cardWidth, cardHeight);
                cardDue.Bounds = new Rectangle(cardBooks.Right + gap, cardTop, cardWidth, cardHeight);
                cardFine.Bounds = new Rectangle(cardDue.Right + gap, cardTop, cardWidth, cardHeight);
            }

            int bodyTop = cardFine.Bottom + (compact ? 24 : 32);

            if (compact)
            {
                activeLoans.Bounds = new Rectangle(left, bodyTop, width, 470);
                LayoutActiveLoanItems(activeLoans);

                hours.Bounds = new Rectangle(left, activeLoans.Bottom + gap, width, 220);
                map.Bounds = new Rectangle(left, hours.Bottom + gap, width, 300);
            }
            else
            {
                int sideW = Math.Max(300, (int)(width * 0.31));
                int mainW = width - sideW - gap;

                activeLoans.Bounds = new Rectangle(left, bodyTop, mainW, 500);
                LayoutActiveLoanItems(activeLoans);

                hours.Bounds = new Rectangle(left + mainW + gap, bodyTop, sideW, 220);

                int mapTop = hours.Bottom + 24;
                int mapHeight = Math.Max(256, activeLoans.Bottom - mapTop);
                map.Bounds = new Rectangle(hours.Left, mapTop, sideW, mapHeight);
            }

            int canvasHeight = Math.Max(activeLoans.Bottom, map.Bottom) + 60;
            canvas.Size = new Size(Math.Max(1, contentHost.ClientSize.Width - 18), canvasHeight);
            contentHost.AutoScrollMinSize = new Size(0, canvasHeight);
        }

        private void ShowBorrowingStatus()
        {
            PrepareCanvas(navBorrowing);

            Label title = CreateLabel("Borrowing Status", 28F, FontStyle.Bold, PrimaryText);
            title.Name = "Title";
            Label subtitle = CreateLabel("Track your active loans, due dates, and return priority.", 11F, FontStyle.Regular, ColorTranslator.FromHtml("#3C4A44"));
            subtitle.Name = "Subtitle";

            DataGridView grid = new DataGridView
            {
                Name = "StatusGrid",
                BackgroundColor = CardSoft,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 48,
                RowTemplate = { Height = 58 },
                GridColor = BorderSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#DDE7EA");
            grid.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryText;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = CardSoft;
            grid.DefaultCellStyle.ForeColor = PrimaryText;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#DDF4EE");
            grid.DefaultCellStyle.SelectionForeColor = PrimaryText;

            grid.Columns.Add("Book", "BOOK TITLE");
            grid.Columns.Add("Issue", "ISSUE DATE");
            grid.Columns.Add("Due", "DUE DATE");
            grid.Columns.Add("Status", "STATUS");

            foreach (MemberLoan loan in loans)
            {
                string status = GetLoanStatus(loan);
                grid.Rows.Add(loan.BookTitle, loan.IssueDate.ToString("MMM dd, yyyy"), loan.DueDate.ToString("MMM dd, yyyy"), status);
            }

            Panel table = new RoundedPanel
            {
                Name = "StatusCard",
                BackColor = CardSoft,
                BorderColor = BorderSoft,
                BorderSize = 1
            };
            table.Controls.Add(grid);

            canvas.Controls.Add(title);
            canvas.Controls.Add(subtitle);
            canvas.Controls.Add(table);

            int left = ClientSize.Width >= 1320 ? 40 : 28;
            int width = Math.Max(920, contentHost.ClientSize.Width - left * 2);
            title.Location = new Point(left, 36);
            subtitle.Location = new Point(left + 2, title.Bottom + 6);
            table.Bounds = new Rectangle(left, subtitle.Bottom + 34, width, 430);
            grid.Bounds = new Rectangle(20, 20, table.Width - 40, table.Height - 40);
            canvas.Size = new Size(Math.Max(1, contentHost.ClientSize.Width - 24), table.Bottom + 60);
            contentHost.AutoScrollMinSize = new Size(0, table.Bottom + 60);
        }

        private Panel CreateMetricCard(string icon, string badge, string value, string caption, Color accent, Color iconBack)
        {
            Panel card = new RoundedPanel
            {
                BackColor = CardBack,
                BorderColor = ColorTranslator.FromHtml("#EEF5F7"),
                BorderSize = 1,
                Radius = 14
            };

            Panel iconBox = new RoundedPanel
            {
                BackColor = iconBack,
                Size = new Size(54, 54),
                Location = new Point(24, 24),
                Radius = 10
            };

            Label iconLabel = CreateLabel(icon, 18F, FontStyle.Bold, accent);
            iconLabel.AutoSize = false;
            iconLabel.TextAlign = ContentAlignment.MiddleCenter;
            iconLabel.Dock = DockStyle.Fill;
            iconBox.Controls.Add(iconLabel);

            Label badgeLabel = CreateLabel(badge, 8F, FontStyle.Bold, badge == "OUTSTANDING" ? ColorTranslator.FromHtml("#3C4A44") : accent);
            badgeLabel.Name = "Badge";

            Label valueLabel = CreateLabel(value, 27F, FontStyle.Bold, PrimaryText);
            valueLabel.Name = "Value";
            valueLabel.Location = new Point(24, 98);

            Label captionLabel = CreateLabel(caption, 10.5F, FontStyle.Regular, PrimaryText);
            captionLabel.Name = "Caption";
            captionLabel.Location = new Point(25, 140);

            card.Controls.Add(iconBox);
            card.Controls.Add(badgeLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(captionLabel);

            card.Resize += (s, e) =>
            {
                bool small = card.Width < 260;
                iconBox.Size = small ? new Size(46, 46) : new Size(54, 54);
                iconBox.Location = new Point(24, 24);
                valueLabel.Font = new Font("Segoe UI", small ? 22F : 27F, FontStyle.Bold);
                captionLabel.Font = new Font("Segoe UI", small ? 9.5F : 10.5F, FontStyle.Regular);
                badgeLabel.Location = new Point(card.Width - badgeLabel.Width - 24, 36);
                valueLabel.Location = new Point(24, Math.Max(88, card.Height - 84));
                captionLabel.Location = new Point(25, Math.Max(128, card.Height - 40));
            };

            return card;
        }


        private Panel BuildActiveLoansCard()
        {
            Panel outer = new RoundedPanel
            {
                BackColor = CardSoft,
                BorderColor = BorderSoft,
                BorderSize = 1,
                Radius = 14
            };

            Panel card = new RoundedPanel
            {
                Name = "Inner",
                BackColor = CardBack,
                Radius = 10
            };

            Label title = CreateLabel("My Active Loans", 18F, FontStyle.Bold, PrimaryText);
            title.Name = "CardTitle";
            title.Location = new Point(28, 28);

            Button history = CreateLinkButton("VIEW HISTORY");
            history.Name = "History";
            history.Click += (s, e) => OpenBorrowingStatusWindow();

            card.Controls.Add(title);
            card.Controls.Add(history);

            int index = 0;
            foreach (MemberLoan loan in loans)
            {
                Panel item = CreateLoanItem(loan, index);
                item.Name = "LoanItem" + index;
                card.Controls.Add(item);
                index++;
            }

            outer.Controls.Add(card);
            outer.Resize += (s, e) =>
            {
                card.Bounds = new Rectangle(4, 4, outer.Width - 8, outer.Height - 8);
                LayoutActiveLoanItems(outer);
            };

            return outer;
        }


        private void LayoutActiveLoanItems(Panel outer)
        {
            Panel? card = outer.Controls["Inner"] as Panel;
            if (card == null)
                card = outer;

            Control? history = card.Controls["History"];
            if (history != null)
                history.Location = new Point(card.Width - history.Width - 28, 31);

            int y = 92;
            foreach (Control ctrl in card.Controls)
            {
                if (!ctrl.Name.StartsWith("LoanItem", StringComparison.Ordinal))
                    continue;

                ctrl.Bounds = new Rectangle(28, y, card.Width - 56, 88);
                y += 108;
            }
        }


        private Panel CreateLoanItem(MemberLoan loan, int index)
        {
            Panel item = new RoundedPanel
            {
                BackColor = FormBack,
                Radius = 8
            };

            Panel cover = new RoundedPanel
            {
                BackColor = GetBookCoverColor(loan),
                Size = new Size(48, 66),
                Location = new Point(18, 11),
                Radius = 5,
                BorderColor = Color.FromArgb(35, 0, 0, 0),
                BorderSize = 1
            };

            Label coverText = new Label
            {
                Text = GetBookCoverText(loan),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 6.2F, FontStyle.Bold),
                ForeColor = GetBookCoverTextColor(loan),
                BackColor = Color.Transparent
            };
            cover.Controls.Add(coverText);

            Label title = CreateLabel(loan.BookTitle, 13F, FontStyle.Bold, PrimaryText);
            title.MaximumSize = new Size(460, 0);
            title.Location = new Point(86, 25);

            string authorText = string.IsNullOrWhiteSpace(loan.Author) ? loan.Category : "by " + loan.Author;
            Label author = CreateLabel(authorText, 9.5F, FontStyle.Regular, SecondaryText);
            author.Location = new Point(87, 51);

            Label dueLabel = CreateLabel("DUE DATE", 7.5F, FontStyle.Bold, ColorTranslator.FromHtml("#3C4A44"));
            dueLabel.Name = "DueLabel";
            dueLabel.TextAlign = ContentAlignment.MiddleRight;
            dueLabel.Size = new Size(130, 18);

            bool urgent = GetLoanStatus(loan) == "OVERDUE" || GetLoanStatus(loan) == "DUE SOON";
            Label dueDate = CreateLabel(loan.DueDate.ToString("MMM dd, yyyy"), 11.5F, FontStyle.Bold, urgent ? ErrorText : PrimaryText);
            dueDate.Name = "DueDate";
            dueDate.TextAlign = ContentAlignment.MiddleRight;
            dueDate.Size = new Size(150, 24);

            item.Controls.Add(cover);
            item.Controls.Add(title);
            item.Controls.Add(author);
            item.Controls.Add(dueLabel);
            item.Controls.Add(dueDate);

            item.Resize += (s, e) =>
            {
                dueLabel.Location = new Point(item.Width - 162, 23);
                dueDate.Location = new Point(item.Width - 172, 44);
            };

            return item;
        }



        private string GetBookCoverText(MemberLoan loan)
        {
            string source = !string.IsNullOrWhiteSpace(loan.Category) && loan.Category != "General"
                ? loan.Category
                : loan.BookTitle;

            string[] words = source.Split(new[] { ' ', '-', '_', '/', ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length >= 2)
                return (words[0][0].ToString() + words[1][0]).ToUpperInvariant();

            if (words.Length == 1)
                return words[0].Length <= 4 ? words[0].ToUpperInvariant() : words[0].Substring(0, 4).ToUpperInvariant();

            return "BOOK";
        }

        private Color GetBookCoverColor(MemberLoan loan)
        {
            string key = (loan.Category + " " + loan.BookTitle).ToLowerInvariant();

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

            return CardDark;
        }

        private Color GetBookCoverTextColor(MemberLoan loan)
        {
            return AccentMint;
        }


        private Panel BuildHoursCard()
        {
            Panel card = new RoundedPanel
            {
                BackColor = CardDark,
                BorderColor = Color.FromArgb(30, 0, 0, 0),
                BorderSize = 1,
                Radius = 14
            };

            Label title = CreateLabel("LIBRARY HOURS", 12F, FontStyle.Bold, AccentEmerald);
            title.Location = new Point(26, 24);
            card.Controls.Add(title);

            AddHoursRow(card, "Mon - Fri", portalSettings.HoursMonFri, 72, Color.White);
            AddHoursRow(card, "Saturday", portalSettings.HoursSaturday, 112, Color.White);
            AddHoursRow(card, "Sunday", portalSettings.HoursSunday, 152,
                portalSettings.HoursSunday.Equals("Closed", StringComparison.OrdinalIgnoreCase)
                    ? ColorTranslator.FromHtml("#F7816D")
                    : Color.White);

            Panel line = new Panel { BackColor = Color.FromArgb(35, 255, 255, 255), Bounds = new Rectangle(26, 182, 240, 1) };

            Label statusDot = CreateLabel("●", 10F, FontStyle.Bold, AccentEmerald);
            statusDot.Location = new Point(26, 194);

            Label status = CreateLabel("SYSTEM STATUS: OPTIMAL", 9F, FontStyle.Bold, Color.White);
            status.Location = new Point(44, 196);
            card.Controls.Add(line);
            card.Controls.Add(statusDot);
            card.Controls.Add(status);

            return card;
        }


        private void AddHoursRow(Panel parent, string day, string time, int y, Color timeColor)
        {
            Label left = CreateLabel(day, 10F, FontStyle.Regular, MutedText);
            left.Location = new Point(26, y);
            Label right = CreateLabel(time, 11F, FontStyle.Bold, timeColor);
            right.TextAlign = ContentAlignment.MiddleRight;
            right.Size = new Size(150, 24);
            right.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            right.Location = new Point(parent.Width - 176, y - 2);
            right.Resize += (s, e) => { };
            parent.Controls.Add(left);
            parent.Controls.Add(right);
            parent.Resize += (s, e) => right.Location = new Point(parent.Width - right.Width - 26, y - 2);
        }

        private Panel BuildLocationCard()
        {
            Panel outer = new RoundedPanel
            {
                BackColor = ColorTranslator.FromHtml("#DDE4E6"),
                Radius = 12
            };

            Panel card = new RoundedPanel
            {
                Name = "MapCard",
                BackColor = ColorTranslator.FromHtml("#41504B"),
                Radius = 10
            };

            Label shelves = CreateLabel("▥ ▥ ▥ ▥ ▥ ▥ ▥", 24F, FontStyle.Bold, Color.FromArgb(95, 255, 255, 255));
            shelves.Name = "Shelves";

            Label map = CreateLabel(portalSettings.BranchName, 11F, FontStyle.Bold, Color.White);
            map.Name = "MapTitle";

            Label floor = CreateLabel(portalSettings.BranchLocation.ToUpperInvariant(), 8F, FontStyle.Bold, AccentMint);
            floor.Name = "MapFloor";
            floor.MaximumSize = new Size(300, 0);

            card.Controls.Add(shelves);
            card.Controls.Add(map);
            card.Controls.Add(floor);
            outer.Controls.Add(card);

            outer.Resize += (s, e) =>
            {
                card.Bounds = new Rectangle(4, 4, outer.Width - 8, outer.Height - 8);
                shelves.Location = new Point(22, 28);
                map.Location = new Point(22, card.Height - 82);
                map.MaximumSize = new Size(card.Width - 44, 0);
                floor.Location = new Point(22, card.Height - 52);
                floor.MaximumSize = new Size(card.Width - 44, 0);
            };

            return outer;
        }

        private Panel BuildRenewCard()
        {
            Panel card = new RoundedPanel
            {
                BackColor = ColorTranslator.FromHtml("#B7EBD7"),
                Radius = 12
            };

            Label title = CreateLabel("Renew Books", 13F, FontStyle.Bold, ColorTranslator.FromHtml("#376758"));
            title.Location = new Point(26, 20);

            Label subtitle = CreateLabel("Extend your due dates online", 9.5F, FontStyle.Regular, ColorTranslator.FromHtml("#5C7D72"));
            subtitle.Location = new Point(27, 46);

            Label arrow = CreateLabel("→", 25F, FontStyle.Regular, ColorTranslator.FromHtml("#376758"));
            arrow.Name = "Arrow";

            card.Controls.Add(title);
            card.Controls.Add(subtitle);
            card.Controls.Add(arrow);
            card.Resize += (s, e) => arrow.Location = new Point(card.Width - 48, 22);

            return card;
        }


        private Button CreatePrimaryButton(string text, int width, int height)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(width, height),
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = ColorTranslator.FromHtml("#004233"),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Button CreateLinkButton(string text)
        {
            Button btn = new Button
            {
                Text = text,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = AccentDeep,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
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

        private decimal GetTotalOutstandingFine()
        {
            decimal total = 0m;

            foreach (MemberLoan loan in loans)
                total += loan.Fine;

            return total;
        }

        private int CountDueSoon()
        {
            int count = 0;
            foreach (MemberLoan loan in loans)
            {
                if (loan.DueDate.Date <= DateTime.Today.AddDays(2))
                    count++;
            }
            return count;
        }

        private string GetLoanStatus(MemberLoan loan)
        {
            if (loan.DueDate.Date < DateTime.Today)
                return "OVERDUE";
            if (loan.DueDate.Date <= DateTime.Today.AddDays(2))
                return "DUE SOON";
            return loan.Status.ToUpperInvariant();
        }

        private string GetFirstName()
        {
            string name = string.IsNullOrWhiteSpace(ClientSession.Username) ? "Member" : ClientSession.Username!;
            return name.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clockTimer.Stop();
            clockTimer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
