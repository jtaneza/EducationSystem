using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace EducationSystem
{
    public partial class ArchiveForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#E2E9EC");
        private readonly Color Outline = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");

        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#376758");

        private readonly Color TertiaryText = ColorTranslator.FromHtml("#A03F30");

        private Panel canvas = null!;
        private Panel pageHeader = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;

        private Panel tableCard = null!;
        private Panel tableTopBar = null!;
        private Label lblTableTitle = null!;
        private Label badgeEntities = null!;
        private Label badgeDays = null!;

        private Panel filterHost = null!;
        private ComboBox cboInstitution = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;

        private DataGridView dgvArchive = null!;
        private Panel footerPanel = null!;
        private Label lblFooterInfo = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Label lblEllipsis = null!;
        private Button btnPageLast = null!;
        private Button btnNext = null!;

        private List<SuperArchiveItem> archiveItems = new List<SuperArchiveItem>();

        private int currentPage = 1;
        private const int PageSize = 10;
        private int totalPages = 1;
        private List<SuperArchiveItem> currentFilteredItems = new List<SuperArchiveItem>();

        public ArchiveForm()
        {
            InitializeComponent();
            BuildUI();
            LoadArchiveFromDatabase();
            LoadInstitutionFilter();
            LoadArchiveItems();
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
                AutoScroll = true,
                BackColor = Background,
                Padding = new Padding(0, 0, 0, 30)
            };

            BuildHeader();
            BuildArchiveTable();

            canvas.Controls.Add(tableCard);
            canvas.Controls.Add(pageHeader);

            Controls.Add(canvas);

            Resize += ArchiveForm_Resize;
            AdjustLayout();
        }

        private void BuildHeader()
        {
            pageHeader = new Panel
            {
                BackColor = Background,
                Height = 100
            };

            lblTitle = new Label
            {
                Text = "Super Admin Archive",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "View and manage all archived transactions across all institutions.",
                Font = new Font("Segoe UI", 11.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            pageHeader.Controls.Add(lblTitle);
            pageHeader.Controls.Add(lblSubTitle);
        }

        private void BuildArchiveTable()
        {
            tableCard = new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle
            };

            tableTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                BackColor = Color.FromArgb(248, 251, 252)
            };

            lblTableTitle = new Label
            {
                Text = "Archived Records",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            badgeEntities = CreateTopBadge("All Entities", AccentEmerald, Color.White);
            badgeDays = CreateTopBadge("Last 30 Days", SurfaceHigh, OnSurfaceVariant);

            filterHost = new Panel
            {
                BackColor = Color.FromArgb(241, 245, 246),
                Size = new Size(220, 38)
            };

            cboInstitution = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                BackColor = filterHost.BackColor,
                ForeColor = OnSurfaceVariant
            };
            cboInstitution.SelectedIndexChanged += (s, e) =>
            {
                currentPage = 1;
                LoadArchiveItems();
            };

            filterHost.Controls.Add(cboInstitution);

            searchHost = new Panel
            {
                BackColor = Color.FromArgb(241, 245, 246),
                Size = new Size(250, 38)
            };

            lblSearchIcon = new Label
            {
                Text = "⌕",
                Font = new Font("Segoe UI Symbol", 15F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = searchHost.BackColor,
                ForeColor = OnSurfaceVariant,
                Font = new Font("Segoe UI", 10F),
                Text = "Search archive..."
            };
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search archive...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search archive...";
                    txtSearch.ForeColor = OnSurfaceVariant;
                }
            };
            txtSearch.TextChanged += (s, e) =>
            {
                currentPage = 1;
                LoadArchiveItems();
            };

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            tableTopBar.Controls.Add(lblTableTitle);
            tableTopBar.Controls.Add(badgeEntities);
            tableTopBar.Controls.Add(badgeDays);
            tableTopBar.Controls.Add(filterHost);
            tableTopBar.Controls.Add(searchHost);

            dgvArchive = new DataGridView
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
                ColumnHeadersHeight = 48,
                GridColor = Outline,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };

            dgvArchive.RowTemplate.Height = 66;
            dgvArchive.DefaultCellStyle.Padding = new Padding(8, 8, 8, 8);

            dgvArchive.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(243, 247, 248);
            dgvArchive.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvArchive.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvArchive.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 247, 248);
            dgvArchive.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;
            dgvArchive.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvArchive.DefaultCellStyle.BackColor = Surface;
            dgvArchive.DefaultCellStyle.ForeColor = OnSurface;
            dgvArchive.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvArchive.DefaultCellStyle.SelectionBackColor = Color.FromArgb(244, 251, 249);
            dgvArchive.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvArchive.Columns.Add("ArchiveID", "ID");
            dgvArchive.Columns.Add("Institution", "Institution");
            dgvArchive.Columns.Add("Type", "Type");
            dgvArchive.Columns.Add("ArchivedDate", "Date Archived");
            dgvArchive.Columns.Add("Actions", "Actions");

            dgvArchive.Columns["ArchiveID"].FillWeight = 14;
            dgvArchive.Columns["Institution"].FillWeight = 37;
            dgvArchive.Columns["Type"].FillWeight = 15;
            dgvArchive.Columns["ArchivedDate"].FillWeight = 22;
            dgvArchive.Columns["Actions"].FillWeight = 12;

            dgvArchive.Columns["ArchiveID"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Type"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["ArchivedDate"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Actions"].HeaderCell.Style.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvArchive.Columns["ArchiveID"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Type"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["ArchivedDate"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Actions"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvArchive.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvArchive.CellPainting += dgvArchive_CellPainting;
            dgvArchive.CellClick += dgvArchive_CellClick;

            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = Surface
            };

            lblFooterInfo = new Label
            {
                Text = "Showing 1 to 5 of 12,482 entries",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            btnPrev = CreatePagerButton("‹", false, false);
            btnPage1 = CreatePagerButton("1", true, true);
            btnPage2 = CreatePagerButton("2", false, true);
            btnPage3 = CreatePagerButton("3", false, true);

            btnPrev.Click += (s, e) =>
            {
                if (currentPage <= 1) return;
                currentPage--;
                LoadArchiveItems();
            };

            btnPage1.Click += (s, e) => GoToArchivePage(btnPage1);
            btnPage2.Click += (s, e) => GoToArchivePage(btnPage2);
            btnPage3.Click += (s, e) => GoToArchivePage(btnPage3);

            lblEllipsis = new Label
            {
                Text = "...",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant
            };

            btnPageLast = CreatePagerButton("1", false, true);
            btnNext = CreatePagerButton("›", false, false);

            btnPageLast.Click += (s, e) =>
            {
                currentPage = totalPages;
                LoadArchiveItems();
            };

            btnNext.Click += (s, e) =>
            {
                if (currentPage >= totalPages) return;
                currentPage++;
                LoadArchiveItems();
            };

            footerPanel.Controls.Add(lblFooterInfo);
            footerPanel.Controls.Add(btnPrev);
            footerPanel.Controls.Add(btnPage1);
            footerPanel.Controls.Add(btnPage2);
            footerPanel.Controls.Add(btnPage3);
            footerPanel.Controls.Add(lblEllipsis);
            footerPanel.Controls.Add(btnPageLast);
            footerPanel.Controls.Add(btnNext);

            tableCard.Controls.Add(dgvArchive);
            tableCard.Controls.Add(footerPanel);
            tableCard.Controls.Add(tableTopBar);
        }

        private Label CreateTopBadge(string text, Color back, Color fore)
        {
            return new Label
            {
                Text = text.ToUpper(),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Padding = new Padding(10, 5, 10, 5),
                BackColor = back,
                ForeColor = fore
            };
        }

        private Button CreatePagerButton(string text, bool active, bool boxed)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 34,
                Width = boxed ? (text.Length > 2 ? 54 : 34) : 34
            };

            if (active)
            {
                btn.BackColor = AccentEmerald;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = OnSurface;
                btn.FlatAppearance.BorderSize = 0;
            }

            return btn;
        }


        private void LoadArchiveFromDatabase()
        {
            archiveItems.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureArchiveSchema(conn);

                LoadArchivedBooks(conn);
                LoadArchivedCategories(conn);
                LoadArchivedUsers(conn);
                LoadArchivedBorrowing(conn);
                LoadArchivedReturns(conn);
                LoadArchivedFines(conn);

                archiveItems = archiveItems
                    .OrderByDescending(x => x.ArchivedDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Archive records could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                archiveItems.Clear();
            }
        }

        private void LoadArchivedBooks(SqlConnection conn)
        {
            if (!TableExists(conn, "Books"))
                return;

            const string query = @"
SELECT
    b.BookID,
    ISNULL(NULLIF(b.BookTitle, ''), 'Untitled Book') AS ItemName,
    b.ClientID,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Institution') AS Institution,
    COALESCE(b.UpdatedAt, b.CreatedAt, SYSUTCDATETIME()) AS ArchivedAt
FROM dbo.Books b
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = b.ClientID
WHERE ISNULL(b.IsArchived, 0) = 1;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string recordId = SafeText(reader["BookID"], "BOOK");
                string institution = SafeText(reader["Institution"], "Unknown Institution");

                archiveItems.Add(new SuperArchiveItem
                {
                    ArchiveID = "#BK-" + recordId.Replace("BK-", ""),
                    RecordID = recordId,
                    Module = "Book",
                    ItemName = SafeText(reader["ItemName"], "Untitled Book"),
                    Institution = institution,
                    InstitutionCode = GetInstitutionCode(institution),
                    ClientID = reader["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ClientID"]),
                    ArchivedDate = reader["ArchivedAt"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ArchivedAt"]),
                    SourceTable = "Books"
                });
            }
        }

        private void LoadArchivedCategories(SqlConnection conn)
        {
            if (!TableExists(conn, "Categories"))
                return;

            const string query = @"
SELECT
    c.CategoryID,
    ISNULL(NULLIF(c.CategoryName, ''), 'Untitled Category') AS ItemName,
    c.ClientID,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Institution') AS Institution,
    COALESCE(c.UpdatedAt, c.CreatedAt, SYSUTCDATETIME()) AS ArchivedAt
FROM dbo.Categories c
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = c.ClientID
WHERE ISNULL(c.IsArchived, 0) = 1;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string recordId = SafeText(reader["CategoryID"], "CAT");
                string institution = SafeText(reader["Institution"], "Unknown Institution");

                archiveItems.Add(new SuperArchiveItem
                {
                    ArchiveID = "#CAT-" + recordId.Replace("CAT-", ""),
                    RecordID = recordId,
                    Module = "Category",
                    ItemName = SafeText(reader["ItemName"], "Untitled Category"),
                    Institution = institution,
                    InstitutionCode = GetInstitutionCode(institution),
                    ClientID = reader["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ClientID"]),
                    ArchivedDate = reader["ArchivedAt"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ArchivedAt"]),
                    SourceTable = "Categories"
                });
            }
        }

        private void LoadArchivedUsers(SqlConnection conn)
        {
            if (!TableExists(conn, "Users"))
                return;

            const string query = @"
SELECT
    u.UserID,
    ISNULL(NULLIF(u.FullName, ''), 'Unnamed User') AS ItemName,
    ISNULL(NULLIF(u.Role, ''), 'Member') AS Role,
    u.ClientID,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Institution') AS Institution,
    COALESCE(u.UpdatedAt, u.CreatedAt, SYSUTCDATETIME()) AS ArchivedAt
FROM dbo.Users u
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = u.ClientID
WHERE ISNULL(u.IsArchived, 0) = 1;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int userId = Convert.ToInt32(reader["UserID"]);
                string role = SafeText(reader["Role"], "Member");
                string institution = SafeText(reader["Institution"], "Unknown Institution");

                archiveItems.Add(new SuperArchiveItem
                {
                    ArchiveID = "#USR-" + userId.ToString("0000"),
                    RecordID = userId.ToString(),
                    Module = role.Contains("LIBRARIAN", StringComparison.OrdinalIgnoreCase) ? "Librarian" : "Member",
                    ItemName = SafeText(reader["ItemName"], "Unnamed User"),
                    Institution = institution,
                    InstitutionCode = GetInstitutionCode(institution),
                    ClientID = reader["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ClientID"]),
                    ArchivedDate = reader["ArchivedAt"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ArchivedAt"]),
                    SourceTable = "Users"
                });
            }
        }

        private void LoadArchivedBorrowing(SqlConnection conn)
        {
            if (!TableExists(conn, "BorrowingRecords"))
                return;

            const string query = @"
SELECT
    br.BorrowID,
    ISNULL(NULLIF(u.FullName, ''), 'Unknown Member') AS MemberName,
    ISNULL(NULLIF(br.BookTitle, ''), 'Unknown Book') AS BookTitle,
    br.ClientID,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Institution') AS Institution,
    COALESCE(br.UpdatedAt, br.CreatedAt, CAST(br.IssueDate AS DATETIME2), SYSUTCDATETIME()) AS ArchivedAt
FROM dbo.BorrowingRecords br
LEFT JOIN dbo.Users u ON u.UserID = br.MemberID
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = br.ClientID
WHERE ISNULL(br.IsArchived, 0) = 1;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["BorrowID"]);
                string institution = SafeText(reader["Institution"], "Unknown Institution");

                archiveItems.Add(new SuperArchiveItem
                {
                    ArchiveID = "#BR-" + id.ToString("0000"),
                    RecordID = id.ToString(),
                    Module = "Borrowing",
                    ItemName = SafeText(reader["MemberName"], "Unknown Member") + " - " + SafeText(reader["BookTitle"], "Unknown Book"),
                    Institution = institution,
                    InstitutionCode = GetInstitutionCode(institution),
                    ClientID = reader["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ClientID"]),
                    ArchivedDate = reader["ArchivedAt"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ArchivedAt"]),
                    SourceTable = "BorrowingRecords"
                });
            }
        }

        private void LoadArchivedReturns(SqlConnection conn)
        {
            if (!TableExists(conn, "ReturnRecords"))
                return;

            const string query = @"
SELECT
    rr.ReturnID,
    ISNULL(NULLIF(rr.MemberName, ''), 'Unknown Member') AS MemberName,
    ISNULL(NULLIF(rr.BookTitle, ''), 'Unknown Book') AS BookTitle,
    rr.ClientID,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Institution') AS Institution,
    COALESCE(rr.UpdatedAt, rr.CreatedAt, CAST(rr.ReturnDate AS DATETIME2), SYSUTCDATETIME()) AS ArchivedAt
FROM dbo.ReturnRecords rr
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = rr.ClientID
WHERE ISNULL(rr.IsArchived, 0) = 1;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ReturnID"]);
                string institution = SafeText(reader["Institution"], "Unknown Institution");

                archiveItems.Add(new SuperArchiveItem
                {
                    ArchiveID = "#RT-" + id.ToString("0000"),
                    RecordID = id.ToString(),
                    Module = "Return",
                    ItemName = SafeText(reader["MemberName"], "Unknown Member") + " - " + SafeText(reader["BookTitle"], "Unknown Book"),
                    Institution = institution,
                    InstitutionCode = GetInstitutionCode(institution),
                    ClientID = reader["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ClientID"]),
                    ArchivedDate = reader["ArchivedAt"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ArchivedAt"]),
                    SourceTable = "ReturnRecords"
                });
            }
        }

        private void LoadArchivedFines(SqlConnection conn)
        {
            if (!TableExists(conn, "FineRecords"))
                return;

            const string query = @"
SELECT
    fr.FineID,
    ISNULL(NULLIF(fr.MemberName, ''), 'Unknown Member') AS MemberName,
    ISNULL(NULLIF(fr.BookTitle, ''), 'Unknown Book') AS BookTitle,
    ISNULL(NULLIF(fr.Reason, ''), 'Fine') AS Reason,
    fr.ClientID,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown Institution') AS Institution,
    COALESCE(fr.UpdatedAt, fr.CreatedAt, SYSUTCDATETIME()) AS ArchivedAt
FROM dbo.FineRecords fr
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = fr.ClientID
WHERE ISNULL(fr.IsArchived, 0) = 1;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["FineID"]);
                string institution = SafeText(reader["Institution"], "Unknown Institution");

                archiveItems.Add(new SuperArchiveItem
                {
                    ArchiveID = "#FN-" + id.ToString("0000"),
                    RecordID = id.ToString(),
                    Module = "Fine",
                    ItemName = SafeText(reader["MemberName"], "Unknown Member") + " - " + SafeText(reader["BookTitle"], "Unknown Book"),
                    Institution = institution,
                    InstitutionCode = GetInstitutionCode(institution),
                    ClientID = reader["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ClientID"]),
                    ArchivedDate = reader["ArchivedAt"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ArchivedAt"]),
                    SourceTable = "FineRecords"
                });
            }
        }

        private void EnsureArchiveSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.ClientLibraries', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ClientLibraries', 'IsArchived') IS NULL
        ALTER TABLE dbo.ClientLibraries ADD IsArchived BIT NOT NULL CONSTRAINT DF_ClientLibraries_SAArchive_IsArchived DEFAULT 0;
