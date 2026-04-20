using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
            AutoScroll = true;

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
                Padding = new Padding(34, 0, 34, 24),
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

            txtSearch.TextChanged += (s, e) => LoadClientsToGrid();

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
            dgvClients.Columns.Add("UserCount", "USER COUNT");
            dgvClients.Columns.Add("Status", "STATUS");
            dgvClients.Columns.Add("Actions", "ACTIONS");

            dgvClients.Columns["SchoolName"].FillWeight = 34;
            dgvClients.Columns["Email"].FillWeight = 24;
            dgvClients.Columns["UserCount"].FillWeight = 14;
            dgvClients.Columns["Status"].FillWeight = 14;
            dgvClients.Columns["Actions"].FillWeight = 14;

            foreach (DataGridViewColumn col in dgvClients.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvClients.CellPainting += dgvClients_CellPainting;
            dgvClients.CellClick += dgvClients_CellClick;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 64,
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

            lblFooterInfo.Location = new Point(26, 22);

            btnNext.Location = new Point(footerPanel.Width - 44, 12);
            btnPage3.Location = new Point(btnNext.Left - 48, 12);
            btnPage2.Location = new Point(btnPage3.Left - 48, 12);
            btnPage1.Location = new Point(btnPage2.Left - 48, 12);
            btnPrev.Location = new Point(btnPage1.Left - 48, 12);
        }

        private void ClientsForm_Resize(object? sender, EventArgs e)
        {
            AdjustResponsiveLayout();
        }

        private void LoadClientsToGrid()
        {
            dgvClients.Rows.Clear();

            string searchText = "";
            if (txtSearch != null &&
                !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                txtSearch.Text != "Search by school name, ID or email...")
            {
                searchText = txtSearch.Text.Trim();
            }

            List<ClientDbItem> clients = ClientService.GetClients(searchText);

            foreach (ClientDbItem client in clients)
            {
                string schoolCell = GetInitials(client.LibraryName) + "|" + client.LibraryName + "|" + client.LibraryCode;

                dgvClients.Rows.Add(
                    client.DbClientID,
                    schoolCell,
                    client.Email,
                    client.UserCount.ToString("N0"),
                    client.Status,
                    "Edit|Delete"
                );
            }

            dgvClients.ClearSelection();
            lblFooterInfo.Text = $"Showing 1 - {clients.Count} of {clients.Count} institutions";
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

            using ClientDialogForm dialog = new ClientDialogForm("Edit Client", libraryCode, editItem);

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

                    MessageBox.Show("Client updated successfully.");
                    LoadClientsToGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating client:\n\n" + ex.Message);
                }
            }
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
            if (e.RowIndex < 0) return;
            if (dgvClients.Columns[e.ColumnIndex].Name != "Actions") return;

            int dbClientId = Convert.ToInt32(dgvClients.Rows[e.RowIndex].Cells["DbClientID"].Value);
            string schoolCell = dgvClients.Rows[e.RowIndex].Cells["SchoolName"].Value?.ToString() ?? "";
            string[] parts = schoolCell.Split('|');
            if (parts.Length < 3) return;

            string libraryName = parts[1];
            string libraryCode = parts[2];

            int relativeX = dgvClients.PointToClient(Cursor.Position).X
                            - dgvClients.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).X;

            if (relativeX < 36)
                EditClient(dbClientId, libraryCode);
            else
                ArchiveClient(dbClientId, libraryName);
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
            else if (columnName == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                bool isActive = text.Equals("Active", StringComparison.OrdinalIgnoreCase);

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
                    "🗑",
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