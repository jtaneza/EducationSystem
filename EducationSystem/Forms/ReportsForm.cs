using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ReportsForm : Form
    {
        private Label lblTitle = null!;
        private Label lblSubInfo = null!;

        private TextBox txtSearch = null!;
        private ComboBox cmbCategory = null!;
        private ComboBox cmbDateRange = null!;
        private DateTimePicker dtpFrom = null!;
        private DateTimePicker dtpTo = null!;
        private Button btnSearch = null!;
        private Button btnGenerate = null!;
        private Button btnDownloadReport = null!;

        private ContextMenuStrip downloadMenu = null!;
        private ToolStripMenuItem csvMenuItem = null!;
        private ToolStripMenuItem pdfMenuItem = null!;

        private Panel topPanel = null!;
        private Panel tablePanel = null!;
        private Panel bottomPanel = null!;
        private DataGridView dgvReports = null!;

        public ReportsForm()
        {
            InitializeComponent();
            BuildReportsUI();
            SeedReportsOnce();
            ApplyDateRangeSelection();
            LoadReportsData();
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

        private void BuildReportsUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            topPanel = new Panel();
            topPanel.BackColor = Color.Snow;
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 150;

            lblTitle = new Label();
            lblTitle.Text = "System Reports";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 14);

            lblSubInfo = new Label();
            lblSubInfo.Text = "Generate and manage reports by category and date range.";
            lblSubInfo.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblSubInfo.ForeColor = Color.DimGray;
            lblSubInfo.AutoSize = true;
            lblSubInfo.Location = new Point(32, 44);

            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Size = new Size(220, 30);
            txtSearch.Location = new Point(30, 82);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.BackColor = Color.White;

            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Size = new Size(180, 30);
            txtSearch.Location = new Point(30, 82);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.BackColor = Color.White;

            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSearch.Size = new Size(90, 32);
            btnSearch.Location = new Point(220, 80);
            StyleButton(btnSearch);
            btnSearch.Click += BtnSearch_Click;

            cmbCategory = new ComboBox();
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Font = new Font("Segoe UI", 10);
            cmbCategory.Size = new Size(120, 30);
            cmbCategory.Location = new Point(325, 82);
            cmbCategory.Items.AddRange(new object[]
            {
            "All",
            "Clients",
            "Monitoring",
            "Modules",
            "Archive"
            });
            cmbCategory.SelectedIndex = 0;

            cmbDateRange = new ComboBox();
            cmbDateRange.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDateRange.Font = new Font("Segoe UI", 10);
            cmbDateRange.Size = new Size(130, 30);
            cmbDateRange.Location = new Point(460, 82);
            cmbDateRange.Items.AddRange(new object[]
            {
            "Daily",
            "Weekly",
            "Monthly",
            "Custom Range"
            });
            cmbDateRange.SelectedIndex = 0;
            cmbDateRange.SelectedIndexChanged += CmbDateRange_SelectedIndexChanged;

            dtpFrom = new DateTimePicker();
            dtpFrom.Font = new Font("Segoe UI", 10);
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.ShowUpDown = false;
            dtpFrom.Size = new Size(125, 30);
            dtpFrom.Location = new Point(605, 82);

            dtpTo = new DateTimePicker();
            dtpTo.Font = new Font("Segoe UI", 10);
            dtpTo.Format = DateTimePickerFormat.Short;
            dtpTo.ShowUpDown = false;
            dtpTo.Size = new Size(125, 30);
            dtpTo.Location = new Point(745, 82);

            btnGenerate = new Button();
            btnGenerate.Text = "Generate Report";
            btnGenerate.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnGenerate.Size = new Size(150, 34);
            btnGenerate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(btnGenerate);
            btnGenerate.Click += BtnGenerate_Click;

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblSubInfo);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(cmbCategory);
            topPanel.Controls.Add(cmbDateRange);
            topPanel.Controls.Add(dtpFrom);
            topPanel.Controls.Add(dtpTo);
            topPanel.Controls.Add(btnGenerate);

            tablePanel = new Panel();
            tablePanel.BackColor = Color.White;
            tablePanel.Dock = DockStyle.Fill;
            tablePanel.Padding = new Padding(30, 18, 30, 12);

            dgvReports = new DataGridView();
            dgvReports.Dock = DockStyle.Fill;
            dgvReports.BackgroundColor = Color.White;
            dgvReports.BorderStyle = BorderStyle.None;
            dgvReports.RowHeadersVisible = false;
            dgvReports.AllowUserToAddRows = false;
            dgvReports.AllowUserToResizeRows = false;
            dgvReports.AllowUserToResizeColumns = false;
            dgvReports.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReports.MultiSelect = false;
            dgvReports.ReadOnly = true;
            dgvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReports.EnableHeadersVisualStyles = false;
            dgvReports.ColumnHeadersHeight = 38;
            dgvReports.RowTemplate.Height = 38;
            dgvReports.GridColor = Color.Gainsboro;
            dgvReports.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReports.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgvReports.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvReports.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvReports.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvReports.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
            dgvReports.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            dgvReports.DefaultCellStyle.BackColor = Color.White;
            dgvReports.DefaultCellStyle.ForeColor = Color.Black;
            dgvReports.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvReports.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvReports.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvReports.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            dgvReports.Columns.Add("ReportID", "Report ID");
            dgvReports.Columns.Add("ReportName", "Report Name");
            dgvReports.Columns.Add("Category", "Category");
            dgvReports.Columns.Add("DateRangeType", "Date Range");
            dgvReports.Columns.Add("GeneratedDate", "Date Generated");
            dgvReports.Columns.Add("GeneratedBy", "Generated By");
            dgvReports.Columns.Add("Status", "Status");

            DataGridViewTextBoxColumn actionCol = new DataGridViewTextBoxColumn();
            actionCol.Name = "Actions";
            actionCol.HeaderText = "Actions";
            actionCol.Width = 150;
            actionCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            actionCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            actionCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvReports.Columns.Add(actionCol);

            foreach (DataGridViewColumn col in dgvReports.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvReports.Columns["ReportID"].FillWeight = 12;
            dgvReports.Columns["ReportName"].FillWeight = 24;
            dgvReports.Columns["Category"].FillWeight = 12;
            dgvReports.Columns["DateRangeType"].FillWeight = 12;
            dgvReports.Columns["GeneratedDate"].FillWeight = 18;
            dgvReports.Columns["GeneratedBy"].FillWeight = 13;
            dgvReports.Columns["Status"].FillWeight = 10;
            dgvReports.Columns["Actions"].FillWeight = 12;

            dgvReports.CellClick += DgvReports_CellClick;

            tablePanel.Controls.Add(dgvReports);

            bottomPanel = new Panel();
            bottomPanel.BackColor = Color.Snow;
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 65;
            bottomPanel.Padding = new Padding(30, 10, 30, 15);

            btnDownloadReport = new Button();
            btnDownloadReport.Text = "Download Report";
            btnDownloadReport.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDownloadReport.Size = new Size(155, 36);
            btnDownloadReport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(btnDownloadReport);
            btnDownloadReport.Click += BtnDownloadReport_Click;

            bottomPanel.Controls.Add(btnDownloadReport);

            downloadMenu = new ContextMenuStrip();
            csvMenuItem = new ToolStripMenuItem("CSV");
            pdfMenuItem = new ToolStripMenuItem("PDF");

            csvMenuItem.Click += CsvMenuItem_Click;
            pdfMenuItem.Click += PdfMenuItem_Click;

            downloadMenu.Items.Add(csvMenuItem);
            downloadMenu.Items.Add(pdfMenuItem);

            Controls.Add(tablePanel);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);

            Resize += ReportsForm_Resize;
            AdjustLayout();

            this.AcceptButton = btnSearch;
        }

        private void ReportsForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (btnGenerate != null)
            {
                btnGenerate.Location = new Point(ClientSize.Width - btnGenerate.Width - 30, 80);
            }

            if (btnDownloadReport != null)
            {
                btnDownloadReport.Location = new Point(ClientSize.Width - btnDownloadReport.Width - 30, 12);
            }
        }

        private void SeedReportsOnce()
        {
            if (ReportStore.IsSeeded) return;

            ReportStore.Reports.Add(new ReportItem
            {
                ReportID = "REP001",
                ReportName = "Client Summary Report",
                Category = "Clients",
                DateRangeType = "Monthly",
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
                GeneratedDate = DateTime.Now.AddMinutes(-30),
                GeneratedBy = "Super Admin",
                Status = "Ready"
            });

            ReportStore.Reports.Add(new ReportItem
            {
                ReportID = "REP002",
                ReportName = "Monitoring Activity Log",
                Category = "Monitoring",
                DateRangeType = "Weekly",
                FromDate = DateTime.Now.Date.AddDays(-6),
                ToDate = DateTime.Now.Date,
                GeneratedDate = DateTime.Now.AddMinutes(-20),
                GeneratedBy = "Super Admin",
                Status = "Ready"
            });

            ReportStore.Reports.Add(new ReportItem
            {
                ReportID = "REP003",
                ReportName = "Archive History Report",
                Category = "Archive",
                DateRangeType = "Daily",
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                GeneratedDate = DateTime.Now.AddMinutes(-10),
                GeneratedBy = "Super Admin",
                Status = "Ready"
            });

            ReportStore.IsSeeded = true;
        }

        private void LoadReportsData()
        {
            BindReportsToGrid(ReportStore.Reports.OrderByDescending(r => r.GeneratedDate).ToList());
        }

        private void BindReportsToGrid(System.Collections.Generic.IEnumerable<ReportItem> reports)
        {
            dgvReports.Rows.Clear();

            foreach (var report in reports)
            {
                dgvReports.Rows.Add(
                    report.ReportID,
                    report.ReportName,
                    report.Category,
                    report.DateRangeType,
                    report.GeneratedDate.ToString("MM/dd/yyyy hh:mm tt"),
                    report.GeneratedBy,
                    report.Status,
                    "👁 View    📦 Archive"
                );
            }

            dgvReports.ClearSelection();
        }

        private string GenerateNextReportId()
        {
            if (!ReportStore.Reports.Any())
                return "REP001";

            int max = ReportStore.Reports
                .Select(r =>
                {
                    string numberPart = r.ReportID.Replace("REP", "");
                    return int.TryParse(numberPart, out int n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            return "REP" + (max + 1).ToString("D3");
        }

        private void CmbDateRange_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ApplyDateRangeSelection();
        }

        private void ApplyDateRangeSelection()
        {
            string selected = cmbDateRange.SelectedItem?.ToString() ?? "Daily";
            DateTime today = DateTime.Today;

            // Always allow clicking calendar
            dtpFrom.Enabled = true;
            dtpTo.Enabled = true;

            if (selected == "Daily")
            {
                dtpFrom.Value = today;
                dtpTo.Value = today;
            }
            else if (selected == "Weekly")
            {
                int diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
                DateTime startOfWeek = today.AddDays(-diff);
                DateTime endOfWeek = startOfWeek.AddDays(6);

                dtpFrom.Value = startOfWeek;
                dtpTo.Value = endOfWeek;
            }
            else if (selected == "Monthly")
            {
                DateTime firstDay = new DateTime(today.Year, today.Month, 1);
                DateTime lastDay = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                dtpFrom.Value = firstDay;
                dtpTo.Value = lastDay;
            }
            else if (selected == "Custom Range")
            {
                if (dtpFrom.Value.Date > dtpTo.Value.Date)
                {
                    dtpTo.Value = dtpFrom.Value.Date;
                }
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            ApplySearchAndCategoryFilter();
        }

        private void ApplySearchAndCategoryFilter()
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            string selectedCategory = cmbCategory.SelectedItem?.ToString() ?? "All";

            var filtered = ReportStore.Reports
                .Where(r =>
                    (string.IsNullOrWhiteSpace(keyword) ||
                     r.ReportID.ToLower().Contains(keyword) ||
                     r.ReportName.ToLower().Contains(keyword) ||
                     r.Category.ToLower().Contains(keyword) ||
                     r.GeneratedBy.ToLower().Contains(keyword) ||
                     r.Status.ToLower().Contains(keyword)) &&
                    (selectedCategory == "All" || r.Category == selectedCategory))
                .OrderByDescending(r => r.GeneratedDate)
                .ToList();

            BindReportsToGrid(filtered);
        }

        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            string category = cmbCategory.SelectedItem?.ToString() ?? "All";
            string rangeType = cmbDateRange.SelectedItem?.ToString() ?? "Daily";
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show("From date cannot be later than To date.");
                return;
            }

            string reportCategory = category == "All" ? "Clients" : category;
            string reportName = reportCategory + " Report";

            ReportStore.Reports.Add(new ReportItem
            {
                ReportID = GenerateNextReportId(),
                ReportName = reportName,
                Category = reportCategory,
                DateRangeType = rangeType,
                FromDate = fromDate,
                ToDate = toDate,
                GeneratedDate = DateTime.Now,
                GeneratedBy = string.IsNullOrWhiteSpace(UserSession.Username) ? "Super Admin" : UserSession.Username,
                Status = "Ready"
            });

            var filtered = ReportStore.Reports
                .Where(r =>
                    (category == "All" || r.Category == category) &&
                    r.FromDate.Date >= fromDate &&
                    r.ToDate.Date <= toDate)
                .OrderByDescending(r => r.GeneratedDate)
                .ToList();

            BindReportsToGrid(filtered);
        }

        private void ArchiveReport(string reportId)
        {
            var report = ReportStore.Reports.FirstOrDefault(r => r.ReportID == reportId);
            if (report == null) return;

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to archive report '{report.ReportName}'?",
                "Archive Report",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            string nextArchiveId = "AR" + (ArchiveStore.ArchivedItems.Count + 1).ToString("D3");

            ArchiveStore.ArchivedItems.Add(new ArchiveItem
            {
                ArchiveID = nextArchiveId,
                Module = "Reports",
                RecordID = report.ReportID,
                ItemName = report.ReportName,
                ExtraInfo = $"{report.Category} | {report.DateRangeType}",
                ArchivedBy = string.IsNullOrWhiteSpace(UserSession.Username) ? "Super Admin" : UserSession.Username,
                ArchivedDate = DateTime.Now,
                Status = "Archived"
            });

            ReportStore.Reports.Remove(report);
            LoadReportsData();
        }

        private ReportItem? GetSelectedReport()
        {
            if (dgvReports.CurrentRow == null) return null;

            string reportId = dgvReports.CurrentRow.Cells["ReportID"].Value?.ToString() ?? "";
            return ReportStore.Reports.FirstOrDefault(r => r.ReportID == reportId);
        }

        private void BtnDownloadReport_Click(object? sender, EventArgs e)
        {
            var selectedReport = GetSelectedReport();
            if (selectedReport == null)
            {
                MessageBox.Show("Please select a report first.");
                return;
            }

            downloadMenu.Show(btnDownloadReport, 0, btnDownloadReport.Height);
        }

        private void CsvMenuItem_Click(object? sender, EventArgs e)
        {
            var report = GetSelectedReport();
            if (report == null)
            {
                MessageBox.Show("Please select a report first.");
                return;
            }

            MessageBox.Show($"CSV export for: {report.ReportName}");
        }

        private void PdfMenuItem_Click(object? sender, EventArgs e)
        {
            var report = GetSelectedReport();
            if (report == null)
            {
                MessageBox.Show("Please select a report first.");
                return;
            }

            MessageBox.Show($"PDF export for: {report.ReportName}");
        }

        private void DgvReports_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvReports.Columns[e.ColumnIndex].Name != "Actions") return;

            Rectangle cellRect = dgvReports.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            int clickX = dgvReports.PointToClient(Cursor.Position).X - cellRect.Left;
            string reportId = dgvReports.Rows[e.RowIndex].Cells["ReportID"].Value?.ToString() ?? "";
            string reportName = dgvReports.Rows[e.RowIndex].Cells["ReportName"].Value?.ToString() ?? "";

            if (clickX < cellRect.Width / 2)
            {
                MessageBox.Show($"View report: {reportName}");
            }
            else
            {
                ArchiveReport(reportId);
            }
        }
    }
}