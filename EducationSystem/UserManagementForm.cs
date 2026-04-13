using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class UserManagementForm : Form
    {
        private Label lblTitle;

        private TabControl tabControl;
        private TabPage tabLibrarian;
        private TabPage tabMember;

        private Panel topPanelLibrarian;
        private Panel topPanelMember;

        private TextBox txtSearchLibrarian;
        private TextBox txtSearchMember;

        private Button btnSearchLibrarian;
        private Button btnSearchMember;

        private Button btnAddLibrarian;
        private Button btnAddMember;

        private DataGridView dgvLibrarian;
        private DataGridView dgvMember;

        private readonly (string Username, string Email, string Password, string Status)[] librarianAccounts =
        {
            ("lib_jane", "jane@school.edu", "lib123", "Active"),
            ("lib_mark", "mark@school.edu", "mark123", "Active")
        };

                private readonly (string Username, string Email, string Password, string Status)[] memberAccounts =
                {
            ("johncruz", "john@school.edu", "john123", "Active"),
            ("maria.santos", "maria@school.edu", "maria123", "Active"),
            ("anneflores", "anne@school.edu", "anne123", "Archived")
        };

        public UserManagementForm()
        {
            InitializeComponent();
            BuildUI();
            LoadDummyData();

            Resize += MembersForm_Resize;
        }

        private void MembersForm_Resize(object? sender, EventArgs e)
        {
            ApplyResponsiveLayout();
        }

        private void BuildUI()
        {
            BackColor = Color.Snow;
            Dock = DockStyle.Fill;

            lblTitle = new Label();
            lblTitle.Text = "User Management";
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(24, 18);

            tabControl = new TabControl();
            tabControl.Location = new Point(24, 58);
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.SelectedIndexChanged += (s, e) => ApplyResponsiveLayout();
            tabControl.Resize += (s, e) => ApplyResponsiveLayout();

            tabLibrarian = new TabPage("Librarians");
            tabMember = new TabPage("Members");

            tabLibrarian.Resize += (s, e) => ApplyResponsiveLayout();
            tabMember.Resize += (s, e) => ApplyResponsiveLayout();

            BuildLibrarianTab();
            BuildMemberTab();

            tabControl.TabPages.Add(tabLibrarian);
            tabControl.TabPages.Add(tabMember);

            Controls.Add(lblTitle);
            Controls.Add(tabControl);

            ApplyResponsiveLayout();
        }

        private void BuildLibrarianTab()
        {
            topPanelLibrarian = new Panel();
            topPanelLibrarian.Height = 50;
            topPanelLibrarian.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            topPanelLibrarian.BackColor = Color.Transparent;

            txtSearchLibrarian = BuildSearchBox();
            btnSearchLibrarian = BuildSearchButton();
            btnAddLibrarian = BuildPrimaryButton("＋ Create New Librarian");

            btnSearchLibrarian.Click += (s, e) => ApplySearch(dgvLibrarian, txtSearchLibrarian.Text);
            btnAddLibrarian.Click += BtnAddLibrarian_Click;

            topPanelLibrarian.Controls.Add(txtSearchLibrarian);
            topPanelLibrarian.Controls.Add(btnSearchLibrarian);
            topPanelLibrarian.Controls.Add(btnAddLibrarian);

            dgvLibrarian = BuildGrid();
            dgvLibrarian.CellClick += DgvLibrarian_CellClick;
            dgvLibrarian.CellPainting += Dgv_CellPainting;

            tabLibrarian.Controls.Add(topPanelLibrarian);
            tabLibrarian.Controls.Add(dgvLibrarian);
        }

        private void BuildMemberTab()
        {
            topPanelMember = new Panel();
            topPanelMember.Height = 50;
            topPanelMember.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            topPanelMember.BackColor = Color.Transparent;

            txtSearchMember = BuildSearchBox();
            btnSearchMember = BuildSearchButton();
            btnAddMember = BuildPrimaryButton("＋ Create New Member");

            btnSearchMember.Click += (s, e) => ApplySearch(dgvMember, txtSearchMember.Text);
            btnAddMember.Click += BtnAddMember_Click;

            topPanelMember.Controls.Add(txtSearchMember);
            topPanelMember.Controls.Add(btnSearchMember);
            topPanelMember.Controls.Add(btnAddMember);

            dgvMember = BuildGrid();
            dgvMember.CellClick += DgvMember_CellClick;
            dgvMember.CellPainting += Dgv_CellPainting;

            tabMember.Controls.Add(topPanelMember);
            tabMember.Controls.Add(dgvMember);
        }

        private TextBox BuildSearchBox()
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Button BuildSearchButton()
        {
            Button btn = new Button();
            btn.Text = "Search";
            btn.Size = new Size(90, 32);
            btn.BackColor = Color.Maroon;
            btn.ForeColor = Color.WhiteSmoke;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private Button BuildPrimaryButton(string text)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.BackColor = Color.Maroon;
            btn.ForeColor = Color.WhiteSmoke;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            return btn;
        }

        private DataGridView BuildGrid()
        {
            DataGridView dgv = new DataGridView();

            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv.BackgroundColor = Color.WhiteSmoke;
            dgv.BorderStyle = BorderStyle.None;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersHeight = 34;
            dgv.EnableHeadersVisualStyles = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Maroon;
            dgv.GridColor = Color.LightGray;

            dgv.Columns.Add("Username", "Username");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("Password", "Password");
            dgv.Columns.Add("Status", "Status");

            DataGridViewTextBoxColumn actionCol = new DataGridViewTextBoxColumn();
            actionCol.Name = "Actions";
            actionCol.HeaderText = "Actions";
            actionCol.Width = 120;
            actionCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgv.Columns.Add(actionCol);

            return dgv;
        }

        private void ApplyResponsiveLayout()
        {
            lblTitle.Location = new Point(24, 18);

            tabControl.Location = new Point(24, 58);
            tabControl.Size = new Size(ClientSize.Width - 48, ClientSize.Height - 82);

            topPanelLibrarian.Location = new Point(12, 12);
            topPanelLibrarian.Size = new Size(tabLibrarian.ClientSize.Width - 24, 40);

            topPanelMember.Location = new Point(12, 12);
            topPanelMember.Size = new Size(tabMember.ClientSize.Width - 24, 40);

            LayoutTopPanel(topPanelLibrarian, txtSearchLibrarian, btnSearchLibrarian, btnAddLibrarian);
            LayoutTopPanel(topPanelMember, txtSearchMember, btnSearchMember, btnAddMember);

            dgvLibrarian.Location = new Point(12, 64);
            dgvLibrarian.Size = new Size(tabLibrarian.ClientSize.Width - 24, tabLibrarian.ClientSize.Height - 76);

            dgvMember.Location = new Point(12, 64);
            dgvMember.Size = new Size(tabMember.ClientSize.Width - 24, tabMember.ClientSize.Height - 76);
        }

        private void LayoutTopPanel(Panel host, TextBox searchBox, Button searchBtn, Button addBtn)
        {
            if (host == null) return;

            host.Height = 40;

            searchBox.Location = new Point(0, 4);
            searchBox.Size = new Size(250, 30);

            searchBtn.Location = new Point(searchBox.Right + 12, 4);
            searchBtn.Size = new Size(90, 32);

            addBtn.Size = new Size(240, 32);
            addBtn.Location = new Point(host.ClientSize.Width - addBtn.Width, 4);
        }

        private void LoadDummyData()
        {
            dgvLibrarian.Rows.Clear();
            dgvMember.Rows.Clear();

            dgvLibrarian.Rows.Add("lib_jane", "jane@school.edu", "lib123", "Active", "");
            dgvLibrarian.Rows.Add("lib_mark", "mark@school.edu", "mark123", "Active", "");

            dgvMember.Rows.Add("johncruz", "john@school.edu", "john123", "Active", "");
            dgvMember.Rows.Add("maria.santos", "maria@school.edu", "maria123", "Active", "");
            dgvMember.Rows.Add("anneflores", "anne@school.edu", "anne123", "Archived", "");
        }



        private void ApplySearch(DataGridView dgv, string keyword)
        {
            keyword = keyword.Trim().ToLower();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                bool visible = string.IsNullOrWhiteSpace(keyword);

                if (!visible)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string value = row.Cells[i].Value?.ToString()?.ToLower() ?? "";
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

        private void BtnAddLibrarian_Click(object? sender, EventArgs e)
        {
            using (UserEntryForm entry = new UserEntryForm("Create New Librarian"))
            {
                if (entry.ShowDialog() == DialogResult.OK)
                {
                    dgvLibrarian.Rows.Add(
                        entry.UsernameValue,
                        entry.EmailValue,
                        "••••••••",
                        "Active",
                        ""
                    );
                }
            }
        }

        private void BtnAddMember_Click(object? sender, EventArgs e)
        {
            using (UserEntryForm entry = new UserEntryForm("Create New Member"))
            {
                if (entry.ShowDialog() == DialogResult.OK)
                {
                    dgvMember.Rows.Add(
                        entry.UsernameValue,
                        entry.EmailValue,
                        "••••••••",
                        "Active",
                        ""
                    );
                }
            }
        }

        private void DgvLibrarian_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvLibrarian.Columns[e.ColumnIndex].Name != "Actions") return;

            HandleActionClick(dgvLibrarian, e.RowIndex, e.ColumnIndex, true);
        }

        private void DgvMember_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvMember.Columns[e.ColumnIndex].Name != "Actions") return;

            HandleActionClick(dgvMember, e.RowIndex, e.ColumnIndex, false);
        }

        private void HandleActionClick(DataGridView dgv, int rowIndex, int columnIndex, bool isLibrarian)
        {
            Rectangle cellRect = dgv.GetCellDisplayRectangle(columnIndex, rowIndex, false);
            int relativeX = dgv.PointToClient(Cursor.Position).X - cellRect.X;

            if (relativeX < cellRect.Width / 2)
            {
                string title = isLibrarian ? "Edit Librarian" : "Edit Member";

                using (UserEntryForm entry = new UserEntryForm(title))
                {
                    entry.UsernameValue = dgv.Rows[rowIndex].Cells["Username"].Value?.ToString() ?? "";
                    entry.EmailValue = dgv.Rows[rowIndex].Cells["Email"].Value?.ToString() ?? "";
                    entry.PasswordValue = "";
                    entry.LoadValues();

                    if (entry.ShowDialog() == DialogResult.OK)
                    {
                        dgv.Rows[rowIndex].Cells["Username"].Value = entry.UsernameValue;
                        dgv.Rows[rowIndex].Cells["Email"].Value = entry.EmailValue;

                        if (!string.IsNullOrWhiteSpace(entry.PasswordValue))
                            dgv.Rows[rowIndex].Cells["Password"].Value = "••••••••";
                    }
                }
            }
            else
            {
                string itemType = isLibrarian ? "librarian" : "member";
                string userName = dgv.Rows[rowIndex].Cells["Username"].Value?.ToString() ?? "this record";

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to archive {itemType} \"{userName}\"?",
                    "Archive Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgv.Rows[rowIndex].Cells["Status"].Value = "Archived";
                }
            }
        }

        private void Dgv_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv) return;
            if (e.RowIndex < 0) return;
            if (dgv.Columns[e.ColumnIndex].Name != "Actions") return;

            e.PaintBackground(e.CellBounds, true);
            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

            string leftIcon = "✎";
            string rightIcon = "📥";

            using (Font iconFont = new Font("Segoe UI Emoji", 11F, FontStyle.Regular))
            using (Brush brush = new SolidBrush(Color.Maroon))
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                Rectangle leftRect = new Rectangle(
                    e.CellBounds.X,
                    e.CellBounds.Y,
                    e.CellBounds.Width / 2,
                    e.CellBounds.Height);

                Rectangle rightRect = new Rectangle(
                    e.CellBounds.X + e.CellBounds.Width / 2,
                    e.CellBounds.Y,
                    e.CellBounds.Width / 2,
                    e.CellBounds.Height);

                e.Graphics.DrawString(leftIcon, iconFont, brush, leftRect, sf);
                e.Graphics.DrawString(rightIcon, iconFont, brush, rightRect, sf);
            }

            e.Handled = true;
        }

    }
}