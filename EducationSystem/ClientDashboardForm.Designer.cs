namespace EducationSystem
{
    partial class ClientDashboardForm
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

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Sidebar = new Panel();
            button7 = new Button();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            panelContent = new Panel();
            topbar = new Panel();
            dropdownarrow = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            profileToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            username = new Label();
            ProfileImage = new PictureBox();
            label1 = new Label();
            pictureBox1 = new PictureBox();

            Sidebar.SuspendLayout();
            topbar.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ProfileImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();

            // Sidebar
            Sidebar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Sidebar.BackColor = Color.WhiteSmoke;
            Sidebar.BorderStyle = BorderStyle.Fixed3D;
            Sidebar.Controls.Add(button7);
            Sidebar.Controls.Add(button6);
            Sidebar.Controls.Add(button5);
            Sidebar.Controls.Add(button4);
            Sidebar.Controls.Add(button3);
            Sidebar.Controls.Add(button2);
            Sidebar.Controls.Add(button1);
            Sidebar.ForeColor = Color.Sienna;
            Sidebar.Location = new Point(0, 0);
            Sidebar.Margin = new Padding(3, 4, 3, 4);
            Sidebar.Name = "Sidebar";
            Sidebar.Size = new Size(232, 707);
            Sidebar.TabIndex = 0;

            // button7
            button7.FlatAppearance.BorderSize = 0;
            button7.FlatStyle = FlatStyle.Flat;
            button7.ForeColor = Color.Maroon;
            button7.Location = new Point(-51, 402);
            button7.Name = "button7";
            button7.Size = new Size(280, 43);
            button7.TabIndex = 7;
            button7.Text = "📥  Archive";
            button7.UseVisualStyleBackColor = true;

            // button6
            button6.FlatAppearance.BorderColor = Color.White;
            button6.FlatAppearance.BorderSize = 0;
            button6.FlatAppearance.MouseDownBackColor = Color.Gray;
            button6.FlatAppearance.MouseOverBackColor = Color.LightGray;
            button6.FlatStyle = FlatStyle.Flat;
            button6.ForeColor = Color.Maroon;
            button6.ImageAlign = ContentAlignment.MiddleLeft;
            button6.Location = new Point(-16, 636);
            button6.Name = "button6";
            button6.Size = new Size(246, 52);
            button6.TabIndex = 6;
            button6.Text = "\u23fb  Logout";
            button6.UseVisualStyleBackColor = true;

            // button5
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.ForeColor = Color.Maroon;
            button5.Location = new Point(-63, 336);
            button5.Name = "button5";
            button5.Size = new Size(280, 43);
            button5.TabIndex = 5;
            button5.Text = "💰 Fines";
            button5.UseVisualStyleBackColor = true;

            // button4
            button4.FlatAppearance.BorderColor = Color.White;
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatAppearance.MouseDownBackColor = Color.Gray;
            button4.FlatAppearance.MouseOverBackColor = Color.LightGray;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.Maroon;
            button4.Location = new Point(-40, 276);
            button4.Margin = new Padding(3, 4, 3, 4);
            button4.Name = "button4";
            button4.Size = new Size(280, 43);
            button4.TabIndex = 4;
            button4.Text = "📚  Borrowing";
            button4.UseVisualStyleBackColor = true;

            // button3
            button3.FlatAppearance.BorderColor = Color.White;
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.MouseDownBackColor = Color.Gray;
            button3.FlatAppearance.MouseOverBackColor = Color.LightGray;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.Maroon;
            button3.Location = new Point(-57, 217);
            button3.Margin = new Padding(3, 4, 3, 4);
            button3.Name = "button3";
            button3.Size = new Size(280, 43);
            button3.TabIndex = 3;
            button3.Text = "📘  Books";
            button3.UseVisualStyleBackColor = true;

            // button2
            button2.FlatAppearance.BorderColor = Color.White;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.MouseDownBackColor = Color.Gray;
            button2.FlatAppearance.MouseOverBackColor = Color.LightGray;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.Maroon;
            button2.Location = new Point(-59, 159);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(280, 43);
            button2.TabIndex = 2;
            button2.Text = "👥  User ";
            button2.UseVisualStyleBackColor = true;

            // button1
            button1.FlatAppearance.BorderColor = Color.White;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.MouseDownBackColor = Color.Gray;
            button1.FlatAppearance.MouseOverBackColor = Color.LightGray;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.Maroon;
            button1.Location = new Point(-41, 105);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(280, 43);
            button1.TabIndex = 1;
            button1.Text = "📊  Dashboard";
            button1.UseVisualStyleBackColor = true;

            // panelContent
            panelContent.BackColor = Color.Snow;
            panelContent.Location = new Point(232, 78);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1138, 629);
            panelContent.TabIndex = 13;

            // topbar
            topbar.BackColor = Color.WhiteSmoke;
            topbar.Controls.Add(dropdownarrow);
            topbar.Controls.Add(username);
            topbar.Controls.Add(ProfileImage);
            topbar.Controls.Add(label1);
            topbar.Controls.Add(pictureBox1);
            topbar.Location = new Point(232, 0);
            topbar.Margin = new Padding(3, 4, 3, 4);
            topbar.Name = "topbar";
            topbar.Size = new Size(1138, 78);
            topbar.TabIndex = 1;
            topbar.Paint += topbar_Paint;

            // dropdownarrow
            dropdownarrow.ContextMenuStrip = contextMenuStrip1;
            dropdownarrow.FlatAppearance.BorderSize = 0;
            dropdownarrow.FlatStyle = FlatStyle.Flat;
            dropdownarrow.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dropdownarrow.ForeColor = Color.Maroon;
            dropdownarrow.Location = new Point(1100, 22);
            dropdownarrow.Name = "dropdownarrow";
            dropdownarrow.Size = new Size(24, 30);
            dropdownarrow.TabIndex = 5;
            dropdownarrow.Text = "▼";
            dropdownarrow.UseVisualStyleBackColor = true;
            dropdownarrow.Click += dropdownarrow_Click;

            // contextMenuStrip1
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { profileToolStripMenuItem, settingsToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(117, 48);

            // profileToolStripMenuItem
            profileToolStripMenuItem.Name = "profileToolStripMenuItem";
            profileToolStripMenuItem.Size = new Size(116, 22);
            profileToolStripMenuItem.Text = "Profile";

            // settingsToolStripMenuItem
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(116, 22);
            settingsToolStripMenuItem.Text = "Settings";

            // username
            username.AutoSize = true;
            username.ForeColor = Color.Maroon;
            username.Location = new Point(1020, 26);
            username.Name = "username";
            username.Size = new Size(80, 20);
            username.TabIndex = 4;
            username.Text = "Username";

            // ProfileImage
            ProfileImage.Location = new Point(980, 18);
            ProfileImage.Margin = new Padding(3, 4, 3, 4);
            ProfileImage.Name = "ProfileImage";
            ProfileImage.Size = new Size(32, 32);
            ProfileImage.SizeMode = PictureBoxSizeMode.Zoom;
            ProfileImage.TabIndex = 3;
            ProfileImage.TabStop = false;
            ProfileImage.Click += pictureBox2_Click;

            // label1
            label1.AutoSize = true;
            label1.BackColor = Color.WhiteSmoke;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Maroon;
            label1.Location = new Point(100, 24);
            label1.Name = "label1";
            label1.Size = new Size(108, 20);
            label1.TabIndex = 2;
            label1.Text = "LibraFlow ERP";
            label1.Click += label1_Click;

            // pictureBox1
            pictureBox1.Location = new Point(0, 6);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(94, 60);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;

            // ClientDashboardForm
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Snow;
            ClientSize = new Size(1370, 707);
            Controls.Add(panelContent);
            Controls.Add(topbar);
            Controls.Add(Sidebar);
            Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ForeColor = Color.Sienna;
            Margin = new Padding(3, 4, 3, 4);
            Name = "ClientDashboardForm";
            Text = "Client Dashboard";
            Load += ClientDashboardForm_Load;

            Sidebar.ResumeLayout(false);
            topbar.ResumeLayout(false);
            topbar.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ProfileImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        private Panel Sidebar;
        private Panel topbar;
        private Button button1;
        private Button button4;
        private Button button3;
        private Button button2;
        private Button button5;
        private Button button6;
        private Button button7;
        private Panel panelContent;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem profileToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private Button dropdownarrow;
        private Label username;
        private PictureBox ProfileImage;
        private Label label1;
        private PictureBox pictureBox1;
    }
}