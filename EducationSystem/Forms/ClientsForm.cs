using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ClientsForm : Form
    {
        private Label lblTitle = null!;
        private TextBox txtSearch = null!;
        private Button btnSearch = null!;
        private Button btnAddClient = null!;
        private ComboBox cboStatusFilter = null!;
        private Label lblFilter = null!;

        private Label lblTotalClients = null!;
        private Label lblActiveClients = null!;
        private Label lblInactiveClients = null!;

        private Panel topPanel = null!;
        private Panel summaryPanel = null!;
        private Panel tableContainer = null!;
        private DataGridView dgvClients = null!;
        private Label lblEmpty = null!;

        public ClientsForm()
        {
            InitializeComponent();
            BuildClientsUI();
            SeedSampleDataOnce();
            LoadClientsToGrid();
            UpdateSummaryLabels();
        }

        private void BuildClientsUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            topPanel = new Panel
            {
                BackColor = Color.Snow,
                Dock = DockStyle.Top,
                Height = 165
            };

            lblTitle = new Label
            {
                Text = "Client Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Maroon,
                AutoSize = true,
                Location = new Point(30, 15)
            };

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(250, 30),
                Location = new Point(30, 58),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            btnSearch = new Button
            {
                Text = "Search",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(90, 30),
                Location = new Point(290, 58)
            };
            StyleButton(btnSearch);
            btnSearch.Click += btnSearch_Click;

            lblFilter = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Maroon,
                AutoSize = true,
                Location = new Point(400, 63)
            };

            cboStatusFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(150, 30),
                Location = new Point(455, 58)
            };
            cboStatusFilter.Items.AddRange(new object[] { "All", "Active", "Inactive" });
            cboStatusFilter.SelectedIndex = 0;
            cboStatusFilter.SelectedIndexChanged += cboStatusFilter_SelectedIndexChanged;

            btnAddClient = new Button
            {
                Text = "+ ADD NEW CLIENT",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(150, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            StyleButton(btnAddClient);
            btnAddClient.Click += btnAddClient_Click;

            summaryPanel = new Panel
            {
                BackColor = Color.Snow,
                Location = new Point(30, 100),
                Size = new Size(500, 45)
            };

            lblTotalClients = CreateSummaryLabel("Total Clients: 0", new Point(0, 10));
            lblActiveClients = CreateSummaryLabel("Active: 0", new Point(170, 10));
            lblInactiveClients = CreateSummaryLabel("Inactive: 0", new Point(290, 10));

            summaryPanel.Controls.Add(lblTotalClients);
            summaryPanel.Controls.Add(lblActiveClients);
            summaryPanel.Controls.Add(lblInactiveClients);

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(lblFilter);
            topPanel.Controls.Add(cboStatusFilter);
            topPanel.Controls.Add(btnAddClient);
            topPanel.Controls.Add(summaryPanel);

            dgvClients = new DataGridView
            {
                Dock = DockStyle.Fill,
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
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 38 },
                GridColor = Color.Gainsboro,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                BackgroundColor = Color.WhiteSmoke
            };

            dgvClients.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvClients.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvClients.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvClients.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvClients.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
            dgvClients.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            dgvClients.DefaultCellStyle.BackColor = Color.White;
            dgvClients.DefaultCellStyle.ForeColor = Color.Black;
            dgvClients.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvClients.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvClients.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvClients.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            dgvClients.Columns.Add("ID", "Client ID");
            dgvClients.Columns.Add("Name", "Library Name");
            dgvClients.Columns.Add("Email", "Email");
            dgvClients.Columns.Add("Status", "Status");

            DataGridViewButtonColumn editCol = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Edit",
                Text = "✎",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 70
            };

            DataGridViewButtonColumn archiveCol = new DataGridViewButtonColumn
            {
                Name = "Archive",
                HeaderText = "Archive",
                Text = "📥",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 80
            };

            dgvClients.Columns.Add(editCol);
            dgvClients.Columns.Add(archiveCol);

            foreach (DataGridViewColumn col in dgvClients.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvClients.Columns["ID"].FillWeight = 18;
            dgvClients.Columns["Name"].FillWeight = 28;
            dgvClients.Columns["Email"].FillWeight = 26;
            dgvClients.Columns["Status"].FillWeight = 14;

            dgvClients.CellClick += dgvClients_CellClick;
            dgvClients.CellFormatting += dgvClients_CellFormatting;

            lblEmpty = new Label
            {
                Text = "No clients found.",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Visible = false
            };

            tableContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 0, 30, 20),
                BackColor = Color.Snow
            };

            tableContainer.Controls.Add(dgvClients);
            tableContainer.Controls.Add(lblEmpty);

            Controls.Add(tableContainer);
            Controls.Add(topPanel);

            Resize += ClientsForm_Resize;
            AdjustTopControls();
            PositionEmptyLabel();
        }

        private Label CreateSummaryLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Maroon,
                AutoSize = true,
                Location = location
            };
        }

        private void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(210, 210, 210);
            btn.UseVisualStyleBackColor = false;
            btn.BackColor = Color.Maroon;
            btn.ForeColor = Color.WhiteSmoke;
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(230, 230, 230);
                btn.ForeColor = Color.Black;
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.Maroon;
                btn.ForeColor = Color.WhiteSmoke;
            };
        }

        private void ClientsForm_Resize(object? sender, EventArgs e)
        {
            AdjustTopControls();
            PositionEmptyLabel();
        }

        private void AdjustTopControls()
        {
            if (btnAddClient == null || txtSearch == null || btnSearch == null)
                return;

            int rightMargin = 30;
            btnAddClient.Location = new Point(
                Math.Max(cboStatusFilter.Right + 40, ClientSize.Width - btnAddClient.Width - rightMargin),
                txtSearch.Top - 1
            );
        }

        private void PositionEmptyLabel()
        {
            if (lblEmpty == null || tableContainer == null) return;

            lblEmpty.Location = new Point(
                Math.Max(30, (tableContainer.Width - lblEmpty.Width) / 2),
                Math.Max(40, (tableContainer.Height - lblEmpty.Height) / 2)
            );
            lblEmpty.BringToFront();
        }

        private void SeedSampleDataOnce()
        {
            if (ClientArchiveStore.IsSeeded) return;

            ClientArchiveStore.ActiveClients.Add(new ClientItem
            {
                ClientID = "CL001",
                LibraryName = "ABC School Library",
                Email = "abc@gmail.com",
                Password = "abc123",
                Status = "Active"
            });

            ClientArchiveStore.ActiveClients.Add(new ClientItem
            {
                ClientID = "CL002",
                LibraryName = "XYZ College Library",
                Email = "xyz@gmail.com",
                Password = "xyz123",
                Status = "Active"
            });

            ClientArchiveStore.ActiveClients.Add(new ClientItem
            {
                ClientID = "CL003",
                LibraryName = "LMN Center",
                Email = "lmn@gmail.com",
                Password = "lmn123",
                Status = "Inactive"
            });

            ClientArchiveStore.IsSeeded = true;
        }

        private void LoadClientsToGrid()
        {
            string keyword = txtSearch?.Text.Trim().ToLower() ?? "";
            string statusFilter = cboStatusFilter?.SelectedItem?.ToString() ?? "All";

            dgvClients.Rows.Clear();

            var filtered = ClientArchiveStore.ActiveClients.Where(c =>
            {
                bool matchesKeyword =
                    string.IsNullOrWhiteSpace(keyword) ||
                    c.ClientID.ToLower().Contains(keyword) ||
                    c.LibraryName.ToLower().Contains(keyword) ||
                    c.Email.ToLower().Contains(keyword) ||
                    c.Status.ToLower().Contains(keyword);

                bool matchesStatus =
                    statusFilter == "All" ||
                    c.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase);

                return matchesKeyword && matchesStatus;
            }).ToList();

            foreach (var client in filtered)
            {
                dgvClients.Rows.Add(
                    client.ClientID,
                    client.LibraryName,
                    client.Email,
                    client.Status
                );
            }

            dgvClients.ClearSelection();
            lblEmpty.Visible = filtered.Count == 0;
            PositionEmptyLabel();
            UpdateSummaryLabels();
        }

        private void UpdateSummaryLabels()
        {
            int total = ClientArchiveStore.ActiveClients.Count;
            int active = ClientArchiveStore.ActiveClients.Count(c => c.Status == "Active");
            int inactive = ClientArchiveStore.ActiveClients.Count(c => c.Status == "Inactive");

            lblTotalClients.Text = $"Total Clients: {total}";
            lblActiveClients.Text = $"Active: {active}";
            lblInactiveClients.Text = $"Inactive: {inactive}";
        }

        private string GenerateNextClientId()
        {
            if (!ClientArchiveStore.ActiveClients.Any() && !ClientArchiveStore.ArchivedClients.Any())
                return "CL001";

            int maxActive = ClientArchiveStore.ActiveClients
                .Select(c => ExtractIdNumber(c.ClientID))
                .DefaultIfEmpty(0)
                .Max();

            int maxArchived = ClientArchiveStore.ArchivedClients
                .Select(c => ExtractIdNumber(c.ClientID))
                .DefaultIfEmpty(0)
                .Max();

            int next = Math.Max(maxActive, maxArchived) + 1;
            return "CL" + next.ToString("D3");
        }

        private int ExtractIdNumber(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId) || clientId.Length < 3) return 0;

            string numberPart = clientId.Replace("CL", "");
            return int.TryParse(numberPart, out int number) ? number : 0;
        }

        private void btnSearch_Click(object? sender, EventArgs e)
        {
            LoadClientsToGrid();
        }

        private void cboStatusFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadClientsToGrid();
        }

        private void btnAddClient_Click(object? sender, EventArgs e)
        {
            string newId = GenerateNextClientId();

            using (var dialog = new ClientDialogForm("Add New Client", newId))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ClientArchiveStore.ActiveClients.Add(new ClientItem
                    {
                        ClientID = dialog.ClientIDValue,
                        LibraryName = dialog.LibraryNameValue,
                        Email = dialog.EmailValue,
                        Password = dialog.PasswordValue,
                        Status = dialog.StatusValue
                    });

                    LoadClientsToGrid();
                }
            }
        }

        private void EditClient(string clientId)
        {
            var client = ClientArchiveStore.ActiveClients.FirstOrDefault(c => c.ClientID == clientId);
            if (client == null) return;

            using (var dialog = new ClientDialogForm("Edit Client", client.ClientID, client))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    client.LibraryName = dialog.LibraryNameValue;
                    client.Email = dialog.EmailValue;
                    client.Password = dialog.PasswordValue;
                    client.Status = dialog.StatusValue;

                    LoadClientsToGrid();
                }
            }
        }

        private void ArchiveClient(string clientId)
        {
            var client = ClientArchiveStore.ActiveClients.FirstOrDefault(c => c.ClientID == clientId);
            if (client == null) return;

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to archive client '{client.LibraryName}' ({client.ClientID})?",
                "Archive Client",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                client.Status = "Archived";
                ClientArchiveStore.ArchivedClients.Add(client);
                ClientArchiveStore.ActiveClients.Remove(client);

                string nextArchiveId = "AR" + (ArchiveStore.ArchivedItems.Count + 1).ToString("D3");

                ArchiveStore.ArchivedItems.Add(new ArchiveItem
                {
                    ArchiveID = nextArchiveId,
                    Module = "Clients",
                    RecordID = client.ClientID,
                    ItemName = client.LibraryName,
                    ExtraInfo = client.Email,
                    ArchivedBy = string.IsNullOrWhiteSpace(UserSession.Username) ? "Super Admin" : UserSession.Username,
                    ArchivedDate = DateTime.Now,
                    Status = "Archived"
                });

                LoadClientsToGrid();
            }
        }

        private void dgvClients_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string clientId = dgvClients.Rows[e.RowIndex].Cells["ID"].Value?.ToString() ?? "";

            if (dgvClients.Columns[e.ColumnIndex].Name == "Edit")
            {
                EditClient(clientId);
            }
            else if (dgvClients.Columns[e.ColumnIndex].Name == "Archive")
            {
                ArchiveClient(clientId);
            }
        }

        private void dgvClients_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvClients.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                string status = e.Value.ToString() ?? "";

                if (status == "Active")
                {
                    e.CellStyle.ForeColor = Color.DarkGreen;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
                else if (status == "Inactive")
                {
                    e.CellStyle.ForeColor = Color.DarkGoldenrod;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            }
        }
    }
}