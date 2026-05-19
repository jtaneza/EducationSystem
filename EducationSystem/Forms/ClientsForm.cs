using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class ClientsForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color SurfaceLowest = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceVariant = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#6B7E95");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");

        private Panel headerPanel = null!;
        private Label lblBreadcrumb = null!;
        private Label lblTitle = null!;
        private Button btnAddSchool = null!;

        private Panel tableCard = null!;
        private Panel toolbarPanel = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;
        private Button btnExport = null!;

        private DataGridView dgvClients = null!;
        private Panel footerPanel = null!;
        private Label lblFooterInfo = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNext = null!;

        private const int PageSize = 3;
        private int currentPage = 1;
        private int totalPages = 1;
        private List<ClientDbItem> currentClients = new List<ClientDbItem>();
        private readonly Dictionary<int, string> clientSubscriptionPlans = new Dictionary<int, string>();

        public ClientsForm()
        {
            InitializeComponent();
            BuildClientsUI();
            LoadClientsToGrid();
        }

        private void BuildClientsUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = false;

            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Background
            };

            lblBreadcrumb = new Label
            {
                Text = "Admin   ›   Client Libraries",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = SecondaryText,
                AutoSize = true,
                Location = new Point(34, 20)
            };

            lblTitle = new Label
            {
                Text = "Institutional Directory",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(34, 48)
            };

            btnAddSchool = new Button
            {
                Text = "＋ Add New School",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Size = new Size(240, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnAddSchool.FlatAppearance.BorderSize = 0;
            btnAddSchool.Click += btnAddClient_Click;

            headerPanel.Controls.Add(lblBreadcrumb);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(btnAddSchool);

            tableCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceLowest,
                Padding = new Padding(34, 0, 34, 34),
                BorderStyle = BorderStyle.FixedSingle
            };

            toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                BackColor = SurfaceLowest
            };

            searchHost = new Panel
            {
                BackColor = SurfaceLow,
                Size = new Size(480, 44)
            };

            lblSearchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 18F),
                ForeColor = SecondaryText,
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = SurfaceLow,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 11F),
                Text = "Search by school name, ID or email..."
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by school name, ID or email...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by school name, ID or email...";
                    txtSearch.ForeColor = SecondaryText;
                }
            };

            txtSearch.TextChanged += (s, e) => { currentPage = 1; LoadClientsToGrid(); };

            btnFilter = CreateToolbarIconButton("☰");
            btnExport = CreateToolbarIconButton("⇩");

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            toolbarPanel.Controls.Add(searchHost);
            toolbarPanel.Controls.Add(btnFilter);
            toolbarPanel.Controls.Add(btnExport);

            dgvClients = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = SurfaceLowest,
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
                ColumnHeadersHeight = 54,
                GridColor = SurfaceVariant,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvClients.RowTemplate.Height = 78;
            dgvClients.DefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
            dgvClients.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 10, 8, 10);

            dgvClients.ColumnHeadersDefaultCellStyle.BackColor = SurfaceLow;
            dgvClients.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvClients.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvClients.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceLow;
            dgvClients.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;

            dgvClients.DefaultCellStyle.BackColor = SurfaceLowest;
            dgvClients.DefaultCellStyle.ForeColor = OnSurface;
            dgvClients.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvClients.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 249, 248);
            dgvClients.DefaultCellStyle.SelectionForeColor = OnSurface;
            dgvClients.AlternatingRowsDefaultCellStyle.BackColor = SurfaceLowest;

            dgvClients.Columns.Add("DbClientID", "DBID");
            dgvClients.Columns["DbClientID"].Visible = false;

            dgvClients.Columns.Add("SchoolName", "SCHOOL NAME");
            dgvClients.Columns.Add("Email", "EMAIL");
            dgvClients.Columns.Add("SubscriptionPlan", "SUBSCRIPTION PLAN");
            dgvClients.Columns.Add("UserCount", "USER COUNT");
            dgvClients.Columns.Add("Status", "STATUS");
            dgvClients.Columns.Add("Actions", "ACTIONS");

            dgvClients.Columns["SchoolName"].FillWeight = 30;
            dgvClients.Columns["Email"].FillWeight = 22;
            dgvClients.Columns["SubscriptionPlan"].FillWeight = 18;
            dgvClients.Columns["UserCount"].FillWeight = 11;
            dgvClients.Columns["Status"].FillWeight = 11;
            dgvClients.Columns["Actions"].FillWeight = 8;

            foreach (DataGridViewColumn col in dgvClients.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvClients.CellPainting += dgvClients_CellPainting;
            dgvClients.CellClick += dgvClients_CellClick;
            dgvClients.CellMouseEnter += dgvClients_CellMouseEnter;
            dgvClients.CellMouseLeave += dgvClients_CellMouseLeave;
            dgvClients.CellToolTipTextNeeded += dgvClients_CellToolTipTextNeeded;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 78,
                BackColor = Color.FromArgb(250, 252, 252)
            };

            lblFooterInfo = new Label
            {
                Text = "Showing 0 institutions",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = SecondaryText,
                AutoSize = true
            };

            btnPrev = CreatePagerButton("‹", false);
            btnPage1 = CreatePagerButton("1", true);
            btnPage2 = CreatePagerButton("2", false);
            btnPage3 = CreatePagerButton("3", false);
            btnNext = CreatePagerButton("›", false);

            footerPanel.Controls.Add(lblFooterInfo);
            footerPanel.Controls.Add(btnPrev);
            footerPanel.Controls.Add(btnPage1);
            footerPanel.Controls.Add(btnPage2);
            footerPanel.Controls.Add(btnPage3);
            footerPanel.Controls.Add(btnNext);

            btnPrev.Click += (s, e) => ChangePage(currentPage - 1);
            btnPage1.Click += (s, e) => ChangePage(1);
            btnPage2.Click += (s, e) => ChangePage(2);
            btnPage3.Click += (s, e) => ChangePage(3);
            btnNext.Click += (s, e) => ChangePage(currentPage + 1);

            tableCard.Controls.Add(dgvClients);
            tableCard.Controls.Add(footerPanel);
            tableCard.Controls.Add(toolbarPanel);

            Controls.Add(tableCard);
            Controls.Add(headerPanel);

            Resize += ClientsForm_Resize;
            AdjustResponsiveLayout();
        }

        private Button CreateToolbarIconButton(string text)
        {
            Button btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                Width = 40,
                Height = 40,
                BackColor = Color.Transparent,
                ForeColor = OnSurface,
                Font = new Font("Segoe UI Symbol", 14F, FontStyle.Regular),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = SurfaceLow;
            return btn;
        }

        private Button CreatePagerButton(string text, bool active)
        {
            Button btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                Width = 40,
                Height = 40,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
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
                btn.FlatAppearance.BorderColor = SurfaceVariant;
                btn.FlatAppearance.BorderSize = 1;
            }

            return btn;
        }

        private void AdjustResponsiveLayout()
        {
            if (btnAddSchool == null) return;

            btnAddSchool.Location = new Point(ClientSize.Width - btnAddSchool.Width - 40, 54);

            tableCard.Padding = new Padding(34, 0, 34, 24);

            searchHost.Location = new Point(26, 22);
            searchHost.Size = new Size(Math.Min(480, Math.Max(300, ClientSize.Width / 3)), 44);

            lblSearchIcon.Location = new Point(12, 8);
            txtSearch.Location = new Point(46, 12);
            txtSearch.Width = searchHost.Width - 58;

            btnExport.Location = new Point(tableCard.Width - 84, 24);
            btnFilter.Location = new Point(btnExport.Left - 48, 24);

            int pagerY = 10;
            int footerTextY = 22;
            lblFooterInfo.Location = new Point(26, footerTextY);

            btnNext.Location = new Point(footerPanel.Width - 58, pagerY);
            btnPage3.Location = new Point(btnNext.Left - 48, pagerY);
            btnPage2.Location = new Point(btnPage3.Left - 48, pagerY);
            btnPage1.Location = new Point(btnPage2.Left - 48, pagerY);
            btnPrev.Location = new Point(btnPage1.Left - 48, pagerY);

            footerPanel.Padding = new Padding(0, 0, 0, 12);
        }

        private void ClientsForm_Resize(object? sender, EventArgs e)
        {
            AdjustResponsiveLayout();
        }

        private void LoadClientsToGrid()
        {
            EnsureClientSubscriptionSchema();
            LoadClientSubscriptionPlans();

            string searchText = "";
            if (txtSearch != null &&
                !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                txtSearch.Text != "Search by school name, ID or email...")
            {
                searchText = txtSearch.Text.Trim();
            }

            currentClients = ClientService.GetClients(searchText);
            totalPages = Math.Max(1, (int)Math.Ceiling(currentClients.Count / (double)PageSize));

            if (currentPage > totalPages)
                currentPage = totalPages;

            if (currentPage < 1)
                currentPage = 1;

            RenderCurrentPage();
        }

        private void ChangePage(int page)
        {
            if (page < 1 || page > totalPages)
                return;

            currentPage = page;
            RenderCurrentPage();
        }

        private void RenderCurrentPage()
        {
            dgvClients.Rows.Clear();

            List<ClientDbItem> pageItems = currentClients
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (ClientDbItem client in pageItems)
            {
                string schoolCell = GetInitials(client.LibraryName) + "|" + client.LibraryName + "|" + client.LibraryCode;

                dgvClients.Rows.Add(
                    client.DbClientID,
                    schoolCell,
                    client.Email,
                    GetCachedSubscriptionPlan(client.DbClientID),
                    client.UserCount.ToString("N0"),
                    NormalizeClientStatus(client.Status),
                    ""
                );
            }

            dgvClients.ClearSelection();

            int total = currentClients.Count;
            int start = total == 0 ? 0 : ((currentPage - 1) * PageSize) + 1;
            int end = Math.Min(currentPage * PageSize, total);

            lblFooterInfo.Text = total == 0
                ? "Showing 0 institutions"
                : $"Showing {start} - {end} of {total} institutions";

            UpdatePagerButtons();
        }

        private void UpdatePagerButtons()
        {
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;

            Button[] pageButtons = { btnPage1, btnPage2, btnPage3 };

            for (int i = 0; i < pageButtons.Length; i++)
            {
                int pageNumber = i + 1;
                Button btn = pageButtons[i];

                btn.Text = pageNumber.ToString();
                btn.Visible = pageNumber <= totalPages;

                bool active = currentPage == pageNumber;
                btn.BackColor = active ? AccentEmerald : Color.Transparent;
                btn.ForeColor = active ? Color.White : OnSurface;
                btn.FlatAppearance.BorderSize = active ? 0 : 1;
                btn.FlatAppearance.BorderColor = SurfaceVariant;
            }

            btnPrev.BackColor = Color.Transparent;
            btnNext.BackColor = Color.Transparent;
            btnPrev.ForeColor = btnPrev.Enabled ? OnSurface : SecondaryText;
            btnNext.ForeColor = btnNext.Enabled ? OnSurface : SecondaryText;

            AdjustResponsiveLayout();
        }


        private void EnsureClientSubscriptionSchema()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"
