using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EducationSystem
{
    public partial class DashboardForm : Form
    {
        private System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        private bool isLoggingOut = false;

        public DashboardForm()
        {
            InitializeComponent();

            profileToolStripMenuItem.Click += profileToolStripMenuItem_Click;
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;

            button1.Click += button1_Click_1; // Dashboard
            button2.Click += button2_Click;   // Clients
            button3.Click += button3_Click;   // Monitoring
            button4.Click += button4_Click;   // Modules
            button5.Click += button5_Click;   // Reports
            button6.Click += button6_Click;   // Logout
            button7.Click += ArchiveButton_Click; // Archive

            StyleSidebarButton(button1);
            StyleSidebarButton(button2);
            StyleSidebarButton(button3);
            StyleSidebarButton(button4);
            StyleSidebarButton(button5);
            StyleSidebarButton(button6);
            StyleSidebarButton(button7);

            StyleDashboardCard(TotalClients);
            StyleDashboardCard(TotalMembers);
            StyleDashboardCard(TotalBooks);
            StyleDashboardCard(ActiveBorrowings);

            TotalClients.Click += (s, e) => LoadContentForm(new ClientsForm());
            TotalMembers.Click += (s, e) => MessageBox.Show("Members module next.");
            TotalBooks.Click += (s, e) => MessageBox.Show("Books module next.");
            ActiveBorrowings.Click += (s, e) => LoadContentForm(new MonitoringForm());

            foreach (Control c in TotalClients.Controls) c.Click += (s, e) => LoadContentForm(new ClientsForm());
            foreach (Control c in TotalMembers.Controls) c.Click += (s, e) => MessageBox.Show("Members module next.");
            foreach (Control c in TotalBooks.Controls) c.Click += (s, e) => MessageBox.Show("Books module next.");
            foreach (Control c in ActiveBorrowings.Controls) c.Click += (s, e) => LoadContentForm(new MonitoringForm());
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            ApplyTheme(UserSession.Theme);
            StartClock();
            RefreshDashboardCounts();
            LoadLineChart();
            LoadPieChart();

            label4.Text = "Super Admin Dashboard";
            panelContent.Visible = false;
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
            card.BackColor = Color.Maroon;
            card.Cursor = Cursors.Hand;

            // GOLDEN LEFT BAR (same as client)
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

            // Hover effect (optional but nice)
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(120, 0, 0);
            };

            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.Maroon;
            };
        }

        private void RefreshDashboardCounts()
        {
            int totalClientsCount = ClientArchiveStore.ActiveClients.Count;
            int totalMembersCount = 0;
            int totalBooksCount = 0;
            int totalTransactionsCount = 0;

            label5.Text = totalClientsCount.ToString();
            label7.Text = totalMembersCount.ToString();
            label9.Text = totalBooksCount.ToString();
            label11.Text = totalTransactionsCount.ToString();

            label4.Text = "Super Admin Dashboard";
        }

        public void LoadUserInfo()
        {
            username.Text = string.IsNullOrWhiteSpace(UserSession.Username)
                ? "Username"
                : UserSession.Username;

            try
            {
                if (!string.IsNullOrWhiteSpace(UserSession.ImagePath) &&
                    File.Exists(UserSession.ImagePath))
                {
                    ProfileImage.Image = Image.FromFile(UserSession.ImagePath);
                }
                else
                {
                    ProfileImage.Image = null;
                }

                ProfileImage.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch
            {
                ProfileImage.Image = null;
            }
        }

        private void ApplyBranding(bool isDark)
{
    try
    {
        string lightLogoPath = Path.Combine(Application.StartupPath, "Assets", "logo_light.png");
        string darkLogoPath = Path.Combine(Application.StartupPath, "Assets", "logo_dark.png");

        string selectedLogo = isDark ? darkLogoPath : lightLogoPath;

        if (File.Exists(selectedLogo))
        {
            if (pictureBox1.Image != null)
            {
                var oldImage = pictureBox1.Image;
                pictureBox1.Image = null;
                oldImage.Dispose();
            }

            using (var fs = new FileStream(selectedLogo, FileMode.Open, FileAccess.Read))
            {
                pictureBox1.Image = Image.FromStream(fs);
            }
        }

        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
    }
    catch
    {
        // keep existing image if loading fails
    }

    // brand text
    label1.ForeColor = isDark ? Color.WhiteSmoke : Color.Maroon;

    // VERY IMPORTANT: remove the white patch behind the title
    label1.BackColor = isDark ? Color.FromArgb(35, 35, 35) : Color.WhiteSmoke;

    // make logo area blend with topbar
    pictureBox1.BackColor = isDark ? Color.FromArgb(35, 35, 35) : Color.WhiteSmoke;
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
            time.ForeColor = UserSession.Theme == "Dark" ? Color.WhiteSmoke : Color.Maroon;
            time.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void LoadLineChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();
            chart1.Titles.Clear();

            ChartArea area = new ChartArea("MainArea");
            area.BackColor = Color.WhiteSmoke;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.LineColor = Color.Gray;
            area.AxisY.LineColor = Color.Gray;
            chart1.ChartAreas.Add(area);

            Title title = new Title("Client Activity Trend");
            title.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            title.ForeColor = Color.Maroon;
            chart1.Titles.Add(title);

            Legend legend = new Legend("MainLegend");
            legend.Enabled = false;
            chart1.Legends.Add(legend);

            Series series = new Series("Activity");
            series.ChartType = SeriesChartType.Line;
            series.BorderWidth = 3;
            series.Color = Color.Maroon;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 7;

            series.Points.AddXY("Mon", 10);
            series.Points.AddXY("Tue", 18);
            series.Points.AddXY("Wed", 12);
            series.Points.AddXY("Thu", 20);
            series.Points.AddXY("Fri", 15);

            chart1.Series.Add(series);
            chart1.BackColor = Color.WhiteSmoke;
        }

        private void LoadPieChart()
        {
            chart2.Series.Clear();
            chart2.ChartAreas.Clear();
            chart2.Legends.Clear();
            chart2.Titles.Clear();

            ChartArea area = new ChartArea("PieArea");
            area.BackColor = Color.White;
            chart2.ChartAreas.Add(area);

            Title title = new Title("Client Distribution");
            title.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            title.ForeColor = Color.Maroon;
            chart2.Titles.Add(title);

            Legend legend = new Legend("PieLegend");
            legend.Docking = Docking.Bottom;
            chart2.Legends.Add(legend);

            Series series = new Series("Status");
            series.ChartType = SeriesChartType.Doughnut;
            series.IsValueShownAsLabel = true;
            series.BorderWidth = 2;

            int active = ClientArchiveStore.ActiveClients.Count(c => c.Status == "Active");
            int inactive = ClientArchiveStore.ActiveClients.Count(c => c.Status == "Inactive");
            int archived = ClientArchiveStore.ArchivedClients.Count;

            if (active == 0 && inactive == 0 && archived == 0)
            {
                series.Points.AddXY("No Clients Yet", 1);
                series.Points[0].Color = Color.Gainsboro;
                series.Points[0].Label = "No Data";
                legend.Enabled = false;
            }
            else
            {
                legend.Enabled = true;

                series.Points.AddXY("Active Clients", active);
                series.Points.AddXY("Inactive Clients", inactive);
                series.Points.AddXY("Archived Clients", archived);

                series.Points[0].Color = Color.RoyalBlue;
                series.Points[1].Color = Color.Goldenrod;
                series.Points[2].Color = Color.Firebrick;
            }

            chart2.Series.Add(series);
            chart2.BackColor = Color.White;
        }

        private void ShowDashboardHome()
        {
            RefreshDashboardCounts();
            LoadLineChart();
            LoadPieChart();
            LoadUserInfo();
            ApplyTheme(UserSession.Theme);

            label4.Visible = true;
            TotalClients.Visible = true;
            TotalMembers.Visible = true;
            TotalBooks.Visible = true;
            ActiveBorrowings.Visible = true;
            chart1.Visible = true;
            chart2.Visible = true;
            panel1.Visible = true;

            panelContent.Controls.Clear();
            panelContent.Visible = false;
        }

        private void HideDashboardHome()
        {
            label4.Visible = false;
            TotalClients.Visible = false;
            TotalMembers.Visible = false;
            TotalBooks.Visible = false;
            ActiveBorrowings.Visible = false;
            chart1.Visible = false;
            chart2.Visible = false;

            // footer stays visible
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

            UserSession.Username = null;
            UserSession.Role = null;
            UserSession.ImagePath = null;
            UserSession.Email = null;
            UserSession.Password = null;
            UserSession.Theme = "Light";

            LoginForm? login = Application.OpenForms.OfType<LoginForm>().FirstOrDefault();

            if (login == null || login.IsDisposed)
            {
                login = new LoginForm();
            }

            login.Show();
            login.BringToFront();
            login.WindowState = FormWindowState.Normal;

            Close();
        }

        public void ApplyTheme(string theme)
        {
            bool isDark = theme == "Dark";

            Color formBack = isDark ? Color.FromArgb(28, 28, 28) : Color.Snow;
            Color sidebarBack = isDark ? Color.FromArgb(35, 35, 35) : Color.WhiteSmoke;
            Color topBack = isDark ? Color.FromArgb(35, 35, 35) : Color.WhiteSmoke;
            Color footerBack = isDark ? Color.FromArgb(35, 35, 35) : Color.WhiteSmoke;
            Color textColor = isDark ? Color.WhiteSmoke : Color.Maroon;
            Color contentBack = Color.Snow;
            Color menuBack = isDark ? Color.FromArgb(45, 45, 45) : Color.White;
            Color menuText = isDark ? Color.WhiteSmoke : Color.Black;

            BackColor = formBack;
            Sidebar.BackColor = sidebarBack;
            topbar.BackColor = topBack;
            panel1.BackColor = footerBack;
            panelContent.BackColor = contentBack;

            label1.ForeColor = textColor;
            username.ForeColor = textColor;
            time.ForeColor = textColor;
            label4.ForeColor = textColor;
            dropdownarrow.ForeColor = textColor;

            foreach (Control ctrl in Sidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.ForeColor = textColor;
                    btn.FlatAppearance.MouseOverBackColor = isDark
                        ? Color.FromArgb(60, 60, 60)
                        : Color.LightGray;
                    btn.FlatAppearance.MouseDownBackColor = isDark
                        ? Color.FromArgb(75, 75, 75)
                        : Color.Gray;
                }
            }

            contextMenuStrip1.BackColor = menuBack;
            contextMenuStrip1.ForeColor = menuText;

            foreach (ToolStripItem item in contextMenuStrip1.Items)
            {
                item.BackColor = menuBack;
                item.ForeColor = menuText;
            }

            chart1.BackColor = isDark ? Color.FromArgb(45, 45, 45) : Color.WhiteSmoke;
            chart2.BackColor = isDark ? Color.FromArgb(45, 45, 45) : Color.White;

            TotalClients.BackColor = Color.Maroon;
            TotalMembers.BackColor = Color.Maroon;
            TotalBooks.BackColor = Color.Maroon;
            ActiveBorrowings.BackColor = Color.Maroon;

            ApplyBranding(isDark);
        }

        private void button1_Click_1(object? sender, EventArgs e)
        {
            ShowDashboardHome();
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new ClientsForm());
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new MonitoringForm());
        }

        private void button4_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new ModulesForm());
        }

        private void button5_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new ReportsForm());
        }

        private void button6_Click(object? sender, EventArgs e)
        {
            DoLogout();
        }

        private void ArchiveButton_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new ArchiveForm());
        }

        private void dropdownarrow_Click(object? sender, EventArgs e)
        {
            contextMenuStrip1.Show(dropdownarrow, 0, dropdownarrow.Height);
        }

        private void profileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new ProfileForm());
        }

        private void settingsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            LoadContentForm(new SettingsForm());
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
    }
}