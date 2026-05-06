using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class MonitoringForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color SurfaceLowest = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#E2E9EC");
        private readonly Color SurfaceVariant = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color PrimaryFixed = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Tertiary = ColorTranslator.FromHtml("#A03F30");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");

        private readonly Color InverseSurface = ColorTranslator.FromHtml("#2B3234");
        private readonly Color InverseOnSurface = ColorTranslator.FromHtml("#EBF2F4");

        private Panel canvasPanel = null!;

        private Label lblPageTitle = null!;
        private Label lblPageSub = null!;
        private Panel searchBox = null!;
        private TextBox txtSearch = null!;

        private Panel activityPanel = null!;
        private DataGridView dgvActivity = null!;

        private Label lblVitalsTitle = null!;
        private Label lblVitalsSub = null!;

        private Panel cardUsers = null!;
        private Panel cardStock = null!;
        private Panel cardFees = null!;

        private Label lblUsersValue = null!;
        private Label lblUsersTag = null!;
        private Label lblStockValue = null!;
        private Label lblFeesValue = null!;
        private Label lblFeesFooter = null!;

        private Panel usersBars = null!;
        private Panel stockProgress = null!;

        private Panel reportPanel = null!;
        private Panel reportImage = null!;
        private Label lblReportTitle = null!;
        private Label lblReportBody = null!;
        private Button btnDeepDive = null!;

        private Panel maintenancePanel = null!;
        private Label lblMaintenanceTitle = null!;
        private Label lblMaintenanceBody = null!;
        private Label lblMaintenanceChip1 = null!;
        private Label lblMaintenanceChip2 = null!;
        private Label lblMaintenanceIcon = null!;

        private readonly List<ActivityItem> activityItems = new List<ActivityItem>();

        public MonitoringForm()
        {
            InitializeComponent();
            BuildMonitoringUI();
            LoadMonitoringDataFromDatabase();
        }

        private void BuildMonitoringUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = true;

            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background
            };
            Controls.Add(canvasPanel);

            lblPageTitle = new Label
            {
                Text = "Activity Feed",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblPageSub = new Label
            {
                Text = "Real-time book transactions across all connected libraries",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            searchBox = new Panel
            {
                BackColor = SurfaceHigh
            };

            Label searchIcon = new Label
            {
                Text = "⌕",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 14F),
                ForeColor = OnSurfaceVariant,
                BackColor = Color.Transparent
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = SurfaceHigh,
                ForeColor = OnSurfaceVariant,
                Font = new Font("Segoe UI", 10.5F),
                Text = "Search activity logs..."
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search activity logs...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search activity logs...";
                    txtSearch.ForeColor = OnSurfaceVariant;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.Text == "Search activity logs...") return;
                LoadActivityRows(txtSearch.Text);
            };

            searchBox.Controls.Add(searchIcon);
            searchBox.Controls.Add(txtSearch);

            activityPanel = CreateCardPanel();
            dgvActivity = new DataGridView
            {
                BackgroundColor = SurfaceLowest,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 48,
                GridColor = SurfaceVariant,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                Dock = DockStyle.Fill
            };
            dgvActivity.RowTemplate.Height = 62;

            dgvActivity.ColumnHeadersDefaultCellStyle.BackColor = SurfaceLow;
            dgvActivity.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvActivity.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvActivity.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceLow;
            dgvActivity.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;

            dgvActivity.DefaultCellStyle.BackColor = SurfaceLowest;
            dgvActivity.DefaultCellStyle.ForeColor = OnSurface;
            dgvActivity.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvActivity.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 250, 247);
            dgvActivity.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvActivity.Columns.Add("Time", "TIME");
            dgvActivity.Columns.Add("Member", "MEMBER");
            dgvActivity.Columns.Add("School", "SCHOOL");
            dgvActivity.Columns.Add("BookTitle", "BOOK TITLE");
            dgvActivity.Columns.Add("Activity", "ACTIVITY");
            dgvActivity.Columns.Add("Status", "STATUS");

            dgvActivity.Columns["Time"].FillWeight = 12;
            dgvActivity.Columns["Member"].FillWeight = 22;
            dgvActivity.Columns["School"].FillWeight = 22;
            dgvActivity.Columns["BookTitle"].FillWeight = 26;
            dgvActivity.Columns["Activity"].FillWeight = 16;
            dgvActivity.Columns["Status"].FillWeight = 14;

            foreach (DataGridViewColumn col in dgvActivity.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvActivity.CellPainting += dgvActivity_CellPainting;
            activityPanel.Controls.Add(dgvActivity);

            lblVitalsTitle = new Label
            {
                Text = "System Vitals",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblVitalsSub = new Label
            {
                Text = "Key performance indicators for the current session",
                Font = new Font("Segoe UI", 11F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            cardUsers = CreateCardPanel();
            cardStock = CreateCardPanel();
            cardFees = new Panel
            {
                BackColor = InverseSurface,
                BorderStyle = BorderStyle.FixedSingle
            };

            BuildUsersCard();
            BuildStockCard();
            BuildFeesCard();
            BuildBottomPanels();

            canvasPanel.Controls.Add(lblPageTitle);
            canvasPanel.Controls.Add(lblPageSub);
            canvasPanel.Controls.Add(searchBox);
            canvasPanel.Controls.Add(activityPanel);
            canvasPanel.Controls.Add(lblVitalsTitle);
            canvasPanel.Controls.Add(lblVitalsSub);
            canvasPanel.Controls.Add(cardUsers);
            canvasPanel.Controls.Add(cardStock);
            canvasPanel.Controls.Add(cardFees);
            canvasPanel.Controls.Add(reportPanel);
            canvasPanel.Controls.Add(maintenancePanel);

            Resize += MonitoringForm_Resize;
            AdjustLayout();
        }

        private void BuildUsersCard()
        {
            Panel icon = CreateIconPanel("USR", Color.FromArgb(235, 250, 245), Primary);
            icon.Location = new Point(24, 26);

            lblUsersTag = new Label
            {
                Text = "DATABASE LIVE",
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 140, 100),
                BackColor = Color.FromArgb(235, 250, 245),
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(205, 28)
            };

            Label title = new Label
            {
                Text = "TOTAL ACTIVE USERS",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                Location = new Point(24, 108)
            };

            lblUsersValue = new Label
            {
                Text = "0",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                Location = new Point(24, 128)
            };

            usersBars = new Panel
            {
                BackColor = SurfaceLowest,
                Location = new Point(24, 196),
                Size = new Size(260, 14)
            };

            cardUsers.Controls.Add(icon);
            cardUsers.Controls.Add(lblUsersTag);
            cardUsers.Controls.Add(title);
            cardUsers.Controls.Add(lblUsersValue);
            cardUsers.Controls.Add(usersBars);
        }

        private void BuildStockCard()
        {
            Panel icon = CreateIconPanel("BK", Color.FromArgb(235, 250, 245), Primary);
            icon.Location = new Point(24, 26);

            Label title = new Label
            {
                Text = "BOOK STOCK",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                Location = new Point(24, 108)
            };

            lblStockValue = new Label
            {
                Text = "0%",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                Location = new Point(24, 126)
            };

            Label lblSuffix = new Label
            {
                Text = "Checked out",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = OnSurface,
                Location = new Point(170, 145)
            };

            stockProgress = new Panel
            {
                BackColor = SurfaceHigh,
                Location = new Point(24, 202),
                Size = new Size(270, 12)
            };

            cardStock.Controls.Add(icon);
            cardStock.Controls.Add(title);
            cardStock.Controls.Add(lblStockValue);
            cardStock.Controls.Add(lblSuffix);
            cardStock.Controls.Add(stockProgress);
        }

        private void BuildFeesCard()
        {
            Panel glow = new Panel
            {
                BackColor = Color.FromArgb(25, 0, 184, 148),
                Size = new Size(120, 120),
                Location = new Point(220, 130)
            };
            glow.Paint += (s, e) =>
            {
                using SolidBrush b = new SolidBrush(Color.FromArgb(22, 0, 184, 148));
                e.Graphics.FillEllipse(b, 0, 0, glow.Width, glow.Height);
            };

            Panel icon = CreateIconPanel("₱", Color.FromArgb(24, 0, 184, 148), PrimaryFixed);
            icon.Location = new Point(24, 26);

            Label lblLive = new Label
            {
                Text = "↗ Live",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = PrimaryFixed,
                Location = new Point(250, 34)
            };

            Label title = new Label
            {
                Text = "FEES CALCULATED TODAY",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(135, 150, 160),
                Location = new Point(24, 110)
            };

            lblFeesValue = new Label
            {
                Text = "₱0.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryFixed,
                Location = new Point(24, 132)
            };

            lblFeesFooter = new Label
            {
                Text = "UPDATED JUST NOW  ⟳",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 130, 140),
                Location = new Point(24, 204)
            };

            cardFees.Controls.Add(glow);
            cardFees.Controls.Add(icon);
            cardFees.Controls.Add(lblLive);
            cardFees.Controls.Add(title);
            cardFees.Controls.Add(lblFeesValue);
            cardFees.Controls.Add(lblFeesFooter);
            glow.SendToBack();
        }

        private void BuildBottomPanels()
        {
            reportPanel = new Panel
            {
                BackColor = SurfaceLow,
                BorderStyle = BorderStyle.FixedSingle
            };

            reportImage = new Panel
            {
                BackColor = InverseSurface,
                BorderStyle = BorderStyle.FixedSingle
            };
            reportImage.Paint += (s, e) =>
            {
                using SolidBrush b = new SolidBrush(Color.FromArgb(12, 0, 184, 148));
                e.Graphics.FillRectangle(b, 0, 0, reportImage.Width, reportImage.Height);

                using Pen p = new Pen(Color.FromArgb(40, 109, 250, 210), 2);
                e.Graphics.DrawRectangle(p, 20, 20, reportImage.Width - 40, reportImage.Height - 40);
                e.Graphics.DrawLine(p, 20, reportImage.Height - 30, reportImage.Width - 20, reportImage.Height - 30);
                e.Graphics.DrawLine(p, 45, 30, 45, reportImage.Height - 30);
            };

            lblReportTitle = new Label
            {
                Text = "Weekly Intelligence\nReport",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = OnSurface
            };

            lblReportBody = new Label
            {
                Text = "System performance is currently loading real data...",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = OnSurfaceVariant
            };

            btnDeepDive = new Button
            {
                Text = "REFRESH MONITORING",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Primary,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDeepDive.FlatAppearance.BorderSize = 0;
            btnDeepDive.Click += (s, e) => LoadMonitoringDataFromDatabase();

            reportPanel.Controls.Add(reportImage);
            reportPanel.Controls.Add(lblReportTitle);
            reportPanel.Controls.Add(lblReportBody);
            reportPanel.Controls.Add(btnDeepDive);

            maintenancePanel = new Panel
            {
                BackColor = PrimaryContainer,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblMaintenanceTitle = new Label
            {
                Text = "Automated Maintenance",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#004233")
            };

            lblMaintenanceBody = new Label
            {
                Text = "Database monitoring is active. No administrator action required.",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = ColorTranslator.FromHtml("#004233")
            };

            lblMaintenanceChip1 = CreateChip("DATABASE");
            lblMaintenanceChip2 = CreateChip("MONITORING");

            lblMaintenanceIcon = new Label
            {
                Text = "✦",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 60F),
                ForeColor = Color.FromArgb(20, 0, 66, 51)
            };

            maintenancePanel.Controls.Add(lblMaintenanceTitle);
            maintenancePanel.Controls.Add(lblMaintenanceBody);
            maintenancePanel.Controls.Add(lblMaintenanceChip1);
            maintenancePanel.Controls.Add(lblMaintenanceChip2);
            maintenancePanel.Controls.Add(lblMaintenanceIcon);
        }

        private Panel CreateCardPanel()
        {
            return new Panel
            {
                BackColor = SurfaceLowest,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Panel CreateIconPanel(string text, Color back, Color fore)
        {
            Panel panel = new Panel
            {
                Size = new Size(52, 52),
                BackColor = back
            };

            Label label = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = fore
            };

            panel.Controls.Add(label);
            return panel;
        }

        private Label CreateChip(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#004233"),
                BackColor = Color.FromArgb(35, 0, 66, 51),
                Padding = new Padding(10, 5, 10, 5)
            };
        }

        private void LoadMonitoringDataFromDatabase()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureMonitoringSchema(conn);

                int activeUsers = GetScalarInt(conn, @"
SELECT COUNT(*)
FROM dbo.Users
WHERE ISNULL(IsArchived, 0) = 0
  AND UPPER(ISNULL(Status, 'ACTIVE')) = 'ACTIVE';");

                int totalBooks = GetScalarInt(conn, @"
SELECT COUNT(*)
FROM dbo.Books
WHERE ISNULL(IsArchived, 0) = 0;");

                int checkedOutBooks = GetScalarInt(conn, @"
SELECT COUNT(*)
FROM dbo.BorrowingRecords
WHERE ISNULL(IsArchived, 0) = 0
  AND UPPER(ISNULL(Status, 'ACTIVE')) IN ('ACTIVE', 'BORROWED', 'OVERDUE');");

                decimal feesToday = GetScalarDecimal(conn, @"
SELECT ISNULL(SUM(Amount), 0)
FROM dbo.FineRecords
WHERE ISNULL(IsArchived, 0) = 0
  AND CAST(COALESCE(CreatedAt, GETDATE()) AS DATE) = CAST(GETDATE() AS DATE);");

                int totalBorrow = GetScalarInt(conn, "SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0;");
                int totalReturns = GetScalarInt(conn, "SELECT COUNT(*) FROM dbo.ReturnRecords WHERE ISNULL(IsArchived, 0) = 0;");
                int totalFines = GetScalarInt(conn, "SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0;");

                lblUsersValue.Text = activeUsers.ToString("N0");

                int rawStockPercent = totalBooks == 0 ? 0 : (int)Math.Round((double)checkedOutBooks / totalBooks * 100);
                int stockPercent = Math.Max(0, Math.Min(100, rawStockPercent));

                lblStockValue.Text = stockPercent + "%";
                BuildStockMeter(stockPercent);

                lblFeesValue.Text = "₱" + feesToday.ToString("N2");
                lblFeesFooter.Text = "UPDATED " + DateTime.Now.ToString("hh:mm tt") + "  ⟳";

                lblReportBody.Text =
                    "Connected libraries: " + GetScalarInt(conn, "SELECT COUNT(*) FROM dbo.ClientLibraries WHERE ISNULL(IsArchived, 0) = 0;").ToString("N0") + "\n" +
                    "Borrowing records: " + totalBorrow.ToString("N0") + "\n" +
                    "Return records: " + totalReturns.ToString("N0") + "\n" +
                    "Fine records: " + totalFines.ToString("N0");

                lblMaintenanceBody.Text =
                    "Database monitoring is active.\n" +
                    "Last refresh: " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt");

                BuildUsersMeter(activeUsers);
                BuildStockMeter(stockPercent);
                LoadActivityFeed(conn);
                LoadActivityRows(GetSearchTerm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Monitoring data could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadActivityFeed(SqlConnection conn)
        {
            activityItems.Clear();

            const string query = @"
SELECT TOP 50
    ActivityTime,
    MemberName,
    LibraryName,
    BookTitle,
    Activity,
    StatusText
FROM
(
    SELECT
        COALESCE(br.CreatedAt, CAST(br.IssueDate AS DATETIME2), SYSUTCDATETIME()) AS ActivityTime,
        ISNULL(NULLIF(u.FullName, ''), 'Unknown Member') AS MemberName,
        ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Library') AS LibraryName,
        ISNULL(NULLIF(br.BookTitle, ''), 'Unknown Book') AS BookTitle,
        'Borrowed' AS Activity,
        CASE
            WHEN UPPER(ISNULL(br.Status, '')) = 'OVERDUE' THEN 'Overdue'
            ELSE 'Success'
        END AS StatusText
    FROM dbo.BorrowingRecords br
    LEFT JOIN dbo.Users u ON u.UserID = br.MemberID
    LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = br.ClientID
    WHERE ISNULL(br.IsArchived, 0) = 0

    UNION ALL

    SELECT
        COALESCE(rr.CreatedAt, CAST(rr.ReturnDate AS DATETIME2), SYSUTCDATETIME()) AS ActivityTime,
        ISNULL(NULLIF(rr.MemberName, ''), 'Unknown Member') AS MemberName,
        ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Library') AS LibraryName,
        ISNULL(NULLIF(rr.BookTitle, ''), 'Unknown Book') AS BookTitle,
        'Returned' AS Activity,
        CASE
            WHEN ISNULL(rr.DaysOverdue, 0) > 0 THEN 'Late'
            ELSE 'Processed'
        END AS StatusText
    FROM dbo.ReturnRecords rr
    LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = rr.ClientID
    WHERE ISNULL(rr.IsArchived, 0) = 0

    UNION ALL

    SELECT
        COALESCE(fr.CreatedAt, SYSUTCDATETIME()) AS ActivityTime,
        ISNULL(NULLIF(fr.MemberName, ''), 'Unknown Member') AS MemberName,
        ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Library') AS LibraryName,
        ISNULL(NULLIF(fr.BookTitle, ''), 'Unknown Book') AS BookTitle,
        'Fine' AS Activity,
        ISNULL(NULLIF(fr.Status, ''), 'Pending') AS StatusText
    FROM dbo.FineRecords fr
    LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = fr.ClientID
    WHERE ISNULL(fr.IsArchived, 0) = 0
) activity
ORDER BY ActivityTime DESC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DateTime activityTime = reader["ActivityTime"] == DBNull.Value
                    ? DateTime.Now
                    : Convert.ToDateTime(reader["ActivityTime"]);

                activityItems.Add(new ActivityItem
                {
                    TimeText = activityTime.ToString("hh:mm tt"),
                    MemberName = SafeText(reader["MemberName"], "Unknown Member"),
                    Initials = GetInitials(SafeText(reader["MemberName"], "Unknown Member")),
                    LibraryName = SafeText(reader["LibraryName"], "Unknown Library"),
                    BookTitle = SafeText(reader["BookTitle"], "Unknown Book"),
                    Activity = SafeText(reader["Activity"], "Activity"),
                    Status = SafeText(reader["StatusText"], "Success")
                });
            }
        }

        private void LoadActivityRows(string keyword)
        {
            dgvActivity.Rows.Clear();

            string q = keyword.Trim();

            IEnumerable<ActivityItem> results = activityItems;

            if (!string.IsNullOrWhiteSpace(q))
            {
                results = results.Where(x =>
                    x.MemberName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    x.LibraryName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    x.BookTitle.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    x.Activity.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    x.Status.Contains(q, StringComparison.OrdinalIgnoreCase));
            }

            foreach (ActivityItem item in results)
            {
                dgvActivity.Rows.Add(
                    item.TimeText,
                    item.Initials + "|" + item.MemberName,
                    item.LibraryName,
                    item.BookTitle,
                    item.Activity,
                    item.Status
                );
            }

            dgvActivity.ClearSelection();
        }

        private string GetSearchTerm()
        {
            if (txtSearch == null || txtSearch.Text == "Search activity logs...")
                return "";

            return txtSearch.Text.Trim();
        }

        private int GetScalarInt(SqlConnection conn, string query)
        {
            using SqlCommand cmd = new SqlCommand(query, conn);
            object? value = cmd.ExecuteScalar();

            if (value == null || value == DBNull.Value)
                return 0;

            return Convert.ToInt32(value);
        }

        private decimal GetScalarDecimal(SqlConnection conn, string query)
        {
            using SqlCommand cmd = new SqlCommand(query, conn);
            object? value = cmd.ExecuteScalar();

            if (value == null || value == DBNull.Value)
                return 0;

            return Convert.ToDecimal(value);
        }

        private string SafeText(object value, string fallback = "")
        {
            if (value == null || value == DBNull.Value)
                return fallback;

            string text = Convert.ToString(value) ?? "";
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "NA";

            string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();

            return (parts[0][0].ToString() + parts[^1][0]).ToUpperInvariant();
        }

        private void EnsureMonitoringSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.ClientLibraries', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ClientLibraries', 'IsArchived') IS NULL
        ALTER TABLE dbo.ClientLibraries ADD IsArchived BIT NOT NULL CONSTRAINT DF_ClientLibraries_Monitor_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
        ALTER TABLE dbo.Users ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Users', 'Status') IS NULL
        ALTER TABLE dbo.Users ADD Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Users_Monitor_Status DEFAULT 'Active';

    IF COL_LENGTH('dbo.Users', 'IsArchived') IS NULL
        ALTER TABLE dbo.Users ADD IsArchived BIT NOT NULL CONSTRAINT DF_Users_Monitor_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Books', 'ClientID') IS NULL
        ALTER TABLE dbo.Books ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Books', 'IsArchived') IS NULL
        ALTER TABLE dbo.Books ADD IsArchived BIT NOT NULL CONSTRAINT DF_Books_Monitor_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.BorrowingRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.BorrowingRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.BorrowingRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_BorrowingRecords_Monitor_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.BorrowingRecords', 'CreatedAt') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_BorrowingRecords_Monitor_CreatedAt DEFAULT SYSUTCDATETIME();
END;

IF OBJECT_ID('dbo.ReturnRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ReturnRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.ReturnRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_ReturnRecords_Monitor_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.ReturnRecords', 'CreatedAt') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_ReturnRecords_Monitor_CreatedAt DEFAULT SYSUTCDATETIME();
END;

IF OBJECT_ID('dbo.FineRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.FineRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.FineRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.FineRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.FineRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_FineRecords_Monitor_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.FineRecords', 'CreatedAt') IS NULL
        ALTER TABLE dbo.FineRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_FineRecords_Monitor_CreatedAt DEFAULT SYSUTCDATETIME();
END;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void BuildUsersMeter(int activeUsers)
        {
            usersBars.Controls.Clear();

            int gap = 6;
            int y = 4;
            int h = 4;
            int totalWidth = 260;
            int fillBars = activeUsers == 0 ? 0 : Math.Min(4, Math.Max(1, activeUsers / 5 + 1));
            int barWidth = (totalWidth - (gap * 3)) / 4;

            for (int i = 0; i < 4; i++)
            {
                Panel bar = new Panel
                {
                    Height = h,
                    Width = barWidth,
                    Left = i * (barWidth + gap),
                    Top = y,
                    BackColor = i < fillBars ? Primary : Color.FromArgb(225, 230, 232)
                };
                usersBars.Controls.Add(bar);
            }
        }

        private void BuildStockMeter(int stockPercent)
        {
            stockProgress.Controls.Clear();

            int safePercent = Math.Max(0, Math.Min(100, stockPercent));

            Panel fill = new Panel
            {
                BackColor = PrimaryContainer,
                Width = (int)(stockProgress.Width * (safePercent / 100.0)),
                Height = stockProgress.Height,
                Left = 0,
                Top = 0
            };
            stockProgress.Controls.Add(fill);
        }

        private void dgvActivity_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvActivity.Columns[e.ColumnIndex].Name;

            if (col == "Member")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string initials = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : raw;

                Color bubbleColor = initials switch
                {
                    "NA" => Color.FromArgb(250, 225, 220),
                    _ => SecondaryContainer
                };

                Color textColor = initials == "NA" ? Tertiary : Primary;

                Rectangle bubble = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 13, 34, 34);

                using (SolidBrush brush = new SolidBrush(bubbleColor))
                    e.Graphics.FillEllipse(brush, bubble);

                TextRenderer.DrawText(
                    e.Graphics,
                    initials,
                    new Font("Segoe UI", 8F, FontStyle.Bold),
                    bubble,
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                Rectangle nameRect = new Rectangle(bubble.Right + 12, e.CellBounds.Y, e.CellBounds.Width - 56, e.CellBounds.Height);

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    nameRect,
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                );

                e.Handled = true;
            }
            else if (col == "Activity")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Color back = text switch
                {
                    "Borrowed" => SecondaryContainer,
                    "Returned" => SurfaceHigh,
                    "Fine" => Color.FromArgb(250, 225, 220),
                    _ => Color.FromArgb(220, 248, 238)
                };

                Color fore = text switch
                {
                    "Returned" => OnSurfaceVariant,
                    "Fine" => Tertiary,
                    _ => Primary
                };

                Size size = TextRenderer.MeasureText(text.ToUpper(), new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                    size.Width + 18,
                    24
                );

                using (SolidBrush brush = new SolidBrush(back))
                    e.Graphics.FillRectangle(brush, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text.ToUpper(),
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    badge,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Color dot = text.Equals("Overdue", StringComparison.OrdinalIgnoreCase) || text.Equals("Late", StringComparison.OrdinalIgnoreCase)
                    ? Tertiary
                    : Color.FromArgb(16, 180, 110);

                Color fore = dot == Tertiary ? Tertiary : Color.FromArgb(16, 125, 88);

                Rectangle dotRect = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + (e.CellBounds.Height / 2) - 4, 8, 8);

                using (SolidBrush brush = new SolidBrush(dot))
                    e.Graphics.FillEllipse(brush, dotRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Regular),
                    new Rectangle(dotRect.Right + 8, e.CellBounds.Y, e.CellBounds.Width - 22, e.CellBounds.Height),
                    fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                );

                e.Handled = true;
            }
        }

        private void MonitoringForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 28;
            int width = ClientSize.Width - (margin * 2);

            lblPageTitle.Location = new Point(margin, 30);
            lblPageSub.Location = new Point(margin, 70);

            searchBox.Size = new Size(320, 40);
            searchBox.Location = new Point(ClientSize.Width - searchBox.Width - margin, 38);

            searchBox.Controls[0].Location = new Point(14, 8);
            txtSearch.Location = new Point(44, 11);
            txtSearch.Width = searchBox.Width - 58;

            activityPanel.Bounds = new Rectangle(margin, 130, width, 292);

            lblVitalsTitle.Location = new Point(margin, activityPanel.Bottom + 38);
            lblVitalsSub.Location = new Point(margin, lblVitalsTitle.Bottom + 6);

            int cardTop = lblVitalsSub.Bottom + 22;
            int cardHeight = 232;

            if (width >= 980)
            {
                int cardWidth = (width - (gap * 2)) / 3;

                cardUsers.Bounds = new Rectangle(margin, cardTop, cardWidth, cardHeight);
                cardStock.Bounds = new Rectangle(cardUsers.Right + gap, cardTop, cardWidth, cardHeight);
                cardFees.Bounds = new Rectangle(cardStock.Right + gap, cardTop, cardWidth, cardHeight);

                int bottomTop = cardUsers.Bottom + 36;
                int bottomWidth = (width - gap) / 2;

                reportPanel.Bounds = new Rectangle(margin, bottomTop, bottomWidth, 258);
                maintenancePanel.Bounds = new Rectangle(reportPanel.Right + gap, bottomTop, bottomWidth, 258);
            }
            else
            {
                int cardWidth = width;

                cardUsers.Bounds = new Rectangle(margin, cardTop, cardWidth, cardHeight);
                cardStock.Bounds = new Rectangle(margin, cardUsers.Bottom + gap, cardWidth, cardHeight);
                cardFees.Bounds = new Rectangle(margin, cardStock.Bottom + gap, cardWidth, cardHeight);

                int bottomTop = cardFees.Bottom + 36;

                reportPanel.Bounds = new Rectangle(margin, bottomTop, width, 258);
                maintenancePanel.Bounds = new Rectangle(margin, reportPanel.Bottom + gap, width, 258);
            }

            LayoutActivityPanel();
            LayoutBottomPanels();

            canvasPanel.AutoScrollMinSize = new Size(0, maintenancePanel.Bottom + 40);
        }

        private void LayoutActivityPanel()
        {
            dgvActivity.Location = new Point(0, 0);
            dgvActivity.Size = new Size(activityPanel.Width - 2, activityPanel.Height - 2);
        }

        private void LayoutBottomPanels()
        {
            reportImage.Bounds = new Rectangle(36, 34, 138, 138);
            lblReportTitle.Location = new Point(reportImage.Right + 28, 38);
            lblReportBody.Location = new Point(reportImage.Right + 28, lblReportTitle.Bottom + 14);
            btnDeepDive.Location = new Point(reportImage.Right + 24, 188);
            btnDeepDive.Size = new Size(190, 34);

            lblMaintenanceTitle.Location = new Point(34, 36);
            lblMaintenanceBody.Location = new Point(34, 82);
            lblMaintenanceChip1.Location = new Point(34, 152);
            lblMaintenanceChip2.Location = new Point(lblMaintenanceChip1.Right + 10, 152);
            lblMaintenanceIcon.Location = new Point(maintenancePanel.Width - 110, 138);
        }

        private class ActivityItem
        {
            public string TimeText { get; set; } = "";
            public string Initials { get; set; } = "";
            public string MemberName { get; set; } = "";
            public string LibraryName { get; set; } = "";
            public string BookTitle { get; set; } = "";
            public string Activity { get; set; } = "";
            public string Status { get; set; } = "";
        }
    }
}