using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ProfileForm : Form
    {
        private readonly Color PageBack = ColorTranslator.FromHtml("#FBF7F7");
        private readonly Color Navy = ColorTranslator.FromHtml("#081D38");
        private readonly Color Accent = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDark = ColorTranslator.FromHtml("#006B55");
        private readonly Color Border = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color FieldBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color TextDark = ColorTranslator.FromHtml("#161D1F");
        private readonly Color TextMuted = ColorTranslator.FromHtml("#64748B");

        private Panel cardPanel = null!;
        private Panel topCardPanel = null!;
        private Panel formPanel = null!;

        private PictureBox picProfile = null!;
        private Button btnUploadImage = null!;

        private Label lblDisplayName = null!;
        private Label lblUsername = null!;

        private TextBox txtFullName = null!;
        private TextBox txtUsername = null!;
        private TextBox txtEmail = null!;
        private TextBox txtCurrentPassword = null!;
        private TextBox txtNewPassword = null!;
        private TextBox txtConfirmPassword = null!;

        private Button btnSaveProfile = null!;
        private Button btnChangePassword = null!;

        private string selectedImagePath = "";

        public ProfileForm()
        {
            InitializeComponent();
            BuildProfileUI();
            LoadProfileData();
        }

        private void BuildProfileUI()
        {
            BackColor = PageBack;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = true;

            cardPanel = new Panel
            {
                Size = new Size(720, 760),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            cardPanel.Paint += CardPanel_Paint;

            topCardPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 230,
                BackColor = Navy
            };

            picProfile = new PictureBox
            {
                Size = new Size(96, 96),
                Location = new Point((cardPanel.Width - 96) / 2, 28),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top
            };
            picProfile.Paint += PicProfile_Paint;

            lblDisplayName = new Label
            {
                Text = "Super Admin",
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(420, 34),
                Location = new Point((cardPanel.Width - 420) / 2, 128)
            };

            lblUsername = new Label
            {
                Text = "@superadmin",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.Gainsboro,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(420, 24),
                Location = new Point((cardPanel.Width - 420) / 2, 160)
            };

            btnUploadImage = CreateButton("Upload Image", Accent, Color.White, 140, 34);
            btnUploadImage.Location = new Point((cardPanel.Width - btnUploadImage.Width) / 2, 190);
            btnUploadImage.Click += BtnUploadImage_Click;

            topCardPanel.Controls.Add(picProfile);
            topCardPanel.Controls.Add(lblDisplayName);
            topCardPanel.Controls.Add(lblUsername);
            topCardPanel.Controls.Add(btnUploadImage);

            formPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            Label heading = CreateLabel("Profile Settings", 20F, FontStyle.Bold, TextDark, 44, 30);
            Label sub = CreateLabel("Update your profile image, name, username, email, and password.", 10.5F, FontStyle.Regular, TextMuted, 44, 63);

            Label fullNameLabel = CreateLabel("FULL NAME", 8.8F, FontStyle.Bold, TextDark, 44, 112);
            txtFullName = CreateTextBox(44, 136, 292);

            Label usernameLabel = CreateLabel("USERNAME", 8.8F, FontStyle.Bold, TextDark, 384, 112);
            txtUsername = CreateTextBox(384, 136, 292);

            Label emailLabel = CreateLabel("EMAIL ADDRESS", 8.8F, FontStyle.Bold, TextDark, 44, 190);
            txtEmail = CreateTextBox(44, 214, 632);

            btnSaveProfile = CreateButton("Save Profile Changes", Accent, Color.White, 224, 44);
            btnSaveProfile.Location = new Point(44, 276);
            btnSaveProfile.Click += BtnSaveProfile_Click;

            Panel divider = new Panel
            {
                BackColor = Border,
                Location = new Point(44, 348),
                Size = new Size(632, 1)
            };

            Label passHeading = CreateLabel("Change Password", 18F, FontStyle.Bold, TextDark, 44, 374);
            Label passSub = CreateLabel("Use a strong password to protect your account.", 10F, FontStyle.Regular, TextMuted, 44, 405);

            Label currentLabel = CreateLabel("CURRENT PASSWORD", 8.8F, FontStyle.Bold, TextDark, 44, 448);
            txtCurrentPassword = CreatePasswordBox(44, 472, 632);

            Label newLabel = CreateLabel("NEW PASSWORD", 8.8F, FontStyle.Bold, TextDark, 44, 526);
            txtNewPassword = CreatePasswordBox(44, 550, 292);

            Label confirmLabel = CreateLabel("CONFIRM PASSWORD", 8.8F, FontStyle.Bold, TextDark, 384, 526);
            txtConfirmPassword = CreatePasswordBox(384, 550, 292);

            btnChangePassword = CreateButton("Change Password", Navy, Color.White, 204, 44);
            btnChangePassword.Location = new Point(44, 622);
            btnChangePassword.Click += BtnChangePassword_Click;

            formPanel.Controls.Add(heading);
            formPanel.Controls.Add(sub);
            formPanel.Controls.Add(fullNameLabel);
            formPanel.Controls.Add(txtFullName);
            formPanel.Controls.Add(usernameLabel);
            formPanel.Controls.Add(txtUsername);
            formPanel.Controls.Add(emailLabel);
            formPanel.Controls.Add(txtEmail);
            formPanel.Controls.Add(btnSaveProfile);
            formPanel.Controls.Add(divider);
            formPanel.Controls.Add(passHeading);
            formPanel.Controls.Add(passSub);
            formPanel.Controls.Add(currentLabel);
            formPanel.Controls.Add(txtCurrentPassword);
            formPanel.Controls.Add(newLabel);
            formPanel.Controls.Add(txtNewPassword);
            formPanel.Controls.Add(confirmLabel);
            formPanel.Controls.Add(txtConfirmPassword);
            formPanel.Controls.Add(btnChangePassword);

            cardPanel.Controls.Add(formPanel);
            cardPanel.Controls.Add(topCardPanel);
            Controls.Add(cardPanel);

            topCardPanel.Resize += TopCardPanel_Resize;
            Resize += (s, e) => CenterCard();
            CenterCard();
        }

        private Button CreateButton(string text, Color back, Color fore, int width, int height)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(width, height),
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Label CreateLabel(string text, float size, FontStyle style, Color color, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                AutoSize = true,
                Location = new Point(x, y),
                BackColor = Color.Transparent
            };
        }

        private TextBox CreateTextBox(int x, int y, int width)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 34),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = FieldBack,
                ForeColor = TextDark,
                Font = new Font("Segoe UI", 10.5F)
            };
        }

        private TextBox CreatePasswordBox(int x, int y, int width)
        {
            TextBox box = CreateTextBox(x, y, width);
            box.UseSystemPasswordChar = true;
            return box;
        }

        private void CardPanel_Paint(object? sender, PaintEventArgs e)
        {
            using Pen border = new Pen(Border, 1);
            e.Graphics.DrawRectangle(border, 0, 0, cardPanel.Width - 1, cardPanel.Height - 1);
        }

        private void CenterCard()
        {
            if (cardPanel == null) return;

            int x = Math.Max(40, (ClientSize.Width - cardPanel.Width) / 2);
            int y = Math.Max(25, (ClientSize.Height - cardPanel.Height) / 2);
            cardPanel.Location = new Point(x, y);

            AutoScrollMinSize = new Size(0, cardPanel.Bottom + 40);
        }

        private void TopCardPanel_Resize(object? sender, EventArgs e)
        {
            picProfile.Location = new Point((topCardPanel.Width - picProfile.Width) / 2, 28);
            lblDisplayName.Location = new Point((topCardPanel.Width - lblDisplayName.Width) / 2, 128);
            lblUsername.Location = new Point((topCardPanel.Width - lblUsername.Width) / 2, 160);
            btnUploadImage.Location = new Point((topCardPanel.Width - btnUploadImage.Width) / 2, 190);
        }

        private void LoadProfileData()
        {
            string username = string.IsNullOrWhiteSpace(UserSession.Username) ? "superadmin" : UserSession.Username;
            string role = string.IsNullOrWhiteSpace(UserSession.Role) ? "Super Admin" : UserSession.Role;

            lblDisplayName.Text = role;
            lblUsername.Text = "@" + username.Replace(" ", "").ToLower();

            txtFullName.Text = role;
            txtUsername.Text = username;
            txtEmail.Text = GetCurrentEmail();

            try
            {
                if (!string.IsNullOrWhiteSpace(UserSession.ImagePath) && File.Exists(UserSession.ImagePath))
                {
                    using Bitmap bmpTemp = new Bitmap(UserSession.ImagePath);
                    picProfile.Image = new Bitmap(bmpTemp);
                    selectedImagePath = UserSession.ImagePath;
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

        private string GetCurrentEmail()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"
SELECT TOP 1 Email
FROM dbo.Users
WHERE Username = @Username
   OR FullName = @Username;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", UserSession.Username ?? "");

                object? result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? "" : Convert.ToString(result) ?? "";
            }
            catch
            {
                return "";
            }
        }

        private void BtnUploadImage_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Choose Profile Image",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            selectedImagePath = dialog.FileName;

            using Bitmap bmpTemp = new Bitmap(selectedImagePath);
            picProfile.Image = new Bitmap(bmpTemp);
            MakePictureCircular();
        }

        private void BtnSaveProfile_Click(object? sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username is required.");
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureUserProfileColumns(conn);

                string query = @"
UPDATE dbo.Users
SET
    FullName = CASE WHEN @FullName = '' THEN FullName ELSE @FullName END,
    Username = @Username,
    Email = @Email,
    ImagePath = @ImagePath,
    UpdatedAt = SYSDATETIME()
WHERE Username = @OldUsername OR FullName = @OldUsername;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@ImagePath", selectedImagePath);
                cmd.Parameters.AddWithValue("@OldUsername", UserSession.Username ?? "");

                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                {
                    MessageBox.Show("No matching user record was found. Make sure the logged-in username exists in Users table.");
                    return;
                }

                UserSession.Username = username;
                UserSession.ImagePath = selectedImagePath;

                lblDisplayName.Text = string.IsNullOrWhiteSpace(fullName) ? UserSession.Role : fullName;
                lblUsername.Text = "@" + username.Replace(" ", "").ToLower();

                MessageBox.Show("Profile updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update profile.\n\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnChangePassword_Click(object? sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("New password is required.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New password and confirm password do not match.");
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string verifyQuery = @"
SELECT COUNT(*)
FROM dbo.Users
WHERE (Username = @Username OR FullName = @Username)
  AND PasswordText = @CurrentPassword;";

                using SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn);
                verifyCmd.Parameters.AddWithValue("@Username", UserSession.Username ?? "");
                verifyCmd.Parameters.AddWithValue("@CurrentPassword", currentPassword);

                int match = Convert.ToInt32(verifyCmd.ExecuteScalar());

                if (match == 0)
                {
                    MessageBox.Show("Current password is incorrect.");
                    return;
                }

                string updateQuery = @"
UPDATE dbo.Users
SET PasswordText = @NewPassword,
    UpdatedAt = SYSDATETIME()
WHERE Username = @Username OR FullName = @Username;";

                using SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@NewPassword", newPassword);
                updateCmd.Parameters.AddWithValue("@Username", UserSession.Username ?? "");
                updateCmd.ExecuteNonQuery();

                txtCurrentPassword.Clear();
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();

                MessageBox.Show("Password changed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to change password.\n\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnsureUserProfileColumns(SqlConnection conn)
        {
            string query = @"
IF COL_LENGTH('dbo.Users', 'Username') IS NULL
    ALTER TABLE dbo.Users ADD Username NVARCHAR(100) NULL;

IF COL_LENGTH('dbo.Users', 'ImagePath') IS NULL
    ALTER TABLE dbo.Users ADD ImagePath NVARCHAR(255) NULL;

IF COL_LENGTH('dbo.Users', 'UpdatedAt') IS NULL
    ALTER TABLE dbo.Users ADD UpdatedAt DATETIME2 NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void MakePictureCircular()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, picProfile.Width - 1, picProfile.Height - 1);
            picProfile.Region = new Region(path);
        }

        private void PicProfile_Paint(object? sender, PaintEventArgs e)
        {
            using Pen pen = new Pen(Color.FromArgb(160, 109, 250, 210), 2);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawEllipse(pen, 1, 1, picProfile.Width - 3, picProfile.Height - 3);
        }
    }
}
