using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ReportsViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = Color.White;
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color OnSecondaryContainer = ColorTranslator.FromHtml("#3B6B5C");
        private readonly Color OnTertiaryContainer = ColorTranslator.FromHtml("#6E1B0F");

        private Panel canvas = null!;

        private Label lblTitle = null!;
        private Label lblSubTitle = null!;

        private Panel rightFilterPanel = null!;
        private Label lblFilterTitle = null!;
        private ComboBox cboInstitution = null!;
        private Button btnExportPdf = null!;

        private Panel statsPanel = null!;
        private Panel cardBooks = null!;
        private Panel cardActiveUsers = null!;
        private Panel cardRevenue = null!;

        private Panel mainPanel = null!;
        private Panel growthCard = null!;
        private Panel tableCard = null!;

        private Label lblGrowthTitle = null!;
        private Label lblGrowthSub = null!;
        private Panel[] growthBars = null!;
        private Label[] monthLabels = null!;
        private Label lblRetentionTitle = null!;
        private Label lblRetentionValue = null!;

        private Panel tableHeader = null!;
        private Label lblTableTitle = null!;
        private DataGridView dgvPerformance = null!;

        private List<InstitutionReportItem> institutions = new List<InstitutionReportItem>();

        public ReportsViewForm()
        {
            InitializeComponent();
            SeedReportData();
            BuildUI();
            LoadInstitutionFilter();
            LoadPerformanceData();
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = false;

            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Background,
                Padding = new Padding(0, 0, 0, 40)
            };

            BuildHeader();
            BuildRightFilter();
            BuildStats();
            BuildMainSection();

            canvas.Controls.Add(mainPanel);
            canvas.Controls.Add(statsPanel);
            canvas.Controls.Add(rightFilterPanel);
            canvas.Controls.Add(lblSubTitle);
            canvas.Controls.Add(lblTitle);

            Controls.Add(canvas);

            Resize += ReportsViewForm_Resize;
            AdjustLayout();
        }

        private void BuildHeader()
        {
            lblTitle = new Label
            {
                Text = "System Oversight Reports",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "Global institutional performance and resource allocation data.",
                Font = new Font("Segoe UI", 11.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };
        }

        private void BuildRightFilter()
        {
            rightFilterPanel = new Panel
            {
                BackColor = Background,
                Height = 78
            };

            lblFilterTitle = new Label
            {
                Text = "FILTER INSTITUTION",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Primary,
                AutoSize = true
            };

            cboInstitution = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                BackColor = Surface,
                ForeColor = OnSurface,
                IntegralHeight = false
            };
            cboInstitution.SelectedIndexChanged += (s, e) => LoadPerformanceData();

            btnExportPdf = new Button
            {
                Text = "Export PDF  ↓",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Surface,
                ForeColor = Primary,
                Cursor = Cursors.Hand
            };
            btnExportPdf.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 230);
            btnExportPdf.FlatAppearance.BorderSize = 1;

            rightFilterPanel.Controls.Add(lblFilterTitle);
            rightFilterPanel.Controls.Add(cboInstitution);
            rightFilterPanel.Controls.Add(btnExportPdf);
        }

        private void BuildStats()
        {
            statsPanel = new Panel
            {
                BackColor = Background,
                Height = 160
            };

            cardBooks = CreateStatCard(
                "📚",
                "Total Books Across All Schools",
                "1,284,092",
                "+12% vs LY",
                SecondaryContainer,
                OnSecondaryContainer,
                false);

            cardActiveUsers = CreateStatCard(
                "📡",
                "Active Users Currently Online",
                "42,910",
                "Live Now",
                Color.FromArgb(230, 248, 244),
                Primary,
                true);

            cardRevenue = CreateStatCard(
                "💳",
                "Total System Revenue/Fines",
                "$248,310",
                "Q3 Snapshot",
                Color.FromArgb(243, 246, 248),
                OnSurfaceVariant,
                false);

            statsPanel.Controls.Add(cardBooks);
            statsPanel.Controls.Add(cardActiveUsers);
            statsPanel.Controls.Add(cardRevenue);
        }

        private Panel CreateStatCard(
            string icon,
            string title,
            string value,
            string tag,
            Color tagBack,
            Color tagFore,
            bool liveDot)
        {
            Panel card = new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.None
            };

            Panel iconBox = new Panel
            {
                BackColor = SurfaceLow,
                Size = new Size(46, 46),
                Location = new Point(22, 22)
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 18F),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            iconBox.Controls.Add(lblIcon);

            Label lblTag = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Padding = new Padding(8, 4, 8, 4),
                BackColor = tagBack,
                ForeColor = tagFore,
                Text = liveDot ? "● " + tag : tag
            };

            Label lblCaption = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            card.Controls.Add(iconBox);
            card.Controls.Add(lblTag);
            card.Controls.Add(lblCaption);
            card.Controls.Add(lblValue);

            card.Tag = new Control[] { iconBox, lblTag, lblCaption, lblValue };
            return card;
        }

        private void BuildMainSection()
        {
            mainPanel = new Panel
            {
                BackColor = Background,
                Height = 450
            };

            BuildGrowthCard();
            BuildTableCard();

            mainPanel.Controls.Add(growthCard);
            mainPanel.Controls.Add(tableCard);
        }

        private void BuildGrowthCard()
        {
            growthCard = new Panel
            {
                BackColor = SurfaceLow
            };

            lblGrowthTitle = new Label
            {
                Text = "Monthly Growth",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblGrowthSub = new Label
            {
                Text = "Registration trends across network",
                Font = new Font("Segoe UI", 10F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            growthBars = new Panel[6];
            for (int i = 0; i < 6; i++)
            {
                growthBars[i] = new Panel
                {
                    BackColor = i == 5 ? Primary : Color.FromArgb(70, 0, 184, 148),
                    Width = 36
                };
                growthCard.Controls.Add(growthBars[i]);
            }

            monthLabels = new Label[6];
            string[] months = { "Mar", "Apr", "May", "Jun", "Jul", "Aug" };

            for (int i = 0; i < 6; i++)
            {
                monthLabels[i] = new Label
                {
                    Text = months[i],
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    ForeColor = i == 5 ? Primary : OnSurfaceVariant,
                    AutoSize = true
                };
                growthCard.Controls.Add(monthLabels[i]);
            }

            lblRetentionTitle = new Label
            {
                Text = "User Retention",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblRetentionValue = new Label
            {
                Text = "94.2%",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Primary,
                AutoSize = true
            };

            growthCard.Controls.Add(lblGrowthTitle);
            growthCard.Controls.Add(lblGrowthSub);
            growthCard.Controls.Add(lblRetentionTitle);
            growthCard.Controls.Add(lblRetentionValue);
        }

        private void BuildTableCard()
        {
            tableCard = new Panel
            {
                BackColor = Surface
            };

            tableHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Surface
            };

            lblTableTitle = new Label
            {
                Text = "Institutional Performance Table",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            tableHeader.Controls.Add(lblTableTitle);

            dgvPerformance = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Surface,
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
                ColumnHeadersHeight = 46,
                GridColor = Color.FromArgb(225, 231, 232),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvPerformance.RowTemplate.Height = 58;
            dgvPerformance.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);

            dgvPerformance.ColumnHeadersDefaultCellStyle.BackColor = SurfaceContainer;
            dgvPerformance.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvPerformance.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPerformance.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceContainer;
            dgvPerformance.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;
            dgvPerformance.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvPerformance.DefaultCellStyle.BackColor = Surface;
            dgvPerformance.DefaultCellStyle.ForeColor = OnSurface;
            dgvPerformance.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvPerformance.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvPerformance.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvPerformance.Columns.Add("SchoolName", "School Name");
            dgvPerformance.Columns.Add("ActiveUsers", "Active Users");
            dgvPerformance.Columns.Add("TotalBooks", "Total Books");
            dgvPerformance.Columns.Add("OverdueRate", "Overdue Rate");

            dgvPerformance.Columns["SchoolName"].FillWeight = 32;
            dgvPerformance.Columns["ActiveUsers"].FillWeight = 18;
            dgvPerformance.Columns["TotalBooks"].FillWeight = 22;
            dgvPerformance.Columns["OverdueRate"].FillWeight = 20;

            dgvPerformance.Columns["ActiveUsers"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvPerformance.Columns["ActiveUsers"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvPerformance.Columns["TotalBooks"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvPerformance.Columns["TotalBooks"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvPerformance.Columns["OverdueRate"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvPerformance.Columns["OverdueRate"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvPerformance.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPerformance.CellPainting += dgvPerformance_CellPainting;

            tableCard.Controls.Add(dgvPerformance);
            tableCard.Controls.Add(tableHeader);
        }

        private void SeedReportData()
        {
            institutions = new List<InstitutionReportItem>
            {
                new InstitutionReportItem("GH", "Greenwood High", 12402, 185000, 2.4m, 94.2m, 4120.50m, new[] { 72, 108, 98, 142, 132, 168 }),
                new InstitutionReportItem("NA", "North Academy", 8192, 92400, 8.1m, 88.7m, 6710.25m, new[] { 60, 86, 92, 110, 98, 120 }),
                new InstitutionReportItem("SL", "St. Luke's Institute", 15300, 210500, 1.9m, 96.8m, 2890.00m, new[] { 80, 102, 118, 136, 148, 160 }),
                new InstitutionReportItem("UA", "Urban Arts Tech", 5016, 45100, 5.4m, 90.1m, 5320.40m, new[] { 52, 64, 74, 88, 84, 100 })
            };
        }

        private void LoadInstitutionFilter()
        {
            cboInstitution.Items.Clear();
            cboInstitution.Items.Add("All Institutions (Global)");

            foreach (InstitutionReportItem item in institutions)
                cboInstitution.Items.Add(item.Name);

            cboInstitution.SelectedIndex = 0;

            cboInstitution.ItemHeight = 24;
        }

        private void LoadPerformanceData()
        {
            string selected = cboInstitution.SelectedItem?.ToString() ?? "All Institutions (Global)";

            if (selected == "All Institutions (Global)")
            {
                LoadGlobalSummary();
                LoadGlobalTable();
                LoadGrowthChart("Monthly Growth", "Registration trends across network", new[] { 72, 108, 98, 142, 132, 168 }, 94.2m);
            }
            else
            {
                InstitutionReportItem? item = institutions.FirstOrDefault(x => x.Name == selected);
                if (item == null) return;

                LoadInstitutionSummary(item);
                LoadInstitutionTable(item);
                LoadGrowthChart($"{item.Name} Growth", $"Monthly activity trend for {item.Name}", item.MonthlyGrowth, item.RetentionRate);
            }
        }

        private void LoadGlobalSummary()
        {
            UpdateStatCard(cardBooks,
                "📚",
                "Total Books Across All Schools",
                institutions.Sum(x => x.TotalBooks).ToString("N0"),
                "+12% vs LY",
                SecondaryContainer,
                OnSecondaryContainer,
                false);

            UpdateStatCard(cardActiveUsers,
                "📡",
                "Active Users Currently Online",
                institutions.Sum(x => x.ActiveUsers).ToString("N0"),
                "Live Now",
                Color.FromArgb(230, 248, 244),
                Primary,
                true);

            UpdateStatCard(cardRevenue,
                "💳",
                "Total System Revenue/Fines",
                "$" + institutions.Sum(x => x.RevenueOrFines).ToString("N0"),
                "Q3 Snapshot",
                Color.FromArgb(243, 246, 248),
                OnSurfaceVariant,
                false);
        }

        private void LoadInstitutionSummary(InstitutionReportItem item)
        {
            UpdateStatCard(cardBooks,
                "📚",
                $"Total Books • {item.Name}",
                item.TotalBooks.ToString("N0"),
                $"{item.Code} Snapshot",
                SecondaryContainer,
                OnSecondaryContainer,
                false);

            UpdateStatCard(cardActiveUsers,
                "📡",
                $"Active Users • {item.Name}",
                item.ActiveUsers.ToString("N0"),
                "Live Now",
                Color.FromArgb(230, 248, 244),
                Primary,
                true);

            UpdateStatCard(cardRevenue,
                "💳",
                $"Revenue/Fines • {item.Name}",
                "$" + item.RevenueOrFines.ToString("N0"),
                "Institution View",
                Color.FromArgb(243, 246, 248),
                OnSurfaceVariant,
                false);
        }

        private void UpdateStatCard(
            Panel card,
            string icon,
            string title,
            string value,
            string tag,
            Color tagBack,
            Color tagFore,
            bool liveDot)
        {
            if (card.Controls.Count < 4) return;

            Panel iconBox = (Panel)card.Controls[0];
            Label lblTag = (Label)card.Controls[1];
            Label lblCaption = (Label)card.Controls[2];
            Label lblValue = (Label)card.Controls[3];

            if (iconBox.Controls.Count > 0 && iconBox.Controls[0] is Label lblIcon)
                lblIcon.Text = icon;

            lblTag.Text = liveDot ? "● " + tag : tag;
            lblTag.BackColor = tagBack;
            lblTag.ForeColor = tagFore;

            lblCaption.Text = title;
            lblValue.Text = value;

            LayoutStatCard(card);
        }

        private void LoadGlobalTable()
        {
            dgvPerformance.Rows.Clear();

            foreach (InstitutionReportItem item in institutions)
            {
                dgvPerformance.Rows.Add(
                    $"{item.Code}|{item.Name}",
                    item.ActiveUsers.ToString("N0"),
                    item.TotalBooks.ToString("N0"),
                    $"{item.OverdueRate:0.0}|{(item.OverdueRate >= 5 ? "#F7816D" : "#B7EBD7")}|{(item.OverdueRate >= 5 ? "#6E1B0F" : "#3B6B5C")}");
            }

            dgvPerformance.ClearSelection();
            lblTableTitle.Text = "Institutional Performance Table";
        }

        private void LoadInstitutionTable(InstitutionReportItem item)
        {
            dgvPerformance.Rows.Clear();

            dgvPerformance.Rows.Add(
                $"{item.Code}|{item.Name}",
                item.ActiveUsers.ToString("N0"),
                item.TotalBooks.ToString("N0"),
                $"{item.OverdueRate:0.0}|{(item.OverdueRate >= 5 ? "#F7816D" : "#B7EBD7")}|{(item.OverdueRate >= 5 ? "#6E1B0F" : "#3B6B5C")}");

            dgvPerformance.ClearSelection();
            lblTableTitle.Text = $"{item.Name} Summary Table";
        }

        private void LoadGrowthChart(string title, string subtitle, int[] heights, decimal retention)
        {
            lblGrowthTitle.Text = title;
            lblGrowthSub.Text = subtitle;
            lblRetentionValue.Text = $"{retention:0.0}%";

            for (int i = 0; i < growthBars.Length; i++)
            {
                growthBars[i].Height = heights[i];
            }

            AdjustLayout();
        }

        private void dgvPerformance_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvPerformance.Columns[e.ColumnIndex].Name;

            if (col == "SchoolName")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string code = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : raw;

                Rectangle badge = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 13, 28, 28);
                using (SolidBrush b = new SolidBrush(ColorTranslator.FromHtml("#0F172A")))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    code,
                    new Font("Segoe UI", 8F, FontStyle.Bold),
                    badge,
                    ColorTranslator.FromHtml("#6DFAD2"),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 48, e.CellBounds.Y + 8, e.CellBounds.Width - 56, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "OverdueRate")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string text = parts[0] + "%";
                Color back = parts.Length > 1 ? ColorTranslator.FromHtml(parts[1]) : SurfaceHigh;
                Color fore = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : OnSurfaceVariant;

                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                int pillWidth = textSize.Width + 20;
                int pillHeight = 24;

                int pillX = e.CellBounds.X + (e.CellBounds.Width - pillWidth) / 2 - 6;
                int pillY = e.CellBounds.Y + (e.CellBounds.Height - pillHeight) / 2;

                Rectangle pill = new Rectangle(pillX, pillY, pillWidth, pillHeight);

                using (SolidBrush b = new SolidBrush(back))
                    e.Graphics.FillEllipse(b, pill);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    pill,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void ReportsViewForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 24;
            int usableWidth = Math.Max(980, canvas.ClientSize.Width - (margin * 2));

            lblTitle.Location = new Point(margin, 40);
            lblSubTitle.Location = new Point(margin, 88);

            rightFilterPanel.Bounds = new Rectangle(canvas.ClientSize.Width - 560 - margin, 126, 560, 64);

            lblFilterTitle.Location = new Point(0, 2);

            cboInstitution.Bounds = new Rectangle(0, 22, 380, 38);
            btnExportPdf.Bounds = new Rectangle(398, 22, 132, 38);

            statsPanel.SetBounds(0, 220, canvas.ClientSize.Width, 190);

            int statWidth = (usableWidth - (gap * 2)) / 3;
            int statTop = 0;
            int statHeight = 150;

            cardBooks.Bounds = new Rectangle(margin, statTop, statWidth, statHeight);
            cardActiveUsers.Bounds = new Rectangle(cardBooks.Right + gap, statTop, statWidth, statHeight);
            cardRevenue.Bounds = new Rectangle(cardActiveUsers.Right + gap, statTop, statWidth, statHeight);

            LayoutStatCard(cardBooks);
            LayoutStatCard(cardActiveUsers);
            LayoutStatCard(cardRevenue);

            mainPanel.SetBounds(0, statsPanel.Bottom + 10, canvas.ClientSize.Width, 450);

            int leftWidth = (int)(usableWidth * 0.39);
            int rightWidth = usableWidth - leftWidth - gap;

            growthCard.Bounds = new Rectangle(margin, 0, leftWidth, 400);
            tableCard.Bounds = new Rectangle(growthCard.Right + gap, 0, rightWidth, 400);

            lblGrowthTitle.Location = new Point(28, 22);
            lblGrowthSub.Location = new Point(28, 52);

            int barBaseY = 286;
            int chartLeft = 26;
            int chartRight = growthCard.Width - 26;
            int chartWidth = chartRight - chartLeft;
            int barWidth = 36;
            int slots = growthBars.Length;
            int spacing = (chartWidth - (slots * barWidth)) / (slots + 1);

            for (int i = 0; i < growthBars.Length; i++)
            {
                int x = chartLeft + spacing + (i * (barWidth + spacing));
                growthBars[i].Width = barWidth;
                growthBars[i].Location = new Point(x, barBaseY - growthBars[i].Height);

                monthLabels[i].Location = new Point(
                    x + (barWidth - monthLabels[i].PreferredWidth) / 2,
                    barBaseY + 12);
            }

            lblRetentionTitle.Location = new Point(24, growthCard.Height - 44);
            lblRetentionValue.Location = new Point(growthCard.Width - lblRetentionValue.Width - 24, growthCard.Height - 44);

            lblTableTitle.Location = new Point(24, 22);

            canvas.AutoScrollMinSize = new Size(0, mainPanel.Bottom + 30);
        }

        private void LayoutStatCard(Panel card)
        {
            if (card.Tag is not Control[] controls || controls.Length < 4) return;

            Panel iconBox = (Panel)controls[0];
            Label lblTag = (Label)controls[1];
            Label lblCaption = (Label)controls[2];
            Label lblValue = (Label)controls[3];

            iconBox.Location = new Point(22, 18);
            lblTag.Location = new Point(card.Width - lblTag.Width - 18, 20);
            lblCaption.Location = new Point(22, 76);
            lblValue.Location = new Point(22, 104);
        }
    }

    public class InstitutionReportItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalBooks { get; set; }
        public decimal OverdueRate { get; set; }
        public decimal RetentionRate { get; set; }
        public decimal RevenueOrFines { get; set; }
        public int[] MonthlyGrowth { get; set; }

        public InstitutionReportItem(
            string code,
            string name,
            int activeUsers,
            int totalBooks,
            decimal overdueRate,
            decimal retentionRate,
            decimal revenueOrFines,
            int[] monthlyGrowth)
        {
            Code = code;
            Name = name;
            ActiveUsers = activeUsers;
            TotalBooks = totalBooks;
            OverdueRate = overdueRate;
            RetentionRate = retentionRate;
            RevenueOrFines = revenueOrFines;
            MonthlyGrowth = monthlyGrowth;
        }
    }
}