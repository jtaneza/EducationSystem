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
            time = new Label();
            label4 = new Label();
            TotalMembers = new Panel();
            label5 = new Label();
            label3 = new Label();
            users = new PictureBox();
            TotalBooks = new Panel();
            label7 = new Label();
            label6 = new Label();
            books = new PictureBox();
            BorrowedBooks = new Panel();
            label9 = new Label();
            label8 = new Label();
            activity = new PictureBox();
            OverdueBooks = new Panel();
            label11 = new Label();
            label10 = new Label();
            pictureBox5 = new PictureBox();
            panel1 = new Panel();
            Sidebar.SuspendLayout();
            topbar.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ProfileImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            TotalMembers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)users).BeginInit();
            TotalBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)books).BeginInit();
            BorrowedBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)activity).BeginInit();
            OverdueBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Sidebar
            // 
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
            Sidebar.Location = new Point(-6, 1);
            Sidebar.Margin = new Padding(3, 4, 3, 4);
            Sidebar.Name = "Sidebar";
            Sidebar.Size = new Size(232, 705);
            Sidebar.TabIndex = 0;
            // 
            // button7
            // 
            button7.FlatAppearance.BorderSize = 0;
            button7.FlatStyle = FlatStyle.Flat;
            button7.ForeColor = Color.Maroon;
            button7.Location = new Point(-51, 402);
            button7.Name = "button7";
            button7.Size = new Size(280, 43);
            button7.TabIndex = 7;
            button7.Text = "📥  Archive";
            button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
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
            // 
            // button5
            // 
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.ForeColor = Color.Maroon;
            button5.Location = new Point(-52, 336);
            button5.Name = "button5";
            button5.Size = new Size(280, 43);
            button5.TabIndex = 5;
            button5.Text = "📃  Reports";
            button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
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
            // 
            // button3
            // 
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
            // 
            // button2
            // 
            button2.FlatAppearance.BorderColor = Color.White;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.MouseDownBackColor = Color.Gray;
            button2.FlatAppearance.MouseOverBackColor = Color.LightGray;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.Maroon;
            button2.Location = new Point(-45, 159);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(280, 43);
            button2.TabIndex = 2;
            button2.Text = "👥  Members";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
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
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.Snow;
            panelContent.Location = new Point(225, 53);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1148, 826);
            panelContent.TabIndex = 13;
            panelContent.Visible = false;
            // 
            // topbar
            // 
            topbar.BackColor = Color.WhiteSmoke;
            topbar.Controls.Add(dropdownarrow);
            topbar.Controls.Add(username);
            topbar.Controls.Add(ProfileImage);
            topbar.Controls.Add(label1);
            topbar.Controls.Add(pictureBox1);
            topbar.Location = new Point(-1, 1);
            topbar.Margin = new Padding(3, 4, 3, 4);
            topbar.Name = "topbar";
            topbar.Size = new Size(1370, 54);
            topbar.TabIndex = 1;
            topbar.Paint += topbar_Paint;
            // 
            // dropdownarrow
            // 
            dropdownarrow.ContextMenuStrip = contextMenuStrip1;
            dropdownarrow.FlatAppearance.BorderSize = 0;
            dropdownarrow.FlatStyle = FlatStyle.Flat;
            dropdownarrow.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dropdownarrow.ForeColor = Color.Maroon;
            dropdownarrow.Location = new Point(1330, 15);
            dropdownarrow.Name = "dropdownarrow";
            dropdownarrow.Size = new Size(24, 30);
            dropdownarrow.TabIndex = 5;
            dropdownarrow.Text = "▼";
            dropdownarrow.UseVisualStyleBackColor = true;
            dropdownarrow.Click += dropdownarrow_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { profileToolStripMenuItem, settingsToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(117, 48);
            // 
            // profileToolStripMenuItem
            // 
            profileToolStripMenuItem.Name = "profileToolStripMenuItem";
            profileToolStripMenuItem.Size = new Size(116, 22);
            profileToolStripMenuItem.Text = "Profile";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(116, 22);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // username
            // 
            username.AutoSize = true;
            username.ForeColor = Color.Maroon;
            username.Location = new Point(1251, 18);
            username.Name = "username";
            username.Size = new Size(80, 20);
            username.TabIndex = 4;
            username.Text = "Username";
            // 
            // ProfileImage
            // 
            ProfileImage.Location = new Point(1211, 8);
            ProfileImage.Margin = new Padding(3, 4, 3, 4);
            ProfileImage.Name = "ProfileImage";
            ProfileImage.Size = new Size(36, 37);
            ProfileImage.SizeMode = PictureBoxSizeMode.Zoom;
            ProfileImage.TabIndex = 3;
            ProfileImage.TabStop = false;
            ProfileImage.Click += pictureBox2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.WhiteSmoke;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Maroon;
            label1.Location = new Point(104, 18);
            label1.Name = "label1";
            label1.Size = new Size(108, 20);
            label1.TabIndex = 2;
            label1.Text = "LibraFlow ERP";
            label1.Click += label1_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.ErrorImage = null;
            pictureBox1.Location = new Point(2, -6);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(118, 66);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // time
            // 
            time.AutoSize = true;
            time.BackColor = Color.WhiteSmoke;
            time.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            time.ForeColor = Color.Maroon;
            time.Location = new Point(20, 19);
            time.Name = "time";
            time.Size = new Size(168, 20);
            time.TabIndex = 6;
            time.Text = "Mm/dd/ yy - hh/mm/ss";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.Maroon;
            label4.Location = new Point(251, 91);
            label4.Name = "label4";
            label4.Size = new Size(129, 20);
            label4.TabIndex = 6;
            label4.Text = "Client Dashboard";
            // 
            // TotalMembers
            // 
            TotalMembers.BackColor = Color.Maroon;
            TotalMembers.Controls.Add(label5);
            TotalMembers.Controls.Add(label3);
            TotalMembers.Controls.Add(users);
            TotalMembers.Location = new Point(254, 143);
            TotalMembers.Name = "TotalMembers";
            TotalMembers.Size = new Size(254, 95);
            TotalMembers.TabIndex = 7;
            TotalMembers.Paint += TotalMembers_Paint;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.WhiteSmoke;
            label5.Location = new Point(159, 54);
            label5.Name = "label5";
            label5.Size = new Size(18, 20);
            label5.TabIndex = 2;
            label5.Text = "0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.WhiteSmoke;
            label3.Location = new Point(115, 30);
            label3.Name = "label3";
            label3.Size = new Size(114, 20);
            label3.TabIndex = 1;
            label3.Text = "Total Members";
            label3.Click += label3_Click;
            // 
            // users
            // 
            users.BackColor = Color.Maroon;
            users.Location = new Point(19, 18);
            users.Name = "users";
            users.Size = new Size(99, 61);
            users.SizeMode = PictureBoxSizeMode.Zoom;
            users.TabIndex = 0;
            users.TabStop = false;
            users.Click += pictureBox2_Click_1;
            // 
            // TotalBooks
            // 
            TotalBooks.BackColor = Color.Maroon;
            TotalBooks.Controls.Add(label7);
            TotalBooks.Controls.Add(label6);
            TotalBooks.Controls.Add(books);
            TotalBooks.ForeColor = Color.SeaShell;
            TotalBooks.Location = new Point(532, 143);
            TotalBooks.Name = "TotalBooks";
            TotalBooks.Size = new Size(254, 95);
            TotalBooks.TabIndex = 8;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.WhiteSmoke;
            label7.Location = new Point(167, 51);
            label7.Name = "label7";
            label7.Size = new Size(18, 20);
            label7.TabIndex = 2;
            label7.Text = "0";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.WhiteSmoke;
            label6.Location = new Point(130, 29);
            label6.Name = "label6";
            label6.Size = new Size(91, 20);
            label6.TabIndex = 1;
            label6.Text = "Total Books";
            // 
            // books
            // 
            books.BackColor = Color.Maroon;
            books.Location = new Point(20, 18);
            books.Name = "books";
            books.Size = new Size(99, 61);
            books.SizeMode = PictureBoxSizeMode.Zoom;
            books.TabIndex = 0;
            books.TabStop = false;
            // 
            // BorrowedBooks
            // 
            BorrowedBooks.BackColor = Color.Maroon;
            BorrowedBooks.Controls.Add(label9);
            BorrowedBooks.Controls.Add(label8);
            BorrowedBooks.Controls.Add(activity);
            BorrowedBooks.ForeColor = Color.SeaShell;
            BorrowedBooks.Location = new Point(811, 143);
            BorrowedBooks.Name = "BorrowedBooks";
            BorrowedBooks.Size = new Size(254, 95);
            BorrowedBooks.TabIndex = 9;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = Color.WhiteSmoke;
            label9.Location = new Point(167, 51);
            label9.Name = "label9";
            label9.Size = new Size(18, 20);
            label9.TabIndex = 2;
            label9.Text = "0";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.ForeColor = Color.WhiteSmoke;
            label8.Location = new Point(111, 30);
            label8.Name = "label8";
            label8.Size = new Size(125, 20);
            label8.TabIndex = 1;
            label8.Text = "Borrowed Books";
            // 
            // activity
            // 
            activity.BackColor = Color.Maroon;
            activity.Location = new Point(20, 19);
            activity.Name = "activity";
            activity.Size = new Size(99, 61);
            activity.SizeMode = PictureBoxSizeMode.Zoom;
            activity.TabIndex = 0;
            activity.TabStop = false;
            // 
            // OverdueBooks
            // 
            OverdueBooks.BackColor = Color.Maroon;
            OverdueBooks.Controls.Add(label11);
            OverdueBooks.Controls.Add(label10);
            OverdueBooks.Controls.Add(pictureBox5);
            OverdueBooks.Location = new Point(1089, 143);
            OverdueBooks.Name = "OverdueBooks";
            OverdueBooks.Size = new Size(254, 95);
            OverdueBooks.TabIndex = 10;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.ForeColor = Color.WhiteSmoke;
            label11.Location = new Point(165, 54);
            label11.Name = "label11";
            label11.Size = new Size(18, 20);
            label11.TabIndex = 2;
            label11.Text = "0";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.FlatStyle = FlatStyle.Flat;
            label10.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.ForeColor = Color.WhiteSmoke;
            label10.Location = new Point(114, 27);
            label10.Name = "label10";
            label10.Size = new Size(115, 20);
            label10.TabIndex = 1;
            label10.Text = "Overdue Books";
            // 
            // pictureBox5
            // 
            pictureBox5.BackColor = Color.Maroon;
            pictureBox5.Location = new Point(20, 19);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(99, 61);
            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.TabIndex = 0;
            pictureBox5.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.WhiteSmoke;
            panel1.Controls.Add(time);
            panel1.ForeColor = Color.PeachPuff;
            panel1.Location = new Point(226, 637);
            panel1.Name = "panel1";
            panel1.Size = new Size(1147, 59);
            panel1.TabIndex = 14;
            // 
            // ClientDashboardForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Snow;
            ClientSize = new Size(1370, 707);
            Controls.Add(panel1);
            Controls.Add(panelContent);
            Controls.Add(OverdueBooks);
            Controls.Add(BorrowedBooks);
            Controls.Add(TotalBooks);
            Controls.Add(TotalMembers);
            Controls.Add(label4);
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
            TotalMembers.ResumeLayout(false);
            TotalMembers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)users).EndInit();
            TotalBooks.ResumeLayout(false);
            TotalBooks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)books).EndInit();
            BorrowedBooks.ResumeLayout(false);
            BorrowedBooks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)activity).EndInit();
            OverdueBooks.ResumeLayout(false);
            OverdueBooks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel Sidebar;
        private Panel topbar;
        private Label label1;
        private PictureBox pictureBox1;
        private Button button1;
        private Button button4;
        private Button button3;
        private Button button2;
        private PictureBox ProfileImage;
        private Label username;
        private Label label4;
        private Button button5;
        private Button button6;
        private Button dropdownarrow;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem profileToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private Panel TotalMembers;
        private Panel TotalBooks;
        private Panel BorrowedBooks;
        private Panel OverdueBooks;
        private PictureBox books;
        private PictureBox activity;
        private PictureBox pictureBox5;
        private Label label5;
        private Label label3;
        private Label label6;
        private Label label7;
        private Label label9;
        private Label label8;
        private Label label11;
        private Label label10;
        private Label time;
        private PictureBox users;
        private Panel panelContent;
        private Button button7;
        private Panel panel1;
    }
}