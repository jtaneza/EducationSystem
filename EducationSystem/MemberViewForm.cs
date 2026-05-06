using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class MemberViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color Outline = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");
        private readonly Color Tertiary = ColorTranslator.FromHtml("#A03F30");

        private Panel canvas = null!;
        private Panel headerPanel = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Panel filterHost = null!;
        private ComboBox cboSchoolFilter = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;
        private Panel filterRow = null!;

        private Panel statsPanel = null!;
        private Panel cardTotal = null!;
        private Panel cardActive = null!;
        private Panel cardPending = null!;
        private Panel cardHealth = null!;

        private Label lblTotalValue = null!;
        private Label lblActiveValue = null!;
        private Label lblPendingValue = null!;
        private Label lblHealthValue = null!;

        private Panel tableCard = null!;
        private Panel tableShell = null!;
        private DataGridView dgvMembers = null!;
        private Panel footerPanel = null!;
        private Label lblFooter = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNext = null!;

        private List<MemberItem> allRows = new List<MemberItem>();
        private List<MemberItem> currentPageRows = new List<MemberItem>();
        private int currentPage = 1;
        private int totalPages = 1;
        private const int PageSize = 5;

        public MemberViewForm()
        {
            InitializeComponent();
            BuildUI();
            LoadMembersFromDatabase();
            LoadSchoolFilter();
            LoadGrid();
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = false;

            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background,
                AutoScroll = true
            };
            canvas.HorizontalScroll.Enabled = false;
            canvas.HorizontalScroll.Visible = false;
            Controls.Add(canvas);

            headerPanel = new Panel
            {
                Height = 150,
                BackColor = Background
            };

            lblTitle = new Label
            {
                Text = "Member Directory",
                Font = new Font("Segoe UI", 25F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "Management and monitoring of system-wide academic personnel.",
                Font = new Font("Segoe UI", 11F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            filterHost = new Panel
            {
                BackColor = SurfaceContainer,
                Size = new Size(220, 38)
            };

            cboSchoolFilter = new ComboBox
            {
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = SurfaceContainer,
                ForeColor = OnSurfaceVariant,
                Font = new Font("Segoe UI", 10F),
                Width = 200
            };
            cboSchoolFilter.SelectedIndexChanged += (s, e) =>
            {
                currentPage = 1;
                LoadGrid();
            };
            filterHost.Controls.Add(cboSchoolFilter);

            searchHost = new Panel
            {
                BackColor = SurfaceContainer,
                Size = new Size(280, 38)
            };

            lblSearchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 16F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = SurfaceContainer,
                ForeColor = OnSurfaceVariant,
                Font = new Font("Segoe UI", 10F),
                Text = "Search by name or email..."
            };
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by name or email...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by name or email...";
                    txtSearch.ForeColor = OnSurfaceVariant;
                }
            };
            txtSearch.TextChanged += (s, e) =>
            {
                currentPage = 1;
                LoadGrid();
            };

            btnFilter = new Button
            {
                Text = "☰",
                FlatStyle = FlatStyle.Flat,
                BackColor = SurfaceContainer,
                ForeColor = OnSurfaceVariant,
                Size = new Size(38, 38),
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderSize = 0;

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubTitle);

            statsPanel = new Panel
            {
                Height = 190,
                BackColor = Background
            };

            cardTotal = CreateStatCard("TOTAL MEMBERS", out lblTotalValue, "+4 this month", Primary, "↗");
            cardActive = CreateStatCard("ACTIVE NOW", out lblActiveValue, "Current Sessions", Primary, "●");
            cardPending = CreateStatCard("PENDING APPROVALS", out lblPendingValue, "Requires Action", Tertiary, "◔");
            cardHealth = CreateStatCard("SYSTEM HEALTH", out lblHealthValue, "Operational", Primary, "🛡");

            statsPanel.Controls.Add(cardTotal);
            statsPanel.Controls.Add(cardActive);
            statsPanel.Controls.Add(cardPending);
            statsPanel.Controls.Add(cardHealth);

            filterRow = new Panel
            {
                Height = 64,
                BackColor = Background
            };
            filterRow.Controls.Add(filterHost);
            filterRow.Controls.Add(searchHost);
            filterRow.Controls.Add(btnFilter);

            tableCard = new Panel
            {
                BackColor = Background,
                Padding = new Padding(34, 0, 34, 24)
            };

            tableShell = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgvMembers = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Surface,
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
                ColumnHeadersHeight = 52,
                GridColor = Outline,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvMembers.RowTemplate.Height = 88;
            dgvMembers.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            dgvMembers.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 10, 8, 10);

            dgvMembers.ColumnHeadersDefaultCellStyle.BackColor = SurfaceLow;
            dgvMembers.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvMembers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvMembers.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceLow;
            dgvMembers.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;

            dgvMembers.DefaultCellStyle.BackColor = Surface;
            dgvMembers.DefaultCellStyle.ForeColor = OnSurface;
            dgvMembers.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvMembers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvMembers.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvMembers.Columns.Add("Id", "ID");
            dgvMembers.Columns.Add("Name", "NAME");
            dgvMembers.Columns.Add("Email", "EMAIL");
            dgvMembers.Columns.Add("School", "SCHOOL");
            dgvMembers.Columns.Add("Phone", "PHONE");
            dgvMembers.Columns.Add("Address", "ADDRESS");
            dgvMembers.Columns.Add("Status", "STATUS");

            dgvMembers.Columns["Id"].FillWeight = 10;
            dgvMembers.Columns["Name"].FillWeight = 20;
            dgvMembers.Columns["Email"].FillWeight = 18;
            dgvMembers.Columns["School"].FillWeight = 18;
            dgvMembers.Columns["Phone"].FillWeight = 14;
            dgvMembers.Columns["Address"].FillWeight = 14;
            dgvMembers.Columns["Status"].FillWeight = 10;

            foreach (DataGridViewColumn col in dgvMembers.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvMembers.CellPainting += dgvMembers_CellPainting;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 64,
                BackColor = Color.FromArgb(250, 252, 252)
            };

            lblFooter = new Label
            {
                Text = "Showing 1 to 4 of 1,402 members",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            btnPrev = CreatePagerButton("‹", false);
            btnPage1 = CreatePagerButton("1", true);
            btnPage2 = CreatePagerButton("2", false);
            btnPage3 = CreatePagerButton("3", false);
            btnNext = CreatePagerButton("›", false);

            btnPrev.Click += (s, e) =>
            {
                if (currentPage <= 1) return;
                currentPage--;
                LoadGrid();
            };

            btnPage1.Click += (s, e) => GoToPage(btnPage1);
            btnPage2.Click += (s, e) => GoToPage(btnPage2);
            btnPage3.Click += (s, e) => GoToPage(btnPage3);

            btnNext.Click += (s, e) =>
            {
                if (currentPage >= totalPages) return;
                currentPage++;
                LoadGrid();
            };

            footerPanel.Controls.Add(lblFooter);
            footerPanel.Controls.Add(btnPrev);
            footerPanel.Controls.Add(btnPage1);
            footerPanel.Controls.Add(btnPage2);
            footerPanel.Controls.Add(btnPage3);
            footerPanel.Controls.Add(btnNext);

            tableShell.Controls.Add(footerPanel);
            tableShell.Controls.Add(dgvMembers);
            tableCard.Controls.Add(tableShell);

            canvas.Controls.Add(tableCard);
            canvas.Controls.Add(filterRow);
            canvas.Controls.Add(statsPanel);
            canvas.Controls.Add(headerPanel);

            Resize += MemberViewForm_Resize;
            AdjustLayout();
        }

        private Panel CreateStatCard(string title, out Label valueLabel, string footerText, Color footerColor, string footerIcon)
        {
            Panel panel = new Panel
            {
                BackColor = SurfaceLow,
                BorderStyle = BorderStyle.None
            };

            Label lblTitleCard = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(24, 20)
            };

            valueLabel = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(24, 48)
            };

            Label lblFooterCard = new Label
            {
                Text = $"{footerIcon} {footerText}",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = footerColor,
                AutoSize = true,
                Location = new Point(24, 100)
            };

            panel.Controls.Add(lblTitleCard);
            panel.Controls.Add(valueLabel);
            panel.Controls.Add(lblFooterCard);

            return panel;
        }

        private Button CreatePagerButton(string text, bool active)
        {
            Button btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                Width = 34,
                Height = 34,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            if (active)
            {
                btn.BackColor = PrimaryContainer;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btn.BackColor = SurfaceContainer;
                btn.ForeColor = OnSurface;
                btn.FlatAppearance.BorderSize = 0;
            }

            return btn;
        }


        private string SafeText(object value, string fallback = "")
        {
            if (value == null || value == DBNull.Value) return fallback;
            string text = Convert.ToString(value) ?? "";
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return "";
            int at = email.IndexOf('@');
            if (at <= 1) return "****";
            return email.Substring(0, 1) + "***" + email.Substring(at);
        }

        private string MaskPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "No phone";

            string digits = new string(phone.Where(char.IsDigit).ToArray());

            if (digits.Length == 0)
                return "No phone";

            if (digits.Length <= 4)
                return "****";

            return "****" + digits.Substring(digits.Length - 4);
        }

        private string MaskAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return "No address";

            string[] parts = address.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length == 0)
                return "No address";

            string first = parts[0].Length <= 1
                ? parts[0] + "***"
                : parts[0].Substring(0, 1) + "***";

            return parts.Length == 1 ? first : first + ", " + parts[^1];
        }

        private void LoadMembersFromDatabase()
        {
            allRows.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureMemberViewSchema(conn);

                const string query = @"
SELECT
    u.UserID,
    u.ClientID,
    u.FullName,
    u.Email,
    u.Phone,
    u.Address,
    u.Role,
    u.Status,
    u.CreatedAt,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown School') AS SchoolName
FROM dbo.Users u
LEFT JOIN dbo.ClientLibraries cl
    ON cl.ClientID = u.ClientID
WHERE UPPER(ISNULL(u.Role, '')) IN ('MEMBER', 'STUDENT', 'TEACHER')
ORDER BY cl.LibraryName ASC, u.UserID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string fullName = SafeText(reader["FullName"], "Unnamed User");
                    string role = SafeText(reader["Role"], "Member");
                    string status = SafeText(reader["Status"], "Active");
                    int userId = Convert.ToInt32(reader["UserID"]);

                    GetStatusColors(status, out string statusBack, out string statusFore);

                    allRows.Add(new MemberItem(
                        "MEM-" + userId.ToString("0000"),
                        GetInitials(fullName),
                        $"{fullName} ({role})",
                        "#B7EBD7",
                        "#1E4F41",
                        MaskEmail(SafeText(reader["Email"])),
                        MaskPhone(SafeText(reader["Phone"])),
                        MaskAddress(SafeText(reader["Address"])),
                        SafeText(reader["SchoolName"], "Unknown School"),
                        status,
                        statusBack,
                        statusFore
                    ));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Member directory could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                allRows.Clear();
            }
        }

        private void EnsureMemberViewSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.ClientLibraries', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ClientLibraries
    (
        ClientID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        LibraryCode NVARCHAR(50) NULL,
        LibraryName NVARCHAR(200) NULL,
        Email NVARCHAR(150) NULL,
        PasswordText NVARCHAR(150) NULL,
        UserCount INT NOT NULL DEFAULT 0,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
        ImagePath NVARCHAR(250) NULL
    );
