using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class DashboardForm : Form
    {
        private readonly System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        private bool isLoggingOut = false;

        private FlowLayoutPanel userHeaderPanel = new FlowLayoutPanel();
        private Panel userHeaderHost = new Panel();
        private Panel sidebarBrandPanel = new Panel();

        private TextBox topSearchBox = new TextBox();
        private Panel topSearchHost = new Panel();
        private Label topSearchIcon = new Label();

        private Label dashboardSubTitle = new Label();
       

        // Dynamic sidebar
        private FlowLayoutPanel superAdminMenuHost = null!;
        private Button navDashboard = null!;
        private Button navManageClients = null!;
        private Button navMonitorSystem = null!;
        private Button navConfigureModules = null!;
        private Button navViewBookCatalog = null!;
        private Button navViewUser = null!;
        private Button navViewCirculation = null!;
        private Button navViewReports = null!;
        private Button navArchive = null!;
        private Button navSignOut = null!;
        private Panel sidebarBottomPanel = null!;

        private Panel userSubMenuPanel = null!;
        private Panel circulationSubMenuPanel = null!;
        private Button navClient = null!;
        private Button navLibrarian = null!;
        private Button navMember = null!;
        private Button navBorrowing = null!;
        private Button navReturns = null!;
        private Button navFines = null!;
        private Label sidebarSubTitleLabel = null!;
        private Button? activeNavButton = null;
        private bool userMenuExpanded = true;
        private bool circulationMenuExpanded = true;

        // Dashboard home panels
        private Panel heroLibrariesCard = new Panel();
        private Panel heroUsersCard = new Panel();
        private Panel heroAlertsCard = new Panel();

        private Label lblHeroLibrariesTitle = new Label();
        private Label lblHeroLibrariesValue = new Label();
        private Label lblHeroLibrariesTrend = new Label();
        private Label lblHeroUsersTitle = new Label();
        private Label lblHeroUsersValue = new Label();
        private Label lblHeroAlertsTitle = new Label();
        private Label lblHeroAlertsValue = new Label();

        private Panel speedPanel = new Panel();
        private Label lblSpeedTitle = new Label();
        private Label lblSpeedSub = new Label();
        private Panel speedChartCard = new Panel();
        private readonly List<Panel> speedBars = new List<Panel>();

        private Panel librariesPanel = new Panel();
        private Label lblLibrariesTitle = new Label();
        private LinkLabel lnkSeeAll = new LinkLabel();
        private DataGridView dgvLibraries = new DataGridView();

        private Panel eventsPanel = new Panel();
        private Label lblEventsTitle = new Label();
        private FlowLayoutPanel eventsFlow = new FlowLayoutPanel();
        private Button fabMonitor = new Button();

        // Palette
        private readonly Color SidebarBack = ColorTranslator.FromHtml("#2B3234");
        private readonly Color SidebarDeep = ColorTranslator.FromHtml("#1F2628");
        private readonly Color SidebarHover = Color.FromArgb(58, 66, 68);
        private readonly Color SidebarActive = Color.FromArgb(18, 0, 184, 148);
        private readonly Color AccentMint = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");

        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color TopBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color FooterBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color CardSoft = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color CardSoftLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color CardDark = ColorTranslator.FromHtml("#2B3234");
        private readonly Color Outline = ColorTranslator.FromHtml("#BBCAC3");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#64748B");
        private readonly Color Tertiary = ColorTranslator.FromHtml("#A03F30");
        private readonly Color GoodSoft = ColorTranslator.FromHtml("#B7EBD7");

        public DashboardForm()
        {
            InitializeComponent();

            SetupResponsiveShell();
            SetupSidebarBranding();
            BuildSuperAdminSidebar();
            BuildSuperAdminSidebar();
            SetupResponsiveHeader();
            SetupTopSearch();
            SetupDashboardHome();

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

            Load += DashboardForm_Load;
            Resize += DashboardForm_Resize;
            topbar.Resize += (s, e) => PositionResponsiveHeader();
            Sidebar.Resize += (s, e) => LayoutSidebar();
        }

        private void DashboardForm_Load(object? sender, EventArgs e)
        {
            LoadUserInfo();
            RefreshDashboardData();
            ApplyTheme(UserSession.Theme);
            StartClock();

            label4.Text = "Dashboard";
            panelContent.Visible = false;

            SetActiveNavButton(navDashboard);
            UpdateTopHeaderSearchVisibility();
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

            topbar.Height = 68;
            panel1.Height = 40;

            MinimumSize = new Size(1180, 760);
            BackColor = FormBack;

            AutoScroll = true;
            AutoScrollMinSize = new Size(0, 1200);
        }

        private void UpdateTopHeaderSearchVisibility()
        {
            if (topSearchHost == null) return;

            topSearchHost.Visible = !panelContent.Visible;
        }


        private void SetupSidebarBranding()
        {
            if (!Sidebar.Controls.Contains(sidebarBrandPanel))
            {
                sidebarBrandPanel.Name = "sidebarBrandPanel";
                sidebarBrandPanel.Height = 104;
                sidebarBrandPanel.Dock = DockStyle.Top;
                sidebarBrandPanel.BackColor = SidebarBack;

                if (pictureBox1.Parent != null) pictureBox1.Parent.Controls.Remove(pictureBox1);
                if (label1.Parent != null) label1.Parent.Controls.Remove(label1);

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.BackColor = SidebarBack;
                label1.BackColor = SidebarBack;

                sidebarSubTitleLabel = new Label
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(190, 203, 210),
                    BackColor = SidebarBack,
                    Text = "Super Admin Dashboard"
                };

                sidebarBrandPanel.Controls.Add(pictureBox1);
                sidebarBrandPanel.Controls.Add(label1);
                sidebarBrandPanel.Controls.Add(sidebarSubTitleLabel);
                Sidebar.Controls.Add(sidebarBrandPanel);
                sidebarBrandPanel.BringToFront();
            }

            LayoutSidebarBranding(true);
        }

        private void LayoutSidebarBranding(bool showText)
        {
            sidebarBrandPanel.Width = Sidebar.Width;
            sidebarBrandPanel.Height = showText ? 104 : 72;

            if (showText)
            {
                pictureBox1.Visible = true;
                label1.Visible = true;
                sidebarSubTitleLabel.Visible = true;

                pictureBox1.Size = new Size(38, 38);
                pictureBox1.Location = new Point(18, 22);

                label1.AutoSize = true;
                label1.Text = "LibraFlow ERP";
                label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                label1.ForeColor = AccentMint;
                label1.Location = new Point(64, 24);

                sidebarSubTitleLabel.Location = new Point(64, 50);
            }
            else
            {
                pictureBox1.Visible = true;
                label1.Visible = false;
                sidebarSubTitleLabel.Visible = false;

                pictureBox1.Size = new Size(34, 34);
                pictureBox1.Location = new Point((Sidebar.Width - pictureBox1.Width) / 2, 18);
            }
        }

        private void BuildSuperAdminSidebar()
        {
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            button8.Visible = false;

            if (superAdminMenuHost != null && Sidebar.Controls.Contains(superAdminMenuHost))
                return;

            sidebarBottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 86,
                BackColor = SidebarBack,
                Padding = new Padding(14, 12, 14, 14)
            };

            superAdminMenuHost = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = SidebarBack,
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 10, 14, 10)
            };

            navDashboard = CreateSidebarNavButton("▦", "Dashboard");
            navManageClients = CreateSidebarNavButton("✦", "Manage Client Libraries");
            navMonitorSystem = CreateSidebarNavButton("◉", "Monitor System");
            navConfigureModules = CreateSidebarNavButton("⚙", "Configure Modules");
            navViewBookCatalog = CreateSidebarNavButton("📖", "View Book Catalog");
            navViewUser = CreateSidebarNavButton("👥", "View User", true);
            navViewCirculation = CreateSidebarNavButton("⇄", "View Circulation", true);
            navViewReports = CreateSidebarNavButton("〽", "View Reports");
            navArchive = CreateSidebarNavButton("🗂", "Archive");
            navSignOut = CreateSidebarNavButton("⏻", "Sign Out");

            navClient = CreateSidebarSubButton("Client");
            navLibrarian = CreateSidebarSubButton("Librarian");
            navMember = CreateSidebarSubButton("Member");
            navBorrowing = CreateSidebarSubButton("View Borrowing");
            navReturns = CreateSidebarSubButton("View Returns");
            navFines = CreateSidebarSubButton("View Fines");

            userSubMenuPanel = CreateSubMenuPanel();
            userSubMenuPanel.Controls.Add(navClient);
            userSubMenuPanel.Controls.Add(navLibrarian);
            userSubMenuPanel.Controls.Add(navMember);

            circulationSubMenuPanel = CreateSubMenuPanel();
            circulationSubMenuPanel.Controls.Add(navBorrowing);
            circulationSubMenuPanel.Controls.Add(navReturns);
            circulationSubMenuPanel.Controls.Add(navFines);

            LayoutSubMenuButtons();

            superAdminMenuHost.Controls.Add(navDashboard);
            superAdminMenuHost.Controls.Add(navManageClients);
            superAdminMenuHost.Controls.Add(navMonitorSystem);
            superAdminMenuHost.Controls.Add(navConfigureModules);
            superAdminMenuHost.Controls.Add(navViewBookCatalog);
            superAdminMenuHost.Controls.Add(navViewUser);
            superAdminMenuHost.Controls.Add(userSubMenuPanel);
            superAdminMenuHost.Controls.Add(navViewCirculation);
            superAdminMenuHost.Controls.Add(circulationSubMenuPanel);
            superAdminMenuHost.Controls.Add(navViewReports);
            superAdminMenuHost.Controls.Add(navArchive);

            sidebarBottomPanel.Controls.Add(navSignOut);

            Sidebar.Controls.Add(superAdminMenuHost);
            Sidebar.Controls.Add(sidebarBottomPanel);

            sidebarBottomPanel.BringToFront();
            superAdminMenuHost.BringToFront();

            navDashboard.Click += (s, e) =>
            {
                SetActiveNavButton(navDashboard);
                ShowDashboardHome();
            };

            navManageClients.Click += (s, e) =>
            {
                SetActiveNavButton(navManageClients);
                LoadContentForm(new ClientsForm());
            };

            navMonitorSystem.Click += (s, e) =>
            {
                SetActiveNavButton(navMonitorSystem);
                LoadContentForm(new MonitoringForm());
            };

            navConfigureModules.Click += (s, e) =>
            {
                SetActiveNavButton(navConfigureModules);
                LoadContentForm(new SettingsForm());
            };

            navViewBookCatalog.Click += (s, e) =>
            {
                SetActiveNavButton(navViewBookCatalog);
                LoadContentForm(new BookCatalogForm());
            };

            navArchive.Click += (s, e) =>
            {
                SetActiveNavButton(navArchive);
                LoadContentForm(new ArchiveForm());
            };

            navClient.Click += (s, e) =>
            {
                SetActiveNavButton(navClient);
                LoadContentForm(new ClientViewForm());

                userMenuExpanded = false;
                userSubMenuPanel.Visible = false;
                UpdateParentArrow(navViewUser, false);
                LayoutSidebar();
            };

            navLibrarian.Click += (s, e) =>
            {
                SetActiveNavButton(navLibrarian);
                LoadContentForm(new LibrarianViewForm());

                userMenuExpanded = false;
                userSubMenuPanel.Visible = false;
                UpdateParentArrow(navViewUser, false);
                LayoutSidebar();
            };

            navMember.Click += (s, e) =>
            {
                SetActiveNavButton(navMember);
                LoadContentForm(new MemberViewForm());

                userMenuExpanded = false;
                userSubMenuPanel.Visible = false;
                UpdateParentArrow(navViewUser, false);
                LayoutSidebar();
            };

            navBorrowing.Click += (s, e) =>
            {
                SetActiveNavButton(navBorrowing);
                LoadContentForm(new BorrowingViewForm());

                circulationMenuExpanded = false;
                circulationSubMenuPanel.Visible = false;
                UpdateParentArrow(navViewCirculation, false);
                LayoutSidebar();
            };

            navReturns.Click += (s, e) =>
            {
                SetActiveNavButton(navReturns);
                LoadContentForm(new ReturnsViewForm());

                circulationMenuExpanded = false;
                circulationSubMenuPanel.Visible = false;
                UpdateParentArrow(navViewCirculation, false);
                LayoutSidebar();
            };

            navFines.Click += (s, e) =>
            {
                SetActiveNavButton(navFines);
                LoadContentForm(new FinesViewForm());

                circulationMenuExpanded = false;
                circulationSubMenuPanel.Visible = false;
                UpdateParentArrow(navViewCirculation, false);
                LayoutSidebar();
            };

            navViewReports.Click += (s, e) =>
            {
                SetActiveNavButton(navViewReports);
                LoadContentForm(new ReportsViewForm());
            };

            navViewUser.Click += (s, e) =>
            {
                userMenuExpanded = !userMenuExpanded;
                userSubMenuPanel.Visible = userMenuExpanded;
                UpdateParentArrow(navViewUser, userMenuExpanded);
                LayoutSidebar();
            };

            navViewCirculation.Click += (s, e) =>
            {
                circulationMenuExpanded = !circulationMenuExpanded;
                circulationSubMenuPanel.Visible = circulationMenuExpanded;
                UpdateParentArrow(navViewCirculation, circulationMenuExpanded);
                LayoutSidebar();
            };

            navSignOut.Click += (s, e) => DoLogout();

            SetActiveNavButton(navDashboard);
        }

        private Button CreateSidebarNavButton(string icon, string text, bool hasArrow = false)
        {
            Button btn = new Button
            {
                Width = 220,
                Height = 52,
                FlatStyle = FlatStyle.Flat,
                BackColor = SidebarBack,
                ForeColor = Color.FromArgb(214, 222, 228),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 16, 0),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 6),
                Tag = hasArrow
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = SidebarHover;
            btn.FlatAppearance.MouseDownBackColor = SidebarHover;
            btn.Text = hasArrow ? $"{icon}  {text}        ▾" : $"{icon}  {text}";

            return btn;
        }

        private Button CreateSidebarSubButton(string text)
        {
            Button btn = new Button
            {
                Width = 180,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                BackColor = SidebarBack,
                ForeColor = Color.FromArgb(172, 183, 191),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(28, 0, 0, 0),
                Cursor = Cursors.Hand,
                Margin = new Padding(18, 0, 0, 4),
                Text = text
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 255, 255, 255);

            return btn;
        }

        private Panel CreateSubMenuPanel()
        {
            return new Panel
            {
                Width = 220,
                Height = 0,
                BackColor = SidebarBack,
                Margin = new Padding(0, -2, 0, 6)
            };
        }

        private void LayoutSubMenuButtons()
        {
            if (userSubMenuPanel != null)
            {
                int top = 0;

                navClient.Location = new Point(18, top);
                navClient.Width = userSubMenuPanel.Width - 18;

                top += navClient.Height + 4;

                navLibrarian.Location = new Point(18, top);
                navLibrarian.Width = userSubMenuPanel.Width - 18;

                top += navLibrarian.Height + 4;

                navMember.Location = new Point(18, top);
                navMember.Width = userSubMenuPanel.Width - 18;
            }

            if (circulationSubMenuPanel != null)
            {
                int top = 0;

                navBorrowing.Location = new Point(18, top);
                navBorrowing.Width = circulationSubMenuPanel.Width - 18;

                top += navBorrowing.Height + 4;

                navReturns.Location = new Point(18, top);
                navReturns.Width = circulationSubMenuPanel.Width - 18;

                top += navReturns.Height + 4;

                navFines.Location = new Point(18, top);
                navFines.Width = circulationSubMenuPanel.Width - 18;
            }
        }

        private void UpdateParentArrow(Button btn, bool expanded)
        {
            string raw = btn.Text;
            if (raw.EndsWith("▾") || raw.EndsWith("▸"))
                raw = raw.Substring(0, raw.Length - 1).TrimEnd();

            btn.Text = expanded ? raw + "  ▾" : raw + "  ▸";
        }

        private void SetActiveNavButton(Button btn)
        {
            activeNavButton = btn;
            ApplyNewSidebarVisualState();
        }

        private void ApplyNewSidebarVisualState()
        {
            if (superAdminMenuHost == null) return;

            foreach (Control ctrl in superAdminMenuHost.Controls)
            {
                if (ctrl is Button btn && btn != navSignOut)
                {
                    btn.BackColor = SidebarBack;
                    btn.ForeColor = Color.FromArgb(214, 222, 228);
                }
                else if (ctrl is Panel panel)
                {
                    foreach (Control sub in panel.Controls)
                    {
                        if (sub is Button subBtn)
                        {
                            subBtn.BackColor = SidebarBack;
                            subBtn.ForeColor = Color.FromArgb(172, 183, 191);
                        }
                    }
                }
            }

            if (activeNavButton != null)
            {
                activeNavButton.BackColor = Color.FromArgb(25, 0, 184, 148);
                activeNavButton.ForeColor = AccentMint;
            }

            navSignOut.BackColor = Color.FromArgb(18, 255, 255, 255);
            navSignOut.ForeColor = Color.FromArgb(255, 138, 128);
        }

        private void SetupTopSearch()
        {
            if (topbar.Controls.Contains(topSearchHost)) return;

            topSearchHost = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#F1F5F9"),
                Height = 38,
                Width = 420
            };

            topSearchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 15F, FontStyle.Regular),
                ForeColor = SecondaryText,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            topSearchBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = topSearchHost.BackColor,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 10F),
                Text = "Search system resources..."
            };

            topSearchBox.GotFocus += (s, e) =>
            {
                if (topSearchBox.Text == "Search system resources...")
                {
                    topSearchBox.Text = "";
                    topSearchBox.ForeColor = PrimaryText;
                }
            };

            topSearchBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(topSearchBox.Text))
                {
                    topSearchBox.Text = "Search system resources...";
                    topSearchBox.ForeColor = SecondaryText;
                }
            };

            topSearchHost.Controls.Add(topSearchIcon);
            topSearchHost.Controls.Add(topSearchBox);
            topbar.Controls.Add(topSearchHost);
        }

        private void SetupResponsiveHeader()
        {
            if (topbar == null) return;
            if (topbar.Controls.Contains(userHeaderHost)) return;

            userHeaderHost.SuspendLayout();
            userHeaderPanel.SuspendLayout();

            userHeaderHost.Name = "userHeaderHost";
            userHeaderHost.BackColor = TopBack;
            userHeaderHost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            userHeaderHost.Height = 52;

            userHeaderPanel.Name = "userHeaderPanel";
            userHeaderPanel.AutoSize = true;
            userHeaderPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            userHeaderPanel.WrapContents = false;
            userHeaderPanel.FlowDirection = FlowDirection.LeftToRight;
            userHeaderPanel.Margin = new Padding(0);
            userHeaderPanel.Padding = new Padding(0);
            userHeaderPanel.BackColor = TopBack;

            if (ProfileImage.Parent != null) ProfileImage.Parent.Controls.Remove(ProfileImage);
            if (username.Parent != null) username.Parent.Controls.Remove(username);
            if (dropdownarrow.Parent != null) dropdownarrow.Parent.Controls.Remove(dropdownarrow);

            ProfileImage.Size = new Size(38, 38);
            ProfileImage.SizeMode = PictureBoxSizeMode.Zoom;
            ProfileImage.Margin = new Padding(0, 7, 10, 0);
            ProfileImage.Cursor = Cursors.Hand;
            ProfileImage.BackColor = TopBack;

            username.AutoSize = true;
            username.MaximumSize = new Size(220, 0);
            username.AutoEllipsis = true;
            username.TextAlign = ContentAlignment.MiddleLeft;
            username.Margin = new Padding(0, 14, 8, 0);
            username.BackColor = TopBack;
            username.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);

            dropdownarrow.AutoSize = true;
            dropdownarrow.TextAlign = ContentAlignment.MiddleLeft;
            dropdownarrow.Margin = new Padding(0, 14, 0, 0);
            dropdownarrow.Cursor = Cursors.Hand;
            dropdownarrow.BackColor = TopBack;

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

            if (topSearchHost != null)
            {
                int leftX = 34;
                topSearchHost.Location = new Point(leftX, (topbar.Height - topSearchHost.Height) / 2);
                topSearchHost.Width = Math.Min(700, Math.Max(260, topbar.Width - userHeaderHost.Width - 90));
                topSearchIcon.Location = new Point(12, 8);
                topSearchBox.Location = new Point(42, 10);
                topSearchBox.Width = topSearchHost.Width - 54;
            }
        }

        private void SetupDashboardHome()
        {
            dashboardSubTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent,
                Text = "Admin Dashboard"
            };

            CreateHeroCards();
            SetupSystemSpeedPanel();
            SetupLibrariesPanel();
            SetupLatestEventsPanel();

            fabMonitor = new Button
            {
                Text = "📈",
                Size = new Size(58, 58),
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 20F),
                Cursor = Cursors.Hand,
                Visible = true
            };
            fabMonitor.FlatAppearance.BorderSize = 0;
            fabMonitor.Click += (s, e) => LoadContentForm(new MonitoringForm());

            Controls.Add(dashboardSubTitle);
            Controls.Add(heroLibrariesCard);
            Controls.Add(heroUsersCard);
            Controls.Add(heroAlertsCard);
            Controls.Add(speedPanel);
            Controls.Add(librariesPanel);
            Controls.Add(eventsPanel);
            Controls.Add(fabMonitor);

            dashboardSubTitle.BringToFront();
            heroLibrariesCard.BringToFront();
            heroUsersCard.BringToFront();
            heroAlertsCard.BringToFront();
            speedPanel.BringToFront();
            librariesPanel.BringToFront();
            eventsPanel.BringToFront();
            fabMonitor.BringToFront();

            TotalClients.Visible = false;
            TotalMembers.Visible = false;
            TotalBooks.Visible = false;
            ActiveBorrowings.Visible = false;
            chart1.Visible = false;
            chart2.Visible = false;
        }

        private void CreateHeroCards()
        {
            heroLibrariesCard = CreateSoftCard(CardBack);
            heroUsersCard = CreateSoftCard(CardDark);
            heroAlertsCard = CreateSoftCard(CardSoft);

            lblHeroLibrariesTitle = CreateLabel("TOTAL LIBRARIES", 11F, FontStyle.Bold, Color.FromArgb(60, 70, 72));
            lblHeroLibrariesValue = CreateLabel("1,284", 34F, FontStyle.Bold, PrimaryText);
            lblHeroLibrariesTrend = CreateLabel("↗ +12% from last month", 10.5F, FontStyle.Bold, AccentDeep);

            lblHeroUsersTitle = CreateLabel("CURRENT USERS", 10F, FontStyle.Bold, AccentMint);
            lblHeroUsersValue = CreateLabel("14.2k", 30F, FontStyle.Bold, AccentMint);

            lblHeroAlertsTitle = CreateLabel("SYSTEM ALERTS", 10F, FontStyle.Bold, Color.FromArgb(60, 70, 72));
            lblHeroAlertsValue = CreateLabel("03", 30F, FontStyle.Bold, PrimaryText);

            Panel iconUsers = new Panel
            {
                Name = "iconUsers",
                Size = new Size(42, 42),
                BackColor = Color.FromArgb(30, 109, 250, 210)
            };
            Label iconUsersLabel = new Label
            {
                Text = "📊",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 15F),
                ForeColor = AccentMint
            };
            iconUsers.Controls.Add(iconUsersLabel);

            Panel iconAlerts = new Panel
            {
                Name = "iconAlerts",
                Size = new Size(42, 42),
                BackColor = Color.FromArgb(255, 235, 228)
            };
            Label iconAlertsLabel = new Label
            {
                Text = "⚠",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 15F),
                ForeColor = Tertiary
            };
            iconAlerts.Controls.Add(iconAlertsLabel);

            Label iconGlow = new Label
            {
                Text = "✣",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 70F, FontStyle.Regular),
                ForeColor = Color.FromArgb(22, 0, 107, 85),
                BackColor = Color.Transparent,
                Name = "heroGlow"
            };

            heroLibrariesCard.Controls.Add(lblHeroLibrariesTitle);
            heroLibrariesCard.Controls.Add(lblHeroLibrariesValue);
            heroLibrariesCard.Controls.Add(lblHeroLibrariesTrend);
            heroLibrariesCard.Controls.Add(iconGlow);

            heroUsersCard.Controls.Add(iconUsers);
            heroUsersCard.Controls.Add(lblHeroUsersTitle);
            heroUsersCard.Controls.Add(lblHeroUsersValue);

            heroAlertsCard.Controls.Add(iconAlerts);
            heroAlertsCard.Controls.Add(lblHeroAlertsTitle);
            heroAlertsCard.Controls.Add(lblHeroAlertsValue);
        }

        private void SetupSystemSpeedPanel()
        {
            speedPanel = CreateSoftCard(CardSoftLow);

            lblSpeedTitle = CreateLabel("System Speed", 22F, FontStyle.Bold, PrimaryText);
            lblSpeedSub = CreateLabel("Real-time load balancing across regional nodes", 10.5F, FontStyle.Regular, SecondaryText);

            speedChartCard = CreateSoftCard(CardBack);

            speedBars.Clear();

            for (int i = 0; i < 14; i++)
            {
                Panel bar = new Panel
                {
                    BackColor = i == 3 || i == 8 || i == 11 ? AccentEmerald : Color.FromArgb(170, 208, 198),
                    Width = 40,
                    Height = 80
                };
                speedBars.Add(bar);
                speedChartCard.Controls.Add(bar);
            }

            speedPanel.Controls.Add(lblSpeedTitle);
            speedPanel.Controls.Add(lblSpeedSub);
            speedPanel.Controls.Add(speedChartCard);
        }

        private void SetupLibrariesPanel()
        {
            librariesPanel = CreateSoftCard(CardBack);

            lblLibrariesTitle = CreateLabel("Main Libraries", 14F, FontStyle.Bold, PrimaryText);
            lnkSeeAll = new LinkLabel
            {
                Text = "See All",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                LinkColor = AccentDeep,
                ActiveLinkColor = AccentEmerald,
                VisitedLinkColor = AccentDeep
            };

            dgvLibraries = new DataGridView
            {
                BackgroundColor = CardBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 46,
                GridColor = Color.FromArgb(240, 244, 245),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvLibraries.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvLibraries.DefaultCellStyle.ForeColor = PrimaryText;
            dgvLibraries.DefaultCellStyle.BackColor = CardBack;
            dgvLibraries.DefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 251, 252);
            dgvLibraries.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvLibraries.ColumnHeadersDefaultCellStyle.BackColor = CardBack;
            dgvLibraries.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvLibraries.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvLibraries.ColumnHeadersDefaultCellStyle.SelectionBackColor = CardBack;
            dgvLibraries.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvLibraries.RowTemplate.Height = 54;

            dgvLibraries.Columns.Add("LibraryName", "LIBRARY NAME");
            dgvLibraries.Columns.Add("Status", "STATUS");
            dgvLibraries.Columns.Add("Users", "USERS");
            dgvLibraries.Columns.Add("Uptime", "UPTIME");

            dgvLibraries.Columns["LibraryName"].FillWeight = 42;
            dgvLibraries.Columns["Status"].FillWeight = 22;
            dgvLibraries.Columns["Users"].FillWeight = 16;
            dgvLibraries.Columns["Uptime"].FillWeight = 20;

            foreach (DataGridViewColumn col in dgvLibraries.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvLibraries.Rows.Add("Metropolitan Public Library", "Running Well", "4,502", "99.98%");
            dgvLibraries.Rows.Add("Oxford Academic Wing", "Active", "12,190", "99.99%");
            dgvLibraries.Rows.Add("Tech-Innovation Hub", "Small Issue", "840", "94.12%");
            dgvLibraries.Rows.Add("Central Research Library", "Running Well", "8,304", "99.87%");
            dgvLibraries.Rows.Add("City Learning Commons", "Active", "5,221", "99.92%");
            dgvLibraries.Rows.Add("West Campus Library", "Running Well", "3,940", "99.71%");
            dgvLibraries.Rows.Add("Digital Knowledge Hub", "Active", "7,112", "99.95%");

            dgvLibraries.CellPainting += DgvLibraries_CellPainting;

            librariesPanel.Controls.Add(lblLibrariesTitle);
            librariesPanel.Controls.Add(lnkSeeAll);
            librariesPanel.Controls.Add(dgvLibraries);
        }

        private void SetupLatestEventsPanel()
        {
            eventsPanel = CreateSoftCard(CardBack);

            lblEventsTitle = CreateLabel("Latest Events", 14F, FontStyle.Bold, PrimaryText);

            eventsFlow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                BackColor = CardBack
            };

            eventsPanel.Controls.Add(lblEventsTitle);
            eventsPanel.Controls.Add(eventsFlow);

            AddEventItem("New Library Added", "National University of Arts", "12 minutes ago", AccentEmerald, "+");
            AddEventItem("System Alert", "Server node us-east-2 slow", "1 hour ago", Tertiary, "!");
            AddEventItem("Software Update", "Successfully sent to all sites", "4 hours ago", AccentDeep, "⟳");
            AddEventItem("Backup Completed", "Cloud archive sync finished", "6 hours ago", AccentDeep, "✓");
            AddEventItem("Client Expansion", "West Campus Library onboarded", "9 hours ago", AccentEmerald, "+");
        }

        private void AddEventItem(string title, string sub, string timeText, Color accent, string symbol)
        {
            Panel item = new Panel
            {
                Width = 280,
                Height = 64,
                BackColor = CardBack,
                Margin = new Padding(0, 0, 0, 6)
            };

            Panel dot = new Panel
            {
                Size = new Size(22, 22),
                BackColor = Color.FromArgb(30, accent),
                Location = new Point(0, 2)
            };
            Label dotLabel = new Label
            {
                Text = symbol,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = accent
            };
            dot.Controls.Add(dotLabel);

            Panel line = new Panel
            {
                Size = new Size(2, 40),
                BackColor = Color.FromArgb(24, 60, 70, 72),
                Location = new Point(10, 22)
            };

            Label t = CreateLabel(title, 10.5F, FontStyle.Bold, PrimaryText);
            Label s = CreateLabel(sub, 9.5F, FontStyle.Regular, SecondaryText);
            Label tm = CreateLabel(timeText, 8.8F, FontStyle.Regular, SecondaryText);

            t.Location = new Point(32, 0);
            s.Location = new Point(32, 20);
            tm.Location = new Point(32, 40);

            item.Controls.Add(line);
            item.Controls.Add(dot);
            item.Controls.Add(t);
            item.Controls.Add(s);
            item.Controls.Add(tm);

            eventsFlow.Controls.Add(item);
        }

        private Panel CreateSoftCard(Color backColor)
        {
            Panel panel = new Panel
            {
                BackColor = backColor,
                BorderStyle = BorderStyle.None
            };

            panel.Paint += (s, e) =>
            {
                using Pen pen = new Pen(Color.FromArgb(30, Outline), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            };

            return panel;
        }

        private Label CreateLabel(string text, float size, FontStyle style, Color color)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private void ApplyResponsiveLayout()
        {
            int w = ClientSize.Width;

            if (w >= 1320)
                ApplyDesktopLayout();
            else if (w >= 1060)
                ApplyTabletLayout();
            else
                ApplySmallLayout();

            PositionResponsiveHeader();
            LayoutSidebar();
            LayoutDashboardHome();
        }

        private void ApplyDesktopLayout()
        {
            Sidebar.Width = 255;
            topbar.Height = 68;
            panel1.Height = 40;
            username.MaximumSize = new Size(220, 0);

            LayoutSidebarBranding(true);
            LayoutSidebar();
        }

        private void ApplyTabletLayout()
        {
            Sidebar.Width = 215;
            topbar.Height = 64;
            panel1.Height = 40;
            username.MaximumSize = new Size(160, 0);

            LayoutSidebarBranding(true);
            LayoutSidebar();
        }

        private void ApplySmallLayout()
        {
            Sidebar.Width = 88;
            topbar.Height = 58;
            panel1.Height = 38;
            username.MaximumSize = new Size(100, 0);

            LayoutSidebarBranding(false);
            LayoutSidebar();
        }

        private void LayoutSidebar()
        {
            LayoutSidebarBranding(Sidebar.Width > 120);

            if (superAdminMenuHost != null)
            {
                superAdminMenuHost.Width = Sidebar.Width;

                int navWidth = Sidebar.Width - 28;

                navDashboard.Width = navWidth;
                navManageClients.Width = navWidth;
                navMonitorSystem.Width = navWidth;
                navConfigureModules.Width = navWidth;
                navViewBookCatalog.Width = navWidth;
                navViewUser.Width = navWidth;
                navViewCirculation.Width = navWidth;
                navViewReports.Width = navWidth;
                navArchive.Width = navWidth;

                userSubMenuPanel.Width = navWidth;
                circulationSubMenuPanel.Width = navWidth;

                navClient.Width = navWidth - 18;
                navLibrarian.Width = navWidth - 18;
                navMember.Width = navWidth - 18;
                navBorrowing.Width = navWidth - 18;
                navReturns.Width = navWidth - 18;
                navFines.Width = navWidth - 18;

                LayoutSubMenuButtons();

                userSubMenuPanel.Height = userMenuExpanded ? 110 : 0;
                circulationSubMenuPanel.Height = circulationMenuExpanded ? 110 : 0;
            }

            if (sidebarBottomPanel != null)
            {
                sidebarBottomPanel.Width = Sidebar.Width;
                navSignOut.Width = Sidebar.Width - 28;
                navSignOut.Height = 48;
                navSignOut.Location = new Point(14, 12);
            }
        }

        private void LayoutDashboardHome()
        {
            if (!label4.Visible) return;

            int sidebarW = Sidebar.Width;
            int topH = topbar.Height;
            int footerH = panel1.Height;

            int left = sidebarW + 30;
            int top = topH + 20;
            int width = ClientSize.Width - sidebarW - 60;

            label4.Location = new Point(left, top + 6);
            label4.Text = "Dashboard";
            label4.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            label4.ForeColor = PrimaryText;
            label4.Visible = true;

            dashboardSubTitle.Location = new Point(left + 2, label4.Bottom + 6);
            dashboardSubTitle.Text = "Admin Dashboard";
            dashboardSubTitle.Visible = true;

            int heroTop = dashboardSubTitle.Bottom + 22;
            int heroGap = 20;

            int librariesW = (int)(width * 0.47);
            int usersW = (int)(width * 0.22);
            int alertsW = width - librariesW - usersW - (heroGap * 2);

            heroLibrariesCard.Bounds = new Rectangle(left, heroTop, librariesW, 160);
            heroUsersCard.Bounds = new Rectangle(heroLibrariesCard.Right + heroGap, heroTop, usersW, 160);
            heroAlertsCard.Bounds = new Rectangle(heroUsersCard.Right + heroGap, heroTop, alertsW, 160);

            LayoutHeroCards();

            int speedTop = heroLibrariesCard.Bottom + 26;
            speedPanel.Bounds = new Rectangle(left, speedTop, width, 390);
            LayoutSpeedPanel();

            int bottomTop = speedPanel.Bottom + 22;
            int bottomGap = 24;

            int librariesPanelW = (int)(width * 0.66);
            int eventsPanelW = width - librariesPanelW - bottomGap;

            int bottomPanelsHeight = 420;

            librariesPanel.Bounds = new Rectangle(left, bottomTop, librariesPanelW, bottomPanelsHeight);
            eventsPanel.Bounds = new Rectangle(librariesPanel.Right + bottomGap, bottomTop, eventsPanelW, bottomPanelsHeight);

            LayoutLibrariesPanel();
            LayoutEventsPanel();

            fabMonitor.Location = new Point(
                ClientSize.Width - fabMonitor.Width - 26,
                ClientSize.Height - footerH - fabMonitor.Height - 26
            );

            panel1.Height = 40;
            time.AutoSize = true;
            time.Location = new Point(10, Math.Max(0, (panel1.Height - time.Height) / 2));

            int dashboardBottom = eventsPanel.Bottom + 40;
            AutoScrollMinSize = new Size(0, dashboardBottom + panel1.Height);

            panel1.BringToFront();
        }

        private void LayoutHeroCards()
        {
            lblHeroLibrariesTitle.Location = new Point(34, 34);
            lblHeroLibrariesValue.Location = new Point(34, 58);
            lblHeroLibrariesTrend.Location = new Point(36, 126);

            Control? glow = heroLibrariesCard.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "heroGlow");
            if (glow != null)
                glow.Location = new Point(heroLibrariesCard.Width - 130, 8);

            Control? iconUsers = heroUsersCard.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "iconUsers");
            if (iconUsers != null) iconUsers.Location = new Point(26, 28);
            lblHeroUsersTitle.Location = new Point(26, 82);
            lblHeroUsersValue.Location = new Point(26, 106);

            Control? iconAlerts = heroAlertsCard.Controls.Cast<Control>().FirstOrDefault(c => c.Name == "iconAlerts");
            if (iconAlerts != null) iconAlerts.Location = new Point(26, 28);
            lblHeroAlertsTitle.Location = new Point(26, 82);
            lblHeroAlertsValue.Location = new Point(26, 106);
        }

        private void LayoutSpeedPanel()
        {
            lblSpeedTitle.Location = new Point(24, 24);
            lblSpeedSub.Location = new Point(24, 58);

            speedChartCard.Bounds = new Rectangle(24, 96, speedPanel.Width - 48, speedPanel.Height - 124);

            int innerPad = 18;
            int chartBottom = speedChartCard.Height - 18;
            int gap = 8;
            int barW = (speedChartCard.Width - (innerPad * 2) - (gap * (speedBars.Count - 1))) / speedBars.Count;

            int[] heights = { 90, 148, 115, 196, 160, 126, 102, 136, 210, 148, 116, 176, 136, 90 };

            for (int i = 0; i < speedBars.Count; i++)
            {
                speedBars[i].Width = barW;
                speedBars[i].Height = heights[i];
                speedBars[i].Left = innerPad + (i * (barW + gap));
                speedBars[i].Top = chartBottom - heights[i];
            }
        }
        private void LayoutLibrariesPanel()
        {
            lblLibrariesTitle.Location = new Point(24, 22);
            lnkSeeAll.Location = new Point(librariesPanel.Width - lnkSeeAll.PreferredWidth - 24, 26);

            dgvLibraries.Location = new Point(0, 64);
            dgvLibraries.Size = new Size(librariesPanel.Width - 2, librariesPanel.Height - 76);
        }
        private void LayoutEventsPanel()
        {
            lblEventsTitle.Location = new Point(24, 22);
            eventsFlow.Location = new Point(24, 62);
            eventsFlow.Size = new Size(eventsPanel.Width - 48, eventsPanel.Height - 86);

            foreach (Control ctrl in eventsFlow.Controls)
                ctrl.Width = eventsFlow.Width - 2;
        }

        private void RefreshDashboardData()
        {
            LoadUserInfo();
            label4.Text = "Dashboard";

            lblHeroLibrariesValue.Text = "1,284";
            lblHeroUsersValue.Text = "14.2k";
            lblHeroAlertsValue.Text = "03";
        }

        public void LoadUserInfo()
        {
            username.Text = string.IsNullOrWhiteSpace(UserSession.Username)
                ? "Super Admin"
                : UserSession.Username;

            try
            {
                if (!string.IsNullOrWhiteSpace(UserSession.ImagePath) && File.Exists(UserSession.ImagePath))
                {
                    using var fs = new FileStream(UserSession.ImagePath, FileMode.Open, FileAccess.Read);
                    ProfileImage.Image = Image.FromStream(fs);
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

            PositionResponsiveHeader();
        }

        private void ApplyBranding()
        {
            try
            {
                string logoPath = Path.Combine(Application.StartupPath, "Assets", "logo.png");

                if (!File.Exists(logoPath))
                    logoPath = FindAssetPath("logo.png");

                if (File.Exists(logoPath))
                {
                    if (pictureBox1.Image != null)
                    {
                        var oldImage = pictureBox1.Image;
                        pictureBox1.Image = null;
                        oldImage.Dispose();
                    }

                    using var fs = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
                    pictureBox1.Image = Image.FromStream(fs);
                }

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch
            {
            }

            label1.ForeColor = AccentMint;
            label1.BackColor = SidebarBack;
            pictureBox1.BackColor = SidebarBack;
            sidebarBrandPanel.BackColor = SidebarBack;
            if (sidebarSubTitleLabel != null) sidebarSubTitleLabel.BackColor = SidebarBack;
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
            time.Font = new Font("Segoe UI", 8.8F, FontStyle.Regular);
            time.ForeColor = SecondaryText;
            time.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void DgvLibraries_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvLibraries.Columns[e.ColumnIndex].Name;

            if (colName == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Color back = GoodSoft;
                Color fore = AccentDeep;

                if (text == "Small Issue")
                {
                    back = Color.FromArgb(255, 214, 204);
                    fore = Tertiary;
                }

                Size size = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 6,
                    e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                    size.Width + 18,
                    24
                );

                if (e.Graphics != null)
                {
                    using (SolidBrush brush = new SolidBrush(back))
                        e.Graphics.FillRectangle(brush, badge);

                    TextRenderer.DrawText(
                        e.Graphics,
                        text,
                        new Font("Segoe UI", 8.5F, FontStyle.Bold),
                        badge,
                        fore,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    );
                }

                e.Handled = true;
            }
            else if (colName == "Uptime")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Color fore = text == "94.12%" ? Tertiary : AccentDeep;

                if (e.Graphics != null)
                {
                    TextRenderer.DrawText(
                        e.Graphics,
                        text,
                        new Font("Segoe UI", 10F, FontStyle.Bold),
                        e.CellBounds,
                        fore,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                    );
                }

                e.Handled = true;
            }
        }

        private void ShowDashboardHome()
        {
            RefreshDashboardData();
            ApplyTheme(UserSession.Theme);

            label4.Visible = true;
            dashboardSubTitle.Visible = true;
            heroLibrariesCard.Visible = true;
            heroUsersCard.Visible = true;
            heroAlertsCard.Visible = true;
            speedPanel.Visible = true;
            librariesPanel.Visible = true;
            eventsPanel.Visible = true;
            fabMonitor.Visible = true;

            panelContent.Controls.Clear();
            panelContent.Visible = false;

            SetActiveNavButton(navDashboard);
            UpdateTopHeaderSearchVisibility();
            ApplyResponsiveLayout();
        }

        private void HideDashboardHome()
        {
            label4.Visible = false;
            dashboardSubTitle.Visible = false;
            heroLibrariesCard.Visible = false;
            heroUsersCard.Visible = false;
            heroAlertsCard.Visible = false;
            speedPanel.Visible = false;
            librariesPanel.Visible = false;
            eventsPanel.Visible = false;
            fabMonitor.Visible = false;
            panel1.Visible = true;

            UpdateTopHeaderSearchVisibility();
        }

        public void LoadContentForm(Form form)
        {
            HideDashboardHome();

            panelContent.Controls.Clear();
            panelContent.Visible = true;
            UpdateTopHeaderSearchVisibility();
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
                login = new LoginForm();

            login.Show();
            login.BringToFront();
            login.WindowState = FormWindowState.Normal;

            Close();
        }

        public void ApplyTheme(string theme)
        {
            BackColor = FormBack;
            Sidebar.BackColor = SidebarBack;
            topbar.BackColor = TopBack;
            panel1.BackColor = FooterBack;
            panelContent.BackColor = FormBack;

            label1.ForeColor = AccentMint;
            username.ForeColor = PrimaryText;
            time.ForeColor = SecondaryText;
            label4.ForeColor = PrimaryText;
            dropdownarrow.ForeColor = PrimaryText;
            dashboardSubTitle.ForeColor = SecondaryText;

            userHeaderPanel.BackColor = TopBack;
            userHeaderHost.BackColor = TopBack;
            ProfileImage.BackColor = TopBack;
            username.BackColor = TopBack;
            dropdownarrow.BackColor = TopBack;

            contextMenuStrip1.BackColor = Color.White;
            contextMenuStrip1.ForeColor = PrimaryText;

            foreach (ToolStripItem item in contextMenuStrip1.Items)
            {
                item.BackColor = Color.White;
                item.ForeColor = PrimaryText;
            }

            ApplyBranding();
            ApplyNewSidebarVisualState();
            LayoutSidebar();
            PositionResponsiveHeader();
        }

        private void button1_Click_1(object? sender, EventArgs e) => ShowDashboardHome();
        private void button2_Click(object? sender, EventArgs e) => LoadContentForm(new ClientsForm());
        private void button3_Click(object? sender, EventArgs e) => LoadContentForm(new MonitoringForm());
        private void button4_Click(object? sender, EventArgs e) => LoadContentForm(new OversightForm());
        private void button5_Click(object? sender, EventArgs e) => LoadContentForm(new LogsForm());
        private void button6_Click(object? sender, EventArgs e) => DoLogout();
        private void ArchiveButton_Click(object? sender, EventArgs e) => LoadContentForm(new ArchiveForm());
        private void button8_Click(object? sender, EventArgs e) => LoadContentForm(new OversightForm());

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

        private void topbar_Paint(object? sender, PaintEventArgs e) { }
        private void label1_Click(object? sender, EventArgs e) { }
        private void pictureBox1_Click(object? sender, EventArgs e) { }
        private void label3_Click(object? sender, EventArgs e) { }
        private void pictureBox2_Click_1(object? sender, EventArgs e) { }
        private void panelContent_Paint(object sender, PaintEventArgs e) { }

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