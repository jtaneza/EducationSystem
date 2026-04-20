using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class BorrowingViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color Outline = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color OnPrimary = Color.White;
        private readonly Color SuccessText = ColorTranslator.FromHtml("#3B6B5C");
        private readonly Color WarningText = ColorTranslator.FromHtml("#6E1B0F");

        private Panel heroPanel = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Panel filterHost = null!;
        private ComboBox cboSchoolFilter = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;

        private Panel statsPanel = null!;
        private Panel cardTotal = null!;
        private Panel cardOverview = null!;
        private Panel healthPanel = null!;
        private Panel dueBlock = null!;
        private Panel overdueBlock = null!;
        private Panel borrowersBlock = null!;

        private Panel tableWrap = null!;
        private Panel tableShell = null!;
        private DataGridView dgvBorrowing = null!;
        private Panel footerPanel = null!;
        private Label lblFooter = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNext = null!;

        private List<BorrowingRowItem> allRows = new List<BorrowingRowItem>();

        public BorrowingViewForm()
        {
            InitializeComponent();
            BuildUI();
            SeedData();
            LoadSchoolFilter();
            LoadBorrowingData();
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = false;

            BuildHero();
            BuildStats();
            BuildTable();

            Controls.Add(tableWrap);
            Controls.Add(statsPanel);
            Controls.Add(heroPanel);

            Resize += BorrowingViewForm_Resize;
            AdjustLayout();
        }

        private void BuildHero()
        {
            heroPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Background
            };

            lblTitle = new Label
            {
                Text = "Global Borrowing Records",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "Comprehensive oversight of all active book circulations across the institutional network.",
                Font = new Font("Segoe UI", 12F),
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
            cboSchoolFilter.SelectedIndexChanged += (s, e) => LoadBorrowingData();
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
            txtSearch.TextChanged += (s, e) => LoadBorrowingData();

            btnFilter = new Button
            {
                Text = "☰",
                Font = new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                Size = new Size(38, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Surface,
                ForeColor = Primary,
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 230);
            btnFilter.FlatAppearance.BorderSize = 1;

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            heroPanel.Controls.Add(lblTitle);
            heroPanel.Controls.Add(lblSubTitle);
            heroPanel.Controls.Add(filterHost);
            heroPanel.Controls.Add(searchHost);
            heroPanel.Controls.Add(btnFilter);
        }

        private void BuildStats()
        {
            statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 156,
                BackColor = Background
            };

            cardTotal = new Panel
            {
                BackColor = SurfaceLow
            };

            Label lblTotalCaption = new Label
            {
                Text = "TOTAL ACTIVE LOANS",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(24, 24)
            };

            Label lblTotalValue = new Label
            {
                Name = "lblTotalValue",
                Text = "12,482",
                Font = new Font("Segoe UI", 30F, FontStyle.Bold),
                ForeColor = Primary,
                AutoSize = true,
                Location = new Point(24, 48)
            };

            Label lblBookIcon = new Label
            {
                Text = "📖",
                Font = new Font("Segoe UI Emoji", 56F),
                ForeColor = Color.FromArgb(24, 40, 48, 52),
                AutoSize = true,
                Location = new Point(220, 20)
            };

            cardTotal.Controls.Add(lblTotalCaption);
            cardTotal.Controls.Add(lblTotalValue);
            cardTotal.Controls.Add(lblBookIcon);

            cardOverview = new Panel
            {
                BackColor = Surface
            };

            dueBlock = CreateOverviewBlock("Upcoming Due", "842", OnSurface, Point.Empty);
            overdueBlock = CreateOverviewBlock("Overdue Notices", "156", WarningText, Point.Empty);
            borrowersBlock = CreateOverviewBlock("Unique Borrowers", "4,120", OnSurface, Point.Empty);

            cardOverview.Controls.Add(dueBlock);
            cardOverview.Controls.Add(overdueBlock);
            cardOverview.Controls.Add(borrowersBlock);

            healthPanel = new Panel
            {
                BackColor = Color.FromArgb(235, 246, 240),
                Size = new Size(138, 52)
            };

            Label lblHealth = new Label
            {
                Text = "SYSTEM HEALTH:\r\nOPTIMAL",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = SuccessText,
                AutoSize = true,
                Location = new Point(14, 9)
            };

            healthPanel.Controls.Add(lblHealth);
            cardOverview.Controls.Add(healthPanel);

            statsPanel.Controls.Add(cardTotal);
            statsPanel.Controls.Add(cardOverview);
        }

        private Panel CreateOverviewBlock(string caption, string value, Color valueColor, Point location)
        {
            Panel panel = new Panel
            {
                Location = location,
                Size = new Size(120, 68),
                BackColor = Color.Transparent
            };

            Label lblCaption = new Label
            {
                Text = caption,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            Label lblValue = new Label
            {
                Name = "lblValue",
                Text = value,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = valueColor,
                AutoSize = true,
                Location = new Point(0, 18)
            };

            panel.Controls.Add(lblCaption);
            panel.Controls.Add(lblValue);
            return panel;
        }

        private void BuildTable()
        {
            tableWrap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background,
                Padding = new Padding(34, 0, 34, 24)
            };

            tableShell = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgvBorrowing = new DataGridView
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
                ColumnHeadersHeight = 58,
                GridColor = Outline,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvBorrowing.RowTemplate.Height = 78;
            dgvBorrowing.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            dgvBorrowing.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 12, 10, 12);

            dgvBorrowing.ColumnHeadersDefaultCellStyle.BackColor = SurfaceContainer;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvBorrowing.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceContainer;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;

            dgvBorrowing.DefaultCellStyle.BackColor = Surface;
            dgvBorrowing.DefaultCellStyle.ForeColor = OnSurface;
            dgvBorrowing.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvBorrowing.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvBorrowing.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvBorrowing.Columns.Add("TransactionId", "TRANSACTION\r\nID");
            dgvBorrowing.Columns.Add("SchoolName", "SCHOOL NAME");
            dgvBorrowing.Columns.Add("MemberName", "MEMBER\r\nNAME");
            dgvBorrowing.Columns.Add("BookTitle", "BOOK TITLE");
            dgvBorrowing.Columns.Add("BorrowDate", "BORROW\r\nDATE");
            dgvBorrowing.Columns.Add("DueDate", "DUE\r\nDATE");
            dgvBorrowing.Columns.Add("Status", "STATUS");

            dgvBorrowing.Columns["TransactionId"].FillWeight = 14;
            dgvBorrowing.Columns["SchoolName"].FillWeight = 18;
            dgvBorrowing.Columns["MemberName"].FillWeight = 14;
            dgvBorrowing.Columns["BookTitle"].FillWeight = 22;
            dgvBorrowing.Columns["BorrowDate"].FillWeight = 12;
            dgvBorrowing.Columns["DueDate"].FillWeight = 12;
            dgvBorrowing.Columns["Status"].FillWeight = 12;

            foreach (DataGridViewColumn col in dgvBorrowing.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvBorrowing.CellPainting += dgvBorrowing_CellPainting;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = SurfaceLow
            };

            lblFooter = new Label
            {
                Text = "Showing 7 of 12,482 entries",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            btnPrev = CreatePagerButton("‹", false);
            btnPage1 = CreatePagerButton("1", true);
            btnPage2 = CreatePagerButton("2", false);
            btnPage3 = CreatePagerButton("3", false);
            btnNext = CreatePagerButton("›", false);

            footerPanel.Controls.Add(lblFooter);
            footerPanel.Controls.Add(btnPrev);
            footerPanel.Controls.Add(btnPage1);
            footerPanel.Controls.Add(btnPage2);
            footerPanel.Controls.Add(btnPage3);
            footerPanel.Controls.Add(btnNext);

            tableShell.Controls.Add(dgvBorrowing);
            tableShell.Controls.Add(footerPanel);
            tableWrap.Controls.Add(tableShell);
        }

        private Button CreatePagerButton(string text, bool active)
        {
            Button btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                Width = 34,
                Height = 34,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            if (active)
            {
                btn.BackColor = Primary;
                btn.ForeColor = OnPrimary;
                btn.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btn.BackColor = Surface;
                btn.ForeColor = OnSurface;
                btn.FlatAppearance.BorderColor = Color.FromArgb(225, 231, 232);
                btn.FlatAppearance.BorderSize = 1;
            }

            return btn;
        }

        private void SeedData()
        {
            allRows = new List<BorrowingRowItem>
            {
                new BorrowingRowItem("TX-94021", "Emerald Peak Academy", "Julian Vance", "Principles of Quantum Mechanics", "Oct 12, 2023", "Oct 26, 2023", false, "In Progress", "#B7EBD7", "#3B6B5C"),
                new BorrowingRowItem("TX-94025", "North Star Institute", "Elena Rodriguez", "Artificial Intelligence: A Modern Approach", "Oct 14, 2023", "Oct 28, 2023", false, "In Progress", "#B7EBD7", "#3B6B5C"),
                new BorrowingRowItem("TX-94029", "Silverwood Prep", "Marcus Thorne", "Advanced Economic Theory", "Oct 10, 2023", "Oct 24, 2023", true, "Overdue", "#F7816D", "#6E1B0F"),
                new BorrowingRowItem("TX-94032", "Emerald Peak Academy", "Sarah Jenkins", "The Art of Computer Programming", "Oct 15, 2023", "Oct 29, 2023", false, "In Progress", "#B7EBD7", "#3B6B5C"),
                new BorrowingRowItem("TX-94038", "Oak Ridge College", "David Chen", "Organic Chemistry Vol. 2", "Oct 08, 2023", "Oct 22, 2023", false, "Returned Today", "#DDE4E6", "#3C4A44"),
                new BorrowingRowItem("TX-94041", "North Star Institute", "Leila Smith", "Biotechnology Fundamentals", "Oct 16, 2023", "Oct 30, 2023", false, "In Progress", "#B7EBD7", "#3B6B5C"),
                new BorrowingRowItem("TX-94045", "Silverwood Prep", "Kevin Wu", "History of Modern Architecture", "Oct 11, 2023", "Oct 25, 2023", true, "Overdue", "#F7816D", "#6E1B0F")
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

        private void LoadBorrowingData()
        {
            dgvBorrowing.Rows.Clear();

            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string term = txtSearch.Text.Trim();
            bool useSearch = !string.IsNullOrWhiteSpace(term) && term != "Search by member, book, or school...";

            IEnumerable<BorrowingRowItem> filtered = allRows;

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

            List<BorrowingRowItem> results = filtered.ToList();

            foreach (var row in results)
            {
                dgvBorrowing.Rows.Add(
                    row.TransactionId,
                    ReplaceFirst(row.SchoolName, " ", "\n"),
                    ReplaceFirst(row.MemberName, " ", "\n"),
                    InsertBookLineBreak(row.BookTitle),
                    InsertDateLineBreak(row.BorrowDate),
                    row.IsDueOverdue ? InsertDateLineBreak(row.DueDate) + "|overdue" : InsertDateLineBreak(row.DueDate),
                    $"{row.Status}|{row.StatusBack}|{row.StatusFore}"
                );
            }

            dgvBorrowing.ClearSelection();
            lblFooter.Text = $"Showing {results.Count} of {results.Count} entries";

            UpdateSummary(results);
        }

        private void UpdateSummary(List<BorrowingRowItem> rows)
        {
            Label? lblTotalValue = cardTotal.Controls.Find("lblTotalValue", true).FirstOrDefault() as Label;
            if (lblTotalValue != null) lblTotalValue.Text = rows.Count.ToString("N0");

            Label? dueValue = dueBlock.Controls.Find("lblValue", true).FirstOrDefault() as Label;
            if (dueValue != null) dueValue.Text = rows.Count(x => x.Status == "In Progress").ToString("N0");

            Label? overdueValue = overdueBlock.Controls.Find("lblValue", true).FirstOrDefault() as Label;
            if (overdueValue != null) overdueValue.Text = rows.Count(x => x.Status == "Overdue").ToString("N0");

            Label? borrowersValue = borrowersBlock.Controls.Find("lblValue", true).FirstOrDefault() as Label;
            if (borrowersValue != null) borrowersValue.Text = rows.Select(x => x.MemberName).Distinct().Count().ToString("N0");
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

        private void dgvBorrowing_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string column = dgvBorrowing.Columns[e.ColumnIndex].Name;

            if (column == "BookTitle")
            {
                e.PaintBackground(e.CellBounds, true);
                string text = e.FormattedValue?.ToString() ?? "";

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Italic),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 16, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

                e.Handled = true;
            }
            else if (column == "DueDate")
            {
                e.PaintBackground(e.CellBounds, true);
                string raw = e.FormattedValue?.ToString() ?? "";
                bool overdue = raw.Contains("|overdue");
                string text = raw.Replace("|overdue", "");

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 16, e.CellBounds.Height - 16),
                    overdue ? WarningText : OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

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

                Size textSize = TextRenderer.MeasureText(text.ToUpper(), new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + (e.CellBounds.Height - 28) / 2,
                    textSize.Width + 20,
                    28
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
        }

        private void BorrowingViewForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 24;
            int width = ClientSize.Width - (margin * 2);

            lblTitle.Location = new Point(margin, 12);
            lblSubTitle.Location = new Point(margin, 60);

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

            int cardHeight = 118;
            int leftWidth = (int)(width * 0.34);
            int rightWidth = width - leftWidth - gap;

            cardTotal.Bounds = new Rectangle(margin, 10, leftWidth, cardHeight);
            cardOverview.Bounds = new Rectangle(cardTotal.Right + gap, 10, rightWidth, cardHeight);

            int innerPadding = 28;
            int blockTop = 28;
            int blockWidth = 120;
            int healthGap = 28;

            healthPanel.Location = new Point(cardOverview.Width - healthPanel.Width - innerPadding, 33);

            int usableLeftArea = healthPanel.Left - healthGap - innerPadding;
            int totalBlocksWidth = blockWidth * 3;
            int equalGap = (usableLeftArea - totalBlocksWidth) / 4;

            if (equalGap < 12)
                equalGap = 12;

            int x1 = innerPadding + equalGap;
            int x2 = x1 + blockWidth + equalGap;
            int x3 = x2 + blockWidth + equalGap;

            dueBlock.Location = new Point(x1, blockTop);
            overdueBlock.Location = new Point(x2, blockTop);
            borrowersBlock.Location = new Point(x3, blockTop);

            lblFooter.Location = new Point(20, 18);
            btnNext.Location = new Point(footerPanel.Width - 42, 11);
            btnPage3.Location = new Point(btnNext.Left - 40, 11);
            btnPage2.Location = new Point(btnPage3.Left - 40, 11);
            btnPage1.Location = new Point(btnPage2.Left - 40, 11);
            btnPrev.Location = new Point(btnPage1.Left - 40, 11);
        }
    }

    public class BorrowingRowItem
    {
        public string TransactionId { get; set; }
        public string SchoolName { get; set; }
        public string MemberName { get; set; }
        public string BookTitle { get; set; }
        public string BorrowDate { get; set; }
        public string DueDate { get; set; }
        public bool IsDueOverdue { get; set; }
        public string Status { get; set; }
        public string StatusBack { get; set; }
        public string StatusFore { get; set; }

        public BorrowingRowItem(string transactionId, string schoolName, string memberName, string bookTitle, string borrowDate, string dueDate, bool isDueOverdue, string status, string statusBack, string statusFore)
        {
            TransactionId = transactionId;
            SchoolName = schoolName;
            MemberName = memberName;
            BookTitle = bookTitle;
            BorrowDate = borrowDate;
            DueDate = dueDate;
            IsDueOverdue = isDueOverdue;
            Status = status;
            StatusBack = statusBack;
            StatusFore = statusFore;
        }
    }
}