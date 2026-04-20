using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ClientViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color Outline = ColorTranslator.FromHtml("#BBCAC3");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryFixed = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color OnSecondaryContainer = ColorTranslator.FromHtml("#3B6B5C");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");
        private readonly Color OnTertiaryContainer = ColorTranslator.FromHtml("#6E1B0F");
        private readonly Color InverseSurface = ColorTranslator.FromHtml("#2B3234");
        private readonly Color OutlineVariant = ColorTranslator.FromHtml("#DDE4E6");

        private Panel canvas = null!;
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
        private Panel cardActiveAccounts = null!;
        private Panel cardSystemHealth = null!;
        private Label lblActiveTitle = null!;
        private Label lblActiveValue = null!;
        private Label lblActiveTrend = null!;
        private Label lblHealthTitle = null!;
        private Label lblHealthValue = null!;
        private Panel healthLine1 = null!;
        private Panel healthLine2 = null!;
        private Panel healthLine3 = null!;

        private Panel tableShell = null!;
        private DataGridView dgvClients = null!;
        private Panel footerPanel = null!;
        private Label lblFooter = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNext = null!;

        private List<ClientDirectoryItem> allClients = new List<ClientDirectoryItem>();

        public ClientViewForm()
        {
            InitializeComponent();
            BuildUI();
            SeedData();
            LoadSchoolFilter();
            LoadClientsGrid();
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
                Padding = new Padding(0, 0, 0, 50)
            };

            BuildHeader();
            BuildStats();
            BuildTable();

            canvas.Controls.Add(tableShell);
            canvas.Controls.Add(statsPanel);
            canvas.Controls.Add(headerPanel);

            Controls.Add(canvas);

            Resize += ClientViewForm_Resize;
            AdjustLayout();
        }

        private void BuildHeader()
        {
            headerPanel = new Panel
            {
                Height = 160,
                Dock = DockStyle.Top,
                BackColor = Background
            };

            lblTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 32F, FontStyle.Bold),
                ForeColor = OnSurface,
                Text = "Institutional Client Directory"
            };

            lblSubTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = OnSurfaceVariant,
                Text = "High-level oversight of all school library subscribers."
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
            cboSchoolFilter.SelectedIndexChanged += (s, e) => LoadClientsGrid();

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
            txtSearch.TextChanged += (s, e) => LoadClientsGrid();

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
        }

        private void BuildStats()
        {
            statsPanel = new Panel
            {
                Height = 220,
                Dock = DockStyle.Top,
                BackColor = Background
            };

            cardActiveAccounts = new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.None
            };

            lblActiveTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                Text = "ACTIVE ACCOUNTS"
            };

            lblActiveValue = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 38F, FontStyle.Bold),
                ForeColor = Primary,
                Text = "08"
            };

            lblActiveTrend = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = OnSecondaryContainer,
                BackColor = SecondaryContainer,
                Padding = new Padding(12, 6, 12, 6),
                Text = "+1 this week"
            };

            cardActiveAccounts.Controls.Add(lblActiveTitle);
            cardActiveAccounts.Controls.Add(lblActiveValue);
            cardActiveAccounts.Controls.Add(lblActiveTrend);

            cardSystemHealth = new Panel
            {
                BackColor = InverseSurface,
                BorderStyle = BorderStyle.None
            };

            lblHealthTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#BBCAC3"),
                Text = "SYSTEM HEALTH"
            };

            lblHealthValue = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = PrimaryFixed,
                Text = "Operational Optimal"
            };

            healthLine1 = new Panel { BackColor = PrimaryFixed, Height = 4, Width = 52 };
            healthLine2 = new Panel { BackColor = PrimaryFixed, Height = 4, Width = 52 };
            healthLine3 = new Panel { BackColor = Color.FromArgb(70, 109, 250, 210), Height = 4, Width = 52 };

            cardSystemHealth.Controls.Add(lblHealthTitle);
            cardSystemHealth.Controls.Add(lblHealthValue);
            cardSystemHealth.Controls.Add(healthLine1);
            cardSystemHealth.Controls.Add(healthLine2);
            cardSystemHealth.Controls.Add(healthLine3);

            statsPanel.Controls.Add(cardActiveAccounts);
            statsPanel.Controls.Add(cardSystemHealth);
        }

        private void BuildTable()
        {
            tableShell = new Panel
            {
                BackColor = SurfaceLow,
                BorderStyle = BorderStyle.None
            };

            dgvClients = new DataGridView
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
                GridColor = OutlineVariant,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvClients.RowTemplate.Height = 72;
            dgvClients.DefaultCellStyle.Padding = new Padding(10, 10, 10, 10);

            dgvClients.ColumnHeadersDefaultCellStyle.BackColor = SurfaceContainer;
            dgvClients.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvClients.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvClients.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceContainer;
            dgvClients.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;

            dgvClients.DefaultCellStyle.BackColor = Surface;
            dgvClients.DefaultCellStyle.ForeColor = OnSurface;
            dgvClients.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvClients.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 249, 248);
            dgvClients.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvClients.Columns.Add("ClientId", "CLIENT ID");
            dgvClients.Columns.Add("SchoolName", "SCHOOL NAME");
            dgvClients.Columns.Add("Location", "LOCATION");
            dgvClients.Columns.Add("PrincipalAdmin", "PRINCIPAL ADMIN");
            dgvClients.Columns.Add("ContactEmail", "CONTACT EMAIL");
            dgvClients.Columns.Add("TotalUsers", "TOTAL USERS");
            dgvClients.Columns.Add("JoinedDate", "JOINED DATE");
            dgvClients.Columns.Add("Status", "STATUS");

            dgvClients.Columns["ClientId"].FillWeight = 14;
            dgvClients.Columns["SchoolName"].FillWeight = 22;
            dgvClients.Columns["Location"].FillWeight = 14;
            dgvClients.Columns["PrincipalAdmin"].FillWeight = 16;
            dgvClients.Columns["ContactEmail"].FillWeight = 18;
            dgvClients.Columns["TotalUsers"].FillWeight = 10;
            dgvClients.Columns["JoinedDate"].FillWeight = 12;
            dgvClients.Columns["Status"].FillWeight = 10;

            dgvClients.Columns["TotalUsers"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvClients.Columns["JoinedDate"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvClients.Columns["Status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            foreach (DataGridViewColumn col in dgvClients.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvClients.CellPainting += dgvClients_CellPainting;
            dgvClients.CellDoubleClick += dgvClients_CellDoubleClick;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                BackColor = Background
            };

            lblFooter = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                Text = "Showing 1-8 of 24 Clients"
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

            tableShell.Controls.Add(dgvClients);
            tableShell.Controls.Add(footerPanel);
        }

        private Button CreatePagerButton(string text, bool active)
        {
            Button btn = new Button
            {
                Text = text,
                Width = 40,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
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
                btn.BackColor = Surface;
                btn.ForeColor = OnSurfaceVariant;
                btn.FlatAppearance.BorderColor = Outline;
                btn.FlatAppearance.BorderSize = 1;
            }

            return btn;
        }

        private void SeedData()
        {
            allClients = new List<ClientDirectoryItem>
            {
                new ClientDirectoryItem("LF-UP-2022", "University of the Philippines", "Quezon City", "Dr. Maria Santos", "m.santos@up.edu.ph", 12450, new DateTime(2022, 1, 12), "Active"),
                new ClientDirectoryItem("LF-ADMU-2023", "Ateneo de Manila University", "Quezon City", "Fr. Roberto Tan", "rtan@ateneo.edu", 8200, new DateTime(2023, 3, 5), "Active"),
                new ClientDirectoryItem("LF-UST-2022", "UST Manila", "Manila", "Angelico Rivera", "arivera@ust.edu.ph", 15600, new DateTime(2022, 8, 20), "Active"),
                new ClientDirectoryItem("LF-USEP-2024", "USeP Davao", "Davao City", "Luzviminda Cruz", "l.cruz@usep.edu.ph", 5400, new DateTime(2024, 2, 14), "Active"),
                new ClientDirectoryItem("LF-USC-2023", "University of San Carlos", "Cebu City", "Benigno Garcia", "bgarcia@usc.edu.ph", 9800, new DateTime(2023, 11, 30), "Active"),
                new ClientDirectoryItem("LF-XU-2022", "Xavier University", "Cagayan de Oro", "Sarah Lim", "slim@xu.edu.ph", 4100, new DateTime(2022, 5, 18), "Suspended"),
                new ClientDirectoryItem("LF-DLSU-2023", "De La Salle University", "Manila", "Br. Raymundo Suplido", "ray.suplido@dlsu.edu.ph", 11200, new DateTime(2023, 10, 10), "Active"),
                new ClientDirectoryItem("LF-CLSU-2024", "Central Luzon State Univ.", "Muñoz, Nueva Ecija", "Dr. Jose Abad", "j.abad@clsu.edu.ph", 7300, new DateTime(2024, 4, 2), "Active")
            };

            lblActiveValue.Text = allClients.Count(c => c.Status == "Active").ToString("D2");
        }

        private void LoadSchoolFilter()
        {
            cboSchoolFilter.Items.Clear();
            cboSchoolFilter.Items.Add("All Schools");

            foreach (string school in allClients.Select(x => x.SchoolName).Distinct().OrderBy(x => x))
                cboSchoolFilter.Items.Add(school);

            cboSchoolFilter.SelectedIndex = 0;
        }

        private void LoadClientsGrid()
        {
            dgvClients.Rows.Clear();

            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string searchText = txtSearch.Text.Trim();

            bool useSearch = !string.IsNullOrWhiteSpace(searchText) && searchText != "Search by name or email...";

            IEnumerable<ClientDirectoryItem> filtered = allClients;

            if (selectedSchool != "All Schools")
                filtered = filtered.Where(x => x.SchoolName == selectedSchool);

            if (useSearch)
            {
                filtered = filtered.Where(x =>
                    x.SchoolName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.ContactEmail.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.PrincipalAdmin.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            List<ClientDirectoryItem> results = filtered.ToList();

            foreach (var client in results)
            {
                dgvClients.Rows.Add(
                    client.ClientId,
                    client.SchoolName,
                    client.Location,
                    client.PrincipalAdmin,
                    client.ContactEmail,
                    client.TotalUsers.ToString("N0"),
                    client.JoinedDate.ToString("MMM dd, yyyy"),
                    client.Status
                );
            }

            dgvClients.ClearSelection();
            lblFooter.Text = $"Showing 1-{results.Count} of {results.Count} Clients";
        }

        private void dgvClients_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvClients.Columns[e.ColumnIndex].Name;

            if (col == "ClientId")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(
                    e.Graphics,
                    e.FormattedValue?.ToString() ?? "",
                    new Font("Consolas", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16),
                    Primary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                e.Handled = true;
            }
            else if (col == "SchoolName")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(
                    e.Graphics,
                    e.FormattedValue?.ToString() ?? "",
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                e.Handled = true;
            }
            else if (col == "TotalUsers")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(
                    e.Graphics,
                    e.FormattedValue?.ToString() ?? "",
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X, e.CellBounds.Y + 10, e.CellBounds.Width, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                e.Handled = true;
            }
            else if (col == "JoinedDate")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(
                    e.Graphics,
                    e.FormattedValue?.ToString() ?? "",
                    new Font("Segoe UI", 10F),
                    new Rectangle(e.CellBounds.X, e.CellBounds.Y + 10, e.CellBounds.Width, e.CellBounds.Height - 16),
                    OnSurfaceVariant,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Color back = text == "Suspended" ? TertiaryContainer : SecondaryContainer;
                Color fore = text == "Suspended" ? OnTertiaryContainer : OnSecondaryContainer;

                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                int badgeWidth = textSize.Width + 22;
                int badgeHeight = 28;

                Rectangle badge = new Rectangle(
                    e.CellBounds.Right - badgeWidth - 16,
                    e.CellBounds.Y + (e.CellBounds.Height - badgeHeight) / 2,
                    badgeWidth,
                    badgeHeight);

                using (SolidBrush brush = new SolidBrush(back))
                    e.Graphics.FillEllipse(brush, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text.ToUpper(),
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    badge,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void dgvClients_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string clientId = dgvClients.Rows[e.RowIndex].Cells["ClientId"].Value?.ToString() ?? "";
            string schoolName = dgvClients.Rows[e.RowIndex].Cells["SchoolName"].Value?.ToString() ?? "";

            MessageBox.Show(
                $"Next step: open all transactions for\n\n{schoolName}\n{clientId}\n\nThen filter borrowing, returns, fines, librarians, and members by this Client ID.",
                "Client Drill-Down",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ClientViewForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int width = Math.Max(1040, canvas.ClientSize.Width - (margin * 2));

            headerPanel.Height = 160;
            lblTitle.Location = new Point(margin, 20);
            lblSubTitle.Location = new Point(margin + 4, 74);

            btnFilter.Location = new Point(ClientSize.Width - 72, 28);

            searchHost.Location = new Point(btnFilter.Left - 290, 28);
            searchHost.Size = new Size(280, 38);

            filterHost.Location = new Point(searchHost.Left - 230, 28);
            filterHost.Size = new Size(220, 38);

            cboSchoolFilter.Location = new Point(10, 6);
            cboSchoolFilter.Width = filterHost.Width - 20;

            lblSearchIcon.Location = new Point(10, 8);
            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = 220;

            statsPanel.Height = 220;

            int gap = 24;
            int leftWidth = (int)(width * 0.30);
            int rightWidth = width - leftWidth - gap;

            cardActiveAccounts.Bounds = new Rectangle(margin, 20, leftWidth, 150);
            cardSystemHealth.Bounds = new Rectangle(cardActiveAccounts.Right + gap, 20, rightWidth, 150);

            lblActiveTitle.Location = new Point(34, 34);
            lblActiveValue.Location = new Point(34, 72);
            lblActiveTrend.Location = new Point(cardActiveAccounts.Width - lblActiveTrend.Width - 34, 104);

            lblHealthTitle.Location = new Point(34, 34);
            lblHealthValue.Location = new Point(34, 62);

            healthLine1.Location = new Point(34, 118);
            healthLine2.Location = new Point(92, 118);
            healthLine3.Location = new Point(150, 118);

            tableShell.Bounds = new Rectangle(margin, headerPanel.Bottom + statsPanel.Height - 10, width, 560);

            lblFooter.Location = new Point(12, 22);

            btnNext.Location = new Point(footerPanel.Width - 52, 16);
            btnPage3.Location = new Point(btnNext.Left - 46, 16);
            btnPage2.Location = new Point(btnPage3.Left - 46, 16);
            btnPage1.Location = new Point(btnPage2.Left - 46, 16);
            btnPrev.Location = new Point(btnPage1.Left - 46, 16);

            canvas.AutoScrollMinSize = new Size(0, tableShell.Bottom + 40);
        }
    }

    public class ClientDirectoryItem
    {
        public string ClientId { get; set; }
        public string SchoolName { get; set; }
        public string Location { get; set; }
        public string PrincipalAdmin { get; set; }
        public string ContactEmail { get; set; }
        public int TotalUsers { get; set; }
        public DateTime JoinedDate { get; set; }
        public string Status { get; set; }

        public ClientDirectoryItem(
            string clientId,
            string schoolName,
            string location,
            string principalAdmin,
            string contactEmail,
            int totalUsers,
            DateTime joinedDate,
            string status)
        {
            ClientId = clientId;
            SchoolName = schoolName;
            Location = location;
            PrincipalAdmin = principalAdmin;
            ContactEmail = contactEmail;
            TotalUsers = totalUsers;
            JoinedDate = joinedDate;
            Status = status;
        }
    }
}