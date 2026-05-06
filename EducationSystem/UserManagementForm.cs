using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class UserManagementForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentSoft = ColorTranslator.FromHtml("#B7EBD7");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color DangerText = ColorTranslator.FromHtml("#BA1A1A");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;

        private Button btnAddUser = null!;
        private Panel tabPanel = null!;
        private Button btnLibrarian = null!;
        private Button btnMember = null!;

        private Panel metricCardOne = null!;
        private Panel metricCardTwo = null!;

        private Panel tableCard = null!;
        private Panel searchPanel = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;

        private DataGridView dgvUsers = null!;
        private Label lblFooter = null!;
        private Panel pagerPanel = null!;

        private bool showingLibrarian = true;
        private bool isLoadingGrid = false;

        public UserManagementForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            BuildUI();

            SuspendLayout();
            canvas.SuspendLayout();

            AdjustLayout();
            LoadLibrarianData();
            ClearGridSelection();

            canvas.ResumeLayout();
            ResumeLayout();

            Resize += (s, e) => AdjustLayout();
        }

        private void BuildUI()
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
                Text = "User Management",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubTitle = new Label
            {
                Text = "Manage and audit library staff and registered members.",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            btnAddUser = new Button
            {
                Text = "👥  Add New Librarian",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddUser.FlatAppearance.BorderSize = 0;
            btnAddUser.Click += BtnAddUser_Click;

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubTitle);
            canvas.Controls.Add(btnAddUser);

            BuildTabs();
            BuildMetricCards();
            BuildTableCard();
        }

        private void BuildTabs()
        {
            tabPanel = new Panel { BackColor = HeaderBack };
            tabPanel.Paint += RoundedPanelPaint;

            btnLibrarian = new Button
            {
                Text = "Librarian",
                FlatStyle = FlatStyle.Flat,
                BackColor = CardBack,
                ForeColor = AccentDeep,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLibrarian.FlatAppearance.BorderSize = 0;
            btnLibrarian.Click += (s, e) => LoadLibrarianData();

            btnMember = new Button
            {
                Text = "Member",
                FlatStyle = FlatStyle.Flat,
                BackColor = HeaderBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 11F),
                Cursor = Cursors.Hand
            };
            btnMember.FlatAppearance.BorderSize = 0;
            btnMember.Click += (s, e) => LoadMemberData();

            tabPanel.Controls.Add(btnLibrarian);
            tabPanel.Controls.Add(btnMember);
            canvas.Controls.Add(tabPanel);
        }

        private void BuildMetricCards()
        {
            metricCardOne = CreateRoundedCard();
            metricCardTwo = CreateRoundedCard();

            canvas.Controls.Add(metricCardOne);
            canvas.Controls.Add(metricCardTwo);

            BuildMetricCard(metricCardOne, "👥", "ACTIVE LIBRARIANS", "0", "");
            BuildMetricCard(metricCardTwo, "📈", "ACTIVE NOW", "0", "Sessions");
        }

        private void BuildMetricCard(Panel card, string iconText, string labelText, string valueText, string badgeText)
        {
            Panel iconBox = new Panel
            {
                BackColor = AccentSoft,
                Size = new Size(52, 52)
            };
            iconBox.Paint += RoundedPanelPaint;

            Label icon = new Label
            {
                Text = iconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 18F),
                BackColor = Color.Transparent
            };
            iconBox.Controls.Add(icon);

            Label lbl = new Label
            {
                Text = labelText,
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            Label value = new Label
            {
                Text = valueText,
                AutoSize = true,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            Label badge = new Label
            {
                Text = badgeText,
                AutoSize = false,
                Size = new Size(80, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = AccentDeep,
                BackColor = AccentSoft,
                Visible = !string.IsNullOrWhiteSpace(badgeText)
            };

            card.Controls.Add(iconBox);
            card.Controls.Add(lbl);
            card.Controls.Add(value);
            card.Controls.Add(badge);

            card.Tag = new Control[] { iconBox, lbl, value, badge };
        }

        private void BuildTableCard()
        {
            tableCard = CreateRoundedCard();
            canvas.Controls.Add(tableCard);

            searchPanel = new Panel { BackColor = HeaderBack };
            searchPanel.Paint += RoundedPanelPaint;

            Label searchIcon = new Label
            {
                Text = "⌕",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 14F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = HeaderBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 10.5F),
                Text = "Search librarians..."
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text.StartsWith("Search") || txtSearch.Text.StartsWith("Filter"))
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = PrimaryText;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = showingLibrarian ? "Search librarians..." : "Filter members...";
                    txtSearch.ForeColor = SecondaryText;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                if (isLoadingGrid) return;
                if (!txtSearch.Focused && txtSearch.Text.StartsWith("Search")) return;
                if (!txtSearch.Focused && txtSearch.Text.StartsWith("Filter")) return;
                ApplySearch(txtSearch.Text);
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(txtSearch);

            btnFilter = new Button
            {
                Text = "≡",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderSize = 0;

            dgvUsers = new DataGridView
            {
                BackgroundColor = CardBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 54,
                RowTemplate = { Height = 78 },
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical,
                ShowCellToolTips = false
            };

            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvUsers.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvUsers.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvUsers.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgvUsers.DefaultCellStyle.BackColor = CardBack;
            dgvUsers.DefaultCellStyle.ForeColor = PrimaryText;
            dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            dgvUsers.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvUsers.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvUsers.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            dgvUsers.RowsDefaultCellStyle.BackColor = CardBack;
            dgvUsers.RowsDefaultCellStyle.SelectionBackColor = CardBack;
            dgvUsers.RowsDefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvUsers.MouseDown += DgvUsers_MouseDown;
            dgvUsers.MouseMove += DgvUsers_MouseMove;

            lblFooter = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            pagerPanel = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(220, 36)
            };

            BuildPager();

            tableCard.Controls.Add(searchPanel);
            tableCard.Controls.Add(btnFilter);
            tableCard.Controls.Add(dgvUsers);
            tableCard.Controls.Add(lblFooter);
            tableCard.Controls.Add(pagerPanel);
        }
        private void DgvUsers_MouseDown(object? sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hit = dgvUsers.HitTest(e.X, e.Y);

            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            string columnName = dgvUsers.Columns[hit.ColumnIndex].Name;

            if (columnName == "EditAction")
            {
                EditSelectedUser(hit.RowIndex);
                return;
            }

            if (columnName == "ArchiveAction")
            {
                ArchiveSelectedUser(hit.RowIndex);
                return;
            }
        }

        private void DgvUsers_MouseMove(object? sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hit = dgvUsers.HitTest(e.X, e.Y);

            if (hit.RowIndex >= 0 && hit.ColumnIndex >= 0)
            {
                string columnName = dgvUsers.Columns[hit.ColumnIndex].Name;

                dgvUsers.Cursor =
                    columnName == "EditAction" || columnName == "ArchiveAction"
                    ? Cursors.Hand
                    : Cursors.Default;
            }
            else
            {
                dgvUsers.Cursor = Cursors.Default;
            }
        }
        private void SetLibrarianColumns()
        {
            dgvUsers.Columns.Clear();

            dgvUsers.Columns.Add("Id", "ID");
            dgvUsers.Columns.Add("Name", "NAME");
            dgvUsers.Columns.Add("Email", "EMAIL");
            dgvUsers.Columns.Add("Role", "ROLE");
            dgvUsers.Columns.Add("Status", "STATUS");
            dgvUsers.Columns.Add("EditAction", "EDIT");
            dgvUsers.Columns.Add("ArchiveAction", "ARCHIVE");

            dgvUsers.Columns["Id"].FillWeight = 15;
            dgvUsers.Columns["Name"].FillWeight = 25;
            dgvUsers.Columns["Email"].FillWeight = 25;
            dgvUsers.Columns["Role"].FillWeight = 15;
            dgvUsers.Columns["Status"].FillWeight = 12;
            dgvUsers.Columns["EditAction"].FillWeight = 6;
            dgvUsers.Columns["ArchiveAction"].FillWeight = 7;

            ApplyCommonColumnStyle();
        }

        private void SetMemberColumns()
        {
            dgvUsers.Columns.Clear();

            dgvUsers.Columns.Add("Id", "ID");
            dgvUsers.Columns.Add("NameIdentity", "NAME");
            dgvUsers.Columns.Add("Email", "EMAIL");
            dgvUsers.Columns.Add("ContactNumber", "CONTACT");
            dgvUsers.Columns.Add("Address", "ADDRESS");
            dgvUsers.Columns.Add("Role", "ROLE");
            dgvUsers.Columns.Add("Status", "STATUS");
            dgvUsers.Columns.Add("EditAction", "EDIT");
            dgvUsers.Columns.Add("ArchiveAction", "ARCHIVE");

            dgvUsers.Columns["Id"].FillWeight = 12;
            dgvUsers.Columns["NameIdentity"].FillWeight = 18;
            dgvUsers.Columns["Email"].FillWeight = 18;
            dgvUsers.Columns["ContactNumber"].FillWeight = 16;
            dgvUsers.Columns["Address"].FillWeight = 18;
            dgvUsers.Columns["Role"].FillWeight = 10;
            dgvUsers.Columns["Status"].FillWeight = 10;
            dgvUsers.Columns["EditAction"].FillWeight = 6;
            dgvUsers.Columns["ArchiveAction"].FillWeight = 7;

            ApplyCommonColumnStyle();
        }

        private void ApplyCommonColumnStyle()
        {
            foreach (DataGridViewColumn col in dgvUsers.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.DefaultCellStyle.BackColor = CardBack;
                col.DefaultCellStyle.SelectionBackColor = CardBack;
                col.DefaultCellStyle.SelectionForeColor = PrimaryText;
            }

            if (dgvUsers.Columns.Contains("EditAction"))
            {
                dgvUsers.Columns["EditAction"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvUsers.Columns["EditAction"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvUsers.Columns["EditAction"].DefaultCellStyle.ForeColor = SecondaryText;
                dgvUsers.Columns["EditAction"].DefaultCellStyle.Font = new Font("Segoe UI Symbol", 12F, FontStyle.Bold);
            }

            if (dgvUsers.Columns.Contains("ArchiveAction"))
            {
                dgvUsers.Columns["ArchiveAction"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvUsers.Columns["ArchiveAction"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvUsers.Columns["ArchiveAction"].DefaultCellStyle.ForeColor = DangerText;
                dgvUsers.Columns["ArchiveAction"].DefaultCellStyle.Font = new Font("Segoe UI Emoji", 11F, FontStyle.Regular);
            }
        }

        private void LoadLibrarianData()
        {
            if (dgvUsers == null) return;

            isLoadingGrid = true;
            showingLibrarian = true;

            btnLibrarian.BackColor = CardBack;
            btnLibrarian.ForeColor = AccentDeep;
            btnLibrarian.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            btnMember.BackColor = HeaderBack;
            btnMember.ForeColor = SecondaryText;
            btnMember.Font = new Font("Segoe UI", 11F);

            btnAddUser.Text = "👥  Add New Librarian";
            txtSearch.Text = "Search librarians...";
            txtSearch.ForeColor = SecondaryText;

            SetLibrarianColumns();
            dgvUsers.Rows.Clear();

            int totalLibrarians = 0;
            int activeLibrarians = 0;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"
SELECT UserID, FullName, Email, Role, IsActive
FROM dbo.Users
WHERE ClientID = @ClientID
  AND IsArchived = 0
  AND Role IN ('Head Librarian', 'Librarian', 'Assistant')
ORDER BY UserID ASC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int userId = Convert.ToInt32(reader["UserID"]);
                    string fullName = reader["FullName"] == DBNull.Value ? "" : Convert.ToString(reader["FullName"]) ?? "";
                    string email = reader["Email"] == DBNull.Value ? "" : Convert.ToString(reader["Email"]) ?? "";
                    string role = reader["Role"] == DBNull.Value ? "Librarian" : Convert.ToString(reader["Role"]) ?? "Librarian";
                    bool isActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]);

                    dgvUsers.Rows.Add(
                        "#LB-" + userId.ToString("000"),
                        fullName,
                        email,
                        role.ToUpper(),
                        isActive ? "Active" : "Paused",
                        "✎",
                        "📥"
                    );

                    dgvUsers.Rows[dgvUsers.Rows.Count - 1].Tag = userId;

                    totalLibrarians++;
                    if (isActive) activeLibrarians++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load librarians from database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            SetMetricCard(metricCardOne, "ACTIVE LIBRARIANS", totalLibrarians.ToString(), "");
            SetMetricCard(metricCardTwo, "ACTIVE NOW", activeLibrarians.ToString(), "Sessions");

            lblFooter.Text = totalLibrarians == 0
                ? "Showing 0 librarians"
                : $"Showing 1-{totalLibrarians} of {totalLibrarians} Librarians";

            isLoadingGrid = false;

            AdjustLayout();
            ClearGridSelection();
        }

        private void LoadMemberData()
        {
            if (dgvUsers == null) return;

            isLoadingGrid = true;
            showingLibrarian = false;

            btnMember.BackColor = CardBack;
            btnMember.ForeColor = AccentDeep;
            btnMember.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            btnLibrarian.BackColor = HeaderBack;
            btnLibrarian.ForeColor = SecondaryText;
            btnLibrarian.Font = new Font("Segoe UI", 11F);

            btnAddUser.Text = "👤  Add New Member";
            txtSearch.Text = "Filter members...";
            txtSearch.ForeColor = SecondaryText;

            SetMemberColumns();
            dgvUsers.Rows.Clear();

            int totalMembers = 0;
            int activeMembers = 0;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"
SELECT UserID, FullName, Email, Role, IsActive, ContactNumber, Address
FROM dbo.Users
WHERE ClientID = @ClientID
  AND IsArchived = 0
  AND Role IN ('Student', 'Teacher')
ORDER BY UserID ASC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int userId = Convert.ToInt32(reader["UserID"]);
                    string name = Convert.ToString(reader["FullName"]) ?? "";
                    string email = Convert.ToString(reader["Email"]) ?? "";
                    string contact = Convert.ToString(reader["ContactNumber"]) ?? "";
                    string address = Convert.ToString(reader["Address"]) ?? "";
                    string role = Convert.ToString(reader["Role"]) ?? "Student";
                    bool isActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]);

                    dgvUsers.Rows.Add(
                        "MB-" + userId.ToString("000"),
                        name,
                        email,
                        contact,
                        address,
                        role.ToUpper(),
                        isActive ? "Active" : "Paused",
                        "✎",
                        "📥"
                    );

                    dgvUsers.Rows[dgvUsers.Rows.Count - 1].Tag = userId;

                    totalMembers++;
                    if (isActive) activeMembers++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load members from database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            SetMetricCard(metricCardOne, "ACTIVE MEMBERS", totalMembers.ToString(), "");
            SetMetricCard(metricCardTwo, "ACTIVE NOW", activeMembers.ToString(), "Members");

            lblFooter.Text = totalMembers == 0
                ? "Showing 0 members"
                : $"Showing 1-{totalMembers} of {totalMembers} members";

            isLoadingGrid = false;

            AdjustLayout();
            ClearGridSelection();
        }

        private void DgvUsers_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvUsers.Columns[e.ColumnIndex].Name;

            if (columnName == "EditAction")
            {
                EditSelectedUser(e.RowIndex);
                return;
            }

            if (columnName == "ArchiveAction")
            {
                ArchiveSelectedUser(e.RowIndex);
                return;
            }
        }

        private void DgvUsers_CellMouseMove(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 &&
                e.ColumnIndex >= 0 &&
                (dgvUsers.Columns[e.ColumnIndex].Name == "EditAction" ||
                 dgvUsers.Columns[e.ColumnIndex].Name == "ArchiveAction"))
            {
                dgvUsers.Cursor = Cursors.Hand;
            }
            else
            {
                dgvUsers.Cursor = Cursors.Default;
            }
        }

        private void EditSelectedUser(int rowIndex)
        {
            int userId = dgvUsers.Rows[rowIndex].Tag is int id ? id : 0;
            if (userId == 0) return;

            if (showingLibrarian)
            {
                string name = dgvUsers.Rows[rowIndex].Cells["Name"].Value?.ToString() ?? "";
                string email = dgvUsers.Rows[rowIndex].Cells["Email"].Value?.ToString() ?? "";
                string role = dgvUsers.Rows[rowIndex].Cells["Role"].Value?.ToString() ?? "Librarian";

                using RegisterLibrarianDialog dlg = new RegisterLibrarianDialog();
                dlg.LoadForEdit(userId, name, email, ToTitleCase(role));

                if (dlg.ShowDialog() == DialogResult.OK)
                    LoadLibrarianData();
            }
            else
            {
                string name = dgvUsers.Rows[rowIndex].Cells["NameIdentity"].Value?.ToString() ?? "";
                string email = dgvUsers.Rows[rowIndex].Cells["Email"].Value?.ToString() ?? "";
                string contact = dgvUsers.Rows[rowIndex].Cells["ContactNumber"].Value?.ToString() ?? "";
                string address = dgvUsers.Rows[rowIndex].Cells["Address"].Value?.ToString() ?? "";
                string role = dgvUsers.Rows[rowIndex].Cells["Role"].Value?.ToString() ?? "Student";

                using RegisterMemberDialog dlg = new RegisterMemberDialog();
                dlg.LoadForEdit(userId, name, email, contact, address, ToTitleCase(role));

                if (dlg.ShowDialog() == DialogResult.OK)
                    LoadMemberData();
            }
        }

        private void ArchiveSelectedUser(int rowIndex)
        {
            int userId = dgvUsers.Rows[rowIndex].Tag is int id ? id : 0;
            if (userId == 0) return;

            DialogResult confirm = MessageBox.Show(
                "Archive this user?",
                "Confirm Archive",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"
UPDATE dbo.Users
SET IsArchived = 1,
    ArchivedByUserID = @ArchivedByUserID,
    ArchivedByRole = @ArchivedByRole,
    ArchivedAt = GETDATE()
WHERE UserID = @UserID
  AND ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                cmd.Parameters.AddWithValue("@ArchivedByUserID", ClientSession.UserID);
                cmd.Parameters.AddWithValue("@ArchivedByRole", ClientSession.Role ?? "Unknown");

                cmd.ExecuteNonQuery();

                if (showingLibrarian)
                    LoadLibrarianData();
                else
                    LoadMemberData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to archive user.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void SetMetricCard(Panel card, string labelText, string valueText, string badgeText)
        {
            if (card?.Tag is not Control[] c) return;

            ((Label)c[1]).Text = labelText;
            ((Label)c[2]).Text = valueText;
            ((Label)c[3]).Text = badgeText;
            ((Label)c[3]).Visible = !string.IsNullOrWhiteSpace(badgeText);
        }

        private void ApplySearch(string keyword)
        {
            keyword = keyword.Trim().ToLower();

            foreach (DataGridViewRow row in dgvUsers.Rows)
            {
                if (row.IsNewRow) continue;

                bool visible = string.IsNullOrWhiteSpace(keyword);

                if (!visible)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        string value = cell.Value?.ToString()?.ToLower() ?? "";
                        if (value.Contains(keyword))
                        {
                            visible = true;
                            break;
                        }
                    }
                }

                row.Visible = visible;
            }
        }

        private void BtnAddUser_Click(object? sender, EventArgs e)
        {
            if (showingLibrarian)
            {
                using RegisterLibrarianDialog dlg = new RegisterLibrarianDialog();

                if (dlg.ShowDialog() == DialogResult.OK)
                    LoadLibrarianData();
            }
            else
            {
                using RegisterMemberDialog dlg = new RegisterMemberDialog();

                if (dlg.ShowDialog() == DialogResult.OK)
                    LoadMemberData();
            }
        }


        private string ToTitleCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            value = value.Trim().ToLower();

            if (value == "student") return "Student";
            if (value == "teacher") return "Teacher";
            if (value == "librarian") return "Librarian";
            if (value == "assistant") return "Assistant";
            if (value == "head librarian") return "Head Librarian";

            return value;
        }

        private string GetInitials(string name)
        {
            string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }

        private void ClearGridSelection()
        {
            if (dgvUsers == null || dgvUsers.IsDisposed) return;
            dgvUsers.ClearSelection();
            dgvUsers.CurrentCell = null;
        }

        private void BuildPager()
        {
            pagerPanel.Controls.Clear();
            pagerPanel.Controls.Add(CreatePagerButton("‹", false, new Point(0, 0)));
            pagerPanel.Controls.Add(CreatePagerButton("1", true, new Point(40, 0)));
            pagerPanel.Controls.Add(CreatePagerButton("2", false, new Point(80, 0)));
            pagerPanel.Controls.Add(CreatePagerButton("3", false, new Point(120, 0)));
            pagerPanel.Controls.Add(CreatePagerButton("›", false, new Point(160, 0)));
        }

        private Button CreatePagerButton(string text, bool active, Point location)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(32, 32),
                Location = location,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = active ? AccentDeep : Color.Transparent,
                ForeColor = active ? Color.White : SecondaryText,
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = active ? 0 : 1;
            btn.FlatAppearance.BorderColor = LineSoft;

            return btn;
        }

        private Panel CreateRoundedCard()
        {
            Panel p = new Panel
            {
                BackColor = CardBack
            };
            p.Paint += RoundedPanelPaint;
            return p;
        }

        private void AdjustLayout()
        {
            if (canvas == null || tableCard == null) return;

            int margin = 34;
            int gap = 18;
            int width = Math.Max(980, canvas.ClientSize.Width - margin * 2);

            lblTitle.Location = new Point(margin, 34);
            lblSubTitle.Location = new Point(margin, 78);

            btnAddUser.Bounds = new Rectangle(width - 250 + margin, 46, 250, 48);

            tabPanel.Bounds = new Rectangle(margin, 130, 310, 58);
            btnLibrarian.Bounds = new Rectangle(6, 6, 148, 46);
            btnMember.Bounds = new Rectangle(158, 6, 146, 46);

            metricCardOne.Bounds = new Rectangle(margin + 340, 130, 310, 96);
            metricCardTwo.Bounds = new Rectangle(metricCardOne.Right + gap, 130, 310, 96);

            LayoutMetricCard(metricCardOne);
            LayoutMetricCard(metricCardTwo);

            tableCard.Bounds = new Rectangle(margin, 260, width, 680);

            searchPanel.Bounds = new Rectangle(26, 26, showingLibrarian ? 480 : 260, 38);
            searchPanel.Controls[0].Location = new Point(12, 7);
            txtSearch.Location = new Point(42, 9);
            txtSearch.Width = searchPanel.Width - 54;

            btnFilter.Bounds = new Rectangle(tableCard.Width - 70, 24, 42, 40);

            dgvUsers.Location = new Point(0, 84);
            dgvUsers.Size = new Size(tableCard.Width, 500);

            lblFooter.Location = new Point(28, tableCard.Height - 45);
            pagerPanel.Location = new Point(tableCard.Width - pagerPanel.Width - 28, tableCard.Height - 54);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 30);
        }

        private void LayoutMetricCard(Panel card)
        {
            if (card?.Tag is not Control[] c) return;

            c[0].Location = new Point(22, 22);
            c[1].Location = new Point(88, 22);
            c[2].Location = new Point(88, 44);
            c[3].Location = new Point(c[2].Right + 8, c[2].Top + 10);
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control p) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using var brush = new SolidBrush(p.BackColor);
            using var path = GetRoundedRectPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 16);

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