END;

IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Books', 'ClientID') IS NULL
        ALTER TABLE dbo.Books ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Books', 'IsArchived') IS NULL
        ALTER TABLE dbo.Books ADD IsArchived BIT NOT NULL CONSTRAINT DF_Books_SAArchive_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.Books', 'UpdatedAt') IS NULL
        ALTER TABLE dbo.Books ADD UpdatedAt DATETIME2 NULL;
END;

IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Categories', 'ClientID') IS NULL
        ALTER TABLE dbo.Categories ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Categories', 'IsArchived') IS NULL
        ALTER TABLE dbo.Categories ADD IsArchived BIT NOT NULL CONSTRAINT DF_Categories_SAArchive_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.Categories', 'UpdatedAt') IS NULL
        ALTER TABLE dbo.Categories ADD UpdatedAt DATETIME2 NULL;
END;

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
        ALTER TABLE dbo.Users ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.Users', 'IsArchived') IS NULL
        ALTER TABLE dbo.Users ADD IsArchived BIT NOT NULL CONSTRAINT DF_Users_SAArchive_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.Users', 'UpdatedAt') IS NULL
        ALTER TABLE dbo.Users ADD UpdatedAt DATETIME2 NULL;
END;

IF OBJECT_ID('dbo.BorrowingRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.BorrowingRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.BorrowingRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_BorrowingRecords_SAArchive_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.BorrowingRecords', 'UpdatedAt') IS NULL
        ALTER TABLE dbo.BorrowingRecords ADD UpdatedAt DATETIME2 NULL;
