using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public class ClientDialogForm : Form
    {
        private Label lblTitle = null!;
        private Label lblClientId = null!;
        private Label lblLibraryName = null!;
        private Label lblEmail = null!;
        private Label lblPassword = null!;
        private Label lblConfirmPassword = null!;
        private Label lblStatus = null!;

        private TextBox txtClientId = null!;
        private TextBox txtLibraryName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPassword = null!;
        private TextBox txtConfirmPassword = null!;
        private ComboBox cmbStatus = null!;

        private Button btnSave = null!;
        private Button btnCancel = null!;
        private Button btnShowPassword = null!;
        private Button btnShowConfirmPassword = null!;

        private bool isPasswordVisible = false;
        private bool isConfirmPasswordVisible = false;

        public string ClientIDValue => txtClientId.Text.Trim();
        public string LibraryNameValue => txtLibraryName.Text.Trim();
        public string EmailValue => txtEmail.Text.Trim();
        public string PasswordValue => txtPassword.Text;
        public string StatusValue => cmbStatus.SelectedItem?.ToString() ?? "Active";

        public ClientDialogForm(string formTitle, string autoClientId, ClientItem? existingClient = null)
        {
            BuildUI(formTitle);

            if (existingClient == null)
            {
                txtClientId.Text = autoClientId;
                cmbStatus.SelectedIndex = 0;
            }
            else
            {
                txtClientId.Text = existingClient.ClientID;
                txtLibraryName.Text = existingClient.LibraryName;
                txtEmail.Text = existingClient.Email;
                txtPassword.Text = existingClient.Password;
                txtConfirmPassword.Text = existingClient.Password;
                cmbStatus.SelectedItem = existingClient.Status;
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
            ClientSize = new Size(470, 380);
            BackColor = Color.Snow;

            lblTitle = new Label();
            lblTitle.Text = formTitle;
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(25, 20);

            lblClientId = new Label();
            lblClientId.Text = "Client ID";
            lblClientId.Location = new Point(30, 70);
            lblClientId.AutoSize = true;

            txtClientId = new TextBox();
            txtClientId.Location = new Point(170, 67);
            txtClientId.Size = new Size(230, 27);
            txtClientId.ReadOnly = true;
            txtClientId.BackColor = Color.WhiteSmoke;

            lblLibraryName = new Label();
            lblLibraryName.Text = "Library Name";
            lblLibraryName.Location = new Point(30, 110);
            lblLibraryName.AutoSize = true;

            txtLibraryName = new TextBox();
            txtLibraryName.Location = new Point(170, 107);
            txtLibraryName.Size = new Size(230, 27);

            lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.Location = new Point(30, 150);
            lblEmail.AutoSize = true;

            txtEmail = new TextBox();
            txtEmail.Location = new Point(170, 147);
            txtEmail.Size = new Size(230, 27);

            lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Location = new Point(30, 190);
            lblPassword.AutoSize = true;

            txtPassword = new TextBox();
            txtPassword.Location = new Point(170, 187);
            txtPassword.Size = new Size(190, 27);
            txtPassword.UseSystemPasswordChar = true;

            btnShowPassword = new Button();
            btnShowPassword.Text = "👁";
            btnShowPassword.Location = new Point(365, 186);
            btnShowPassword.Size = new Size(35, 29);
            btnShowPassword.FlatStyle = FlatStyle.Flat;
            btnShowPassword.FlatAppearance.BorderSize = 0;
            btnShowPassword.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnShowPassword.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnShowPassword.BackColor = Color.Transparent;
            btnShowPassword.Cursor = Cursors.Hand;
            btnShowPassword.TabStop = false;
            btnShowPassword.Click += TogglePasswordVisibility;

            lblConfirmPassword = new Label();
            lblConfirmPassword.Text = "Confirm Password";
            lblConfirmPassword.Location = new Point(30, 230);
            lblConfirmPassword.AutoSize = true;

            txtConfirmPassword = new TextBox();
            txtConfirmPassword.Location = new Point(170, 227);
            txtConfirmPassword.Size = new Size(190, 27);
            txtConfirmPassword.UseSystemPasswordChar = true;

            btnShowConfirmPassword = new Button();
            btnShowConfirmPassword.Text = "👁";
            btnShowConfirmPassword.Location = new Point(365, 226);
            btnShowConfirmPassword.Size = new Size(35, 29);
            btnShowConfirmPassword.FlatStyle = FlatStyle.Flat;
            btnShowConfirmPassword.FlatAppearance.BorderSize = 0;
            btnShowConfirmPassword.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnShowConfirmPassword.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnShowConfirmPassword.BackColor = Color.Transparent;
            btnShowConfirmPassword.Cursor = Cursors.Hand;
            btnShowConfirmPassword.TabStop = false;
            btnShowConfirmPassword.Click += ToggleConfirmPasswordVisibility;

            lblStatus = new Label();
            lblStatus.Text = "Status";
            lblStatus.Location = new Point(30, 270);
            lblStatus.AutoSize = true;

            cmbStatus = new ComboBox();
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Location = new Point(170, 267);
            cmbStatus.Size = new Size(230, 28);
            cmbStatus.Items.AddRange(new object[] { "Active", "Inactive" });

            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(240, 320);
            btnSave.Size = new Size(75, 32);
            StyleButton(btnSave, Color.Maroon, Color.FromArgb(230, 230, 230));
            btnSave.Click += BtnSave_Click;

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(325, 320);
            btnCancel.Size = new Size(75, 32);
            StyleButton(btnCancel, Color.Gray, Color.Silver);
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.Add(lblTitle);
            Controls.Add(lblClientId);
            Controls.Add(txtClientId);
            Controls.Add(lblLibraryName);
            Controls.Add(txtLibraryName);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnShowPassword);
            Controls.Add(lblConfirmPassword);
            Controls.Add(txtConfirmPassword);
            Controls.Add(btnShowConfirmPassword);
            Controls.Add(lblStatus);
            Controls.Add(cmbStatus);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void TogglePasswordVisibility(object? sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            txtPassword.UseSystemPasswordChar = !isPasswordVisible;
            btnShowPassword.Text = isPasswordVisible ? "🙈" : "👁";
        }

        private void ToggleConfirmPasswordVisibility(object? sender, EventArgs e)
        {
            isConfirmPasswordVisible = !isConfirmPasswordVisible;
            txtConfirmPassword.UseSystemPasswordChar = !isConfirmPasswordVisible;
            btnShowConfirmPassword.Text = isConfirmPasswordVisible ? "🙈" : "👁";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLibraryName.Text))
            {
                MessageBox.Show("Please enter Library Name.");
                txtLibraryName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter Email.");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter Password.");
                txtPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Please confirm Password.");
                txtConfirmPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Password and Confirm Password do not match.");
                txtConfirmPassword.Focus();
                return;
            }

            if (cmbStatus.SelectedIndex < 0)
            {
                MessageBox.Show("Please select Status.");
                cmbStatus.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}