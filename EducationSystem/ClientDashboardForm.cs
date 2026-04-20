using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ClientDashboardForm : Form
    {
        private readonly Color SidebarBack = ColorTranslator.FromHtml("#2B3234");
        private readonly Color SidebarHover = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color SidebarActive = ColorTranslator.FromHtml("#00B894");
        private readonly Color SidebarActiveText = ColorTranslator.FromHtml("#004233");

        private readonly Color AccentMint = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");
        private readonly Color AccentDangerSoft = ColorTranslator.FromHtml("#F7816D");

        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color TopBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color CardSoft = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color CardSoft2 = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color MutedText = ColorTranslator.FromHtml("#BBCAC3");

        private Panel sidebarBrandPanel = null!;
        private Label sidebarSubTitleLabel = null!;
        private Panel sidebarBottomPanel = null!;
        private FlowLayoutPanel clientMenuHost = null!;

        private FlowLayoutPanel userHeaderPanel = new FlowLayoutPanel();
        private Panel userHeaderHost = new Panel();

        private Panel topSearchHost = new Panel();
        private Label topSearchIcon = new Label();
        private TextBox topSearchBox = new TextBox();

        private Button navDashboard = null!;
        private Button navLibrarySetup = null!;
        private Button navCategoryManagement = null!;
        private Button navBookManagement = null!;
        private Button navUserManagement = null!;
        private Button navCirculation = null!;
        private Button navGenerateReports = null!;
        private Button navArchive = null!;
        private Button navSignOut = null!;

        private Panel userSubMenuPanel = null!;
        private Panel circulationSubMenuPanel = null!;
        private Button navLibrarian = null!;
        private Button navMember = null!;
        private Button navBorrow = null!;
        private Button navReturn = null!;
        private Button navFine = null!;

        private Button? activeNavButton = null;
        private bool userMenuExpanded = false;
        private bool circulationMenuExpanded = false;

        private Label pageTitle = null!;
        private Label pageSubTitle = null!;
        private Panel card1 = null!;
        private Panel card2 = null!;
        private Panel card3 = null!;
        private Panel card4 = null!;
        private Panel tableCard = null!;
        private DataGridView dgvRecent = null!;

        private Label recentTitleLabel = null!;
        private LinkLabel recentViewAllLink = null!;

        private System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        private Panel footerPanel = null!;
        private Label time = null!;

        private Form? activeContentForm = null;

        public ClientDashboardForm()
        {
            InitializeComponent();

            SetupResponsiveShell();
            SetupSidebarBranding();
            BuildClientSidebar();
            SetupResponsiveHeader();
            SetupTopSearch();
            SetupFooter();

            profileToolStripMenuItem.Click += profileToolStripMenuItem_Click;
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;

            Resize += ClientDashboardForm_Resize;
            topbar.Resize += (s, e) => PositionResponsiveHeader();
            Sidebar.Resize += (s, e) => LayoutSidebar();
        }

        private void ClientDashboardForm_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            ApplyTheme();
            ApplyBranding();
            StartClock();
            ShowDashboardHome();
        }

        private void ClientDashboardForm_Resize(object? sender, EventArgs e)
        {
            ApplyResponsiveLayout();
            PositionResponsiveHeader();
            PositionFooter();
        }

        private void SetupResponsiveShell()
        {
            Sidebar.Dock = DockStyle.Left;
            topbar.Dock = DockStyle.Top;
            panelContent.Dock = DockStyle.Fill;

            Sidebar.Width = 308;
            topbar.Height = 78;

            MinimumSize = new Size(1280, 820);
            BackColor = FormBack;
            panelContent.BackColor = FormBack;
            topbar.BackColor = TopBack;
        }

        private void SetupSidebarBranding()
        {
            if (!Sidebar.Controls.Contains(sidebarBrandPanel))
            {
                sidebarBrandPanel = new Panel();
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
                    Text = "Client Admin Dashboard"
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

        private void BuildClientSidebar()
        {
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;

            if (clientMenuHost != null && Sidebar.Controls.Contains(clientMenuHost))
                return;

            sidebarBottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 84,
                BackColor = SidebarBack,
                Padding = new Padding(14, 12, 14, 14)
            };

            clientMenuHost = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = SidebarBack,
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 14, 14, 10)
            };

            navDashboard = CreateSidebarNavButton("▦", "Dashboard");
            navLibrarySetup = CreateSidebarNavButton("⚙", "Library Setup");
            navCategoryManagement = CreateSidebarNavButton("◫", "Category Management");
            navBookManagement = CreateSidebarNavButton("📖", "Book Management");
            navUserManagement = CreateSidebarNavButton("👥", "User Management", true);
            navCirculation = CreateSidebarNavButton("⇄", "Circulation", true);
            navGenerateReports = CreateSidebarNavButton("〽", "Generate Reports");
            navArchive = CreateSidebarNavButton("🗂", "Archive");
            navSignOut = CreateSidebarNavButton("⏻", "Sign Out");

            navLibrarian = CreateSidebarSubButton("Librarian");
            navMember = CreateSidebarSubButton("Member");

            navBorrow = CreateSidebarSubButton("Borrow");
            navReturn = CreateSidebarSubButton("Return");
            navFine = CreateSidebarSubButton("Fine");

            userSubMenuPanel = CreateSubMenuPanel();
            userSubMenuPanel.Controls.Add(navLibrarian);
            userSubMenuPanel.Controls.Add(navMember);

            circulationSubMenuPanel = CreateSubMenuPanel();
            circulationSubMenuPanel.Controls.Add(navBorrow);
            circulationSubMenuPanel.Controls.Add(navReturn);
            circulationSubMenuPanel.Controls.Add(navFine);

            LayoutSubMenuButtons();

            clientMenuHost.Controls.Add(navDashboard);
            clientMenuHost.Controls.Add(navLibrarySetup);
            clientMenuHost.Controls.Add(navCategoryManagement);
            clientMenuHost.Controls.Add(navBookManagement);
            clientMenuHost.Controls.Add(navUserManagement);
            clientMenuHost.Controls.Add(userSubMenuPanel);
            clientMenuHost.Controls.Add(navCirculation);
            clientMenuHost.Controls.Add(circulationSubMenuPanel);
            clientMenuHost.Controls.Add(navGenerateReports);
            clientMenuHost.Controls.Add(navArchive);

            sidebarBottomPanel.Controls.Add(navSignOut);

            Sidebar.Controls.Add(clientMenuHost);
            Sidebar.Controls.Add(sidebarBottomPanel);

            sidebarBottomPanel.BringToFront();
            clientMenuHost.BringToFront();

            navDashboard.Click += (s, e) =>
            {
                SetActiveNavButton(navDashboard);

                if (topSearchHost != null)
                    topSearchHost.Visible = true;

                ShowDashboardHome();
            };

            navLibrarySetup.Click += (s, e) =>
            {
                SetActiveNavButton(navLibrarySetup);

                if (topSearchHost != null)
                    topSearchHost.Visible = false;

                LoadContentForm(new LibrarySetupForm());
            };

            navCategoryManagement.Click += (s, e) =>
            {
                SetActiveNavButton(navCategoryManagement);

                if (topSearchHost != null)
                    topSearchHost.Visible = false;

                LoadContentForm(new CategoryManagementForm());
            };

            navBookManagement.Click += (s, e) =>
            {
                SetActiveNavButton(navBookManagement);

                if (topSearchHost != null)
                    topSearchHost.Visible = false;

                LoadContentForm(new BookManagementForm());
            };

            navUserManagement.Click += (s, e) =>
            {
                userMenuExpanded = !userMenuExpanded;
                userSubMenuPanel.Visible = userMenuExpanded;
                UpdateParentArrow(navUserManagement, userMenuExpanded);
                LayoutSidebar();
            };

            navLibrarian.Click += (s, e) =>
            {
                SetActiveNavButton(navLibrarian);
                MessageBox.Show("Librarian module");
            };

            navMember.Click += (s, e) =>
            {
                SetActiveNavButton(navMember);
                MessageBox.Show("Member module");
            };

            navCirculation.Click += (s, e) =>
            {
                circulationMenuExpanded = !circulationMenuExpanded;
                circulationSubMenuPanel.Visible = circulationMenuExpanded;
                UpdateParentArrow(navCirculation, circulationMenuExpanded);
                LayoutSidebar();
            };

            navBorrow.Click += (s, e) =>
            {
                SetActiveNavButton(navBorrow);
                MessageBox.Show("Borrow module");
            };

            navReturn.Click += (s, e) =>
            {
                SetActiveNavButton(navReturn);
                MessageBox.Show("Return module");
            };

            navFine.Click += (s, e) =>
            {
                SetActiveNavButton(navFine);
                MessageBox.Show("Fine module");
            };

            navGenerateReports.Click += (s, e) =>
            {
                SetActiveNavButton(navGenerateReports);
                MessageBox.Show("Reports module");
            };

            navArchive.Click += (s, e) =>
            {
                SetActiveNavButton(navArchive);
                MessageBox.Show("Archive module");
            };

            navSignOut.Click += (s, e) => DoClientSignOut();

            SetActiveNavButton(navDashboard);
        }

        private void SetupTopSearch()
        {
            if (topbar.Controls.Contains(topSearchHost))
                return;

            topSearchHost = new Panel
            {
                BackColor = CardSoft2,
                Height = 44,
                Width = 380
            };

            topSearchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 15F),
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
                Text = "Search collection or members..."
            };

            topSearchBox.GotFocus += (s, e) =>
            {
                if (topSearchBox.Text == "Search collection or members...")
                {
                    topSearchBox.Text = "";
                    topSearchBox.ForeColor = PrimaryText;
                }
            };

            topSearchBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(topSearchBox.Text))
                {
                    topSearchBox.Text = "Search collection or members...";
                    topSearchBox.ForeColor = SecondaryText;
                }
            };

            topSearchHost.Controls.Add(topSearchIcon);
            topSearchHost.Controls.Add(topSearchBox);
            topbar.Controls.Add(topSearchHost);
        }

        private void SetupResponsiveHeader()
        {
            if (topbar.Controls.Contains(userHeaderHost))
                return;

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

            if (ProfileImage.Parent != null)
                ProfileImage.Parent.Controls.Remove(ProfileImage);

            if (username.Parent != null)
                username.Parent.Controls.Remove(username);

            if (dropdownarrow.Parent != null)
                dropdownarrow.Parent.Controls.Remove(dropdownarrow);

            ProfileImage.Size = new Size(38, 38);
            ProfileImage.SizeMode = PictureBoxSizeMode.Zoom;
            ProfileImage.Margin = new Padding(0, 7, 10, 0);
            ProfileImage.Cursor = Cursors.Hand;
            ProfileImage.BackColor = TopBack;

            username.AutoSize = true;
            username.MaximumSize = new Size(240, 0);
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
            dropdownarrow.FlatAppearance.BorderSize = 0;
            dropdownarrow.FlatStyle = FlatStyle.Flat;
            dropdownarrow.Text = "▼";

            userHeaderPanel.Controls.Add(ProfileImage);
            userHeaderPanel.Controls.Add(username);
            userHeaderPanel.Controls.Add(dropdownarrow);

            userHeaderHost.Controls.Add(userHeaderPanel);
            topbar.Controls.Add(userHeaderHost);

            PositionResponsiveHeader();
        }

        private void PositionResponsiveHeader()
        {
            if (!topbar.Controls.Contains(userHeaderHost))
                return;

            userHeaderPanel.PerformLayout();
            userHeaderHost.Width = userHeaderPanel.PreferredSize.Width;
            userHeaderHost.Height = Math.Max(50, userHeaderPanel.PreferredSize.Height);

            int rightPadding = 18;
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
                topSearchIcon.Location = new Point(12, 9);
                topSearchBox.Location = new Point(42, 12);
                topSearchBox.Width = topSearchHost.Width - 54;
            }
        }

        private void SetupDashboardHome()
        {
            pageTitle = new Label
            {
                Text = "School Library Dashboard",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                AutoSize = true
            };

            pageSubTitle = new Label
            {
                Text = "Detailed operational insights and collection monitoring.",
                Font = new Font("Segoe UI", 13F),
                ForeColor = SecondaryText,
                AutoSize = true
            };

            card1 = CreateDashboardCard("24,812", "TOTAL COLLECTION", "+12%", AccentDeep, false, false, "📚");
            card2 = CreateDashboardCard("3,205", "ACTIVE MEMBERS", "Steady", AccentDeep, true, false, "👥");
            card3 = CreateDashboardCard("148", "BOOKS BORROWED", "-4%", AccentDanger, false, false, "⇄");
            card4 = CreateDashboardCard("24", "OVERDUE RETURNS", "Action Needed", AccentDanger, false, true, "!");

            tableCard = new Panel
            {
                BackColor = CardBack
            };

            recentTitleLabel = new Label
            {
                Text = "Recent Activity",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = PrimaryText,
                AutoSize = true
            };

            recentViewAllLink = new LinkLabel
            {
                Text = "View All Activity",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                LinkColor = AccentDeep,
                ActiveLinkColor = AccentEmerald,
                VisitedLinkColor = AccentDeep
            };

            dgvRecent = new DataGridView
            {
                BackgroundColor = CardBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 52,
                GridColor = Color.FromArgb(225, 231, 232),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.None
            };

            dgvRecent.ColumnHeadersDefaultCellStyle.BackColor = CardSoft;
            dgvRecent.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvRecent.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvRecent.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvRecent.DefaultCellStyle.BackColor = CardBack;
            dgvRecent.DefaultCellStyle.ForeColor = PrimaryText;
            dgvRecent.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvRecent.DefaultCellStyle.SelectionBackColor = Color.FromArgb(244, 250, 253);
            dgvRecent.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvRecent.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvRecent.RowTemplate.Height = 68;

            dgvRecent.Columns.Add("Txn", "TRANSACTION ID");
            dgvRecent.Columns.Add("Book", "BOOK TITLE");
            dgvRecent.Columns.Add("Member", "MEMBER");
            dgvRecent.Columns.Add("Date", "DATE");
            dgvRecent.Columns.Add("Status", "STATUS");

            dgvRecent.Columns["Txn"].FillWeight = 18;
            dgvRecent.Columns["Book"].FillWeight = 27;
            dgvRecent.Columns["Member"].FillWeight = 21;
            dgvRecent.Columns["Date"].FillWeight = 17;
            dgvRecent.Columns["Status"].FillWeight = 17;

            dgvRecent.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvRecent.Columns["Date"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRecent.Columns["Status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvRecent.Columns["Date"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRecent.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvRecent.Rows.Add("TRX-9821", "The Great Gatsby\r\nF. Scott Fitzgerald", "John Smith", "Oct 24, 2023", "Returned");
            dgvRecent.Rows.Add("TRX-9822", "Principles of Physics\r\nWalker, Resnick", "Sara Connor", "Oct 25, 2023", "Borrowed");
            dgvRecent.Rows.Add("TRX-9823", "1984\r\nGeorge Orwell", "Mike Wazowski", "Oct 20, 2023", "Overdue");
            dgvRecent.Rows.Add("TRX-9824", "Digital Electronics\r\nMorris Mano", "Elena Gilbert", "Oct 26, 2023", "Borrowed");

            dgvRecent.CellPainting += DgvRecent_CellPainting;

            tableCard.Controls.Add(recentTitleLabel);
            tableCard.Controls.Add(recentViewAllLink);
            tableCard.Controls.Add(dgvRecent);

            panelContent.Controls.Add(pageTitle);
            panelContent.Controls.Add(pageSubTitle);
            panelContent.Controls.Add(card1);
            panelContent.Controls.Add(card2);
            panelContent.Controls.Add(card3);
            panelContent.Controls.Add(card4);
            panelContent.Controls.Add(tableCard);

            LayoutDashboardHome();
        }

        private Panel CreateDashboardCard(string value, string caption, string badgeText, Color badgeColor, bool activeAccent, bool dangerValue, string iconText)
        {
            Panel card = new Panel
            {
                BackColor = CardBack
            };

            Panel accentLeft = new Panel
            {
                Dock = DockStyle.Left,
                Width = 4,
                BackColor = AccentEmerald
            };
            card.Controls.Add(accentLeft);

            Panel iconBox = new Panel
            {
                Size = new Size(44, 44),
                BackColor = dangerValue ? Color.FromArgb(35, 247, 129, 109) : ColorTranslator.FromHtml("#B7EBD7"),
                Location = new Point(24, 24)
            };
            MakeRounded(iconBox, 10);

            Label iconLabel = new Label
            {
                Text = iconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 16F),
                ForeColor = dangerValue ? AccentDanger : AccentDeep
            };
            iconBox.Controls.Add(iconLabel);

            Label badge = new Label
            {
                Text = badgeText,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = badgeColor,
                AutoSize = true,
                BackColor = Color.FromArgb(22, badgeColor),
                Padding = new Padding(8, 4, 8, 4)
            };

            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = dangerValue ? AccentDanger : PrimaryText,
                AutoSize = true,
                Location = new Point(24, 78)
            };

            Label captionLabel = new Label
            {
                Text = caption,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                AutoSize = true,
                Location = new Point(24, 126)
            };

            card.Controls.Add(iconBox);
            card.Controls.Add(badge);
            card.Controls.Add(valueLabel);
            card.Controls.Add(captionLabel);

            card.Resize += (s, e) =>
            {
                badge.Location = new Point(card.Width - badge.Width - 24, 26);
            };

            return card;
        }

        private void LayoutDashboardHome()
        {
            if (pageTitle == null || pageTitle.Parent != panelContent)
                return;

            int left = 42;
            int top = 34;
            int gap = 24;
            int width = panelContent.Width - 84;
            int cardWidth = (width - (gap * 3)) / 4;
            int cardHeight = 168;

            pageTitle.Location = new Point(left, top);
            pageSubTitle.Location = new Point(left, top + 54);

            int cardsTop = top + 112;

            card1.Bounds = new Rectangle(left, cardsTop, cardWidth, cardHeight);
            card2.Bounds = new Rectangle(card1.Right + gap, cardsTop, cardWidth, cardHeight);
            card3.Bounds = new Rectangle(card2.Right + gap, cardsTop, cardWidth, cardHeight);
            card4.Bounds = new Rectangle(card3.Right + gap, cardsTop, cardWidth, cardHeight);

            tableCard.Bounds = new Rectangle(left, card1.Bottom + 34, width, 390);

            recentTitleLabel.Location = new Point(36, 22);
            recentViewAllLink.Location = new Point(
                tableCard.Width - recentViewAllLink.PreferredWidth - 32,
                28
            );

            dgvRecent.Location = new Point(0, 74);
            dgvRecent.Size = new Size(tableCard.Width, tableCard.Height - 74);
        }

        private void SetupFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 34,
                BackColor = TopBack
            };

            time = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.8F, FontStyle.Regular),
                ForeColor = SecondaryText,
                BackColor = TopBack,
                TextAlign = ContentAlignment.MiddleLeft
            };

            footerPanel.Controls.Add(time);
            Controls.Add(footerPanel);
            footerPanel.BringToFront();

            PositionFooter();
        }

        private void PositionFooter()
        {
            if (footerPanel == null || time == null)
                return;

            time.Location = new Point(14, Math.Max(0, (footerPanel.Height - time.Height) / 2));
        }

        private void ApplyResponsiveLayout()
        {
            int w = ClientSize.Width;

            if (w >= 1320)
                Sidebar.Width = 308;
            else if (w >= 1060)
                Sidebar.Width = 250;
            else
                Sidebar.Width = 92;

            LayoutSidebarBranding(Sidebar.Width > 120);
            LayoutSidebar();
            LayoutDashboardHome();
        }

        private void LayoutSidebar()
        {
            LayoutSidebarBranding(Sidebar.Width > 120);

            if (clientMenuHost != null)
            {
                clientMenuHost.Width = Sidebar.Width;
                int navWidth = Sidebar.Width - 28;

                navDashboard.Width = navWidth;
                navLibrarySetup.Width = navWidth;
                navCategoryManagement.Width = navWidth;
                navBookManagement.Width = navWidth;
                navUserManagement.Width = navWidth;
                navCirculation.Width = navWidth;
                navGenerateReports.Width = navWidth;
                navArchive.Width = navWidth;

                userSubMenuPanel.Width = navWidth;
                circulationSubMenuPanel.Width = navWidth;

                navLibrarian.Width = navWidth - 18;
                navMember.Width = navWidth - 18;
                navBorrow.Width = navWidth - 18;
                navReturn.Width = navWidth - 18;
                navFine.Width = navWidth - 18;

                LayoutSubMenuButtons();

                userSubMenuPanel.Height = userMenuExpanded ? 72 : 0;
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

        private void LayoutSubMenuButtons()
        {
            int top = 0;

            navLibrarian.Location = new Point(18, top);
            navLibrarian.Width = userSubMenuPanel.Width - 18;

            top += navLibrarian.Height + 4;
            navMember.Location = new Point(18, top);
            navMember.Width = userSubMenuPanel.Width - 18;

            top = 0;
            navBorrow.Location = new Point(18, top);
            navBorrow.Width = circulationSubMenuPanel.Width - 18;

            top += navBorrow.Height + 4;
            navReturn.Location = new Point(18, top);
            navReturn.Width = circulationSubMenuPanel.Width - 18;

            top += navReturn.Height + 4;
            navFine.Location = new Point(18, top);
            navFine.Width = circulationSubMenuPanel.Width - 18;
        }

        public void LoadUserInfo()
        {
            username.Text = string.IsNullOrWhiteSpace(ClientSession.LibraryName)
                ? "ABC School Library"
                : ClientSession.LibraryName;

            try
            {
                if (!string.IsNullOrWhiteSpace(ClientSession.ImagePath) && File.Exists(ClientSession.ImagePath))
                {
                    using FileStream fs = new FileStream(ClientSession.ImagePath, FileMode.Open, FileAccess.Read);
                    ProfileImage.Image = Image.FromStream(fs);
                }
                else
                {
                    string defaultPath = Path.Combine(Application.StartupPath, "Assets", "client.png");
                    if (!File.Exists(defaultPath))
                        defaultPath = FindAssetPath("client.png");

                    if (File.Exists(defaultPath))
                    {
                        using FileStream fs = new FileStream(defaultPath, FileMode.Open, FileAccess.Read);
                        ProfileImage.Image = Image.FromStream(fs);
                    }
                    else
                    {
                        ProfileImage.Image = null;
                    }
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
            if (sidebarSubTitleLabel != null)
                sidebarSubTitleLabel.BackColor = SidebarBack;
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
            if (time == null)
                return;

            time.Text = DateTime.Now.ToString("MMMM dd, yyyy - hh:mm:ss tt");
            time.Font = new Font("Segoe UI", 8.8F, FontStyle.Regular);
            time.ForeColor = SecondaryText;
            time.TextAlign = ContentAlignment.MiddleLeft;
            PositionFooter();
        }

        private void ShowDashboardHome()
        {
            if (activeContentForm != null)
            {
                try
                {
                    activeContentForm.Hide();
                    activeContentForm.Dispose();
                }
                catch
                {
                }

                activeContentForm = null;
            }

            panelContent.SuspendLayout();
            panelContent.Controls.Clear();
            panelContent.BackColor = FormBack;

            SetupDashboardHome();

            panelContent.Visible = true;
            panelContent.BringToFront();
            LayoutDashboardHome();
            panelContent.ResumeLayout();
        }

        public void LoadContentForm(Form form)
        {
            if (activeContentForm != null)
            {
                try
                {
                    activeContentForm.Hide();
                    activeContentForm.Dispose();
                }
                catch
                {
                }

                activeContentForm = null;
            }

            panelContent.SuspendLayout();
            panelContent.Controls.Clear();

            activeContentForm = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panelContent.Controls.Add(form);
            form.Show();

            panelContent.ResumeLayout();
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
            ApplySidebarVisualState();
        }

        private void ApplySidebarVisualState()
        {
            if (clientMenuHost == null)
                return;

            foreach (Control ctrl in clientMenuHost.Controls)
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
                activeNavButton.BackColor = SidebarActive;
                activeNavButton.ForeColor = SidebarActiveText;
            }

            navSignOut.BackColor = Color.FromArgb(18, 255, 255, 255);
            navSignOut.ForeColor = Color.FromArgb(255, 138, 128);
        }

        private void ApplyTheme()
        {
            BackColor = FormBack;
            Sidebar.BackColor = SidebarBack;
            topbar.BackColor = TopBack;
            panelContent.BackColor = FormBack;

            label1.ForeColor = AccentMint;
            label1.BackColor = SidebarBack;
            pictureBox1.BackColor = SidebarBack;
            username.ForeColor = PrimaryText;
            username.BackColor = TopBack;
            dropdownarrow.ForeColor = PrimaryText;
            dropdownarrow.BackColor = TopBack;
            ProfileImage.BackColor = TopBack;

            ApplySidebarVisualState();
            PositionResponsiveHeader();
        }

        private void DoClientSignOut()
        {
            Hide();

            try
            {
                LoginForm login = new LoginForm();
                login.StartPosition = FormStartPosition.CenterScreen;
                login.Show();
            }
            catch
            {
                MessageBox.Show("Login form not found. Replace LoginForm with your actual login form class.");
                Show();
                return;
            }

            Close();
        }

        private void DgvRecent_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0)
                return;

            string col = dgv.Columns[e.ColumnIndex].Name;

            if (col == "Book")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                string[] parts = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
                string title = parts.Length > 0 ? parts[0] : text;
                string author = parts.Length > 1 ? parts[1] : "";

                TextRenderer.DrawText(
                    e.Graphics,
                    title,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 14, e.CellBounds.Y + 10, e.CellBounds.Width - 20, 20),
                    PrimaryText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    author,
                    new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    new Rectangle(e.CellBounds.X + 14, e.CellBounds.Y + 32, e.CellBounds.Width - 20, 18),
                    SecondaryText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Color back = text == "Overdue"
                    ? ColorTranslator.FromHtml("#F7816D")
                    : text == "Returned"
                        ? ColorTranslator.FromHtml("#B7EBD7")
                        : Color.FromArgb(48, 109, 250, 210);

                Color fore = text == "Overdue"
                    ? ColorTranslator.FromHtml("#6E1B0F")
                    : AccentDeep;

                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                int badgeWidth = textSize.Width + 18;
                int badgeHeight = 26;

                Rectangle badge = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - badgeWidth) / 2,
                    e.CellBounds.Y + (e.CellBounds.Height - badgeHeight) / 2,
                    badgeWidth,
                    badgeHeight);

                using (SolidBrush b = new SolidBrush(back))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    badge,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void profileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Profile");
        }

        private void settingsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Settings");
        }

        private void dropdownarrow_Click(object? sender, EventArgs e)
        {
            contextMenuStrip1.Show(dropdownarrow, 0, dropdownarrow.Height);
        }

        private void pictureBox2_Click(object? sender, EventArgs e)
        {
            contextMenuStrip1.Show(ProfileImage, 0, ProfileImage.Height);
        }

        private void MakeCircular(Control control)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, control.Width - 1, control.Height - 1);
            control.Region = new Region(path);
        }

        private void MakeRounded(Control control, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(control.Width - d - 1, 0, d, d, 270, 90);
            path.AddArc(control.Width - d - 1, control.Height - d - 1, d, d, 0, 90);
            path.AddArc(0, control.Height - d - 1, d, d, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private void topbar_Paint(object sender, PaintEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}