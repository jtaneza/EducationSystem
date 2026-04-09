using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ModulesForm : Form
    {
        private Label lblTitle = null!;
        private Label lblSubInfo = null!;
        private TextBox txtSearch = null!;
        private Button btnSearch = null!;
        private Button btnAddModule = null!;

        private Panel topPanel = null!;
        private Panel tablePanel = null!;
        private DataGridView dgvModules = null!;

        public ModulesForm()
        {
            InitializeComponent();
            BuildModulesUI();
            SeedModulesOnce();
            LoadModulesData();
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

        private void BuildModulesUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            topPanel = new Panel();
            topPanel.BackColor = Color.Snow;
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 120;

            lblTitle = new Label();
            lblTitle.Text = "System Modules";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 12);

            lblSubInfo = new Label();
            lblSubInfo.Text = "Manage enabled features and system modules for the platform.";
            lblSubInfo.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblSubInfo.ForeColor = Color.DimGray;
            lblSubInfo.AutoSize = true;
            lblSubInfo.Location = new Point(32, 42);

            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Size = new Size(250, 30);
            txtSearch.Location = new Point(30, 72);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.BackColor = Color.White;

            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSearch.Size = new Size(90, 30);
            btnSearch.Location = new Point(290, 72);
            StyleButton(btnSearch);
            btnSearch.Click += BtnSearch_Click;

            btnAddModule = new Button();
            btnAddModule.Text = "+ ADD MODULE";
            btnAddModule.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAddModule.Size = new Size(140, 32);
            btnAddModule.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(btnAddModule);
            btnAddModule.Click += BtnAddModule_Click;

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblSubInfo);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(btnAddModule);

            tablePanel = new Panel();
            tablePanel.BackColor = Color.White;
            tablePanel.BorderStyle = BorderStyle.FixedSingle;
            tablePanel.Dock = DockStyle.Fill;
            tablePanel.Padding = new Padding(30, 20, 30, 20);

            dgvModules = new DataGridView();
            dgvModules.Dock = DockStyle.Fill;
            dgvModules.BackgroundColor = Color.White;
            dgvModules.BorderStyle = BorderStyle.None;
            dgvModules.RowHeadersVisible = false;
            dgvModules.AllowUserToAddRows = false;
            dgvModules.AllowUserToResizeRows = false;
            dgvModules.AllowUserToResizeColumns = false;
            dgvModules.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvModules.MultiSelect = false;
            dgvModules.ReadOnly = true;
            dgvModules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvModules.EnableHeadersVisualStyles = false;
            dgvModules.ColumnHeadersHeight = 38;
            dgvModules.RowTemplate.Height = 38;
            dgvModules.GridColor = Color.Gainsboro;
            dgvModules.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvModules.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgvModules.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvModules.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvModules.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvModules.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
            dgvModules.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            dgvModules.DefaultCellStyle.BackColor = Color.White;
            dgvModules.DefaultCellStyle.ForeColor = Color.Black;
            dgvModules.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvModules.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvModules.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvModules.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            dgvModules.Columns.Add("ModuleID", "Module ID");
            dgvModules.Columns.Add("ModuleName", "Module Name");
            dgvModules.Columns.Add("Description", "Description");
            dgvModules.Columns.Add("Status", "Status");
            dgvModules.Columns.Add("Scope", "Scope");

            DataGridViewTextBoxColumn actionCol = new DataGridViewTextBoxColumn();
            actionCol.Name = "Actions";
            actionCol.HeaderText = "Actions";
            actionCol.Width = 170;
            actionCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            actionCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            actionCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvModules.Columns.Add(actionCol);

            foreach (DataGridViewColumn col in dgvModules.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvModules.Columns["ModuleID"].FillWeight = 14;
            dgvModules.Columns["ModuleName"].FillWeight = 18;
            dgvModules.Columns["Description"].FillWeight = 30;
            dgvModules.Columns["Status"].FillWeight = 12;
            dgvModules.Columns["Scope"].FillWeight = 14;
            dgvModules.Columns["Actions"].FillWeight = 12;

            dgvModules.CellClick += DgvModules_CellClick;

            tablePanel.Controls.Add(dgvModules);

            Controls.Add(tablePanel);
            Controls.Add(topPanel);

            Resize += ModulesForm_Resize;
            AdjustTopControls();
        }

        private void ModulesForm_Resize(object? sender, EventArgs e)
        {
            AdjustTopControls();
        }

        private void AdjustTopControls()
        {
            if (btnAddModule == null) return;
            btnAddModule.Location = new Point(ClientSize.Width - btnAddModule.Width - 30, 70);
        }

        private void SeedModulesOnce()
        {
            if (ModuleStore.IsSeeded) return;

            ModuleStore.Modules.Add(new ModuleItem
            {
                ModuleID = "MOD001",
                ModuleName = "Book Catalog",
                Description = "Manage all books and catalog records",
                Status = "Enabled",
                Scope = "Global"
            });

            ModuleStore.Modules.Add(new ModuleItem
            {
                ModuleID = "MOD002",
                ModuleName = "Members",
                Description = "Manage library member accounts",
                Status = "Enabled",
                Scope = "Client-based"
            });

            ModuleStore.Modules.Add(new ModuleItem
            {
                ModuleID = "MOD003",
                ModuleName = "Staff",
                Description = "Manage librarian and staff accounts",
                Status = "Enabled",
                Scope = "Client-based"
            });

            ModuleStore.Modules.Add(new ModuleItem
            {
                ModuleID = "MOD004",
                ModuleName = "Borrowing",
                Description = "Borrowing transactions and workflows",
                Status = "Enabled",
                Scope = "Client-based"
            });

            ModuleStore.Modules.Add(new ModuleItem
            {
                ModuleID = "MOD005",
                ModuleName = "Returning",
                Description = "Book returns and processing",
                Status = "Enabled",
                Scope = "Client-based"
            });

            ModuleStore.Modules.Add(new ModuleItem
            {
                ModuleID = "MOD006",
                ModuleName = "Fines",
                Description = "Fine calculation and management",
                Status = "Disabled",
                Scope = "Client-based"
            });

            ModuleStore.Modules.Add(new ModuleItem
            {
                ModuleID = "MOD007",
                ModuleName = "Reports",
                Description = "Generate reports and summaries",
                Status = "Enabled",
                Scope = "Global"
            });

            ModuleStore.Modules.Add(new ModuleItem
            {
                ModuleID = "MOD008",
                ModuleName = "Notifications",
                Description = "System and user notifications",
                Status = "Disabled",
                Scope = "Global"
            });

            ModuleStore.IsSeeded = true;
        }

        private void LoadModulesData()
        {
            dgvModules.Rows.Clear();

            foreach (var module in ModuleStore.Modules)
            {
                dgvModules.Rows.Add(
                    module.ModuleID,
                    module.ModuleName,
                    module.Description,
                    module.Status,
                    module.Scope,
                    module.Status == "Enabled"
                        ? "✎ Edit    ⛔ Disable"
                        : "✎ Edit    ✅ Enable"
                );
            }

            dgvModules.ClearSelection();
        }

        private string GenerateNextModuleId()
        {
            if (!ModuleStore.Modules.Any())
                return "MOD001";

            int max = ModuleStore.Modules
                .Select(m =>
                {
                    string numberPart = m.ModuleID.Replace("MOD", "");
                    return int.TryParse(numberPart, out int n) ? n : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            return "MOD" + (max + 1).ToString("D3");
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            dgvModules.Rows.Clear();

            var filtered = ModuleStore.Modules
                .Where(m =>
                    m.ModuleID.ToLower().Contains(keyword) ||
                    m.ModuleName.ToLower().Contains(keyword) ||
                    m.Description.ToLower().Contains(keyword) ||
                    m.Status.ToLower().Contains(keyword) ||
                    m.Scope.ToLower().Contains(keyword))
                .ToList();

            foreach (var module in filtered)
            {
                dgvModules.Rows.Add(
                    module.ModuleID,
                    module.ModuleName,
                    module.Description,
                    module.Status,
                    module.Scope,
                    module.Status == "Enabled"
                        ? "✎ Edit    ⛔ Disable"
                        : "✎ Edit    ✅ Enable"
                );
            }

            dgvModules.ClearSelection();
        }

        private void BtnAddModule_Click(object? sender, EventArgs e)
        {
            string newId = GenerateNextModuleId();

            using (var dialog = new ModuleDialogForm("Add Module", newId))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ModuleStore.Modules.Add(new ModuleItem
                    {
                        ModuleID = dialog.ModuleIDValue,
                        ModuleName = dialog.ModuleNameValue,
                        Description = dialog.DescriptionValue,
                        Status = dialog.StatusValue,
                        Scope = dialog.ScopeValue
                    });

                    LoadModulesData();
                }
            }
        }

        private void EditModule(string moduleId)
        {
            var module = ModuleStore.Modules.FirstOrDefault(m => m.ModuleID == moduleId);
            if (module == null) return;

            using (var dialog = new ModuleDialogForm("Edit Module", module.ModuleID, module))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    module.ModuleName = dialog.ModuleNameValue;
                    module.Description = dialog.DescriptionValue;
                    module.Status = dialog.StatusValue;
                    module.Scope = dialog.ScopeValue;

                    LoadModulesData();
                }
            }
        }

        private void ToggleModuleStatus(string moduleId)
        {
            var module = ModuleStore.Modules.FirstOrDefault(m => m.ModuleID == moduleId);
            if (module == null) return;

            bool willEnable = module.Status == "Disabled";

            DialogResult result = MessageBox.Show(
                willEnable
                    ? $"Are you sure you want to enable '{module.ModuleName}'?"
                    : $"Are you sure you want to disable '{module.ModuleName}'?",
                willEnable ? "Enable Module" : "Disable Module",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                module.Status = willEnable ? "Enabled" : "Disabled";
                LoadModulesData();
            }
        }

        private void DgvModules_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvModules.Columns[e.ColumnIndex].Name != "Actions") return;

            Rectangle cellRect = dgvModules.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            int clickX = dgvModules.PointToClient(Cursor.Position).X - cellRect.Left;

            string moduleId = dgvModules.Rows[e.RowIndex].Cells["ModuleID"].Value?.ToString() ?? "";

            if (clickX < cellRect.Width / 2)
            {
                EditModule(moduleId);
            }
            else
            {
                ToggleModuleStatus(moduleId);
            }
        }
    }
}