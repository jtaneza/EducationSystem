using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class BookManagementForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color SoftBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentBlue = ColorTranslator.FromHtml("#2563EB");
        private readonly Color AccentBlueSoft = ColorTranslator.FromHtml("#EFF6FF");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");
        private readonly Color AccentDangerSoft = ColorTranslator.FromHtml("#F7816D");
        private readonly Color AccentDangerSoftBg = ColorTranslator.FromHtml("#FDE8E4");
        private readonly Color AccentSuccessSoft = ColorTranslator.FromHtml("#B7EBD7");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Button btnAddBook = null!;

        private Panel cardAllBooks = null!;
        private Panel cardBorrowed = null!;
        private Panel cardLowStock = null!;

        private Panel tableCard = null!;
        private Panel searchPanel = null!;
        private TextBox txtSearch = null!;
        private DataGridView dgvBooks = null!;
        private Label lblFooter = null!;
        private Panel pagerPanel = null!;

        private readonly List<BookRow> books = new List<BookRow>();
        private readonly List<BookCategoryOption> categoryOptions = new List<BookCategoryOption>();
        private List<BookRow> filteredBooks = new List<BookRow>();

        private int currentPage = 1;
        private int totalPages = 1;
        private const int PageSize = 4;

        private Button btnPrevPage = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNextPage = null!;

        private const string SearchPlaceholder = "Search by title, author or ISBN...";

        private sealed class BookRow
        {
            public string BookId { get; set; } = "";
            public string Title { get; set; } = "";
            public string Author { get; set; } = "";
            public string Category { get; set; } = "";
            public string Genre { get; set; } = "";
            public string GroupName { get; set; } = "";
            public int? PublishYear { get; set; }
            public int Quantity { get; set; } = 1;
            public string Status { get; set; } = "In Stock";
        }

        public BookManagementForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Text = "BookManagementForm";

            BuildInterface();
            AdjustLayout();

            Load += async (s, e) =>
            {
                await LoadBookDataAsync();
                AdjustLayout();
                ClearGridSelection();
            };

            Resize += (s, e) => AdjustLayout();
        }

        private void BuildInterface()
        {
            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = FormBack,
                AutoScroll = true
            };
            canvas.HorizontalScroll.Enabled = false;
            canvas.HorizontalScroll.Visible = false;
            Controls.Add(canvas);

            lblTitle = new Label
            {
                Text = "Book Management",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubTitle = new Label
            {
                Text = "Catalog oversight and inventory control",
                AutoSize = true,
                Font = new Font("Segoe UI", 14F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            btnAddBook = new Button
            {
                Text = "+  Add New Book",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddBook.FlatAppearance.BorderSize = 0;
            btnAddBook.Click += async (s, e) => await ShowBookDialogAsync(null);

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubTitle);

            BuildMetricCards();
            BuildTableCard();
        }

        private void BuildMetricCards()
        {
            cardAllBooks = CreateRoundedCard();
            cardBorrowed = CreateRoundedCard();
            cardLowStock = CreateRoundedCard();

            canvas.Controls.Add(cardAllBooks);
            canvas.Controls.Add(cardBorrowed);
            canvas.Controls.Add(cardLowStock);

            BuildAllBooksCard();
            BuildBorrowedCard();
            BuildLowStockCard();
        }

        private void BuildAllBooksCard()
        {
            Panel iconWrap = CreateIconWrap(ColorTranslator.FromHtml("#ECFDF3"));
            Label icon = CreateIconLabel("▣", AccentDeep);
            iconWrap.Controls.Add(icon);

            Label badge = CreateBadge("Live", ColorTranslator.FromHtml("#ECFDF3"), AccentDeep);
            Label label = CreateMetricLabel("All Books");
            Label value = CreateMetricValue("0");

            cardAllBooks.Controls.Add(iconWrap);
            cardAllBooks.Controls.Add(badge);
            cardAllBooks.Controls.Add(label);
            cardAllBooks.Controls.Add(value);

            cardAllBooks.Tag = new Control[] { iconWrap, badge, label, value };
        }

        private void BuildBorrowedCard()
        {
            Panel iconWrap = CreateIconWrap(AccentBlueSoft);
            Label icon = CreateIconLabel("📖", AccentBlue);
            iconWrap.Controls.Add(icon);

            Label badge = CreateBadge("Live", AccentBlueSoft, AccentBlue);
            Label label = CreateMetricLabel("Books with Students");
            Label value = CreateMetricValue("0");

            cardBorrowed.Controls.Add(iconWrap);
            cardBorrowed.Controls.Add(badge);
            cardBorrowed.Controls.Add(label);
            cardBorrowed.Controls.Add(value);

            cardBorrowed.Tag = new Control[] { iconWrap, badge, label, value };
        }

        private void BuildLowStockCard()
        {
            Panel iconWrap = CreateIconWrap(AccentDangerSoftBg);
            Label icon = CreateIconLabel("⚠", AccentDanger);
            iconWrap.Controls.Add(icon);

            Label badge = CreateBadge("Alert", AccentDangerSoftBg, AccentDangerSoft);
            Label label = CreateMetricLabel("Needs More Copies");

            Label value = new Label
            {
                Text = "0",
                AutoSize = true,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            Label suffix = new Label
            {
                Text = "books",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            cardLowStock.Controls.Add(iconWrap);
            cardLowStock.Controls.Add(badge);
            cardLowStock.Controls.Add(label);
            cardLowStock.Controls.Add(value);
            cardLowStock.Controls.Add(suffix);

            cardLowStock.Tag = new Control[] { iconWrap, badge, label, value, suffix };
        }

        private void BuildTableCard()
        {
            tableCard = CreateRoundedCard();
            canvas.Controls.Add(tableCard);

            searchPanel = new Panel
            {
                BackColor = SoftBack
            };
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
                BorderStyle = BorderStyle.None,
                BackColor = SoftBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 11F),
                Text = SearchPlaceholder
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

                currentPage = 1;
                RebuildBookRows(GetCurrentSearchText());
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(txtSearch);

            tableCard.Controls.Add(searchPanel);
            tableCard.Controls.Add(btnAddBook);

            dgvBooks = new DataGridView
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
                ColumnHeadersHeight = 54,
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ShowCellToolTips = false
            };

            dgvBooks.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvBooks.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvBooks.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvBooks.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvBooks.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvBooks.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvBooks.DefaultCellStyle.BackColor = CardBack;
            dgvBooks.DefaultCellStyle.ForeColor = PrimaryText;
            dgvBooks.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            dgvBooks.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvBooks.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvBooks.DefaultCellStyle.Padding = new Padding(6, 6, 6, 6);

            dgvBooks.RowsDefaultCellStyle.BackColor = CardBack;
            dgvBooks.RowsDefaultCellStyle.SelectionBackColor = CardBack;
            dgvBooks.RowsDefaultCellStyle.SelectionForeColor = PrimaryText;

            dgvBooks.RowTemplate.Height = 90;

            dgvBooks.Columns.Add("BookId", "BOOK\r\nID");
            dgvBooks.Columns.Add("TitleAuthor", "TITLE & AUTHOR");
            dgvBooks.Columns.Add("Category", "CATEGORY");
            dgvBooks.Columns.Add("Genre", "GENRE");
            dgvBooks.Columns.Add("Group", "GROUP");
            dgvBooks.Columns.Add("Year", "YEAR");
            dgvBooks.Columns.Add("Qty", "QTY");
            dgvBooks.Columns.Add("Status", "STATUS");
            dgvBooks.Columns.Add("Actions", "ACTIONS");

            ApplyColumnWidths();

            foreach (DataGridViewColumn col in dgvBooks.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvBooks.CellPainting += DgvBooks_CellPainting;
            dgvBooks.MouseClick += DgvBooks_MouseClick;
            dgvBooks.MouseMove += DgvBooks_MouseMove;
            dgvBooks.DataBindingComplete += (s, e) => ClearGridSelection();
            dgvBooks.SelectionChanged += (s, e) => ClearGridSelection();

            lblFooter = new Label
            {
                Text = "Showing 0 of 0 books",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            pagerPanel = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(190, 36)
            };

            BuildPager();

            tableCard.Controls.Add(dgvBooks);
            tableCard.Controls.Add(lblFooter);
            tableCard.Controls.Add(pagerPanel);
        }

        private async Task LoadBookDataAsync()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                await conn.OpenAsync();

                await EnsureBooksTableAsync(conn);
                await EnsureCategoriesTableAsync(conn);
                await LoadCategoryOptionsAsync(conn);
                await LoadBooksAsync(conn);

                RebuildBookRows(GetCurrentSearchText());
                UpdateMetricCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Books could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async Task LoadBooksAsync(SqlConnection conn)
        {
            books.Clear();

            const string query = @"
SELECT BookID, BookTitle, Author, Category, Genre, GroupName, PublishYear, Quantity, Status
FROM dbo.Books
WHERE IsArchived = 0
  AND ClientID = @ClientID
ORDER BY CreatedAt DESC, BookTitle ASC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                books.Add(new BookRow
                {
                    BookId = Convert.ToString(reader["BookID"]) ?? "",
                    Title = Convert.ToString(reader["BookTitle"]) ?? "",
                    Author = Convert.ToString(reader["Author"]) ?? "",
                    Category = Convert.ToString(reader["Category"]) ?? "",
                    Genre = Convert.ToString(reader["Genre"]) ?? "",
                    GroupName = Convert.ToString(reader["GroupName"]) ?? "",
                    PublishYear = reader["PublishYear"] == DBNull.Value ? null : Convert.ToInt32(reader["PublishYear"]),
                    Quantity = reader["Quantity"] == DBNull.Value ? 1 : Convert.ToInt32(reader["Quantity"]),
                    Status = Convert.ToString(reader["Status"]) ?? "In Stock"
                });
            }
        }

        private async Task LoadCategoryOptionsAsync(SqlConnection conn)
        {
            categoryOptions.Clear();

            const string query = @"
SELECT CategoryName, Genre, GroupName
FROM dbo.Categories
WHERE IsArchived = 0
  AND ClientID = @ClientID
ORDER BY CategoryName ASC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categoryOptions.Add(new BookCategoryOption(
                    Convert.ToString(reader["CategoryName"]) ?? "",
                    Convert.ToString(reader["Genre"]) ?? "",
                    Convert.ToString(reader["GroupName"]) ?? ""
                ));
            }
        }

        private async Task<bool> RefreshCategoryOptionsAsync()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                await conn.OpenAsync();

                await EnsureCategoriesTableAsync(conn);
                await LoadCategoryOptionsAsync(conn);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Category choices could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
        }

        private async Task ShowBookDialogAsync(BookRow? book)
        {
            if (!await RefreshCategoryOptionsAsync())
                return;

            if (categoryOptions.Count == 0)
            {
                MessageBox.Show(
                    "Please add at least one category first. The book dialog uses the Category, Genre, and Group data from dbo.Categories.",
                    "No Categories Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            string recordedBy = GetRecordedBy();
            BookDialogData? existingBook = book == null ? null : new BookDialogData
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Category = book.Category,
                Genre = book.Genre,
                GroupName = book.GroupName,
                PublishYear = book.PublishYear,
                Quantity = book.Quantity,
                Status = book.Status
            };

            using AddBookDialog dialog = new AddBookDialog(
                book?.BookId ?? GenerateBookId(),
                recordedBy,
                categoryOptions,
                existingBook
            );

            if (ShowBookDialogWithOverlay(dialog) != DialogResult.OK)
                return;

            try
            {
                if (book == null)
                    await InsertBookAsync(dialog.BookData, recordedBy);
                else
                    await UpdateBookAsync(dialog.BookData, recordedBy);

                await LoadBookDataAsync();
                ClearGridSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Book was not saved to the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async Task InsertBookAsync(BookDialogData data, string recordedBy)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            await conn.OpenAsync();
            await EnsureBooksTableAsync(conn);

            const string query = @"
INSERT INTO dbo.Books
    (ClientID, BookID, BookTitle, Author, Category, Genre, GroupName, PublishYear, Quantity, Status, RecordedBy, CreatedAt, IsArchived)
VALUES
    (@ClientID, @BookID, @BookTitle, @Author, @Category, @Genre, @GroupName, @PublishYear, @Quantity, @Status, @RecordedBy, SYSUTCDATETIME(), 0);";

            using SqlCommand cmd = new SqlCommand(query, conn);
            AddBookParameters(cmd, data, recordedBy);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task UpdateBookAsync(BookDialogData data, string recordedBy)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            await conn.OpenAsync();
            await EnsureBooksTableAsync(conn);

            const string query = @"
UPDATE dbo.Books
SET BookTitle = @BookTitle,
    Author = @Author,
    Category = @Category,
    Genre = @Genre,
    GroupName = @GroupName,
    PublishYear = @PublishYear,
    Quantity = @Quantity,
    Status = @Status,
    RecordedBy = @RecordedBy,
    UpdatedAt = SYSUTCDATETIME()
WHERE BookID = @BookID
  AND ClientID = @ClientID
  AND IsArchived = 0;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            AddBookParameters(cmd, data, recordedBy);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task ArchiveBookAsync(BookRow book)
        {
            DialogResult confirm = MessageBox.Show(
                $"Archive \"{book.Title}\"?",
                "Archive Book",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                await conn.OpenAsync();
                await EnsureBooksTableAsync(conn);

                const string query = @"
UPDATE dbo.Books
SET IsArchived = 1,
    UpdatedAt = SYSUTCDATETIME()
WHERE BookID = @BookID
  AND ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BookID", book.BookId);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                await cmd.ExecuteNonQueryAsync();

                books.Remove(book);
                RebuildBookRows(GetCurrentSearchText());
                UpdateMetricCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Book was not archived.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void AddBookParameters(SqlCommand cmd, BookDialogData data, string recordedBy)
        {
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            cmd.Parameters.AddWithValue("@BookID", data.BookId);
            cmd.Parameters.AddWithValue("@BookTitle", data.Title);
            cmd.Parameters.AddWithValue("@Author", data.Author);
            cmd.Parameters.AddWithValue("@Category", data.Category);
            cmd.Parameters.AddWithValue("@Genre", data.Genre);
            cmd.Parameters.AddWithValue("@GroupName", data.GroupName);
            cmd.Parameters.AddWithValue("@PublishYear", data.PublishYear.HasValue ? data.PublishYear.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity", data.Quantity);
            cmd.Parameters.AddWithValue("@Status", data.Status);
            cmd.Parameters.AddWithValue("@RecordedBy", recordedBy);
        }

        private async Task EnsureBooksTableAsync(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.Books', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Books
    (
        BookID NVARCHAR(20) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        BookTitle NVARCHAR(250) NOT NULL,
        Author NVARCHAR(150) NOT NULL,
        Category NVARCHAR(150) NOT NULL,
        Genre NVARCHAR(100) NOT NULL,
        GroupName NVARCHAR(100) NOT NULL,
        PublishYear INT NULL,
        Quantity INT NOT NULL DEFAULT 1,
        Status NVARCHAR(50) NOT NULL DEFAULT 'In Stock',
        RecordedBy NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.Books', 'ClientID') IS NULL
    ALTER TABLE dbo.Books ADD ClientID INT NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task EnsureCategoriesTableAsync(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.Categories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        CategoryID NVARCHAR(20) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        CategoryName NVARCHAR(150) NOT NULL,
        Genre NVARCHAR(100) NOT NULL,
        GroupName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        AddedBy NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.Categories', 'ClientID') IS NULL
    ALTER TABLE dbo.Categories ADD ClientID INT NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        private void RebuildBookRows(string keyword)
        {
            if (dgvBooks == null)
                return;

            string q = keyword.Trim().ToLowerInvariant();

            filteredBooks = books
                .Where(book =>
                    string.IsNullOrWhiteSpace(q) ||
                    $"{book.BookId} {book.Title} {book.Author} {book.Category} {book.Genre} {book.GroupName} {book.Status}"
                        .ToLowerInvariant()
                        .Contains(q))
                .ToList();

            totalPages = Math.Max(1, (int)Math.Ceiling(filteredBooks.Count / (double)PageSize));

            if (currentPage > totalPages)
                currentPage = totalPages;

            if (currentPage < 1)
                currentPage = 1;

            List<BookRow> pageBooks = filteredBooks
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            dgvBooks.Rows.Clear();

            foreach (BookRow book in pageBooks)
            {
                int rowIndex = dgvBooks.Rows.Add(
                    book.BookId,
                    $"{book.Title}\\n{book.Author}",
                    book.Category,
                    book.Genre,
                    book.GroupName,
                    book.PublishYear?.ToString() ?? "",
                    book.Quantity.ToString("00"),
                    GetDisplayStatus(book),
                    ""
                );

                dgvBooks.Rows[rowIndex].Tag = book;
            }

            UpdateFooter(filteredBooks.Count, pageBooks.Count);
            UpdatePagerButtons();
            ClearGridSelection();
        }

        private string GetDisplayStatus(BookRow book)
        {
            if (book.Quantity <= 2 && book.Status.Equals("In Stock", StringComparison.OrdinalIgnoreCase))
                return "Low\nStock";

            return book.Status;
        }

        private void UpdateFooter(int totalFilteredCount, int visiblePageCount)
        {
            if (lblFooter == null)
                return;

            if (books.Count == 0 || totalFilteredCount == 0)
            {
                lblFooter.Text = $"Showing 0 of {books.Count:N0} books";
                return;
            }

            int start = ((currentPage - 1) * PageSize) + 1;
            int end = Math.Min(start + visiblePageCount - 1, totalFilteredCount);

            lblFooter.Text = $"Showing {start:N0} to {end:N0} of {totalFilteredCount:N0} books";
        }

        private string GetCurrentSearchText()
        {
            if (txtSearch == null || txtSearch.Text == SearchPlaceholder)
                return "";

            return txtSearch.Text;
        }

        private string GenerateBookId()
        {
            string id;
            do
            {
                id = "BK-" + Random.Shared.Next(1000, 10000);
            }
            while (books.Any(book => book.BookId.Equals(id, StringComparison.OrdinalIgnoreCase)));

            return id;
        }

        private string GetRecordedBy()
        {
            if (!string.IsNullOrWhiteSpace(ClientSession.Username))
                return ClientSession.Username!;

            if (!string.IsNullOrWhiteSpace(UserSession.Username))
                return UserSession.Username!;

            return "ADM-088";
        }

        private void UpdateMetricCards()
        {
            SetMetricValue(cardAllBooks, books.Count.ToString("N0"));
            SetMetricValue(cardBorrowed, GetBorrowedCount().ToString("N0"));
            SetMetricValue(cardLowStock, books.Count(book => book.Quantity <= 2).ToString("N0"));
            LayoutMetricCards();
        }


        private int GetBorrowedCount()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                const string query = @"
SELECT COUNT(*)
FROM dbo.BorrowingRecords
WHERE ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0
  AND UPPER(ISNULL(Status, '')) <> 'RETURNED';";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);

                return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
            }
            catch
            {
                return 0;
            }
        }

        private void SetMetricValue(Panel card, string text)
        {
            if (card.Tag is Control[] controls && controls.Length > 3 && controls[3] is Label value)
                value.Text = text;
        }

        private void ClearGridSelection()
        {
            if (dgvBooks == null) return;
            dgvBooks.ClearSelection();
            dgvBooks.CurrentCell = null;
        }

        private DialogResult ShowBookDialogWithOverlay(AddBookDialog dialog)
        {
            Form? owner = FindForm();
            if (owner == null)
                return dialog.ShowDialog(this);

            using Form overlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Bounds = owner.Bounds,
                BackColor = Color.Black,
                Opacity = 0.34,
                ShowInTaskbar = false,
                TopMost = false
            };

            overlay.Show(owner);
            overlay.BringToFront();

            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = new Point(
                overlay.Left + Math.Max(0, (overlay.Width - dialog.Width) / 2),
                overlay.Top + Math.Max(0, (overlay.Height - dialog.Height) / 2)
            );

            return dialog.ShowDialog(overlay);
        }

        private (Rectangle Edit, Rectangle Archive) GetActionIconBounds(Rectangle cellBounds)
        {
            const int iconSize = 24;
            const int gap = 10;

            int totalWidth = (iconSize * 2) + gap;
            int left = cellBounds.X + Math.Max(0, (cellBounds.Width - totalWidth) / 2);
            int top = cellBounds.Y + Math.Max(0, (cellBounds.Height - iconSize) / 2);

            return (
                new Rectangle(left, top, iconSize, iconSize),
                new Rectangle(left + iconSize + gap, top, iconSize, iconSize)
            );
        }

        private void DgvBooks_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            if (dgv.Columns[hit.ColumnIndex].Name != "Actions")
                return;

            if (dgv.Rows[hit.RowIndex].Tag is not BookRow book)
                return;

            Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
            if (!cellBounds.Contains(e.Location))
                return;

            if (e.X < cellBounds.Left + (cellBounds.Width / 2))
                _ = ShowBookDialogAsync(book);
            else
                _ = ArchiveBookAsync(book);
        }

        private void DgvBooks_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
            bool overActionCell = false;

            if (hit.RowIndex >= 0 &&
                hit.ColumnIndex >= 0 &&
                dgv.Columns[hit.ColumnIndex].Name == "Actions")
            {
                Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
                overActionCell = cellBounds.Contains(e.Location);
            }

            dgv.Cursor = overActionCell ? Cursors.Hand : Cursors.Default;
        }

        private void BuildPager()
        {
            pagerPanel.Controls.Clear();

            btnPrevPage = CreatePagerButton("‹", false, new Point(0, 0));
            btnPage1 = CreatePagerButton("1", true, new Point(40, 0));
            btnPage2 = CreatePagerButton("2", false, new Point(80, 0));
            btnPage3 = CreatePagerButton("3", false, new Point(120, 0));
            btnNextPage = CreatePagerButton("›", false, new Point(160, 0));

            btnPrevPage.Click += (s, e) =>
            {
                if (currentPage <= 1) return;
                currentPage--;
                RebuildBookRows(GetCurrentSearchText());
            };

            btnNextPage.Click += (s, e) =>
            {
                if (currentPage >= totalPages) return;
                currentPage++;
                RebuildBookRows(GetCurrentSearchText());
            };

            btnPage1.Click += (s, e) => GoToBookPage(btnPage1);
            btnPage2.Click += (s, e) => GoToBookPage(btnPage2);
            btnPage3.Click += (s, e) => GoToBookPage(btnPage3);

            pagerPanel.Controls.Add(btnPrevPage);
            pagerPanel.Controls.Add(btnPage1);
            pagerPanel.Controls.Add(btnPage2);
            pagerPanel.Controls.Add(btnPage3);
            pagerPanel.Controls.Add(btnNextPage);

            UpdatePagerButtons();
        }

        private void GoToBookPage(Button button)
        {
            if (button.Tag == null)
                return;

            if (int.TryParse(button.Tag.ToString(), out int page))
            {
                currentPage = page;
                RebuildBookRows(GetCurrentSearchText());
            }
        }

        private Button CreatePagerButton(string text, bool active, Point location)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(32, 32),
                Location = location,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D9E0E2");
            ApplyPagerStyle(btn, active, true);

            return btn;
        }

        private void UpdatePagerButtons()
        {
            if (btnPrevPage == null || btnPage1 == null || btnPage2 == null || btnPage3 == null || btnNextPage == null)
                return;

            totalPages = Math.Max(1, totalPages);

            btnPrevPage.Enabled = currentPage > 1;
            btnNextPage.Enabled = currentPage < totalPages;

            ApplyPagerStyle(btnPrevPage, false, btnPrevPage.Enabled);
            ApplyPagerStyle(btnNextPage, false, btnNextPage.Enabled);

            int firstPage = Math.Max(1, currentPage - 1);

            if (currentPage >= totalPages && totalPages >= 3)
                firstPage = totalPages - 2;

            SetNumberPagerButton(btnPage1, firstPage);
            SetNumberPagerButton(btnPage2, firstPage + 1);
            SetNumberPagerButton(btnPage3, firstPage + 2);
        }

        private void SetNumberPagerButton(Button button, int pageNumber)
        {
            bool visible = pageNumber >= 1 && pageNumber <= totalPages;
            button.Visible = visible;

            if (!visible)
                return;

            button.Text = pageNumber.ToString();
            button.Tag = pageNumber;
            ApplyPagerStyle(button, pageNumber == currentPage, true);
        }

        private void ApplyPagerStyle(Button button, bool active, bool enabled)
        {
            button.Enabled = enabled;
            button.BackColor = active ? AccentDeep : Color.Transparent;
            button.ForeColor = active ? Color.White : enabled ? SecondaryText : Color.FromArgb(150, 165, 170);
            button.FlatAppearance.BorderSize = active ? 0 : 1;
        }

        private Panel CreateRoundedCard()
        {
            Panel p = new Panel
            {
                BackColor = CardBack
            };

            p.Paint += RoundedPanelPaint;
            return p;
        }

        private Panel CreateIconWrap(Color backColor)
        {
            Panel p = new Panel
            {
                BackColor = backColor,
                Size = new Size(42, 42)
            };

            p.Paint += RoundedPanelPaint;
            return p;
        }

        private Label CreateIconLabel(string text, Color color)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 16F, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private Label CreateBadge(string text, Color bg, Color fg)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Size = new Size(58, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = bg,
                ForeColor = fg
            };
        }

        private Label CreateMetricLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };
        }

        private Label CreateMetricValue(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };
        }

        private void AdjustLayout()
        {
            int margin = 20;
            int gap = 24;

            int usableWidth = Math.Max(1080, canvas.ClientSize.Width - (margin * 2));

            lblTitle.Location = new Point(margin, 28);
            lblSubTitle.Location = new Point(margin, 72);

            int cardsTop = 118;
            int cardWidth = (usableWidth - (gap * 2)) / 3;
            int cardHeight = 155;

            cardAllBooks.Bounds = new Rectangle(margin, cardsTop, cardWidth, cardHeight);
            cardBorrowed.Bounds = new Rectangle(cardAllBooks.Right + gap, cardsTop, cardWidth, cardHeight);
            cardLowStock.Bounds = new Rectangle(cardBorrowed.Right + gap, cardsTop, cardWidth, cardHeight);

            LayoutMetricCards();

            int tableTop = cardAllBooks.Bottom + 28;
            tableCard.Bounds = new Rectangle(margin, tableTop, usableWidth, 640);

            btnAddBook.Bounds = new Rectangle(tableCard.Width - 190 - 22, 18, 190, 42);

            int searchGap = 10;
            int searchWidth = 420;
            int searchX = btnAddBook.Left - searchGap - searchWidth;

            searchPanel.Bounds = new Rectangle(searchX, 19, searchWidth, 38);
            searchPanel.Bounds = new Rectangle(searchX, 19, searchWidth, 38);

            if (searchPanel.Controls.Count > 0)
            {
                searchPanel.Controls[0].Location = new Point(12, 7);
            }

            txtSearch.Location = new Point(40, 9);
            txtSearch.Width = searchPanel.Width - 50;

            dgvBooks.Location = new Point(0, 78);
            dgvBooks.Size = new Size(tableCard.Width, 414);

            ApplyColumnWidths();

            lblFooter.Location = new Point(20, 574);
            pagerPanel.Location = new Point(tableCard.Width - pagerPanel.Width - 20, 566);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 24);
        }

        private void ApplyColumnWidths()
        {
            if (dgvBooks == null || dgvBooks.Columns.Count == 0)
                return;

            dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvBooks.Columns["BookId"].FillWeight = 10;
            dgvBooks.Columns["TitleAuthor"].FillWeight = 18;
            dgvBooks.Columns["Category"].FillWeight = 10;
            dgvBooks.Columns["Genre"].FillWeight = 10;
            dgvBooks.Columns["Group"].FillWeight = 10;
            dgvBooks.Columns["Year"].FillWeight = 10;
            dgvBooks.Columns["Qty"].FillWeight = 10;
            dgvBooks.Columns["Status"].FillWeight = 11;
            dgvBooks.Columns["Actions"].FillWeight = 11;

            dgvBooks.Columns["BookId"].DefaultCellStyle.Padding = new Padding(0);
            dgvBooks.Columns["TitleAuthor"].DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            dgvBooks.Columns["Category"].DefaultCellStyle.Padding = new Padding(0);
            dgvBooks.Columns["Genre"].DefaultCellStyle.Padding = new Padding(0);
            dgvBooks.Columns["Group"].DefaultCellStyle.Padding = new Padding(0);
            dgvBooks.Columns["Status"].DefaultCellStyle.Padding = new Padding(0);

            dgvBooks.Columns["BookId"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["TitleAuthor"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvBooks.Columns["Category"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Genre"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Group"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Year"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Qty"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Actions"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvBooks.Columns["BookId"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Category"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Genre"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Group"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Year"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBooks.Columns["Actions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void LayoutMetricCards()
        {
            void LayoutCard(Panel card, int badgeOffset)
            {
                var c = (Control[])card.Tag;

                Panel iconWrap = (Panel)c[0];
                Label badge = (Label)c[1];
                Label label = (Label)c[2];
                Label value = (Label)c[3];

                iconWrap.Location = new Point(20, 20);
                badge.Location = new Point(card.Width - badgeOffset, 20);

                label.Location = new Point(20, 80);
                value.Location = new Point(20, 105);
            }

            LayoutCard(cardAllBooks, 70);
            LayoutCard(cardBorrowed, 60);
            LayoutCard(cardLowStock, 65);

            var c2 = (Control[])cardLowStock.Tag;
            Label value2 = (Label)c2[3];
            Label suffix = (Label)c2[4];

            suffix.Location = new Point(value2.Right + 6, value2.Top + 10);
        }

        private void DgvBooks_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0)
                return;

            string col = dgv.Columns[e.ColumnIndex].Name;
            string text = e.FormattedValue?.ToString() ?? "";

            if (col == "TitleAuthor")
            {
                e.PaintBackground(e.CellBounds, true);

                string[] parts = text.Split('\n');
                string title = parts.Length > 0 ? parts[0] : text;
                string author = parts.Length > 1 ? string.Join("\n", parts, 1, parts.Length - 1) : "";

                Rectangle book = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 16, 40, 56);

                using (SolidBrush b = new SolidBrush(Color.FromArgb(245, 247, 248)))
                    e.Graphics.FillRectangle(b, book);

                using (Pen p = new Pen(Color.FromArgb(220, 225, 228)))
                    e.Graphics.DrawRectangle(p, book);

                TextRenderer.DrawText(
                    e.Graphics,
                    GetCoverText(title),
                    new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    book,
                    Color.Black,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                TextRenderer.DrawText(
                    e.Graphics,
                    title,
                    new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 60, e.CellBounds.Y + 14, e.CellBounds.Width - 66, 28),
                    PrimaryText,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                TextRenderer.DrawText(
                    e.Graphics,
                    author,
                    new Font("Segoe UI", 9.5F),
                    new Rectangle(e.CellBounds.X + 60, e.CellBounds.Y + 42, e.CellBounds.Width - 66, e.CellBounds.Height - 46),
                    SecondaryText,
                    TextFormatFlags.Left | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (col == "Category" || col == "Genre" || col == "Group")
            {
                e.PaintBackground(e.CellBounds, true);

                string[] tags = text
                    .Split('\n')
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .ToArray();

                if (tags.Length == 0)
                {
                    e.Handled = true;
                    return;
                }

                int totalBadgesHeight = tags.Length == 1 ? 26 : (tags.Length * 26) + ((tags.Length - 1) * 8);
                int top = e.CellBounds.Y + (e.CellBounds.Height - totalBadgesHeight) / 2;

                foreach (var tag in tags)
                {
                    Size sz = TextRenderer.MeasureText(tag, new Font("Segoe UI", 9.5F));
                    int maxBadgeWidth = Math.Max(34, e.CellBounds.Width - 18);
                    int badgeWidth = Math.Min(sz.Width + 24, maxBadgeWidth);

                    Rectangle badge = new Rectangle(
                        e.CellBounds.X + (e.CellBounds.Width - badgeWidth) / 2,
                        top,
                        badgeWidth,
                        26
                    );

                    using (SolidBrush b = new SolidBrush(Color.FromArgb(80, AccentSuccessSoft)))
                        e.Graphics.FillEllipse(b, badge);

                    TextRenderer.DrawText(
                        e.Graphics,
                        tag,
                        new Font("Segoe UI", 9.5F),
                        badge,
                        ColorTranslator.FromHtml("#3B6B5C"),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                    top += 34;
                }

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                bool low = text.Contains("Low", StringComparison.OrdinalIgnoreCase) ||
                           text.Contains("Restricted", StringComparison.OrdinalIgnoreCase);
                Color dot = low ? AccentDanger : AccentDeep;
                Color fg = low ? AccentDanger : AccentDeep;

                using Font statusFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                Size statusSize = TextRenderer.MeasureText(text, statusFont);
                int textWidth = Math.Min(statusSize.Width, Math.Max(42, e.CellBounds.Width - 34));
                int groupWidth = textWidth + 18;
                int groupLeft = e.CellBounds.X + Math.Max(8, (e.CellBounds.Width - groupWidth) / 2);

                Rectangle dotRect = new Rectangle(groupLeft, e.CellBounds.Y + 36, 8, 8);

                using (SolidBrush b = new SolidBrush(dot))
                    e.Graphics.FillEllipse(b, dotRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    statusFont,
                    new Rectangle(groupLeft + 18, e.CellBounds.Y + 18, textWidth, e.CellBounds.Height - 18),
                    fg,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                var actionBounds = GetActionIconBounds(e.CellBounds);

                TextRenderer.DrawText(
                    e.Graphics,
                    "✎",
                    new Font("Segoe UI Symbol", 10F, FontStyle.Bold),
                    actionBounds.Edit,
                    AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    "📥",
                    new Font("Segoe UI Symbol", 10F, FontStyle.Bold),
                    actionBounds.Archive,
                    AccentDanger,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private string GetCoverText(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "BOOK";

            string compact = new string(title
                .Where(char.IsLetterOrDigit)
                .Take(6)
                .ToArray());

            return string.IsNullOrWhiteSpace(compact) ? "BOOK" : compact.ToUpperInvariant();
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
