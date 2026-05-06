using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class ReportsForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SoftBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentSoft = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");
        private readonly Color AccentDangerSoft = ColorTranslator.FromHtml("#F7816D");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Button btnExportPdf = null!;

        private Panel datasetCard = null!;
        private Button btnGeneral = null!;
        private Button btnBorrowing = null!;
        private Button btnReturns = null!;
        private Button btnFines = null!;

        private Panel statCard1 = null!;
        private Panel statCard2 = null!;
        private Panel statCard3 = null!;

        private Panel chartCard = null!;
        private Label lblChartTitle = null!;
        private Label lblChartSubtitle = null!;
        private ComboBox cmbRange = null!;
        private Chart reportsChart = null!;
        private Panel chartLegendPanel = null!;
        private Panel mainLegendDot = null!;
        private Panel alertLegendDot = null!;
        private Label lblMainLegend = null!;
        private Label lblAlertLegend = null!;

        private Panel tableCard = null!;
        private Label lblTableTitle = null!;
        private Panel searchPanel = null!;
        private TextBox txtSearch = null!;
        private DataGridView dgvReports = null!;

        private string currentCategory = "General";
        private string currentRange = "Monthly";
        private DataTable reportTable = new DataTable();

        // Philippines time offset. This makes Daily reports show the real local day,
        // so Saturday transactions do not appear under Monday because of UTC dates.
        private const int ReportUtcOffsetHours = 8;

        public ReportsForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            BuildUI();

            Load += (s, e) =>
            {
                AdjustLayout();
                currentRange = "Monthly";
                LoadCategory("General");
                ClearGridSelection();
            };

            Resize += (s, e) => AdjustLayout();
        }

        private void BuildUI()
        {
            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = FormBack,
                AutoScroll = true
            };
            Controls.Add(canvas);

            lblTitle = new Label
            {
                Text = "Generate Library Reports",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubtitle = new Label
            {
                Text = "Compile and export administrative insights from the library system.",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            btnExportPdf = new Button
            {
                Text = "⇩  Export PDF",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.Click += (s, e) => ExportCurrentReportPdf();

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubtitle);
            canvas.Controls.Add(btnExportPdf);

            BuildDatasetCard();
            BuildStatCards();
            BuildChartCard();
            BuildTableCard();
        }

        private void BuildDatasetCard()
        {
            datasetCard = CreateRoundedCard();
            datasetCard.BackColor = HeaderBack;
            canvas.Controls.Add(datasetCard);

            Label title = new Label
            {
                Text = "ACTIVE REPORT DATASET",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            Label sub = new Label
            {
                Text = "Switch views to update visualizer and logs.",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            btnGeneral = CreateTabButton("General");
            btnBorrowing = CreateTabButton("Borrowing");
            btnReturns = CreateTabButton("Returns");
            btnFines = CreateTabButton("Fines");

            btnGeneral.Click += (s, e) => LoadCategory("General");
            btnBorrowing.Click += (s, e) => LoadCategory("Borrowing");
            btnReturns.Click += (s, e) => LoadCategory("Returns");
            btnFines.Click += (s, e) => LoadCategory("Fines");

            datasetCard.Controls.Add(title);
            datasetCard.Controls.Add(sub);
            datasetCard.Controls.Add(btnGeneral);
            datasetCard.Controls.Add(btnBorrowing);
            datasetCard.Controls.Add(btnReturns);
            datasetCard.Controls.Add(btnFines);

            datasetCard.Tag = new Control[] { title, sub, btnGeneral, btnBorrowing, btnReturns, btnFines };
        }

        private Button CreateTabButton(string text)
        {
            Button btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 10.5F),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void BuildStatCards()
        {
            statCard1 = CreateRoundedCard();
            statCard2 = CreateRoundedCard();
            statCard3 = CreateRoundedCard();

            canvas.Controls.Add(statCard1);
            canvas.Controls.Add(statCard2);
            canvas.Controls.Add(statCard3);

            BuildStatCard(statCard1, "▥", "Total Reports Generated", "1,284", AccentSoft, AccentDeep);
            BuildStatCard(statCard2, "▣", "Recent Activity Volume", "+14.2%", AccentSoft, AccentDeep);
            BuildStatCard(statCard3, "◆", "System Data Integrity", "99.9%", ColorTranslator.FromHtml("#FDE8E4"), AccentDanger);
        }

        private void BuildStatCard(Panel card, string iconText, string labelText, string valueText, Color iconBack, Color iconFore)
        {
            Panel iconBox = new Panel
            {
                Size = new Size(52, 52),
                BackColor = iconBack
            };
            iconBox.Paint += RoundedPanelPaint;

            Label icon = new Label
            {
                Text = iconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 17F, FontStyle.Bold),
                ForeColor = iconFore,
                BackColor = Color.Transparent
            };
            iconBox.Controls.Add(icon);

            Label label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            Label value = new Label
            {
                Text = valueText,
                AutoSize = true,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            card.Controls.Add(iconBox);
            card.Controls.Add(label);
            card.Controls.Add(value);
            card.Tag = new Control[] { iconBox, label, value };
        }

        private void BuildChartCard()
        {
            chartCard = CreateRoundedCard();
            chartCard.BackColor = HeaderBack;
            canvas.Controls.Add(chartCard);

            lblChartTitle = new Label
            {
                Text = "Monthly Report Trends",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblChartSubtitle = new Label
            {
                Text = "Aggregated system activity for General Reports",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            cmbRange = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = SoftBack,
                ForeColor = SecondaryText,
                FlatStyle = FlatStyle.Flat
            };
            cmbRange.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly", "Yearly" });
            cmbRange.SelectedIndexChanged += (s, e) =>
            {
                if (cmbRange.SelectedItem == null) return;

                currentRange = cmbRange.SelectedItem.ToString() ?? "Monthly";
                LoadChart(currentRange);
            };
            cmbRange.SelectedItem = "Monthly";

            chartLegendPanel = new Panel
            {
                BackColor = HeaderBack
            };

            mainLegendDot = new Panel
            {
                Size = new Size(12, 12),
                BackColor = AccentEmerald
            };
            mainLegendDot.Paint += CircleDotPaint;

            alertLegendDot = new Panel
            {
                Size = new Size(12, 12),
                BackColor = ColorTranslator.FromHtml("#AAB8B3")
            };
            alertLegendDot.Paint += CircleDotPaint;

            lblMainLegend = new Label
            {
                Text = "Main = Total activity",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            lblAlertLegend = new Label
            {
                Text = "Alert = Pending alerts",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            chartLegendPanel.Controls.Add(mainLegendDot);
            chartLegendPanel.Controls.Add(lblMainLegend);
            chartLegendPanel.Controls.Add(alertLegendDot);
            chartLegendPanel.Controls.Add(lblAlertLegend);

            reportsChart = new Chart
            {
                BackColor = HeaderBack
            };

            ChartArea area = new ChartArea("MainArea");
            area.BackColor = HeaderBack;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = LineSoft;
            area.AxisX.LineColor = LineSoft;
            area.AxisY.LineColor = LineSoft;
            area.AxisX.LabelStyle.ForeColor = SecondaryText;
            area.AxisY.LabelStyle.ForeColor = ColorTranslator.FromHtml("#9AA8A2");
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            area.AxisX.Interval = 1;
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 100;

            reportsChart.ChartAreas.Add(area);

            Series completed = new Series("Completed")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = AccentEmerald,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 8,
                MarkerColor = Color.White,
                MarkerBorderColor = AccentEmerald,
                MarkerBorderWidth = 2
            };

            Series pending = new Series("Pending")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash,
                Color = ColorTranslator.FromHtml("#AAB8B3")
            };

            reportsChart.AntiAliasing = AntiAliasingStyles.All;
            reportsChart.Series.Add(completed);
            reportsChart.Series.Add(pending);

            chartCard.Controls.Add(lblChartTitle);
            chartCard.Controls.Add(lblChartSubtitle);
            chartCard.Controls.Add(cmbRange);
            chartCard.Controls.Add(chartLegendPanel);
            chartCard.Controls.Add(reportsChart);
        }

        private void BuildTableCard()
        {
            tableCard = CreateRoundedCard();
            canvas.Controls.Add(tableCard);

            lblTableTitle = new Label
            {
                Text = "Generated Report Logs",
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            searchPanel = new Panel
            {
                BackColor = HeaderBack
            };
            searchPanel.Paint += RoundedPanelPaint;

            Label searchIcon = new Label
            {
                Text = "⌕",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 14F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            txtSearch = new TextBox
            {
                Text = "Search reports...",
                BorderStyle = BorderStyle.None,
                BackColor = HeaderBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 10.5F)
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search reports...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = PrimaryText;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search reports...";
                    txtSearch.ForeColor = SecondaryText;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                if (!txtSearch.Focused && txtSearch.Text == "Search reports...")
                    return;

                ApplySearch(txtSearch.Text);
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(txtSearch);

            dgvReports = new DataGridView
            {
                BackgroundColor = CardBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 52,
                RowTemplate = { Height = 58 },
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical,
                ShowCellToolTips = false
            };

            dgvReports.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvReports.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvReports.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvReports.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvReports.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvReports.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgvReports.DefaultCellStyle.BackColor = CardBack;
            dgvReports.DefaultCellStyle.ForeColor = PrimaryText;
            dgvReports.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvReports.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvReports.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvReports.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            dgvReports.RowsDefaultCellStyle.BackColor = CardBack;
            dgvReports.RowsDefaultCellStyle.SelectionBackColor = CardBack;
            dgvReports.RowsDefaultCellStyle.SelectionForeColor = PrimaryText;

            tableCard.Controls.Add(lblTableTitle);
            tableCard.Controls.Add(searchPanel);
            tableCard.Controls.Add(dgvReports);
        }

        private void LoadCategory(string category)
        {
            currentCategory = category;
            StyleCategoryButtons();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureReportsSchema(conn);

                reportTable = category switch
                {
                    "Borrowing" => LoadBorrowingReportTable(conn),
                    "Returns" => LoadReturnsReportTable(conn),
                    "Fines" => LoadFinesReportTable(conn),
                    _ => LoadGeneralReportTable(conn)
                };

                dgvReports.DataSource = reportTable;

                foreach (DataGridViewColumn col in dgvReports.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                UpdateChartLegend(category);

                LoadChart(currentRange);
                UpdateStatCards(conn);
                ClearGridSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Report data could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private DataTable CreateReportTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Report ID");
            table.Columns.Add("Report Name");
            table.Columns.Add("Category");
            table.Columns.Add("Generated Date");
            table.Columns.Add("Status");
            return table;
        }

        private DataTable LoadGeneralReportTable(SqlConnection conn)
        {
            DataTable table = CreateReportTable();

            int books = ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Books WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;");
            int users = ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Users WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;");
            int categories = ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Categories WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;");
            int activeBorrow = ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND UPPER(Status) = 'ACTIVE';");

            table.Rows.Add("GEN-001", $"Total Books: {books:N0}", "General", DateTime.Today.ToString("MMM dd, yyyy"), "Completed");
            table.Rows.Add("GEN-002", $"Registered Users: {users:N0}", "General", DateTime.Today.ToString("MMM dd, yyyy"), "Completed");
            table.Rows.Add("GEN-003", $"Categories: {categories:N0}", "General", DateTime.Today.ToString("MMM dd, yyyy"), "Completed");
            table.Rows.Add("GEN-004", $"Active Borrow Records: {activeBorrow:N0}", "General", DateTime.Today.ToString("MMM dd, yyyy"), "Completed");

            return table;
        }

        private DataTable LoadBorrowingReportTable(SqlConnection conn)
        {
            DataTable table = CreateReportTable();

            const string query = @"
SELECT TOP 100
    BorrowID,
    BookTitle,
    IssueDate,
    DueDate,
    Status
FROM dbo.BorrowingRecords
WHERE ISNULL(IsArchived, 0) = 0
  AND ClientID = @ClientID
ORDER BY BorrowID DESC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["BorrowID"]);
                string title = Convert.ToString(reader["BookTitle"]) ?? "Untitled Book";
                DateTime issueDate = reader["IssueDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["IssueDate"]);
                string status = Convert.ToString(reader["Status"]) ?? "ACTIVE";

                table.Rows.Add(
                    "BR-" + id.ToString("0000"),
                    title,
                    "Borrowing",
                    issueDate.ToString("MMM dd, yyyy"),
                    NormalizeReportStatus(status)
                );
            }

            return table;
        }

        private DataTable LoadReturnsReportTable(SqlConnection conn)
        {
            DataTable table = CreateReportTable();

            const string query = @"
SELECT TOP 100
    ReturnID,
    MemberName,
    BookTitle,
    ReturnDate,
    ReturnStatus,
    BookCondition
FROM dbo.ReturnRecords
WHERE ISNULL(IsArchived, 0) = 0
  AND ClientID = @ClientID
ORDER BY ReturnID DESC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ReturnID"]);
                string member = Convert.ToString(reader["MemberName"]) ?? "";
                string title = Convert.ToString(reader["BookTitle"]) ?? "Untitled Book";
                string status = Convert.ToString(reader["ReturnStatus"]) ?? "RETURNED";
                string condition = Convert.ToString(reader["BookCondition"]) ?? "GOOD";
                DateTime returnDate = reader["ReturnDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["ReturnDate"]);

                table.Rows.Add(
                    "RT-" + id.ToString("0000"),
                    $"{member} - {title} ({condition})",
                    "Returns",
                    returnDate.ToString("MMM dd, yyyy"),
                    NormalizeReportStatus(status)
                );
            }

            return table;
        }

        private DataTable LoadFinesReportTable(SqlConnection conn)
        {
            DataTable table = CreateReportTable();

            const string query = @"
SELECT TOP 100
    FineID,
    MemberName,
    BookTitle,
    Reason,
    Amount,
    Status,
    CreatedAt
FROM dbo.FineRecords
WHERE ISNULL(IsArchived, 0) = 0
  AND ClientID = @ClientID
ORDER BY FineID DESC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["FineID"]);
                string member = Convert.ToString(reader["MemberName"]) ?? "";
                string title = Convert.ToString(reader["BookTitle"]) ?? "";
                string reason = Convert.ToString(reader["Reason"]) ?? "";
                decimal amount = reader["Amount"] == DBNull.Value ? 0M : Convert.ToDecimal(reader["Amount"]);
                string status = Convert.ToString(reader["Status"]) ?? "Unpaid";
                DateTime created = reader["CreatedAt"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["CreatedAt"]);

                table.Rows.Add(
                    "FN-" + id.ToString("0000"),
                    $"{member} - {title} / {reason} ({amount:₱#,##0.00})",
                    "Fines",
                    created.ToString("MMM dd, yyyy"),
                    NormalizeReportStatus(status)
                );
            }

            return table;
        }

        private string NormalizeReportStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Completed";

            string value = status.Trim().ToUpperInvariant();

            if (value == "ACTIVE")
                return "Active";

            if (value == "RETURNED")
                return "Completed";

            if (value == "PAID")
                return "Completed";

            if (value == "UNPAID")
                return "Pending";

            if (value == "LATE" || value == "OVERDUE")
                return "Pending";

            return status;
        }

        private int ExecuteCount(SqlConnection conn, string query)
        {
            using SqlCommand cmd = new SqlCommand(query, conn);

            if (query.Contains("@ClientID", StringComparison.OrdinalIgnoreCase))
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);

            object? result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        private decimal ExecuteDecimal(SqlConnection conn, string query)
        {
            using SqlCommand cmd = new SqlCommand(query, conn);

            if (query.Contains("@ClientID", StringComparison.OrdinalIgnoreCase))
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);

            object? result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0M : Convert.ToDecimal(result);
        }

        private void UpdateStatCards(SqlConnection conn)
        {
            int totalReports =
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Books WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Users WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.ReturnRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;");

            int recentActivity =
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= DATEADD(day, -30, SYSUTCDATETIME());") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.ReturnRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= DATEADD(day, -30, SYSUTCDATETIME());") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= DATEADD(day, -30, SYSUTCDATETIME());");

            int books = ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Books WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;");
            int booksWithIds = ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Books WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND NULLIF(BookID, '') IS NOT NULL;");
            decimal integrity = books == 0 ? 100M : (decimal)booksWithIds / books * 100M;

            SetStatCard(statCard1, "Total Database Records", totalReports.ToString("N0"));
            SetStatCard(statCard2, "Recent Activity Volume", recentActivity.ToString("N0"));
            SetStatCard(statCard3, "System Data Integrity", integrity.ToString("N1") + "%");
        }

        private void SetStatCard(Panel card, string labelText, string valueText)
        {
            if (card.Tag is not Control[] c || c.Length < 3)
                return;

            if (c[1] is Label label)
                label.Text = labelText;

            if (c[2] is Label value)
                value.Text = valueText;
        }

        private void EnsureReportsSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.Books', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Books
    (
        BookID NVARCHAR(20) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        BookTitle NVARCHAR(250) NOT NULL,
        Author NVARCHAR(150) NULL,
        Category NVARCHAR(150) NULL,
        Genre NVARCHAR(100) NULL,
        GroupName NVARCHAR(100) NULL,
        PublishYear INT NULL,
        Quantity INT NOT NULL DEFAULT 1,
        Status NVARCHAR(50) NOT NULL DEFAULT 'In Stock',
        RecordedBy NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Books', 'ClientID') IS NULL
        ALTER TABLE dbo.Books ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Books', 'IsArchived') IS NULL
        ALTER TABLE dbo.Books ADD IsArchived BIT NOT NULL CONSTRAINT DF_Books_Report_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        FullName NVARCHAR(150) NOT NULL,
        Role NVARCHAR(50) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
        ALTER TABLE dbo.Users ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Users', 'IsArchived') IS NULL
        ALTER TABLE dbo.Users ADD IsArchived BIT NOT NULL CONSTRAINT DF_Users_Report_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.Categories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        CategoryID NVARCHAR(20) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        CategoryName NVARCHAR(150) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Categories', 'ClientID') IS NULL
        ALTER TABLE dbo.Categories ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Categories', 'IsArchived') IS NULL
        ALTER TABLE dbo.Categories ADD IsArchived BIT NOT NULL CONSTRAINT DF_Categories_Report_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.BorrowingRecords', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BorrowingRecords
    (
        BorrowID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NOT NULL DEFAULT 1,
        MemberID INT NULL,
        BookID NVARCHAR(20) NULL,
        BookTitle NVARCHAR(250) NOT NULL,
        IssueDate DATE NOT NULL DEFAULT CAST(SYSUTCDATETIME() AS DATE),
        DueDate DATE NOT NULL DEFAULT CAST(SYSUTCDATETIME() AS DATE),
        Status NVARCHAR(50) NOT NULL DEFAULT 'ACTIVE',
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF OBJECT_ID('dbo.BorrowingRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.BorrowingRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.BorrowingRecords', 'CreatedAt') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_BorrowingRecords_Report_CreatedAt DEFAULT SYSUTCDATETIME();

    IF COL_LENGTH('dbo.BorrowingRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_BorrowingRecords_Report_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.ReturnRecords', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReturnRecords
    (
        ReturnID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        BorrowID INT NULL,
        MemberName NVARCHAR(150) NOT NULL,
        BookTitle NVARCHAR(250) NOT NULL,
        ReturnDate DATE NOT NULL DEFAULT CAST(SYSUTCDATETIME() AS DATE),
        BookCondition NVARCHAR(40) NOT NULL DEFAULT 'GOOD',
        ReturnStatus NVARCHAR(40) NOT NULL DEFAULT 'RETURNED',
        DaysOverdue INT NOT NULL DEFAULT 0,
        FineAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF OBJECT_ID('dbo.ReturnRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ReturnRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.ReturnRecords', 'CreatedAt') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_ReturnRecords_Report_CreatedAt DEFAULT SYSUTCDATETIME();

    IF COL_LENGTH('dbo.ReturnRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_ReturnRecords_Report_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.FineRecords', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.FineRecords
    (
        FineID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        BorrowID INT NULL,
        MemberID INT NULL,
        MemberName NVARCHAR(150) NOT NULL,
        BookID NVARCHAR(20) NULL,
        BookTitle NVARCHAR(250) NULL,
        Reason NVARCHAR(150) NOT NULL,
        Amount DECIMAL(10,2) NOT NULL DEFAULT 0,
        Status NVARCHAR(40) NOT NULL DEFAULT 'Unpaid',
        Remarks NVARCHAR(500) NULL,
        RecordedBy NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF OBJECT_ID('dbo.FineRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.FineRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.FineRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.FineRecords', 'CreatedAt') IS NULL
        ALTER TABLE dbo.FineRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_FineRecords_Report_CreatedAt DEFAULT SYSUTCDATETIME();

    IF COL_LENGTH('dbo.FineRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.FineRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_FineRecords_Report_IsArchived DEFAULT 0;
END;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void StyleCategoryButtons()
        {
            Button[] buttons = { btnGeneral, btnBorrowing, btnReturns, btnFines };

            foreach (Button btn in buttons)
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = PrimaryText;
                btn.Font = new Font("Segoe UI", 10.5F);
            }

            Button active = currentCategory switch
            {
                "Borrowing" => btnBorrowing,
                "Returns" => btnReturns,
                "Fines" => btnFines,
                _ => btnGeneral
            };

            active.BackColor = CardBack;
            active.ForeColor = AccentDeep;
            active.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        }

        private void UpdateChartLegend(string category)
        {
            if (lblMainLegend == null || lblAlertLegend == null)
                return;

            if (category == "Borrowing")
            {
                lblMainLegend.Text = "Main = Borrowed books";
                lblAlertLegend.Text = "Alert = Overdue books";
                lblChartSubtitle.Text = "Borrowing Reports";
            }
            else if (category == "Returns")
            {
                lblMainLegend.Text = "Main = Completed returns";
                lblAlertLegend.Text = "Alert = Late returns";
                lblChartSubtitle.Text = "Returns Reports";
            }
            else if (category == "Fines")
            {
                lblMainLegend.Text = "Main = Paid fines";
                lblAlertLegend.Text = "Alert = Unpaid fines";
                lblChartSubtitle.Text = "Fine Reports";
            }
            else
            {
                lblMainLegend.Text = "Main = Total activity";
                lblAlertLegend.Text = "Alert = Pending alerts";
                lblChartSubtitle.Text = "General Reports";
            }
        }

        private void LoadChart(string range)
        {
            if (reportsChart == null || reportsChart.Series.Count < 2)
                return;

            ChartArea area = reportsChart.ChartAreas["MainArea"];

            reportsChart.Series["Completed"].Points.Clear();
            reportsChart.Series["Pending"].Points.Clear();
            area.AxisX.CustomLabels.Clear();

            string[] labels = GetChartLabels(range);
            int[] completed = new int[labels.Length];
            int[] pending = new int[labels.Length];

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureReportsSchema(conn);

                if (range == "Daily")
                    LoadDailyGraphFromDatabase(conn, labels, completed, pending);
                else
                    LoadRangeGraphFromDatabase(conn, range, labels, completed, pending);
            }
            catch
            {
                // Keep zero values if database cannot be reached.
            }

            lblChartTitle.Text = range switch
            {
                "Daily" => "Today Report Trends",
                "Weekly" => "This Week Report Trends",
                "Yearly" => "Yearly Report Trends",
                "Monthly" => "This Month Report Trends",
                _ => "This Year Report Trends"
            };

            int maxValue = Math.Max(10, completed.Concat(pending).DefaultIfEmpty(0).Max());
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = Math.Max(10, Math.Ceiling(maxValue / 10.0) * 10);
            area.AxisY.Interval = Math.Max(1, area.AxisY.Maximum / 5);

            area.AxisX.Minimum = 0.5;
            area.AxisX.Maximum = labels.Length + 0.5;
            area.AxisX.Interval = 1;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LabelStyle.IsStaggered = false;

            reportsChart.Series["Completed"].ChartType = SeriesChartType.Line;
            reportsChart.Series["Pending"].ChartType = SeriesChartType.Line;
            reportsChart.Series["Completed"].XValueType = ChartValueType.Int32;
            reportsChart.Series["Pending"].XValueType = ChartValueType.Int32;
            reportsChart.Series["Completed"].BorderWidth = 3;
            reportsChart.Series["Pending"].BorderWidth = 2;
            reportsChart.Series["Completed"].MarkerStyle = MarkerStyle.Circle;
            reportsChart.Series["Completed"].MarkerSize = 8;
            reportsChart.Series["Pending"].MarkerStyle = MarkerStyle.Circle;
            reportsChart.Series["Pending"].MarkerSize = 7;

            for (int i = 0; i < labels.Length; i++)
            {
                int x = i + 1;

                reportsChart.Series["Completed"].Points.AddXY(x, completed[i]);
                reportsChart.Series["Pending"].Points.AddXY(x, pending[i]);
                area.AxisX.CustomLabels.Add(x - 0.5, x + 0.5, labels[i]);
            }

            reportsChart.Invalidate();
        }

        private void LoadDailyGraphFromDatabase(SqlConnection conn, string[] labels, int[] completed, int[] pending)
        {
            // Daily means TODAY only.
            LoadRangeGraphFromDatabase(conn, "Daily", labels, completed, pending);
        }

        private void LoadRangeGraphFromDatabase(SqlConnection conn, string range, string[] labels, int[] completed, int[] pending)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                GetDateRangeForLabel(range, labels[i], i, out DateTime fromDate, out DateTime toDate);

                completed[i] = GetCompletedCount(conn, currentCategory, fromDate, toDate);
                pending[i] = GetPendingCount(conn, currentCategory, fromDate, toDate);
            }
        }

        private string[] GetChartLabels(string range)
        {
            DateTime todayLocal = DateTime.UtcNow.AddHours(ReportUtcOffsetHours).Date;

            if (range == "Daily")
                return new[] { todayLocal.ToString("MMM dd").ToUpperInvariant() };

            if (range == "Weekly")
                return new[] { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };

            if (range == "Monthly")
                return new[] { "WEEK 1", "WEEK 2", "WEEK 3", "WEEK 4", "WEEK 5" };

            return new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
        }

        private void GetDateRangeForLabel(string range, string label, int index, out DateTime fromDate, out DateTime toDate)
        {
            DateTime todayLocal = DateTime.UtcNow.AddHours(ReportUtcOffsetHours).Date;

            if (range == "Daily")
            {
                // Daily = today only
                DateTime localStart = todayLocal;
                DateTime localEnd = todayLocal.AddDays(1);

                fromDate = localStart.AddHours(-ReportUtcOffsetHours);
                toDate = localEnd.AddHours(-ReportUtcOffsetHours);
                return;
            }

            if (range == "Weekly")
            {
                // Weekly = current week only, displayed by day
                int diff = ((int)todayLocal.DayOfWeek + 6) % 7;
                DateTime monday = todayLocal.AddDays(-diff);

                DateTime localStart = monday.AddDays(index);
                DateTime localEnd = localStart.AddDays(1);

                fromDate = localStart.AddHours(-ReportUtcOffsetHours);
                toDate = localEnd.AddHours(-ReportUtcOffsetHours);
                return;
            }

            if (range == "Monthly")
            {
                // Monthly = current month only, displayed by week
                DateTime monthStart = new DateTime(todayLocal.Year, todayLocal.Month, 1);
                DateTime monthEndExclusive = monthStart.AddMonths(1);

                DateTime localStart = monthStart.AddDays(index * 7);
                DateTime localEnd = localStart.AddDays(7);

                if (localStart >= monthEndExclusive)
                {
                    fromDate = monthEndExclusive.AddHours(-ReportUtcOffsetHours);
                    toDate = monthEndExclusive.AddHours(-ReportUtcOffsetHours);
                    return;
                }

                if (localEnd > monthEndExclusive)
                    localEnd = monthEndExclusive;

                fromDate = localStart.AddHours(-ReportUtcOffsetHours);
                toDate = localEnd.AddHours(-ReportUtcOffsetHours);
                return;
            }

            // Yearly = current year only, displayed by month
            DateTime localMonthStart = new DateTime(todayLocal.Year, index + 1, 1);
            DateTime localMonthEnd = localMonthStart.AddMonths(1);

            fromDate = localMonthStart.AddHours(-ReportUtcOffsetHours);
            toDate = localMonthEnd.AddHours(-ReportUtcOffsetHours);
        }

        private int GetCompletedCount(SqlConnection conn, string category, DateTime fromDate, DateTime toDate)
        {
            string query = category switch
            {
                "Borrowing" => @"SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= @FromDate AND CreatedAt < @ToDate;",
                "Returns" => @"SELECT COUNT(*) FROM dbo.ReturnRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND ReturnDate >= CAST(DATEADD(hour, @OffsetHours, @FromDate) AS DATE) AND ReturnDate < CAST(DATEADD(hour, @OffsetHours, @ToDate) AS DATE);",
                "Fines" => @"SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND UPPER(Status) = 'PAID' AND CreatedAt >= @FromDate AND CreatedAt < @ToDate;",
                _ => @"
SELECT
(
    SELECT COUNT(*) FROM dbo.Books WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= @FromDate AND CreatedAt < @ToDate
) +
(
    SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= @FromDate AND CreatedAt < @ToDate
) +
(
    SELECT COUNT(*) FROM dbo.ReturnRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND ReturnDate >= CAST(DATEADD(hour, @OffsetHours, @FromDate) AS DATE) AND ReturnDate < CAST(DATEADD(hour, @OffsetHours, @ToDate) AS DATE)
) +
(
    SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= @FromDate AND CreatedAt < @ToDate
);"
            };

            return ExecuteDateCount(conn, query, fromDate, toDate);
        }

        private int GetPendingCount(SqlConnection conn, string category, DateTime fromDate, DateTime toDate)
        {
            string query = category switch
            {
                "Borrowing" => @"SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND UPPER(Status) = 'ACTIVE' AND DueDate < CAST(SYSUTCDATETIME() AS DATE) AND CreatedAt >= @FromDate AND CreatedAt < @ToDate;",
                "Returns" => @"SELECT COUNT(*) FROM dbo.ReturnRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND (UPPER(ReturnStatus) LIKE '%LATE%' OR ISNULL(DaysOverdue, 0) > 0) AND ReturnDate >= CAST(DATEADD(hour, @OffsetHours, @FromDate) AS DATE) AND ReturnDate < CAST(DATEADD(hour, @OffsetHours, @ToDate) AS DATE);",
                "Fines" => @"SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND UPPER(Status) = 'UNPAID' AND CreatedAt >= @FromDate AND CreatedAt < @ToDate;",
                _ => @"SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND UPPER(Status) = 'UNPAID' AND CreatedAt >= @FromDate AND CreatedAt < @ToDate;"
            };

            return ExecuteDateCount(conn, query, fromDate, toDate);
        }

        private int ExecuteDateCount(SqlConnection conn, string query, DateTime fromDate, DateTime toDate)
        {
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FromDate", fromDate);
            cmd.Parameters.AddWithValue("@ToDate", toDate);
            cmd.Parameters.AddWithValue("@OffsetHours", ReportUtcOffsetHours);

            if (query.Contains("@ClientID", StringComparison.OrdinalIgnoreCase))
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);

            object? result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        private bool IsDateInCurrentRange(DateTime date)
        {
            return IsDateInsideSelectedRange(date, currentRange);
        }

        private DataTable FilterTableForCurrentRange(DataTable source)
        {
            DataTable filtered = source.Clone();

            foreach (DataRow row in source.Rows)
            {
                string dateText = Convert.ToString(row["Generated Date"]) ?? "";

                if (DateTime.TryParse(dateText, out DateTime reportDate) && IsDateInCurrentRange(reportDate))
                    filtered.ImportRow(row);
            }

            return filtered;
        }

        private void ExportCurrentReportPdf()
        {
            try
            {
                using SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Export Library Report PDF",
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"LibraryReports_{currentCategory}_{currentRange}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureReportsSchema(conn);

                List<string> lines = BuildPdfReportLines(conn);
                SimplePdfWriter.WriteTextPdf(saveDialog.FileName, lines);

                MessageBox.Show(
                    $"{currentRange} {currentCategory} PDF report exported successfully.",
                    "Export Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "PDF report could not be exported.\n\n" + ex.Message,
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string GetRangeDescription(string selectedRange)
        {
            DateTime todayLocal = DateTime.UtcNow.AddHours(ReportUtcOffsetHours).Date;

            if (selectedRange == "Daily")
                return "today only: " + todayLocal.ToString("MMM dd, yyyy");

            if (selectedRange == "Weekly")
            {
                int diff = ((int)todayLocal.DayOfWeek + 6) % 7;
                DateTime weekStart = todayLocal.AddDays(-diff);
                DateTime weekEnd = weekStart.AddDays(6);
                return "current week only: " + weekStart.ToString("MMM dd") + " - " + weekEnd.ToString("MMM dd, yyyy");
            }

            if (selectedRange == "Monthly")
                return "current month only: " + todayLocal.ToString("MMMM yyyy");

            return "current year only: " + todayLocal.ToString("yyyy");
        }

        private string GetCurrentLibraryName(SqlConnection conn)
        {
            if (!string.IsNullOrWhiteSpace(ClientSession.LibraryName))
                return ClientSession.LibraryName!;

            try
            {
                const string query = @"
SELECT TOP 1 LibraryName
FROM dbo.ClientLibraries
WHERE ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                object? result = cmd.ExecuteScalar();

                string name = result == null || result == DBNull.Value ? "" : Convert.ToString(result) ?? "";
                return string.IsNullOrWhiteSpace(name) ? "Current Client Library" : name;
            }
            catch
            {
                return "Current Client Library";
            }
        }

        private List<string> BuildPdfReportLines(SqlConnection conn)
        {
            // IMPORTANT:
            // PDF export is filtered by BOTH current selected tab and current selected range.
            // Example: Returns + Daily = Daily Returns report only.
            // It will NOT export General, Borrowing, and Fines unless those tabs are selected.

            string selectedCategory = currentCategory;
            string selectedRange = currentRange;

            List<string> lines = new List<string>
            {
                "LIBRAFLOW ERP - LIBRARY REPORT",
                "School: " + GetCurrentLibraryName(conn),
                "Generated: " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt"),
                "Selected Report: " + selectedCategory,
                "Selected Range: " + selectedRange + " (" + GetRangeDescription(selectedRange) + ")",
                "",
                "CHART LEGEND",
                GetLegendText(selectedCategory),
                ""
            };

            AddSummaryLines(conn, lines);

            lines.Add("");
            lines.Add($"{selectedRange.ToUpperInvariant()} {selectedCategory.ToUpperInvariant()} REPORTS ONLY");
            lines.Add(new string('-', 80));

            DataTable table = selectedCategory switch
            {
                "Borrowing" => LoadBorrowingReportTable(conn),
                "Returns" => LoadReturnsReportTable(conn),
                "Fines" => LoadFinesReportTable(conn),
                _ => LoadGeneralReportTable(conn)
            };

            DataTable filteredTable = FilterReportTableBySelectedRange(table, selectedCategory, selectedRange);

            if (filteredTable.Rows.Count == 0)
            {
                lines.Add("No records found for the selected report and selected range.");
            }
            else
            {
                foreach (DataRow row in filteredTable.Rows)
                    lines.Add($"{row["Report ID"]} | {row["Report Name"]} | {row["Generated Date"]} | {row["Status"]}");
            }

            lines.Add("");
            AddGraphDataLines(conn, lines, selectedCategory);

            return lines;
        }

        private DataTable FilterReportTableBySelectedRange(DataTable source, string selectedCategory, string selectedRange)
        {
            // General summary rows are already live totals, so keep them for the selected General report.
            if (selectedCategory == "General")
                return source;

            DataTable filtered = source.Clone();

            foreach (DataRow row in source.Rows)
            {
                string dateText = Convert.ToString(row["Generated Date"]) ?? "";

                if (!DateTime.TryParse(dateText, out DateTime reportDate))
                    continue;

                if (IsDateInsideSelectedRange(reportDate, selectedRange))
                    filtered.ImportRow(row);
            }

            return filtered;
        }

        private bool IsDateInsideSelectedRange(DateTime date, string selectedRange)
        {
            DateTime todayLocal = DateTime.UtcNow.AddHours(ReportUtcOffsetHours).Date;
            DateTime target = date.Date;

            if (selectedRange == "Daily")
                return target == todayLocal;

            if (selectedRange == "Weekly")
            {
                int diff = ((int)todayLocal.DayOfWeek + 6) % 7;
                DateTime weekStart = todayLocal.AddDays(-diff);
                DateTime weekEnd = weekStart.AddDays(6);

                return target >= weekStart && target <= weekEnd;
            }

            if (selectedRange == "Monthly")
                return target.Year == todayLocal.Year && target.Month == todayLocal.Month;

            return target.Year == todayLocal.Year;
        }

        private void AddSummaryLines(SqlConnection conn, List<string> lines)
        {
            int totalRecords =
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Books WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.Users WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.ReturnRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID;");

            int recentActivity =
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.BorrowingRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= DATEADD(day, -30, SYSUTCDATETIME());") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.ReturnRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= DATEADD(day, -30, SYSUTCDATETIME());") +
                ExecuteCount(conn, "SELECT COUNT(*) FROM dbo.FineRecords WHERE ISNULL(IsArchived, 0) = 0 AND ClientID = @ClientID AND CreatedAt >= DATEADD(day, -30, SYSUTCDATETIME());");

            lines.Add("SUMMARY");
            lines.Add("Total Database Records: " + totalRecords.ToString("N0"));
            lines.Add("Recent Activity Volume: " + recentActivity.ToString("N0"));
            lines.Add("System Data Integrity: " + (statCard3.Tag is Control[] c && c.Length > 2 && c[2] is Label l ? l.Text : "100.0%"));
            lines.Add("");
        }

        private void AddGraphDataLines(SqlConnection conn, List<string> lines, string category)
        {
            string[] labels = GetChartLabels(currentRange);
            int[] completed = new int[labels.Length];
            int[] pending = new int[labels.Length];

            string previousCategory = currentCategory;
            currentCategory = category;

            if (currentRange == "Daily")
                LoadDailyGraphFromDatabase(conn, labels, completed, pending);
            else
                LoadRangeGraphFromDatabase(conn, currentRange, labels, completed, pending);

            currentCategory = previousCategory;

            lines.Add(currentRange + " Graph Data:");
            lines.Add(GetLegendText(category));

            for (int i = 0; i < labels.Length; i++)
            {
                string cleanLabel = labels[i].Replace("\n", " ");
                lines.Add($"{cleanLabel}: Main={completed[i]}, Alert={pending[i]}");
            }
        }

        private string GetLegendText(string category)
        {
            if (category == "Borrowing")
                return "Main = Borrowed books, Alert = Overdue books";

            if (category == "Returns")
                return "Main = Completed returns, Alert = Late returns";

            if (category == "Fines")
                return "Main = Paid fines, Alert = Unpaid fines";

            return "Main = Total activity, Alert = Pending alerts";
        }

        private sealed class SimplePdfWriter
        {
            public static void WriteTextPdf(string path, List<string> lines)
            {
                List<string> objects = new List<string>();
                List<int> pageObjectNumbers = new List<int>();

                const int linesPerPage = 42;
                int pageCount = Math.Max(1, (int)Math.Ceiling(lines.Count / (double)linesPerPage));

                for (int page = 0; page < pageCount; page++)
                {
                    List<string> pageLines = lines.Skip(page * linesPerPage).Take(linesPerPage).ToList();
                    string stream = BuildPageStream(pageLines, page + 1, pageCount);

                    int contentObjectNumber = objects.Count + 1;
                    objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream");

                    int pageObjectNumber = objects.Count + 1;
                    pageObjectNumbers.Add(pageObjectNumber);
                    objects.Add($"<< /Type /Page /Parent 0 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 0 0 R /F2 0 0 R >> >> /Contents {contentObjectNumber} 0 R >>");
                }

                int fontRegularNumber = objects.Count + 1;
                objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

                int fontBoldNumber = objects.Count + 1;
                objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");

                int pagesObjectNumber = objects.Count + 1;
                string kids = string.Join(" ", pageObjectNumbers.Select(n => $"{n} 0 R"));
                objects.Add($"<< /Type /Pages /Count {pageCount} /Kids [{kids}] >>");

                int catalogObjectNumber = objects.Count + 1;
                objects.Add($"<< /Type /Catalog /Pages {pagesObjectNumber} 0 R >>");

                for (int i = 0; i < objects.Count; i++)
                {
                    objects[i] = objects[i]
                        .Replace("/Parent 0 0 R", $"/Parent {pagesObjectNumber} 0 R")
                        .Replace("/F1 0 0 R", $"/F1 {fontRegularNumber} 0 R")
                        .Replace("/F2 0 0 R", $"/F2 {fontBoldNumber} 0 R");
                }

                using FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                using StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);

                writer.WriteLine("%PDF-1.4");

                List<long> offsets = new List<long> { 0 };

                for (int i = 0; i < objects.Count; i++)
                {
                    writer.Flush();
                    offsets.Add(fs.Position);
                    writer.WriteLine($"{i + 1} 0 obj");
                    writer.WriteLine(objects[i]);
                    writer.WriteLine("endobj");
                }

                writer.Flush();
                long xrefPosition = fs.Position;

                writer.WriteLine("xref");
                writer.WriteLine($"0 {objects.Count + 1}");
                writer.WriteLine("0000000000 65535 f ");

                for (int i = 1; i < offsets.Count; i++)
                    writer.WriteLine(offsets[i].ToString("0000000000") + " 00000 n ");

                writer.WriteLine("trailer");
                writer.WriteLine($"<< /Size {objects.Count + 1} /Root {catalogObjectNumber} 0 R >>");
                writer.WriteLine("startxref");
                writer.WriteLine(xrefPosition);
                writer.WriteLine("%%EOF");
            }

            private static string BuildPageStream(List<string> lines, int pageNumber, int pageCount)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("BT");
                sb.AppendLine("/F2 16 Tf");
                sb.AppendLine("50 750 Td");
                sb.AppendLine("(" + EscapePdf("Library Reports") + ") Tj");
                sb.AppendLine("/F1 9 Tf");
                sb.AppendLine("0 -18 Td");

                foreach (string raw in lines)
                {
                    string line = raw.Length > 105 ? raw.Substring(0, 105) : raw;
                    sb.AppendLine("(" + EscapePdf(line) + ") Tj");
                    sb.AppendLine("0 -14 Td");
                }

                sb.AppendLine("/F1 8 Tf");
                sb.AppendLine($"0 -20 Td");
                sb.AppendLine("(" + EscapePdf($"Page {pageNumber} of {pageCount}") + ") Tj");
                sb.AppendLine("ET");

                return sb.ToString();
            }

            private static string EscapePdf(string text)
            {
                return text
                    .Replace("\\", "\\\\")
                    .Replace("(", "\\(")
                    .Replace(")", "\\)")
                    .Replace("₱", "PHP ");
            }
        }

        private void ApplySearch(string keyword)
        {
            keyword = keyword.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                dgvReports.DataSource = reportTable;
                return;
            }

            var filtered = reportTable.AsEnumerable()
                .Where(row => row.ItemArray.Any(field =>
                    (field?.ToString() ?? "").ToLower().Contains(keyword)));

            dgvReports.DataSource = filtered.Any()
                ? filtered.CopyToDataTable()
                : null;
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 26;
            int width = Math.Max(980, canvas.ClientSize.Width - margin * 2);

            lblTitle.Location = new Point(margin, 34);
            lblSubtitle.Location = new Point(margin, 82);
            btnExportPdf.Bounds = new Rectangle(width - 175 + margin, 50, 175, 52);

            int leftWidth = (int)(width * 0.66);
            int rightWidth = width - leftWidth - gap;

            datasetCard.Bounds = new Rectangle(margin, 140, leftWidth, 250);

            if (datasetCard.Tag is Control[] d)
            {
                d[0].Location = new Point(28, 92);
                d[1].Location = new Point(28, 118);

                int btnTop = 162;
                int btnW = (datasetCard.Width - 56) / 4;

                btnGeneral.Bounds = new Rectangle(28, btnTop, btnW - 8, 44);
                btnBorrowing.Bounds = new Rectangle(btnGeneral.Right + 8, btnTop, btnW - 8, 44);
                btnReturns.Bounds = new Rectangle(btnBorrowing.Right + 8, btnTop, btnW - 8, 44);
                btnFines.Bounds = new Rectangle(btnReturns.Right + 8, btnTop, btnW - 8, 44);
            }

            statCard1.Bounds = new Rectangle(datasetCard.Right + gap, 140, rightWidth, 76);
            statCard2.Bounds = new Rectangle(datasetCard.Right + gap, statCard1.Bottom + 14, rightWidth, 76);
            statCard3.Bounds = new Rectangle(datasetCard.Right + gap, statCard2.Bottom + 14, rightWidth, 76);

            LayoutStatCard(statCard1);
            LayoutStatCard(statCard2);
            LayoutStatCard(statCard3);

            chartCard.Bounds = new Rectangle(margin, datasetCard.Bottom + 28, width, 390);

            lblChartTitle.Location = new Point(28, 28);
            lblChartSubtitle.Location = new Point(28, 62);
            cmbRange.Bounds = new Rectangle(chartCard.Width - 160, 34, 120, 32);

            chartLegendPanel.Bounds = new Rectangle(chartCard.Width - 500, 70, 460, 36);
            mainLegendDot.Location = new Point(0, 7);
            lblMainLegend.Location = new Point(20, 2);
            alertLegendDot.Location = new Point(230, 7);
            lblAlertLegend.Location = new Point(250, 2);

            reportsChart.Bounds = new Rectangle(36, 120, chartCard.Width - 72, chartCard.Height - 140);

            tableCard.Bounds = new Rectangle(margin, chartCard.Bottom + 28, width, 330);
            lblTableTitle.Location = new Point(28, 24);

            searchPanel.Bounds = new Rectangle(tableCard.Width - 330, 20, 300, 38);
            searchPanel.Controls[0].Location = new Point(12, 7);
            txtSearch.Location = new Point(42, 9);
            txtSearch.Width = searchPanel.Width - 54;

            dgvReports.Bounds = new Rectangle(0, 76, tableCard.Width, tableCard.Height - 86);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 30);
        }

        private void LayoutStatCard(Panel card)
        {
            if (card.Tag is not Control[] c) return;

            c[0].Location = new Point(22, 12);
            c[1].Location = new Point(88, 15);
            c[2].Location = new Point(88, 38);
        }

        private Panel CreateRoundedCard()
        {
            Panel p = new Panel
            {
                BackColor = CardBack
            };
            p.Paint += RoundedPanelPaint;
            return p;
        }

        private void ClearGridSelection()
        {
            if (dgvReports == null) return;
            dgvReports.ClearSelection();
            dgvReports.CurrentCell = null;
        }

        private void CircleDotPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control dot) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(dot.BackColor);
            e.Graphics.Clear(HeaderBack);
            e.Graphics.FillEllipse(brush, 0, 0, dot.Width - 1, dot.Height - 1);
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control p) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var brush = new SolidBrush(p.BackColor);
            using var path = GetRoundedRectPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 18);
            e.Graphics.FillPath(brush, path);
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}