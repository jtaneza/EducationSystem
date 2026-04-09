using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EducationSystem
{
    public partial class ClientDashboardForm : Form
    {
        private System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        private bool isLoggingOut = false;

        private Panel recentMembersPanel = null!;
        private Panel recentBooksPanel = null!;
        private Label lblRecentMembersTitle = null!;
        private Label lblRecentBooksTitle = null!;
        private ListView lvRecentMembers = null!;
        private ListView lvRecentBooks = null!;
        private Chart chart1 = null!;

        public ClientDashboardForm()
        {
            InitializeComponent();

            topbar.Resize += (s, e) => UpdateHeaderUserLayout();
            this.Resize += (s, e) => UpdateHeaderUserLayout();

            profileToolStripMenuItem.Click += profileToolStripMenuItem_Click;
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;

            button1.Click += button1_Click_1;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
            button5.Click += button5_Click;
            button6.Click += button6_Click;
            button7.Click += ArchiveButton_Click;

            StyleSidebarButton(button1);
            StyleSidebarButton(button2);
            StyleSidebarButton(button3);
            StyleSidebarButton(button4);
            StyleSidebarButton(button5);
            StyleSidebarButton(button6);
            StyleSidebarButton(button7);

            StyleDashboardCard(TotalMembers);
            StyleDashboardCard(TotalBooks);
            StyleDashboardCard(BorrowedBooks);
            StyleDashboardCard(OverdueBooks);

            NormalizeSidebarButtons();
            BuildCardContent();
            BuildChartPanel();
            BuildDashboardDetails();
        }

        private void ClientDashboardForm_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            ApplyTheme("Light");
            StartClock();
            RefreshDashboardCounts();
            LoadLineChart();
            LoadDummyTables();
            LoadCardIcons();

            label4.Text = "Client Dashboard";
            panelContent.Visible = false;
        }

        private void NormalizeSidebarButtons()
        {
            Button[] buttons = { button1, button2, button3, button4, button5, button7, button6 };
            int[] tops = { 105, 159, 217, 276, 336, 402, 636 };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Location = new Point(0, tops[i]);
                buttons[i].Size = new Size(226, i == 6 ? 52 : 43);
                buttons[i].TextAlign = ContentAlignment.MiddleLeft;
                buttons[i].Padding = new Padding(22, 0, 0, 0);
                buttons[i].FlatStyle = FlatStyle.Flat;
                buttons[i].FlatAppearance.BorderSize = 0;
            }
        }

        private void BuildCardContent()
        {
            BuildSingleCard(TotalMembers, users, label3, label5, "Total Members", "0");
            BuildSingleCard(TotalBooks, books, label6, label7, "Total Books", "0");
            BuildSingleCard(BorrowedBooks, activity, label8, label9, "Borrowed Books", "0");
            BuildSingleCard(OverdueBooks, pictureBox5, label10, label11, "Overdue Books", "0");
        }

        private void BuildSingleCard(Panel card, PictureBox iconBox, Label titleLabel, Label valueLabel, string title, string value)
        {
            card.BackColor = Color.Maroon;

            titleLabel.Text = title;
            valueLabel.Text = value;

            titleLabel.ForeColor = Color.WhiteSmoke;
            titleLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            valueLabel.ForeColor = Color.White;
            valueLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);

            iconBox.SizeMode = PictureBoxSizeMode.Zoom;
            iconBox.BackColor = Color.Maroon;

            if (!card.Controls.OfType<Panel>().Any(p => p.Name == "accent"))
            {
                Panel accent = new Panel();
                accent.Name = "accent";
                accent.BackColor = Color.Goldenrod;
                accent.Width = 6;
                accent.Dock = DockStyle.Left;

                card.Controls.Add(accent);
                accent.BringToFront();
            }
        }

        private void BuildChartPanel()
        {
            chart1 = new Chart();
            chart1.BackColor = Color.WhiteSmoke;
            chart1.Location = new Point(254, 279);
            chart1.Size = new Size(1090, 190);
            chart1.BorderlineColor = Color.Gainsboro;
            chart1.BorderlineDashStyle = ChartDashStyle.Solid;
            chart1.BorderlineWidth = 1;

            ChartArea area = new ChartArea("MainArea");
            area.BackColor = Color.WhiteSmoke;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisX.LineColor = Color.Silver;
            area.AxisY.LineColor = Color.Silver;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            chart1.ChartAreas.Add(area);

            Legend legend = new Legend("Legend1");
            legend.Enabled = false;
            chart1.Legends.Add(legend);

            Title title = new Title("Library Activity Trend");
            title.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            title.ForeColor = Color.Maroon;
            chart1.Titles.Add(title);

            Controls.Add(chart1);
        }

        private void LoadLineChart()
        {
            chart1.Series.Clear();

            Series series = new Series("Activity");
            series.ChartType = SeriesChartType.Line;
            series.BorderWidth = 3;
            series.Color = Color.RoyalBlue;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 7;

            series.Points.AddXY("Mon", 8);
            series.Points.AddXY("Tue", 15);
            series.Points.AddXY("Wed", 11);
            series.Points.AddXY("Thu", 14);
            series.Points.AddXY("Fri", 9);

            chart1.Series.Add(series);
        }

        private void BuildDashboardDetails()
        {
            recentMembersPanel = new Panel();
            recentMembersPanel.BackColor = Color.WhiteSmoke;
            recentMembersPanel.Location = new Point(254, 485);
            recentMembersPanel.Size = new Size(540, 170);
            recentMembersPanel.BorderStyle = BorderStyle.None;

            lblRecentMembersTitle = new Label();
            lblRecentMembersTitle.Text = "Recent Members";
            lblRecentMembersTitle.ForeColor = Color.Maroon;
            lblRecentMembersTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblRecentMembersTitle.AutoSize = true;
            lblRecentMembersTitle.Location = new Point(18, 14);

            lvRecentMembers = new ListView();
            lvRecentMembers.Location = new Point(18, 42);
            lvRecentMembers.Size = new Size(500, 110);
            lvRecentMembers.View = View.Details;
            lvRecentMembers.FullRowSelect = true;
            lvRecentMembers.GridLines = false;
            lvRecentMembers.BorderStyle = BorderStyle.FixedSingle;
            lvRecentMembers.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lvRecentMembers.Columns.Add("Member Name", 210);
            lvRecentMembers.Columns.Add("ID", 90);
            lvRecentMembers.Columns.Add("Status", 160);
            lvRecentMembers.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            recentMembersPanel.Controls.Add(lblRecentMembersTitle);
            recentMembersPanel.Controls.Add(lvRecentMembers);

            recentBooksPanel = new Panel();
            recentBooksPanel.BackColor = Color.WhiteSmoke;
            recentBooksPanel.Location = new Point(804, 485);
            recentBooksPanel.Size = new Size(540, 170);
            recentBooksPanel.BorderStyle = BorderStyle.None;

            lblRecentBooksTitle = new Label();
            lblRecentBooksTitle.Text = "Recent Books";
            lblRecentBooksTitle.ForeColor = Color.Maroon;
            lblRecentBooksTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblRecentBooksTitle.AutoSize = true;
            lblRecentBooksTitle.Location = new Point(18, 14);

            lvRecentBooks = new ListView();
            lvRecentBooks.Location = new Point(18, 42);
            lvRecentBooks.Size = new Size(500, 110);
            lvRecentBooks.View = View.Details;
            lvRecentBooks.FullRowSelect = true;
            lvRecentBooks.GridLines = false;
            lvRecentBooks.BorderStyle = BorderStyle.FixedSingle;
            lvRecentBooks.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lvRecentBooks.Columns.Add("Book Title", 220);
            lvRecentBooks.Columns.Add("Code", 90);
            lvRecentBooks.Columns.Add("Status", 150);
            lvRecentBooks.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            recentBooksPanel.Controls.Add(lblRecentBooksTitle);
            recentBooksPanel.Controls.Add(lvRecentBooks);

            Controls.Add(recentMembersPanel);
            Controls.Add(recentBooksPanel);
        }

        private void LoadDummyTables()
        {
            lvRecentMembers.Items.Clear();
            lvRecentBooks.Items.Clear();

            lvRecentMembers.Items.Add(new ListViewItem(new[] { "John Cruz", "MB001", "Active" }));
            lvRecentMembers.Items.Add(new ListViewItem(new[] { "Maria Santos", "MB002", "Active" }));
            lvRecentMembers.Items.Add(new ListViewItem(new[] { "Paolo Reyes", "MB003", "Pending" }));
            lvRecentMembers.Items.Add(new ListViewItem(new[] { "Anne Flores", "MB004", "Active" }));

            lvRecentBooks.Items.Add(new ListViewItem(new[] { "Introduction to C#", "BK001", "Available" }));
            lvRecentBooks.Items.Add(new ListViewItem(new[] { "Database Systems", "BK002", "Borrowed" }));
            lvRecentBooks.Items.Add(new ListViewItem(new[] { "Operating Systems", "BK003", "Available" }));
            lvRecentBooks.Items.Add(new ListViewItem(new[] { "Software Engineering", "BK004", "Borrowed" }));
        }

        private void StyleSidebarButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(210, 210, 210);
            btn.Cursor = Cursors.Hand;
        }

        private void StyleDashboardCard(Panel card)
        {
            card.Cursor = Cursors.Hand;

            card.MouseEnter += (s, e) => { card.BackColor = Color.FromArgb(120, 0, 0); };
            card.MouseLeave += (s, e) => { card.BackColor = Color.Maroon; };
        }

        private void RefreshDashboardCounts()
        {
            label5.Text = "0";
            label7.Text = "0";
            label9.Text = "0";
            label11.Text = "0";
            label4.Text = "Client Dashboard";
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

        private void UpdateHeaderUserLayout()
        {
            int rightMargin = 18;

            username.AutoSize = true;
            username.MaximumSize = new Size(350, 24);
            username.TextAlign = ContentAlignment.MiddleLeft;

            dropdownarrow.AutoSize = false;
            dropdownarrow.Size = new Size(24, 24);
            dropdownarrow.Location = new Point(
                topbar.Width - dropdownarrow.Width - rightMargin,
                18
            );

            username.Location = new Point(
                dropdownarrow.Left - username.PreferredWidth - 6,
                18
            );

            ProfileImage.Location = new Point(
                username.Left - ProfileImage.Width - 8,
                10
            );
        }

        public void LoadUserInfo()
        {
            username.Text = string.IsNullOrWhiteSpace(ClientSession.Username)
                ? "Client Admin"
                : ClientSession.Username;

            try
            {
                string profilePath = "";

                if (!string.IsNullOrWhiteSpace(ClientSession.ImagePath) &&
                    File.Exists(ClientSession.ImagePath))
                {
                    profilePath = ClientSession.ImagePath;
                }
                else
                {
                    profilePath = FindAssetPath("client.png");
                }

                if (!string.IsNullOrWhiteSpace(profilePath) && File.Exists(profilePath))
                {
                    if (ProfileImage.Image != null)
                    {
                        var old = ProfileImage.Image;
                        ProfileImage.Image = null;
                        old.Dispose();
                    }

                    using (var fs = new FileStream(profilePath, FileMode.Open, FileAccess.Read))
                    {
                        ProfileImage.Image = Image.FromStream(fs);
                    }
                }
                else
                {
                    ProfileImage.Image = null;
                }

                ProfileImage.SizeMode = PictureBoxSizeMode.Zoom;
                ProfileImage.BackColor = Color.WhiteSmoke;
            }
            catch
            {
                ProfileImage.Image = null;
            }

            try
            {
                string logoPath = FindAssetPath("logo_light.png");

                if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
                {
                    if (pictureBox1.Image != null)
                    {
                        var old = pictureBox1.Image;
                        pictureBox1.Image = null;
                        old.Dispose();
                    }

                    using (var fs = new FileStream(logoPath, FileMode.Open, FileAccess.Read))
                    {
                        pictureBox1.Image = Image.FromStream(fs);
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                }

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.BackColor = Color.WhiteSmoke;
            }
            catch
            {
                pictureBox1.Image = null;
            }

            label1.ForeColor = Color.Maroon;
            label1.BackColor = Color.WhiteSmoke;

            UpdateHeaderUserLayout();
        }

        private void LoadCardIcons()
        {
            try
            {
                string membersPath = FindAssetPath("members.png");
                string booksPath = FindAssetPath("books.png");
                string borrowPath = FindAssetPath("borrow.png");
                string overduePath = FindAssetPath("overdue.png");

                if (!string.IsNullOrWhiteSpace(membersPath))
                    users.Image = Image.FromFile(membersPath);

                if (!string.IsNullOrWhiteSpace(booksPath))
                    books.Image = Image.FromFile(booksPath);

                if (!string.IsNullOrWhiteSpace(borrowPath))
                    activity.Image = Image.FromFile(borrowPath);

                if (!string.IsNullOrWhiteSpace(overduePath))
                    pictureBox5.Image = Image.FromFile(overduePath);

                users.SizeMode = PictureBoxSizeMode.Zoom;
                books.SizeMode = PictureBoxSizeMode.Zoom;
                activity.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;

                users.BackColor = Color.Maroon;
                books.BackColor = Color.Maroon;
                activity.BackColor = Color.Maroon;
                pictureBox5.BackColor = Color.Maroon;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading icons: " + ex.Message);
            }
        }

        private void StartClock()
        {
            clockTimer.Interval = 1000;
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();
            UpdateDateTime();
        }

        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            time.Text = DateTime.Now.ToString("MMMM dd, yyyy - hh:mm:ss tt");
            time.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            time.ForeColor = Color.Maroon;
            time.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void ShowDashboardHome()
        {
            RefreshDashboardCounts();
            LoadLineChart();
            LoadDummyTables();
            LoadUserInfo();
            LoadCardIcons();

            label4.Visible = true;
            TotalMembers.Visible = true;
            TotalBooks.Visible = true;
            BorrowedBooks.Visible = true;
            OverdueBooks.Visible = true;
            chart1.Visible = true;
            recentMembersPanel.Visible = true;
            recentBooksPanel.Visible = true;
            panel1.Visible = true;

            panelContent.Controls.Clear();
            panelContent.Visible = false;
        }

        private void HideDashboardHome()
        {
            label4.Visible = false;
            TotalMembers.Visible = false;
            TotalBooks.Visible = false;
            BorrowedBooks.Visible = false;
            OverdueBooks.Visible = false;
            if (chart1 != null) chart1.Visible = false;
            if (recentMembersPanel != null) recentMembersPanel.Visible = false;
            if (recentBooksPanel != null) recentBooksPanel.Visible = false;
            panel1.Visible = true;
        }

        public void LoadContentForm(Form form)
        {
            HideDashboardHome();

            panelContent.Controls.Clear();
            panelContent.Visible = true;
            panelContent.BringToFront();
            panel1.BringToFront();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelContent.Controls.Add(form);
            form.Show();
        }

        public void ApplyTheme(string theme)
        {
            BackColor = Color.Snow;
            Sidebar.BackColor = Color.WhiteSmoke;
            topbar.BackColor = Color.WhiteSmoke;
            panel1.BackColor = Color.WhiteSmoke;
            panelContent.BackColor = Color.Snow;

            label1.ForeColor = Color.Maroon;
            label1.BackColor = Color.WhiteSmoke;

            username.ForeColor = Color.Maroon;
            username.BackColor = Color.WhiteSmoke;

            time.ForeColor = Color.Maroon;
            time.BackColor = Color.WhiteSmoke;

            label4.ForeColor = Color.Maroon;
            dropdownarrow.ForeColor = Color.Maroon;
            dropdownarrow.BackColor = Color.WhiteSmoke;

            pictureBox1.BackColor = Color.WhiteSmoke;
            ProfileImage.BackColor = Color.WhiteSmoke;

            foreach (Control ctrl in Sidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.ForeColor = Color.Maroon;
                    btn.BackColor = Color.WhiteSmoke;
                    btn.FlatAppearance.MouseOverBackColor = Color.LightGray;
                    btn.FlatAppearance.MouseDownBackColor = Color.Gray;
                }
            }

            contextMenuStrip1.BackColor = Color.White;
            contextMenuStrip1.ForeColor = Color.Black;

            foreach (ToolStripItem item in contextMenuStrip1.Items)
            {
                item.BackColor = Color.White;
                item.ForeColor = Color.Black;
            }

            TotalMembers.BackColor = Color.Maroon;
            TotalBooks.BackColor = Color.Maroon;
            BorrowedBooks.BackColor = Color.Maroon;
            OverdueBooks.BackColor = Color.Maroon;

            if (recentMembersPanel != null) recentMembersPanel.BackColor = Color.WhiteSmoke;
            if (recentBooksPanel != null) recentBooksPanel.BackColor = Color.WhiteSmoke;
            if (chart1 != null) chart1.BackColor = Color.WhiteSmoke;
        }

        private void DoLogout()
        {
            if (isLoggingOut) return;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            isLoggingOut = true;
            ClientSession.Clear();

            LoginForm? login = Application.OpenForms.OfType<LoginForm>().FirstOrDefault();

            if (login == null || login.IsDisposed)
                login = new LoginForm();

            login.Show();
            login.BringToFront();
            login.WindowState = FormWindowState.Normal;

            Close();
        }

        private void button1_Click_1(object? sender, EventArgs e)
        {
            ShowDashboardHome();
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Members module next.");
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Books module next.");
        }

        private void button4_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Borrowing module next.");
        }

        private void button5_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Reports module next.");
        }

        private void button6_Click(object? sender, EventArgs e)
        {
            DoLogout();
        }

        private void ArchiveButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Archive module next.");
        }

        private void dropdownarrow_Click(object? sender, EventArgs e)
        {
            contextMenuStrip1.Show(dropdownarrow, 0, dropdownarrow.Height);
        }

        private void profileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new ClientProfileForm());
        }

        private void settingsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new ClientSettingsForm());
        }

        private void pictureBox2_Click(object? sender, EventArgs e)
        {
            contextMenuStrip1.Show(ProfileImage, 0, ProfileImage.Height);
        }

        private void topbar_Paint(object? sender, PaintEventArgs e)
        {
        }

        private void label1_Click(object? sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object? sender, EventArgs e)
        {
        }

        private void label3_Click(object? sender, EventArgs e)
        {
        }

        private void pictureBox2_Click_1(object? sender, EventArgs e)
        {
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (clockTimer != null)
            {
                clockTimer.Stop();
                clockTimer.Dispose();
            }

            base.OnFormClosed(e);
        }

        private void BorrowedBooks_Paint(object sender, PaintEventArgs e)
        {
        }

        private void TotalMembers_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}