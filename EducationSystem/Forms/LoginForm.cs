using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class LoginForm : Form
    {
        private bool isPasswordVisible = false;

        public LoginForm()
        {
            InitializeComponent();
            this.AcceptButton = login;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;

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

            // SUPER ADMIN LOGIN
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
                this.Hide();
                return;
            }

            // CLIENT ADMIN LOGIN
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

                ClientDashboardForm clientDashboard = new ClientDashboardForm();
                clientDashboard.Show();
                this.Hide();
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
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
    }
}