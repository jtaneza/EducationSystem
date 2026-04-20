using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class FinesViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = Color.White;
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color OnPrimaryContainer = ColorTranslator.FromHtml("#004233");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");
        private readonly Color OnTertiaryContainer = ColorTranslator.FromHtml("#6E1B0F");
        private readonly Color InverseSurface = ColorTranslator.FromHtml("#2B3234");

        private Panel canvas = null!;

        private Panel heroPanel = null!;
        private Panel cardOutstanding = null!;
        private Panel cardPaid = null!;
        private Panel cardUnpaid = null!;

        private Label lblOutstandingTitle = null!;
        private Label lblOutstandingValue = null!;
        private Label lblOutstandingTrend = null!;
        private Label lblOutstandingIcon = null!;

        private Label lblPaidTitle = null!;
        private Label lblPaidValue = null!;
        private Panel paidProgressTrack = null!;
        private Panel paidProgressFill = null!;

        private Label lblUnpaidTitle = null!;
        private Label lblUnpaidValue = null!;
        private Label lblUnpaidNote = null!;

        private Panel tableWrap = null!;
        private Panel tableCard = null!;
        private Panel tableHeader = null!;
        private Label lblTableTitle = null!;
        private Label lblTableSub = null!;
        private Panel filterHost = null!;
        private ComboBox cboSchoolFilter = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;

        private DataGridView dgvFines = null!;
        private Panel footerPanel = null!;
        private Label lblFooter = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNext = null!;

        private Panel insightPanel = null!;
        private Panel insightInstitution = null!;
        private Panel insightHealth = null!;
        private Panel insightAlert = null!;

        private List<FinesRowItem> allRows = new List<FinesRowItem>();

        public FinesViewForm()
        {
            InitializeComponent();
            BuildUI();
            SeedData();
            LoadSchoolFilter();
            LoadFinesData();
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
                Padding = new Padding(0, 0, 0, 120)
            };

            BuildHero();
            BuildTable();
            BuildInsights();

            canvas.Controls.Add(insightPanel);
            canvas.Controls.Add(tableWrap);
            canvas.Controls.Add(heroPanel);

            Controls.Add(canvas);

            Resize += FinesViewForm_Resize;
            AdjustLayout();
        }

        private void BuildHero()
        {
            heroPanel = new Panel
            {
                BackColor = Background,
                Height = 190
            };

            cardOutstanding = new Panel { BackColor = SurfaceLow };
            cardPaid = new Panel { BackColor = Surface };
            cardUnpaid = new Panel { BackColor = InverseSurface };

            lblOutstandingTitle = new Label
            {
                Text = "Total Outstanding Fines",
                Font = new Font("Segoe UI", 12F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            lblOutstandingValue = new Label
            {
                Text = "$12,480.00",
                Font = new Font("Segoe UI", 40F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblOutstandingTrend = new Label
            {
                Text = "↗ +12% from last month",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnTertiaryContainer,
                BackColor = TertiaryContainer,
                AutoSize = true,
                Padding = new Padding(10, 6, 10, 6)
            };

            lblOutstandingIcon = new Label
            {
                Text = "💵",
                Font = new Font("Segoe UI Emoji", 56F),
                ForeColor = Color.FromArgb(18, 0, 107, 85),
                AutoSize = true
            };

            cardOutstanding.Controls.Add(lblOutstandingTitle);
            cardOutstanding.Controls.Add(lblOutstandingValue);
            cardOutstanding.Controls.Add(lblOutstandingTrend);
            cardOutstanding.Controls.Add(lblOutstandingIcon);

            lblPaidTitle = new Label
            {
                Text = "PAID (THIS WEEK)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            lblPaidValue = new Label
            {
                Text = "$4,250",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = Primary,
                AutoSize = true
            };

            paidProgressTrack = new Panel { BackColor = SurfaceHigh, Height = 4 };
            paidProgressFill = new Panel { BackColor = Primary, Height = 4 };

            cardPaid.Controls.Add(lblPaidTitle);
            cardPaid.Controls.Add(lblPaidValue);
            cardPaid.Controls.Add(paidProgressTrack);
            cardPaid.Controls.Add(paidProgressFill);

            lblUnpaidTitle = new Label
            {
                Text = "UNPAID ACCOUNTS",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#BBCAC3"),
                AutoSize = true
            };

            lblUnpaidValue = new Label
            {
                Text = "342",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true
            };

            lblUnpaidNote = new Label
            {
                Text = "Requires immediate oversight",
                Font = new Font("Segoe UI", 10F),
                ForeColor = ColorTranslator.FromHtml("#6DFAD2"),
                AutoSize = true
            };

            cardUnpaid.Controls.Add(lblUnpaidTitle);
            cardUnpaid.Controls.Add(lblUnpaidValue);
            cardUnpaid.Controls.Add(lblUnpaidNote);

            heroPanel.Controls.Add(cardOutstanding);
            heroPanel.Controls.Add(cardPaid);
            heroPanel.Controls.Add(cardUnpaid);
        }

        private void BuildTable()
        {
            tableWrap = new Panel
            {
                BackColor = Background,
                Height = 610
            };

            tableCard = new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle
            };

            tableHeader = new Panel
            {
                Height = 120,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(248, 251, 252)
            };

            lblTableTitle = new Label
            {
                Text = "Global Fines Registry",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblTableSub = new Label
            {
                Text = "Real-time monitoring across all institutions",
                Font = new Font("Segoe UI", 11F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            filterHost = new Panel
            {
                BackColor = SurfaceContainer,
                Size = new Size(220, 38)
            };

            cboSchoolFilter = new ComboBox
            {
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = SurfaceContainer,
                ForeColor = OnSurfaceVariant,
                Font = new Font("Segoe UI", 10F),
                Width = 200
            };
            cboSchoolFilter.SelectedIndexChanged += (s, e) => LoadFinesData();
            filterHost.Controls.Add(cboSchoolFilter);

            searchHost = new Panel
            {
                BackColor = SurfaceContainer,
                Size = new Size(290, 38)
            };

            lblSearchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 16F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = SurfaceContainer,
                ForeColor = OnSurfaceVariant,
                Font = new Font("Segoe UI", 10F),
                Text = "Search by member, fine, or school..."
            };
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by member, fine, or school...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by member, fine, or school...";
                    txtSearch.ForeColor = OnSurfaceVariant;
                }
            };
            txtSearch.TextChanged += (s, e) => LoadFinesData();

            btnFilter = CreateHeaderButton("☰", 44);

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            tableHeader.Controls.Add(lblTableTitle);
            tableHeader.Controls.Add(lblTableSub);
            tableHeader.Controls.Add(filterHost);
            tableHeader.Controls.Add(searchHost);
            tableHeader.Controls.Add(btnFilter);

            dgvFines = new DataGridView
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
                ColumnHeadersHeight = 50,
                GridColor = Color.FromArgb(230, 235, 236),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvFines.RowTemplate.Height = 72;
            dgvFines.DefaultCellStyle.Padding = new Padding(8, 10, 8, 10);
            dgvFines.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 10, 10, 10);

            dgvFines.ColumnHeadersDefaultCellStyle.BackColor = SurfaceContainer;
            dgvFines.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvFines.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvFines.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceContainer;
            dgvFines.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;
            dgvFines.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvFines.DefaultCellStyle.BackColor = Surface;
            dgvFines.DefaultCellStyle.ForeColor = OnSurface;
            dgvFines.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvFines.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvFines.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvFines.Columns.Add("FineId", "FINE ID");
            dgvFines.Columns.Add("SchoolName", "SCHOOL NAME");
            dgvFines.Columns.Add("MemberName", "MEMBER NAME");
            dgvFines.Columns.Add("Reason", "REASON");
            dgvFines.Columns.Add("Amount", "AMOUNT");
            dgvFines.Columns.Add("Status", "STATUS");

            dgvFines.Columns["FineId"].FillWeight = 16;
            dgvFines.Columns["SchoolName"].FillWeight = 20;
            dgvFines.Columns["MemberName"].FillWeight = 22;
            dgvFines.Columns["Reason"].FillWeight = 20;
            dgvFines.Columns["Amount"].FillWeight = 11;
            dgvFines.Columns["Status"].FillWeight = 11;

            dgvFines.Columns["Amount"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvFines.Columns["Amount"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvFines.Columns["Status"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvFines.Columns["Status"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvFines.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvFines.CellPainting += dgvFines_CellPainting;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = Color.FromArgb(250, 252, 252)
            };

            lblFooter = new Label
            {
                Text = "Showing 6 of 1,240 records across the global network",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            btnPrev = CreatePagerButton("‹", false, 34);
            btnPage1 = CreatePagerButton("1", true, 34);
            btnPage2 = CreatePagerButton("2", false, 34);
            btnPage3 = CreatePagerButton("3", false, 34);
            btnNext = CreatePagerButton("›", false, 34);

            footerPanel.Controls.Add(lblFooter);
            footerPanel.Controls.Add(btnPrev);
            footerPanel.Controls.Add(btnPage1);
            footerPanel.Controls.Add(btnPage2);
            footerPanel.Controls.Add(btnPage3);
            footerPanel.Controls.Add(btnNext);

            tableCard.Controls.Add(dgvFines);
            tableCard.Controls.Add(footerPanel);
            tableCard.Controls.Add(tableHeader);
            tableWrap.Controls.Add(tableCard);
        }

        private void BuildInsights()
        {
            insightPanel = new Panel
            {
                BackColor = Background,
                Height = 230
            };

            insightInstitution = CreateInsightCard(
                Color.FromArgb(10, 0, 107, 85),
                "🏫",
                "Highest Volume Inst.",
                "St. Andrews",
                "12% of total system fines");

            insightHealth = CreateInsightCard(
                Surface,
                "📊",
                "System Health",
                "94.2%",
                "Recovery rate within 30 days");

            insightAlert = CreateInsightCard(
                Color.FromArgb(10, 160, 63, 48),
                "❗",
                "Alert Threshold",
                "Critical",
                "12 accounts exceed $500 limit");

            insightPanel.Controls.Add(insightInstitution);
            insightPanel.Controls.Add(insightHealth);
            insightPanel.Controls.Add(insightAlert);
        }

        private Panel CreateInsightCard(Color backColor, string icon, string title, string value, string subtitle)
        {
            Panel card = new Panel
            {
                BackColor = backColor
            };

            Panel iconBox = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(44, 44),
                Location = new Point(28, 18)
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 16F),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            iconBox.Controls.Add(lblIcon);

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(86, 30)
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(28, 78)
            };

            Label lblSub = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(28, 122)
            };

            card.Controls.Add(iconBox);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblSub);

            return card;
        }

        private Button CreateHeaderButton(string text, int width)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Width = width,
                Height = 38,
                FlatStyle = FlatStyle.Flat,
                BackColor = Surface,
                ForeColor = OnSurface,
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderColor = Color.FromArgb(225, 231, 232);
            btn.FlatAppearance.BorderSize = 1;
            return btn;
        }

        private Button CreatePagerButton(string text, bool active, int width)
        {
            Button btn = new Button
            {
                Text = text,
                Width = width,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            if (active)
            {
                btn.BackColor = PrimaryContainer;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = OnSurface;
                btn.FlatAppearance.BorderSize = 0;
            }

            return btn;
        }

        private void SeedData()
        {
            allRows = new List<FinesRowItem>
            {
                new FinesRowItem("#FIN-89210", "St. Andrews Institute", "JS", "Julian Sommers", "#B7EBD7", "#3B6B5C", "Overdue (Textbook: Calc III)", 15.50m, "PAID", "#B7EBD7", "#3B6B5C"),
                new FinesRowItem("#FIN-89211", "Westview Tech", "EM", "Elena Martinez", "#DDE4E6", "#161D1F", "Damaged Media Equipment", 120.00m, "UNPAID", "#F7816D", "#6E1B0F"),
                new FinesRowItem("#FIN-89212", "Global Academy", "RK", "Rahul Kapoor", "#B7EBD7", "#3B6B5C", "Late Return: 12 Days", 24.00m, "PAID", "#B7EBD7", "#3B6B5C"),
                new FinesRowItem("#FIN-89213", "St. Andrews Institute", "LC", "Liam Chen", "#DDE4E6", "#161D1F", "Lost Keycard Replacement", 50.00m, "UNPAID", "#F7816D", "#6E1B0F"),
                new FinesRowItem("#FIN-89214", "Oakwood Prep", "SF", "Sarah Fabel", "#B7EBD7", "#3B6B5C", "Overdue (Fiction Collection)", 8.75m, "PAID", "#B7EBD7", "#3B6B5C"),
                new FinesRowItem("#FIN-89215", "Westview Tech", "AB", "Arjun Bose", "#DDE4E6", "#161D1F", "Unreturned Lab Equipment", 215.00m, "UNPAID", "#F7816D", "#6E1B0F")
            };
        }

        private void LoadSchoolFilter()
        {
            cboSchoolFilter.Items.Clear();
            cboSchoolFilter.Items.Add("All Schools");

            foreach (string school in allRows.Select(x => x.SchoolName).Distinct().OrderBy(x => x))
                cboSchoolFilter.Items.Add(school);

            cboSchoolFilter.SelectedIndex = 0;
        }

        private void LoadFinesData()
        {
            dgvFines.Rows.Clear();

            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string term = txtSearch.Text.Trim();
            bool useSearch = !string.IsNullOrWhiteSpace(term) && term != "Search by member, fine, or school...";

            IEnumerable<FinesRowItem> filtered = allRows;

            if (selectedSchool != "All Schools")
                filtered = filtered.Where(x => x.SchoolName == selectedSchool);

            if (useSearch)
            {
                filtered = filtered.Where(x =>
                    x.SchoolName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.MemberName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Reason.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.FineId.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            List<FinesRowItem> results = filtered.ToList();

            foreach (var row in results)
            {
                dgvFines.Rows.Add(
                    row.FineId,
                    row.SchoolName,
                    $"{row.Initials}|{row.MemberName}|{row.MemberBack}|{row.MemberFore}",
                    InsertReasonLineBreak(row.Reason),
                    "$" + row.Amount.ToString("0.00"),
                    $"{row.Status}|{row.StatusBack}|{row.StatusFore}"
                );
            }

            dgvFines.ClearSelection();
            lblFooter.Text = $"Showing {results.Count} of {results.Count} records across the global network";

            lblOutstandingValue.Text = "$" + results.Sum(x => x.Amount).ToString("N2");
            lblPaidValue.Text = "$" + results.Where(x => x.Status == "PAID").Sum(x => x.Amount).ToString("N0");
            lblUnpaidValue.Text = results.Count(x => x.Status == "UNPAID").ToString("N0");
        }

        private string InsertReasonLineBreak(string text)
        {
            string[] words = text.Split(' ');
            if (words.Length <= 2) return text;
            int mid = words.Length / 2;
            return string.Join(" ", words.Take(mid)) + "\n" + string.Join(" ", words.Skip(mid));
        }

        private void dgvFines_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string column = dgvFines.Columns[e.ColumnIndex].Name;

            if (column == "FineId")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (column == "SchoolName")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (column == "MemberName")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string initials = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : "";
                Color circleBack = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : SurfaceHigh;
                Color circleFore = parts.Length > 3 ? ColorTranslator.FromHtml(parts[3]) : OnSurface;

                Rectangle circle = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 20, 28, 28);
                using (SolidBrush brush = new SolidBrush(circleBack))
                    e.Graphics.FillEllipse(brush, circle);

                TextRenderer.DrawText(
                    e.Graphics,
                    initials,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    circle,
                    circleFore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 10F),
                    new Rectangle(e.CellBounds.X + 48, e.CellBounds.Y + 10, e.CellBounds.Width - 56, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (column == "Reason")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Italic),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16),
                    OnSurfaceVariant,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

                e.Handled = true;
            }
            else if (column == "Amount")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 11F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X, e.CellBounds.Y + 10, e.CellBounds.Width, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (column == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string text = parts[0];
                Color back = parts.Length > 1 ? ColorTranslator.FromHtml(parts[1]) : SurfaceHigh;
                Color fore = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : OnSurfaceVariant;

                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                int badgeWidth = textSize.Width + 22;
                int badgeHeight = 28;

                Rectangle badge = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - badgeWidth) / 2 - 4,
                    e.CellBounds.Y + (e.CellBounds.Height - badgeHeight) / 2,
                    badgeWidth,
                    badgeHeight);

                using (SolidBrush brush = new SolidBrush(back))
                    e.Graphics.FillEllipse(brush, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    badge,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void FinesViewForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 24;
            int width = Math.Max(900, canvas.ClientSize.Width - (margin * 2));

            heroPanel.SetBounds(0, 0, canvas.ClientSize.Width, 190);

            int heroTop = 18;
            int heroHeight = 170;

            int leftWidth = (int)(width * 0.46);
            int middleWidth = (int)(width * 0.22);
            int rightWidth = width - leftWidth - middleWidth - (gap * 2);

            cardOutstanding.Bounds = new Rectangle(margin, heroTop, leftWidth, heroHeight);
            cardPaid.Bounds = new Rectangle(cardOutstanding.Right + gap, heroTop, middleWidth, heroHeight);
            cardUnpaid.Bounds = new Rectangle(cardPaid.Right + gap, heroTop, rightWidth, heroHeight);

            lblOutstandingTitle.Location = new Point(24, 22);
            lblOutstandingValue.Location = new Point(24, 48);
            lblOutstandingTrend.Location = new Point(24, 122);
            lblOutstandingIcon.Location = new Point(cardOutstanding.Width - 98, 82);

            lblPaidTitle.Location = new Point(24, 22);
            lblPaidValue.Location = new Point(24, 52);
            paidProgressTrack.Bounds = new Rectangle(24, cardPaid.Height - 34, cardPaid.Width - 48, 4);
            paidProgressFill.Bounds = new Rectangle(24, cardPaid.Height - 34, (int)((cardPaid.Width - 48) * 0.75), 4);

            lblUnpaidTitle.Location = new Point(24, 22);
            lblUnpaidValue.Location = new Point(24, 52);
            lblUnpaidNote.Location = new Point(24, cardUnpaid.Height - 40);

            tableWrap.SetBounds(0, heroPanel.Bottom, canvas.ClientSize.Width, 610);
            tableCard.SetBounds(margin, 0, width, 610);

            lblTableTitle.Location = new Point(28, 16);
            lblTableSub.Location = new Point(28, 48);

            btnFilter.Location = new Point(tableHeader.Width - btnFilter.Width - 24, 68);

            searchHost.Location = new Point(btnFilter.Left - 300, 68);
            searchHost.Size = new Size(290, 38);

            filterHost.Location = new Point(searchHost.Left - 230, 68);
            filterHost.Size = new Size(220, 38);

            cboSchoolFilter.Location = new Point(10, 6);
            cboSchoolFilter.Width = filterHost.Width - 20;

            lblSearchIcon.Location = new Point(10, 8);
            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = 230;

            lblFooter.Location = new Point(24, 20);
            btnNext.Location = new Point(footerPanel.Width - 42, 11);
            btnPage3.Location = new Point(btnNext.Left - 40, 11);
            btnPage2.Location = new Point(btnPage3.Left - 40, 11);
            btnPage1.Location = new Point(btnPage2.Left - 40, 11);
            btnPrev.Location = new Point(btnPage1.Left - 40, 11);

            insightPanel.SetBounds(0, tableWrap.Bottom + 18, canvas.ClientSize.Width, 230);

            int insightGap = 24;
            int insightWidth = (width - (insightGap * 2)) / 3;
            int insightHeight = 180;
            int insightTop = 18;

            insightInstitution.Bounds = new Rectangle(margin, insightTop, insightWidth, insightHeight);
            insightHealth.Bounds = new Rectangle(insightInstitution.Right + insightGap, insightTop, insightWidth, insightHeight);
            insightAlert.Bounds = new Rectangle(insightHealth.Right + insightGap, insightTop, insightWidth, insightHeight);

            canvas.AutoScrollMinSize = new Size(0, insightPanel.Bottom + 140);
        }
    }

    public class FinesRowItem
    {
        public string FineId { get; set; }
        public string SchoolName { get; set; }
        public string Initials { get; set; }
        public string MemberName { get; set; }
        public string MemberBack { get; set; }
        public string MemberFore { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string StatusBack { get; set; }
        public string StatusFore { get; set; }

        public FinesRowItem(string fineId, string schoolName, string initials, string memberName, string memberBack, string memberFore, string reason, decimal amount, string status, string statusBack, string statusFore)
        {
            FineId = fineId;
            SchoolName = schoolName;
            Initials = initials;
            MemberName = memberName;
            MemberBack = memberBack;
            MemberFore = memberFore;
            Reason = reason;
            Amount = amount;
            Status = status;
            StatusBack = statusBack;
            StatusFore = statusFore;
        }
    }
}