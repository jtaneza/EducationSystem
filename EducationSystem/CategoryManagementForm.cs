using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class CategoryManagementForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color CardSoft = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color FieldBack = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color DarkCard = ColorTranslator.FromHtml("#2B3234");
        private readonly Color AccentMint = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color MutedText = ColorTranslator.FromHtml("#BBCAC3");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");

        private Panel canvas = null!;

        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Button btnAddCategory = null!;

        private Panel cardTotalGenres = null!;
        private Panel cardPopularGenre = null!;
        private Panel cardInsights = null!;
        private Panel directoryCard = null!;

        private Label lblDirectoryTitle = null!;
        private Panel filterBox = null!;
        private TextBox txtFilter = null!;
        private Label lblFilterIcon = null!;

        private DataGridView dgvCategories = null!;
        private Label lblFooterInfo = null!;
        private Panel pagerPanel = null!;

        public CategoryManagementForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Text = "CategoryManagementForm";

            BuildInterface();
            AdjustLayout();

            Load += CategoryManagementForm_Load;
            Resize += (s, e) => AdjustLayout();
        }

        private void CategoryManagementForm_Load(object? sender, EventArgs e)
        {
            AdjustLayout();
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
                Text = "Category Management",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText
            };

            lblSubTitle = new Label
            {
                Text = "Organize and curate the library's intellectual mapping.",
                AutoSize = true,
                Font = new Font("Segoe UI", 14F),
                ForeColor = SecondaryText
            };

            btnAddCategory = new Button
            {
                Text = "+  Add New Category",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddCategory.FlatAppearance.BorderSize = 0;

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubTitle);
            canvas.Controls.Add(btnAddCategory);

            BuildStatCards();
            BuildDirectoryCard();
        }

        private void BuildStatCards()
        {
            cardTotalGenres = CreateLightCard();
            cardPopularGenre = CreateLightCard();
            cardInsights = new Panel { BackColor = DarkCard };
            cardInsights.Paint += RoundedPanelPaint;

            canvas.Controls.Add(cardTotalGenres);
            canvas.Controls.Add(cardPopularGenre);
            canvas.Controls.Add(cardInsights);

            BuildTotalGenresCard();
            BuildPopularGenreCard();
            BuildInsightsCard();
        }

        private void BuildTotalGenresCard()
        {
            var iconWrap = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#B7EBD7"),
                Size = new Size(54, 54),
                Location = new Point(26, 26)
            };
            iconWrap.Paint += RoundedPanelPaint;

            var icon = new Label
            {
                Text = "⚏",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 18F, FontStyle.Bold),
                ForeColor = AccentDeep
            };

            var badge = new Label
            {
                Text = "+2 New",
                AutoSize = false,
                Size = new Size(74, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 109, 250, 210),
                ForeColor = AccentDeep
            };

            var value = new Label
            {
                Text = "24",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText
            };

            var caption = new Label
            {
                Text = "TOTAL GENRES MANAGED",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = SecondaryText
            };

            iconWrap.Controls.Add(icon);
            cardTotalGenres.Controls.Add(iconWrap);
            cardTotalGenres.Controls.Add(badge);
            cardTotalGenres.Controls.Add(value);
            cardTotalGenres.Controls.Add(caption);

            cardTotalGenres.Tag = new Control[] { iconWrap, badge, value, caption };
        }

        private void BuildPopularGenreCard()
        {
            var iconWrap = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#B7EBD7"),
                Size = new Size(54, 54),
                Location = new Point(26, 26)
            };
            iconWrap.Paint += RoundedPanelPaint;

            var icon = new Label
            {
                Text = "↗",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 18F, FontStyle.Bold),
                ForeColor = AccentDeep
            };

            var value = new Label
            {
                Text = "Science Fiction",
                AutoSize = true,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = PrimaryText
            };

            var caption = new Label
            {
                Text = "MOST POPULAR GENRE",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = SecondaryText
            };

            iconWrap.Controls.Add(icon);
            cardPopularGenre.Controls.Add(iconWrap);
            cardPopularGenre.Controls.Add(value);
            cardPopularGenre.Controls.Add(caption);

            cardPopularGenre.Tag = new Control[] { iconWrap, value, caption };
        }

        private void BuildInsightsCard()
        {
            var title = new Label
            {
                Text = "System Insights",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = AccentMint
            };

            var body = new Label
            {
                Text = "Academic categories have seen a 12% increase in cross-referencing this term.",
                AutoSize = false,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(220, 228, 230)
            };

            var visual = new Panel
            {
                BackColor = Color.FromArgb(40, 109, 250, 210)
            };
            visual.Paint += RoundedPanelPaint;
            visual.Paint += (s, e) =>
            {
                using SolidBrush b = new SolidBrush(Color.FromArgb(70, 22, 29, 31));
                e.Graphics.FillRectangle(b, 20, 48, 12, 26);
                e.Graphics.FillRectangle(b, 40, 58, 12, 16);
                e.Graphics.FillRectangle(b, 60, 34, 12, 40);
            };

            cardInsights.Controls.Add(title);
            cardInsights.Controls.Add(body);
            cardInsights.Controls.Add(visual);

            cardInsights.Tag = new Control[] { title, body, visual };
        }

        private void BuildDirectoryCard()
        {
            directoryCard = CreateLightCard();
            canvas.Controls.Add(directoryCard);

            lblDirectoryTitle = new Label
            {
                Text = "☰  Category Directory",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = PrimaryText
            };

            filterBox = new Panel
            {
                BackColor = FieldBack
            };
            filterBox.Paint += RoundedPanelPaint;

            lblFilterIcon = new Label
            {
                Text = "⌕",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 13F),
                ForeColor = SecondaryText
            };

            txtFilter = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = FieldBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 11F),
                Text = "Filter Search"
            };

            txtFilter.GotFocus += (s, e) =>
            {
                if (txtFilter.Text == "Filter Search")
                {
                    txtFilter.Text = "";
                    txtFilter.ForeColor = PrimaryText;
                }
            };

            txtFilter.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtFilter.Text))
                {
                    txtFilter.Text = "Filter Search";
                    txtFilter.ForeColor = SecondaryText;
                }
            };

            filterBox.Controls.Add(lblFilterIcon);
            filterBox.Controls.Add(txtFilter);

            dgvCategories = new DataGridView
            {
                BackgroundColor = CardBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 52,
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.None
            };

            dgvCategories.ColumnHeadersDefaultCellStyle.BackColor = CardSoft;
            dgvCategories.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvCategories.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvCategories.DefaultCellStyle.BackColor = CardBack;
            dgvCategories.DefaultCellStyle.ForeColor = PrimaryText;
            dgvCategories.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgvCategories.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvCategories.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvCategories.RowTemplate.Height = 76;

            dgvCategories.Columns.Add("CategoryId", "CATEGORY ID");
            dgvCategories.Columns.Add("GenreName", "GENRE NAME");
            dgvCategories.Columns.Add("GroupName", "GROUP");
            dgvCategories.Columns.Add("BookCount", "BOOK COUNT");
            dgvCategories.Columns.Add("Actions", "ACTIONS");

            dgvCategories.Columns["CategoryId"].FillWeight = 18;
            dgvCategories.Columns["GenreName"].FillWeight = 30;
            dgvCategories.Columns["GroupName"].FillWeight = 24;
            dgvCategories.Columns["BookCount"].FillWeight = 18;
            dgvCategories.Columns["Actions"].FillWeight = 10;
            dgvCategories.Columns["Actions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvCategories.Columns["Actions"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvCategories.Rows.Add("CAT-001", "Science Fiction", "Fiction & Fantasy", "1240", "✎   ⌦");
            dgvCategories.Rows.Add("CAT-042", "Quantum Physics", "Pure Sciences", "458", "✎   ⌦");
            dgvCategories.Rows.Add("CAT-109", "Medieval History", "History & Geography", "892", "✎   ⌦");
            dgvCategories.Rows.Add("CAT-215", "Modern Architecture", "Fine Arts", "214", "✎   ⌦");

            dgvCategories.CellPainting += DgvCategories_CellPainting;

            lblFooterInfo = new Label
            {
                Text = "SHOWING 4 OF 24 GENRES",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = SecondaryText
            };

            pagerPanel = new Panel
            {
                BackColor = Color.Transparent
            };

            BuildPager();

            directoryCard.Controls.Add(lblDirectoryTitle);
            directoryCard.Controls.Add(filterBox);
            directoryCard.Controls.Add(dgvCategories);
            directoryCard.Controls.Add(lblFooterInfo);
            directoryCard.Controls.Add(pagerPanel);
        }

        private void BuildPager()
        {
            Button prev = CreatePagerButton("‹", false);
            Button one = CreatePagerButton("1", true);
            Button two = CreatePagerButton("2", false);
            Button three = CreatePagerButton("3", false);
            Button next = CreatePagerButton("›", false);

            pagerPanel.Controls.Add(prev);
            pagerPanel.Controls.Add(one);
            pagerPanel.Controls.Add(two);
            pagerPanel.Controls.Add(three);
            pagerPanel.Controls.Add(next);

            prev.Location = new Point(0, 0);
            one.Location = new Point(40, 0);
            two.Location = new Point(80, 0);
            three.Location = new Point(120, 0);
            next.Location = new Point(160, 0);

            pagerPanel.Size = new Size(200, 36);
        }

        private Button CreatePagerButton(string text, bool active)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(32, 32),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = active ? AccentDeep : Color.Transparent,
                ForeColor = active ? Color.White : SecondaryText,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Panel CreateLightCard()
        {
            Panel p = new Panel { BackColor = CardBack };
            p.Paint += RoundedPanelPaint;
            return p;
        }

        private void AdjustLayout()
        {
            int margin = 40;
            int gap = 24;
            int usableWidth = Math.Max(1080, canvas.ClientSize.Width - (margin * 2));

            lblTitle.Location = new Point(margin, 34);
            lblSubTitle.Location = new Point(margin, 88);

            btnAddCategory.Bounds = new Rectangle(
                margin + usableWidth - 250,
                50,
                250,
                50
            );

            int cardsTop = 150;
            int smallCardWidth = (int)(usableWidth * 0.31);
            int insightsWidth = usableWidth - (smallCardWidth * 2) - (gap * 2);

            cardTotalGenres.Bounds = new Rectangle(margin, cardsTop, smallCardWidth, 174);
            cardPopularGenre.Bounds = new Rectangle(cardTotalGenres.Right + gap, cardsTop, smallCardWidth, 174);
            cardInsights.Bounds = new Rectangle(cardPopularGenre.Right + gap, cardsTop, insightsWidth, 174);

            LayoutStatCards();

            int directoryTop = cardTotalGenres.Bottom + 34;
            directoryCard.Bounds = new Rectangle(margin, directoryTop, usableWidth, 604);

            lblDirectoryTitle.Location = new Point(28, 28);
            filterBox.Bounds = new Rectangle(directoryCard.Width - 250 - 28, 22, 226, 40);
            lblFilterIcon.Location = new Point(12, 9);
            txtFilter.Location = new Point(38, 11);
            txtFilter.Width = filterBox.Width - 48;

            dgvCategories.Location = new Point(0, 94);
            dgvCategories.Size = new Size(directoryCard.Width, 408);

            lblFooterInfo.Location = new Point(28, directoryCard.Height - 48);
            pagerPanel.Location = new Point(directoryCard.Width - pagerPanel.Width - 28, directoryCard.Height - 56);

            canvas.AutoScrollMinSize = new Size(0, directoryCard.Bottom + 40);
        }

        private void LayoutStatCards()
        {
            {
                var c = (Control[])cardTotalGenres.Tag;
                Panel iconWrap = (Panel)c[0];
                Label badge = (Label)c[1];
                Label value = (Label)c[2];
                Label caption = (Label)c[3];

                iconWrap.Location = new Point(26, 26);
                badge.Location = new Point(cardTotalGenres.Width - 100, 32);
                value.Location = new Point(26, 88);
                caption.Location = new Point(26, 132);
            }

            {
                var c = (Control[])cardPopularGenre.Tag;
                Panel iconWrap = (Panel)c[0];
                Label value = (Label)c[1];
                Label caption = (Label)c[2];

                iconWrap.Location = new Point(26, 26);
                value.Location = new Point(26, 92);
                caption.Location = new Point(26, 132);
            }

            {
                var c = (Control[])cardInsights.Tag;
                Label title = (Label)c[0];
                Label body = (Label)c[1];
                Panel visual = (Panel)c[2];

                title.Location = new Point(26, 28);
                body.Location = new Point(26, 66);
                body.Size = new Size(cardInsights.Width - 150, 72);

                visual.Bounds = new Rectangle(cardInsights.Width - 96, 72, 86, 96);
            }
        }

        private void DgvCategories_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0)
                return;

            string col = dgv.Columns[e.ColumnIndex].Name;
            string text = e.FormattedValue?.ToString() ?? "";

            if (col == "CategoryId")
            {
                e.PaintBackground(e.CellBounds, true);

                Size txt = TextRenderer.MeasureText(text, new Font("Segoe UI", 10F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 18,
                    e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                    txt.Width + 20,
                    24);

                using (SolidBrush b = new SolidBrush(Color.FromArgb(16, 0, 107, 85)))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    badge,
                    AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "GenreName")
            {
                e.PaintBackground(e.CellBounds, true);

                string icon = "●";
                if (text == "Science Fiction") icon = "◉";
                if (text == "Quantum Physics") icon = "⚛";
                if (text == "Medieval History") icon = "✎";
                if (text == "Modern Architecture") icon = "✿";

                Rectangle circle = new Rectangle(e.CellBounds.X + 14, e.CellBounds.Y + 20, 34, 34);
                using (SolidBrush b = new SolidBrush(ColorTranslator.FromHtml("#B7EBD7")))
                    e.Graphics.FillEllipse(b, circle);

                TextRenderer.DrawText(
                    e.Graphics,
                    icon,
                    new Font("Segoe UI Symbol", 10F, FontStyle.Bold),
                    circle,
                    PrimaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 11F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 60, e.CellBounds.Y + 8, e.CellBounds.Width - 66, e.CellBounds.Height - 16),
                    PrimaryText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "BookCount")
            {
                e.PaintBackground(e.CellBounds, true);

                int count = 0;
                int.TryParse(text, out count);

                int percent = 20;
                if (count >= 1200) percent = 85;
                else if (count >= 800) percent = 65;
                else if (count >= 400) percent = 40;

                TextRenderer.DrawText(
                    e.Graphics,
                    count.ToString("N0"),
                    new Font("Segoe UI", 11F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + 8, 56, e.CellBounds.Height - 16),
                    PrimaryText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                Rectangle rail = new Rectangle(e.CellBounds.X + 54, e.CellBounds.Y + (e.CellBounds.Height - 6) / 2, 102, 6);
                Rectangle fill = new Rectangle(rail.X, rail.Y, (rail.Width * percent) / 100, 6);

                using (SolidBrush bg = new SolidBrush(LineSoft))
                    e.Graphics.FillRectangle(bg, rail);
                using (SolidBrush fg = new SolidBrush(AccentDeep))
                    e.Graphics.FillRectangle(fg, fill);

                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle editRect = new Rectangle(e.CellBounds.Right - 80, e.CellBounds.Y + (e.CellBounds.Height - 24) / 2, 24, 24);
                Rectangle archiveRect = new Rectangle(e.CellBounds.Right - 40, e.CellBounds.Y + (e.CellBounds.Height - 24) / 2, 24, 24);

                TextRenderer.DrawText(
                    e.Graphics,
                    "✎",
                    new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                    editRect,
                    AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    "⌦",
                    new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                    archiveRect,
                    AccentDanger,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;

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