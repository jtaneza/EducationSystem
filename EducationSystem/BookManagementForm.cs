using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class BookManagementForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color SoftBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentBlue = ColorTranslator.FromHtml("#2563EB");
        private readonly Color AccentBlueSoft = ColorTranslator.FromHtml("#EFF6FF");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");
        private readonly Color AccentDangerSoft = ColorTranslator.FromHtml("#F7816D");
        private readonly Color AccentDangerSoftBg = ColorTranslator.FromHtml("#FDE8E4");
        private readonly Color AccentSuccessSoft = ColorTranslator.FromHtml("#B7EBD7");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Button btnAddBook = null!;

        private Panel cardAllBooks = null!;
        private Panel cardBorrowed = null!;
        private Panel cardLowStock = null!;

        private Panel tableCard = null!;
        private Panel searchPanel = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;
        private Button btnMore = null!;
        private DataGridView dgvBooks = null!;
        private Label lblFooter = null!;
        private Panel pagerPanel = null!;

        public BookManagementForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Text = "BookManagementForm";

            BuildInterface();
            AdjustLayout();

            Load += (s, e) =>
            {
                AdjustLayout();
                ClearGridSelection();
            };

            Resize += (s, e) => AdjustLayout();
        }

        private void BuildInterface()
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
                Text = "Book Management",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubTitle = new Label
            {
                Text = "Catalog oversight and inventory control",
                AutoSize = true,
                Font = new Font("Segoe UI", 14F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            btnAddBook = new Button
            {
                Text = "+  Add New Book",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddBook.FlatAppearance.BorderSize = 0;

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubTitle);
            canvas.Controls.Add(btnAddBook);

            BuildMetricCards();
            BuildTableCard();
        }

        private void BuildMetricCards()
        {
            cardAllBooks = CreateRoundedCard();
            cardBorrowed = CreateRoundedCard();
            cardLowStock = CreateRoundedCard();

            canvas.Controls.Add(cardAllBooks);
            canvas.Controls.Add(cardBorrowed);
            canvas.Controls.Add(cardLowStock);

            BuildAllBooksCard();
            BuildBorrowedCard();
            BuildLowStockCard();
        }

        private void BuildAllBooksCard()
        {
            Panel iconWrap = CreateIconWrap(ColorTranslator.FromHtml("#ECFDF3"));
            Label icon = CreateIconLabel("▣", AccentDeep);
            iconWrap.Controls.Add(icon);

            Label badge = CreateBadge("+2.4%", ColorTranslator.FromHtml("#ECFDF3"), AccentDeep);
            Label label = CreateMetricLabel("All Books");
            Label value = CreateMetricValue("24,512");

            cardAllBooks.Controls.Add(iconWrap);
            cardAllBooks.Controls.Add(badge);
            cardAllBooks.Controls.Add(label);
            cardAllBooks.Controls.Add(value);

            cardAllBooks.Tag = new Control[] { iconWrap, badge, label, value };
        }

        private void BuildBorrowedCard()
        {
            Panel iconWrap = CreateIconWrap(AccentBlueSoft);
            Label icon = CreateIconLabel("📖", AccentBlue);
            iconWrap.Controls.Add(icon);

            Label badge = CreateBadge("Live", AccentBlueSoft, AccentBlue);
            Label label = CreateMetricLabel("Books with Students");
            Label value = CreateMetricValue("18,290");

            cardBorrowed.Controls.Add(iconWrap);
            cardBorrowed.Controls.Add(badge);
            cardBorrowed.Controls.Add(label);
            cardBorrowed.Controls.Add(value);

            cardBorrowed.Tag = new Control[] { iconWrap, badge, label, value };
        }

        private void BuildLowStockCard()
        {
            Panel iconWrap = CreateIconWrap(AccentDangerSoftBg);
            Label icon = CreateIconLabel("⚠", AccentDanger);
            iconWrap.Controls.Add(icon);

            Label badge = CreateBadge("Alert", AccentDangerSoftBg, AccentDangerSoft);
            Label label = CreateMetricLabel("Needs More Copies");

            Label value = new Label
            {
                Text = "42",
                AutoSize = true,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            Label suffix = new Label
            {
                Text = "books",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            cardLowStock.Controls.Add(iconWrap);
            cardLowStock.Controls.Add(badge);
            cardLowStock.Controls.Add(label);
            cardLowStock.Controls.Add(value);
            cardLowStock.Controls.Add(suffix);

            cardLowStock.Tag = new Control[] { iconWrap, badge, label, value, suffix };
        }

        private void BuildTableCard()
        {
            tableCard = CreateRoundedCard();
            canvas.Controls.Add(tableCard);

            searchPanel = new Panel
            {
                BackColor = SoftBack
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
                BorderStyle = BorderStyle.None,
                BackColor = SoftBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 11F),
                Text = "Search by title, author or ISBN..."
            };
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by title, author or ISBN...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = PrimaryText;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by title, author or ISBN...";
                    txtSearch.ForeColor = SecondaryText;
                }
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(txtSearch);

            btnFilter = CreateHeaderActionButton("☰");
            btnMore = CreateHeaderActionButton("⋮");

            dgvBooks = new DataGridView
            {
                BackgroundColor = CardBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 54,
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvBooks.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvBooks.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvBooks.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvBooks.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvBooks.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvBooks.DefaultCellStyle.BackColor = CardBack;
            dgvBooks.DefaultCellStyle.ForeColor = PrimaryText;
            dgvBooks.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgvBooks.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvBooks.DefaultCellStyle.SelectionForeColor = PrimaryText;

            dgvBooks.RowsDefaultCellStyle.BackColor = CardBack;
            dgvBooks.RowsDefaultCellStyle.SelectionBackColor = CardBack;
            dgvBooks.RowsDefaultCellStyle.SelectionForeColor = PrimaryText;

            dgvBooks.RowTemplate.Height = 106;

            dgvBooks.Columns.Add("BookId", "BOOK\r\nID");
            dgvBooks.Columns.Add("TitleAuthor", "TITLE & AUTHOR");
            dgvBooks.Columns.Add("Category", "CATEGORY");
            dgvBooks.Columns.Add("Year", "YEAR");
            dgvBooks.Columns.Add("Qty", "QTY");
            dgvBooks.Columns.Add("Status", "STATUS");
            dgvBooks.Columns.Add("Actions", "ACTIONS");

            dgvBooks.Columns["Year"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Qty"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Actions"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvBooks.Columns["Year"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Actions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            foreach (DataGridViewColumn col in dgvBooks.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvBooks.Rows.Add("BK-\r\n2001", "The Great Gatsby\nF. Scott Fitzgerald", "Fiction", "1925", "12", "In Stock", "");
            dgvBooks.Rows.Add("BK-\r\n2002", "1984\nGeorge Orwell", "Classic\nDystopian", "1949", "08", "In Stock", "");
            dgvBooks.Rows.Add("BK-\r\n2003", "A Brief History\nof Time\nStephen Hawking", "Science", "1988", "02", "Low\nStock", "");

            dgvBooks.CellPainting += DgvBooks_CellPainting;
            dgvBooks.DataBindingComplete += (s, e) => ClearGridSelection();
            dgvBooks.SelectionChanged += (s, e) => ClearGridSelection();

            lblFooter = new Label
            {
                Text = "Showing 1 to 10 of 24,512 books",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            pagerPanel = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(190, 36)
            };
            BuildPager();

            tableCard.Controls.Add(searchPanel);
            tableCard.Controls.Add(btnFilter);
            tableCard.Controls.Add(btnMore);
            tableCard.Controls.Add(dgvBooks);
            tableCard.Controls.Add(lblFooter);
            tableCard.Controls.Add(pagerPanel);
        }

        private void ClearGridSelection()
        {
            if (dgvBooks == null) return;
            dgvBooks.ClearSelection();
            dgvBooks.CurrentCell = null;
        }

        private void BuildPager()
        {
            pagerPanel.Controls.Add(CreatePagerButton("‹", false, new Point(0, 0)));
            pagerPanel.Controls.Add(CreatePagerButton("1", true, new Point(40, 0)));
            pagerPanel.Controls.Add(CreatePagerButton("2", false, new Point(80, 0)));
            pagerPanel.Controls.Add(CreatePagerButton("3", false, new Point(120, 0)));
            pagerPanel.Controls.Add(CreatePagerButton("›", false, new Point(160, 0)));
        }

        private Button CreatePagerButton(string text, bool active, Point location)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(32, 32),
                Location = location,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = active ? AccentDeep : Color.Transparent,
                ForeColor = active ? Color.White : SecondaryText,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = active ? 0 : 1;
            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D9E0E2");
            return btn;
        }

        private Button CreateHeaderActionButton(string text)
        {
            Button btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
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

        private Panel CreateIconWrap(Color backColor)
        {
            Panel p = new Panel
            {
                BackColor = backColor,
                Size = new Size(42, 42)
            };
            p.Paint += RoundedPanelPaint;
            return p;
        }

        private Label CreateIconLabel(string text, Color color)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 16F, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private Label CreateBadge(string text, Color bg, Color fg)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Size = new Size(58, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = bg,
                ForeColor = fg
            };
        }

        private Label CreateMetricLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };
        }

        private Label CreateMetricValue(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };
        }

        private void AdjustLayout()
        {
            int margin = 36;
            int gap = 24;
            int usableWidth = Math.Max(1080, canvas.ClientSize.Width - (margin * 2));

            lblTitle.Location = new Point(margin, 34);
            lblSubTitle.Location = new Point(margin, 88);

            btnAddBook.Bounds = new Rectangle(margin + usableWidth - 186, 56, 186, 52);

            int cardsTop = 150;
            int cardWidth = (usableWidth - (gap * 2)) / 3;
            int cardHeight = 178;

            cardAllBooks.Bounds = new Rectangle(margin, cardsTop, cardWidth, cardHeight);
            cardBorrowed.Bounds = new Rectangle(cardAllBooks.Right + gap, cardsTop, cardWidth, cardHeight);
            cardLowStock.Bounds = new Rectangle(cardBorrowed.Right + gap, cardsTop, cardWidth, cardHeight);

            LayoutMetricCards();

            int tableTop = cardAllBooks.Bottom + 34;
            tableCard.Bounds = new Rectangle(margin, tableTop, usableWidth, 640);

            searchPanel.Bounds = new Rectangle(34, 28, 482, 42);
            searchPanel.Controls[0].Location = new Point(14, 8);
            txtSearch.Location = new Point(46, 12);
            txtSearch.Width = searchPanel.Width - 58;

            btnMore.Bounds = new Rectangle(tableCard.Width - 58, 28, 32, 32);
            btnFilter.Bounds = new Rectangle(tableCard.Width - 104, 28, 32, 32);

            dgvBooks.Location = new Point(0, 102);
            dgvBooks.Size = new Size(tableCard.Width, 430);

            ApplyColumnWidths();

            lblFooter.Location = new Point(34, tableCard.Height - 46);
            pagerPanel.Location = new Point(tableCard.Width - pagerPanel.Width - 34, tableCard.Height - 52);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 40);
        }

        private void ApplyColumnWidths()
        {
            if (dgvBooks == null || dgvBooks.Columns.Count == 0)
                return;

            int total = dgvBooks.Width - 2;

            int bookId = (int)(total * 0.13);
            int titleAuthor = (int)(total * 0.31);
            int category = (int)(total * 0.19);
            int year = (int)(total * 0.10);
            int qty = (int)(total * 0.08);
            int status = (int)(total * 0.11);

            int actions = total - (bookId + titleAuthor + category + year + qty + status);

            dgvBooks.Columns["BookId"].Width = bookId;
            dgvBooks.Columns["TitleAuthor"].Width = titleAuthor;
            dgvBooks.Columns["Category"].Width = category;
            dgvBooks.Columns["Year"].Width = year;
            dgvBooks.Columns["Qty"].Width = qty;
            dgvBooks.Columns["Status"].Width = status;
            dgvBooks.Columns["Actions"].Width = actions;
        }

        private void LayoutMetricCards()
        {
            {
                var c = (Control[])cardAllBooks.Tag;
                Panel iconWrap = (Panel)c[0];
                Label badge = (Label)c[1];
                Label label = (Label)c[2];
                Label value = (Label)c[3];

                iconWrap.Location = new Point(26, 26);
                badge.Location = new Point(cardAllBooks.Width - 84, 28);
                label.Location = new Point(26, 88);
                value.Location = new Point(26, 112);
            }

            {
                var c = (Control[])cardBorrowed.Tag;
                Panel iconWrap = (Panel)c[0];
                Label badge = (Label)c[1];
                Label label = (Label)c[2];
                Label value = (Label)c[3];

                iconWrap.Location = new Point(26, 26);
                badge.Location = new Point(cardBorrowed.Width - 70, 28);
                label.Location = new Point(26, 88);
                value.Location = new Point(26, 112);
            }

            {
                var c = (Control[])cardLowStock.Tag;
                Panel iconWrap = (Panel)c[0];
                Label badge = (Label)c[1];
                Label label = (Label)c[2];
                Label value = (Label)c[3];
                Label suffix = (Label)c[4];

                iconWrap.Location = new Point(26, 26);
                badge.Location = new Point(cardLowStock.Width - 74, 28);
                label.Location = new Point(26, 88);
                value.Location = new Point(26, 108);
                suffix.Location = new Point(value.Right + 6, 126);
            }
        }

        private void DgvBooks_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0)
                return;

            string col = dgv.Columns[e.ColumnIndex].Name;
            string text = e.FormattedValue?.ToString() ?? "";

            if (col == "TitleAuthor")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle book = new Rectangle(e.CellBounds.X + 14, e.CellBounds.Y + 20, 42, 58);
                using (SolidBrush b = new SolidBrush(Color.FromArgb(245, 247, 248)))
                    e.Graphics.FillRectangle(b, book);
                using (Pen p = new Pen(Color.FromArgb(220, 225, 228)))
                    e.Graphics.DrawRectangle(p, book);

                string cover = e.RowIndex switch
                {
                    0 => "GATSBY",
                    1 => "1984",
                    _ => "TIME"
                };

                TextRenderer.DrawText(
                    e.Graphics,
                    cover,
                    new Font("Segoe UI", 8F, FontStyle.Bold),
                    book,
                    Color.Black,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                string[] parts = text.Split('\n');
                string title = parts.Length > 0 ? parts[0] : text;
                string author = parts.Length > 1 ? string.Join("\n", parts, 1, parts.Length - 1) : "";

                TextRenderer.DrawText(
                    e.Graphics,
                    title,
                    new Font("Segoe UI", 11F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 74, e.CellBounds.Y + 16, e.CellBounds.Width - 80, 34),
                    PrimaryText,
                    TextFormatFlags.Left | TextFormatFlags.WordBreak);

                TextRenderer.DrawText(
                    e.Graphics,
                    author,
                    new Font("Segoe UI", 10F),
                    new Rectangle(e.CellBounds.X + 74, e.CellBounds.Y + 48, e.CellBounds.Width - 80, e.CellBounds.Height - 54),
                    SecondaryText,
                    TextFormatFlags.Left | TextFormatFlags.WordBreak);

                e.Handled = true;
            }
            else if (col == "Category")
            {
                e.PaintBackground(e.CellBounds, true);

                string[] tags = text.Split('\n');
                int top = e.CellBounds.Y + 32;

                foreach (var tag in tags)
                {
                    Size sz = TextRenderer.MeasureText(tag, new Font("Segoe UI", 10F));
                    Rectangle badge = new Rectangle(e.CellBounds.X + 14, top, sz.Width + 24, 28);

                    using (SolidBrush b = new SolidBrush(Color.FromArgb(80, AccentSuccessSoft)))
                        e.Graphics.FillEllipse(b, badge);

                    TextRenderer.DrawText(
                        e.Graphics,
                        tag,
                        new Font("Segoe UI", 10F),
                        badge,
                        ColorTranslator.FromHtml("#3B6B5C"),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                    top += 32;
                }

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                bool low = text.Contains("Low");
                Color dot = low ? AccentDanger : AccentDeep;
                Color fg = low ? AccentDanger : AccentDeep;

                Rectangle dotRect = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 42, 8, 8);
                using (SolidBrush b = new SolidBrush(dot))
                    e.Graphics.FillEllipse(b, dotRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 26, e.CellBounds.Y + 26, e.CellBounds.Width - 28, e.CellBounds.Height - 20),
                    fg,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle editRect = new Rectangle(e.CellBounds.Right - 78, e.CellBounds.Y + 36, 24, 24);
                Rectangle archiveRect = new Rectangle(e.CellBounds.Right - 34, e.CellBounds.Y + 36, 24, 24);

                TextRenderer.DrawText(
                    e.Graphics,
                    "✎",
                    new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                    editRect,
                    SecondaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    "⌦",
                    new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                    archiveRect,
                    SecondaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
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