using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ClientProfileForm : Form
    {
        private Panel cardPanel = null!;
        private Panel topCardPanel = null!;
        private Panel bottomCardPanel = null!;

        private PictureBox picProfile = null!;
        private Label lblDisplayName = null!;
        private Label lblUsername = null!;

        private LinkLabel lnkEditProfile = null!;
        private LinkLabel lnkChangePassword = null!;

        public ClientProfileForm()
        {
            InitializeComponent();
            BuildProfileUI();
            LoadProfileData();
        }

        private void BuildProfileUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            cardPanel = new Panel();
            cardPanel.Size = new Size(380, 255);
            cardPanel.Location = new Point(35, 25);
            cardPanel.BackColor = Color.White;
            cardPanel.BorderStyle = BorderStyle.FixedSingle;

            topCardPanel = new Panel();
            topCardPanel.Dock = DockStyle.Top;
            topCardPanel.Height = 178;
            topCardPanel.BackColor = Color.FromArgb(8, 29, 56);

            picProfile = new PictureBox();
            picProfile.Size = new Size(82, 82);
            picProfile.Location = new Point(149, 40);
            picProfile.SizeMode = PictureBoxSizeMode.Zoom;
            picProfile.BackColor = Color.Transparent;
            picProfile.Paint += PicProfile_Paint;

            lblDisplayName = new Label();
            lblDisplayName.Font = new Font("Segoe UI", 16, FontStyle.Regular);
            lblDisplayName.ForeColor = Color.White;
            lblDisplayName.AutoSize = false;
            lblDisplayName.TextAlign = ContentAlignment.MiddleCenter;
            lblDisplayName.Size = new Size(300, 32);
            lblDisplayName.Location = new Point(40, 118);

            lblUsername = new Label();
            lblUsername.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblUsername.ForeColor = Color.Gainsboro;
            lblUsername.AutoSize = false;
            lblUsername.TextAlign = ContentAlignment.MiddleCenter;
            lblUsername.Size = new Size(300, 26);
            lblUsername.Location = new Point(40, 146);

            topCardPanel.Controls.Add(picProfile);
            topCardPanel.Controls.Add(lblDisplayName);
            topCardPanel.Controls.Add(lblUsername);

            bottomCardPanel = new Panel();
            bottomCardPanel.Dock = DockStyle.Fill;
            bottomCardPanel.BackColor = Color.White;

            lnkEditProfile = new LinkLabel();
            lnkEditProfile.Text = "✎ Edit Profile";
            lnkEditProfile.Location = new Point(16, 14);
            lnkEditProfile.Size = new Size(120, 22);
            lnkEditProfile.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lnkEditProfile.LinkColor = Color.DimGray;
            lnkEditProfile.ActiveLinkColor = Color.Maroon;
            lnkEditProfile.VisitedLinkColor = Color.DimGray;
            lnkEditProfile.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkEditProfile.Click += LnkEditProfile_Click;

            lnkChangePassword = new LinkLabel();
            lnkChangePassword.Text = "🔒 Change Password";
            lnkChangePassword.Location = new Point(16, 42);
            lnkChangePassword.Size = new Size(160, 22);
            lnkChangePassword.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lnkChangePassword.LinkColor = Color.DimGray;
            lnkChangePassword.ActiveLinkColor = Color.Maroon;
            lnkChangePassword.VisitedLinkColor = Color.DimGray;
            lnkChangePassword.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkChangePassword.Click += LnkChangePassword_Click;

            bottomCardPanel.Controls.Add(lnkEditProfile);
            bottomCardPanel.Controls.Add(lnkChangePassword);

            cardPanel.Controls.Add(bottomCardPanel);
            cardPanel.Controls.Add(topCardPanel);

            Controls.Add(cardPanel);

            topCardPanel.Resize += TopCardPanel_Resize;
        }

        private void TopCardPanel_Resize(object? sender, EventArgs e)
        {
            picProfile.Location = new Point((topCardPanel.Width - picProfile.Width) / 2, 40);
        }

        private void LoadProfileData()
        {
            string displayName = string.IsNullOrWhiteSpace(ClientSession.LibraryName)
                ? "Client Admin"
                : ClientSession.LibraryName!;

            lblDisplayName.Text = displayName;
            lblUsername.Text = "@" + displayName.Replace(" ", "").ToLower();

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

        private void LnkEditProfile_Click(object? sender, EventArgs e)
        {
            Form? parentForm = this.Parent?.FindForm();
            if (parentForm is ClientDashboardForm dashboard)
            {
                dashboard.LoadContentForm(new ClientSettingsForm());
            }
        }

        private void LnkChangePassword_Click(object? sender, EventArgs e)
        {
            Form? parentForm = this.Parent?.FindForm();
            if (parentForm is ClientDashboardForm dashboard)
            {
                ClientSettingsForm settings = new ClientSettingsForm();
                settings.OpenPasswordSection();
                dashboard.LoadContentForm(settings);
            }
        }
    }
}