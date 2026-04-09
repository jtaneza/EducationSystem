using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ClientsForm : Form
    {
        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnAddClient;
        private DataGridView dgvClients;
        private Panel topPanel;
        private Panel tableContainer;

        public ClientsForm()
        {
            InitializeComponent();
            BuildClientsUI();
            SeedSampleDataOnce();
            LoadClientsToGrid();
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

        private void BuildClientsUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            // TOP PANEL
            topPanel = new Panel();
            topPanel.BackColor = Color.Snow;
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 115;

            // TITLE
            lblTitle = new Label();
            lblTitle.Text = "Clients Management";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 15);

            // SEARCH BOX
            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Size = new Size(250, 30);
            txtSearch.Location = new Point(30, 58);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.BackColor = Color.White;

            // SEARCH BUTTON
            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSearch.Size = new Size(90, 30);
            btnSearch.Location = new Point(290, 58);
            StyleButton(btnSearch);
            btnSearch.Click += btnSearch_Click;

            // ADD BUTTON
            btnAddClient = new Button();
            btnAddClient.Text = "+ ADD NEW CLIENT";
            btnAddClient.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAddClient.Size = new Size(150, 32);
            btnAddClient.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(btnAddClient);
            btnAddClient.Click += btnAddClient_Click;

            // initial position, then adjusted again on resize
            btnAddClient.Location = new Point(Math.Max(390, Width - 210), 57);

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(btnAddClient);

            // keep Add button aligned to the right and on same row as search
            Resize += ClientsForm_Resize;
            AdjustTopControls();

            // DATA GRID
            dgvClients = new DataGridView();
            dgvClients.Dock = DockStyle.Fill;
            dgvClients.BackgroundColor = Color.White;
            dgvClients.BorderStyle = BorderStyle.None;
            dgvClients.RowHeadersVisible = false;
            dgvClients.AllowUserToAddRows = false;
            dgvClients.AllowUserToResizeRows = false;
            dgvClients.AllowUserToResizeColumns = false;
            dgvClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClients.MultiSelect = false;
            dgvClients.ReadOnly = true;
            dgvClients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClients.EnableHeadersVisualStyles = false;
            dgvClients.ColumnHeadersHeight = 38;
            dgvClients.RowTemplate.Height = 38;
            dgvClients.GridColor = Color.Gainsboro;
            dgvClients.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvClients.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvClients.BackgroundColor = Color.WhiteSmoke;

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

            DataGridViewTextBoxColumn actionCol = new DataGridViewTextBoxColumn();
            actionCol.Name = "Actions";
            actionCol.HeaderText = "Actions";
            actionCol.Width = 110;
            actionCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            actionCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            actionCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvClients.Columns.Add(actionCol);

            foreach (DataGridViewColumn col in dgvClients.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvClients.Columns["ID"].FillWeight = 18;
            dgvClients.Columns["Name"].FillWeight = 28;
            dgvClients.Columns["Email"].FillWeight = 26;
            dgvClients.Columns["Status"].FillWeight = 18;
            dgvClients.Columns["Actions"].FillWeight = 10;

            dgvClients.CellClick += dgvClients_CellClick;

            tableContainer = new Panel();
            tableContainer.Dock = DockStyle.Fill;
            tableContainer.Padding = new Padding(30, 0, 30, 20);
            tableContainer.BackColor = Color.Snow;
            tableContainer.Controls.Add(dgvClients);

            Controls.Add(tableContainer);
            Controls.Add(topPanel);
        }

        private void ClientsForm_Resize(object? sender, EventArgs e)
        {
            AdjustTopControls();
        }

        private void AdjustTopControls()
        {
            if (btnAddClient == null || txtSearch == null || btnSearch == null)
                return;

            // keep Add button aligned with search controls row
            int rightMargin = 30;
            btnAddClient.Location = new Point(
                Math.Max(btnSearch.Right + 40, ClientSize.Width - btnAddClient.Width - rightMargin),
                txtSearch.Top - 1
            );
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
            dgvClients.Rows.Clear();

            foreach (var client in ClientArchiveStore.ActiveClients)
            {
                dgvClients.Rows.Add(
                    client.ClientID,
                    client.LibraryName,
                    client.Email,
                    client.Status,
                    "✎     📥"
                );
            }

            dgvClients.ClearSelection();
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
            string keyword = txtSearch.Text.Trim().ToLower();

            dgvClients.Rows.Clear();

            var filtered = ClientArchiveStore.ActiveClients
                .Where(c =>
                    c.ClientID.ToLower().Contains(keyword) ||
                    c.LibraryName.ToLower().Contains(keyword) ||
                    c.Email.ToLower().Contains(keyword) ||
                    c.Status.ToLower().Contains(keyword))
                .ToList();

            foreach (var client in filtered)
            {
                dgvClients.Rows.Add(
                    client.ClientID,
                    client.LibraryName,
                    client.Email,
                    client.Status,
                    "✎     📥"
                );
            }

            dgvClients.ClearSelection();
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

            if (dgvClients.Columns[e.ColumnIndex].Name == "Actions")
            {
                Rectangle cellRect = dgvClients.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                int clickX = dgvClients.PointToClient(Cursor.Position).X - cellRect.Left;

                string clientId = dgvClients.Rows[e.RowIndex].Cells["ID"].Value?.ToString() ?? "";

                if (clickX < cellRect.Width / 2)
                {
                    EditClient(clientId);
                }
                else
                {
                    ArchiveClient(clientId);
                }
            }
        }
    }
}