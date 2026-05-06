using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

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
        private Panel filterRow = null!;

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
            LoadClientsFromDatabase();
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
            canvas.Controls.Add(filterRow);
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
                Height = 140,
                Dock = DockStyle.Top,
                BackColor = Background
            };

            lblTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 25F, FontStyle.Bold),
                ForeColor = OnSurface,
                Text = "Client Directory"
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

            filterRow = new Panel
            {
                BackColor = Background,
                Height = 64
            };
            filterRow.Controls.Add(filterHost);
            filterRow.Controls.Add(searchHost);
            filterRow.Controls.Add(btnFilter);
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
            dgvClients.Columns.Add("ContactEmail", "EMAIL");
            dgvClients.Columns.Add("TotalUsers", "TOTAL USERS");
            dgvClients.Columns.Add("JoinedDate", "JOINED DATE");
            dgvClients.Columns.Add("Status", "STATUS");

            dgvClients.Columns["ClientId"].FillWeight = 14;
            dgvClients.Columns["SchoolName"].FillWeight = 30;
            dgvClients.Columns["ContactEmail"].FillWeight = 26;
            dgvClients.Columns["TotalUsers"].FillWeight = 12;
            dgvClients.Columns["JoinedDate"].FillWeight = 14;
            dgvClients.Columns["Status"].FillWeight = 12;

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

            tableShell.Controls.Add(footerPanel);
            tableShell.Controls.Add(dgvClients);
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

        private void LoadClientsFromDatabase()
        {
            allClients.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureClientViewSchema(conn);

                const string query = @"
SELECT
    cl.ClientID,
    cl.LibraryCode,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unnamed School') AS SchoolName,
    ISNULL(NULLIF(cl.Email, ''), '') AS ContactEmail,
    cl.CreatedAt,
    ISNULL(NULLIF(cl.Status, ''), 'Active') AS Status,
    (
        SELECT COUNT(*)
        FROM dbo.Users u
        WHERE u.ClientID = cl.ClientID
    ) AS TotalUsers
FROM dbo.ClientLibraries cl
ORDER BY cl.LibraryName ASC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int clientIdValue = Convert.ToInt32(reader["ClientID"]);
                    string libraryCode = Convert.ToString(reader["LibraryCode"]) ?? "";
                    string clientCode = string.IsNullOrWhiteSpace(libraryCode)
                        ? "CL-" + clientIdValue.ToString("0000")
                        : libraryCode;

                    allClients.Add(new ClientDirectoryItem(
                        clientCode,
                        Convert.ToString(reader["SchoolName"]) ?? "Unnamed School",
                        "",
                        "",
                        Convert.ToString(reader["ContactEmail"]) ?? "",
                        reader["TotalUsers"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalUsers"]),
                        reader["CreatedAt"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["CreatedAt"]),
                        Convert.ToString(reader["Status"]) ?? "Active"
                    ));
                }

                lblActiveValue.Text = allClients.Count(c => c.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)).ToString("D2");
                lblActiveTrend.Text = $"{allClients.Count:N0} total client libraries";
                lblHealthValue.Text = allClients.Count == 0 ? "No Client Data" : "Operational";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Client directory could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                allClients.Clear();
                lblActiveValue.Text = "00";
                lblActiveTrend.Text = "No live data";
                lblHealthValue.Text = "Database Check";
            }
        }

        private void EnsureClientViewSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.ClientLibraries', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ClientLibraries
    (
        ClientID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        LibraryCode NVARCHAR(50) NULL,
        LibraryName NVARCHAR(200) NULL,
        Email NVARCHAR(150) NULL,
        PasswordText NVARCHAR(150) NULL,
        UserCount INT NOT NULL DEFAULT 0,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
        ImagePath NVARCHAR(250) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF COL_LENGTH('dbo.ClientLibraries', 'LibraryCode') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD LibraryCode NVARCHAR(50) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'LibraryName') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD LibraryName NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'Email') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD Email NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'PasswordText') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD PasswordText NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'UserCount') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD UserCount INT NOT NULL CONSTRAINT DF_ClientLibraries_UserCount_View DEFAULT 0;

IF COL_LENGTH('dbo.ClientLibraries', 'Status') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD Status NVARCHAR(50) NOT NULL CONSTRAINT DF_ClientLibraries_Status_View2 DEFAULT 'Active';

IF COL_LENGTH('dbo.ClientLibraries', 'ImagePath') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD ImagePath NVARCHAR(250) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'CreatedAt') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_ClientLibraries_CreatedAt_View2 DEFAULT SYSUTCDATETIME();

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        FullName NVARCHAR(150) NULL,
        Email NVARCHAR(150) NULL,
        PasswordText NVARCHAR(150) NULL,
        Role NVARCHAR(50) NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
    ALTER TABLE dbo.Users ADD ClientID INT NULL;

