namespace EducationSystem
{
    partial class DashboardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardForm));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            Sidebar = new Panel();
            button8 = new Button();
            button7 = new Button();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
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
            label2 = new Label();
            TotalClients = new Panel();
            label5 = new Label();
            label3 = new Label();
            users = new PictureBox();
            TotalMembers = new Panel();
            label7 = new Label();
            label6 = new Label();
            books = new PictureBox();
            TotalBooks = new Panel();
            label9 = new Label();
            label8 = new Label();
            activity = new PictureBox();
            ActiveBorrowings = new Panel();
            label11 = new Label();
            label10 = new Label();
            pictureBox5 = new PictureBox();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            panelContent = new Panel();
            panel1 = new Panel();
            Sidebar.SuspendLayout();
            topbar.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ProfileImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            TotalClients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)users).BeginInit();
            TotalMembers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)books).BeginInit();
            TotalBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)activity).BeginInit();
            ActiveBorrowings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart2).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Sidebar
            // 
            Sidebar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Sidebar.BackColor = Color.SeaShell;
            Sidebar.BorderStyle = BorderStyle.Fixed3D;
            Sidebar.Controls.Add(button8);
            Sidebar.Controls.Add(button7);
            Sidebar.Controls.Add(button6);
            Sidebar.Controls.Add(button5);
            Sidebar.Controls.Add(button4);
            Sidebar.Controls.Add(button3);
            Sidebar.Controls.Add(button2);
            Sidebar.Controls.Add(button1);
            Sidebar.ForeColor = Color.Maroon;
            Sidebar.Location = new Point(-6, -4);
            Sidebar.Margin = new Padding(3, 4, 3, 4);
            Sidebar.Name = "Sidebar";
            Sidebar.Size = new Size(232, 718);
            Sidebar.TabIndex = 0;
            // 
            // button8
            // 
            button8.FlatAppearance.BorderSize = 0;
            button8.FlatStyle = FlatStyle.Flat;
            button8.ForeColor = Color.Maroon;
            button8.Location = new Point(-41, 337);
            button8.Name = "button8";
            button8.Size = new Size(270, 43);
            button8.TabIndex = 8;
            button8.Text = "🔍  Oversight";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button7
            // 
            button7.FlatAppearance.BorderSize = 0;
            button7.FlatStyle = FlatStyle.Flat;
            button7.ForeColor = Color.Maroon;
            button7.Location = new Point(-51, 465);
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
            button6.Location = new Point(-2, 636);
            button6.Name = "button6";
            button6.Size = new Size(226, 52);
            button6.TabIndex = 6;
            button6.Text = "\u23fb  Logout";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button5
            // 
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.ForeColor = Color.Maroon;
            button5.Location = new Point(-53, 401);
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
            button4.Location = new Point(-49, 276);
            button4.Margin = new Padding(3, 4, 3, 4);
            button4.Name = "button4";
            button4.Size = new Size(280, 43);
            button4.TabIndex = 4;
            button4.Text = "📚  Modules";
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
            button3.Location = new Point(-40, 217);
            button3.Margin = new Padding(3, 4, 3, 4);
            button3.Name = "button3";
            button3.Size = new Size(280, 43);
            button3.TabIndex = 3;
            button3.Text = "⚙  Monitoring";
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
            button2.Location = new Point(-57, 159);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(280, 43);
            button2.TabIndex = 2;
            button2.Text = "🏢  Clients";
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
            button1.Click += button1_Click_1;
            // 
            // topbar
            // 
            topbar.BackColor = Color.SeaShell;
            topbar.Controls.Add(dropdownarrow);
            topbar.Controls.Add(username);
            topbar.Controls.Add(ProfileImage);
            topbar.Controls.Add(label1);
            topbar.Controls.Add(pictureBox1);
            topbar.Location = new Point(-6, 1);
            topbar.Margin = new Padding(3, 4, 3, 4);
            topbar.Name = "topbar";
            topbar.Size = new Size(1403, 54);
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
            dropdownarrow.Location = new Point(1332, 15);
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
            username.Location = new Point(1253, 18);
            username.Name = "username";
            username.Size = new Size(80, 20);
            username.TabIndex = 4;
            username.Text = "Username";
            // 
            // ProfileImage
            // 
            ProfileImage.Image = (Image)resources.GetObject("ProfileImage.Image");
            ProfileImage.Location = new Point(1212, 8);
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
            pictureBox1.ErrorImage = (Image)resources.GetObject("pictureBox1.ErrorImage");
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
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
            label4.Size = new Size(180, 20);
            label4.TabIndex = 6;
            label4.Text = "Super Admin Dashboard";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(439, 325);
            label2.Name = "label2";
            label2.Size = new Size(0, 20);
            label2.TabIndex = 0;
            // 
            // TotalClients
            // 
            TotalClients.BackColor = Color.Maroon;
            TotalClients.Controls.Add(label5);
            TotalClients.Controls.Add(label3);
            TotalClients.Controls.Add(users);
            TotalClients.Location = new Point(254, 143);
            TotalClients.Name = "TotalClients";
            TotalClients.Size = new Size(254, 95);
            TotalClients.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.WhiteSmoke;
            label5.Location = new Point(157, 54);
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
            label3.Location = new Point(125, 30);
            label3.Name = "label3";
            label3.Size = new Size(95, 20);
            label3.TabIndex = 1;
            label3.Text = "Total Clients";
            label3.Click += label3_Click;
            // 
            // users
            // 
            users.BackColor = Color.Maroon;
            users.Image = (Image)resources.GetObject("users.Image");
            users.Location = new Point(16, 18);
            users.Name = "users";
            users.Size = new Size(99, 61);
            users.SizeMode = PictureBoxSizeMode.Zoom;
            users.TabIndex = 0;
            users.TabStop = false;
            users.Click += pictureBox2_Click_1;
            // 
            // TotalMembers
            // 
            TotalMembers.BackColor = Color.Maroon;
            TotalMembers.Controls.Add(label7);
            TotalMembers.Controls.Add(label6);
            TotalMembers.Controls.Add(books);
            TotalMembers.ForeColor = Color.SeaShell;
            TotalMembers.Location = new Point(532, 143);
            TotalMembers.Name = "TotalMembers";
            TotalMembers.Size = new Size(254, 95);
            TotalMembers.TabIndex = 8;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.WhiteSmoke;
            label7.Location = new Point(160, 51);
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
            label6.Location = new Point(120, 29);
            label6.Name = "label6";
            label6.Size = new Size(114, 20);
            label6.TabIndex = 1;
            label6.Text = "Total Members";
            // 
            // books
            // 
            books.BackColor = Color.Maroon;
            books.Image = (Image)resources.GetObject("books.Image");
            books.Location = new Point(16, 18);
            books.Name = "books";
            books.Size = new Size(99, 61);
            books.SizeMode = PictureBoxSizeMode.Zoom;
            books.TabIndex = 0;
            books.TabStop = false;
            // 
            // TotalBooks
            // 
            TotalBooks.BackColor = Color.Maroon;
            TotalBooks.Controls.Add(label9);
            TotalBooks.Controls.Add(label8);
            TotalBooks.Controls.Add(activity);
            TotalBooks.ForeColor = Color.SeaShell;
            TotalBooks.Location = new Point(811, 143);
            TotalBooks.Name = "TotalBooks";
            TotalBooks.Size = new Size(254, 95);
            TotalBooks.TabIndex = 9;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = Color.WhiteSmoke;
            label9.Location = new Point(160, 51);
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
            label8.Location = new Point(129, 30);
            label8.Name = "label8";
            label8.Size = new Size(91, 20);
            label8.TabIndex = 1;
            label8.Text = "Total Books";
            // 
            // activity
            // 
            activity.BackColor = Color.Maroon;
            activity.Image = (Image)resources.GetObject("activity.Image");
            activity.Location = new Point(19, 19);
            activity.Name = "activity";
            activity.Size = new Size(99, 61);
            activity.SizeMode = PictureBoxSizeMode.Zoom;
            activity.TabIndex = 0;
            activity.TabStop = false;
            // 
            // ActiveBorrowings
            // 
            ActiveBorrowings.BackColor = Color.Maroon;
            ActiveBorrowings.Controls.Add(label11);
            ActiveBorrowings.Controls.Add(label10);
            ActiveBorrowings.Controls.Add(pictureBox5);
            ActiveBorrowings.Location = new Point(1089, 143);
            ActiveBorrowings.Name = "ActiveBorrowings";
            ActiveBorrowings.Size = new Size(254, 95);
            ActiveBorrowings.TabIndex = 10;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.ForeColor = Color.WhiteSmoke;
            label11.Location = new Point(168, 54);
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
            label10.Location = new Point(133, 27);
            label10.Name = "label10";
            label10.Size = new Size(97, 20);
            label10.TabIndex = 1;
            label10.Text = "Transactions";
            // 
            // pictureBox5
            // 
            pictureBox5.BackColor = Color.Maroon;
            pictureBox5.Image = (Image)resources.GetObject("pictureBox5.Image");
            pictureBox5.Location = new Point(20, 19);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(99, 61);
            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.TabIndex = 0;
            pictureBox5.TabStop = false;
            // 
            // chart1
            // 
            chart1.BackColor = Color.WhiteSmoke;
            chart1.BackImageTransparentColor = Color.DimGray;
            chart1.BorderlineColor = Color.OrangeRed;
            chartArea5.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            chart1.Legends.Add(legend5);
            chart1.Location = new Point(254, 279);
            chart1.Name = "chart1";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            chart1.Series.Add(series5);
            chart1.Size = new Size(596, 300);
            chart1.TabIndex = 11;
            chart1.Text = "chart1";
            // 
            // chart2
            // 
            chartArea6.Name = "ChartArea1";
            chart2.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            chart2.Legends.Add(legend6);
            chart2.Location = new Point(896, 289);
            chart2.Name = "chart2";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            chart2.Series.Add(series6);
            chart2.Size = new Size(447, 272);
            chart2.TabIndex = 12;
            chart2.Text = "chart2";
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.Snow;
            panelContent.Location = new Point(226, 53);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1147, 581);
            panelContent.TabIndex = 13;
            panelContent.Visible = false;
            panelContent.Paint += panelContent_Paint;
            // 
            // panel1
            // 
            panel1.BackColor = Color.WhiteSmoke;
            panel1.Controls.Add(time);
            panel1.ForeColor = Color.PeachPuff;
            panel1.Location = new Point(226, 637);
            panel1.Name = "panel1";
            panel1.Size = new Size(1147, 69);
            panel1.TabIndex = 14;
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Snow;
            ClientSize = new Size(1370, 707);
            Controls.Add(panel1);
            Controls.Add(panelContent);
            Controls.Add(chart2);
            Controls.Add(chart1);
            Controls.Add(ActiveBorrowings);
            Controls.Add(TotalBooks);
            Controls.Add(TotalMembers);
            Controls.Add(TotalClients);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(topbar);
            Controls.Add(Sidebar);
            Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ForeColor = Color.Sienna;
            Margin = new Padding(3, 4, 3, 4);
            Name = "DashboardForm";
            Text = "Dashboard";
            TransparencyKey = Color.SeaShell;
            Load += DashboardForm_Load;
            Sidebar.ResumeLayout(false);
            topbar.ResumeLayout(false);
            topbar.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ProfileImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            TotalClients.ResumeLayout(false);
            TotalClients.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)users).EndInit();
            TotalMembers.ResumeLayout(false);
            TotalMembers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)books).EndInit();
            TotalBooks.ResumeLayout(false);
            TotalBooks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)activity).EndInit();
            ActiveBorrowings.ResumeLayout(false);
            ActiveBorrowings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            ((System.ComponentModel.ISupportInitialize)chart2).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel Sidebar;
        private Panel topbar;
        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
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
        private Panel TotalClients;
        private Panel TotalMembers;
        private Panel TotalBooks;
        private Panel ActiveBorrowings;
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
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private Panel panelContent;
        private Button button7;
        private Panel panel1;
        private Button button8;
    }
}