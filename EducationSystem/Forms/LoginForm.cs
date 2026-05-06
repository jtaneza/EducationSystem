using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class LoginForm : Form
    {
        private bool isPasswordVisible = false;
        private readonly Dictionary<string, string> loginHistory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ListBox emailRecommendations = null!;
        private ListBox passwordRecommendations = null!;
        private bool isApplyingRecommendation = false;

        // Palette
        private readonly Color LeftPanelBack = Color.FromArgb(31, 41, 46);
        private readonly Color LeftPanelGlow = Color.FromArgb(20, 191, 166);
        private readonly Color RightPanelBack = Color.FromArgb(250, 250, 248);
        private readonly Color InputBack = Color.FromArgb(236, 242, 240);
        private readonly Color InputText = Color.FromArgb(18, 98, 82);   // darker green
        private readonly Color InputBorder = Color.FromArgb(198, 220, 214);
        private readonly Color PrimaryButton = Color.FromArgb(20, 191, 166);
        private readonly Color PrimaryButtonHover = Color.FromArgb(16, 160, 139);
        private readonly Color HeadingText = Color.FromArgb(33, 37, 41);
        private readonly Color MutedText = Color.FromArgb(120, 125, 130);
        private readonly Color LabelText = Color.FromArgb(90, 96, 100);

        public LoginForm()
        {
            InitializeComponent();
            AcceptButton = login;

            Load += LoginForm_Load;
            Resize += LoginForm_Resize;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            LoadBranding();

            isPasswordVisible = false;
            password.UseSystemPasswordChar = true;
            password.PasswordChar = '\u25CF';

            picEye.Enabled = true;
            picEye.Visible = true;
            picEye.Cursor = Cursors.Hand;
            picEye.SizeMode = PictureBoxSizeMode.Zoom;
            picEye.BringToFront();

            picEye.Click -= picEye_Click;
            picEye.Click += picEye_Click;

            LoadLoginHistory();

            if (Settings.Default.RememberMe)
            {
                emailadd.Text = Settings.Default.UserEmail;
                password.Text = Settings.Default.UserPassword;
                rememberme.Checked = true;

                isPasswordVisible = false;
                password.UseSystemPasswordChar = true;
                password.PasswordChar = '\u25CF';
            }

            ConfigureLoginRecommendations();
            ArrangeLoginLayout();
        }

        private void LoginForm_Resize(object? sender, EventArgs e)
        {
            ArrangeLoginLayout();
        }

        private void ApplyTheme()
        {
            BackColor = RightPanelBack;

            panelLeft.BackColor = LeftPanelBack;
            panelRight.BackColor = RightPanelBack;

            lblWelcome.ForeColor = HeadingText;
            lblSubtitle.ForeColor = MutedText;

            lblEmail.ForeColor = LabelText;
            lblPassword.ForeColor = LabelText;

            lblQuote.ForeColor = Color.FromArgb(215, 220, 223);
            lblStatus.ForeColor = LeftPanelGlow;
            lblFooter.ForeColor = Color.FromArgb(150, 155, 160);

            StyleInput(emailadd);
            StyleInput(password);

            StylePrimaryButton(login);

            rememberme.ForeColor = MutedText;
            rememberme.BackColor = Color.Transparent;

            linkLabel1.LinkColor = LeftPanelGlow;
            linkLabel1.ActiveLinkColor = PrimaryButtonHover;
            linkLabel1.VisitedLinkColor = LeftPanelGlow;

            picEye.BackColor = InputBack;
            picLogo.BackColor = Color.Transparent;

            emailadd.Multiline = true;
            password.Multiline = true;

            panelRight.Invalidate();
        }

        private void StyleInput(TextBox txt)
        {
            txt.BackColor = InputBack;
            txt.ForeColor = InputText;
            txt.BorderStyle = BorderStyle.None;
            txt.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
        }

        private void StylePrimaryButton(Button btn)
        {
            btn.BackColor = PrimaryButton;
            btn.ForeColor = Color.FromArgb(20, 28, 30);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = PrimaryButton;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            btn.MouseEnter -= LoginButton_MouseEnter;
            btn.MouseLeave -= LoginButton_MouseLeave;
            btn.MouseEnter += LoginButton_MouseEnter;
            btn.MouseLeave += LoginButton_MouseLeave;
        }

        private void LoginButton_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = PrimaryButtonHover;
                btn.FlatAppearance.BorderColor = PrimaryButtonHover;
            }
        }

        private void LoginButton_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = PrimaryButton;
                btn.FlatAppearance.BorderColor = PrimaryButton;
            }
        }

        private void ArrangeLoginLayout()
        {
            int leftWidth = Math.Max(350, ClientSize.Width / 2);

            panelLeft.Width = leftWidth;
            panelRight.Left = panelLeft.Right;

            // Bigger logo
            picLogo.Size = new Size(340, 220);
            picLogo.Location = new Point((panelLeft.Width - picLogo.Width) / 2, 78);

            int quoteWidth = Math.Min(320, panelLeft.Width - 90);
            lblQuote.MaximumSize = new Size(quoteWidth, 0);
            lblQuote.Location = new Point((panelLeft.Width - quoteWidth) / 2, picLogo.Bottom + 34);

            lblStatus.Location = new Point((panelLeft.Width - lblStatus.PreferredWidth) / 2, panelLeft.Height - 95);
            lblFooter.Location = new Point((panelLeft.Width - lblFooter.PreferredWidth) / 2, panelLeft.Height - 55);

            // Right login layout
            int formWidth = Math.Min(360, panelRight.Width - 90);
            int startX = Math.Max(45, (panelRight.Width - formWidth) / 2);
            int topY = Math.Max(70, (panelRight.Height - 430) / 2);

            lblWelcome.Location = new Point(startX, topY);
            lblSubtitle.Location = new Point(startX, lblWelcome.Bottom + 8);

            lblEmail.Location = new Point(startX, lblSubtitle.Bottom + 34);
            emailadd.Location = new Point(startX + 10, lblEmail.Bottom + 20);
            emailadd.Size = new Size(formWidth - 20, 20);

            lblPassword.Location = new Point(startX, lblEmail.Bottom + 62);
            linkLabel1.Location = new Point(startX + formWidth - linkLabel1.PreferredWidth, lblPassword.Top);
            password.Location = new Point(startX + 10, lblPassword.Bottom + 20);
            password.Size = new Size(formWidth - 50, 20);

            picEye.Size = new Size(22, 22);
            picEye.Location = new Point(startX + formWidth - 34, password.Top - 1);

            rememberme.Location = new Point(startX, lblPassword.Bottom + 62);

            login.Location = new Point(startX, rememberme.Bottom + 32);
            login.Size = new Size(formWidth, 40);

            lblPortalStatus.Location = new Point(startX + (formWidth - lblPortalStatus.PreferredWidth) / 2, login.Bottom + 34);
            LayoutRecommendationBoxes();
        }

        private void ConfigureLoginRecommendations()
        {
            if (emailRecommendations != null && passwordRecommendations != null)
                return;

            emailRecommendations = CreateRecommendationList();
            passwordRecommendations = CreateRecommendationList();

            emailRecommendations.Click += (s, e) => ApplySelectedEmailRecommendation();
            passwordRecommendations.Click += (s, e) => ApplySelectedPasswordRecommendation();
            emailRecommendations.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    ApplySelectedEmailRecommendation();
                    emailadd.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    emailRecommendations.Visible = false;
                    emailadd.Focus();
                }
            };
            passwordRecommendations.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    ApplySelectedPasswordRecommendation();
                    password.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    passwordRecommendations.Visible = false;
                    password.Focus();
                }
            };

            emailadd.TextChanged += (s, e) =>
            {
                if (isApplyingRecommendation) return;

                ShowEmailRecommendations();

                string email = emailadd.Text.Trim();
                if (loginHistory.TryGetValue(email, out string savedPassword) && !password.Focused)
                    ApplyPasswordText(savedPassword);
            };

            password.TextChanged += (s, e) =>
            {
                if (isApplyingRecommendation) return;
                ShowPasswordRecommendations();
            };

            emailadd.LostFocus += (s, e) => BeginInvoke(new Action(() =>
            {
                if (!emailadd.Focused && !emailRecommendations.Focused)
                    emailRecommendations.Visible = false;
            }));

            password.LostFocus += (s, e) => BeginInvoke(new Action(() =>
            {
                if (!password.Focused && !passwordRecommendations.Focused)
                    passwordRecommendations.Visible = false;
            }));

            emailadd.KeyDown += EmailRecommendation_KeyDown;
            password.KeyDown += PasswordRecommendation_KeyDown;

            panelRight.Controls.Add(emailRecommendations);
            panelRight.Controls.Add(passwordRecommendations);
            LayoutRecommendationBoxes();
        }

        private ListBox CreateRecommendationList()
        {
            return new ListBox
            {
                Visible = false,
                IntegralHeight = false,
                Height = 92,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = InputBack,
                ForeColor = InputText,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
        }

        private void LayoutRecommendationBoxes()
        {
            if (emailRecommendations == null || passwordRecommendations == null)
                return;

            emailRecommendations.Bounds = new Rectangle(
                emailadd.Left - 10,
                emailadd.Top + 28,
                emailadd.Width + 20,
                emailRecommendations.Height
            );

            passwordRecommendations.Bounds = new Rectangle(
                password.Left - 10,
                password.Top + 28,
                password.Width + 44,
                passwordRecommendations.Height
            );

            emailRecommendations.BringToFront();
            passwordRecommendations.BringToFront();
            picEye.BringToFront();
        }

        private void ShowEmailRecommendations()
        {
            if (!emailadd.Focused || loginHistory.Count == 0 || string.IsNullOrWhiteSpace(emailadd.Text))
            {
                emailRecommendations.Visible = false;
                return;
            }

            string keyword = emailadd.Text.Trim();
            var matches = loginHistory.Keys
                .Where(email => string.IsNullOrWhiteSpace(keyword) ||
                                email.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(email => email)
                .Take(6)
                .ToArray();

            ShowRecommendations(emailRecommendations, matches);
        }

        private void ShowPasswordRecommendations()
        {
            if (!password.Focused || loginHistory.Count == 0 || string.IsNullOrWhiteSpace(password.Text))
            {
                passwordRecommendations.Visible = false;
                return;
            }

            string keyword = password.Text.Trim();
            string email = emailadd.Text.Trim();

            IEnumerable<string> candidates = loginHistory.TryGetValue(email, out string savedPassword)
                ? new[] { savedPassword }
                : loginHistory.Values.Distinct();

            var matches = candidates
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Where(value => string.IsNullOrWhiteSpace(keyword) ||
                                value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                .Take(6)
                .ToArray();

            ShowRecommendations(passwordRecommendations, matches);
        }

        private void ShowRecommendations(ListBox list, string[] items)
        {
            list.BeginUpdate();
            list.Items.Clear();
            list.Items.AddRange(items);
            list.EndUpdate();

            list.Height = Math.Max(28, Math.Min(120, items.Length * 24 + 6));
            LayoutRecommendationBoxes();
            list.Visible = items.Length > 0;

            if (list.Visible)
                list.BringToFront();
        }

        private void ApplySelectedEmailRecommendation()
        {
            if (emailRecommendations.SelectedItem == null) return;

            string selectedEmail = emailRecommendations.SelectedItem.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(selectedEmail)) return;

            isApplyingRecommendation = true;
            emailadd.Text = selectedEmail;
            emailadd.SelectionStart = emailadd.TextLength;

            if (loginHistory.TryGetValue(selectedEmail, out string savedPassword))
                ApplyPasswordText(savedPassword);

            isApplyingRecommendation = false;
            emailRecommendations.Visible = false;
            passwordRecommendations.Visible = false;
        }

        private void ApplySelectedPasswordRecommendation()
        {
            if (passwordRecommendations.SelectedItem == null) return;

            string selectedPassword = passwordRecommendations.SelectedItem.ToString() ?? "";
            ApplyPasswordText(selectedPassword);
            password.SelectionStart = password.TextLength;
            passwordRecommendations.Visible = false;
        }

        private void ApplyPasswordText(string value)
        {
            isApplyingRecommendation = true;
            password.Text = value;
            password.SelectionStart = password.TextLength;
            password.UseSystemPasswordChar = !isPasswordVisible;
            if (!isPasswordVisible)
                password.PasswordChar = '\u25CF';
            isApplyingRecommendation = false;
        }

        private void EmailRecommendation_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!emailRecommendations.Visible) return;

            if (e.KeyCode == Keys.Down && emailRecommendations.Items.Count > 0)
            {
                emailRecommendations.Focus();
                emailRecommendations.SelectedIndex = Math.Max(0, emailRecommendations.SelectedIndex);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                if (emailRecommendations.Items.Count > 0)
                {
                    if (emailRecommendations.SelectedIndex < 0)
                        emailRecommendations.SelectedIndex = 0;

                    ApplySelectedEmailRecommendation();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                emailRecommendations.Visible = false;
            }
        }

        private void PasswordRecommendation_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!passwordRecommendations.Visible) return;

            if (e.KeyCode == Keys.Down && passwordRecommendations.Items.Count > 0)
            {
                passwordRecommendations.Focus();
                passwordRecommendations.SelectedIndex = Math.Max(0, passwordRecommendations.SelectedIndex);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                if (passwordRecommendations.Items.Count > 0)
                {
                    if (passwordRecommendations.SelectedIndex < 0)
                        passwordRecommendations.SelectedIndex = 0;

                    ApplySelectedPasswordRecommendation();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                passwordRecommendations.Visible = false;
            }
        }

        private void LoadLoginHistory()
        {
            loginHistory.Clear();

            try
            {
                string historyPath = GetLoginHistoryPath();
                if (File.Exists(historyPath))
                {
                    foreach (string line in File.ReadAllLines(historyPath))
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length != 2) continue;

                        string email = DecodeHistoryValue(parts[0]);
                        string savedPassword = DecodeHistoryValue(parts[1]);

                        if (!string.IsNullOrWhiteSpace(email))
                            loginHistory[email] = savedPassword;
                    }
                }

                if (Settings.Default.RememberMe && !string.IsNullOrWhiteSpace(Settings.Default.UserEmail))
                    loginHistory[Settings.Default.UserEmail] = Settings.Default.UserPassword;
            }
            catch
            {
            }
        }

        private void SaveLoginHistory(string email, string userPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userPassword))
                    return;

                loginHistory[email] = userPassword;

                string historyPath = GetLoginHistoryPath();
                Directory.CreateDirectory(Path.GetDirectoryName(historyPath)!);

                string[] lines = loginHistory
                    .OrderBy(item => item.Key)
                    .Select(item => EncodeHistoryValue(item.Key) + "\t" + EncodeHistoryValue(item.Value))
                    .ToArray();

                File.WriteAllLines(historyPath, lines);
            }
            catch
            {
            }
        }

        private string GetLoginHistoryPath()
        {
            return Path.Combine(Application.UserAppDataPath, "login_history.dat");
        }

        private string EncodeHistoryValue(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? ""));
        }

        private string DecodeHistoryValue(string value)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }
            catch
            {
                return "";
            }
        }

        private void LoadBranding()
        {
            try
            {
                string logoPath = FindAssetPath("logo.png");

                if (string.IsNullOrWhiteSpace(logoPath))
                    logoPath = FindAssetPath("logo_light.png");

                if (string.IsNullOrWhiteSpace(logoPath))
                    logoPath = FindAssetPath("new_logo.png");

                if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
                {
                    if (picLogo.Image != null)
                    {
                        var old = picLogo.Image;
                        picLogo.Image = null;
                        old.Dispose();
                    }

                    using (var fs = new FileStream(logoPath, FileMode.Open, FileAccess.Read))
                    {
                        picLogo.Image = Image.FromStream(fs);
                    }
                }

                picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch
            {
            }
        }

        private string FindAssetPath(string fileName)
        {
            string[] possiblePaths =
            {
                Path.Combine(Application.StartupPath, "Assets", fileName),
                Path.Combine(Application.StartupPath, "..", "..", "..", "Assets", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", fileName)
            };

            foreach (string path in possiblePaths)
            {
                string fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            return "";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = emailadd.Text.Trim();
            string userPassword = password.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userPassword))
            {
                MessageBox.Show("Please enter email and password.");
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string query = @"
SELECT 
    u.UserID,
    u.ClientID,
    u.FullName,
    u.Email,
    u.PasswordText,
    u.Role,
    u.IsActive,
    u.IsArchived,
    u.ImagePath,
    c.LibraryName
FROM dbo.Users u
LEFT JOIN dbo.ClientLibraries c ON u.ClientID = c.ClientID
WHERE u.Email = @Email
  AND u.PasswordText = @PasswordText;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordText", userPassword);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    MessageBox.Show("Invalid email or password.");
                    return;
                }

                bool isActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]);
                bool isArchived = reader["IsArchived"] != DBNull.Value && Convert.ToBoolean(reader["IsArchived"]);

                if (isArchived)
                {
                    MessageBox.Show("This account is archived and cannot log in.");
                    return;
                }

                if (!isActive)
                {
                    MessageBox.Show("Your account is inactive. Please contact the administrator.");
                    return;
                }

                int clientId = reader["ClientID"] == DBNull.Value
    ? 0
    : Convert.ToInt32(reader["ClientID"]);
                string fullName = reader["FullName"] == DBNull.Value ? "" : Convert.ToString(reader["FullName"]) ?? "";
                string role = reader["Role"] == DBNull.Value ? "" : Convert.ToString(reader["Role"]) ?? "";
                string userEmail = reader["Email"] == DBNull.Value ? "" : Convert.ToString(reader["Email"]) ?? "";
                string imagePath = reader["ImagePath"] == DBNull.Value ? "Assets\\user.png" : Convert.ToString(reader["ImagePath"]) ?? "Assets\\user.png";
                string libraryName = reader["LibraryName"] == DBNull.Value ? "ABC School Library" : Convert.ToString(reader["LibraryName"]) ?? "ABC School Library";

                SaveRememberMe(email, userPassword);
                SaveLoginHistory(email, userPassword);

                UserSession.Username = fullName;
                UserSession.Role = role;
                UserSession.Email = userEmail;
                UserSession.Password = userPassword;
                UserSession.ImagePath = Application.StartupPath + "\\" + imagePath;

                if (role.Equals("Super Admin", StringComparison.OrdinalIgnoreCase))
                {
                    DashboardForm dashboard = new DashboardForm();
                    dashboard.Show();
                    Hide();
                    return;
                }

                if (role.Equals("Client Admin", StringComparison.OrdinalIgnoreCase))
                {
                    ClientSession.ClientId = clientId;
                    ClientSession.LibraryName = libraryName;
                    ClientSession.Email = userEmail;
                    ClientSession.Role = role;
                    ClientSession.Username = fullName;
                    ClientSession.ImagePath = Application.StartupPath + "\\" + imagePath;
                    ClientSession.UserID = Convert.ToInt32(reader["UserID"]);

                    ClientDashboardForm clientDashboard = new ClientDashboardForm();
                    clientDashboard.Show();
                    Hide();
                    return;
                }

                if (role.Equals("Head Librarian", StringComparison.OrdinalIgnoreCase) ||
     role.Equals("Librarian", StringComparison.OrdinalIgnoreCase) ||
     role.Equals("Assistant", StringComparison.OrdinalIgnoreCase))
                {
                    ClientSession.ClientId = clientId;
                    ClientSession.LibraryName = libraryName;
                    ClientSession.Email = userEmail;
                    ClientSession.Role = role;
                    ClientSession.Username = fullName;
                    ClientSession.ImagePath = Application.StartupPath + "\\" + imagePath;

                    LibrarianDashboardForm librarianDashboard = new LibrarianDashboardForm();
                    librarianDashboard.Show();
                    Hide();
                    return;
                }

                if (role.Equals("Student", StringComparison.OrdinalIgnoreCase) ||
                 role.Equals("Teacher", StringComparison.OrdinalIgnoreCase) ||
                 role.Equals("Member", StringComparison.OrdinalIgnoreCase))
                {
                    ClientSession.ClientId = clientId;
                    ClientSession.LibraryName = libraryName;
                    ClientSession.Email = userEmail;
                    ClientSession.Role = role;
                    ClientSession.Username = fullName;
                    ClientSession.ImagePath = Application.StartupPath + "\\" + imagePath;
                    ClientSession.UserID = Convert.ToInt32(reader["UserID"]);

                    MemberDashboardForm memberDashboard = new MemberDashboardForm();
                    memberDashboard.Show();
                    Hide();
                    return;
                }


                MessageBox.Show("Unknown role: " + role);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Login failed.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void SaveRememberMe(string email, string userPassword)
        {
            if (rememberme.Checked)
            {
                Settings.Default.UserEmail = email;
                Settings.Default.UserPassword = userPassword;
                Settings.Default.RememberMe = true;
            }
            else
            {
                Settings.Default.UserEmail = "";
                Settings.Default.UserPassword = "";
                Settings.Default.RememberMe = false;
            }

            Settings.Default.Save();
        }

        private void picEye_Click(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                password.UseSystemPasswordChar = false;
                password.PasswordChar = '\0';
            }
            else
            {
                password.UseSystemPasswordChar = true;
                password.PasswordChar = '\u25CF';
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void picLogo_Click(object sender, EventArgs e)
        {
        }

        private void panelRight_Paint(object sender, PaintEventArgs e)
        {
            using Pen pen = new Pen(InputBorder, 1);

            int emailBoxLeft = emailadd.Left - 10;
            int emailBoxTop = emailadd.Top - 8;
            int emailBoxWidth = emailadd.Width + 20;
            int emailBoxHeight = 36;

            int passwordBoxLeft = password.Left - 10;
            int passwordBoxTop = password.Top - 8;
            int passwordBoxWidth = password.Width + 44;
            int passwordBoxHeight = 36;

            using SolidBrush brush = new SolidBrush(InputBack);

            Rectangle emailRect = new Rectangle(emailBoxLeft, emailBoxTop, emailBoxWidth, emailBoxHeight);
            Rectangle passwordRect = new Rectangle(passwordBoxLeft, passwordBoxTop, passwordBoxWidth, passwordBoxHeight);

            e.Graphics.FillRectangle(brush, emailRect);
            e.Graphics.FillRectangle(brush, passwordRect);

            e.Graphics.DrawRectangle(pen, emailRect);
            e.Graphics.DrawRectangle(pen, passwordRect);
        }

        private void panelLeft_Paint(object sender, PaintEventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
    }
}



