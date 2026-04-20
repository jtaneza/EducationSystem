using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ReturnsViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color Surface = Color.White;
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color OnSecondaryContainer = ColorTranslator.FromHtml("#3B6B5C");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");
        private readonly Color OnTertiaryContainer = ColorTranslator.FromHtml("#6E1B0F");

        private Panel heroPanel = null!;
        private Label lblBigNumber = null!;
        private Label lblHeroSub = null!;
        private Panel summaryCard = null!;
        private Label lblOnTimeTitle = null!;
        private Label lblOnTimeValue = null!;
        private Label lblSchoolsTitle = null!;
        private Label lblSchoolsValue = null!;
        private Panel filterHost = null!;
        private ComboBox cboSchoolFilter = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;

        private Panel tableWrap = null!;
        private Panel tableCard = null!;
        private Panel tableHeader = null!;
        private Label lblTableTitle = null!;
        private DataGridView dgvReturns = null!;
        private Panel footerPanel = null!;
        private Label lblFooter = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNext = null!;

        private List<ReturnsRowItem> allRows = new List<ReturnsRowItem>();

        public ReturnsViewForm()
        {
            InitializeComponent();
            BuildUI();
            SeedData();
            LoadSchoolFilter();
            LoadReturnsData();
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = false;

            BuildHero();
            BuildTable();

            Controls.Add(tableWrap);
            Controls.Add(heroPanel);

            Resize += ReturnsViewForm_Resize;
            AdjustLayout();
        }

        private void BuildHero()
        {
            heroPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 210,
                BackColor = Background
            };

            lblBigNumber = new Label
            {
                Text = "3,842",
                Font = new Font("Segoe UI", 38F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblHeroSub = new Label
            {
                Text = "Total Books Returned This Semester",
                Font = new Font("Segoe UI", 12.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            summaryCard = new Panel
            {
                BackColor = SurfaceLow,
                Size = new Size(360, 106)
            };

            lblOnTimeTitle = new Label
            {
                Text = "ON TIME RATE",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            lblOnTimeValue = new Label
            {
                Text = "94.2%",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Primary,
                AutoSize = true
            };

            lblSchoolsTitle = new Label
            {
                Text = "TOTAL SCHOOLS",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            lblSchoolsValue = new Label
            {
                Text = "124",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = OnSurface,
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
            cboSchoolFilter.SelectedIndexChanged += (s, e) => LoadReturnsData();
            filterHost.Controls.Add(cboSchoolFilter);

            searchHost = new Panel
            {
                BackColor = SurfaceContainer,
                Size = new Size(280, 38)
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
                Text = "Search by member, book, or school..."
            };
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by member, book, or school...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by member, book, or school...";
                    txtSearch.ForeColor = OnSurfaceVariant;
                }
            };
            txtSearch.TextChanged += (s, e) => LoadReturnsData();

            btnFilter = new Button
            {
                Text = "☰",
                Font = new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                Size = new Size(38, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = SurfaceHigh,
                ForeColor = OnSurfaceVariant,
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderSize = 0;

            summaryCard.Controls.Add(lblOnTimeTitle);
            summaryCard.Controls.Add(lblOnTimeValue);
            summaryCard.Controls.Add(lblSchoolsTitle);
            summaryCard.Controls.Add(lblSchoolsValue);

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            heroPanel.Controls.Add(lblBigNumber);
            heroPanel.Controls.Add(lblHeroSub);
            heroPanel.Controls.Add(summaryCard);
            heroPanel.Controls.Add(filterHost);
            heroPanel.Controls.Add(searchHost);
            heroPanel.Controls.Add(btnFilter);
        }

        private void BuildTable()
        {
            tableWrap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background,
                Padding = new Padding(34, 0, 34, 24)
            };

            tableCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceLow,
                Padding = new Padding(1)
            };

            Panel innerCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Surface
            };

            tableHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                BackColor = Surface
            };

            lblTableTitle = new Label
            {
                Text = "System-Wide Return Logs",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            tableHeader.Controls.Add(lblTableTitle);

            dgvReturns = new DataGridView
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
                ColumnHeadersHeight = 64,
                GridColor = Color.FromArgb(220, 228, 230),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvReturns.RowTemplate.Height = 86;
            dgvReturns.DefaultCellStyle.Padding = new Padding(8, 10, 8, 10);
            dgvReturns.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 12, 10, 12);

            dgvReturns.ColumnHeadersDefaultCellStyle.BackColor = SurfaceContainer;
            dgvReturns.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvReturns.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvReturns.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceContainer;
            dgvReturns.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;

            dgvReturns.DefaultCellStyle.BackColor = Surface;
            dgvReturns.DefaultCellStyle.ForeColor = OnSurface;
            dgvReturns.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvReturns.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvReturns.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvReturns.Columns.Add("TransactionId", "TRANSACTION\r\nID");
            dgvReturns.Columns.Add("SchoolName", "SCHOOL\r\nNAME");
            dgvReturns.Columns.Add("MemberName", "MEMBER\r\nNAME");
            dgvReturns.Columns.Add("BookTitle", "BOOK TITLE");
            dgvReturns.Columns.Add("ReturnDate", "RETURN\r\nDATE");
            dgvReturns.Columns.Add("Status", "STATUS");

            dgvReturns.Columns["TransactionId"].FillWeight = 16;
            dgvReturns.Columns["SchoolName"].FillWeight = 18;
            dgvReturns.Columns["MemberName"].FillWeight = 16;
            dgvReturns.Columns["BookTitle"].FillWeight = 20;
            dgvReturns.Columns["ReturnDate"].FillWeight = 14;
            dgvReturns.Columns["Status"].FillWeight = 16;

            foreach (DataGridViewColumn col in dgvReturns.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvReturns.CellPainting += dgvReturns_CellPainting;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = SurfaceContainer
            };

            lblFooter = new Label
            {
                Text = "Showing 1-6 of 3,842 returns",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            btnPrev = CreateFooterButton("Previous", false, 74);
            btnPage1 = CreateFooterButton("1", true, 34);
            btnPage2 = CreateFooterButton("2", false, 34);
            btnPage3 = CreateFooterButton("3", false, 34);
            btnNext = CreateFooterButton("Next", false, 54);

            footerPanel.Controls.Add(lblFooter);
            footerPanel.Controls.Add(btnPrev);
            footerPanel.Controls.Add(btnPage1);
            footerPanel.Controls.Add(btnPage2);
            footerPanel.Controls.Add(btnPage3);
            footerPanel.Controls.Add(btnNext);

            innerCard.Controls.Add(dgvReturns);
            innerCard.Controls.Add(footerPanel);
            innerCard.Controls.Add(tableHeader);

            tableCard.Controls.Add(innerCard);
            tableWrap.Controls.Add(tableCard);
        }

        private Button CreateFooterButton(string text, bool active, int width)
        {
            Button btn = new Button
            {
                Text = text,
                Width = width,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, active ? FontStyle.Bold : FontStyle.Regular),
                Cursor = Cursors.Hand
            };

            if (active)
            {
                btn.BackColor = Primary;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = OnSurfaceVariant;
                btn.FlatAppearance.BorderSize = 0;
            }

            return btn;
        }

        private void SeedData()
        {
            allRows = new List<ReturnsRowItem>
            {
                new ReturnsRowItem("#TX-89231", "Northwood Academy", "Alexander Pierce", "The Great Gatsby", "Oct 24, 2023", "ON TIME", "#B7EBD7", "#3B6B5C"),
                new ReturnsRowItem("#TX-89245", "Summit High School", "Elena Rodriguez", "Calculus Vol. 2", "Oct 24, 2023", "LATE", "#F7816D", "#6E1B0F"),
                new ReturnsRowItem("#TX-89252", "Brookside Prep", "Marcus Thorne", "Art Through the Ages", "Oct 23, 2023", "ON TIME", "#B7EBD7", "#3B6B5C"),
                new ReturnsRowItem("#TX-89260", "Oakwood International", "Sarah Jenkins", "Genomics 101", "Oct 23, 2023", "ON TIME", "#B7EBD7", "#3B6B5C"),
                new ReturnsRowItem("#TX-89271", "Lakeside Magnet", "Julian Vance", "Advanced Microeconomics", "Oct 22, 2023", "LATE", "#F7816D", "#6E1B0F"),
                new ReturnsRowItem("#TX-89288", "Northwood Academy", "Chloe Zheng", "Pride and Prejudice", "Oct 22, 2023", "ON TIME", "#B7EBD7", "#3B6B5C")
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

        private void LoadReturnsData()
        {
            dgvReturns.Rows.Clear();

            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string term = txtSearch.Text.Trim();
            bool useSearch = !string.IsNullOrWhiteSpace(term) && term != "Search by member, book, or school...";

            IEnumerable<ReturnsRowItem> filtered = allRows;

            if (selectedSchool != "All Schools")
                filtered = filtered.Where(x => x.SchoolName == selectedSchool);

            if (useSearch)
            {
                filtered = filtered.Where(x =>
                    x.SchoolName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.MemberName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.BookTitle.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.TransactionId.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            List<ReturnsRowItem> results = filtered.ToList();

            foreach (var row in results)
            {
                dgvReturns.Rows.Add(
                    row.TransactionId,
                    ReplaceFirst(row.SchoolName, " ", "\n"),
                    ReplaceFirst(row.MemberName, " ", "\n"),
                    InsertBookLineBreak(row.BookTitle),
                    InsertDateLineBreak(row.ReturnDate),
                    $"{row.Status}|{row.StatusBack}|{row.StatusFore}"
                );
            }

            dgvReturns.ClearSelection();
            lblFooter.Text = $"Showing 1-{results.Count} of {results.Count} returns";

            lblBigNumber.Text = results.Count.ToString("N0");
            lblSchoolsValue.Text = results.Select(x => x.SchoolName).Distinct().Count().ToString("N0");

            int onTimeCount = results.Count == 0 ? 0 : results.Count(x => x.Status == "ON TIME");
            decimal onTimeRate = results.Count == 0 ? 0 : (decimal)onTimeCount / results.Count * 100;
            lblOnTimeValue.Text = $"{onTimeRate:0.0}%";
        }

        private string InsertBookLineBreak(string text)
        {
            string[] words = text.Split(' ');
            if (words.Length <= 2) return text;
            int mid = words.Length / 2;
            return string.Join(" ", words.Take(mid)) + "\n" + string.Join(" ", words.Skip(mid));
        }

        private string InsertDateLineBreak(string text)
        {
            return text.Replace(", ", ",\n");
        }

        private static string ReplaceFirst(string input, string oldValue, string newValue)
        {
            int index = input.IndexOf(oldValue, StringComparison.Ordinal);
            if (index < 0) return input;

            return input.Substring(0, index) + newValue + input.Substring(index + oldValue.Length);
        }

        private void dgvReturns_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string column = dgvReturns.Columns[e.ColumnIndex].Name;

            if (column == "BookTitle")
            {
                e.PaintBackground(e.CellBounds, true);
                string text = e.FormattedValue?.ToString() ?? "";

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Italic),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

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

                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 9F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 12,
                    e.CellBounds.Y + (e.CellBounds.Height - 42) / 2,
                    textSize.Width + 28,
                    42);

                using (SolidBrush brush = new SolidBrush(back))
                    e.Graphics.FillEllipse(brush, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 9F, FontStyle.Bold),
                    badge,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void ReturnsViewForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int rightMargin = 34;

            lblBigNumber.Location = new Point(margin, 14);
            lblHeroSub.Location = new Point(margin, 98);

            btnFilter.Location = new Point(ClientSize.Width - btnFilter.Width - 34, 36);

            searchHost.Location = new Point(btnFilter.Left - 290, 36);
            searchHost.Size = new Size(280, 38);

            filterHost.Location = new Point(searchHost.Left - 230, 36);
            filterHost.Size = new Size(220, 38);

            cboSchoolFilter.Location = new Point(10, 6);
            cboSchoolFilter.Width = filterHost.Width - 20;

            lblSearchIcon.Location = new Point(10, 8);
            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = 220;

            summaryCard.Location = new Point(ClientSize.Width - summaryCard.Width - rightMargin, 86);

            lblOnTimeTitle.Location = new Point(28, 22);
            lblOnTimeValue.Location = new Point(28, 48);
            lblSchoolsTitle.Location = new Point(192, 22);
            lblSchoolsValue.Location = new Point(192, 48);

            lblTableTitle.Location = new Point(36, 28);

            lblFooter.Location = new Point(36, 18);

            btnNext.Location = new Point(footerPanel.Width - 64, 11);
            btnPage3.Location = new Point(btnNext.Left - 40, 11);
            btnPage2.Location = new Point(btnPage3.Left - 40, 11);
            btnPage1.Location = new Point(btnPage2.Left - 40, 11);
            btnPrev.Location = new Point(btnPage1.Left - 82, 11);
        }
    }

    public class ReturnsRowItem
    {
        public string TransactionId { get; set; }
        public string SchoolName { get; set; }
        public string MemberName { get; set; }
        public string BookTitle { get; set; }
        public string ReturnDate { get; set; }
        public string Status { get; set; }
        public string StatusBack { get; set; }
        public string StatusFore { get; set; }

        public ReturnsRowItem(string transactionId, string schoolName, string memberName, string bookTitle, string returnDate, string status, string statusBack, string statusFore)
        {
            TransactionId = transactionId;
            SchoolName = schoolName;
            MemberName = memberName;
            BookTitle = bookTitle;
            ReturnDate = returnDate;
            Status = status;
            StatusBack = statusBack;
            StatusFore = statusFore;
        }
    }
}