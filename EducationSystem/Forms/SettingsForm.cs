using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class SettingsForm : Form
    {
        private Panel mainPanel = null!;

        private Label lblTitle = null!;
        private Panel titleLine = null!;

        private PictureBox picProfile = null!;
        private Label lblPhotoNote = null!;
        private Button btnBrowsePhoto = null!;

        private Label lblName = null!;
        private Label lblUsername = null!;
        private Label lblEmail = null!;
        private Label lblCurrentPassword = null!;
        private Label lblNewPassword = null!;
        private Label lblConfirmPassword = null!;

        private TextBox txtName = null!;
        private TextBox txtUsername = null!;
        private TextBox txtEmail = null!;
        private TextBox txtCurrentPassword = null!;
        private TextBox txtNewPassword = null!;
        private TextBox txtConfirmPassword = null!;

        private Button btnUpdate = null!;
        private Button btnChangePassword = null!;
        private Button btnBack = null!;

        private string selectedImagePath = "";
        private bool focusPasswordSection = false;

        public SettingsForm()
        {
            InitializeComponent();
            BuildSettingsUI();
            LoadSettingsData();
        }

        public void OpenPasswordSection()
        {
            focusPasswordSection = true;
        }

        private void StylePrimaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(130, 0, 0);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(90, 0, 0);
            btn.BackColor = Color.Maroon;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleGoldButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 150, 0);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(170, 120, 0);
            btn.BackColor = Color.Goldenrod;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private Label CreateFieldLabel(string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lbl.ForeColor = Color.Sienna;
            return lbl;
        }

        private TextBox CreateFieldBox(int x, int y, int width)
        {
            TextBox txt = new TextBox();
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 25);
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            return txt;
        }

        private void BuildSettingsUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = true;

            mainPanel = new Panel();
            mainPanel.Location = new Point(70, 30);
            mainPanel.Size = new Size(980, 420);
            mainPanel.BackColor = Color.White;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;

            lblTitle = new Label();
            lblTitle.Text = "◉ ACCOUNT SETTINGS";
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(18, 14);

            titleLine = new Panel();
            titleLine.BackColor = Color.Goldenrod;
            titleLine.Location = new Point(0, 42);
            titleLine.Size = new Size(mainPanel.Width, 2);

            picProfile = new PictureBox();
            picProfile.Size = new Size(88, 88);
            picProfile.Location = new Point(110, 95);
            picProfile.SizeMode = PictureBoxSizeMode.Zoom;
            picProfile.BackColor = Color.White;
            picProfile.Paint += PicProfile_Paint;

            lblPhotoNote = new Label();
            lblPhotoNote.Text = "Profile Picture";
            lblPhotoNote.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            lblPhotoNote.ForeColor = Color.DimGray;
            lblPhotoNote.AutoSize = true;
            lblPhotoNote.Location = new Point(108, 190);

            btnBrowsePhoto = new Button();
            btnBrowsePhoto.Text = "Change";
            btnBrowsePhoto.Size = new Size(90, 32);
            btnBrowsePhoto.Location = new Point(105, 220);
            StyleGoldButton(btnBrowsePhoto);
            btnBrowsePhoto.Click += BtnBrowsePhoto_Click;

            int formX = 280;
            int fieldWidth = 630;

            lblName = CreateFieldLabel("Name", formX, 70);
            txtName = CreateFieldBox(formX, 88, fieldWidth);

            lblUsername = CreateFieldLabel("Username", formX, 125);
            txtUsername = CreateFieldBox(formX, 143, fieldWidth);

            lblEmail = CreateFieldLabel("Email", formX, 180);
            txtEmail = CreateFieldBox(formX, 198, fieldWidth);

            int passY = 255;

            lblCurrentPassword = CreateFieldLabel("Current Password", formX, passY);
            txtCurrentPassword = CreateFieldBox(formX, passY + 18, 190);
            txtCurrentPassword.UseSystemPasswordChar = true;

            lblNewPassword = CreateFieldLabel("New Password", formX + 215, passY);
            txtNewPassword = CreateFieldBox(formX + 215, passY + 18, 190);
            txtNewPassword.UseSystemPasswordChar = true;

            lblConfirmPassword = CreateFieldLabel("Confirm Password", formX + 440, passY);
            txtConfirmPassword = CreateFieldBox(formX + 440, passY + 18, 190);
            txtConfirmPassword.UseSystemPasswordChar = true;

            int btnY = 345;

            btnUpdate = new Button();
            btnUpdate.Text = "Update";
            btnUpdate.Size = new Size(100, 34);
            btnUpdate.Location = new Point(620, btnY);
            StylePrimaryButton(btnUpdate);
            btnUpdate.Click += BtnUpdate_Click;

            btnChangePassword = new Button();
            btnChangePassword.Text = "Change Password";
            btnChangePassword.Size = new Size(150, 34);
            btnChangePassword.Location = new Point(730, btnY);
            StylePrimaryButton(btnChangePassword);
            btnChangePassword.Click += BtnChangePassword_Click;

            btnBack = new Button();
            btnBack.Text = "Back";
            btnBack.Size = new Size(80, 34);
            btnBack.Location = new Point(890, btnY);
            StylePrimaryButton(btnBack);
            btnBack.Click += BtnBack_Click;

            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(titleLine);

            mainPanel.Controls.Add(picProfile);
            mainPanel.Controls.Add(lblPhotoNote);
            mainPanel.Controls.Add(btnBrowsePhoto);

            mainPanel.Controls.Add(lblName);
            mainPanel.Controls.Add(txtName);
            mainPanel.Controls.Add(lblUsername);
            mainPanel.Controls.Add(txtUsername);
            mainPanel.Controls.Add(lblEmail);
            mainPanel.Controls.Add(txtEmail);

            mainPanel.Controls.Add(lblCurrentPassword);
            mainPanel.Controls.Add(txtCurrentPassword);
            mainPanel.Controls.Add(lblNewPassword);
            mainPanel.Controls.Add(txtNewPassword);
            mainPanel.Controls.Add(lblConfirmPassword);
            mainPanel.Controls.Add(txtConfirmPassword);

            mainPanel.Controls.Add(btnUpdate);
            mainPanel.Controls.Add(btnChangePassword);
            mainPanel.Controls.Add(btnBack);

            Controls.Add(mainPanel);

            Shown += SettingsForm_Shown;
        }

        private void LoadSettingsData()
        {
            txtName.Text = string.IsNullOrWhiteSpace(UserSession.Role) ? "Admin" : UserSession.Role;
            txtUsername.Text = string.IsNullOrWhiteSpace(UserSession.Username) ? "admin" : UserSession.Username;
            txtEmail.Text = string.IsNullOrWhiteSpace(UserSession.Email) ? "supadmin@gmail.com" : UserSession.Email;

            selectedImagePath = UserSession.ImagePath ?? "";

            try
            {
                if (!string.IsNullOrWhiteSpace(UserSession.ImagePath) && File.Exists(UserSession.ImagePath))
                {
                    using (var bmpTemp = new Bitmap(UserSession.ImagePath))
                    {
                        picProfile.Image = new Bitmap(bmpTemp);
                    }
                }
                else
                {
                    picProfile.Image = null;
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

        private void SettingsForm_Shown(object? sender, EventArgs e)
        {
            if (focusPasswordSection)
            {
                txtCurrentPassword.Focus();
            }
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            Form? parentForm = this.Parent?.FindForm();
            if (parentForm is DashboardForm dashboard)
            {
                dashboard.LoadContentForm(new ProfileForm());
            }
        }

        private void BtnBrowsePhoto_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Profile Picture";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;

                    try
                    {
                        using (var bmpTemp = new Bitmap(selectedImagePath))
                        {
                            picProfile.Image = new Bitmap(bmpTemp);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Unable to load selected image.");
                    }
                }
            }
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter name.");
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter username.");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter email.");
                txtEmail.Focus();
                return;
            }

            UserSession.Role = txtName.Text.Trim();
            UserSession.Username = txtUsername.Text.Trim();
            UserSession.Email = txtEmail.Text.Trim();

            if (!string.IsNullOrWhiteSpace(selectedImagePath))
            {
                UserSession.ImagePath = selectedImagePath;
            }

            Form? parentForm = this.Parent?.FindForm();
            if (parentForm is DashboardForm dashboard)
            {
                dashboard.LoadUserInfo();
            }

            MessageBox.Show("Account updated successfully.");
        }

        private void BtnChangePassword_Click(object? sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                MessageBox.Show("Please enter current password.");
                txtCurrentPassword.Focus();
                return;
            }

            if (currentPassword != (UserSession.Password ?? ""))
            {
                MessageBox.Show("Current password is incorrect.");
                txtCurrentPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Please enter new password.");
                txtNewPassword.Focus();
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Confirm password does not match.");
                txtConfirmPassword.Focus();
                return;
            }

            UserSession.Password = newPassword;
            txtCurrentPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();

            MessageBox.Show("Password changed successfully.");
        }
    }
}