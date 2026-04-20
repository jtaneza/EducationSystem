namespace EducationSystem
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelLeft = new Panel();
            lblFooter = new Label();
            lblStatus = new Label();
            lblQuote = new Label();
            picLogo = new PictureBox();
            panelRight = new Panel();
            lblPortalStatus = new Label();
            picEye = new PictureBox();
            linkLabel1 = new LinkLabel();
            rememberme = new CheckBox();
            login = new Button();
            password = new TextBox();
            lblPassword = new Label();
            emailadd = new TextBox();
            lblEmail = new Label();
            lblSubtitle = new Label();
            lblWelcome = new Label();
            notifyIcon1 = new NotifyIcon(components);
            panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picEye).BeginInit();
            SuspendLayout();
            // 
            // panelLeft
            // 
            panelLeft.Controls.Add(lblFooter);
            panelLeft.Controls.Add(lblStatus);
            panelLeft.Controls.Add(lblQuote);
            panelLeft.Controls.Add(picLogo);
            panelLeft.Dock = DockStyle.Left;
            panelLeft.Location = new Point(0, 0);
            panelLeft.Name = "panelLeft";
            panelLeft.Size = new Size(420, 560);
            panelLeft.TabIndex = 0;
            panelLeft.Paint += panelLeft_Paint;
            // 
            // lblFooter
            // 
            lblFooter.AutoSize = true;
            lblFooter.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            lblFooter.Location = new Point(92, 495);
            lblFooter.Name = "lblFooter";
            lblFooter.Size = new Size(221, 13);
            lblFooter.TabIndex = 3;
            lblFooter.Text = "© 2026 LIBRAFLOW ERP • PRIVACY • TERMS";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.Location = new Point(90, 462);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(196, 15);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "SYSTEM STATUS • ALL OPERATIONAL";
            // 
            // lblQuote
            // 
            lblQuote.AutoSize = true;
            lblQuote.Font = new Font("Segoe UI", 12F, FontStyle.Italic);
            lblQuote.Location = new Point(62, 314);
            lblQuote.Name = "lblQuote";
            lblQuote.Size = new Size(275, 63);
            lblQuote.TabIndex = 1;
            lblQuote.Text = "\"Easy tools for managing books,\r\nmembers, and records.\"";
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.Location = new Point(40, 78);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(340, 220);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            picLogo.Click += picLogo_Click;
            // 
            // panelRight
            // 
            panelRight.Controls.Add(lblPortalStatus);
            panelRight.Controls.Add(picEye);
            panelRight.Controls.Add(linkLabel1);
            panelRight.Controls.Add(rememberme);
            panelRight.Controls.Add(login);
            panelRight.Controls.Add(password);
            panelRight.Controls.Add(lblPassword);
            panelRight.Controls.Add(emailadd);
            panelRight.Controls.Add(lblEmail);
            panelRight.Controls.Add(lblSubtitle);
            panelRight.Controls.Add(lblWelcome);
            panelRight.Dock = DockStyle.Fill;
            panelRight.Location = new Point(420, 0);
            panelRight.Name = "panelRight";
            panelRight.Size = new Size(480, 560);
            panelRight.TabIndex = 1;
            panelRight.Paint += panelRight_Paint;
            // 
            // lblPortalStatus
            // 
            lblPortalStatus.AutoSize = true;
            lblPortalStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPortalStatus.Location = new Point(96, 420);
            lblPortalStatus.Name = "lblPortalStatus";
            lblPortalStatus.Size = new Size(237, 15);
            lblPortalStatus.TabIndex = 10;
            lblPortalStatus.Text = "SYSTEM STATUS • ALL SYSTEMS OPERATIONAL";
            // 
            // picEye
            // 
            picEye.Cursor = Cursors.Hand;
            picEye.Location = new Point(381, 254);
            picEye.Name = "picEye";
            picEye.Size = new Size(22, 22);
            picEye.SizeMode = PictureBoxSizeMode.Zoom;
            picEye.TabIndex = 9;
            picEye.TabStop = false;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(315, 215);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(91, 15);
            linkLabel1.TabIndex = 8;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Forgot Password?";
            // 
            // rememberme
            // 
            rememberme.AutoSize = true;
            rememberme.Location = new Point(78, 299);
            rememberme.Name = "rememberme";
            rememberme.Size = new Size(138, 19);
            rememberme.TabIndex = 7;
            rememberme.Text = "Keep me logged in";
            rememberme.UseVisualStyleBackColor = true;
            // 
            // login
            // 
            login.Location = new Point(78, 344);
            login.Name = "login";
            login.Size = new Size(330, 40);
            login.TabIndex = 6;
            login.Text = "LOGIN TO PORTAL";
            login.UseVisualStyleBackColor = true;
            login.Click += btnLogin_Click;
            // 
            // password
            // 
            password.BorderStyle = BorderStyle.None;
            password.Location = new Point(78, 244);
            password.Name = "password";
            password.Size = new Size(330, 16);
            password.TabIndex = 5;
            password.TextChanged += textBox2_TextChanged;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPassword.Location = new Point(78, 215);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(68, 15);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "PASSWORD";
            // 
            // emailadd
            // 
            emailadd.BorderStyle = BorderStyle.None;
            emailadd.Location = new Point(78, 168);
            emailadd.Name = "emailadd";
            emailadd.Size = new Size(330, 16);
            emailadd.TabIndex = 3;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEmail.Location = new Point(78, 139);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(92, 15);
            lblEmail.TabIndex = 2;
            lblEmail.Text = "EMAIL ADDRESS";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblSubtitle.Location = new Point(78, 90);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(271, 19);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Please enter your credentials to access the portal.";
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblWelcome.Location = new Point(78, 40);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(223, 45);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Welcome back";
            // 
            // notifyIcon1
            // 
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 560);
            Controls.Add(panelRight);
            Controls.Add(panelLeft);
            MinimumSize = new Size(860, 540);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            Load += LoginForm_Load;
            panelLeft.ResumeLayout(false);
            panelLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            panelRight.ResumeLayout(false);
            panelRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picEye).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelLeft;
        private Panel panelRight;
        private PictureBox picLogo;
        private Label lblQuote;
        private Label lblStatus;
        private Label lblFooter;

        private Label lblWelcome;
        private Label lblSubtitle;
        private Label lblEmail;
        private TextBox emailadd;
        private Label lblPassword;
        private TextBox password;
        private Button login;
        private CheckBox rememberme;
        private LinkLabel linkLabel1;
        private PictureBox picEye;
        private Label lblPortalStatus;

        private NotifyIcon notifyIcon1;
    }
}