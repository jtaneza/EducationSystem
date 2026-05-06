using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class ClientArchiveForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#BA1A1A");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Panel tableCard = null!;
        private Label lblTableTitle = null!;
        private Panel searchPanel = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;
        private DataGridView dgvArchive = null!;
        private Label lblFooter = null!;
        private Panel pagerPanel = null!;

        private const string SearchPlaceholder = "Search archive by name or ID...";

        private sealed class ArchiveRow
        {
            public string SourceTable { get; set; } = "";
            public string PrimaryKeyColumn { get; set; } = "";
            public string PrimaryKeyValue { get; set; } = "";
            public string ItemName { get; set; } = "";
            public string ItemIdText { get; set; } = "";
            public string RecordType { get; set; } = "";
            public DateTime ArchivedAt { get; set; }
            public string Reason { get; set; } = "";
            public string ArchivedBy { get; set; } = "";
        }

        public ClientArchiveForm()
        {
            InitializeComponent();
            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BuildUI();
            Load += (s, e) => { LoadArchiveData(); AdjustLayout(); ClearGridSelection(); };
            Resize += (s, e) => AdjustLayout();
        }

        private void BuildUI()
        {
            canvas = new Panel { Dock = DockStyle.Fill, BackColor = FormBack, AutoScroll = true };
            Controls.Add(canvas);

            lblTitle = new Label
            {
                Text = "Archive Management",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubtitle = new Label
            {
                Text = "Review and manage removed library data records.",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubtitle);
            BuildTableCard();
        }

        private void BuildTableCard()
        {
            tableCard = CreateRoundedCard();
            canvas.Controls.Add(tableCard);

            lblTableTitle = new Label
            {
                Text = "Repository Explorer",
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            searchPanel = new Panel { BackColor = CardBack };
            searchPanel.Paint += RoundedPanelPaint;

            Label searchIcon = new Label
            {
                Text = "⌕",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 14F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            txtSearch = new TextBox
            {
                Text = SearchPlaceholder,
                BorderStyle = BorderStyle.None,
                BackColor = CardBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 10.5F)
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == SearchPlaceholder)
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = PrimaryText;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = SearchPlaceholder;
                    txtSearch.ForeColor = SecondaryText;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                if (!txtSearch.Focused && txtSearch.Text == SearchPlaceholder)
                    return;

                LoadArchiveData(txtSearch.Text);
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(txtSearch);

            btnFilter = new Button
            {
                Text = "≡",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderSize = 0;

            dgvArchive = new DataGridView
            {
                BackgroundColor = CardBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 62,
                RowTemplate = { Height = 86 },
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical,
                ShowCellToolTips = false
            };

            dgvArchive.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvArchive.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvArchive.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvArchive.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvArchive.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvArchive.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgvArchive.DefaultCellStyle.BackColor = CardBack;
            dgvArchive.DefaultCellStyle.ForeColor = PrimaryText;
            dgvArchive.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            dgvArchive.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvArchive.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvArchive.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            dgvArchive.RowsDefaultCellStyle.BackColor = CardBack;
            dgvArchive.RowsDefaultCellStyle.SelectionBackColor = CardBack;
            dgvArchive.RowsDefaultCellStyle.SelectionForeColor = PrimaryText;

            dgvArchive.Columns.Add("Item", "ITEM NAME / ID");
            dgvArchive.Columns.Add("Role", "TYPE");
            dgvArchive.Columns.Add("DateSaved", "DATE\nSAVED");
            dgvArchive.Columns.Add("Reason", "REASON FOR\nARCHIVING");
            dgvArchive.Columns.Add("ArchivedBy", "ARCHIVED BY");
            dgvArchive.Columns.Add("Actions", "ACTIONS");
            ApplyColumnStyle();

            dgvArchive.CellPainting += DgvArchive_CellPainting;
            dgvArchive.CellClick += DgvArchive_CellClick;
            dgvArchive.CellMouseClick += DgvArchive_CellMouseClick;
            dgvArchive.MouseClick += DgvArchive_MouseClick;
            dgvArchive.MouseMove += DgvArchive_MouseMove;
            dgvArchive.SelectionChanged += (s, e) => ClearGridSelection();

            lblFooter = new Label
            {
                Text = "Showing 0 archived records",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            pagerPanel = new Panel { BackColor = Color.Transparent, Size = new Size(220, 34) };
            BuildPager();

            tableCard.Controls.Add(lblTableTitle);
            tableCard.Controls.Add(searchPanel);
            tableCard.Controls.Add(btnFilter);
            tableCard.Controls.Add(dgvArchive);
            tableCard.Controls.Add(lblFooter);
            tableCard.Controls.Add(pagerPanel);
        }

        private void LoadArchiveData(string search = "")
        {
            dgvArchive.Rows.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureArchiveSchema(conn);

                List<ArchiveRow> rows = new List<ArchiveRow>();
                string keyword = search.Trim();

                LoadArchivedUsers(conn, rows, keyword);
                LoadArchivedBooks(conn, rows, keyword);
                LoadArchivedCategories(conn, rows, keyword);
                LoadArchivedBorrowingRecords(conn, rows, keyword);
                LoadArchivedReturnRecords(conn, rows, keyword);
                LoadArchivedFineRecords(conn, rows, keyword);

                rows.Sort((a, b) => b.ArchivedAt.CompareTo(a.ArchivedAt));

                foreach (ArchiveRow row in rows)
                {
                    int rowIndex = dgvArchive.Rows.Add(
                        $"{row.ItemName}\nID: {row.ItemIdText}",
                        row.RecordType.ToUpperInvariant(),
                        row.ArchivedAt.ToString("MMM dd,\nyyyy"),
                        row.Reason,
                        row.ArchivedBy,
                        ""
                    );

                    dgvArchive.Rows[rowIndex].Tag = row;
                }

                lblFooter.Text = $"Showing {dgvArchive.Rows.Count} archived records";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load archived records.\n\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ClearGridSelection();
        }

        private bool MatchesSearch(string search, params string[] values)
        {
            if (string.IsNullOrWhiteSpace(search)) return true;
            foreach (string value in values)
                if ((value ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }

        private void LoadArchivedUsers(SqlConnection conn, List<ArchiveRow> rows, string search)
        {
            if (!TableExists(conn, "Users") || !ColumnExists(conn, "Users", "IsArchived")) return;
            const string query = @"
SELECT UserID, FullName, Email, Role, CreatedAt, ArchivedByUserID, ArchivedByRole, ArchivedAt
FROM dbo.Users
WHERE ClientID = @ClientID AND IsArchived = 1
ORDER BY ArchivedAt DESC, UserID DESC;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["UserID"]);
                string name = Convert.ToString(reader["FullName"]) ?? "";
                string email = Convert.ToString(reader["Email"]) ?? "";
                string role = Convert.ToString(reader["Role"]) ?? "";
                DateTime archivedAt = reader["ArchivedAt"] == DBNull.Value ? ReadDate(reader, "CreatedAt") : Convert.ToDateTime(reader["ArchivedAt"]);
                string archivedById = reader["ArchivedByUserID"] == DBNull.Value ? "Unknown" : "USR-" + Convert.ToInt32(reader["ArchivedByUserID"]).ToString("000");
                string archivedByRole = Convert.ToString(reader["ArchivedByRole"]) ?? "Unknown";
                if (!MatchesSearch(search, id.ToString(), name, email, role, archivedById, archivedByRole, "User")) continue;
                rows.Add(new ArchiveRow { SourceTable = "Users", PrimaryKeyColumn = "UserID", PrimaryKeyValue = id.ToString(), ItemName = name, ItemIdText = $"USR-{id:000}", RecordType = role, ArchivedAt = archivedAt, Reason = "User archived", ArchivedBy = $"{archivedById}\n{archivedByRole}" });
            }
        }

        private void LoadArchivedBooks(SqlConnection conn, List<ArchiveRow> rows, string search)
        {
            if (!TableExists(conn, "Books") || !ColumnExists(conn, "Books", "IsArchived")) return;
            const string query = @"
SELECT BookID, BookTitle, Author, Category, Genre, GroupName, CreatedAt, UpdatedAt, RecordedBy
FROM dbo.Books
WHERE ClientID = @ClientID
  AND IsArchived = 1
ORDER BY COALESCE(UpdatedAt, CreatedAt) DESC, BookID DESC;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string id = Convert.ToString(reader["BookID"]) ?? "";
                string title = Convert.ToString(reader["BookTitle"]) ?? "";
                string author = Convert.ToString(reader["Author"]) ?? "";
                string category = Convert.ToString(reader["Category"]) ?? "";
                string genre = Convert.ToString(reader["Genre"]) ?? "";
                string group = Convert.ToString(reader["GroupName"]) ?? "";
                string recordedBy = Convert.ToString(reader["RecordedBy"]) ?? "Unknown";
                if (!MatchesSearch(search, id, title, author, category, genre, group, recordedBy, "Book")) continue;
                rows.Add(new ArchiveRow { SourceTable = "Books", PrimaryKeyColumn = "BookID", PrimaryKeyValue = id, ItemName = title, ItemIdText = id, RecordType = "Book", ArchivedAt = ReadDate(reader, "UpdatedAt", "CreatedAt"), Reason = "Book archived", ArchivedBy = recordedBy });
            }
        }

        private void LoadArchivedCategories(SqlConnection conn, List<ArchiveRow> rows, string search)
        {
            if (TableExists(conn, "Categories") && ColumnExists(conn, "Categories", "IsArchived"))
            {
                const string query = @"
SELECT CategoryID, CategoryName, Genre, GroupName, AddedBy, CreatedAt, UpdatedAt
FROM dbo.Categories
WHERE ClientID = @ClientID
  AND IsArchived = 1
ORDER BY COALESCE(UpdatedAt, CreatedAt) DESC, CategoryID DESC;";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string id = Convert.ToString(reader["CategoryID"]) ?? "";
                    string name = Convert.ToString(reader["CategoryName"]) ?? "";
                    string genre = Convert.ToString(reader["Genre"]) ?? "";
                    string group = Convert.ToString(reader["GroupName"]) ?? "";
                    string addedBy = Convert.ToString(reader["AddedBy"]) ?? "Unknown";
                    if (!MatchesSearch(search, id, name, genre, group, addedBy, "Category")) continue;
                    rows.Add(new ArchiveRow { SourceTable = "Categories", PrimaryKeyColumn = "CategoryID", PrimaryKeyValue = id, ItemName = name, ItemIdText = id, RecordType = "Category", ArchivedAt = ReadDate(reader, "UpdatedAt", "CreatedAt"), Reason = "Category archived", ArchivedBy = addedBy });
                }
            }
        }

        private void LoadArchivedBorrowingRecords(SqlConnection conn, List<ArchiveRow> rows, string search)
        {
            if (!TableExists(conn, "BorrowingRecords") || !ColumnExists(conn, "BorrowingRecords", "IsArchived")) return;
            const string query = @"
SELECT b.BorrowID, b.BookID, b.BookTitle, b.Status, b.CreatedAt, b.MemberID, u.FullName
FROM dbo.BorrowingRecords b
LEFT JOIN dbo.Users u ON b.MemberID = u.UserID AND u.ClientID = @ClientID
WHERE b.ClientID = @ClientID
  AND b.IsArchived = 1
ORDER BY b.BorrowID DESC;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["BorrowID"]);
                string memberName = Convert.ToString(reader["FullName"]) ?? "Unknown Member";
                string bookId = Convert.ToString(reader["BookID"]) ?? "";
                string bookTitle = Convert.ToString(reader["BookTitle"]) ?? "";
                string status = Convert.ToString(reader["Status"]) ?? "";
                if (!MatchesSearch(search, id.ToString(), memberName, bookId, bookTitle, status, "Borrow")) continue;
                rows.Add(new ArchiveRow { SourceTable = "BorrowingRecords", PrimaryKeyColumn = "BorrowID", PrimaryKeyValue = id.ToString(), ItemName = $"{memberName} - {bookTitle}", ItemIdText = $"BRW-{id:0000}", RecordType = "Borrow", ArchivedAt = ReadDate(reader, "CreatedAt"), Reason = "Borrowing record archived", ArchivedBy = "System" });
            }
        }

        private void LoadArchivedReturnRecords(SqlConnection conn, List<ArchiveRow> rows, string search)
        {
            if (!TableExists(conn, "ReturnRecords") || !ColumnExists(conn, "ReturnRecords", "IsArchived")) return;
            const string query = @"
SELECT ReturnID, BorrowID, MemberName, BookTitle, ReturnDate, BookCondition, ReturnStatus, CreatedAt, UpdatedAt
FROM dbo.ReturnRecords
WHERE ClientID = @ClientID
  AND IsArchived = 1
ORDER BY COALESCE(UpdatedAt, CreatedAt) DESC, ReturnID DESC;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ReturnID"]);
                string memberName = Convert.ToString(reader["MemberName"]) ?? "";
                string bookTitle = Convert.ToString(reader["BookTitle"]) ?? "";
                string condition = Convert.ToString(reader["BookCondition"]) ?? "";
                string status = Convert.ToString(reader["ReturnStatus"]) ?? "";
                if (!MatchesSearch(search, id.ToString(), memberName, bookTitle, condition, status, "Return")) continue;
                rows.Add(new ArchiveRow { SourceTable = "ReturnRecords", PrimaryKeyColumn = "ReturnID", PrimaryKeyValue = id.ToString(), ItemName = $"{memberName} - {bookTitle}", ItemIdText = $"RET-{id:0000}", RecordType = "Return", ArchivedAt = ReadDate(reader, "UpdatedAt", "CreatedAt", "ReturnDate"), Reason = $"Return archived ({condition}/{status})", ArchivedBy = "System" });
            }
        }

        private void LoadArchivedFineRecords(SqlConnection conn, List<ArchiveRow> rows, string search)
        {
            if (!TableExists(conn, "FineRecords") || !ColumnExists(conn, "FineRecords", "IsArchived")) return;
            const string query = @"
SELECT FineID, MemberName, BookID, BookTitle, Reason, Amount, Status, RecordedBy, CreatedAt, UpdatedAt
FROM dbo.FineRecords
WHERE ClientID = @ClientID
  AND IsArchived = 1
ORDER BY COALESCE(UpdatedAt, CreatedAt) DESC, FineID DESC;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["FineID"]);
                string memberName = Convert.ToString(reader["MemberName"]) ?? "";
                string bookId = Convert.ToString(reader["BookID"]) ?? "";
                string bookTitle = Convert.ToString(reader["BookTitle"]) ?? "";
                string reason = Convert.ToString(reader["Reason"]) ?? "";
                string status = Convert.ToString(reader["Status"]) ?? "";
                string recordedBy = Convert.ToString(reader["RecordedBy"]) ?? "Unknown";
                if (!MatchesSearch(search, id.ToString(), memberName, bookId, bookTitle, reason, status, recordedBy, "Fine")) continue;
                rows.Add(new ArchiveRow { SourceTable = "FineRecords", PrimaryKeyColumn = "FineID", PrimaryKeyValue = id.ToString(), ItemName = $"{memberName} - {bookTitle}", ItemIdText = $"FN-{id:0000}", RecordType = "Fine", ArchivedAt = ReadDate(reader, "UpdatedAt", "CreatedAt"), Reason = $"Fine archived ({reason}/{status})", ArchivedBy = recordedBy });
            }
        }

        private void RestoreArchiveRow(ArchiveRow row)
        {
            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to restore this archived record?\n\n{row.RecordType}: {row.ItemName}\nID: {row.ItemIdText}",
                "Confirm Restore",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                if (!TableExists(conn, row.SourceTable) || !ColumnExists(conn, row.SourceTable, "IsArchived"))
                {
                    MessageBox.Show(
                        "This archive source cannot be restored automatically.",
                        "Restore Not Available",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                using SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    string query = BuildRestoreQuery(row);
                    using SqlCommand cmd = new SqlCommand(query, conn, tx);
                    AddIdParameter(cmd, "@ID", row.PrimaryKeyValue);
                    cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                    cmd.ExecuteNonQuery();

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }

                MessageBox.Show(
                    "Archived record restored successfully.",
                    "Restore Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadArchiveData(GetSearchText());
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

        private string BuildRestoreQuery(ArchiveRow row)
        {
            string table = row.SourceTable;
            string key = row.PrimaryKeyColumn;

            if (table.Equals("Users", StringComparison.OrdinalIgnoreCase))
            {
                return $@"
UPDATE dbo.{table}
SET IsArchived = 0,
    ArchivedAt = NULL,
    ArchivedByUserID = NULL,
    ArchivedByRole = NULL
WHERE {key} = @ID
  AND ClientID = @ClientID;";
            }

            if (table.Equals("Books", StringComparison.OrdinalIgnoreCase) ||
                table.Equals("Categories", StringComparison.OrdinalIgnoreCase) ||
                table.Equals("BorrowingRecords", StringComparison.OrdinalIgnoreCase) ||
                table.Equals("ReturnRecords", StringComparison.OrdinalIgnoreCase) ||
                table.Equals("FineRecords", StringComparison.OrdinalIgnoreCase))
            {
                return $@"
UPDATE dbo.{table}
SET IsArchived = 0,
    UpdatedAt = SYSUTCDATETIME()
WHERE {key} = @ID
  AND ClientID = @ClientID;";
            }

            return $@"
UPDATE dbo.{table}
SET IsArchived = 0
WHERE {key} = @ID
  AND ClientID = @ClientID;";
        }

        private void DeleteArchiveRow(ArchiveRow row)
        {
            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to permanently delete this archived record?\n\n{row.RecordType}: {row.ItemName}\nID: {row.ItemIdText}\n\nThis action cannot be undone.",
                "Confirm Permanent Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            DialogResult finalConfirm = MessageBox.Show(
                "Final confirmation: permanently delete this record now?",
                "Delete Permanently",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (finalConfirm != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                if (!TableExists(conn, row.SourceTable))
                    return;

                // FIX:
                // Do schema checks BEFORE BeginTransaction.
                // Your error happened because ColumnExists() used ExecuteScalar while
                // the connection already had a pending local transaction.
                bool hasIsArchived = ColumnExists(conn, row.SourceTable, "IsArchived");

                using SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    string query = hasIsArchived
                        ? $"DELETE FROM dbo.{row.SourceTable} WHERE {row.PrimaryKeyColumn} = @ID AND ClientID = @ClientID AND IsArchived = 1;"
                        : $"DELETE FROM dbo.{row.SourceTable} WHERE {row.PrimaryKeyColumn} = @ID AND ClientID = @ClientID;";

                    using SqlCommand cmd = new SqlCommand(query, conn, tx);
                    AddIdParameter(cmd, "@ID", row.PrimaryKeyValue);
                    cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);

                    int affected = cmd.ExecuteNonQuery();

                    if (affected <= 0)
                        throw new InvalidOperationException("No archived record was deleted. It may have already been restored or deleted.");

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }

                MessageBox.Show(
                    "Archived record permanently deleted.",
                    "Delete Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadArchiveData(GetSearchText());
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

        private void AddIdParameter(SqlCommand cmd, string parameterName, string value)
        {
            if (int.TryParse(value, out int intId)) cmd.Parameters.AddWithValue(parameterName, intId);
            else cmd.Parameters.AddWithValue(parameterName, value);
        }

        private string GetSearchText() => txtSearch == null || txtSearch.Text == SearchPlaceholder ? "" : txtSearch.Text;

        private void EnsureArchiveSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL ALTER TABLE dbo.Users ADD ClientID INT NULL;
    IF COL_LENGTH('dbo.Users', 'IsArchived') IS NULL ALTER TABLE dbo.Users ADD IsArchived BIT NOT NULL CONSTRAINT DF_Users_IsArchived DEFAULT 0;
    IF COL_LENGTH('dbo.Users', 'ArchivedAt') IS NULL ALTER TABLE dbo.Users ADD ArchivedAt DATETIME2 NULL;
    IF COL_LENGTH('dbo.Users', 'ArchivedByUserID') IS NULL ALTER TABLE dbo.Users ADD ArchivedByUserID INT NULL;
    IF COL_LENGTH('dbo.Users', 'ArchivedByRole') IS NULL ALTER TABLE dbo.Users ADD ArchivedByRole NVARCHAR(100) NULL;
END;
IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Books', 'ClientID') IS NULL ALTER TABLE dbo.Books ADD ClientID INT NULL;
    IF COL_LENGTH('dbo.Books', 'IsArchived') IS NULL ALTER TABLE dbo.Books ADD IsArchived BIT NOT NULL CONSTRAINT DF_Books_IsArchived DEFAULT 0;
    IF COL_LENGTH('dbo.Books', 'UpdatedAt') IS NULL ALTER TABLE dbo.Books ADD UpdatedAt DATETIME2 NULL;
END;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Categories', 'ClientID') IS NULL ALTER TABLE dbo.Categories ADD ClientID INT NULL;
    IF COL_LENGTH('dbo.Categories', 'IsArchived') IS NULL ALTER TABLE dbo.Categories ADD IsArchived BIT NOT NULL CONSTRAINT DF_Categories_IsArchived DEFAULT 0;
    IF COL_LENGTH('dbo.Categories', 'UpdatedAt') IS NULL ALTER TABLE dbo.Categories ADD UpdatedAt DATETIME2 NULL;
END;
IF OBJECT_ID('dbo.BorrowingRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.BorrowingRecords', 'ClientID') IS NULL ALTER TABLE dbo.BorrowingRecords ADD ClientID INT NULL;
    IF COL_LENGTH('dbo.BorrowingRecords', 'IsArchived') IS NULL ALTER TABLE dbo.BorrowingRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_BorrowingRecords_IsArchived DEFAULT 0;
    IF COL_LENGTH('dbo.BorrowingRecords', 'UpdatedAt') IS NULL ALTER TABLE dbo.BorrowingRecords ADD UpdatedAt DATETIME2 NULL;
END;
IF OBJECT_ID('dbo.ReturnRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ReturnRecords', 'ClientID') IS NULL ALTER TABLE dbo.ReturnRecords ADD ClientID INT NULL;
    IF COL_LENGTH('dbo.ReturnRecords', 'IsArchived') IS NULL ALTER TABLE dbo.ReturnRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_ReturnRecords_IsArchived DEFAULT 0;
    IF COL_LENGTH('dbo.ReturnRecords', 'UpdatedAt') IS NULL ALTER TABLE dbo.ReturnRecords ADD UpdatedAt DATETIME2 NULL;
END;
IF OBJECT_ID('dbo.FineRecords', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.FineRecords', 'ClientID') IS NULL ALTER TABLE dbo.FineRecords ADD ClientID INT NULL;
    IF COL_LENGTH('dbo.FineRecords', 'IsArchived') IS NULL ALTER TABLE dbo.FineRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_FineRecords_IsArchived DEFAULT 0;
    IF COL_LENGTH('dbo.FineRecords', 'UpdatedAt') IS NULL ALTER TABLE dbo.FineRecords ADD UpdatedAt DATETIME2 NULL;
END;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private bool TableExists(SqlConnection conn, string tableName)
        {
            const string query = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TableName", tableName);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private bool ColumnExists(SqlConnection conn, string tableName, string columnName)
        {
            const string query = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TableName", tableName);
            cmd.Parameters.AddWithValue("@ColumnName", columnName);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private string GetString(SqlDataReader reader, params string[] columns)
        {
            foreach (string column in columns)
                if (ReaderHasColumn(reader, column) && reader[column] != DBNull.Value)
                    return Convert.ToString(reader[column]) ?? "";
            return "";
        }

        private DateTime ReadDate(SqlDataReader reader, params string[] columns)
        {
            foreach (string column in columns)
                if (ReaderHasColumn(reader, column) && reader[column] != DBNull.Value && DateTime.TryParse(Convert.ToString(reader[column]), out DateTime date))
                    return date;
            return DateTime.Today;
        }

        private string FirstExistingColumn(SqlDataReader reader, params string[] columns)
        {
            foreach (string column in columns)
                if (ReaderHasColumn(reader, column)) return column;
            return columns.Length > 0 ? columns[0] : "";
        }

        private bool ReaderHasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private void RestoreUser(int userId) { }
        private void DeleteUser(int userId) { }

        private void ApplyColumnStyle()
        {
            dgvArchive.Columns["Item"].FillWeight = 26;
            dgvArchive.Columns["Role"].FillWeight = 14;
            dgvArchive.Columns["DateSaved"].FillWeight = 13;
            dgvArchive.Columns["Reason"].FillWeight = 20;
            dgvArchive.Columns["ArchivedBy"].FillWeight = 15;
            dgvArchive.Columns["Actions"].FillWeight = 10;
            dgvArchive.Columns["Actions"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvArchive.Columns["Actions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn col in dgvArchive.Columns) col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void DgvArchive_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0) return;
            string col = dgv.Columns[e.ColumnIndex].Name;
            string text = e.FormattedValue?.ToString() ?? "";
            if (col == "Item")
            {
                e.PaintBackground(e.CellBounds, true);
                string[] parts = text.Split('\n');
                string name = parts.Length > 0 ? parts[0] : text;
                string id = parts.Length > 1 ? parts[1] : "";
                TextRenderer.DrawText(e.Graphics, name, new Font("Segoe UI", 10.5F, FontStyle.Bold), new Rectangle(e.CellBounds.X + 18, e.CellBounds.Y + 22, e.CellBounds.Width - 28, 28), PrimaryText, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                TextRenderer.DrawText(e.Graphics, id, new Font("Segoe UI", 8.5F), new Rectangle(e.CellBounds.X + 18, e.CellBounds.Y + 48, e.CellBounds.Width - 28, 22), SecondaryText, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                e.Handled = true;
            }
            else if (col == "Role")
            {
                e.PaintBackground(e.CellBounds, true);
                Color bg = ColorTranslator.FromHtml("#FFF0E7");
                Color fg = ColorTranslator.FromHtml("#EA580C");
                string badgeText = $"●  {text}";
                Size sz = TextRenderer.MeasureText(badgeText, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(e.CellBounds.X + 14, e.CellBounds.Y + 30, Math.Min(e.CellBounds.Width - 20, sz.Width + 22), 26);
                using SolidBrush b = new SolidBrush(bg);
                e.Graphics.FillRectangle(b, badge);
                TextRenderer.DrawText(e.Graphics, badgeText, new Font("Segoe UI", 8.5F, FontStyle.Bold), badge, fg, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                var actionBounds = GetActionIconBounds(e.CellBounds);

                TextRenderer.DrawText(
                    e.Graphics,
                    "↺",
                    new Font("Segoe UI Symbol", 15F, FontStyle.Bold),
                    actionBounds.Restore,
                    AccentEmerald,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    "🗑",
                    new Font("Segoe UI Emoji", 11F, FontStyle.Bold),
                    actionBounds.Delete,
                    AccentDanger,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private (Rectangle Restore, Rectangle Delete) GetActionIconBounds(Rectangle cellBounds)
        {
            const int iconSize = 28;
            const int gap = 16;

            int totalWidth = (iconSize * 2) + gap;
            int left = cellBounds.X + Math.Max(0, (cellBounds.Width - totalWidth) / 2);
            int top = cellBounds.Y + Math.Max(0, (cellBounds.Height - iconSize) / 2);

            return (
                new Rectangle(left, top, iconSize, iconSize),
                new Rectangle(left + iconSize + gap, top, iconSize, iconSize)
            );
        }

        private void DgvArchive_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Kept for keyboard/cell activation support.
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvArchive.Columns[e.ColumnIndex].Name != "Actions")
                return;

            if (dgvArchive.Rows[e.RowIndex].Tag is not ArchiveRow row)
                return;

            // If user clicks the Actions cell but not exactly an icon, show a small choice.
            using ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem restore = new ToolStripMenuItem("Restore");
            ToolStripMenuItem delete = new ToolStripMenuItem("Delete permanently");

            restore.Click += (s, args) => RestoreArchiveRow(row);
            delete.Click += (s, args) => DeleteArchiveRow(row);

            menu.Items.Add(restore);
            menu.Items.Add(delete);

            Rectangle cellRect = dgvArchive.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            menu.Show(dgvArchive, new Point(cellRect.Left + 4, cellRect.Bottom - 4));
        }

        private void DgvArchive_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvArchive.Columns[e.ColumnIndex].Name != "Actions")
                return;

            if (dgvArchive.Rows[e.RowIndex].Tag is not ArchiveRow row)
                return;

            Rectangle cellBounds = dgvArchive.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            Point clickPoint = new Point(cellBounds.Left + e.X, cellBounds.Top + e.Y);

            var actionBounds = GetActionIconBounds(cellBounds);

            if (actionBounds.Restore.Contains(clickPoint))
                RestoreArchiveRow(row);
            else if (actionBounds.Delete.Contains(clickPoint))
                DeleteArchiveRow(row);
        }

        private void DgvArchive_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            if (dgv.Columns[hit.ColumnIndex].Name != "Actions")
                return;

            if (dgv.Rows[hit.RowIndex].Tag is not ArchiveRow row)
                return;

            Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
            var actionBounds = GetActionIconBounds(cellBounds);

            if (actionBounds.Restore.Contains(e.Location))
                RestoreArchiveRow(row);
            else if (actionBounds.Delete.Contains(e.Location))
                DeleteArchiveRow(row);
        }

        private void DgvArchive_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
            bool overAction = false;

            if (hit.RowIndex >= 0 &&
                hit.ColumnIndex >= 0 &&
                dgv.Columns[hit.ColumnIndex].Name == "Actions")
            {
                Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
                var actionBounds = GetActionIconBounds(cellBounds);

                overAction = actionBounds.Restore.Contains(e.Location) ||
                             actionBounds.Delete.Contains(e.Location);
            }

            dgv.Cursor = overAction ? Cursors.Hand : Cursors.Default;
        }

        private void BuildPager()
        {
            pagerPanel.Controls.Clear();
            Label page = new Label { Text = "Page 1", AutoSize = false, Size = new Size(130, 32), Location = new Point(42, 0), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = PrimaryText };
            pagerPanel.Controls.Add(page);
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int width = Math.Max(980, canvas.ClientSize.Width - margin * 2);
            lblTitle.Location = new Point(margin, 34);
            lblSubtitle.Location = new Point(margin, 82);
            tableCard.Bounds = new Rectangle(margin, 140, width, 560);
            lblTableTitle.Location = new Point(28, 28);
            searchPanel.Bounds = new Rectangle(tableCard.Width - 430, 22, 350, 40);
            searchPanel.Controls[0].Location = new Point(12, 8);
            txtSearch.Location = new Point(42, 10);
            txtSearch.Width = searchPanel.Width - 54;
            btnFilter.Bounds = new Rectangle(tableCard.Width - 66, 22, 42, 40);
            dgvArchive.Bounds = new Rectangle(0, 90, tableCard.Width, 390);
            lblFooter.Location = new Point(28, tableCard.Height - 45);
            pagerPanel.Location = new Point(tableCard.Width - pagerPanel.Width - 28, tableCard.Height - 52);
            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 30);
        }

        private Panel CreateRoundedCard()
        {
            Panel p = new Panel { BackColor = CardBack };
            p.Paint += RoundedPanelPaint;
            return p;
        }

        private void ClearGridSelection()
        {
            if (dgvArchive == null) return;
            dgvArchive.ClearSelection();
            dgvArchive.CurrentCell = null;
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control p) return;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var brush = new SolidBrush(p.BackColor);
            using var path = GetRoundedRectPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 18);
            e.Graphics.FillPath(brush, path);
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}