IF COL_LENGTH('dbo.Users', 'Status') IS NULL
    ALTER TABLE dbo.Users ADD Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Users_Status_ClientView DEFAULT 'Active';

IF COL_LENGTH('dbo.Users', 'CreatedAt') IS NULL
    ALTER TABLE dbo.Users ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt_ClientView DEFAULT SYSUTCDATETIME();

UPDATE dbo.ClientLibraries
SET LibraryName = CONCAT('Library ', ClientID)
WHERE LibraryName IS NULL OR LTRIM(RTRIM(LibraryName)) = '';";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void LoadSchoolFilter()
        {
            cboSchoolFilter.Items.Clear();
            cboSchoolFilter.Items.Add("All Schools");

            foreach (string school in allClients
                .Select(x => x.SchoolName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x))
            {
                cboSchoolFilter.Items.Add(school);
            }

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
                filtered = filtered.Where(x => x.SchoolName.Equals(selectedSchool, StringComparison.OrdinalIgnoreCase));

            if (useSearch)
            {
                filtered = filtered.Where(x =>
                    x.SchoolName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.ContactEmail.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.ClientId.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            List<ClientDirectoryItem> results = filtered.ToList();

            foreach (var client in results)
            {
                dgvClients.Rows.Add(
                client.ClientId,
                client.SchoolName,
                client.ContactEmail,
                client.TotalUsers.ToString("N0"),
                client.JoinedDate.ToString("MMM dd, yyyy"),
                client.Status
            );
            }

            dgvClients.ClearSelection();

            lblActiveValue.Text = results.Count(x => x.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)).ToString("D2");
            lblActiveTrend.Text = $"{results.Count:N0} total client libraries";

            lblFooter.Text = results.Count == 0
                ? "Showing 0 of 0 Clients"
                : $"Showing 1-{results.Count} of {results.Count} Clients";
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
            else if (col == "ContactEmail")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(
                    e.Graphics,
                    e.FormattedValue?.ToString() ?? "",
                    new Font("Segoe UI", 10F),
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
            int gap = 24;
            int clientWidth = canvas.ClientSize.Width;
            int width = Math.Max(1040, clientWidth - (margin * 2));

            int y = 0;

            headerPanel.Bounds = new Rectangle(0, y, clientWidth, 140);
            lblTitle.Location = new Point(margin, 28);
            lblSubTitle.Location = new Point(margin + 4, 78);
            y += headerPanel.Height;

            statsPanel.Bounds = new Rectangle(0, y, clientWidth, 190);

            int leftWidth = (int)(width * 0.30);
            int rightWidth = width - leftWidth - gap;

            cardActiveAccounts.Bounds = new Rectangle(margin, 18, leftWidth, 130);
            cardSystemHealth.Bounds = new Rectangle(cardActiveAccounts.Right + gap, 18, rightWidth, 130);

            lblActiveTitle.Location = new Point(34, 28);
            lblActiveValue.Location = new Point(34, 60);
            lblActiveTrend.Location = new Point(cardActiveAccounts.Width - lblActiveTrend.Width - 34, 88);

            lblHealthTitle.Location = new Point(34, 28);
            lblHealthValue.Location = new Point(34, 54);

            healthLine1.Location = new Point(34, 108);
            healthLine2.Location = new Point(92, 108);
            healthLine3.Location = new Point(150, 108);

            y += statsPanel.Height;

            filterRow.Bounds = new Rectangle(0, y, clientWidth, 64);

            btnFilter.Location = new Point(clientWidth - btnFilter.Width - margin, 12);

            searchHost.Location = new Point(btnFilter.Left - searchHost.Width - 10, 12);
            searchHost.Size = new Size(280, 38);

            filterHost.Location = new Point(searchHost.Left - filterHost.Width - 10, 12);
            filterHost.Size = new Size(220, 38);

            cboSchoolFilter.Location = new Point(10, 6);
            cboSchoolFilter.Width = filterHost.Width - 20;

            lblSearchIcon.Location = new Point(10, 8);
            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = searchHost.Width - 54;

            y += filterRow.Height;

            int tableHeight = 430;
            tableShell.Bounds = new Rectangle(margin, y, width, tableHeight);

            lblFooter.Location = new Point(12, 22);

            btnNext.Location = new Point(footerPanel.Width - 52, 16);
            btnPage3.Location = new Point(btnNext.Left - 46, 16);
            btnPage2.Location = new Point(btnPage3.Left - 46, 16);
            btnPage1.Location = new Point(btnPage2.Left - 46, 16);
            btnPrev.Location = new Point(btnPage1.Left - 46, 16);

            canvas.AutoScrollMinSize = new Size(0, tableShell.Bottom + 50);
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