IF COL_LENGTH('dbo.ClientLibraries', 'SubscriptionPlan') IS NULL
BEGIN
    ALTER TABLE dbo.ClientLibraries
    ADD SubscriptionPlan NVARCHAR(50) NOT NULL
        CONSTRAINT DF_ClientLibraries_SubscriptionPlan DEFAULT 'One-Time Payment';
END;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // The grid can still load even if the column cannot be created.
            }
        }

        private void LoadClientSubscriptionPlans()
        {
            clientSubscriptionPlans.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureClientSubscriptionSchema();

                string query = @"
SELECT ClientID, ISNULL(NULLIF(SubscriptionPlan, ''), 'One-Time Payment') AS SubscriptionPlan
FROM dbo.ClientLibraries;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int clientId = Convert.ToInt32(reader["ClientID"]);
                    string plan = Convert.ToString(reader["SubscriptionPlan"]) ?? "One-Time Payment";
                    clientSubscriptionPlans[clientId] = NormalizeSubscriptionPlan(plan);
                }
            }
            catch
            {
                // Keep defaults if database read fails.
            }
        }

        private string GetCachedSubscriptionPlan(int dbClientId)
        {
            if (clientSubscriptionPlans.TryGetValue(dbClientId, out string? plan))
                return NormalizeSubscriptionPlan(plan);

            return "One-Time Payment";
        }

        private string NormalizeSubscriptionPlan(string? plan)
        {
            if (string.Equals(plan, "Yearly Subscription", StringComparison.OrdinalIgnoreCase))
                return "Yearly Subscription";

            return "One-Time Payment";
        }

        private void UpdateClientSubscriptionPlan(int dbClientId, string subscriptionPlan)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureClientSubscriptionSchema();

                string query = @"
