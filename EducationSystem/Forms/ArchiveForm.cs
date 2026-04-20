using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ArchiveForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#E2E9EC");
        private readonly Color Outline = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");

        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#376758");

        private readonly Color TertiaryText = ColorTranslator.FromHtml("#A03F30");

        private Panel canvas = null!;
        private Panel pageHeader = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;

        private Panel tableCard = null!;
        private Panel tableTopBar = null!;
        private Label lblTableTitle = null!;
        private Label badgeEntities = null!;
        private Label badgeDays = null!;
        
        private Panel filterHost = null!;
        private ComboBox cboInstitution = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;

        private DataGridView dgvArchive = null!;
        private Panel footerPanel = null!;
        private Label lblFooterInfo = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Label lblEllipsis = null!;
        private Button btnPageLast = null!;
        private Button btnNext = null!;

        public ArchiveForm()
        {
            InitializeComponent();
            BuildUI();
            SeedArchiveIfEmpty();
            LoadInstitutionFilter();
            LoadArchiveItems();
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
                Padding = new Padding(0, 0, 0, 30)
            };

            BuildHeader();
            BuildArchiveTable();

            canvas.Controls.Add(tableCard);
            canvas.Controls.Add(pageHeader);

            Controls.Add(canvas);

            Resize += ArchiveForm_Resize;
            AdjustLayout();
        }

        private void BuildHeader()
        {
            pageHeader = new Panel
            {
                BackColor = Background,
                Height = 100
            };

            lblTitle = new Label
            {
                Text = "Super Admin Archive",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "View and manage all archived transactions across all institutions.",
                Font = new Font("Segoe UI", 11.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            pageHeader.Controls.Add(lblTitle);
            pageHeader.Controls.Add(lblSubTitle);
        }

        private void BuildArchiveTable()
        {
            tableCard = new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle
            };

            tableTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                BackColor = Color.FromArgb(248, 251, 252)
            };

            lblTableTitle = new Label
            {
                Text = "Archived Records",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            badgeEntities = CreateTopBadge("All Entities", AccentEmerald, Color.White);
            badgeDays = CreateTopBadge("Last 30 Days", SurfaceHigh, OnSurfaceVariant);

            filterHost = new Panel
            {
                BackColor = Color.FromArgb(241, 245, 246),
                Size = new Size(220, 38)
            };

            cboInstitution = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                BackColor = filterHost.BackColor,
                ForeColor = OnSurfaceVariant
            };
            cboInstitution.SelectedIndexChanged += (s, e) => LoadArchiveItems();

            filterHost.Controls.Add(cboInstitution);

            searchHost = new Panel
            {
                BackColor = Color.FromArgb(241, 245, 246),
                Size = new Size(250, 38)
            };

            lblSearchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 15F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = searchHost.BackColor,
                ForeColor = OnSurfaceVariant,
                Font = new Font("Segoe UI", 10F),
                Text = "Search archive..."
            };
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search archive...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search archive...";
                    txtSearch.ForeColor = OnSurfaceVariant;
                }
            };
            txtSearch.TextChanged += (s, e) => LoadArchiveItems();

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            tableTopBar.Controls.Add(lblTableTitle);
            tableTopBar.Controls.Add(badgeEntities);
            tableTopBar.Controls.Add(badgeDays);
            tableTopBar.Controls.Add(filterHost);
            tableTopBar.Controls.Add(searchHost);

            dgvArchive = new DataGridView
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
                ColumnHeadersHeight = 48,
                GridColor = Outline,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvArchive.RowTemplate.Height = 66;
            dgvArchive.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);

            dgvArchive.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(243, 247, 248);
            dgvArchive.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvArchive.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvArchive.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 247, 248);
            dgvArchive.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;
            dgvArchive.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvArchive.DefaultCellStyle.BackColor = Surface;
            dgvArchive.DefaultCellStyle.ForeColor = OnSurface;
            dgvArchive.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvArchive.DefaultCellStyle.SelectionBackColor = Color.FromArgb(244, 251, 249);
            dgvArchive.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvArchive.Columns.Add("ArchiveID", "ID");
            dgvArchive.Columns.Add("Institution", "Institution");
            dgvArchive.Columns.Add("Type", "Type");
            dgvArchive.Columns.Add("ArchivedDate", "Date Archived");
            dgvArchive.Columns.Add("Actions", "Actions");

            dgvArchive.Columns["ArchiveID"].FillWeight = 14;
            dgvArchive.Columns["Institution"].FillWeight = 37;
            dgvArchive.Columns["Type"].FillWeight = 15;
            dgvArchive.Columns["ArchivedDate"].FillWeight = 22;
            dgvArchive.Columns["Actions"].FillWeight = 12;

            dgvArchive.Columns["ArchiveID"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Type"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["ArchivedDate"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Actions"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvArchive.Columns["ArchiveID"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Type"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["ArchivedDate"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Actions"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvArchive.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvArchive.CellPainting += dgvArchive_CellPainting;
            dgvArchive.CellClick += dgvArchive_CellClick;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = Surface
            };

            lblFooterInfo = new Label
            {
                Text = "Showing 1 to 5 of 12,482 entries",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
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
                ForeColor = OnSurfaceVariant
            };

            btnPageLast = CreatePagerButton("2496", false, true);
            btnNext = CreatePagerButton("›", false, false);

            footerPanel.Controls.Add(lblFooterInfo);
            footerPanel.Controls.Add(btnPrev);
            footerPanel.Controls.Add(btnPage1);
            footerPanel.Controls.Add(btnPage2);
            footerPanel.Controls.Add(btnPage3);
            footerPanel.Controls.Add(lblEllipsis);
            footerPanel.Controls.Add(btnPageLast);
            footerPanel.Controls.Add(btnNext);

            tableCard.Controls.Add(dgvArchive);
            tableCard.Controls.Add(footerPanel);
            tableCard.Controls.Add(tableTopBar);
        }

        private Label CreateTopBadge(string text, Color back, Color fore)
        {
            return new Label
            {
                Text = text.ToUpper(),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Padding = new Padding(10, 5, 10, 5),
                BackColor = back,
                ForeColor = fore
            };
        }

        private Button CreatePagerButton(string text, bool active, bool boxed)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 34,
                Width = boxed ? (text.Length > 2 ? 54 : 34) : 34
            };

            if (active)
            {
                btn.BackColor = AccentEmerald;
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

        private void SeedArchiveIfEmpty()
        {
            if (ArchiveStore.ArchivedItems.Count > 0) return;

            ArchiveStore.ArchivedItems.Add(new ArchiveItem
            {
                ArchiveID = "#LF-9021",
                Module = "Transaction",
                RecordID = "TRX-1021",
                ItemName = "Oakwood College Library",
                ExtraInfo = "OC",
                ArchivedBy = "Admin Astra",
                ArchivedDate = new DateTime(2023, 10, 12, 14, 30, 0),
                Status = "Archived"
            });

            ArchiveStore.ArchivedItems.Add(new ArchiveItem
            {
                ArchiveID = "#LF-8842",
                Module = "Book",
                RecordID = "BK-8842",
                ItemName = "Summit View Academy",
                ExtraInfo = "SV",
                ArchivedBy = "Admin Astra",
                ArchivedDate = new DateTime(2023, 10, 11, 9, 15, 0),
                Status = "Archived"
            });

            ArchiveStore.ArchivedItems.Add(new ArchiveItem
            {
                ArchiveID = "#LF-7920",
                Module = "Member",
                RecordID = "MB-7920",
                ItemName = "Metropolis University",
                ExtraInfo = "MU",
                ArchivedBy = "Admin Astra",
                ArchivedDate = new DateTime(2023, 10, 8, 18, 44, 0),
                Status = "Archived"
            });

            ArchiveStore.ArchivedItems.Add(new ArchiveItem
            {
                ArchiveID = "#LF-7712",
                Module = "Transaction",
                RecordID = "TRX-7712",
                ItemName = "Oakwood College Library",
                ExtraInfo = "OC",
                ArchivedBy = "Admin Astra",
                ArchivedDate = new DateTime(2023, 10, 5, 11, 20, 0),
                Status = "Archived"
            });

            ArchiveStore.ArchivedItems.Add(new ArchiveItem
            {
                ArchiveID = "#LF-7690",
                Module = "Book",
                RecordID = "BK-7690",
                ItemName = "Summit View Academy",
                ExtraInfo = "SV",
                ArchivedBy = "Admin Astra",
                ArchivedDate = new DateTime(2023, 9, 28, 16, 50, 0),
                Status = "Archived"
            });
        }

        private void LoadInstitutionFilter()
        {
            cboInstitution.Items.Clear();
            cboInstitution.Items.Add("All Institutions");

            foreach (string school in ArchiveStore.ArchivedItems
                         .Select(x => x.ItemName)
                         .Distinct()
                         .OrderBy(x => x))
            {
                cboInstitution.Items.Add(school);
            }

            cboInstitution.SelectedIndex = 0;
        }

        private void LoadArchiveItems()
        {
            dgvArchive.Rows.Clear();

            string selectedInstitution = cboInstitution.SelectedItem?.ToString() ?? "All Institutions";
            string searchText = txtSearch.Text.Trim();

            bool useSearch = !string.IsNullOrWhiteSpace(searchText) && searchText != "Search archive...";

            var filteredItems = ArchiveStore.ArchivedItems.AsEnumerable();

            if (selectedInstitution != "All Institutions")
            {
                filteredItems = filteredItems.Where(x => x.ItemName == selectedInstitution);
            }

            if (useSearch)
            {
                filteredItems = filteredItems.Where(x =>
                    x.ArchiveID.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.ItemName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Module.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.RecordID.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.ArchivedBy.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            var results = filteredItems.ToList();

            foreach (ArchiveItem item in results)
            {
                dgvArchive.Rows.Add(
                    item.ArchiveID,
                    $"{item.ExtraInfo}|{item.ItemName}|{GetInstitutionBack(item.Module)}|{GetInstitutionFore(item.Module)}",
                    item.Module,
                    item.ArchivedDate.ToString("MMM dd, yyyy • HH:mm"),
                    "Restore|Delete");
            }

            dgvArchive.ClearSelection();
            lblFooterInfo.Text = $"Showing 1 to {results.Count} of {results.Count:N0} entries";
        }

        private string GetInstitutionBack(string module)
        {
            return module switch
            {
                "Transaction" => "#B7EBD7",
                "Book" => "#D9F4EC",
                "Member" => "#FDE7E2",
                _ => "#E2E9EC"
            };
        }

        private string GetInstitutionFore(string module)
        {
            return module switch
            {
                "Transaction" => "#376758",
                "Book" => "#006B55",
                "Member" => "#A03F30",
                _ => "#3C4A44"
            };
        }

        private void ArchiveForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int width = Math.Max(980, canvas.ClientSize.Width - (margin * 2));

            pageHeader.SetBounds(0, 0, canvas.ClientSize.Width, 100);

            lblTitle.Location = new Point(margin, 28);
            lblSubTitle.Location = new Point(margin, 72);

            tableCard.SetBounds(margin, pageHeader.Bottom + 8, width, 584);

            lblTableTitle.Location = new Point(22, 18);
            badgeEntities.Location = new Point(lblTableTitle.Right + 18, 17);
            badgeDays.Location = new Point(badgeEntities.Right + 8, 17);

            searchHost.Location = new Point(tableTopBar.Width - searchHost.Width - 18, 14);
            lblSearchIcon.Location = new Point(10, 7);
            txtSearch.Location = new Point(38, 10);
            txtSearch.Width = searchHost.Width - 48;

            filterHost.Location = new Point(searchHost.Left - filterHost.Width - 10, 14);
            cboInstitution.Location = new Point(10, 6);
            cboInstitution.Width = filterHost.Width - 20;

            lblFooterInfo.Location = new Point(22, 20);

            btnNext.Location = new Point(footerPanel.Width - 40, 11);
            btnPageLast.Location = new Point(btnNext.Left - btnPageLast.Width - 6, 11);
            lblEllipsis.Location = new Point(btnPageLast.Left - 22, 18);
            btnPage3.Location = new Point(lblEllipsis.Left - btnPage3.Width - 6, 11);
            btnPage2.Location = new Point(btnPage3.Left - btnPage2.Width - 6, 11);
            btnPage1.Location = new Point(btnPage2.Left - btnPage1.Width - 6, 11);
            btnPrev.Location = new Point(btnPage1.Left - btnPrev.Width - 6, 11);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 30);
        }

        private void dgvArchive_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvArchive.Columns[e.ColumnIndex].Name;

            if (col == "Institution")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');

                string code = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : "";
                Color back = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : SurfaceHigh;
                Color fore = parts.Length > 3 ? ColorTranslator.FromHtml(parts[3]) : OnSurface;

                Rectangle badge = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 16, 24, 24);
                using (SolidBrush b = new SolidBrush(back))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    code,
                    new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    badge,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 46, e.CellBounds.Y + 8, e.CellBounds.Width - 54, e.CellBounds.Height - 14),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (col == "Type")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                string icon = text switch
                {
                    "Transaction" => "⇄",
                    "Book" => "📘",
                    "Member" => "👤",
                    _ => "•"
                };

                Color back = text == "Book" ? SecondaryContainer : SurfaceHigh;
                Color fore = text == "Book" ? SecondaryText : OnSurfaceVariant;

                Size textSize = TextRenderer.MeasureText(text.ToUpper(), new Font("Segoe UI", 8F, FontStyle.Bold));
                int badgeWidth = textSize.Width + 34;

                Rectangle badge = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - badgeWidth) / 2,
                    e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                    badgeWidth,
                    24);

                using (SolidBrush b = new SolidBrush(back))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    icon,
                    new Font("Segoe UI Emoji", 9F),
                    new Rectangle(badge.X + 6, badge.Y, 14, badge.Height),
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    text.ToUpper(),
                    new Font("Segoe UI", 8F, FontStyle.Bold),
                    new Rectangle(badge.X + 18, badge.Y, badge.Width - 20, badge.Height),
                    fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                int iconTop = e.CellBounds.Y + (e.CellBounds.Height - 28) / 2;
                int totalWidth = 28 + 8 + 28;
                int startX = e.CellBounds.X + (e.CellBounds.Width - totalWidth) / 2;

                Rectangle restoreRect = new Rectangle(startX, iconTop, 28, 28);
                Rectangle deleteRect = new Rectangle(startX + 36, iconTop, 28, 28);

                using (SolidBrush b = new SolidBrush(Color.FromArgb(20, 0, 184, 148)))
                    e.Graphics.FillRectangle(b, restoreRect);

                using (SolidBrush b = new SolidBrush(Color.FromArgb(20, 247, 129, 109)))
                    e.Graphics.FillRectangle(b, deleteRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    "↺",
                    new Font("Segoe UI Symbol", 11F, FontStyle.Bold),
                    restoreRect,
                    AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    "✕",
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    deleteRect,
                    TertiaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void dgvArchive_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvArchive.Columns[e.ColumnIndex].Name != "Actions") return;

            Rectangle cellRect = dgvArchive.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            Point clientPoint = dgvArchive.PointToClient(Cursor.Position);

            int iconTop = cellRect.Y + (cellRect.Height - 28) / 2;
            int totalWidth = 28 + 8 + 28;
            int startX = cellRect.X + (cellRect.Width - totalWidth) / 2;

            Rectangle restoreRect = new Rectangle(startX, iconTop, 28, 28);
            Rectangle deleteRect = new Rectangle(startX + 36, iconTop, 28, 28);

            string archiveId = dgvArchive.Rows[e.RowIndex].Cells["ArchiveID"].Value?.ToString() ?? "";
            ArchiveItem? item = ArchiveStore.ArchivedItems.FirstOrDefault(x => x.ArchiveID == archiveId);
            if (item == null) return;

            if (restoreRect.Contains(clientPoint))
            {
                ArchiveStore.ArchivedItems.Remove(item);
                LoadInstitutionFilter();
                LoadArchiveItems();
                return;
            }

            if (deleteRect.Contains(clientPoint))
            {
                DialogResult result = MessageBox.Show(
                    $"Permanently delete {item.ArchiveID}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    ArchiveStore.ArchivedItems.Remove(item);
                    LoadInstitutionFilter();
                    LoadArchiveItems();
                }
            }
        }
    }
}