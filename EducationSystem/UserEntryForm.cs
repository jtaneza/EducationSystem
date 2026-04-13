using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class UserEntryForm : Form
    {
        private Label lblTitle;
        private Label lblUsername;
        private Label lblEmail;
        private Label lblPassword;
        private Label lblPicture;

        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtPicture;

        private Button btnBrowse;
        private Button btnSave;
        private Button btnCancel;

        public string UsernameValue
        {
            get => txtUsername.Text.Trim();
            set => txtUsername.Text = value;
        }

        public string EmailValue
        {
            get => txtEmail.Text.Trim();
            set => txtEmail.Text = value;
        }

        public string PasswordValue
        {
            get => txtPassword.Text;
            set => txtPassword.Text = value;
        }

        public string ProfilePictureFileName
        {
            get => txtPicture.Text.Trim();
            set => txtPicture.Text = value;
        }

        public UserEntryForm(string title)
        {
            InitializeComponent();
            BuildUI(title);
        }

        public void LoadValues()
        {
            txtUsername.Text = UsernameValue;
            txtEmail.Text = EmailValue;
            txtPicture.Text = ProfilePictureFileName;
        }

        private void BuildUI(string title)
        {
            Text = title;
            BackColor = Color.Snow;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(500, 320);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);

            lblUsername = new Label() { Text = "Username", Location = new Point(20, 70), AutoSize = true };
            lblEmail = new Label() { Text = "Email", Location = new Point(20, 110), AutoSize = true };
            lblPassword = new Label() { Text = "Password", Location = new Point(20, 150), AutoSize = true };
            lblPicture = new Label() { Text = "Profile Picture", Location = new Point(20, 190), AutoSize = true };

            txtUsername = new TextBox() { Location = new Point(130, 66), Width = 250 };
            txtEmail = new TextBox() { Location = new Point(130, 106), Width = 250 };
            txtPassword = new TextBox() { Location = new Point(130, 146), Width = 250, UseSystemPasswordChar = true };
            txtPicture = new TextBox() { Location = new Point(130, 186), Width = 170, ReadOnly = true };

            btnBrowse = new Button()
            {
                Text = "Browse",
                Location = new Point(310, 184),
                Size = new Size(70, 28)
            };
            btnBrowse.Click += BtnBrowse_Click;

            btnSave = new Button()
            {
                Text = "Save",
                Location = new Point(220, 235),
                Size = new Size(75, 32),
                BackColor = Color.Maroon,
                ForeColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button()
            {
                Text = "Cancel",
                Location = new Point(305, 235),
                Size = new Size(75, 32)
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(lblEmail);
            Controls.Add(lblPassword);
            Controls.Add(lblPicture);
            Controls.Add(txtUsername);
            Controls.Add(txtEmail);
            Controls.Add(txtPassword);
            Controls.Add(txtPicture);
            Controls.Add(btnBrowse);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPicture.Text = System.IO.Path.GetFileName(ofd.FileName);
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required.");
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}