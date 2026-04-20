using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class LoginForm : Form
    {
        private bool isPasswordVisible = false;

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
            password.PasswordChar = '●';

            picEye.Enabled = true;
            picEye.Visible = true;
            picEye.Cursor = Cursors.Hand;
            picEye.SizeMode = PictureBoxSizeMode.Zoom;
            picEye.BringToFront();

            picEye.Click -= picEye_Click;
            picEye.Click += picEye_Click;

            if (Settings.Default.RememberMe)
            {
                emailadd.Text = Settings.Default.UserEmail;
                password.Text = Settings.Default.UserPassword;
                rememberme.Checked = true;

                isPasswordVisible = false;
                password.UseSystemPasswordChar = true;
                password.PasswordChar = '●';
            }

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

            if (email.Equals("supadmin@gmail.com", StringComparison.OrdinalIgnoreCase) &&
                userPassword == "sup123")
            {
                SaveRememberMe(email, userPassword);

                UserSession.Username = "Super Admin";
                UserSession.Role = "Super Admin";
                UserSession.Email = "supadmin@gmail.com";
                UserSession.Password = "sup123";
                UserSession.ImagePath = Application.StartupPath + @"\Assets\user.png";

                DashboardForm dashboard = new DashboardForm();
                dashboard.Show();
                Hide();
                return;
            }

            ClientItem? client = ClientArchiveStore.ActiveClients
                .FirstOrDefault(c =>
                    c.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    c.Password == userPassword);

            if (client != null)
            {
                if (!client.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Your account is inactive. Please contact the Super Admin.");
                    return;
                }

                SaveRememberMe(email, userPassword);

                ClientSession.ClientId = client.ClientID;
                ClientSession.LibraryName = client.LibraryName;
                ClientSession.Email = client.Email;
                ClientSession.Role = "Client Admin";
                ClientSession.Username = client.LibraryName;
                ClientSession.ImagePath = Application.StartupPath + @"\Assets\client.png";

                ClientDashboardForm clientDashboard = new ClientDashboardForm();
                clientDashboard.Show();
                Hide();
                return;
            }

            if (email.Equals("john@school.edu", StringComparison.OrdinalIgnoreCase) && userPassword == "john123")
            {
                SaveRememberMe(email, userPassword);

                UserSession.Username = "John Cruz";
                UserSession.Role = "Member";
                UserSession.Email = "john@school.edu";
                UserSession.Password = "john123";
                UserSession.ImagePath = Application.StartupPath + @"\Assets\user.png";

                MessageBox.Show("Member dashboard next.");
                Hide();
                return;
            }

            if (email.Equals("maria@school.edu", StringComparison.OrdinalIgnoreCase) && userPassword == "maria123")
            {
                SaveRememberMe(email, userPassword);

                UserSession.Username = "Maria Santos";
                UserSession.Role = "Member";
                UserSession.Email = "maria@school.edu";
                UserSession.Password = "maria123";
                UserSession.ImagePath = Application.StartupPath + @"\Assets\user.png";

                MessageBox.Show("Member dashboard next.");
                Hide();
                return;
            }

            if (email.Equals("anne@school.edu", StringComparison.OrdinalIgnoreCase) && userPassword == "anne123")
            {
                MessageBox.Show("This account is archived and cannot log in.");
                return;
            }

            MessageBox.Show("Invalid email or password.");
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
                password.PasswordChar = '●';
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