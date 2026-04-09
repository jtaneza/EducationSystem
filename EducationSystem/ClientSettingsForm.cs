using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ClientSettingsForm : Form
    {
        private Panel settingsPanel = null!;
        private Panel headerLine = null!;

        private Label lblHeader = null!;

        private PictureBox picProfile = null!;
        private Label lblProfileText = null!;
        private Button btnChangePhoto = null!;

        private Label lblName = null!;
        private Label lblUsername = null!;
        private Label lblEmail = null!;

        private TextBox txtName = null!;
        private TextBox txtUsername = null!;
        private TextBox txtEmail = null!;

        private Label lblCurrentPassword = null!;
        private Label lblNewPassword = null!;
        private Label lblConfirmPassword = null!;

        private TextBox txtCurrentPassword = null!;
        private TextBox txtNewPassword = null!;
        private TextBox txtConfirmPassword = null!;

        private Button btnUpdate = null!;
        private Button btnChangePassword = null!;
        private Button btnBack = null!;

        public ClientSettingsForm()
        {
            InitializeComponent();
            BuildUI();
            LoadClientData();
        }

        public void OpenPasswordSection()
        {
            txtCurrentPassword.Focus();
        }

        private void BuildUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            settingsPanel = new Panel();
            settingsPanel.Size = new Size(980, 420);
            settingsPanel.Location = new Point(70, 35);
            settingsPanel.BackColor = Color.White;
            settingsPanel.BorderStyle = BorderStyle.FixedSingle;

            lblHeader = new Label();
            lblHeader.Text = "◉ ACCOUNT SETTINGS";
            lblHeader.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblHeader.ForeColor = Color.Black;
            lblHeader.AutoSize = true;
            lblHeader.Location = new Point(22, 18);

            headerLine = new Panel();
            headerLine.BackColor = Color.Goldenrod;
            headerLine.Location = new Point(0, 42);
            headerLine.Size = new Size(settingsPanel.Width, 2);
            headerLine.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            picProfile = new PictureBox();
            picProfile.Size = new Size(86, 86);
            picProfile.Location = new Point(110, 96);
            picProfile.SizeMode = PictureBoxSizeMode.Zoom;
            picProfile.BackColor = Color.White;
            picProfile.Paint += PicProfile_Paint;

            lblProfileText = new Label();
            lblProfileText.Text = "Profile Picture";
            lblProfileText.Font = new Font("Segoe UI", 9);
            lblProfileText.ForeColor = Color.DimGray;
            lblProfileText.AutoSize = true;
            lblProfileText.Location = new Point(112, 192);

            btnChangePhoto = new Button();
            btnChangePhoto.Text = "Change";
            btnChangePhoto.BackColor = Color.Goldenrod;
            btnChangePhoto.ForeColor = Color.White;
            btnChangePhoto.FlatStyle = FlatStyle.Flat;
            btnChangePhoto.FlatAppearance.BorderSize = 0;
            btnChangePhoto.Size = new Size(90, 33);
            btnChangePhoto.Location = new Point(104, 226);
            btnChangePhoto.Cursor = Cursors.Hand;
            btnChangePhoto.Click += BtnChangePhoto_Click;

            int leftX = 280;
            int textWidth = 630;

            lblName = new Label();
            lblName.Text = "Name";
            lblName.ForeColor = Color.SaddleBrown;
            lblName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblName.AutoSize = true;
            lblName.Location = new Point(leftX, 72);

            txtName = new TextBox();
            txtName.Size = new Size(textWidth, 27);
            txtName.Location = new Point(leftX, 98);

            lblUsername = new Label();
            lblUsername.Text = "Username";
            lblUsername.ForeColor = Color.SaddleBrown;
            lblUsername.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(leftX, 128);

            txtUsername = new TextBox();
            txtUsername.Size = new Size(textWidth, 27);
            txtUsername.Location = new Point(leftX, 154);

            lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.ForeColor = Color.SaddleBrown;
            lblEmail.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(leftX, 184);

            txtEmail = new TextBox();
            txtEmail.Size = new Size(textWidth, 27);
            txtEmail.Location = new Point(leftX, 210);

            int passY = 270;
            int passWidth = 190;

            lblCurrentPassword = new Label();
            lblCurrentPassword.Text = "Current Password";
            lblCurrentPassword.ForeColor = Color.SaddleBrown;
            lblCurrentPassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCurrentPassword.AutoSize = true;
            lblCurrentPassword.Location = new Point(leftX, passY);

            txtCurrentPassword = new TextBox();
            txtCurrentPassword.Size = new Size(passWidth, 27);
            txtCurrentPassword.Location = new Point(leftX, passY + 24);
            txtCurrentPassword.UseSystemPasswordChar = true;

            lblNewPassword = new Label();
            lblNewPassword.Text = "New Password";
            lblNewPassword.ForeColor = Color.SaddleBrown;
            lblNewPassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblNewPassword.AutoSize = true;
            lblNewPassword.Location = new Point(leftX + 215, passY);

            txtNewPassword = new TextBox();
            txtNewPassword.Size = new Size(passWidth, 27);
            txtNewPassword.Location = new Point(leftX + 215, passY + 24);
            txtNewPassword.UseSystemPasswordChar = true;

            lblConfirmPassword = new Label();
            lblConfirmPassword.Text = "Confirm Password";
            lblConfirmPassword.ForeColor = Color.SaddleBrown;
            lblConfirmPassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new Point(leftX + 440, passY);

            txtConfirmPassword = new TextBox();
            txtConfirmPassword.Size = new Size(passWidth, 27);
            txtConfirmPassword.Location = new Point(leftX + 440, passY + 24);
            txtConfirmPassword.UseSystemPasswordChar = true;

            btnUpdate = new Button();
            btnUpdate.Text = "Update";
            btnUpdate.BackColor = Color.Maroon;
            btnUpdate.ForeColor = Color.White;
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Size = new Size(100, 35);
            btnUpdate.Location = new Point(620, 346);
            btnUpdate.Cursor = Cursors.Hand;
            btnUpdate.Click += BtnUpdate_Click;

            btnChangePassword = new Button();
            btnChangePassword.Text = "Change Password";
            btnChangePassword.BackColor = Color.Maroon;
            btnChangePassword.ForeColor = Color.White;
            btnChangePassword.FlatStyle = FlatStyle.Flat;
            btnChangePassword.FlatAppearance.BorderSize = 0;
            btnChangePassword.Size = new Size(150, 35);
            btnChangePassword.Location = new Point(730, 346);
            btnChangePassword.Cursor = Cursors.Hand;
            btnChangePassword.Click += BtnChangePassword_Click;

            btnBack = new Button();
            btnBack.Text = "Back";
            btnBack.BackColor = Color.Maroon;
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Size = new Size(80, 35);
            btnBack.Location = new Point(890, 346);
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += BtnBack_Click;

            settingsPanel.Controls.Add(lblHeader);
            settingsPanel.Controls.Add(headerLine);
            settingsPanel.Controls.Add(picProfile);
            settingsPanel.Controls.Add(lblProfileText);
            settingsPanel.Controls.Add(btnChangePhoto);

            settingsPanel.Controls.Add(lblName);
            settingsPanel.Controls.Add(txtName);
            settingsPanel.Controls.Add(lblUsername);
            settingsPanel.Controls.Add(txtUsername);
            settingsPanel.Controls.Add(lblEmail);
            settingsPanel.Controls.Add(txtEmail);

            settingsPanel.Controls.Add(lblCurrentPassword);
            settingsPanel.Controls.Add(txtCurrentPassword);
            settingsPanel.Controls.Add(lblNewPassword);
            settingsPanel.Controls.Add(txtNewPassword);
            settingsPanel.Controls.Add(lblConfirmPassword);
            settingsPanel.Controls.Add(txtConfirmPassword);

            settingsPanel.Controls.Add(btnUpdate);
            settingsPanel.Controls.Add(btnChangePassword);
            settingsPanel.Controls.Add(btnBack);

            Controls.Add(settingsPanel);
        }

        private void LoadClientData()
        {
            string displayName = string.IsNullOrWhiteSpace(ClientSession.LibraryName)
                ? "Client Admin"
                : ClientSession.LibraryName!;

            txtName.Text = displayName;
            txtUsername.Text = displayName;
            txtEmail.Text = string.IsNullOrWhiteSpace(ClientSession.Email) ? "" : ClientSession.Email!;

            try
            {
                if (!string.IsNullOrWhiteSpace(ClientSession.ImagePath) && File.Exists(ClientSession.ImagePath))
                {
                    using (var bmpTemp = new Bitmap(ClientSession.ImagePath))
                    {
                        picProfile.Image = new Bitmap(bmpTemp);
                    }
                }
                else
                {
                    string defaultPath = Path.Combine(Application.StartupPath, "Assets", "client.png");
                    if (File.Exists(defaultPath))
                    {
                        using (var bmpTemp = new Bitmap(defaultPath))
                        {
                            picProfile.Image = new Bitmap(bmpTemp);
                        }
                    }
                    else
                    {
                        picProfile.Image = null;
                    }
                }
            }
            catch
            {
                picProfile.Image = null;
            }

            MakePictureCircular();
        }

        private void MakePictureCircular()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, picProfile.Width - 1, picProfile.Height - 1);
            picProfile.Region = new Region(path);
        }

        private void PicProfile_Paint(object? sender, PaintEventArgs e)
        {
            using Pen pen = new Pen(Color.FromArgb(180, 60, 255), 2);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawEllipse(pen, 1, 1, picProfile.Width - 3, picProfile.Height - 3);
        }

        private void BtnChangePhoto_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select Profile Picture";
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string assetsFolder = Path.Combine(Application.StartupPath, "Assets", "ClientProfiles");
                Directory.CreateDirectory(assetsFolder);

                string extension = Path.GetExtension(ofd.FileName);
                string fileName = "client_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                string destinationPath = Path.Combine(assetsFolder, fileName);

                File.Copy(ofd.FileName, destinationPath, true);
                ClientSession.ImagePath = destinationPath;

                if (picProfile.Image != null)
                {
                    var old = picProfile.Image;
                    picProfile.Image = null;
                    old.Dispose();
                }

                using (var bmpTemp = new Bitmap(destinationPath))
                {
                    picProfile.Image = new Bitmap(bmpTemp);
                }

                MakePictureCircular();

                MessageBox.Show("Profile picture updated.");

                RefreshDashboardHeader();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update profile picture: " + ex.Message);
            }
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            string newName = txtName.Text.Trim();
            string newUsername = txtUsername.Text.Trim();
            string newEmail = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(newName) ||
                string.IsNullOrWhiteSpace(newUsername) ||
                string.IsNullOrWhiteSpace(newEmail))
            {
                MessageBox.Show("Please complete name, username, and email.");
                return;
            }

            ClientSession.LibraryName = newName;
            ClientSession.Username = newUsername;
            ClientSession.Email = newEmail;

            MessageBox.Show("Client account updated.");
            RefreshDashboardHeader();
        }

        private void BtnChangePassword_Click(object? sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please complete all password fields.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ClientSession.Password))
            {
                MessageBox.Show("No existing password found.");
                return;
            }

            if (currentPassword != ClientSession.Password)
            {
                MessageBox.Show("Current password is incorrect.");
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New password and confirm password do not match.");
                return;
            }

            if (newPassword == ClientSession.Password)
            {
                MessageBox.Show("New password must be different from current password.");
                return;
            }

            // Save
            ClientSession.Password = newPassword;

            txtCurrentPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();

            MessageBox.Show("Client password changed successfully.");
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            Form? parentForm = this.Parent?.FindForm();

            if (parentForm is ClientDashboardForm dashboard)
            {
                dashboard.LoadContentForm(new ClientProfileForm());
            }
        }

        private void RefreshDashboardHeader()
        {
            Form? parentForm = this.Parent?.FindForm();

            if (parentForm is ClientDashboardForm dashboard)
            {
                dashboard.LoadUserInfo();
            }
        }
    }
}