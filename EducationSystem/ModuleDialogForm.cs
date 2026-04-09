using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ModuleDialogForm : Form
    {
        private Label lblTitle = null!;
        private Label lblModuleId = null!;
        private Label lblModuleName = null!;
        private Label lblDescription = null!;
        private Label lblStatus = null!;
        private Label lblScope = null!;

        private TextBox txtModuleId = null!;
        private TextBox txtModuleName = null!;
        private TextBox txtDescription = null!;
        private ComboBox cmbStatus = null!;
        private ComboBox cmbScope = null!;

        private Button btnSave = null!;
        private Button btnCancel = null!;

        public string ModuleIDValue => txtModuleId.Text.Trim();
        public string ModuleNameValue => txtModuleName.Text.Trim();
        public string DescriptionValue => txtDescription.Text.Trim();
        public string StatusValue => cmbStatus.SelectedItem?.ToString() ?? "Enabled";
        public string ScopeValue => cmbScope.SelectedItem?.ToString() ?? "Global";

        public ModuleDialogForm(string formTitle, string autoModuleId, ModuleItem? existingModule = null)
        {
            InitializeComponent();
            BuildUI(formTitle);

            if (existingModule == null)
            {
                txtModuleId.Text = autoModuleId;
                cmbStatus.SelectedIndex = 0;
                cmbScope.SelectedIndex = 0;
            }
            else
            {
                txtModuleId.Text = existingModule.ModuleID;
                txtModuleName.Text = existingModule.ModuleName;
                txtDescription.Text = existingModule.Description;
                cmbStatus.SelectedItem = existingModule.Status;
                cmbScope.SelectedItem = existingModule.Scope;
            }
        }

        private void StyleButton(Button btn, Color baseColor, Color hoverColor)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.UseVisualStyleBackColor = false;
            btn.BackColor = baseColor;
            btn.ForeColor = Color.White;

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = hoverColor;
                if (baseColor == Color.Maroon)
                    btn.ForeColor = Color.Black;
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = baseColor;
                btn.ForeColor = Color.White;
            };
        }

        private void BuildUI(string formTitle)
        {
            Text = formTitle;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(470, 340);
            BackColor = Color.Snow;

            lblTitle = new Label();
            lblTitle.Text = formTitle;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(25, 20);

            lblModuleId = new Label();
            lblModuleId.Text = "Module ID";
            lblModuleId.Location = new Point(30, 70);
            lblModuleId.AutoSize = true;

            txtModuleId = new TextBox();
            txtModuleId.Location = new Point(170, 67);
            txtModuleId.Size = new Size(230, 27);
            txtModuleId.ReadOnly = true;
            txtModuleId.BackColor = Color.WhiteSmoke;

            lblModuleName = new Label();
            lblModuleName.Text = "Module Name";
            lblModuleName.Location = new Point(30, 110);
            lblModuleName.AutoSize = true;

            txtModuleName = new TextBox();
            txtModuleName.Location = new Point(170, 107);
            txtModuleName.Size = new Size(230, 27);

            lblDescription = new Label();
            lblDescription.Text = "Description";
            lblDescription.Location = new Point(30, 150);
            lblDescription.AutoSize = true;

            txtDescription = new TextBox();
            txtDescription.Location = new Point(170, 147);
            txtDescription.Size = new Size(230, 27);

            lblStatus = new Label();
            lblStatus.Text = "Status";
            lblStatus.Location = new Point(30, 190);
            lblStatus.AutoSize = true;

            cmbStatus = new ComboBox();
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Location = new Point(170, 187);
            cmbStatus.Size = new Size(230, 28);
            cmbStatus.Items.AddRange(new object[] { "Enabled", "Disabled" });

            lblScope = new Label();
            lblScope.Text = "Scope";
            lblScope.Location = new Point(30, 230);
            lblScope.AutoSize = true;

            cmbScope = new ComboBox();
            cmbScope.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScope.Location = new Point(170, 227);
            cmbScope.Size = new Size(230, 28);
            cmbScope.Items.AddRange(new object[] { "Global", "Client-based" });

            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(240, 280);
            btnSave.Size = new Size(75, 32);
            StyleButton(btnSave, Color.Maroon, Color.FromArgb(230, 230, 230));
            btnSave.Click += BtnSave_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(325, 280);
            btnCancel.Size = new Size(75, 32);
            StyleButton(btnCancel, Color.Gray, Color.Silver);
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.Add(lblTitle);
            Controls.Add(lblModuleId);
            Controls.Add(txtModuleId);
            Controls.Add(lblModuleName);
            Controls.Add(txtModuleName);
            Controls.Add(lblDescription);
            Controls.Add(txtDescription);
            Controls.Add(lblStatus);
            Controls.Add(cmbStatus);
            Controls.Add(lblScope);
            Controls.Add(cmbScope);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModuleName.Text))
            {
                MessageBox.Show("Please enter Module Name.");
                txtModuleName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter Description.");
                txtDescription.Focus();
                return;
            }

            if (cmbStatus.SelectedIndex < 0)
            {
                MessageBox.Show("Please select Status.");
                cmbStatus.Focus();
                return;
            }

            if (cmbScope.SelectedIndex < 0)
            {
                MessageBox.Show("Please select Scope.");
                cmbScope.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}