END;

IF COL_LENGTH('dbo.ClientLibraries', 'LibraryName') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD LibraryName NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
    ALTER TABLE dbo.Users ADD ClientID INT NULL;

IF COL_LENGTH('dbo.Users', 'Role') IS NULL
    ALTER TABLE dbo.Users ADD Role NVARCHAR(50) NULL;

IF COL_LENGTH('dbo.Users', 'Status') IS NULL
    ALTER TABLE dbo.Users ADD Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Users_Status_MemberViewFixed DEFAULT 'Active';

IF COL_LENGTH('dbo.Users', 'CreatedAt') IS NULL
    ALTER TABLE dbo.Users ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt_MemberViewFixed DEFAULT SYSUTCDATETIME();

IF COL_LENGTH('dbo.Users', 'Phone') IS NULL
    ALTER TABLE dbo.Users ADD Phone NVARCHAR(50) NULL;

IF COL_LENGTH('dbo.Users', 'Address') IS NULL
    ALTER TABLE dbo.Users ADD Address NVARCHAR(250) NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "U";

            string[] parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return parts[0].Substring(0, 1).ToUpperInvariant();

            return (parts[0].Substring(0, 1) + parts[^1].Substring(0, 1)).ToUpperInvariant();
        }

        private void GetStatusColors(string status, out string back, out string fore)
        {
            string normalized = status.Trim().ToUpperInvariant();

            if (normalized == "SUSPENDED" || normalized == "INACTIVE")
            {
                back = "#F7816D";
                fore = "#6E1B0F";
            }
            else if (normalized == "PENDING")
            {
                back = "#FFD6A5";
                fore = "#7A3E00";
            }
            else
            {
                back = "#B7EBD7";
                fore = "#1E4F41";
            }
        }

        private void LoadSchoolFilter()
        {
            cboSchoolFilter.Items.Clear();
            cboSchoolFilter.Items.Add("All Schools");

            foreach (string school in allRows
                .Select(x => x.School)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x))
            {
                cboSchoolFilter.Items.Add(school);
            }

            cboSchoolFilter.SelectedIndex = 0;
        }

        private void LoadGrid()
        {
            dgvMembers.Rows.Clear();

            if (cboSchoolFilter.Items.Count == 0)
                return;

            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string term = txtSearch.Text;
            bool useFilter = !string.IsNullOrWhiteSpace(term) && term != "Search by name or email...";

            IEnumerable<MemberItem> filtered = allRows;

            if (selectedSchool != "All Schools")
                filtered = filtered.Where(x => x.School.Equals(selectedSchool, StringComparison.OrdinalIgnoreCase));

            if (useFilter)
            {
                filtered = filtered.Where(x =>
                    x.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Email.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Phone.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Address.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.School.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            List<MemberItem> results = filtered.ToList();

            totalPages = Math.Max(1, (int)Math.Ceiling(results.Count / (double)PageSize));

            if (currentPage > totalPages)
                currentPage = totalPages;

            if (currentPage < 1)
                currentPage = 1;

            currentPageRows = results
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (var row in currentPageRows)
            {
                dgvMembers.Rows.Add(
                    row.Id,
                    $"{row.AvatarKind}|{row.Name}|{row.AvatarBack}|{row.AvatarFore}",
                    row.Email,
                    row.School,
                    row.Phone,
                    row.Address,
                    $"{row.Status}|{row.StatusBack}|{row.StatusFore}"
                );
            }

            dgvMembers.ClearSelection();

            lblTotalValue.Text = results.Count.ToString("N0");
            lblActiveValue.Text = results.Count(x => x.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)).ToString("N0");
            lblPendingValue.Text = results.Count(x => x.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)).ToString("N0");
            lblHealthValue.Text = results.Count == 0 ? "0%" : "100%";

            if (results.Count == 0)
            {
                lblFooter.Text = "Showing 0 of 0 members";
            }
            else
            {
                int start = ((currentPage - 1) * PageSize) + 1;
                int end = Math.Min(start + currentPageRows.Count - 1, results.Count);
                lblFooter.Text = $"Showing {start} to {end} of {results.Count} members";
            }

            UpdatePager();
        }

        private void GoToPage(Button button)
        {
            if (button.Tag == null) return;

            if (int.TryParse(button.Tag.ToString(), out int page))
            {
                currentPage = page;
                LoadGrid();
            }
        }

        private void UpdatePager()
        {
            if (btnPrev == null || btnNext == null || btnPage1 == null || btnPage2 == null || btnPage3 == null)
                return;

            totalPages = Math.Max(1, totalPages);

            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;

            StylePagerButton(btnPrev, false);
            StylePagerButton(btnNext, false);

            int page1 = Math.Max(1, currentPage - 1);

            if (currentPage >= totalPages && totalPages >= 3)
                page1 = totalPages - 2;

            int page2 = page1 + 1;
            int page3 = page1 + 2;

            SetPageButton(btnPage1, page1, page1 <= totalPages);
            SetPageButton(btnPage2, page2, page2 <= totalPages);
            SetPageButton(btnPage3, page3, page3 <= totalPages);
        }

        private void SetPageButton(Button button, int pageNumber, bool visible)
        {
            button.Visible = visible;
            button.Text = pageNumber.ToString();
            button.Tag = pageNumber;
            StylePagerButton(button, currentPage == pageNumber);
        }

        private void StylePagerButton(Button button, bool active)
        {
            if (active)
            {
                button.BackColor = PrimaryContainer;
                button.ForeColor = Color.White;
            }
            else
            {
                button.BackColor = SurfaceContainer;
                button.ForeColor = button.Enabled ? OnSurface : Color.FromArgb(150, 165, 170);
            }
        }



        private void dgvMembers_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string column = dgvMembers.Columns[e.ColumnIndex].Name;

            if (column == "Id")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string display = raw.Replace("-", "-\n");

                TextRenderer.DrawText(
                    e.Graphics,
                    display,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 14, e.CellBounds.Width - 20, e.CellBounds.Height - 20),
                    Primary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

                e.Handled = true;
            }
            else if (column == "Name")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');

                string kind = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : "";
                Color faceColor = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : SurfaceContainer;
                Color faceFore = parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3])
                    ? ColorTranslator.FromHtml(parts[3])
                    : Color.White;

                Rectangle avatar = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 24, 34, 34);

                using (SolidBrush brush = new SolidBrush(faceColor))
                    e.Graphics.FillEllipse(brush, avatar);

                string avatarText = kind == "IMG" ? "●" : kind;
                Font avatarFont = kind == "IMG"
                    ? new Font("Segoe UI Symbol", 10F, FontStyle.Bold)
                    : new Font("Segoe UI", 9F, FontStyle.Bold);

                TextRenderer.DrawText(
                    e.Graphics,
                    avatarText,
                    avatarFont,
                    avatar,
                    faceFore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(avatar.Right + 12, e.CellBounds.Y, e.CellBounds.Width - 56, e.CellBounds.Height),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak
                );

                e.Handled = true;
            }
            else if (column == "Email" || column == "School" || column == "Phone" || column == "Address")
            {
                e.PaintBackground(e.CellBounds, true);

                TextRenderer.DrawText(
                    e.Graphics,
                    e.FormattedValue?.ToString() ?? "",
                    new Font("Segoe UI", 10F),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis
                );

                e.Handled = true;
            }
            else if (column == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string text = parts[0];
                Color back = parts.Length > 1 ? ColorTranslator.FromHtml(parts[1]) : SurfaceContainer;
                Color fore = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : OnSurface;

                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 9F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 12,
                    e.CellBounds.Y + (e.CellBounds.Height - 28) / 2,
                    textSize.Width + 28,
                    28
                );

                using (SolidBrush brush = new SolidBrush(back))
                    e.Graphics.FillRectangle(brush, badge);

                Rectangle dotRect = new Rectangle(badge.X + 8, badge.Y + 10, 8, 8);
                using (SolidBrush dotBrush = new SolidBrush(fore))
                    e.Graphics.FillEllipse(dotBrush, dotRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 9F, FontStyle.Bold),
                    new Rectangle(dotRect.Right + 6, badge.Y, badge.Width - 20, badge.Height),
                    fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
        }

        private void MemberViewForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (canvas == null) return;

            int margin = 34;
            int gap = 22;
            int clientWidth = canvas.ClientSize.Width;
            int width = Math.Max(960, clientWidth - (margin * 2));

            int y = 0;

            headerPanel.Bounds = new Rectangle(0, y, clientWidth, 140);
            lblTitle.Location = new Point(margin, 28);
            lblSubTitle.Location = new Point(margin, 72);
            y += headerPanel.Height;

            statsPanel.Bounds = new Rectangle(0, y, clientWidth, 175);

            int statWidth = (width - (gap * 3)) / 4;
            int statHeight = 120;

            cardTotal.Bounds = new Rectangle(margin, 18, statWidth, statHeight);
            cardActive.Bounds = new Rectangle(cardTotal.Right + gap, 18, statWidth, statHeight);
            cardPending.Bounds = new Rectangle(cardActive.Right + gap, 18, statWidth, statHeight);
            cardHealth.Bounds = new Rectangle(cardPending.Right + gap, 18, statWidth, statHeight);
            y += statsPanel.Height;

            filterRow.Bounds = new Rectangle(0, y, clientWidth, 64);

            btnFilter.Location = new Point(clientWidth - btnFilter.Width - margin, 12);

            searchHost.Location = new Point(btnFilter.Left - searchHost.Width - 10, 12);
            searchHost.Size = new Size(280, 38);

            filterHost.Location = new Point(searchHost.Left - filterHost.Width - 10, 12);
            filterHost.Size = new Size(220, 38);

            cboSchoolFilter.Location = new Point(10, 6);
            cboSchoolFilter.Width = filterHost.Width - 20;

            lblSearchIcon.Location = new Point(10, 8);
            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = searchHost.Width - 54;

            y += filterRow.Height;

            int tableHeight = 460;
            tableCard.Bounds = new Rectangle(0, y, clientWidth, tableHeight);
            tableCard.Padding = new Padding(margin, 0, margin, 24);

            lblFooter.Location = new Point(24, 20);

            btnNext.Location = new Point(footerPanel.Width - 42, 13);
            btnPage3.Location = new Point(btnNext.Left - 42, 13);
            btnPage2.Location = new Point(btnPage3.Left - 42, 13);
            btnPage1.Location = new Point(btnPage2.Left - 42, 13);
            btnPrev.Location = new Point(btnPage1.Left - 42, 13);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 40);
        }
    }

    public class MemberItem
    {
        public string Id { get; set; }
        public string AvatarKind { get; set; }
        public string Name { get; set; }
        public string AvatarBack { get; set; }
        public string AvatarFore { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string School { get; set; }
        public string Status { get; set; }
        public string StatusBack { get; set; }
        public string StatusFore { get; set; }

        public MemberItem(
            string id,
            string avatarKind,
            string name,
            string avatarBack,
            string avatarFore,
            string email,
            string phone,
            string address,
            string school,
            string status,
            string statusBack,
            string statusFore)
        {
            Id = id;
            AvatarKind = avatarKind;
            Name = name;
            AvatarBack = avatarBack;
            AvatarFore = avatarFore;
            Email = email;
            Phone = phone;
            Address = address;
            School = school;
            Status = status;
            StatusBack = statusBack;
            StatusFore = statusFore;
        }
    }
}