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

        private Panel chartPanel = null!;
        private Label lblChartTitle = null!;
        private Chart chartStatus = null!;

        private Panel alertsPanel = null!;
        private Label lblAlertsTitle = null!;
        private ListBox lstAlerts = null!;

        private Panel tablePanel = null!;
        private Label lblTableTitle = null!;
        private DataGridView dgvActivityLogs = null!;

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
            topPanel.Height = 120;

            lblTitle = new Label();
            lblTitle.Text = "System Activity & Monitoring";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 12);

            lblSubInfo = new Label();
            lblSubInfo.Text = "Track client status, alerts, and recent system activities.";
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

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblSubInfo);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(btnRefresh);

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
            lblChartTitle.Text = "Client Status Overview";
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
            lblAlertsTitle.Text = "System Alerts";
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
            lblTableTitle.Text = "Recent Activity Logs";
            lblTableTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTableTitle.ForeColor = Color.Maroon;
            lblTableTitle.AutoSize = true;
            lblTableTitle.Location = new Point(15, 12);

            dgvActivityLogs = new DataGridView();
            dgvActivityLogs.Location = new Point(15, 42);
            dgvActivityLogs.Size = new Size(985, 225);
            dgvActivityLogs.BackgroundColor = Color.White;
            dgvActivityLogs.BorderStyle = BorderStyle.None;
            dgvActivityLogs.RowHeadersVisible = false;
            dgvActivityLogs.AllowUserToAddRows = false;
            dgvActivityLogs.AllowUserToResizeRows = false;
            dgvActivityLogs.AllowUserToResizeColumns = false;
            dgvActivityLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvActivityLogs.MultiSelect = false;
            dgvActivityLogs.ReadOnly = true;
            dgvActivityLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvActivityLogs.EnableHeadersVisualStyles = false;
            dgvActivityLogs.ColumnHeadersHeight = 35;
            dgvActivityLogs.RowTemplate.Height = 35;
            dgvActivityLogs.GridColor = Color.Gainsboro;
            dgvActivityLogs.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvActivityLogs.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgvActivityLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvActivityLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvActivityLogs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvActivityLogs.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
            dgvActivityLogs.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            dgvActivityLogs.DefaultCellStyle.BackColor = Color.White;
            dgvActivityLogs.DefaultCellStyle.ForeColor = Color.Black;
            dgvActivityLogs.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvActivityLogs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvActivityLogs.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvActivityLogs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            dgvActivityLogs.Columns.Add("ActivityID", "Activity ID");
            dgvActivityLogs.Columns.Add("ClientLibrary", "Client Library");
            dgvActivityLogs.Columns.Add("ActivityType", "Activity Type");
            dgvActivityLogs.Columns.Add("Role", "Role");
            dgvActivityLogs.Columns.Add("DateTime", "Date / Time");
            dgvActivityLogs.Columns.Add("Status", "Status");

            foreach (DataGridViewColumn col in dgvActivityLogs.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            tablePanel.Controls.Add(lblTableTitle);
            tablePanel.Controls.Add(dgvActivityLogs);

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

            dgvActivityLogs.Size = new Size(tablePanel.Width - 35, 225);

            contentPanel.AutoScrollMinSize = new Size(0, tablePanel.Bottom + 40);
        }

        private void LoadMonitoringData()
        {
            int totalClients = ClientArchiveStore.ActiveClients.Count + ClientArchiveStore.ArchivedClients.Count;
            int activeClients = ClientArchiveStore.ActiveClients.Count(c => c.Status == "Active");
            int inactiveClients = ClientArchiveStore.ActiveClients.Count(c => c.Status == "Inactive");
            int archivedClients = ClientArchiveStore.ArchivedClients.Count;

            chartStatus.Series.Clear();
            chartStatus.Titles.Clear();

            Series series = new Series("Clients");
            series.ChartType = SeriesChartType.Doughnut;
            series.IsValueShownAsLabel = true;
            series.BorderWidth = 2;

            if (activeClients == 0 && inactiveClients == 0 && archivedClients == 0)
            {
                // DEMO VALUES
                activeClients = 3;
                inactiveClients = 1;
                archivedClients = 1;
            }

            series.Points.AddXY("Active", activeClients);
            series.Points.AddXY("Inactive", inactiveClients);
            series.Points.AddXY("Archived", archivedClients);

            series.Points[0].Color = Color.RoyalBlue;
            series.Points[1].Color = Color.Goldenrod;
            series.Points[2].Color = Color.Firebrick;

            chartStatus.Series.Add(series);

            lstAlerts.Items.Clear();
            lstAlerts.Items.Add("• Total clients in system: " + totalClients);
            lstAlerts.Items.Add("• Active clients: " + activeClients);
            lstAlerts.Items.Add("• Archived clients: " + archivedClients);
            lstAlerts.Items.Add("• LMN Center is inactive");
            lstAlerts.Items.Add("• 3 failed login attempts today");
            lstAlerts.Items.Add("• System status: Online");
            lstAlerts.Items.Add("• Database status: Connected");

            dgvActivityLogs.Rows.Clear();
            dgvActivityLogs.Rows.Add("ACT001", "ABC School Library", "Login", "Admin/Client", DateTime.Now.AddMinutes(-45).ToString("MM/dd/yyyy hh:mm tt"), "Success");
            dgvActivityLogs.Rows.Add("ACT002", "XYZ College Library", "Borrow Book", "Librarian", DateTime.Now.AddMinutes(-30).ToString("MM/dd/yyyy hh:mm tt"), "Success");
            dgvActivityLogs.Rows.Add("ACT003", "LMN Center", "Status Check", "Super Admin", DateTime.Now.AddMinutes(-20).ToString("MM/dd/yyyy hh:mm tt"), "Reviewed");
            dgvActivityLogs.Rows.Add("ACT004", "ABC School Library", "Member Update", "Admin/Client", DateTime.Now.AddMinutes(-10).ToString("MM/dd/yyyy hh:mm tt"), "Completed");
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            foreach (DataGridViewRow row in dgvActivityLogs.Rows)
            {
                bool visible =
                    row.Cells["ActivityID"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["ClientLibrary"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["ActivityType"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["Role"].Value?.ToString()?.ToLower().Contains(keyword) == true ||
                    row.Cells["Status"].Value?.ToString()?.ToLower().Contains(keyword) == true;

                row.Visible = string.IsNullOrWhiteSpace(keyword) || visible;
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadMonitoringData();

            foreach (DataGridViewRow row in dgvActivityLogs.Rows)
            {
                row.Visible = true;
            }
        }
    }
}