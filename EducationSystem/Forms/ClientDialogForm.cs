using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public class ClientDialogForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color Outline = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color Primary = ColorTranslator.FromHtml("#00B894");
        private readonly Color PrimaryDark = ColorTranslator.FromHtml("#006B55");
        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#6B7E95");

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

        private void BuildUI(string formTitle)
        {
            Text = formTitle;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(560, 470);
            BackColor = Background;
            Font = new Font("Segoe UI", 10F);

            Panel card = new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(18, 18),
                Size = new Size(524, 434)
            };
            Controls.Add(card);

            lblTitle = new Label
            {
                Text = formTitle,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(28, 24)
            };

            lblClientId = CreateLabel("Client ID", 28, 88);
            lblLibraryName = CreateLabel("Library Name", 28, 138);
            lblEmail = CreateLabel("Email", 28, 188);
            lblPassword = CreateLabel("Password", 28, 238);
            lblConfirmPassword = CreateLabel("Confirm Password", 28, 288);
            lblStatus = CreateLabel("Status", 28, 338);

            txtClientId = CreateTextBox(170, 84, 300);
            txtClientId.ReadOnly = true;
            txtClientId.BackColor = SurfaceLow;

            txtLibraryName = CreateTextBox(170, 134, 300);
            txtEmail = CreateTextBox(170, 184, 300);

            txtPassword = CreateTextBox(170, 234, 260);
            txtPassword.UseSystemPasswordChar = true;

            btnShowPassword = CreateEyeButton(436, 234);
            btnShowPassword.Click += TogglePasswordVisibility;

            txtConfirmPassword = CreateTextBox(170, 284, 260);
            txtConfirmPassword.UseSystemPasswordChar = true;

            btnShowConfirmPassword = CreateEyeButton(436, 284);
            btnShowConfirmPassword.Click += ToggleConfirmPasswordVisibility;

            cmbStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(170, 334),
                Size = new Size(300, 34),
                Font = new Font("Segoe UI", 10F),
                BackColor = Surface,
                ForeColor = OnSurface,
                FlatStyle = FlatStyle.Flat
            };
            cmbStatus.Items.AddRange(new object[] { "Active", "Inactive" });

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(314, 386),
                Size = new Size(74, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Primary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(396, 386),
                Size = new Size(74, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#8C98A4"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            AcceptButton = btnSave;
            CancelButton = btnCancel;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblClientId);
            card.Controls.Add(lblLibraryName);
            card.Controls.Add(lblEmail);
            card.Controls.Add(lblPassword);
            card.Controls.Add(lblConfirmPassword);
            card.Controls.Add(lblStatus);

            card.Controls.Add(txtClientId);
            card.Controls.Add(txtLibraryName);
            card.Controls.Add(txtEmail);
            card.Controls.Add(txtPassword);
            card.Controls.Add(btnShowPassword);
            card.Controls.Add(txtConfirmPassword);
            card.Controls.Add(btnShowConfirmPassword);
            card.Controls.Add(cmbStatus);

            card.Controls.Add(btnSave);
            card.Controls.Add(btnCancel);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = OnSurface
            };
        }

        private TextBox CreateTextBox(int x, int y, int width)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 32),
                Font = new Font("Segoe UI", 10F),
                ForeColor = OnSurface,
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle
            };
            return txt;
        }

        private Button CreateEyeButton(int x, int y)
        {
            Button btn = new Button
            {
                Text = "👁",
                Location = new Point(x, y),
                Size = new Size(34, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = SurfaceLow,
                ForeColor = SecondaryText,
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btn.FlatAppearance.BorderColor = Outline;
            btn.FlatAppearance.BorderSize = 1;
            return btn;
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