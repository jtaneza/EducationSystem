using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class BookCatalogForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#E2E9EC");
        private readonly Color Outline = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#64748B");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentMint = ColorTranslator.FromHtml("#6DFAD2");

        private readonly Color GoodSoft = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color WarnSoft = ColorTranslator.FromHtml("#FDE7E2");
        private readonly Color WarnText = ColorTranslator.FromHtml("#A03F30");

        private Panel headerPanel = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Button btnFilter = null!;
        private Button btnAddEntry = null!;

        private Panel statsPanel = null!;
        private Panel cardTotal = null!;
        private Panel cardInstitutions = null!;
        private Panel cardRate = null!;

        private Label lblTotalValue = null!;
        private Label lblInstitutionValue = null!;
        private Label lblRateValue = null!;
        private Panel circulationTrack = null!;

        private Panel tableCard = null!;
        private DataGridView dgvBooks = null!;
        private Panel footerPanel = null!;
        private Label lblFooterInfo = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Label lblEllipsis = null!;
        private Button btnPageLast = null!;
        private Button btnNext = null!;

        public BookCatalogForm()
        {
            InitializeComponent();
            BuildUI();
            LoadSampleBooks();
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = true;

            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 170,
                BackColor = Background
            };

            lblTitle = new Label
            {
                Text = "Global Book Catalog",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "A unified view of all books across all client institutions.",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(85, 100, 110),
                AutoSize = true
            };

            btnFilter = new Button
            {
                Text = "☰  Filter by Category  ▾",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(220, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Surface,
                ForeColor = OnSurface,
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderColor = Outline;
            btnFilter.FlatAppearance.BorderSize = 1;

            btnAddEntry = new Button
            {
                Text = "+  Add New Entry",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Size = new Size(186, 44),
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAddEntry.FlatAppearance.BorderSize = 0;

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubTitle);
            headerPanel.Controls.Add(btnFilter);
            headerPanel.Controls.Add(btnAddEntry);

            statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 210,
                BackColor = Background
            };

            cardTotal = CreateCardPanel();
            cardInstitutions = CreateCardPanel();
            cardRate = CreateCardPanel();

            BuildStatsCards();

            statsPanel.Controls.Add(cardTotal);
            statsPanel.Controls.Add(cardInstitutions);
            statsPanel.Controls.Add(cardRate);

            tableCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(0)
            };

            dgvBooks = new DataGridView
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
                ColumnHeadersHeight = 56,
                GridColor = Outline,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvBooks.RowTemplate.Height = 84;
            dgvBooks.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            dgvBooks.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 12, 10, 12);

            dgvBooks.ColumnHeadersDefaultCellStyle.BackColor = SurfaceLow;
            dgvBooks.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvBooks.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceLow;
            dgvBooks.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;

            dgvBooks.DefaultCellStyle.BackColor = Surface;
            dgvBooks.DefaultCellStyle.ForeColor = OnSurface;
            dgvBooks.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvBooks.DefaultCellStyle.SelectionBackColor = Color.FromArgb(244, 251, 249);
            dgvBooks.DefaultCellStyle.SelectionForeColor = OnSurface;
            dgvBooks.AlternatingRowsDefaultCellStyle.BackColor = Surface;

            dgvBooks.Columns.Add("BookId", "BOOK ID");
            dgvBooks.Columns.Add("BookName", "BOOK NAME");
            dgvBooks.Columns.Add("Category", "CATEGORY");
            dgvBooks.Columns.Add("AddedBy", "ADDED BY");
            dgvBooks.Columns.Add("Status", "STATUS");

            dgvBooks.Columns["BookId"].FillWeight = 14;
            dgvBooks.Columns["BookName"].FillWeight = 32;
            dgvBooks.Columns["Category"].FillWeight = 16;
            dgvBooks.Columns["AddedBy"].FillWeight = 22;
            dgvBooks.Columns["Status"].FillWeight = 16;

            foreach (DataGridViewColumn col in dgvBooks.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvBooks.CellPainting += dgvBooks_CellPainting;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 64,
                BackColor = SurfaceLow
            };

            lblFooterInfo = new Label
            {
                Text = "Showing 1 to 4 of 142,839 entries",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = SecondaryText,
                AutoSize = true
            };

            btnPrev = CreatePagerButton("‹", false, false);
            btnPage1 = CreatePagerButton("1", true, true);
            btnPage2 = CreatePagerButton("2", false, true);
            btnPage3 = CreatePagerButton("3", false, true);
            lblEllipsis = new Label
            {
                Text = "...",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurface,
                BackColor = Color.Transparent
            };
            btnPageLast = CreatePagerButton("1204", false, true);
            btnNext = CreatePagerButton("›", false, false);

            footerPanel.Controls.Add(lblFooterInfo);
            footerPanel.Controls.Add(btnPrev);
            footerPanel.Controls.Add(btnPage1);
            footerPanel.Controls.Add(btnPage2);
            footerPanel.Controls.Add(btnPage3);
            footerPanel.Controls.Add(lblEllipsis);
            footerPanel.Controls.Add(btnPageLast);
            footerPanel.Controls.Add(btnNext);

            tableCard.Controls.Add(dgvBooks);
            tableCard.Controls.Add(footerPanel);

            Controls.Add(tableCard);
            Controls.Add(statsPanel);
            Controls.Add(headerPanel);

            Resize += BookCatalogForm_Resize;
            AdjustLayout();
        }

        private Panel CreateCardPanel()
        {
            return new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void BuildStatsCards()
        {
            Label totalTitle = new Label
            {
                Text = "TOTAL UNIFIED CATALOG",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 70, 72),
                AutoSize = true,
                Location = new Point(28, 34)
            };

            lblTotalValue = new Label
            {
                Text = "142,839",
                Font = new Font("Segoe UI", 44F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(28, 66)
            };

            Label totalTrend = new Label
            {
                Text = "↗  +1,204 this month",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = AccentDeep,
                AutoSize = true,
                Location = new Point(30, 146)
            };

            cardTotal.Controls.Add(totalTitle);
            cardTotal.Controls.Add(lblTotalValue);
            cardTotal.Controls.Add(totalTrend);

            Label institutionsTitle = new Label
            {
                Text = "ACTIVE\r\nINSTITUTIONS",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 70, 72),
                AutoSize = true,
                Location = new Point(28, 30)
            };

            lblInstitutionValue = new Label
            {
                Text = "42",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(28, 88)
            };

            cardInstitutions.Controls.Add(institutionsTitle);
            cardInstitutions.Controls.Add(lblInstitutionValue);
            cardInstitutions.Controls.Add(CreateAvatarBadge("HA", 28, 146, Color.FromArgb(230, 234, 238), OnSurface));
            cardInstitutions.Controls.Add(CreateAvatarBadge("SV", 58, 146, Color.FromArgb(210, 215, 223), OnSurface));
            cardInstitutions.Controls.Add(CreateAvatarBadge("LC", 88, 146, Color.FromArgb(180, 190, 204), OnSurface));
            cardInstitutions.Controls.Add(CreateAvatarBadge("+39", 118, 146, AccentMint, OnSurface));

            Label rateTitle = new Label
            {
                Text = "CIRCULATION\r\nRATE",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 70, 72),
                AutoSize = true,
                Location = new Point(28, 30)
            };

            lblRateValue = new Label
            {
                Text = "68%",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(28, 88)
            };

            circulationTrack = new Panel
            {
                BackColor = SurfaceHigh,
                Size = new Size(176, 10),
                Location = new Point(28, 150)
            };

            Panel fill = new Panel
            {
                BackColor = AccentEmerald,
                Size = new Size((int)(176 * 0.68), 10),
                Location = new Point(0, 0)
            };

            circulationTrack.Controls.Add(fill);

            cardRate.Controls.Add(rateTitle);
            cardRate.Controls.Add(lblRateValue);
            cardRate.Controls.Add(circulationTrack);
        }

        private Panel CreateAvatarBadge(string text, int x, int y, Color back, Color fore)
        {
            Panel panel = new Panel
            {
                Size = new Size(30, 30),
                BackColor = back,
                Location = new Point(x, y)
            };

            Label lbl = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = fore
            };

            panel.Controls.Add(lbl);
            return panel;
        }

        private Button CreatePagerButton(string text, bool active, bool boxed)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 34
            };

            if (boxed)
            {
                btn.Width = text.Length > 2 ? 54 : 34;
            }
            else
            {
                btn.Width = 34;
            }

            if (active)
            {
                btn.BackColor = AccentDeep;
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

        private void LoadSampleBooks()
        {
            dgvBooks.Rows.Clear();

            dgvBooks.Rows.Add("BK-90234", "The Great Gatsby|cover1", "Fiction", "Harborview Science Academy|Admin: Elena Rossi", "In Stock");
            dgvBooks.Rows.Add("BK-44812", "Advanced Quantum Physics|cover2", "Science", "St. Jude Medical Univ.|Admin: Marcus Thorne", "Borrowed");
            dgvBooks.Rows.Add("BK-11029", "The Rise of Empires|cover3", "History", "Global Heritage High|Admin: Sarah Jenkins", "In Stock");
            dgvBooks.Rows.Add("BK-55391", "Calculus Vol. 2|cover4", "Mathematics", "Lakeview Polytechnic|Admin: David Wu", "In Stock");

            dgvBooks.ClearSelection();
        }

        private void BookCatalogForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 24;
            int width = ClientSize.Width - (margin * 2);

            lblTitle.Location = new Point(margin, 34);
            lblSubTitle.Location = new Point(margin, 84);

            btnAddEntry.Location = new Point(ClientSize.Width - btnAddEntry.Width - margin, 76);
            btnFilter.Location = new Point(btnAddEntry.Left - btnFilter.Width - 12, 78);

            if (width >= 1120)
            {
                statsPanel.Height = 230;

                int leftWide = (int)(width * 0.50);
                int rightSmall = (width - leftWide - (gap * 2)) / 2;

                cardTotal.Bounds = new Rectangle(margin, 14, leftWide, 190);
                cardInstitutions.Bounds = new Rectangle(cardTotal.Right + gap, 14, rightSmall, 190);
                cardRate.Bounds = new Rectangle(cardInstitutions.Right + gap, 14, rightSmall, 190);
            }
            else
            {
                statsPanel.Height = 430;

                int full = width;
                int half = (width - gap) / 2;

                cardTotal.Bounds = new Rectangle(margin, 14, full, 180);
                cardInstitutions.Bounds = new Rectangle(margin, cardTotal.Bottom + gap, half, 180);
                cardRate.Bounds = new Rectangle(cardInstitutions.Right + gap, cardTotal.Bottom + gap, half, 180);
            }

            lblFooterInfo.Location = new Point(24, 22);

            btnNext.Location = new Point(footerPanel.Width - 34, 15);
            btnPageLast.Location = new Point(btnNext.Left - btnPageLast.Width - 6, 15);
            lblEllipsis.Location = new Point(btnPageLast.Left - 26, 22);
            btnPage3.Location = new Point(lblEllipsis.Left - btnPage3.Width - 6, 15);
            btnPage2.Location = new Point(btnPage3.Left - btnPage2.Width - 6, 15);
            btnPage1.Location = new Point(btnPage2.Left - btnPage1.Width - 6, 15);
            btnPrev.Location = new Point(btnPage1.Left - btnPrev.Width - 6, 15);
        }

        private void dgvBooks_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvBooks.Columns[e.ColumnIndex].Name;

            if (col == "BookId")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Rectangle badge = new Rectangle(e.CellBounds.X + 18, e.CellBounds.Y + 18, 54, 40);

                using (SolidBrush b = new SolidBrush(SurfaceLow))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text.Replace("-", "-\n"),
                    new Font("Consolas", 9F, FontStyle.Bold),
                    badge,
                    Color.FromArgb(60, 70, 72),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (col == "BookName")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string title = parts[0];

                Rectangle cover = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 12, 40, 50);

                using (SolidBrush b = new SolidBrush(Color.FromArgb(40, 43, 50, 52)))
                    e.Graphics.FillRectangle(b, cover);

                using (Pen p = new Pen(Color.FromArgb(90, 109, 250, 210), 1))
                    e.Graphics.DrawRectangle(p, cover);

                TextRenderer.DrawText(
                    e.Graphics,
                    "BOOK",
                    new Font("Segoe UI", 6.5F, FontStyle.Bold),
                    cover,
                    AccentMint,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                Rectangle titleRect = new Rectangle(cover.Right + 14, e.CellBounds.Y, e.CellBounds.Width - 70, e.CellBounds.Height);

                TextRenderer.DrawText(
                    e.Graphics,
                    title,
                    new Font("Segoe UI", 11F, FontStyle.Bold),
                    titleRect,
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

                e.Handled = true;
            }
            else if (col == "Category")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Size size = TextRenderer.MeasureText(text, new Font("Segoe UI", 10F));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + (e.CellBounds.Height - 28) / 2,
                    size.Width + 22,
                    28
                );

                using (SolidBrush b = new SolidBrush(SurfaceHigh))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Regular),
                    badge,
                    Color.FromArgb(70, 80, 84),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (col == "AddedBy")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string school = parts.Length > 0 ? parts[0] : "";
                string admin = parts.Length > 1 ? parts[1] : "";

                Rectangle schoolRect = new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + 14, e.CellBounds.Width - 16, 26);
                Rectangle adminRect = new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + 40, e.CellBounds.Width - 16, 18);

                TextRenderer.DrawText(
                    e.Graphics,
                    school,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    schoolRect,
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

                TextRenderer.DrawText(
                    e.Graphics,
                    admin.ToUpper(),
                    new Font("Segoe UI", 7.5F, FontStyle.Regular),
                    adminRect,
                    SecondaryText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                bool inStock = text == "In Stock";

                Color back = inStock ? GoodSoft : WarnSoft;
                Color fore = inStock ? AccentDeep : WarnText;
                Color dot = inStock ? AccentDeep : WarnText;

                Size size = TextRenderer.MeasureText(text, new Font("Segoe UI", 9F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 12,
                    e.CellBounds.Y + (e.CellBounds.Height - 28) / 2,
                    size.Width + 28,
                    28
                );

                using (SolidBrush b = new SolidBrush(back))
                    e.Graphics.FillRectangle(b, badge);

                Rectangle dotRect = new Rectangle(badge.X + 10, badge.Y + 10, 8, 8);
                using (SolidBrush b = new SolidBrush(dot))
                    e.Graphics.FillEllipse(b, dotRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 9F, FontStyle.Bold),
                    new Rectangle(dotRect.Right + 6, badge.Y, badge.Width - 22, badge.Height),
                    fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
        }
    }
}