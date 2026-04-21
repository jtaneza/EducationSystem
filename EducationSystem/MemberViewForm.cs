using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class MemberViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color Outline = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");
        private readonly Color Tertiary = ColorTranslator.FromHtml("#A03F30");

        private Panel headerPanel = null!;
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
        private Panel cardActive = null!;
        private Panel cardPending = null!;
        private Panel cardHealth = null!;

        private Label lblTotalValue = null!;
        private Label lblActiveValue = null!;
        private Label lblPendingValue = null!;
        private Label lblHealthValue = null!;

        private Panel tableCard = null!;
        private Panel tableShell = null!;
        private DataGridView dgvMembers = null!;
        private Panel footerPanel = null!;
        private Label lblFooter = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNext = null!;

        private List<MemberItem> allRows = new List<MemberItem>();

        public MemberViewForm()
        {
            InitializeComponent();
            BuildUI();
            SeedData();
            LoadSchoolFilter();
            LoadGrid();
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
                Height = 150,
                BackColor = Background
            };

            lblTitle = new Label
            {
                Text = "Member Directory",
                Font = new Font("Segoe UI", 25F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "Management and monitoring of system-wide academic personnel.",
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
            cboSchoolFilter.SelectedIndexChanged += (s, e) => LoadGrid();
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
                Text = "Search by name or email..."
            };
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by name or email...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by name or email...";
                    txtSearch.ForeColor = OnSurfaceVariant;
                }
            };
            txtSearch.TextChanged += (s, e) => LoadGrid();

            btnFilter = new Button
            {
                Text = "☰",
                FlatStyle = FlatStyle.Flat,
                BackColor = SurfaceContainer,
                ForeColor = OnSurfaceVariant,
                Size = new Size(38, 38),
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderSize = 0;

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubTitle);
            headerPanel.Controls.Add(filterHost);
            headerPanel.Controls.Add(searchHost);
            headerPanel.Controls.Add(btnFilter);

            statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 190,
                BackColor = Background
            };

            cardTotal = CreateStatCard("TOTAL MEMBERS", out lblTotalValue, "+4 this month", Primary, "↗");
            cardActive = CreateStatCard("ACTIVE NOW", out lblActiveValue, "Current Sessions", Primary, "●");
            cardPending = CreateStatCard("PENDING APPROVALS", out lblPendingValue, "Requires Action", Tertiary, "◔");
            cardHealth = CreateStatCard("SYSTEM HEALTH", out lblHealthValue, "Operational", Primary, "🛡");

            statsPanel.Controls.Add(cardTotal);
            statsPanel.Controls.Add(cardActive);
            statsPanel.Controls.Add(cardPending);
            statsPanel.Controls.Add(cardHealth);

            tableCard = new Panel
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

            dgvMembers = new DataGridView
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
                ColumnHeadersHeight = 52,
                GridColor = Outline,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvMembers.RowTemplate.Height = 88;
            dgvMembers.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            dgvMembers.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 10, 8, 10);

            dgvMembers.ColumnHeadersDefaultCellStyle.BackColor = SurfaceLow;
            dgvMembers.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvMembers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvMembers.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceLow;
            dgvMembers.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;

            dgvMembers.DefaultCellStyle.BackColor = Surface;
            dgvMembers.DefaultCellStyle.ForeColor = OnSurface;
            dgvMembers.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvMembers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvMembers.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvMembers.Columns.Add("Id", "ID");
            dgvMembers.Columns.Add("Name", "NAME");
            dgvMembers.Columns.Add("Email", "EMAIL");
            dgvMembers.Columns.Add("School", "SCHOOL");
            dgvMembers.Columns.Add("Phone", "PHONE");
            dgvMembers.Columns.Add("Address", "ADDRESS");
            dgvMembers.Columns.Add("Status", "STATUS");

            dgvMembers.Columns["Id"].FillWeight = 13;
            dgvMembers.Columns["Name"].FillWeight = 18;
            dgvMembers.Columns["Email"].FillWeight = 18;
            dgvMembers.Columns["School"].FillWeight = 18;
            dgvMembers.Columns["Phone"].FillWeight = 14;
            dgvMembers.Columns["Address"].FillWeight = 17;
            dgvMembers.Columns["Status"].FillWeight = 12;

            foreach (DataGridViewColumn col in dgvMembers.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvMembers.CellPainting += dgvMembers_CellPainting;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 64,
                BackColor = Color.FromArgb(250, 252, 252)
            };

            lblFooter = new Label
            {
                Text = "Showing 1 to 4 of 1,402 members",
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

            tableShell.Controls.Add(dgvMembers);
            tableShell.Controls.Add(footerPanel);
            tableCard.Controls.Add(tableShell);

            Controls.Add(tableCard);
            Controls.Add(statsPanel);
            Controls.Add(headerPanel);

            Resize += MemberViewForm_Resize;
            AdjustLayout();
        }

        private Panel CreateStatCard(string title, out Label valueLabel, string footerText, Color footerColor, string footerIcon)
        {
            Panel panel = new Panel
            {
                BackColor = SurfaceLow,
                BorderStyle = BorderStyle.None
            };

            Label lblTitleCard = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(24, 20)
            };

            valueLabel = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(24, 48)
            };

            Label lblFooterCard = new Label
            {
                Text = $"{footerIcon} {footerText}",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = footerColor,
                AutoSize = true,
                Location = new Point(24, 100)
            };

            panel.Controls.Add(lblTitleCard);
            panel.Controls.Add(valueLabel);
            panel.Controls.Add(lblFooterCard);

            return panel;
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
                btn.BackColor = PrimaryContainer;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btn.BackColor = SurfaceContainer;
                btn.ForeColor = OnSurface;
                btn.FlatAppearance.BorderSize = 0;
            }

            return btn;
        }

        private void SeedData()
        {
            allRows = new List<MemberItem>
            {
                new MemberItem("#MEM-8821", "IMG", "Elena Vance", "#F2D3C6", "", "evance@univ-central.edu", "+1 (555) 012-9932", "423 Academic Square, Apt 8B", "University of the Philippines", "Active", "#B7EBD7", "#1E4F41"),
                new MemberItem("#MEM-8822", "IMG", "Julian Sol", "#D95C4E", "", "j.sol@univ-central.edu", "+1 (555) 231-0012", "89 Science Park Rd, Faculty Housing", "Ateneo de Manila University", "Active", "#B7EBD7", "#1E4F41"),
                new MemberItem("#MEM-8823", "MK", "Mina Kinsley", "#B7EBD7", "#1E4F41", "mina.k@student.edu", "+1 (555) 441-2990", "North Dormitory, Room 102", "UST Manila", "Suspended", "#F7816D", "#6E1B0F"),
                new MemberItem("#MEM-8824", "IMG", "Marcus Thorne", "#1E2A35", "", "m.thorne@inst-tech.org", "+1 (555) 998-1122", "International Guesthouse, Suite 12", "University of San Carlos", "Active", "#B7EBD7", "#1E4F41")
            };
        }

        private void LoadSchoolFilter()
        {
            cboSchoolFilter.Items.Clear();
            cboSchoolFilter.Items.Add("All Schools");

            foreach (string school in allRows.Select(x => x.School).Distinct().OrderBy(x => x))
                cboSchoolFilter.Items.Add(school);

            cboSchoolFilter.SelectedIndex = 0;
        }

        private void LoadGrid()
        {
            dgvMembers.Rows.Clear();

            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string term = txtSearch.Text;
            bool useFilter = !string.IsNullOrWhiteSpace(term) && term != "Search by name or email...";

            IEnumerable<MemberItem> filtered = allRows;

            if (selectedSchool != "All Schools")
                filtered = filtered.Where(x => x.School == selectedSchool);

            if (useFilter)
            {
                filtered = filtered.Where(x =>
                    x.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Email.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            List<MemberItem> results = filtered.ToList();

            foreach (var row in results)
            {
                dgvMembers.Rows.Add(
                    row.Id,
                    $"{row.AvatarKind}|{row.Name}|{row.AvatarBack}|{row.AvatarFore}",
                    row.Email,
                    row.School,
                    row.Phone,
                    row.Address,
                    $"{row.Status}|{row.StatusBack}|{row.StatusFore}"
                );
            }

            dgvMembers.ClearSelection();

            lblTotalValue.Text = "1,402";
            lblActiveValue.Text = "28";
            lblPendingValue.Text = "03";
            lblHealthValue.Text = "98%";

            lblFooter.Text = $"Showing 1 to {results.Count} of {results.Count} members";
        }

        private void dgvMembers_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string column = dgvMembers.Columns[e.ColumnIndex].Name;

            if (column == "Id")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string display = raw.Replace("-", "-\n");

                TextRenderer.DrawText(
                    e.Graphics,
                    display,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 14, e.CellBounds.Width - 20, e.CellBounds.Height - 20),
                    Primary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

                e.Handled = true;
            }
            else if (column == "Name")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');

                string kind = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : "";
                Color faceColor = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : SurfaceContainer;
                Color faceFore = parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3])
                    ? ColorTranslator.FromHtml(parts[3])
                    : Color.White;

                Rectangle avatar = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 24, 34, 34);

                using (SolidBrush brush = new SolidBrush(faceColor))
                    e.Graphics.FillEllipse(brush, avatar);

                string avatarText = kind == "IMG" ? "●" : kind;
                Font avatarFont = kind == "IMG"
                    ? new Font("Segoe UI Symbol", 10F, FontStyle.Bold)
                    : new Font("Segoe UI", 9F, FontStyle.Bold);

                TextRenderer.DrawText(
                    e.Graphics,
                    avatarText,
                    avatarFont,
                    avatar,
                    faceFore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(avatar.Right + 12, e.CellBounds.Y, e.CellBounds.Width - 56, e.CellBounds.Height),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

                e.Handled = true;
            }
            else if (column == "School")
            {
                e.PaintBackground(e.CellBounds, true);

                TextRenderer.DrawText(
                    e.Graphics,
                    e.FormattedValue?.ToString() ?? "",
                    new Font("Segoe UI", 10F),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis
                );

                e.Handled = true;
            }
            else if (column == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string text = parts[0];
                Color back = parts.Length > 1 ? ColorTranslator.FromHtml(parts[1]) : SurfaceContainer;
                Color fore = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : OnSurface;

                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 9F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 12,
                    e.CellBounds.Y + (e.CellBounds.Height - 28) / 2,
                    textSize.Width + 28,
                    28
                );

                using (SolidBrush brush = new SolidBrush(back))
                    e.Graphics.FillRectangle(brush, badge);

                Rectangle dotRect = new Rectangle(badge.X + 8, badge.Y + 10, 8, 8);
                using (SolidBrush dotBrush = new SolidBrush(fore))
                    e.Graphics.FillEllipse(dotBrush, dotRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 9F, FontStyle.Bold),
                    new Rectangle(dotRect.Right + 6, badge.Y, badge.Width - 20, badge.Height),
                    fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
        }

        private void MemberViewForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 22;
            int width = ClientSize.Width - (margin * 2);

            lblTitle.Location = new Point(margin, 26);
            lblSubTitle.Location = new Point(margin, 64);

            btnFilter.Location = new Point(ClientSize.Width - 50, 34);

            searchHost.Location = new Point(btnFilter.Left - 290, 34);
            searchHost.Size = new Size(280, 38);

            filterHost.Location = new Point(searchHost.Left - 230, 34);
            filterHost.Size = new Size(220, 38);

            cboSchoolFilter.Location = new Point(10, 6);
            cboSchoolFilter.Width = filterHost.Width - 20;

            lblSearchIcon.Location = new Point(10, 8);
            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = 220;

            int statWidth = (width - (gap * 3)) / 4;
            int statHeight = 132;

            cardTotal.Bounds = new Rectangle(margin, 18, statWidth, statHeight);
            cardActive.Bounds = new Rectangle(cardTotal.Right + gap, 18, statWidth, statHeight);
            cardPending.Bounds = new Rectangle(cardActive.Right + gap, 18, statWidth, statHeight);
            cardHealth.Bounds = new Rectangle(cardPending.Right + gap, 18, statWidth, statHeight);

            lblFooter.Location = new Point(24, 20);

            btnNext.Location = new Point(footerPanel.Width - 42, 13);
            btnPage3.Location = new Point(btnNext.Left - 42, 13);
            btnPage2.Location = new Point(btnPage3.Left - 42, 13);
            btnPage1.Location = new Point(btnPage2.Left - 42, 13);
            btnPrev.Location = new Point(btnPage1.Left - 42, 13);
        }
    }

    public class MemberItem
    {
        public string Id { get; set; }
        public string AvatarKind { get; set; }
        public string Name { get; set; }
        public string AvatarBack { get; set; }
        public string AvatarFore { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string School { get; set; }
        public string Status { get; set; }
        public string StatusBack { get; set; }
        public string StatusFore { get; set; }

        public MemberItem(
            string id,
            string avatarKind,
            string name,
            string avatarBack,
            string avatarFore,
            string email,
            string phone,
            string address,
            string school,
            string status,
            string statusBack,
            string statusFore)
        {
            Id = id;
            AvatarKind = avatarKind;
            Name = name;
            AvatarBack = avatarBack;
            AvatarFore = avatarFore;
            Email = email;
            Phone = phone;
            Address = address;
            School = school;
            Status = status;
            StatusBack = statusBack;
            StatusFore = statusFore;
        }
    }
}