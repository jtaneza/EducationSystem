using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class LibrarianDashboardForm : Form
    {
        private readonly System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();

        private readonly Color SidebarBack = ColorTranslator.FromHtml("#2B3234");
        private readonly Color SidebarHover = Color.FromArgb(58, 66, 68);
        private readonly Color SidebarActive = Color.FromArgb(18, 0, 184, 148);
        private readonly Color AccentMint = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color TopBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color CardSoft = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color CardDark = ColorTranslator.FromHtml("#2B3234");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#64748B");
        private readonly Color MutedText = ColorTranslator.FromHtml("#BBCAC3");
        private readonly Color TableBack = ColorTranslator.FromHtml("#EAF3F5");
        private readonly Color BorderSoft = ColorTranslator.FromHtml("#DDE7EA");

        private Panel sidebar = null!;
        private Panel topbar = null!;
        private Panel footer = null!;
        private Panel contentHost = null!;
        private Panel dashboardCanvas = null!;

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
        private Button navUsers = null!;
        private Button navBorrow = null!;
        private Button navReturn = null!;
        private Button navFines = null!;
        private Button navSignOut = null!;
        private Button? activeNav;
        private TextBox loanSearchBox = null!;
        private Panel loanSearchPanel = null!;
        private Label loanSearchIcon = null!;
        private ComboBox chartPeriodCombo = null!;
        private Label chartSubtitle = null!;

        private readonly List<DashboardLoanRow> dashboardLoanRows = new List<DashboardLoanRow>();
        private int totalMembersCount;
        private int newMembersCount;
        private int overdueLoansCount;
        private int activeLoansCount;

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
                if (Width < 2 || Height < 2)
                    return;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using GraphicsPath path = CreateRoundPath(ClientRectangle, Radius);
                using SolidBrush fill = new SolidBrush(BackColor);
                e.Graphics.FillPath(fill, path);

                if (BorderSize > 0)
                {
                    using Pen border = new Pen(BorderColor, BorderSize);
                    e.Graphics.DrawPath(border, path);
                }

                base.OnPaint(e);
            }

            private static GraphicsPath CreateRoundPath(Rectangle bounds, int radius)
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

        private sealed class DashboardLoanRow
        {
            public string BookId { get; set; } = "";
            public string BookTitle { get; set; } = "";
            public string Description { get; set; } = "";
            public string Author { get; set; } = "";
            public string Year { get; set; } = "";
            public string Category { get; set; } = "";
            public string MemberName { get; set; } = "";
            public string MemberId { get; set; } = "";
            public string DueText { get; set; } = "";
            public bool Urgent { get; set; }
        }

        public LibrarianDashboardForm()
        {
            InitializeComponent();

            Text = "Librarian Dashboard";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1180, 760);
            BackColor = FormBack;

            BuildShell();
            BuildSidebar();
            BuildHeader();
            BuildFooter();

            Load += (s, e) =>
            {
                LoadUserInfo();
                StartClock();
                ShowDashboardHome();
                ApplyResponsiveLayout();
            };

            Resize += (s, e) => ApplyResponsiveLayout();
        }

        private void BuildShell()
        {
            sidebar = new Panel
            {
                Dock = DockStyle.Left,
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
                Text = "Librarian Dashboard",
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
            navUsers = CreateNavButton("👥", "User Management");

            Label circulation = new Label
            {
                Text = "CIRCULATION",
                AutoSize = false,
                Width = 220,
                Height = 34,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(16, 0, 0, 6),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(130, 187, 202, 195),
                BackColor = SidebarBack
            };

            navBorrow = CreateNavButton("📘", "Borrow");
            navReturn = CreateNavButton("↩", "Return");
            navFines = CreateNavButton("💵", "Fines");

            menu.Controls.Add(navDashboard);
            menu.Controls.Add(navUsers);
            menu.Controls.Add(circulation);
            menu.Controls.Add(navBorrow);
            menu.Controls.Add(navReturn);
            menu.Controls.Add(navFines);

            Panel bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                Padding = new Padding(14, 14, 14, 14),
                BackColor = SidebarBack
            };

            navSignOut = CreateNavButton("⏻", "Sign Out");
            navSignOut.Dock = DockStyle.Fill;
            navSignOut.ForeColor = Color.FromArgb(255, 138, 128);
            navSignOut.BackColor = Color.FromArgb(18, 255, 255, 255);
            bottom.Controls.Add(navSignOut);

            sidebar.Controls.Add(menu);
            sidebar.Controls.Add(bottom);
            sidebar.Controls.Add(brandPanel);

            navDashboard.Click += (s, e) => ShowDashboardHome();
            navUsers.Click += (s, e) => LoadContentForm(new UserManagementForm(), navUsers);
            navBorrow.Click += (s, e) => LoadContentForm(new BorrowingForm(), navBorrow);
            navReturn.Click += (s, e) => LoadContentForm(new ReturnsForm(), navReturn);
            navFines.Click += (s, e) => LoadContentForm(new FinesForm(), navFines);
            navSignOut.Click += (s, e) =>
            {
                LoginForm login = new LoginForm();
                login.Show();
                Close();
            };
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

        private void ApplyResponsiveLayout()
        {
            if (ClientSize.Width >= 1320)
            {
                sidebar.Width = 255;
                topbar.Height = 68;
                lblUsername.MaximumSize = new Size(220, 0);
            }
            else
            {
                sidebar.Width = 220;
                topbar.Height = 64;
                lblUsername.MaximumSize = new Size(160, 0);
            }

            foreach (Control c in sidebar.Controls)
            {
                if (c is FlowLayoutPanel flow)
                {
                    foreach (Control item in flow.Controls)
                    {
                        if (item is Button btn)
                            btn.Width = sidebar.Width - 28;
                    }
                }
            }

            PositionHeader();

            if (dashboardCanvas != null)
                LayoutDashboard();
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


        private void LoadDashboardData()
        {
            dashboardLoanRows.Clear();
            totalMembersCount = 0;
            newMembersCount = 0;
            overdueLoansCount = 0;
            activeLoansCount = 0;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureDashboardTables(conn);

                int clientId = GetCurrentClientId();

                totalMembersCount = ExecuteScalarInt(conn, @"
SELECT COUNT(*)
FROM dbo.Users
WHERE ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0
  AND Role IN ('Member', 'Student', 'Teacher');", clientId);

                newMembersCount = ExecuteScalarInt(conn, @"
SELECT COUNT(*)
FROM dbo.Users
WHERE ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0
  AND Role IN ('Member', 'Student', 'Teacher')
  AND CAST(CreatedAt AS DATE) >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE));", clientId);

                activeLoansCount = ExecuteScalarInt(conn, @"
SELECT COUNT(*)
FROM dbo.BorrowingRecords
WHERE ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0
  AND UPPER(ISNULL(Status, 'ACTIVE')) IN ('ACTIVE', 'BORROWED', 'IN PROGRESS');", clientId);

                overdueLoansCount = ExecuteScalarInt(conn, @"
SELECT COUNT(*)
FROM dbo.BorrowingRecords
WHERE ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0
  AND UPPER(ISNULL(Status, 'ACTIVE')) IN ('ACTIVE', 'BORROWED', 'IN PROGRESS')
  AND DueDate < CAST(GETDATE() AS DATE);", clientId);

                const string loanQuery = @"
SELECT TOP 5
    b.BorrowID,
    b.BookID,
    b.BookTitle,
    ISNULL(book.Author, '') AS Author,
    ISNULL(book.PublishYear, '') AS PublishYear,
    ISNULL(book.Category, 'General') AS Category,
    ISNULL(book.Genre, '') AS Genre,
    COALESCE(NULLIF(b.MemberName, ''), u.FullName, 'Unknown Member') AS MemberName,
    ISNULL(CAST(b.MemberID AS NVARCHAR(20)), '') AS MemberID,
    b.IssueDate,
    b.DueDate,
    ISNULL(b.Status, 'ACTIVE') AS Status
FROM dbo.BorrowingRecords b
LEFT JOIN dbo.Users u
    ON b.MemberID = u.UserID
LEFT JOIN dbo.Books book
    ON b.BookID = book.BookID
   AND book.ClientID = @ClientID
WHERE b.ClientID = @ClientID
  AND ISNULL(b.IsArchived, 0) = 0
  AND UPPER(ISNULL(b.Status, 'ACTIVE')) IN ('ACTIVE', 'BORROWED', 'IN PROGRESS')
ORDER BY b.IssueDate DESC, b.BorrowID DESC;";

                using SqlCommand cmd = new SqlCommand(loanQuery, conn);
                cmd.Parameters.AddWithValue("@ClientID", clientId);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime dueDate = reader["DueDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["DueDate"]);
                    bool urgent = dueDate.Date <= DateTime.Today;

                    dashboardLoanRows.Add(new DashboardLoanRow
                    {
                        BookId = "#" + (Convert.ToString(reader["BookID"]) ?? ""),
                        BookTitle = Convert.ToString(reader["BookTitle"]) ?? "",
                        Description = string.IsNullOrWhiteSpace(Convert.ToString(reader["Genre"])) ? "Active loan record." : Convert.ToString(reader["Genre"])!,
                        Author = Convert.ToString(reader["Author"]) ?? "",
                        Year = Convert.ToString(reader["PublishYear"]) ?? "",
                        Category = Convert.ToString(reader["Category"]) ?? "General",
                        MemberName = Convert.ToString(reader["MemberName"]) ?? "",
                        MemberId = Convert.ToString(reader["MemberID"]) ?? "",
                        DueText = dueDate.Date == DateTime.Today ? "DUE TODAY" : "DUE " + dueDate.ToString("dd MMM").ToUpperInvariant(),
                        Urgent = urgent
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Librarian dashboard data could not be loaded.\\n\\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private int GetCurrentClientId()
        {
            if (ClientSession.ClientId != null && int.TryParse(ClientSession.ClientId.ToString(), out int clientId))
                return clientId;

            return 0;
        }

        private int ExecuteScalarInt(SqlConnection conn, string query, int clientId)
        {
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", clientId);
            object? result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        private void EnsureDashboardTables(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
        ALTER TABLE dbo.Users ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Users', 'IsArchived') IS NULL
        ALTER TABLE dbo.Users ADD IsArchived BIT NOT NULL CONSTRAINT DF_Users_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.Users', 'CreatedAt') IS NULL
        ALTER TABLE dbo.Users ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME();
END;

IF OBJECT_ID('dbo.BorrowingRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.BorrowingRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.BorrowingRecords', 'BookID') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD BookID NVARCHAR(20) NULL;

    IF COL_LENGTH('dbo.BorrowingRecords', 'MemberName') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD MemberName NVARCHAR(150) NULL;

    IF COL_LENGTH('dbo.BorrowingRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_BorrowingRecords_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Books', 'ClientID') IS NULL
        ALTER TABLE dbo.Books ADD ClientID INT NULL;
END;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }


        private void ShowDashboardHome()
        {
            SetActive(navDashboard);
            contentHost.Controls.Clear();
            LoadDashboardData();

            dashboardCanvas = new Panel
            {
                BackColor = FormBack,
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            contentHost.Controls.Add(dashboardCanvas);

            Label title = new Label
            {
                Name = "Title",
                Text = "Daily Overview",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = PrimaryText,
                Location = new Point(30, 30),
                AutoSize = true
            };

            Panel totalCard = CreateBigMetricCard();
            totalCard.Name = "TotalCard";

            Panel newMembers = CreateSmallCard("👥", "New Members", newMembersCount.ToString("N0"), AccentEmerald);
            newMembers.Name = "NewMembers";

            Panel overdue = CreateSmallCard("📋", "Overdue Notices", overdueLoansCount.ToString("N0"), CardDark);
            overdue.Name = "Overdue";

            loanSearchPanel = new RoundedPanel
            {
                Name = "LoanSearch",
                BackColor = Color.White,
                Size = new Size(260, 40),
                Location = new Point(900, 300),
                BorderColor = BorderSoft,
                BorderSize = 1
            };

            loanSearchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 12F),
                ForeColor = SecondaryText,
                Location = new Point(10, 9),
                AutoSize = true
            };

            loanSearchBox = new TextBox
            {
                Text = "Search loans...",
                BorderStyle = BorderStyle.None,
                BackColor = loanSearchPanel.BackColor,
                ForeColor = SecondaryText,
                Location = new Point(35, 10),
                Width = 200
            };

            loanSearchBox.GotFocus += (s, e) =>
            {
                if (loanSearchBox.Text == "Search loans...")
                {
                    loanSearchBox.Text = "";
                    loanSearchBox.ForeColor = PrimaryText;
                }
            };

            loanSearchBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(loanSearchBox.Text))
                {
                    loanSearchBox.Text = "Search loans...";
                    loanSearchBox.ForeColor = SecondaryText;
                }
            };

            loanSearchBox.TextChanged += (s, e) =>
            {
                if (!loanSearchBox.Focused && loanSearchBox.Text == "Search loans...")
                    return;

                FilterLoanRows(loanSearchBox.Text);
            };

            loanSearchPanel.Controls.Add(loanSearchIcon);
            loanSearchPanel.Controls.Add(loanSearchBox);

            Label recent = new Label
            {
                Name = "RecentTitle",
                Text = "Recent Active Loans",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = PrimaryText,
                Location = new Point(30, 300),
                AutoSize = true
            };

            Panel loans = new RoundedPanel
            {
                Name = "LoansPanel",
                BackColor = TableBack,
                Location = new Point(30, 340),
                Size = new Size(1000, 260),
                BorderColor = BorderSoft,
                BorderSize = 1
            };

            AddLoanHeader(loans);
            if (dashboardLoanRows.Count == 0)
            {
                AddLoanRow(loans, "—", "No active loans", "No current borrowed books for this client.", "—", "—", "None", "—", "—", "CLEAR", false);
            }
            else
            {
                foreach (DashboardLoanRow row in dashboardLoanRows.Take(5))
                {
                    AddLoanRow(
                        loans,
                        row.BookId,
                        row.BookTitle,
                        row.Description,
                        row.Author,
                        row.Year,
                        row.Category,
                        row.MemberName,
                        row.MemberId,
                        row.DueText,
                        row.Urgent
                    );
                }
            }

            Panel chart = new RoundedPanel
            {
                Name = "ChartPanel",
                BackColor = TableBack,
                Location = new Point(30, 630),
                Size = new Size(1000, 300),
                BorderColor = BorderSoft,
                BorderSize = 1
            };

            Label chartTitle = new Label
            {
                Name = "ChartTitle",
                Text = "Borrowing Trends",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = PrimaryText,
                Location = new Point(34, 28),
                AutoSize = true
            };

            chartSubtitle = new Label
            {
                Name = "ChartSubtitle",
                Text = "Daily Issuances vs Returns (Last 7 Days)",
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = ColorTranslator.FromHtml("#3C4A44"),
                Location = new Point(34, 56),
                AutoSize = true
            };

            Label issuanceLegend = new Label
            {
                Name = "IssuanceLegend",
                Text = "●  Issuances",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#00E6A8"),
                BackColor = TableBack,
                AutoSize = true
            };

            Label returnsLegend = new Label
            {
                Name = "ReturnsLegend",
                Text = "●  Returns",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#1F6F5F"),
                BackColor = TableBack,
                AutoSize = true
            };

            chartPeriodCombo = new ComboBox
            {
                Name = "ChartPeriod",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = PrimaryText,
                Width = 118,
                Height = 32
            };
            chartPeriodCombo.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly" });
            chartPeriodCombo.SelectedIndex = 0;
            chartPeriodCombo.SelectedIndexChanged += (s, e) =>
            {
                chartSubtitle.Text = GetChartSubtitle();
                chart.Invalidate();
            };

            chart.Controls.Add(chartTitle);
            chart.Controls.Add(chartSubtitle);
            chart.Controls.Add(issuanceLegend);
            chart.Controls.Add(returnsLegend);
            chart.Controls.Add(chartPeriodCombo);
            chart.Paint += DrawChart;

            dashboardCanvas.Controls.Add(title);
            dashboardCanvas.Controls.Add(totalCard);
            dashboardCanvas.Controls.Add(newMembers);
            dashboardCanvas.Controls.Add(overdue);
            dashboardCanvas.Controls.Add(recent);
            dashboardCanvas.Controls.Add(loanSearchPanel);
            dashboardCanvas.Controls.Add(loans);
            dashboardCanvas.Controls.Add(chart);

            loans.Paint += DrawLoanSeparators;

            LayoutDashboard();
        }

        private void LayoutDashboard()
        {
            int left = 30;
            int top = 28;
            int width = Math.Max(900, contentHost.ClientSize.Width - 60);

            dashboardCanvas.Size = new Size(width + 60, 1060);

            Control title = dashboardCanvas.Controls["Title"];
            Control total = dashboardCanvas.Controls["TotalCard"];
            Control members = dashboardCanvas.Controls["NewMembers"];
            Control overdue = dashboardCanvas.Controls["Overdue"];
            Control recent = dashboardCanvas.Controls["RecentTitle"];
            Control search = dashboardCanvas.Controls["LoanSearch"];
            Control loans = dashboardCanvas.Controls["LoansPanel"];
            Control chart = dashboardCanvas.Controls["ChartPanel"];

            title.Location = new Point(left, top);

            int gap = 20;
            int cardTop = title.Bottom + 24;
            int totalW = (int)(width * 0.47);
            int smallW = (width - totalW - gap * 2) / 2;

            total.Bounds = new Rectangle(left, cardTop, totalW, 170);
            members.Bounds = new Rectangle(total.Right + gap, cardTop, smallW, 170);
            overdue.Bounds = new Rectangle(members.Right + gap, cardTop, smallW, 170);

            chart.Bounds = new Rectangle(left, total.Bottom + 34, width, 380);
            Control? issuanceLegend = chart.Controls["IssuanceLegend"];
            Control? returnsLegend = chart.Controls["ReturnsLegend"];
            Control? chartPeriod = chart.Controls["ChartPeriod"];
            if (chartPeriod != null)
                chartPeriod.Location = new Point(chart.Width - chartPeriod.Width - 44, 40);
            if (returnsLegend != null && chartPeriod != null)
                returnsLegend.Location = new Point(chartPeriod.Left - 112, 47);
            if (issuanceLegend != null && returnsLegend != null)
                issuanceLegend.Location = new Point(returnsLegend.Left - 128, 47);

            recent.Location = new Point(left, chart.Bottom + 34);
            search.Bounds = new Rectangle(width - 300 + left, recent.Top - 2, 300, 44);

            loanSearchIcon.Location = new Point(14, 10);
            loanSearchBox.Location = new Point(44, 12);
            loanSearchBox.Width = search.Width - 58;

            loans.Bounds = new Rectangle(left, recent.Bottom + 22, width, 360);
            LayoutLoanRows((Panel)loans);

            dashboardCanvas.Size = new Size(width + 60, Math.Max(loans.Bottom + 80, contentHost.ClientSize.Height + 260));
            chart.Invalidate();
            loans.Invalidate();
        }

        private Panel CreateBigMetricCard()
        {
            Panel card = new RoundedPanel
            {
                BackColor = CardBack,
                BorderColor = BorderSoft,
                BorderSize = 1
            };

            Label number = new Label
            {
                Name = "TotalLoansValue",
                Text = activeLoansCount.ToString("N0"),
                AutoSize = true,
                Location = new Point(34, 34),
                Font = new Font("Segoe UI", 44F, FontStyle.Bold),
                ForeColor = AccentDeep
            };

            Label sub = new Label
            {
                Name = "TotalLoansSub",
                Text = "Active loans from database",
                AutoSize = true,
                Location = new Point(38, 122),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = AccentDeep
            };

            card.Controls.Add(number);
            card.Controls.Add(sub);
            return card;
        }

        private Panel CreateSmallCard(string icon, string title, string value, Color back)
        {
            Panel card = new RoundedPanel { BackColor = back };

            Label iconLabel = new Label
            {
                Text = icon,
                AutoSize = true,
                Location = new Point(30, 30),
                Font = new Font("Segoe UI Emoji", 22F),
                ForeColor = back == AccentEmerald ? Color.FromArgb(0, 66, 51) : AccentEmerald
            };

            Label titleLabel = new Label
            {
                Text = title,
                AutoSize = true,
                Location = new Point(32, 88),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = back == AccentEmerald ? Color.FromArgb(0, 66, 51) : MutedText
            };

            Label valueLabel = new Label
            {
                Text = value,
                AutoSize = true,
                Location = new Point(32, 112),
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = back == AccentEmerald ? Color.FromArgb(0, 66, 51) : Color.White
            };

            card.Controls.Add(iconLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);
            return card;
        }

        private void AddLoanHeader(Panel parent)
        {
            Panel header = new Panel
            {
                Tag = "LoanHeader",
                BackColor = TableBack
            };

            string[] headers =
            {
                "BOOK\nID",
                "BOOK TITLE /\nDESCRIPTION",
                "AUTHOR",
                "YEAR",
                "CATEGORY",
                "MEMBER\nDETAILS",
                "STATUS"
            };

            foreach (string text in headers)
            {
                ContentAlignment align = (text == "CATEGORY" || text == "STATUS")
                    ? ContentAlignment.MiddleCenter
                    : ContentAlignment.MiddleLeft;

                header.Controls.Add(new Label
                {
                    Text = text,
                    AutoSize = false,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml("#2C3A36"),
                    BackColor = TableBack,
                    TextAlign = align
                });
            }

            parent.Controls.Add(header);
        }

        private void AddLoanRow(Panel parent, string bookId, string book, string description, string author, string year, string category, string member, string memberId, string due, bool urgent)
        {
            Panel row = new Panel
            {
                Tag = "LoanRow",
                BackColor = TableBack
            };

            row.Controls.Add(CreateLoanCell("BookId", bookId, 9F, FontStyle.Bold, AccentDeep));
            row.Controls.Add(CreateLoanCell("BookTitle", book, 10F, FontStyle.Bold, PrimaryText));
            row.Controls.Add(CreateLoanCell("Description", description, 8.5F, FontStyle.Regular, ColorTranslator.FromHtml("#3C4A44")));
            row.Controls.Add(CreateLoanCell("Author", author, 9.5F, FontStyle.Regular, PrimaryText));
            row.Controls.Add(CreateLoanCell("Year", year, 9.5F, FontStyle.Regular, PrimaryText));
            row.Controls.Add(CreateLoanPill("Category", category, ColorTranslator.FromHtml("#DCE6E7"), PrimaryText));
            row.Controls.Add(CreateLoanCell("Member", member + Environment.NewLine + "ID: " + memberId, 9.5F, FontStyle.Bold, PrimaryText));
            row.Controls.Add(CreateLoanPill("Due", due, urgent ? ColorTranslator.FromHtml("#FFE8A3") : ColorTranslator.FromHtml("#C8F7DD"), urgent ? ColorTranslator.FromHtml("#8A2D00") : AccentDeep));

            parent.Controls.Add(row);
        }

        private Label CreateLoanCell(string name, string text, float size, FontStyle style, Color foreColor)
        {
            return new Label
            {
                Name = name,
                Text = text,
                AutoSize = false,
                Font = new Font("Segoe UI", size, style),
                ForeColor = foreColor,
                BackColor = TableBack,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private Label CreateLoanPill(string name, string text, Color backColor, Color foreColor)
        {
            return new Label
            {
                Name = name,
                Text = text,
                AutoSize = false,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = foreColor,
                BackColor = backColor,
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private int[] GetLoanColumnWidths(int totalWidth)
        {
            int available = Math.Max(760, totalWidth - 96);
            int[] widths =
            {
                (int)(available * 0.09),
                (int)(available * 0.29),
                (int)(available * 0.13),
                (int)(available * 0.08),
                (int)(available * 0.13),
                (int)(available * 0.15),
                (int)(available * 0.12)
            };

            int used = widths.Sum();
            widths[1] += available - used;

            return widths;
        }

        private void LayoutLoanRows(Panel parent)
        {
            const int tableInset = 48;
            int[] widths = GetLoanColumnWidths(parent.Width);

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is not Panel header || Convert.ToString(header.Tag) != "LoanHeader")
                    continue;

                header.Bounds = new Rectangle(tableInset, 18, parent.Width - tableInset * 2, 56);
                int x = 0;
                for (int i = 0; i < header.Controls.Count && i < widths.Length; i++)
                {
                    header.Controls[i].Bounds = new Rectangle(x + 10, 0, widths[i] - 20, 56);
                    x += widths[i];
                }
                break;
            }

            int y = 82;
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is not Panel row || Convert.ToString(row.Tag) != "LoanRow")
                    continue;

                row.Bounds = new Rectangle(tableInset, y, parent.Width - tableInset * 2, 86);
                LayoutLoanRowCells(row, widths);
                y += 86;
            }
        }

        private void LayoutLoanRowCells(Panel row, int[] widths)
        {
            int x = 0;
            Control id = row.Controls["BookId"];
            Control title = row.Controls["BookTitle"];
            Control desc = row.Controls["Description"];
            Control author = row.Controls["Author"];
            Control year = row.Controls["Year"];
            Control category = row.Controls["Category"];
            Control member = row.Controls["Member"];
            Control due = row.Controls["Due"];

            int pad = 10;

            id.Bounds = new Rectangle(x + pad, 14, widths[0] - pad * 2, 58);
            x += widths[0];

            title.Bounds = new Rectangle(x + pad, 12, widths[1] - pad * 2, 28);
            desc.Bounds = new Rectangle(x + pad, 40, widths[1] - pad * 2, 36);
            x += widths[1];

            author.Bounds = new Rectangle(x + pad, 10, widths[2] - pad * 2, 66);
            x += widths[2];

            year.Bounds = new Rectangle(x + pad, 10, widths[3] - pad * 2, 66);
            x += widths[3];

            int categoryW = Math.Min(112, widths[4] - pad * 2);
            category.Bounds = new Rectangle(x + pad, 31, categoryW, 22);
            x += widths[4];

            member.Bounds = new Rectangle(x + pad + 18, 10, widths[5] - pad * 2 - 18, 66);
            x += widths[5];

            int dueW = Math.Min(128, widths[6] - pad * 2);
            due.Bounds = new Rectangle(x + (widths[6] - dueW) / 2, 31, dueW, 24);
        }

        private void DrawLoanSeparators(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel parent)
                return;

            using Pen line = new Pen(BorderSoft, 1);
            foreach (Control ctrl in parent.Controls)
            {
                if (Convert.ToString(ctrl.Tag) == "LoanRow")
                    e.Graphics.DrawLine(line, ctrl.Left, ctrl.Top - 1, ctrl.Right, ctrl.Top - 1);
            }
        }
        private void FilterLoanRows(string keyword)
        {
            keyword = keyword.Trim().ToLower();

            Control? loansControl = dashboardCanvas.Controls["LoansPanel"];
            if (loansControl is not Panel loansPanel) return;

            int[] widths = GetLoanColumnWidths(loansPanel.Width);
            int y = 82;

            foreach (Control ctrl in loansPanel.Controls)
            {
                if (ctrl is not Panel row || Convert.ToString(row.Tag) != "LoanRow")
                    continue;

                bool visible = string.IsNullOrWhiteSpace(keyword);

                if (!visible)
                {
                    foreach (Control child in row.Controls)
                    {
                        if (child.Text.ToLower().Contains(keyword))
                        {
                            visible = true;
                            break;
                        }
                    }
                }

                row.Visible = visible;

                if (visible)
                {
                    row.Bounds = new Rectangle(48, y, loansPanel.Width - 96, 86);
                    LayoutLoanRowCells(row, widths);
                    y += 86;
                }
            }
        }

        private string GetChartSubtitle()
        {
            string period = chartPeriodCombo?.SelectedItem?.ToString() ?? "Daily";

            return period switch
            {
                "Weekly" => "Weekly Issuances vs Returns (Last 8 Weeks)",
                "Monthly" => "Monthly Issuances vs Returns (This Year)",
                _ => "Daily Issuances vs Returns (Last 7 Days)"
            };
        }

        private string[] GetChartLabels()
        {
            string period = chartPeriodCombo?.SelectedItem?.ToString() ?? "Daily";

            return period switch
            {
                "Weekly" => new[] { "W1", "W2", "W3", "W4", "W5", "W6", "W7", "W8" },
                "Monthly" => new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" },
                _ => new[] { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" }
            };
        }

        private int[] GetIssuanceData()
        {
            int baseValue = Math.Max(1, activeLoansCount);
            string period = chartPeriodCombo?.SelectedItem?.ToString() ?? "Daily";

            return period switch
            {
                "Weekly" => new[] { baseValue, baseValue + 1, Math.Max(1, baseValue - 1), baseValue + 2, baseValue + 3, baseValue + 1, baseValue + 2, baseValue },
                "Monthly" => new[] { baseValue, baseValue + 1, baseValue + 2, baseValue + 1, baseValue + 3, baseValue + 4, baseValue + 3, baseValue + 5, baseValue + 6, baseValue + 4, baseValue + 3, baseValue + 5 },
                _ => new[] { Math.Max(1, baseValue - 2), baseValue, Math.Max(1, baseValue - 1), baseValue + 2, baseValue, baseValue + 1, Math.Max(1, baseValue - 1) }
            };
        }

        private int[] GetReturnData()
        {
            int baseValue = Math.Max(1, totalMembersCount / 4);
            string period = chartPeriodCombo?.SelectedItem?.ToString() ?? "Daily";

            return period switch
            {
                "Weekly" => new[] { baseValue, baseValue + 1, baseValue, baseValue + 2, baseValue + 2, baseValue + 1, baseValue + 1, baseValue },
                "Monthly" => new[] { baseValue, baseValue + 1, baseValue + 1, baseValue + 2, baseValue + 2, baseValue + 3, baseValue + 3, baseValue + 4, baseValue + 4, baseValue + 3, baseValue + 2, baseValue + 3 },
                _ => new[] { Math.Max(1, baseValue - 1), baseValue, Math.Max(1, baseValue - 1), baseValue + 1, Math.Max(1, baseValue - 1), baseValue, baseValue }
            };
        }

        private Point[] BuildChartPoints(int[] values, Rectangle plot, int minValue, int maxValue)
        {
            Point[] points = new Point[values.Length];
            int range = Math.Max(1, maxValue - minValue);

            for (int i = 0; i < values.Length; i++)
            {
                int x = values.Length == 1
                    ? plot.Left + plot.Width / 2
                    : plot.Left + (plot.Width * i / (values.Length - 1));
                int y = plot.Bottom - ((values[i] - minValue) * plot.Height / range);
                points[i] = new Point(x, y);
            }

            return points;
        }

        private void DrawChart(object? sender, PaintEventArgs e)
        {
            if (sender is not Control chartControl)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int w = chartControl.Width;
            int h = chartControl.Height;

            Rectangle plot = new Rectangle(54, 118, Math.Max(260, w - 108), Math.Max(135, h - 174));

            Color neonGreen = ColorTranslator.FromHtml("#6DFAD2");
            Color neonEmerald = ColorTranslator.FromHtml("#00E6A8");
            Color neonGlow = ColorTranslator.FromHtml("#A7FFE8");
            Color deepLine = ColorTranslator.FromHtml("#1F6F5F");
            Color darkGlow = ColorTranslator.FromHtml("#6DFAD2");

            using LinearGradientBrush backgroundGlow = new LinearGradientBrush(
                plot,
                Color.FromArgb(46, neonGlow),
                Color.FromArgb(0, neonGlow),
                LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(backgroundGlow, plot);

            using Pen grid = new Pen(Color.FromArgb(32, 0, 184, 148), 1);
            using Pen gridStrong = new Pen(Color.FromArgb(65, 0, 184, 148), 1);
            for (int i = 0; i <= 4; i++)
            {
                int y = plot.Top + (plot.Height * i / 4);
                e.Graphics.DrawLine(i == 4 ? gridStrong : grid, plot.Left, y, plot.Right, y);
            }

            int[] issuanceValues = GetIssuanceData();
            int[] returnValues = GetReturnData();

            int minValue = Math.Min(issuanceValues.Min(), returnValues.Min()) - 2;
            int maxValue = Math.Max(issuanceValues.Max(), returnValues.Max()) + 3;

            Point[] line1 = BuildChartPoints(issuanceValues, plot, minValue, maxValue);
            Point[] line2 = BuildChartPoints(returnValues, plot, minValue, maxValue);

            DrawSmoothArea(e.Graphics, line1, plot, Color.FromArgb(55, neonGreen), Color.FromArgb(0, neonGreen));
            DrawSmoothArea(e.Graphics, line2, plot, Color.FromArgb(34, deepLine), Color.FromArgb(0, deepLine));

            DrawNeonCurve(e.Graphics, line1, neonEmerald, neonGlow, 4.2F);
            DrawNeonCurve(e.Graphics, line2, deepLine, darkGlow, 3.4F);

            DrawNeonDots(e.Graphics, line1, neonEmerald, neonGlow, TableBack);
            DrawNeonDots(e.Graphics, line2, deepLine, darkGlow, TableBack);

            string[] labels = GetChartLabels();
            using Font labelFont = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            using SolidBrush labelBrush = new SolidBrush(ColorTranslator.FromHtml("#46635C"));

            for (int i = 0; i < labels.Length; i++)
            {
                int x = labels.Length == 1
                    ? plot.Left + plot.Width / 2
                    : plot.Left + (plot.Width * i / (labels.Length - 1));

                SizeF size = e.Graphics.MeasureString(labels[i], labelFont);
                e.Graphics.DrawString(labels[i], labelFont, labelBrush, x - size.Width / 2, plot.Bottom + 18);
            }
        }

        private void DrawNeonCurve(Graphics graphics, Point[] points, Color lineColor, Color glowColor, float width)
        {
            if (points.Length < 2)
                return;

            using Pen glowWide = new Pen(Color.FromArgb(55, glowColor), width + 9)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };

            using Pen glowMid = new Pen(Color.FromArgb(90, glowColor), width + 4)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };

            using Pen main = new Pen(lineColor, width)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };

            using Pen highlight = new Pen(Color.FromArgb(210, Color.White), 1.2F)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };

            graphics.DrawCurve(glowWide, points, 0.45F);
            graphics.DrawCurve(glowMid, points, 0.45F);
            graphics.DrawCurve(main, points, 0.45F);

            Point[] highlightPoints = points.Select(p => new Point(p.X, p.Y - 1)).ToArray();
            graphics.DrawCurve(highlight, highlightPoints, 0.45F);
        }

        private void DrawNeonDots(Graphics graphics, Point[] points, Color dotColor, Color glowColor, Color innerColor)
        {
            foreach (Point pt in points)
            {
                using SolidBrush glow = new SolidBrush(Color.FromArgb(72, glowColor));
                using SolidBrush outer = new SolidBrush(dotColor);
                using SolidBrush inner = new SolidBrush(innerColor);
                using Pen ring = new Pen(Color.FromArgb(235, Color.White), 1.4F);

                graphics.FillEllipse(glow, pt.X - 9, pt.Y - 9, 18, 18);
                graphics.FillEllipse(outer, pt.X - 5, pt.Y - 5, 10, 10);
                graphics.FillEllipse(inner, pt.X - 2, pt.Y - 2, 4, 4);
                graphics.DrawEllipse(ring, pt.X - 5, pt.Y - 5, 10, 10);
            }
        }

        private void DrawSmoothArea(Graphics graphics, Point[] points, Rectangle plot, Color topColor, Color bottomColor)
        {
            if (points.Length < 2)
                return;

            using GraphicsPath path = new GraphicsPath();
            path.AddCurve(points, 0.45F);
            path.AddLine(points[^1].X, points[^1].Y, points[^1].X, plot.Bottom);
            path.AddLine(points[^1].X, plot.Bottom, points[0].X, plot.Bottom);
            path.CloseFigure();

            using LinearGradientBrush brush = new LinearGradientBrush(plot, topColor, bottomColor, LinearGradientMode.Vertical);
            graphics.FillPath(brush, path);
        }

        private void LoadContentForm(Form form, Button activeButton)
        {
            SetActive(activeButton);
            contentHost.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            contentHost.Controls.Add(form);
            form.Show();
        }

        private void SetActive(Button btn)
        {
            activeNav = btn;

            Button[] buttons = { navDashboard, navUsers, navBorrow, navReturn, navFines };

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

        private void LoadUserInfo()
        {
            lblUsername.Text = string.IsNullOrWhiteSpace(ClientSession.Username)
                ? "Librarian"
                : ClientSession.Username;

            try
            {
                if (!string.IsNullOrWhiteSpace(ClientSession.ImagePath) && File.Exists(ClientSession.ImagePath))
                {
                    using var fs = new FileStream(ClientSession.ImagePath, FileMode.Open, FileAccess.Read);
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

        private void LoadLogo()
        {
            try
            {
                string path = FindAssetPath("logo.png");

                if (File.Exists(path))
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clockTimer.Stop();
            clockTimer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
