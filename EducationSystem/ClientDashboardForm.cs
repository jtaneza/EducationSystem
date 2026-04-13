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

        private FlowLayoutPanel userHeaderPanel = new FlowLayoutPanel();
        private Panel userHeaderHost = new Panel();
        private Panel sidebarBrandPanel = new Panel();
        private Label welcomeLabel = new Label();

        public ClientDashboardForm()
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

            BuildCardContent();
            BuildChartPanel();
            BuildDashboardDetails();

            this.Load += ClientDashboardForm_Load;
            this.Resize += ClientDashboardForm_Resize;
            topbar.Resize += (s, e) => PositionResponsiveHeader();
            Sidebar.Resize += (s, e) => LayoutSidebar();
        }

        private void ClientDashboardForm_Load(object? sender, EventArgs e)
        {
            LoadUserInfo();
            ApplyTheme("Light");
            StartClock();
            RefreshDashboardCounts();
            LoadLineChart();
            LoadDummyTables();
            LoadCardIcons();

            label4.Text = "Librarian Dashboard";
            panelContent.Visible = false;

            ApplyResponsiveLayout();
        }

        private void ClientDashboardForm_Resize(object? sender, EventArgs e)
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
            panel1.Height = 52;

            this.MinimumSize = new Size(1000, 650);
        }

        private void SetupExtraLabels()
        {
            if (!Controls.Contains(welcomeLabel))
            {
                welcomeLabel.Name = "welcomeLabel";
                welcomeLabel.AutoSize = true;
                welcomeLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                welcomeLabel.ForeColor = Color.Maroon;
                welcomeLabel.BackColor = Color.Transparent;
                Controls.Add(welcomeLabel);
                welcomeLabel.BringToFront();
            }

            time.AutoSize = true;
            time.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void SetupSidebarBranding()
        {
            if (!Sidebar.Controls.Contains(sidebarBrandPanel))
            {
                sidebarBrandPanel.Name = "sidebarBrandPanel";
                sidebarBrandPanel.Height = 78;
                sidebarBrandPanel.Dock = DockStyle.Top;
                sidebarBrandPanel.BackColor = Color.FromArgb(245, 238, 230);

                if (pictureBox1.Parent != null) pictureBox1.Parent.Controls.Remove(pictureBox1);
                if (label1.Parent != null) label1.Parent.Controls.Remove(label1);

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.BackColor = Color.FromArgb(245, 238, 230);
                label1.BackColor = Color.FromArgb(245, 238, 230);

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

        private void SetupResponsiveHeader()
        {
            if (topbar == null) return;
            if (topbar.Controls.Contains(userHeaderHost)) return;

            userHeaderHost.SuspendLayout();
            userHeaderPanel.SuspendLayout();

            userHeaderHost.Name = "userHeaderHost";
            userHeaderHost.BackColor = Color.FromArgb(250, 245, 240);
            userHeaderHost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            userHeaderHost.Height = 50;

            userHeaderPanel.Name = "userHeaderPanel";
            userHeaderPanel.AutoSize = true;
            userHeaderPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            userHeaderPanel.WrapContents = false;
            userHeaderPanel.FlowDirection = FlowDirection.LeftToRight;
            userHeaderPanel.Margin = new Padding(0);
            userHeaderPanel.Padding = new Padding(0);
            userHeaderPanel.BackColor = Color.FromArgb(250, 245, 240);

            if (ProfileImage.Parent != null) ProfileImage.Parent.Controls.Remove(ProfileImage);
            if (username.Parent != null) username.Parent.Controls.Remove(username);
            if (dropdownarrow.Parent != null) dropdownarrow.Parent.Controls.Remove(dropdownarrow);

            ProfileImage.Size = new Size(32, 32);
            ProfileImage.SizeMode = PictureBoxSizeMode.Zoom;
            ProfileImage.Margin = new Padding(0, 9, 8, 0);
            ProfileImage.Cursor = Cursors.Hand;
            ProfileImage.BackColor = Color.FromArgb(250, 245, 240);

            username.AutoSize = true;
            username.MaximumSize = new Size(220, 0);
            username.AutoEllipsis = true;
            username.TextAlign = ContentAlignment.MiddleLeft;
            username.Margin = new Padding(0, 13, 8, 0);
            username.BackColor = Color.FromArgb(250, 245, 240);

            dropdownarrow.AutoSize = true;
            dropdownarrow.TextAlign = ContentAlignment.MiddleLeft;
            dropdownarrow.Margin = new Padding(0, 13, 0, 0);
            dropdownarrow.Cursor = Cursors.Hand;
            dropdownarrow.BackColor = Color.FromArgb(250, 245, 240);

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
            panel1.Height = 54;

            username.MaximumSize = new Size(220, 0);

            LayoutSidebarBranding(true);
            SetSidebarButtonStyle(true);
        }

        private void ApplyTabletLayout()
        {
            Sidebar.Width = 170;
            topbar.Height = 56;
            panel1.Height = 50;

            username.MaximumSize = new Size(150, 0);

            LayoutSidebarBranding(true);
            SetSidebarButtonStyle(true);
        }

        private void ApplySmallLayout()
        {
            Sidebar.Width = 78;
            topbar.Height = 52;
            panel1.Height = 46;

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
            ConfigureSidebarButton(button2, "👥  Member Management", showText);
            ConfigureSidebarButton(button3, "🔄  Transactions", showText);
            ConfigureSidebarButton(button7, "🗂  Archive", showText);
            ConfigureSidebarButton(button6, "⏻  Logout", showText);

            button4.Visible = false;
            button5.Visible = false;
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

            Button[] mainButtons = { button1, button2, button3, button7 };

            foreach (Button btn in mainButtons)
            {
                btn.Top = top;
                btn.Left = 6;
                btn.Width = Sidebar.Width - 12;
                top += btn.Height + gap;
            }

            button6.Left = 6;
            button6.Width = Sidebar.Width - 12;
            button6.Top = Sidebar.Height - button6.Height - 20;
        }

        private void BuildCardContent()
        {
            BuildSingleCard(TotalMembers, users, label3, label5, "Daily Transactions", "0");
            BuildSingleCard(TotalBooks, books, label6, label7, "Borrowed Today", "0");
            BuildSingleCard(BorrowedBooks, activity, label8, label9, "Returned Today", "0");
            BuildSingleCard(OverdueBooks, pictureBox5, label10, label11, "Fine Alerts", "0");
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
                accent.BackColor = Color.FromArgb(184, 134, 11);
                accent.Width = 6;
                accent.Dock = DockStyle.Left;

                card.Controls.Add(accent);
                accent.BringToFront();
            }
        }

        private void LayoutClientCardContents()
        {
            LayoutSingleClientCard(TotalMembers, users, label3, label5);
            LayoutSingleClientCard(TotalBooks, books, label6, label7);
            LayoutSingleClientCard(BorrowedBooks, activity, label8, label9);
            LayoutSingleClientCard(OverdueBooks, pictureBox5, label10, label11);
        }

        private void LayoutSingleClientCard(Panel card, PictureBox icon, Label titleLabel, Label valueLabel)
        {
            int iconSize = 56;
            int groupWidth = 200;
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

        private void BuildChartPanel()
        {
            chart1 = new Chart();
            chart1.BackColor = Color.FromArgb(255, 252, 248);
            chart1.BorderlineColor = Color.FromArgb(220, 210, 200);
            chart1.BorderlineDashStyle = ChartDashStyle.Solid;
            chart1.BorderlineWidth = 1;

            ChartArea area = new ChartArea("MainArea");
            area.BackColor = Color.FromArgb(255, 252, 248);
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

            Title title = new Title("Daily Transaction Trend");
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
            recentMembersPanel.BackColor = Color.FromArgb(255, 252, 248);
            recentMembersPanel.BorderStyle = BorderStyle.None;

            lblRecentMembersTitle = new Label();
            lblRecentMembersTitle.Text = "Recent Member Activity";
            lblRecentMembersTitle.ForeColor = Color.Maroon;
            lblRecentMembersTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblRecentMembersTitle.AutoSize = true;

            lvRecentMembers = new ListView();
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
            recentBooksPanel.BackColor = Color.FromArgb(255, 252, 248);
            recentBooksPanel.BorderStyle = BorderStyle.None;

            lblRecentBooksTitle = new Label();
            lblRecentBooksTitle.Text = "Recent Transactions";
            lblRecentBooksTitle.ForeColor = Color.Maroon;
            lblRecentBooksTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblRecentBooksTitle.AutoSize = true;

            lvRecentBooks = new ListView();
            lvRecentBooks.View = View.Details;
            lvRecentBooks.FullRowSelect = true;
            lvRecentBooks.GridLines = false;
            lvRecentBooks.BorderStyle = BorderStyle.FixedSingle;
            lvRecentBooks.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lvRecentBooks.Columns.Add("Transaction", 220);
            lvRecentBooks.Columns.Add("Code", 90);
            lvRecentBooks.Columns.Add("Status", 150);
            lvRecentBooks.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            recentBooksPanel.Controls.Add(lblRecentBooksTitle);
            recentBooksPanel.Controls.Add(lvRecentBooks);

            Controls.Add(recentMembersPanel);
            Controls.Add(recentBooksPanel);
        }

        private void LayoutDashboardHome()
        {
            if (!label4.Visible) return;

            int sidebarW = Sidebar.Width;
            int topH = topbar.Height;
            int footerH = panel1.Height;

            int contentLeft = sidebarW + 26;
            int contentTop = topH + 22;
            int contentWidth = this.ClientSize.Width - sidebarW - 52;
            int contentHeight = this.ClientSize.Height - topH - footerH - 34;

            if (contentWidth < 300 || contentHeight < 200) return;

            welcomeLabel.Location = new Point(contentLeft, contentTop + 2);
            label4.Location = new Point(contentLeft, contentTop + 30);

            int cardsTop = contentTop + 70;
            int gap = 18;
            int cardHeight = 96;

            if (contentWidth >= 1200)
            {
                int cardWidth = (contentWidth - (gap * 3)) / 4;

                TotalMembers.Bounds = new Rectangle(contentLeft, cardsTop, cardWidth, cardHeight);
                TotalBooks.Bounds = new Rectangle(contentLeft + cardWidth + gap, cardsTop, cardWidth, cardHeight);
                BorrowedBooks.Bounds = new Rectangle(contentLeft + (cardWidth + gap) * 2, cardsTop, cardWidth, cardHeight);
                OverdueBooks.Bounds = new Rectangle(contentLeft + (cardWidth + gap) * 3, cardsTop, cardWidth, cardHeight);

                int chartTop = cardsTop + cardHeight + 26;
                chart1.Bounds = new Rectangle(contentLeft, chartTop, contentWidth, 210);

                int tablesTop = chartTop + chart1.Height + 16;
                int panelGap = 20;
                int panelWidth = (contentWidth - panelGap) / 2;
                int panelHeight = 170;

                recentMembersPanel.Bounds = new Rectangle(contentLeft, tablesTop, panelWidth, panelHeight);
                recentBooksPanel.Bounds = new Rectangle(contentLeft + panelWidth + panelGap, tablesTop, panelWidth, panelHeight);
            }
            else if (contentWidth >= 820)
            {
                int cardWidth = (contentWidth - gap) / 2;

                TotalMembers.Bounds = new Rectangle(contentLeft, cardsTop, cardWidth, cardHeight);
                TotalBooks.Bounds = new Rectangle(contentLeft + cardWidth + gap, cardsTop, cardWidth, cardHeight);
                BorrowedBooks.Bounds = new Rectangle(contentLeft, cardsTop + cardHeight + gap, cardWidth, cardHeight);
                OverdueBooks.Bounds = new Rectangle(contentLeft + cardWidth + gap, cardsTop + cardHeight + gap, cardWidth, cardHeight);

                int chartTop = cardsTop + (cardHeight * 2) + (gap * 2) + 20;
                chart1.Bounds = new Rectangle(contentLeft, chartTop, contentWidth, 220);

                int tablesTop = chartTop + chart1.Height + 18;
                int panelHeight = 160;

                recentMembersPanel.Bounds = new Rectangle(contentLeft, tablesTop, contentWidth, panelHeight);
                recentBooksPanel.Bounds = new Rectangle(contentLeft, tablesTop + panelHeight + gap, contentWidth, panelHeight);
            }
            else
            {
                int cardWidth = contentWidth;

                TotalMembers.Bounds = new Rectangle(contentLeft, cardsTop, cardWidth, cardHeight);
                TotalBooks.Bounds = new Rectangle(contentLeft, cardsTop + cardHeight + gap, cardWidth, cardHeight);
                BorrowedBooks.Bounds = new Rectangle(contentLeft, cardsTop + (cardHeight + gap) * 2, cardWidth, cardHeight);
                OverdueBooks.Bounds = new Rectangle(contentLeft, cardsTop + (cardHeight + gap) * 3, cardWidth, cardHeight);

                int chartTop = cardsTop + (cardHeight + gap) * 4 + 20;
                chart1.Bounds = new Rectangle(contentLeft, chartTop, contentWidth, 220);

                int tablesTop = chartTop + chart1.Height + 18;
                int panelHeight = 150;

                recentMembersPanel.Bounds = new Rectangle(contentLeft, tablesTop, contentWidth, panelHeight);
                recentBooksPanel.Bounds = new Rectangle(contentLeft, tablesTop + panelHeight + gap, contentWidth, panelHeight);
            }

            LayoutClientCardContents();
            LayoutDetailPanels();
            AlignFooterItems();
        }

        private void LayoutDetailPanels()
        {
            if (recentMembersPanel == null || recentBooksPanel == null) return;

            LayoutSingleDetailPanel(recentMembersPanel, lblRecentMembersTitle, lvRecentMembers);
            LayoutSingleDetailPanel(recentBooksPanel, lblRecentBooksTitle, lvRecentBooks);
        }

        private void LayoutSingleDetailPanel(Panel host, Label title, ListView list)
        {
            title.Location = new Point(18, 14);
            list.Location = new Point(18, 42);
            list.Size = new Size(host.Width - 36, host.Height - 54);
        }

        private void AlignFooterItems()
        {
            panel1.Height = 54;

            time.AutoSize = true;
            int footerCenterY = Math.Max(0, (panel1.Height - time.Height) / 2);
            time.Location = new Point(22, footerCenterY);

            button6.Left = 6;
            button6.Width = Sidebar.Width - 12;
            button6.Top = Sidebar.Height - button6.Height - 20;
        }

        private void LoadDummyTables()
        {
            lvRecentMembers.Items.Clear();
            lvRecentBooks.Items.Clear();

            lvRecentMembers.Items.Add(new ListViewItem(new[] { "John Cruz", "MB001", "Borrowed Book" }));
            lvRecentMembers.Items.Add(new ListViewItem(new[] { "Maria Santos", "MB002", "Returned Book" }));
            lvRecentMembers.Items.Add(new ListViewItem(new[] { "Paolo Reyes", "MB003", "Fine Applied" }));
            lvRecentMembers.Items.Add(new ListViewItem(new[] { "Anne Flores", "MB004", "Updated" }));

            lvRecentBooks.Items.Add(new ListViewItem(new[] { "Borrow", "TR001", "Completed" }));
            lvRecentBooks.Items.Add(new ListViewItem(new[] { "Return", "TR002", "Completed" }));
            lvRecentBooks.Items.Add(new ListViewItem(new[] { "Fine Applied", "TR003", "Pending" }));
            lvRecentBooks.Items.Add(new ListViewItem(new[] { "Borrow", "TR004", "Completed" }));
        }

        private void StyleSidebarButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 220, 210);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(220, 205, 195);
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        private void StyleDashboardCard(Panel card)
        {
            card.Cursor = Cursors.Hand;

            card.MouseEnter += (s, e) => { card.BackColor = Color.FromArgb(120, 0, 0); };
            card.MouseLeave += (s, e) => { card.BackColor = Color.Maroon; };
        }

        private void RefreshDashboardCounts()
        {
            label5.Text = "24";
            label7.Text = "8";
            label9.Text = "6";
            label11.Text = "2";
            label4.Text = "Librarian Dashboard";
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

        public void LoadUserInfo()
        {
            username.Text = string.IsNullOrWhiteSpace(ClientSession.Username)
                ? "Librarian"
                : ClientSession.Username;

            try
            {
                string profilePath = "";

                if (!string.IsNullOrWhiteSpace(ClientSession.ImagePath) && File.Exists(ClientSession.ImagePath))
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
                ProfileImage.BackColor = Color.Transparent;
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
                pictureBox1.BackColor = Color.Transparent;
            }
            catch
            {
                pictureBox1.Image = null;
            }

            label1.ForeColor = Color.Maroon;
            label1.BackColor = Color.Transparent;

            welcomeLabel.Text = $"Welcome, {username.Text}!";
            PositionResponsiveHeader();
        }

        private void LoadCardIcons()
        {
            try
            {
                string membersPath = FindAssetPath("members.png");
                string booksPath = FindAssetPath("borrow.png");
                string borrowPath = FindAssetPath("return.png");
                string overduePath = FindAssetPath("fine.png");

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
            ApplyTheme("Light");

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

            ApplyResponsiveLayout();
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
            Color formBack = Color.FromArgb(252, 248, 244);
            Color sidebarBack = Color.FromArgb(245, 238, 230);
            Color topBack = Color.FromArgb(250, 245, 240);
            Color footerBack = Color.FromArgb(250, 245, 240);
            Color contentBack = Color.FromArgb(252, 248, 244);
            Color panelBack = Color.FromArgb(255, 252, 248);
            Color textColor = Color.Maroon;
            Color menuBack = Color.FromArgb(255, 252, 248);
            Color menuText = Color.Black;

            BackColor = formBack;
            Sidebar.BackColor = sidebarBack;
            topbar.BackColor = topBack;
            panel1.BackColor = footerBack;
            panelContent.BackColor = contentBack;

            label1.ForeColor = textColor;
            label1.BackColor = Color.Transparent;

            username.ForeColor = textColor;
            username.BackColor = topBack;

            time.ForeColor = textColor;
            time.BackColor = Color.Transparent;

            label4.ForeColor = textColor;
            dropdownarrow.ForeColor = textColor;
            dropdownarrow.BackColor = topBack;

            pictureBox1.BackColor = sidebarBack;
            ProfileImage.BackColor = topBack;

            userHeaderPanel.BackColor = topBack;
            userHeaderHost.BackColor = topBack;
            sidebarBrandPanel.BackColor = sidebarBack;

            welcomeLabel.ForeColor = textColor;
            welcomeLabel.BackColor = Color.Transparent;

            foreach (Control ctrl in Sidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.ForeColor = Color.Maroon;
                    btn.BackColor = sidebarBack;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 220, 210);
                    btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(220, 205, 195);
                }
            }

            contextMenuStrip1.BackColor = menuBack;
            contextMenuStrip1.ForeColor = menuText;

            foreach (ToolStripItem item in contextMenuStrip1.Items)
            {
                item.BackColor = menuBack;
                item.ForeColor = menuText;
            }

            TotalMembers.BackColor = Color.Maroon;
            TotalBooks.BackColor = Color.Maroon;
            BorrowedBooks.BackColor = Color.Maroon;
            OverdueBooks.BackColor = Color.Maroon;

            if (recentMembersPanel != null) recentMembersPanel.BackColor = panelBack;
            if (recentBooksPanel != null) recentBooksPanel.BackColor = panelBack;
            if (chart1 != null) chart1.BackColor = panelBack;
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
            LoadContentForm(new UserManagementForm());
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Transactions module next.");
        }

        private void button4_Click(object? sender, EventArgs e)
        {
            // hidden
        }

        private void button5_Click(object? sender, EventArgs e)
        {
            // hidden
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