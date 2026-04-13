using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EducationSystem
{
    public partial class MonitoringForm : Form
    {
        private Label lblTitle = null!;
        private Label lblSubInfo = null!;
        private TextBox txtSearch = null!;
        private Button btnSearch = null!;
        private Button btnRefresh = null!;

        private Panel topPanel = null!;
        private Panel contentPanel = null!;

        private Panel summaryPanel = null!;
        private Label lblLibrarians = null!;
        private Label lblMembers = null!;
        private Label lblBooks = null!;
        private Label lblBorrows = null!;
        private Label lblReturns = null!;
        private Label lblFines = null!;

        private Panel chartPanel = null!;
        private Label lblChartTitle = null!;
        private Chart chartStatus = null!;

        private Panel alertsPanel = null!;
        private Label lblAlertsTitle = null!;
        private ListBox lstAlerts = null!;

        private Panel tablePanel = null!;
        private Label lblTableTitle = null!;
        private DataGridView dgvMonitoringLogs = null!;

        public MonitoringForm()
        {
            InitializeComponent();
            BuildMonitoringUI();
            LoadMonitoringData();
        }

        private void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(210, 210, 210);
            btn.UseVisualStyleBackColor = false;

            btn.BackColor = Color.Maroon;
            btn.ForeColor = Color.WhiteSmoke;

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(230, 230, 230);
                btn.ForeColor = Color.Black;
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.Maroon;
                btn.ForeColor = Color.WhiteSmoke;
            };
        }

        private Label CreateSummaryLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Maroon,
                AutoSize = true,
                Location = location
            };
        }

        private void BuildMonitoringUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = false;

            topPanel = new Panel();
            topPanel.BackColor = Color.Snow;
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 155;

            lblTitle = new Label();
            lblTitle.Text = "Library Activity Overview";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 12);

            lblSubInfo = new Label();
            lblSubInfo.Text = "Monitor librarians, members, book catalogs, borrow/return records, fines, reports, and system status.";
            lblSubInfo.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblSubInfo.ForeColor = Color.DimGray;
            lblSubInfo.AutoSize = true;
            lblSubInfo.Location = new Point(32, 42);

            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Size = new Size(250, 30);
            txtSearch.Location = new Point(30, 72);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.BackColor = Color.White;

            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSearch.Size = new Size(90, 30);
            btnSearch.Location = new Point(290, 72);
            StyleButton(btnSearch);
            btnSearch.Click += BtnSearch_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Size = new Size(100, 32);
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(btnRefresh);
            btnRefresh.Click += BtnRefresh_Click;

            summaryPanel = new Panel();
            summaryPanel.BackColor = Color.Snow;
            summaryPanel.Location = new Point(30, 110);
            summaryPanel.Size = new Size(1000, 30);

            lblLibrarians = CreateSummaryLabel("Librarians: 0", new Point(0, 5));
            lblMembers = CreateSummaryLabel("Members: 0", new Point(140, 5));
            lblBooks = CreateSummaryLabel("Book Catalogs: 0", new Point(270, 5));
            lblBorrows = CreateSummaryLabel("Borrow Records: 0", new Point(450, 5));
            lblReturns = CreateSummaryLabel("Return Records: 0", new Point(640, 5));
            lblFines = CreateSummaryLabel("Fine Records: 0", new Point(820, 5));

            summaryPanel.Controls.Add(lblLibrarians);
            summaryPanel.Controls.Add(lblMembers);
            summaryPanel.Controls.Add(lblBooks);
            summaryPanel.Controls.Add(lblBorrows);
            summaryPanel.Controls.Add(lblReturns);
            summaryPanel.Controls.Add(lblFines);

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblSubInfo);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(btnRefresh);
            topPanel.Controls.Add(summaryPanel);

            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.Snow;
            contentPanel.Padding = new Padding(30, 10, 30, 20);
            contentPanel.AutoScroll = true;

            chartPanel = new Panel();
            chartPanel.BackColor = Color.White;
            chartPanel.BorderStyle = BorderStyle.FixedSingle;
            chartPanel.Location = new Point(30, 20);
            chartPanel.Size = new Size(650, 280);

            lblChartTitle = new Label();
            lblChartTitle.Text = "System Status Overview";
            lblChartTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblChartTitle.ForeColor = Color.Maroon;
            lblChartTitle.AutoSize = true;
            lblChartTitle.Location = new Point(15, 12);

            chartStatus = new Chart();
            chartStatus.Location = new Point(15, 42);
            chartStatus.Size = new Size(615, 215);
            chartStatus.BackColor = Color.White;

            ChartArea chartArea = new ChartArea("StatusArea");
            chartArea.BackColor = Color.White;
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartStatus.ChartAreas.Add(chartArea);

            Legend legend = new Legend("MainLegend");
            legend.Docking = Docking.Bottom;
            chartStatus.Legends.Add(legend);

            chartPanel.Controls.Add(lblChartTitle);
            chartPanel.Controls.Add(chartStatus);

            alertsPanel = new Panel();
            alertsPanel.BackColor = Color.FromArgb(245, 245, 245);
            alertsPanel.BorderStyle = BorderStyle.FixedSingle;
            alertsPanel.Location = new Point(700, 20);
            alertsPanel.Size = new Size(350, 280);

            lblAlertsTitle = new Label();
            lblAlertsTitle.Text = "Monitoring Alerts";
            lblAlertsTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblAlertsTitle.ForeColor = Color.DarkRed;
            lblAlertsTitle.AutoSize = true;
            lblAlertsTitle.Location = new Point(15, 12);

            lstAlerts = new ListBox();
            lstAlerts.Location = new Point(15, 42);
            lstAlerts.Size = new Size(315, 205);
            lstAlerts.Font = new Font("Segoe UI", 10);
            lstAlerts.BorderStyle = BorderStyle.None;
            lstAlerts.BackColor = Color.FromArgb(245, 245, 245);

            alertsPanel.Controls.Add(lblAlertsTitle);
            alertsPanel.Controls.Add(lstAlerts);

            tablePanel = new Panel();
            tablePanel.BackColor = Color.White;
            tablePanel.BorderStyle = BorderStyle.FixedSingle;
            tablePanel.Location = new Point(30, 320);
            tablePanel.Size = new Size(1020, 290);

            lblTableTitle = new Label();
            lblTableTitle.Text = "Recent Monitoring Logs";
            lblTableTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTableTitle.ForeColor = Color.Maroon;
            lblTableTitle.AutoSize = true;
            lblTableTitle.Location = new Point(15, 12);

            dgvMonitoringLogs = new DataGridView();
            dgvMonitoringLogs.Location = new Point(15, 42);
            dgvMonitoringLogs.Size = new Size(985, 225);
            dgvMonitoringLogs.BackgroundColor = Color.White;
            dgvMonitoringLogs.BorderStyle = BorderStyle.None;
            dgvMonitoringLogs.RowHeadersVisible = false;
            dgvMonitoringLogs.AllowUserToAddRows = false;
            dgvMonitoringLogs.AllowUserToResizeRows = false;
            dgvMonitoringLogs.AllowUserToResizeColumns = false;
            dgvMonitoringLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMonitoringLogs.MultiSelect = false;
            dgvMonitoringLogs.ReadOnly = true;
            dgvMonitoringLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMonitoringLogs.EnableHeadersVisualStyles = false;
            dgvMonitoringLogs.ColumnHeadersHeight = 35;
            dgvMonitoringLogs.RowTemplate.Height = 35;
            dgvMonitoringLogs.GridColor = Color.Gainsboro;
            dgvMonitoringLogs.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvMonitoringLogs.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgvMonitoringLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvMonitoringLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvMonitoringLogs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMonitoringLogs.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
            dgvMonitoringLogs.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            dgvMonitoringLogs.DefaultCellStyle.BackColor = Color.White;
            dgvMonitoringLogs.DefaultCellStyle.ForeColor = Color.Black;
            dgvMonitoringLogs.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvMonitoringLogs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvMonitoringLogs.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvMonitoringLogs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            dgvMonitoringLogs.Columns.Add("LogID", "Log ID");
            dgvMonitoringLogs.Columns.Add("Module", "Module");
            dgvMonitoringLogs.Columns.Add("RecordName", "Record Name");
            dgvMonitoringLogs.Columns.Add("Action", "Action");
            dgvMonitoringLogs.Columns.Add("CheckedBy", "Checked By");
            dgvMonitoringLogs.Columns.Add("DateTime", "Date / Time");
            dgvMonitoringLogs.Columns.Add("Status", "Status");

            foreach (DataGridViewColumn col in dgvMonitoringLogs.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            tablePanel.Controls.Add(lblTableTitle);
            tablePanel.Controls.Add(dgvMonitoringLogs);

            contentPanel.Controls.Add(chartPanel);
            contentPanel.Controls.Add(alertsPanel);
            contentPanel.Controls.Add(tablePanel);

            Controls.Add(contentPanel);
            Controls.Add(topPanel);

            Resize += MonitoringForm_Resize;
            AdjustLayout();
        }

        private void MonitoringForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (btnRefresh == null) return;

            btnRefresh.Location = new Point(ClientSize.Width - btnRefresh.Width - 30, 71);

            int contentWidth = ClientSize.Width - 60;
            int left = 30;
            int gap = 20;
            int rightPanelWidth = 350;
            int chartWidth = contentWidth - rightPanelWidth - gap;

            chartPanel.Location = new Point(left, 20);
            chartPanel.Size = new Size(Math.Max(500, chartWidth), 280);

            chartStatus.Size = new Size(chartPanel.Width - 35, 215);

            alertsPanel.Location = new Point(chartPanel.Right + gap, 20);
            alertsPanel.Size = new Size(rightPanelWidth, 280);

            lstAlerts.Size = new Size(alertsPanel.Width - 35, 205);

            tablePanel.Location = new Point(left, 320);
            tablePanel.Size = new Size(contentWidth, 290);

            dgvMonitoringLogs.Size = new Size(tablePanel.Width - 35, 225);

            contentPanel.AutoScrollMinSize = new Size(0, tablePanel.Bottom + 40);
        }

        private void LoadMonitoringData()
        {
            int librarians = 4;
            int members = 25;
            int bookCatalogs = 120;
            int borrowRecords = 38;
            int returnRecords = 32;
            int fineRecords = 6;

            lblLibrarians.Text = $"Librarians: {librarians}";
            lblMembers.Text = $"Members: {members}";
            lblBooks.Text = $"Book Catalogs: {bookCatalogs}";
            lblBorrows.Text = $"Borrow Records: {borrowRecords}";
            lblReturns.Text = $"Return Records: {returnRecords}";
            lblFines.Text = $"Fine Records: {fineRecords}";

            chartStatus.Series.Clear();
            chartStatus.Titles.Clear();

            Series series = new Series("Monitoring");
            series.ChartType = SeriesChartType.Doughnut;
            series.IsValueShownAsLabel = true;
            series.BorderWidth = 2;

            series.Points.AddXY("Borrow", borrowRecords);
            series.Points.AddXY("Return", returnRecords);
            series.Points.AddXY("Fines", fineRecords);

            series.Points[0].Color = Color.RoyalBlue;
            series.Points[1].Color = Color.Goldenrod;
            series.Points[2].Color = Color.Firebrick;

            chartStatus.Series.Add(series);

            lstAlerts.Items.Clear();
            lstAlerts.Items.Add("• Total librarians monitored: " + librarians);
            lstAlerts.Items.Add("• Total members monitored: " + members);
            lstAlerts.Items.Add("• Book catalog records available: " + bookCatalogs);
            lstAlerts.Items.Add("• Borrow records under review: " + borrowRecords);
            lstAlerts.Items.Add("• Return records under review: " + returnRecords);
            lstAlerts.Items.Add("• Fine records found: " + fineRecords);
            lstAlerts.Items.Add("• Reports available for review");
            lstAlerts.Items.Add("• System status: Online");
            lstAlerts.Items.Add("• Database status: Connected");

            dgvMonitoringLogs.Rows.Clear();
            dgvMonitoringLogs.Rows.Add("MON001", "Librarians", "Maria Santos", "View Librarian", "Super Admin", DateTime.Now.AddMinutes(-45).ToString("MM/dd/yyyy hh:mm tt"), "Reviewed");
            dgvMonitoringLogs.Rows.Add("MON002", "Members", "John Cruz", "View Member", "Super Admin", DateTime.Now.AddMinutes(-35).ToString("MM/dd/yyyy hh:mm tt"), "Reviewed");
            dgvMonitoringLogs.Rows.Add("MON003", "Book Catalog", "Database Systems", "View Book Catalog", "Super Admin", DateTime.Now.AddMinutes(-28).ToString("MM/dd/yyyy hh:mm tt"), "Reviewed");
            dgvMonitoringLogs.Rows.Add("MON004", "Borrow Records", "BR001", "View Borrow Record", "Super Admin", DateTime.Now.AddMinutes(-20).ToString("MM/dd/yyyy hh:mm tt"), "Reviewed");
            dgvMonitoringLogs.Rows.Add("MON005", "Return Records", "RT001", "View Return Record", "Super Admin", DateTime.Now.AddMinutes(-14).ToString("MM/dd/yyyy hh:mm tt"), "Reviewed");
            dgvMonitoringLogs.Rows.Add("MON006", "Fine Records", "FN001", "View Fine Record", "Super Admin", DateTime.Now.AddMinutes(-9).ToString("MM/dd/yyyy hh:mm tt"), "Reviewed");
            dgvMonitoringLogs.Rows.Add("MON007", "Reports", "Monthly Usage Report", "View Report", "Super Admin", DateTime.Now.AddMinutes(-4).ToString("MM/dd/yyyy hh:mm tt"), "Reviewed");
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            foreach (DataGridViewRow row in dgvMonitoringLogs.Rows)
            {
                bool visible =
                    row.Cells["LogID"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["Module"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["RecordName"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["Action"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["CheckedBy"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["Status"].Value?.ToString()?.ToLower().Contains(keyword) == true;

                row.Visible = string.IsNullOrWhiteSpace(keyword) || visible;
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadMonitoringData();

            foreach (DataGridViewRow row in dgvMonitoringLogs.Rows)
            {
                row.Visible = true;
            }
        }
    }
}