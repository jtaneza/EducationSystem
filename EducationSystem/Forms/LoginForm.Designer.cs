namespace EducationSystem
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            label1 = new Label();
            label2 = new Label();
            emailadd = new TextBox();
            label3 = new Label();
            password = new TextBox();
            login = new Button();
            panelRight = new Panel();
            picEye = new PictureBox();
            picLogo = new PictureBox();
            linkLabel1 = new LinkLabel();
            rememberme = new CheckBox();
            notifyIcon1 = new NotifyIcon(components);
            panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picEye).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Black", 20.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Maroon;
            label1.Location = new Point(436, 39);
            label1.Name = "label1";
            label1.Size = new Size(206, 37);
            label1.TabIndex = 0;
            label1.Text = "LibraFlow ERP";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Maroon;
            label2.Location = new Point(404, 97);
            label2.Name = "label2";
            label2.Size = new Size(83, 15);
            label2.TabIndex = 1;
            label2.Text = "Email Address";
            // 
            // emailadd
            // 
            emailadd.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            emailadd.ForeColor = Color.Sienna;
            emailadd.Location = new Point(404, 115);
            emailadd.Multiline = true;
            emailadd.Name = "emailadd";
            emailadd.Size = new Size(265, 28);
            emailadd.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Maroon;
            label3.Location = new Point(404, 166);
            label3.Name = "label3";
            label3.Size = new Size(59, 15);
            label3.TabIndex = 3;
            label3.Text = "Password";
            // 
            // password
            // 
            password.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            password.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            password.ForeColor = Color.Sienna;
            password.Location = new Point(405, 184);
            password.Name = "password";
            password.PasswordChar = '*';
            password.Size = new Size(265, 25);
            password.TabIndex = 4;
            password.UseSystemPasswordChar = true;
            password.TextChanged += textBox2_TextChanged;
            // 
            // login
            // 
            login.BackColor = Color.Maroon;
            login.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            login.ForeColor = SystemColors.ButtonHighlight;
            login.Location = new Point(404, 261);
            login.Name = "login";
            login.Size = new Size(265, 34);
            login.TabIndex = 5;
            login.Text = "Login";
            login.UseVisualStyleBackColor = false;
            login.Click += btnLogin_Click;
            // 
            // panelRight
            // 
            panelRight.BackColor = Color.SeaShell;
            panelRight.Controls.Add(picEye);
            panelRight.Controls.Add(picLogo);
            panelRight.Controls.Add(linkLabel1);
            panelRight.Controls.Add(rememberme);
            panelRight.Controls.Add(login);
            panelRight.Controls.Add(label3);
            panelRight.Controls.Add(emailadd);
            panelRight.Controls.Add(label2);
            panelRight.Controls.Add(label1);
            panelRight.Controls.Add(password);
            panelRight.Dock = DockStyle.Right;
            panelRight.Location = new Point(-2, 0);
            panelRight.Name = "panelRight";
            panelRight.Size = new Size(738, 450);
            panelRight.TabIndex = 6;
            panelRight.Paint += panelRight_Paint;
            // 
            // picEye
            // 
            picEye.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            picEye.BackColor = SystemColors.Window;
            picEye.Cursor = Cursors.Hand;
            picEye.Image = (Image)resources.GetObject("picEye.Image");
            picEye.Location = new Point(635, 186);
            picEye.Name = "picEye";
            picEye.Size = new Size(33, 20);
            picEye.SizeMode = PictureBoxSizeMode.Zoom;
            picEye.TabIndex = 8;
            picEye.TabStop = false;
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(53, 4);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(346, 386);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            picLogo.Click += picLogo_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = Color.DarkRed;
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = Color.Red;
            linkLabel1.Location = new Point(480, 298);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(127, 15);
            linkLabel1.TabIndex = 7;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Forgot Your Password?";
            // 
            // rememberme
            // 
            rememberme.AutoSize = true;
            rememberme.ForeColor = Color.Maroon;
            rememberme.Location = new Point(405, 224);
            rememberme.Name = "rememberme";
            rememberme.Size = new Size(109, 19);
            rememberme.TabIndex = 6;
            rememberme.Text = "Remember me?";
            rememberme.UseVisualStyleBackColor = true;
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
            BackColor = Color.SeaShell;
            ClientSize = new Size(736, 450);
            Controls.Add(panelRight);
            Name = "LoginForm";
            Text = "A";
            Load += LoginForm_Load;
            panelRight.ResumeLayout(false);
            panelRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picEye).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox emailadd;
        private Label label3;
        private TextBox password;
        private Button login;
        private Panel panelRight;
        private LinkLabel linkLabel1;
        private CheckBox rememberme;
        private PictureBox picLogo;
        private NotifyIcon notifyIcon1;
        private PictureBox picEye;
    }
}