UPDATE dbo.ClientLibraries
SET SubscriptionPlan = @SubscriptionPlan
WHERE ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SubscriptionPlan", NormalizeSubscriptionPlan(subscriptionPlan));
                cmd.Parameters.AddWithValue("@ClientID", dbClientId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Subscription plan could not be saved.\n\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateClientSubscriptionPlanByLibraryCode(string libraryCode, string subscriptionPlan)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureClientSubscriptionSchema();

                string query = @"
UPDATE dbo.ClientLibraries
SET SubscriptionPlan = @SubscriptionPlan
WHERE LibraryCode = @LibraryCode;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SubscriptionPlan", NormalizeSubscriptionPlan(subscriptionPlan));
                cmd.Parameters.AddWithValue("@LibraryCode", libraryCode);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Subscription plan could not be saved.\n\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string NormalizeClientStatus(string? status)
        {
            if (string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Disabled", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "0", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "False", StringComparison.OrdinalIgnoreCase))
            {
                return "Inactive";
            }

            return "Active";
        }

        private string GetInitials(string libraryName)
        {
            string[] words = libraryName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return "CL";
            if (words.Length == 1) return words[0].Substring(0, Math.Min(2, words[0].Length)).ToUpper();
            return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
        }

        private void btnAddClient_Click(object? sender, EventArgs e)
        {
            string newId = ClientService.GenerateNextLibraryCode();

            using ClientDialogForm dialog = new ClientDialogForm("Register New Client", newId);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ClientService.AddClientWithAdmin(
                        dialog.ClientIDValue,
                        dialog.LibraryNameValue,
                        dialog.EmailValue,
                        dialog.PasswordValue,
                        dialog.StatusValue
                    );

                    UpdateClientSubscriptionPlanByLibraryCode(dialog.ClientIDValue, dialog.SubscriptionPlanValue);

                    MessageBox.Show("Client saved successfully.");
                    LoadClientsToGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving client:\n\n" + ex.Message);
                }
            }
        }

        private void EditClient(int dbClientId, string libraryCode)
        {
            ClientDbItem? client = ClientService.GetClients()
                .FirstOrDefault(c => c.DbClientID == dbClientId);

            if (client == null) return;

            ClientItem editItem = new ClientItem
            {
                ClientID = libraryCode,
                LibraryName = client.LibraryName,
                Email = client.Email,
                Password = client.PasswordText,
                Status = client.Status
            };

            using ClientDialogForm dialog = new ClientDialogForm("Edit Client", libraryCode, editItem, GetCachedSubscriptionPlan(dbClientId));

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ClientService.UpdateClientWithAdmin(
                        dbClientId,
                        dialog.LibraryNameValue,
                        dialog.EmailValue,
                        dialog.PasswordValue,
                        dialog.StatusValue
                    );

                    UpdateClientSubscriptionPlan(dbClientId, dialog.SubscriptionPlanValue);

                    MessageBox.Show("Client updated successfully.");
                    LoadClientsToGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating client:\n\n" + ex.Message);
                }
            }
        }

        private void ToggleClientStatus(int dbClientId, string libraryName)
        {
            ClientDbItem? client = ClientService.GetClients()
                .FirstOrDefault(c => c.DbClientID == dbClientId);

            if (client == null) return;

            string currentStatus = NormalizeClientStatus(client.Status);
            string newStatus = currentStatus.Equals("Active", StringComparison.OrdinalIgnoreCase)
                ? "Inactive"
                : "Active";

            DialogResult result = MessageBox.Show(
                $"Set '{libraryName}' status to {newStatus}?",
                "Update Client Status",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            try
            {
                UpdateClientStatusOnly(dbClientId, newStatus);

                if (dgvClients.CurrentCell != null && dgvClients.Columns[dgvClients.CurrentCell.ColumnIndex].Name == "Status")
                {
                    dgvClients.Rows[dgvClients.CurrentCell.RowIndex].Cells["Status"].Value = newStatus;
                    dgvClients.InvalidateCell(dgvClients.CurrentCell);
                }

                MessageBox.Show("Client status updated successfully.");
                LoadClientsToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating client status:\n\n" + ex.Message);
            }
        }



        private void UpdateClientStatusOnly(int dbClientId, string newStatus)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();

            string ensureQuery = @"
IF COL_LENGTH('dbo.ClientLibraries', 'Status') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD Status NVARCHAR(50) NOT NULL CONSTRAINT DF_ClientLibraries_Status DEFAULT 'Active';

IF COL_LENGTH('dbo.ClientLibraries', 'IsActive') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD IsActive BIT NOT NULL CONSTRAINT DF_ClientLibraries_IsActive DEFAULT 1;";

            using (SqlCommand ensureCmd = new SqlCommand(ensureQuery, conn))
                ensureCmd.ExecuteNonQuery();

            bool isActive = newStatus.Equals("Active", StringComparison.OrdinalIgnoreCase);

            string updateQuery = @"
UPDATE dbo.ClientLibraries
SET
    Status = @Status,
    IsActive = @IsActive
WHERE ClientID = @ClientID;

UPDATE dbo.Users
SET
    Status = @Status,
    IsActive = @IsActive
WHERE ClientID = @ClientID;";

            using SqlCommand cmd = new SqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@Status", newStatus);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.Parameters.AddWithValue("@ClientID", dbClientId);
            cmd.ExecuteNonQuery();
        }

        private void ArchiveClient(int dbClientId, string libraryName)
        {
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to archive '{libraryName}'?",
                "Archive Client",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    ClientService.ArchiveClient(
                        dbClientId,
                        string.IsNullOrWhiteSpace(UserSession.Username) ? "Super Admin" : UserSession.Username
                    );

                    MessageBox.Show("Client archived successfully.");
                    LoadClientsToGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error archiving client:\n\n" + ex.Message);
                }
            }
        }

        private void dgvClients_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string columnName = dgvClients.Columns[e.ColumnIndex].Name;

            int dbClientId = Convert.ToInt32(dgvClients.Rows[e.RowIndex].Cells["DbClientID"].Value);
            string schoolCell = dgvClients.Rows[e.RowIndex].Cells["SchoolName"].Value?.ToString() ?? "";
            string[] parts = schoolCell.Split('|');
            if (parts.Length < 3) return;

            string libraryName = parts[1];
            string libraryCode = parts[2];

            if (columnName == "Status")
            {
                ToggleClientStatus(dbClientId, libraryName);
                return;
            }

            if (columnName != "Actions") return;

            Rectangle cellRect = dgvClients.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            Point mouse = dgvClients.PointToClient(Cursor.Position);
            int relativeX = mouse.X - cellRect.X;

            // Edit icon area.
            if (relativeX >= 8 && relativeX <= 42)
            {
                EditClient(dbClientId, libraryCode);
                return;
            }

            // Archive icon area.
            if (relativeX >= 48 && relativeX <= 86)
            {
                ArchiveClient(dbClientId, libraryName);
                return;
            }
        }

        private void dgvClients_CellMouseEnter(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string columnName = dgvClients.Columns[e.ColumnIndex].Name;
            if (columnName == "Actions" || columnName == "Status")
                dgvClients.Cursor = Cursors.Hand;
        }

        private void dgvClients_CellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            dgvClients.Cursor = Cursors.Default;
        }

        private void dgvClients_CellToolTipTextNeeded(object? sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "";
        }

        private void dgvClients_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvClients.Columns[e.ColumnIndex].Name;

            if (columnName == "SchoolName")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string initials = parts.Length > 0 ? parts[0] : "";
                string schoolName = parts.Length > 1 ? parts[1] : "";
                string clientId = parts.Length > 2 ? parts[2] : "";

                Rectangle avatar = new Rectangle(e.CellBounds.X + 14, e.CellBounds.Y + (e.CellBounds.Height - 40) / 2, 40, 40);

                using (SolidBrush brush = new SolidBrush(SurfaceLow))
                    e.Graphics.FillRectangle(brush, avatar);

                TextRenderer.DrawText(
                    e.Graphics,
                    initials,
                    new Font("Segoe UI", 11F, FontStyle.Bold),
                    avatar,
                    AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                Rectangle nameRect = new Rectangle(avatar.Right + 12, e.CellBounds.Y + 14, e.CellBounds.Width - avatar.Width - 26, 24);
                Rectangle idRect = new Rectangle(avatar.Right + 12, e.CellBounds.Y + 38, e.CellBounds.Width - avatar.Width - 26, 18);

                TextRenderer.DrawText(
                    e.Graphics,
                    schoolName,
                    new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    nameRect,
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                TextRenderer.DrawText(
                    e.Graphics,
                    "ID: " + clientId,
                    new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    idRect,
                    SecondaryText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (columnName == "SubscriptionPlan")
            {
                e.PaintBackground(e.CellBounds, true);

                string plan = e.FormattedValue?.ToString() ?? "One-Time Payment";
                bool yearly = plan.Equals("Yearly Subscription", StringComparison.OrdinalIgnoreCase);

                int badgeWidth = yearly ? 145 : 130;
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 12,
                    e.CellBounds.Y + (e.CellBounds.Height - 28) / 2,
                    Math.Min(badgeWidth, e.CellBounds.Width - 24),
                    28
                );

                using (SolidBrush brush = new SolidBrush(yearly ? Color.FromArgb(224, 245, 240) : Color.FromArgb(232, 239, 241)))
                    e.Graphics.FillRectangle(brush, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    plan,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    badge,
                    yearly ? AccentDeep : OnSurface,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                );

                e.Handled = true;
            }
            else if (columnName == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                bool isActive = NormalizeClientStatus(text).Equals("Active", StringComparison.OrdinalIgnoreCase);

                Rectangle track = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + (e.CellBounds.Height - 24) / 2, 40, 22);
                Rectangle knob = isActive
                    ? new Rectangle(track.Right - 20, track.Y + 1, 20, 20)
                    : new Rectangle(track.X, track.Y + 1, 20, 20);

                using (SolidBrush trackBrush = new SolidBrush(isActive ? AccentEmerald : Color.FromArgb(220, 226, 230)))
                    e.Graphics.FillRectangle(trackBrush, track);

                using (SolidBrush knobBrush = new SolidBrush(Color.White))
                    e.Graphics.FillEllipse(knobBrush, knob);

                TextRenderer.DrawText(
                    e.Graphics,
                    isActive ? "Active" : "Inactive",
                    new Font("Segoe UI", 9F, FontStyle.Bold),
                    new Rectangle(track.Right + 10, e.CellBounds.Y, e.CellBounds.Width - track.Width - 18, e.CellBounds.Height),
                    isActive ? AccentDeep : OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (columnName == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle editRect = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + (e.CellBounds.Height - 24) / 2, 24, 24);
                Rectangle delRect = new Rectangle(e.CellBounds.X + 52, e.CellBounds.Y + (e.CellBounds.Height - 24) / 2, 24, 24);

                TextRenderer.DrawText(
                    e.Graphics,
                    "✎",
                    new Font("Segoe UI Symbol", 12F, FontStyle.Regular),
                    editRect,
                    SecondaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                TextRenderer.DrawText(
                    e.Graphics,
                    "📥",
                    new Font("Segoe UI Emoji", 11F, FontStyle.Regular),
                    delRect,
                    SecondaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
        }
    }
}