END;

IF OBJECT_ID('dbo.ReturnRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ReturnRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.ReturnRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_ReturnRecords_SAArchive_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.ReturnRecords', 'UpdatedAt') IS NULL
        ALTER TABLE dbo.ReturnRecords ADD UpdatedAt DATETIME2 NULL;
END;

IF OBJECT_ID('dbo.FineRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.FineRecords', 'ClientID') IS NULL
        ALTER TABLE dbo.FineRecords ADD ClientID INT NULL;

    IF COL_LENGTH('dbo.FineRecords', 'IsArchived') IS NULL
        ALTER TABLE dbo.FineRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_FineRecords_SAArchive_IsArchived DEFAULT 0;

    IF COL_LENGTH('dbo.FineRecords', 'UpdatedAt') IS NULL
        ALTER TABLE dbo.FineRecords ADD UpdatedAt DATETIME2 NULL;
END;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private bool TableExists(SqlConnection conn, string tableName)
        {
            using SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName;",
                conn);

            cmd.Parameters.AddWithValue("@TableName", tableName);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private string SafeText(object value, string fallback = "")
        {
            if (value == null || value == DBNull.Value)
                return fallback;

            string text = Convert.ToString(value) ?? "";
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        }

        private string GetInstitutionCode(string institution)
        {
            string[] words = institution.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return "NA";

            if (words.Length == 1)
                return words[0].Substring(0, Math.Min(2, words[0].Length)).ToUpperInvariant();

            return (words[0][0].ToString() + words[1][0]).ToUpperInvariant();
        }


        private void LoadInstitutionFilter()
        {
            string previous = cboInstitution.SelectedItem?.ToString() ?? "All Institutions";

            cboInstitution.Items.Clear();
            cboInstitution.Items.Add("All Institutions");

            foreach (string school in archiveItems
                         .Select(x => x.Institution)
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .OrderBy(x => x))
            {
                cboInstitution.Items.Add(school);
            }

            int index = cboInstitution.Items.IndexOf(previous);
            cboInstitution.SelectedIndex = index >= 0 ? index : 0;
        }


        private void LoadArchiveItems()
        {
            dgvArchive.Rows.Clear();

            string selectedInstitution = cboInstitution.SelectedItem?.ToString() ?? "All Institutions";
            string searchText = txtSearch.Text.Trim();

            bool useSearch = !string.IsNullOrWhiteSpace(searchText) && searchText != "Search archive...";

            IEnumerable<SuperArchiveItem> filteredItems = archiveItems.AsEnumerable();

            if (selectedInstitution != "All Institutions")
            {
                filteredItems = filteredItems.Where(x => x.Institution.Equals(selectedInstitution, StringComparison.OrdinalIgnoreCase));
            }

            if (useSearch)
            {
                filteredItems = filteredItems.Where(x =>
                    x.ArchiveID.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.ItemName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Institution.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Module.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.RecordID.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            currentFilteredItems = filteredItems.ToList();

            totalPages = Math.Max(1, (int)Math.Ceiling(currentFilteredItems.Count / (double)PageSize));

            if (currentPage > totalPages)
                currentPage = totalPages;

            if (currentPage < 1)
                currentPage = 1;

            List<SuperArchiveItem> pageItems = currentFilteredItems
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (SuperArchiveItem item in pageItems)
            {
                dgvArchive.Rows.Add(
                    item.ArchiveID,
                    $"{item.InstitutionCode}|{item.Institution}|{GetInstitutionBack(item.Module)}|{GetInstitutionFore(item.Module)}",
                    item.Module,
                    item.ArchivedDate.ToString("MMM dd, yyyy • HH:mm"),
                    "Restore|Delete");
            }

            dgvArchive.ClearSelection();

            if (currentFilteredItems.Count == 0)
            {
                lblFooterInfo.Text = "Showing 0 of 0 entries";
            }
            else
            {
                int start = ((currentPage - 1) * PageSize) + 1;
                int end = Math.Min(start + pageItems.Count - 1, currentFilteredItems.Count);
                lblFooterInfo.Text = $"Showing {start} to {end} of {currentFilteredItems.Count:N0} entries";
            }

            UpdatePager();
        }

        private void GoToArchivePage(Button button)
        {
            if (button.Tag == null)
                return;

            if (int.TryParse(button.Tag.ToString(), out int page))
            {
                currentPage = page;
                LoadArchiveItems();
            }
        }

        private void UpdatePager()
        {
            totalPages = Math.Max(1, totalPages);

            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;

            btnPrev.ForeColor = btnPrev.Enabled ? OnSurface : Color.FromArgb(170, 185, 190);
            btnNext.ForeColor = btnNext.Enabled ? OnSurface : Color.FromArgb(170, 185, 190);

            int page1 = Math.Max(1, currentPage - 1);

            if (currentPage >= totalPages && totalPages >= 3)
                page1 = totalPages - 2;
            else if (currentPage == totalPages && totalPages == 2)
                page1 = 1;

            int page2 = page1 + 1;
            int page3 = page1 + 2;

            SetPagerButton(btnPage1, page1, page1 <= totalPages);
            SetPagerButton(btnPage2, page2, page2 <= totalPages);
            SetPagerButton(btnPage3, page3, page3 <= totalPages);

            bool showLast = totalPages > 3 && page3 < totalPages;
            lblEllipsis.Visible = showLast;
            btnPageLast.Visible = showLast;

            btnPageLast.Text = totalPages.ToString();
            btnPageLast.Tag = totalPages;
            StylePagerButton(btnPageLast, currentPage == totalPages);

            AdjustLayout();
        }

        private void SetPagerButton(Button button, int pageNumber, bool visible)
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
                button.BackColor = AccentEmerald;
                button.ForeColor = Color.White;
            }
            else
            {
                button.BackColor = Color.Transparent;
                button.ForeColor = OnSurface;
            }
        }

        private string GetInstitutionBack(string module)
        {
            return module switch
            {
                "Borrowing" => "#B7EBD7",
                "Return" => "#B7EBD7",
                "Fine" => "#FDE7E2",
                "Book" => "#D9F4EC",
                "Category" => "#D9F4EC",
                "Librarian" => "#E2E9EC",
                "Member" => "#FDE7E2",
                _ => "#E2E9EC"
            };
        }

        private string GetInstitutionFore(string module)
        {
            return module switch
            {
                "Borrowing" => "#376758",
                "Return" => "#376758",
                "Fine" => "#A03F30",
                "Book" => "#006B55",
                "Category" => "#006B55",
                "Librarian" => "#3C4A44",
                "Member" => "#A03F30",
                _ => "#3C4A44"
            };
        }

        private void ArchiveForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int width = Math.Max(980, canvas.ClientSize.Width - (margin * 2));

            pageHeader.SetBounds(0, 0, canvas.ClientSize.Width, 100);

            lblTitle.Location = new Point(margin, 28);
            lblSubTitle.Location = new Point(margin, 72);

            tableCard.SetBounds(margin, pageHeader.Bottom + 8, width, 584);

            lblTableTitle.Location = new Point(22, 18);
            badgeEntities.Location = new Point(lblTableTitle.Right + 18, 17);
            badgeDays.Location = new Point(badgeEntities.Right + 8, 17);

            searchHost.Location = new Point(tableTopBar.Width - searchHost.Width - 18, 14);
            lblSearchIcon.Location = new Point(10, 7);
            txtSearch.Location = new Point(38, 10);
            txtSearch.Width = searchHost.Width - 48;

            filterHost.Location = new Point(searchHost.Left - filterHost.Width - 10, 14);
            cboInstitution.Location = new Point(10, 6);
            cboInstitution.Width = filterHost.Width - 20;

            lblFooterInfo.Location = new Point(22, 20);

            int pagerRight = footerPanel.Width - 24;

            btnNext.Location = new Point(pagerRight - btnNext.Width, 11);
            pagerRight = btnNext.Left - 8;

            if (btnPageLast.Visible)
            {
                btnPageLast.Width = Math.Max(34, TextRenderer.MeasureText(btnPageLast.Text, btnPageLast.Font).Width + 18);
                btnPageLast.Location = new Point(pagerRight - btnPageLast.Width, 11);
                pagerRight = btnPageLast.Left - 8;

                lblEllipsis.Location = new Point(pagerRight - lblEllipsis.Width, 18);
                pagerRight = lblEllipsis.Left - 8;
            }

            if (btnPage3.Visible)
            {
                btnPage3.Location = new Point(pagerRight - btnPage3.Width, 11);
                pagerRight = btnPage3.Left - 8;
            }

            if (btnPage2.Visible)
            {
                btnPage2.Location = new Point(pagerRight - btnPage2.Width, 11);
                pagerRight = btnPage2.Left - 8;
            }

            if (btnPage1.Visible)
            {
                btnPage1.Location = new Point(pagerRight - btnPage1.Width, 11);
                pagerRight = btnPage1.Left - 8;
            }

            btnPrev.Location = new Point(pagerRight - btnPrev.Width, 11);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 30);
        }

        private void dgvArchive_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvArchive.Columns[e.ColumnIndex].Name;

            if (col == "Institution")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');

                string code = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : "";
                Color back = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : SurfaceHigh;
                Color fore = parts.Length > 3 ? ColorTranslator.FromHtml(parts[3]) : OnSurface;

                Rectangle badge = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 16, 24, 24);
                using (SolidBrush b = new SolidBrush(back))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    code,
                    new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    badge,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 46, e.CellBounds.Y + 8, e.CellBounds.Width - 54, e.CellBounds.Height - 14),
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (col == "Type")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                string icon = text switch
                {
                    "Borrowing" => "⇄",
                    "Return" => "↩",
                    "Fine" => "₱",
                    "Book" => "📘",
                    "Category" => "▦",
                    "Librarian" => "👤",
                    "Member" => "👤",
                    _ => "•"
                };

                Color back = (text == "Book" || text == "Category") ? SecondaryContainer : SurfaceHigh;
                Color fore = (text == "Book" || text == "Category") ? SecondaryText : OnSurfaceVariant;

                Size textSize = TextRenderer.MeasureText(text.ToUpper(), new Font("Segoe UI", 8F, FontStyle.Bold));
                int badgeWidth = textSize.Width + 34;

                Rectangle badge = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - badgeWidth) / 2,
                    e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                    badgeWidth,
                    24);

                using (SolidBrush b = new SolidBrush(back))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    icon,
                    new Font("Segoe UI Emoji", 9F),
                    new Rectangle(badge.X + 6, badge.Y, 14, badge.Height),
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    text.ToUpper(),
                    new Font("Segoe UI", 8F, FontStyle.Bold),
                    new Rectangle(badge.X + 18, badge.Y, badge.Width - 20, badge.Height),
                    fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                int iconTop = e.CellBounds.Y + (e.CellBounds.Height - 28) / 2;
                int totalWidth = 28 + 8 + 28;
                int startX = e.CellBounds.X + (e.CellBounds.Width - totalWidth) / 2;

                Rectangle restoreRect = new Rectangle(startX, iconTop, 28, 28);
                Rectangle deleteRect = new Rectangle(startX + 36, iconTop, 28, 28);

                using (SolidBrush b = new SolidBrush(Color.FromArgb(20, 0, 184, 148)))
                    e.Graphics.FillRectangle(b, restoreRect);

                using (SolidBrush b = new SolidBrush(Color.FromArgb(20, 247, 129, 109)))
                    e.Graphics.FillRectangle(b, deleteRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    "↺",
                    new Font("Segoe UI Symbol", 11F, FontStyle.Bold),
                    restoreRect,
                    AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    "✕",
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    deleteRect,
                    TertiaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }


        private void dgvArchive_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvArchive.Columns[e.ColumnIndex].Name != "Actions") return;

            Rectangle cellRect = dgvArchive.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            Point clientPoint = dgvArchive.PointToClient(Cursor.Position);

            int iconTop = cellRect.Y + (cellRect.Height - 28) / 2;
            int totalWidth = 28 + 8 + 28;
            int startX = cellRect.X + (cellRect.Width - totalWidth) / 2;

            Rectangle restoreRect = new Rectangle(startX, iconTop, 28, 28);
            Rectangle deleteRect = new Rectangle(startX + 36, iconTop, 28, 28);

            string archiveId = dgvArchive.Rows[e.RowIndex].Cells["ArchiveID"].Value?.ToString() ?? "";
            SuperArchiveItem? item = archiveItems.FirstOrDefault(x => x.ArchiveID == archiveId);
            if (item == null) return;

            if (restoreRect.Contains(clientPoint))
            {
                RestoreArchiveItem(item);
                return;
            }

            if (deleteRect.Contains(clientPoint))
            {
                DeleteArchiveItem(item);
            }
        }

        private void RestoreArchiveItem(SuperArchiveItem item)
        {
            DialogResult result = MessageBox.Show(
                $"Restore {item.ArchiveID}?",
                "Restore Archived Record",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string keyColumn = GetKeyColumn(item.SourceTable);

                string query = $@"
UPDATE dbo.{item.SourceTable}
SET IsArchived = 0,
    UpdatedAt = SYSUTCDATETIME()
WHERE {keyColumn} = @RecordID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                AddRecordIdParameter(cmd, item);
                cmd.ExecuteNonQuery();

                LoadArchiveFromDatabase();
                LoadInstitutionFilter();
                LoadArchiveItems();

                MessageBox.Show(
                    "Record restored successfully.",
                    "Restored",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Archived record could not be restored.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DeleteArchiveItem(SuperArchiveItem item)
        {
            DialogResult result = MessageBox.Show(
                $"Permanently delete {item.ArchiveID}?\n\nThis cannot be undone.",
                "Confirm Permanent Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string keyColumn = GetKeyColumn(item.SourceTable);

                string query = $@"
DELETE FROM dbo.{item.SourceTable}
WHERE {keyColumn} = @RecordID
  AND IsArchived = 1;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                AddRecordIdParameter(cmd, item);
                cmd.ExecuteNonQuery();

                LoadArchiveFromDatabase();
                LoadInstitutionFilter();
                LoadArchiveItems();

                MessageBox.Show(
                    "Archived record permanently deleted.",
                    "Deleted",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Archived record could not be deleted.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string GetKeyColumn(string sourceTable)
        {
            return sourceTable switch
            {
                "Books" => "BookID",
                "Categories" => "CategoryID",
                "Users" => "UserID",
                "BorrowingRecords" => "BorrowID",
                "ReturnRecords" => "ReturnID",
                "FineRecords" => "FineID",
                _ => throw new InvalidOperationException("Unknown archive table: " + sourceTable)
            };
        }

        private void AddRecordIdParameter(SqlCommand cmd, SuperArchiveItem item)
        {
            if (item.SourceTable == "Users" ||
                item.SourceTable == "BorrowingRecords" ||
                item.SourceTable == "ReturnRecords" ||
                item.SourceTable == "FineRecords")
            {
                cmd.Parameters.AddWithValue("@RecordID", Convert.ToInt32(item.RecordID));
            }
            else
            {
                cmd.Parameters.AddWithValue("@RecordID", item.RecordID);
            }
        }
    }

    public class SuperArchiveItem
    {
        public string ArchiveID { get; set; } = "";
        public string RecordID { get; set; } = "";
        public string Module { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string Institution { get; set; } = "";
        public string InstitutionCode { get; set; } = "";
        public int ClientID { get; set; }
        public DateTime ArchivedDate { get; set; }
        public string SourceTable { get; set; } = "";
    }
}