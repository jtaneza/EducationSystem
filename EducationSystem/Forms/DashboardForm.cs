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

        private FlowLayoutPanel userHeaderPanel = new FlowLayoutPanel();
        private Panel userHeaderHost = new Panel();
        private Panel sidebarBrandPanel = new Panel();

        private Label welcomeLabel = new Label();

        public DashboardForm()
        {
            InitializeComponent();

            SetupResponsiveShell();
            SetupSidebarBranding();
            SetupResponsiveHeader();
            SetupExtraLabels();

            profileToolStripMenuItem.Click += profileToolStripMenuItem_Click;
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;

            button1.Click += button1_Click_1;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
            button5.Click += button5_Click;
            button6.Click += button6_Click;
            button7.Click += ArchiveButton_Click;
            button8.Click += button8_Click;

            StyleSidebarButton(button1);
            StyleSidebarButton(button2);
            StyleSidebarButton(button3);
            StyleSidebarButton(button4);
            StyleSidebarButton(button5);
            StyleSidebarButton(button6);
            StyleSidebarButton(button7);
            StyleSidebarButton(button8);

            StyleDashboardCard(TotalClients);
            StyleDashboardCard(TotalMembers);
            StyleDashboardCard(TotalBooks);
            StyleDashboardCard(ActiveBorrowings);

            TotalClients.Click += (s, e) => LoadContentForm(new ClientsForm());
            TotalMembers.Click += (s, e) => LoadContentForm(new ClientsForm());
            TotalBooks.Click += (s, e) => LoadContentForm(new MonitoringForm());
            ActiveBorrowings.Click += (s, e) => LoadContentForm(new ArchiveForm());

            foreach (Control c in TotalClients.Controls) c.Click += (s, e) => LoadContentForm(new ClientsForm());
            foreach (Control c in TotalMembers.Controls) c.Click += (s, e) => LoadContentForm(new ClientsForm());
            foreach (Control c in TotalBooks.Controls) c.Click += (s, e) => LoadContentForm(new MonitoringForm());
            foreach (Control c in ActiveBorrowings.Controls) c.Click += (s, e) => LoadContentForm(new ArchiveForm());

            this.Load += DashboardForm_Load;
            this.Resize += DashboardForm_Resize;
            topbar.Resize += (s, e) => PositionResponsiveHeader();
            Sidebar.Resize += (s, e) => LayoutSidebar();
        }

        private void DashboardForm_Load(object? sender, EventArgs e)
        {
            LoadUserInfo();
            ApplyTheme(UserSession.Theme);
            StartClock();
            RefreshDashboardCounts();
            LoadLineChart();
            LoadPieChart();
            LoadSuperAdminCardIcons();

            label4.Text = "Super Admin Dashboard";
            panelContent.Visible = false;

            ApplyResponsiveLayout();
        }

        private void DashboardForm_Resize(object? sender, EventArgs e)
        {
            ApplyResponsiveLayout();
            PositionResponsiveHeader();
        }

        private void SetupResponsiveShell()
        {
            Sidebar.Dock = DockStyle.Left;
            topbar.Dock = DockStyle.Top;
            panel1.Dock = DockStyle.Bottom;
            panelContent.Dock = DockStyle.Fill;

            topbar.Height = 60;
            panel1.Height = 45;

            this.MinimumSize = new Size(1000, 650);
        }

        private void SetupSidebarBranding()
        {
            if (!Sidebar.Controls.Contains(sidebarBrandPanel))
            {
                sidebarBrandPanel.Name = "sidebarBrandPanel";
                sidebarBrandPanel.Height = 78;
                sidebarBrandPanel.Dock = DockStyle.Top;
                sidebarBrandPanel.BackColor = Color.SeaShell;

                if (pictureBox1.Parent != null) pictureBox1.Parent.Controls.Remove(pictureBox1);
                if (label1.Parent != null) label1.Parent.Controls.Remove(label1);

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.BackColor = Color.SeaShell;
                label1.BackColor = Color.SeaShell;

                sidebarBrandPanel.Controls.Add(pictureBox1);
                sidebarBrandPanel.Controls.Add(label1);
                Sidebar.Controls.Add(sidebarBrandPanel);

                sidebarBrandPanel.BringToFront();
            }

            LayoutSidebarBranding(true);
        }

        private void LayoutSidebarBranding(bool showText)
        {
            sidebarBrandPanel.Width = Sidebar.Width;
            sidebarBrandPanel.Height = showText ? 74 : 60;

            if (showText)
            {
                pictureBox1.Visible = true;
                label1.Visible = true;

                pictureBox1.Size = new Size(36, 36);
                pictureBox1.Location = new Point(18, 18);

                label1.AutoSize = true;
                label1.TextAlign = ContentAlignment.MiddleLeft;
                label1.Location = new Point(58, 24);
            }
            else
            {
                pictureBox1.Visible = true;
                label1.Visible = false;

                pictureBox1.Size = new Size(34, 34);
                pictureBox1.Location = new Point((Sidebar.Width - pictureBox1.Width) / 2, 13);
            }
        }

        private void SetupExtraLabels()
        {
            if (!Controls.Contains(welcomeLabel))
            {
                welcomeLabel.Name = "welcomeLabel";
                welcomeLabel.AutoSize = true;
                welcomeLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                welcomeLabel.ForeColor = Color.Maroon;
                welcomeLabel.BackColor = Color.SeaShell;
                Controls.Add(welcomeLabel);
                welcomeLabel.BringToFront();
            }

            time.AutoSize = true;
            time.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void SetupResponsiveHeader()
        {
            if (topbar == null) return;
            if (topbar.Controls.Contains(userHeaderHost)) return;

            userHeaderHost.SuspendLayout();
            userHeaderPanel.SuspendLayout();

            userHeaderHost.Name = "userHeaderHost";
            userHeaderHost.BackColor = Color.SeaShell;
            userHeaderHost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            userHeaderHost.Height = 50;

            userHeaderPanel.Name = "userHeaderPanel";
            userHeaderPanel.AutoSize = true;
            userHeaderPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            userHeaderPanel.WrapContents = false;
            userHeaderPanel.FlowDirection = FlowDirection.LeftToRight;
            userHeaderPanel.Margin = new Padding(0);
            userHeaderPanel.Padding = new Padding(0);
            userHeaderPanel.BackColor = Color.SeaShell;

            if (ProfileImage.Parent != null) ProfileImage.Parent.Controls.Remove(ProfileImage);
            if (username.Parent != null) username.Parent.Controls.Remove(username);
            if (dropdownarrow.Parent != null) dropdownarrow.Parent.Controls.Remove(dropdownarrow);

            ProfileImage.Size = new Size(32, 32);
            ProfileImage.SizeMode = PictureBoxSizeMode.Zoom;
            ProfileImage.Margin = new Padding(0, 9, 8, 0);
            ProfileImage.Cursor = Cursors.Hand;
            ProfileImage.BackColor = Color.SeaShell;

            username.AutoSize = true;
            username.MaximumSize = new Size(220, 0);
            username.AutoEllipsis = true;
            username.TextAlign = ContentAlignment.MiddleLeft;
            username.Margin = new Padding(0, 13, 8, 0);
            username.BackColor = Color.SeaShell;

            dropdownarrow.AutoSize = true;
            dropdownarrow.TextAlign = ContentAlignment.MiddleLeft;
            dropdownarrow.Margin = new Padding(0, 13, 0, 0);
            dropdownarrow.Cursor = Cursors.Hand;
            dropdownarrow.BackColor = Color.SeaShell;

            userHeaderPanel.Controls.Add(ProfileImage);
            userHeaderPanel.Controls.Add(username);
            userHeaderPanel.Controls.Add(dropdownarrow);

            userHeaderHost.Controls.Add(userHeaderPanel);
            topbar.Controls.Add(userHeaderHost);

            userHeaderPanel.ResumeLayout();
            userHeaderHost.ResumeLayout();

            PositionResponsiveHeader();
        }

        private void PositionResponsiveHeader()
        {
            if (!topbar.Controls.Contains(userHeaderHost)) return;

            userHeaderPanel.PerformLayout();

            userHeaderHost.Width = userHeaderPanel.PreferredSize.Width;
            userHeaderHost.Height = Math.Max(50, userHeaderPanel.PreferredSize.Height);

            int rightPadding = 20;
            int topPadding = Math.Max(0, (topbar.ClientSize.Height - userHeaderHost.Height) / 2);

            userHeaderHost.Location = new Point(
                topbar.ClientSize.Width - userHeaderHost.Width - rightPadding,
                topPadding
            );

            userHeaderPanel.Location = new Point(0, 0);
            userHeaderHost.BringToFront();
        }

        private void ApplyResponsiveLayout()
        {
            int w = this.ClientSize.Width;

            if (w >= 1280)
            {
                ApplyDesktopLayout();
            }
            else if (w >= 980)
            {
                ApplyTabletLayout();
            }
            else
            {
                ApplySmallLayout();
            }

            PositionResponsiveHeader();
            LayoutSidebar();
            LayoutDashboardHome();
        }

        private void ApplyDesktopLayout()
        {
            Sidebar.Width = 225;
            topbar.Height = 60;
            panel1.Height = 45;

            username.MaximumSize = new Size(220, 0);

            LayoutSidebarBranding(true);
            SetSidebarButtonStyle(true);
        }

        private void ApplyTabletLayout()
        {
            Sidebar.Width = 170;
            topbar.Height = 56;
            panel1.Height = 42;

            username.MaximumSize = new Size(150, 0);

            LayoutSidebarBranding(true);
            SetSidebarButtonStyle(true);
        }

        private void ApplySmallLayout()
        {
            Sidebar.Width = 78;
            topbar.Height = 52;
            panel1.Height = 40;

            username.MaximumSize = new Size(95, 0);

            LayoutSidebarBranding(false);
            SetSidebarButtonStyle(false);
        }

        private void LayoutSidebar()
        {
            LayoutSidebarButtons();
            LayoutSidebarBranding(Sidebar.Width > 120);
        }

        private void SetSidebarButtonStyle(bool showText)
        {
            ConfigureSidebarButton(button1, "📊  Dashboard", showText);
            ConfigureSidebarButton(button2, "🏢  Client Management", showText);
            ConfigureSidebarButton(button3, "🔎  Monitoring", showText);
            ConfigureSidebarButton(button8, "⚙  System Configuration", showText);
            ConfigureSidebarButton(button7, "🗂  Archive Management", showText);
            ConfigureSidebarButton(button6, "⏻  Logout", showText);

            button4.Visible = false; // remove Library Monitoring from sidebar
            button5.Visible = false; // still unused
        }

        private void ConfigureSidebarButton(Button btn, string fullText, bool showText)
        {
            btn.Width = Sidebar.Width - 12;
            btn.Left = 6;
            btn.Height = 44;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.UseVisualStyleBackColor = false;

            if (showText)
            {
                btn.Text = fullText;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(18, 0, 0, 0);
            }
            else
            {
                string iconOnly = fullText.Split(new[] { "  " }, StringSplitOptions.None)[0];
                btn.Text = iconOnly;
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.Padding = new Padding(0);
            }
        }

        private void LayoutSidebarButtons()
        {
            int top = sidebarBrandPanel.Bottom + 22;
            int gap = 10;

            Button[] mainButtons = { button1, button2, button3, button8, button7 };

            foreach (Button btn in mainButtons)
            {
                btn.Top = top;
                btn.Left = 6;
                btn.Width = Sidebar.Width - 12;
                top += btn.Height + gap;
            }

            button6.Left = 6;
            button6.Width = Sidebar.Width - 12;
            button6.Top = Sidebar.Height - button6.Height - 18;
        }

        private void LayoutDashboardHome()
        {
            if (!label4.Visible) return;

            int sidebarW = Sidebar.Width;
            int topH = topbar.Height;
            int footerH = panel1.Height;

            int contentLeft = sidebarW + 28;
            int contentTop = topH + 24;
            int contentWidth = this.ClientSize.Width - sidebarW - 56;
            int contentHeight = this.ClientSize.Height - topH - footerH - 36;

            if (contentWidth < 300 || contentHeight < 200) return;

            welcomeLabel.Location = new Point(contentLeft, contentTop);
            label4.Location = new Point(contentLeft, contentTop + 48);

            int cardsTop = contentTop + 96;
            int gap = 20;
            int cardHeight = 96;

            if (contentWidth >= 1200)
            {
                int cardWidth = (contentWidth - (gap * 3)) / 4;

                TotalClients.Bounds = new Rectangle(contentLeft, cardsTop, cardWidth, cardHeight);
                TotalMembers.Bounds = new Rectangle(contentLeft + cardWidth + gap, cardsTop, cardWidth, cardHeight);
                TotalBooks.Bounds = new Rectangle(contentLeft + (cardWidth + gap) * 2, cardsTop, cardWidth, cardHeight);
                ActiveBorrowings.Bounds = new Rectangle(contentLeft + (cardWidth + gap) * 3, cardsTop, cardWidth, cardHeight);

                int chartsTop = cardsTop + cardHeight + 30;
                int chartWidth = (contentWidth - gap) / 2;
                int chartHeight = 330;

                chart1.Bounds = new Rectangle(contentLeft, chartsTop, chartWidth, chartHeight);
                chart2.Bounds = new Rectangle(contentLeft + chartWidth + gap, chartsTop, chartWidth, chartHeight);
            }
            else if (contentWidth >= 820)
            {
                int cardWidth = (contentWidth - gap) / 2;

                TotalClients.Bounds = new Rectangle(contentLeft, cardsTop, cardWidth, cardHeight);
                TotalMembers.Bounds = new Rectangle(contentLeft + cardWidth + gap, cardsTop, cardWidth, cardHeight);
                TotalBooks.Bounds = new Rectangle(contentLeft, cardsTop + cardHeight + gap, cardWidth, cardHeight);
                ActiveBorrowings.Bounds = new Rectangle(contentLeft + cardWidth + gap, cardsTop + cardHeight + gap, cardWidth, cardHeight);

                int chartsTop = cardsTop + (cardHeight * 2) + (gap * 2) + 24;
                int chartHeight = 290;

                chart1.Bounds = new Rectangle(contentLeft, chartsTop, contentWidth, chartHeight);
                chart2.Bounds = new Rectangle(contentLeft, chartsTop + chartHeight + gap, contentWidth, chartHeight);
            }
            else
            {
                int cardWidth = contentWidth;

                TotalClients.Bounds = new Rectangle(contentLeft, cardsTop, cardWidth, cardHeight);
                TotalMembers.Bounds = new Rectangle(contentLeft, cardsTop + cardHeight + gap, cardWidth, cardHeight);
                TotalBooks.Bounds = new Rectangle(contentLeft, cardsTop + (cardHeight + gap) * 2, cardWidth, cardHeight);
                ActiveBorrowings.Bounds = new Rectangle(contentLeft, cardsTop + (cardHeight + gap) * 3, cardWidth, cardHeight);

                int chartsTop = cardsTop + (cardHeight + gap) * 4 + 24;
                int chartHeight = 240;

                chart1.Bounds = new Rectangle(contentLeft, chartsTop, contentWidth, chartHeight);
                chart2.Bounds = new Rectangle(contentLeft, chartsTop + chartHeight + gap, contentWidth, chartHeight);
            }

            LayoutDashboardCardContents();
            AlignFooterItems();
        }

        private void LayoutDashboardCardContents()
        {
            LayoutSingleDashboardCard(TotalClients, users, label3, label5);
            LayoutSingleDashboardCard(TotalMembers, books, label6, label7);
            LayoutSingleDashboardCard(TotalBooks, activity, label8, label9);
            LayoutSingleDashboardCard(ActiveBorrowings, pictureBox5, label10, label11);
        }

        private void LayoutSingleDashboardCard(Panel card, PictureBox icon, Label titleLabel, Label valueLabel)
        {
            int iconSize = 56;
            int groupWidth = 190;
            int gap = 16;

            int startX = Math.Max(12, (card.Width - groupWidth) / 2);

            icon.Size = new Size(iconSize, iconSize);
            icon.SizeMode = PictureBoxSizeMode.Zoom;
            icon.Location = new Point(startX, (card.Height - iconSize) / 2);

            int textX = icon.Right + gap;
            int textWidth = groupWidth - iconSize - gap;

            titleLabel.AutoSize = false;
            titleLabel.Width = textWidth;
            titleLabel.Height = 24;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(textX, card.Height / 2 - 20);

            valueLabel.AutoSize = false;
            valueLabel.Width = textWidth;
            valueLabel.Height = 24;
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;
            valueLabel.Location = new Point(textX, card.Height / 2 + 6);
        }

        private void AlignFooterItems()
        {
            if (panel1 == null) return;

            panel1.Height = 56;

            time.AutoSize = true;
            int footerCenterY = Math.Max(0, (panel1.Height - time.Height) / 2);
            time.Location = new Point(22, footerCenterY);

            button6.Left = 6;
            button6.Width = Sidebar.Width - 12;
            button6.Top = Sidebar.Height - button6.Height - 22;
        }

        private void StyleSidebarButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(210, 210, 210);
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void StyleDashboardCard(Panel card)
        {
            card.BackColor = Color.Maroon;
            card.Cursor = Cursors.Hand;

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

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(120, 0, 0);
            card.MouseLeave += (s, e) => card.BackColor = Color.Maroon;
        }

        private void RefreshDashboardCounts()
        {
            int totalClientsCount = ClientArchiveStore.ActiveClients.Count;
            int activeClientsCount = ClientArchiveStore.ActiveClients.Count(c => c.Status == "Active");
            int inactiveClientsCount = ClientArchiveStore.ActiveClients.Count(c => c.Status == "Inactive");
            int archivedClientsCount = ClientArchiveStore.ArchivedClients.Count;

            label3.Text = "Total Clients";
            label6.Text = "Active Clients";
            label8.Text = "Inactive Clients";
            label10.Text = "Archived Clients";

            label5.Text = totalClientsCount.ToString();
            label7.Text = activeClientsCount.ToString();
            label9.Text = inactiveClientsCount.ToString();
            label11.Text = archivedClientsCount.ToString();

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
                    using (var fs = new FileStream(UserSession.ImagePath, FileMode.Open, FileAccess.Read))
                    {
                        ProfileImage.Image = Image.FromStream(fs);
                    }
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

            welcomeLabel.Text = $"Welcome, {username.Text}!";

            PositionResponsiveHeader();
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
            }

            label1.ForeColor = isDark ? Color.WhiteSmoke : Color.Maroon;
            label1.BackColor = Color.Transparent;
            pictureBox1.BackColor = Color.Transparent;
            sidebarBrandPanel.BackColor = Color.Transparent;
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

        private void LoadSuperAdminCardIcons()
        {
            try
            {
                string clientsPath = FindAssetPath("clients.png");
                string activePath = FindAssetPath("active.png");
                string inactivePath = FindAssetPath("inactive.png");
                string archivePath = FindAssetPath("archive.png");

                if (!string.IsNullOrWhiteSpace(clientsPath))
                {
                    if (users.Image != null)
                    {
                        var old = users.Image;
                        users.Image = null;
                        old.Dispose();
                    }
                    users.Image = Image.FromFile(clientsPath);
                }

                if (!string.IsNullOrWhiteSpace(activePath))
                {
                    if (books.Image != null)
                    {
                        var old = books.Image;
                        books.Image = null;
                        old.Dispose();
                    }
                    books.Image = Image.FromFile(activePath);
                }

                if (!string.IsNullOrWhiteSpace(inactivePath))
                {
                    if (activity.Image != null)
                    {
                        var old = activity.Image;
                        activity.Image = null;
                        old.Dispose();
                    }
                    activity.Image = Image.FromFile(inactivePath);
                }

                if (!string.IsNullOrWhiteSpace(archivePath))
                {
                    if (pictureBox5.Image != null)
                    {
                        var old = pictureBox5.Image;
                        pictureBox5.Image = null;
                        old.Dispose();
                    }
                    pictureBox5.Image = Image.FromFile(archivePath);
                }

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
                MessageBox.Show("Error loading dashboard icons: " + ex.Message);
            }
        }
        private void ShowDashboardHome()
        {
            RefreshDashboardCounts();
            LoadLineChart();
            LoadPieChart();
            LoadUserInfo();
            ApplyTheme(UserSession.Theme);
            LoadSuperAdminCardIcons();

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

            ApplyResponsiveLayout();
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

            Color formBack = isDark ? Color.FromArgb(28, 28, 28) : Color.FromArgb(252, 248, 244);
            Color sidebarBack = isDark ? Color.FromArgb(35, 35, 35) : Color.FromArgb(245, 238, 230);
            Color topBack = isDark ? Color.FromArgb(35, 35, 35) : Color.FromArgb(250, 245, 240);
            Color footerBack = isDark ? Color.FromArgb(35, 35, 35) : Color.FromArgb(250, 245, 240);
            Color textColor = isDark ? Color.WhiteSmoke : Color.Maroon;
            Color contentBack = isDark ? Color.FromArgb(28, 28, 28) : Color.FromArgb(252, 248, 244);
            Color menuBack = isDark ? Color.FromArgb(45, 45, 45) : Color.FromArgb(255, 252, 248);
            Color menuText = isDark ? Color.WhiteSmoke : Color.Black;

            Color cardBack = Color.Maroon;
            Color cardAccent = Color.FromArgb(184, 134, 11);

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
            welcomeLabel.ForeColor = textColor;
            welcomeLabel.BackColor = Color.Transparent;

            userHeaderPanel.BackColor = topBack;
            userHeaderHost.BackColor = topBack;
            ProfileImage.BackColor = topBack;
            username.BackColor = topBack;
            dropdownarrow.BackColor = topBack;

            foreach (Control ctrl in Sidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.ForeColor = textColor;
                    btn.BackColor = sidebarBack;
                    btn.FlatAppearance.MouseOverBackColor = isDark
                        ? Color.FromArgb(60, 60, 60)
                        : Color.FromArgb(232, 220, 210);
                    btn.FlatAppearance.MouseDownBackColor = isDark
                        ? Color.FromArgb(75, 75, 75)
                        : Color.FromArgb(220, 205, 195);
                }
            }

            contextMenuStrip1.BackColor = menuBack;
            contextMenuStrip1.ForeColor = menuText;

            foreach (ToolStripItem item in contextMenuStrip1.Items)
            {
                item.BackColor = menuBack;
                item.ForeColor = menuText;
            }

            chart1.BackColor = isDark ? Color.FromArgb(45, 45, 45) : Color.FromArgb(255, 252, 248);
            chart2.BackColor = isDark ? Color.FromArgb(45, 45, 45) : Color.FromArgb(255, 252, 248);

            TotalClients.BackColor = cardBack;
            TotalMembers.BackColor = cardBack;
            TotalBooks.BackColor = cardBack;
            ActiveBorrowings.BackColor = cardBack;

            SetCardAccentColor(TotalClients, cardAccent);
            SetCardAccentColor(TotalMembers, cardAccent);
            SetCardAccentColor(TotalBooks, cardAccent);
            SetCardAccentColor(ActiveBorrowings, cardAccent);

            ApplyBranding(isDark);
            LayoutSidebar();
            PositionResponsiveHeader();
        }
        private void SetCardAccentColor(Panel card, Color accentColor)
        {
            foreach (Control ctrl in card.Controls)
            {
                if (ctrl is Panel panel && panel.Name == "accent")
                {
                    panel.BackColor = accentColor;
                }
            }
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

        private void button8_Click(object sender, EventArgs e)
        {
            LoadContentForm(new OversightForm());
        }

        private void panelContent_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}