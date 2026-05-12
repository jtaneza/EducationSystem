using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class ReturnsForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SoftBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentSoft = ColorTranslator.FromHtml("#B7EBD7");

        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");
        private readonly Color AccentDangerBg = ColorTranslator.FromHtml("#F7816D");
        private readonly Color DarkCard = ColorTranslator.FromHtml("#2B3234");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Button btnProcessReturn = null!;

        private Panel cardReturns = null!;
        private Panel cardLate = null!;
        private Panel cardDamaged = null!;
        private Panel cardAccuracy = null!;

        private Panel tableCard = null!;
        private Panel searchPanel = null!;
        private TextBox txtSearch = null!;
        private Label lblTableTitle = null!;
        private DataGridView dgvReturns = null!;
        private Label lblFooter = null!;
        private Panel pagerPanel = null!;

        private readonly List<ReturnRow> returnRecords = new List<ReturnRow>();
        private readonly List<ReturnLoanOption> returnLoanOptions = new List<ReturnLoanOption>();
        private List<ReturnRow> filteredReturnRecords = new List<ReturnRow>();

        private int currentPage = 1;
        private int totalPages = 1;
        private const int PageSize = 4;

        private Button btnPrevPage = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNextPage = null!;

        private const string SearchPlaceholder = "Search records...";

        private sealed class ReturnRow
        {
            public int ReturnId { get; set; }
            public int? BorrowId { get; set; }
            public string MemberName { get; set; } = "";
            public string BookTitle { get; set; } = "";
            public DateTime ReturnDate { get; set; } = DateTime.Today;
            public string Condition { get; set; } = "GOOD";
            public string Status { get; set; } = "ON-TIME";
            public int DaysOverdue { get; set; }
            public decimal FineAmount { get; set; }
        }

        public ReturnsForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            BuildUI();

            Load += (s, e) =>
            {
                AdjustLayout();
                LoadReturnData();
                ClearGridSelection();
            };

            Resize += (s, e) => AdjustLayout();
        }

        private void BuildUI()
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
                Text = "Returns Management",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubtitle = new Label
            {
                Text = "Manage student book returns, assess condition, and track compliance.",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            btnProcessReturn = new Button
            {
                Text = "+   Process Return",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnProcessReturn.FlatAppearance.BorderSize = 0;
            btnProcessReturn.Click += BtnProcessReturn_Click;

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubtitle);
            canvas.Controls.Add(btnProcessReturn);

            BuildCards();
            BuildTable();
        }

        private void BuildCards()
        {
            cardReturns = CreateRoundedCard();
            cardLate = CreateRoundedCard();
            cardDamaged = CreateRoundedCard();
            cardAccuracy = CreateRoundedCard();
            cardAccuracy.BackColor = DarkCard;

            canvas.Controls.Add(cardReturns);
            canvas.Controls.Add(cardLate);
            canvas.Controls.Add(cardDamaged);
            canvas.Controls.Add(cardAccuracy);

            BuildMetricCard(cardReturns, "✓", "0", "RETURNS THIS MONTH", AccentSoft, AccentDeep, false);
            BuildMetricCard(cardLate, "⚠", "0", "LATE RETURNS TODAY", ColorTranslator.FromHtml("#FDE8E4"), AccentDanger, false);
            BuildMetricCard(cardDamaged, "▰", "0", "DAMAGED REPORTS", AccentSoft, AccentDeep, false);
            BuildMetricCard(cardAccuracy, "", "100%", "RETURN ACCURACY RATE", DarkCard, AccentEmerald, true);
        }

        private void BuildMetricCard(Panel card, string iconText, string valueText, string labelText, Color iconBack, Color accentColor, bool dark)
        {
            Panel iconBox = new Panel
            {
                BackColor = iconBack,
                Size = new Size(44, 44),
                Visible = !dark
            };
            iconBox.Paint += RoundedPanelPaint;

            Label icon = new Label
            {
                Text = iconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 18F, FontStyle.Bold),
                ForeColor = accentColor,
                BackColor = Color.Transparent
            };
            iconBox.Controls.Add(icon);

            Label value = new Label
            {
                Text = valueText,
                AutoSize = true,
                Font = new Font("Segoe UI", dark ? 24F : 24F, FontStyle.Bold),
                ForeColor = dark ? ColorTranslator.FromHtml("#6DFAD2") : PrimaryText,
                BackColor = Color.Transparent
            };

            Label label = new Label
            {
                Text = labelText,
                AutoSize = false,
                Size = new Size(260, 26),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = dark ? ColorTranslator.FromHtml("#EEF5F7") : SecondaryText,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(iconBox);
            card.Controls.Add(value);
            card.Controls.Add(label);

            card.Tag = new Control[] { iconBox, value, label };
        }

        private void BuildTable()
        {
            tableCard = CreateRoundedCard();
            tableCard.BackColor = SoftBack;
            canvas.Controls.Add(tableCard);

            searchPanel = new Panel
            {
                BackColor = HeaderBack
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
                Text = SearchPlaceholder,
                BorderStyle = BorderStyle.None,
                BackColor = HeaderBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 11F)
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
                ApplySearch(txtSearch.Text);
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(txtSearch);

            lblTableTitle = new Label
            {
                Text = "Recent Return History",
                AutoSize = true,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            dgvReturns = new DataGridView
            {
                BackgroundColor = SoftBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 64,
                RowTemplate = { Height = 78 },
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical,
                ShowCellToolTips = false
            };

            dgvReturns.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvReturns.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvReturns.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvReturns.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvReturns.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvReturns.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgvReturns.DefaultCellStyle.BackColor = SoftBack;
            dgvReturns.DefaultCellStyle.ForeColor = PrimaryText;
            dgvReturns.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            dgvReturns.DefaultCellStyle.SelectionBackColor = SoftBack;
            dgvReturns.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvReturns.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            dgvReturns.RowsDefaultCellStyle.BackColor = SoftBack;
            dgvReturns.RowsDefaultCellStyle.SelectionBackColor = SoftBack;
            dgvReturns.RowsDefaultCellStyle.SelectionForeColor = PrimaryText;

            dgvReturns.Columns.Add("ReturnId", "RETURN\r\nID");
            dgvReturns.Columns.Add("MemberName", "MEMBER NAME");
            dgvReturns.Columns.Add("BookTitle", "BOOK TITLE");
            dgvReturns.Columns.Add("ReturnDate", "RETURN\r\nDATE");
            dgvReturns.Columns.Add("Condition", "CONDITION");
            dgvReturns.Columns.Add("Status", "STATUS");
            dgvReturns.Columns.Add("Actions", "ACTIONS");

            ApplyColumnStyle();

            dgvReturns.CellPainting += DgvReturns_CellPainting;
            dgvReturns.MouseClick += DgvReturns_MouseClick;
            dgvReturns.MouseMove += DgvReturns_MouseMove;
            dgvReturns.SelectionChanged += (s, e) => ClearGridSelection();

            lblFooter = new Label
            {
                Text = "SHOWING 0 OF 0 RECORDS",
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            pagerPanel = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(190, 36)
            };
            BuildPager();

            tableCard.Controls.Add(searchPanel);
            tableCard.Controls.Add(lblTableTitle);
            tableCard.Controls.Add(dgvReturns);
            tableCard.Controls.Add(lblFooter);
            tableCard.Controls.Add(pagerPanel);
        }

        private void BtnProcessReturn_Click(object? sender, EventArgs e)
        {
            ShowReturnDialog(null);
        }

        private void ShowReturnDialog(ReturnRow? row)
        {
            LoadReturnLoanOptions();

            ReturnDialogData? existing = row == null ? null : new ReturnDialogData
            {
                ReturnId = row.ReturnId,
                BorrowId = row.BorrowId,
                MemberName = row.MemberName,
                BookTitle = row.BookTitle,
                ReturnDate = row.ReturnDate,
                Condition = row.Condition,
                Status = row.Status,
                DaysOverdue = row.DaysOverdue,
                FineAmount = row.FineAmount
            };

            using ProcessReturnDialog dialog = new ProcessReturnDialog(returnLoanOptions, existing);

            if (ShowReturnDialogWithOverlay(dialog) != DialogResult.OK)
                return;

            try
            {
                if (row == null)
                    InsertReturnRecord(dialog.ReturnData);
                else
                    UpdateReturnRecord(dialog.ReturnData);

                LoadReturnData();
                ClearGridSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Return record was not saved.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private DialogResult ShowReturnDialogWithOverlay(ProcessReturnDialog dialog)
        {
            Form? owner = FindForm();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowInTaskbar = false;
            dialog.TopMost = false;

            if (owner == null)
            {
                Rectangle screen = Screen.FromControl(this).WorkingArea;
                dialog.StartPosition = FormStartPosition.Manual;
                dialog.Location = new Point(
                    screen.Left + Math.Max(0, (screen.Width - dialog.Width) / 2),
                    screen.Top + Math.Max(0, (screen.Height - dialog.Height) / 2)
                );

                return dialog.ShowDialog(this);
            }

            return dialog.ShowDialog(owner);
        }

        private void LoadReturnData()
        {
            returnRecords.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureReturnRecordsTable(conn);

                const string query = @"
SELECT ReturnID, BorrowID, MemberName, BookTitle, ReturnDate, BookCondition, ReturnStatus, DaysOverdue, FineAmount
FROM dbo.ReturnRecords
WHERE IsArchived = 0
  AND ClientID = @ClientID
ORDER BY ReturnID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    returnRecords.Add(new ReturnRow
                    {
                        ReturnId = Convert.ToInt32(reader["ReturnID"]),
                        BorrowId = reader["BorrowID"] == DBNull.Value ? null : Convert.ToInt32(reader["BorrowID"]),
                        MemberName = Convert.ToString(reader["MemberName"]) ?? "",
                        BookTitle = Convert.ToString(reader["BookTitle"]) ?? "",
                        ReturnDate = Convert.ToDateTime(reader["ReturnDate"]),
                        Condition = Convert.ToString(reader["BookCondition"]) ?? "GOOD",
                        Status = Convert.ToString(reader["ReturnStatus"]) ?? "ON-TIME",
                        DaysOverdue = reader["DaysOverdue"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DaysOverdue"]),
                        FineAmount = reader["FineAmount"] == DBNull.Value ? 0M : Convert.ToDecimal(reader["FineAmount"])
                    });
                }

                currentPage = 1;
                ApplySearch(GetCurrentSearchText());
                UpdateMetricCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load return records.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RebuildReturnRows()
        {
            ApplySearch(GetCurrentSearchText());
        }

        private void RenderReturnPage()
        {
            dgvReturns.Rows.Clear();

            List<ReturnRow> pageRows = filteredReturnRecords
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (ReturnRow row in pageRows)
            {
                int rowIndex = dgvReturns.Rows.Add(
                    "RTN-" + row.ReturnId.ToString("0000"),
                    row.MemberName,
                    InsertBookLineBreak(row.BookTitle),
                    row.ReturnDate.ToString("MMM dd,\nyyyy"),
                    row.Condition.ToUpperInvariant(),
                    row.Status.Replace("-", "-\n"),
                    ""
                );

                dgvReturns.Rows[rowIndex].Tag = row;
            }

            UpdateFooter(filteredReturnRecords.Count, pageRows.Count);
            UpdatePagerButtons();
            ClearGridSelection();
        }

        private void LoadReturnLoanOptions()
        {
            returnLoanOptions.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                const string query = @"
SELECT b.BorrowID, u.FullName, b.BookTitle, b.DueDate
FROM dbo.BorrowingRecords b
INNER JOIN dbo.Users u ON b.MemberID = u.UserID
WHERE b.ClientID = @ClientID
  AND b.IsArchived = 0
  AND b.Status <> 'RETURNED'
ORDER BY b.BorrowID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    returnLoanOptions.Add(new ReturnLoanOption(
                        Convert.ToInt32(reader["BorrowID"]),
                        Convert.ToString(reader["FullName"]) ?? "",
                        Convert.ToString(reader["BookTitle"]) ?? "",
                        Convert.ToDateTime(reader["DueDate"])
                    ));
                }
            }
            catch
            {
                returnLoanOptions.Clear();
            }
        }

        private void InsertReturnRecord(ReturnDialogData data)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();

            EnsureReturnRecordsTable(conn);

            using SqlTransaction tx = conn.BeginTransaction();

            try
            {
                const string insertQuery = @"
INSERT INTO dbo.ReturnRecords
    (ClientID, BorrowID, MemberName, BookTitle, ReturnDate, BookCondition, ReturnStatus, DaysOverdue, FineAmount, CreatedAt, IsArchived)
VALUES
    (@ClientID, @BorrowID, @MemberName, @BookTitle, @ReturnDate, @BookCondition, @ReturnStatus, @DaysOverdue, @FineAmount, SYSUTCDATETIME(), 0);";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, tx))
                {
                    AddReturnParameters(cmd, data);
                    cmd.ExecuteNonQuery();
                }

                if (data.BorrowId.HasValue)
                {
                    string? bookId = null;

                    const string getBookQuery = @"
SELECT BookID
FROM dbo.BorrowingRecords
WHERE BorrowID = @BorrowID
  AND ClientID = @ClientID;";

                    using (SqlCommand cmd = new SqlCommand(getBookQuery, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@BorrowID", data.BorrowId.Value);
                        cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                        object? result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                            bookId = Convert.ToString(result);
                    }

                    if (!string.IsNullOrWhiteSpace(bookId))
                    {
                        const string increaseBookQtyQuery = @"
UPDATE dbo.Books
SET Quantity = Quantity + 1,
    UpdatedAt = SYSUTCDATETIME()
WHERE BookID = @BookID
  AND ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0;";

                        using (SqlCommand cmd = new SqlCommand(increaseBookQtyQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@BookID", bookId);
                            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                            cmd.ExecuteNonQuery();
                        }

                        const string updateBookStatusQuery = @"
UPDATE dbo.Books
SET Status =
    CASE
        WHEN Quantity <= 0 THEN 'Out of Stock'
        WHEN Quantity <= 2 THEN 'Low Stock'
        ELSE 'In Stock'
    END,
    UpdatedAt = SYSUTCDATETIME()
WHERE BookID = @BookID
  AND ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0;";

                        using (SqlCommand cmd = new SqlCommand(updateBookStatusQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@BookID", bookId);
                            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    const string updateBorrowQuery = @"
UPDATE dbo.BorrowingRecords
SET Status = 'RETURNED'
WHERE BorrowID = @BorrowID
  AND ClientID = @ClientID;";

                    using (SqlCommand cmd = new SqlCommand(updateBorrowQuery, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@BorrowID", data.BorrowId.Value);
                        cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                        cmd.ExecuteNonQuery();
                    }
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        private void UpdateReturnRecord(ReturnDialogData data)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();
            EnsureReturnRecordsTable(conn);

            const string query = @"
UPDATE dbo.ReturnRecords
SET BorrowID = @BorrowID,
    MemberName = @MemberName,
    BookTitle = @BookTitle,
    ReturnDate = @ReturnDate,
    BookCondition = @BookCondition,
    ReturnStatus = @ReturnStatus,
    DaysOverdue = @DaysOverdue,
    FineAmount = @FineAmount,
    UpdatedAt = SYSUTCDATETIME()
WHERE ReturnID = @ReturnID
  AND ClientID = @ClientID
  AND IsArchived = 0;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ReturnID", data.ReturnId);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            AddReturnParameters(cmd, data);
            cmd.ExecuteNonQuery();
        }

        private void AddReturnParameters(SqlCommand cmd, ReturnDialogData data)
        {
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            cmd.Parameters.AddWithValue("@BorrowID", data.BorrowId.HasValue ? data.BorrowId.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MemberName", data.MemberName);
            cmd.Parameters.AddWithValue("@BookTitle", data.BookTitle);
            cmd.Parameters.AddWithValue("@ReturnDate", data.ReturnDate.Date);
            cmd.Parameters.AddWithValue("@BookCondition", data.Condition);
            cmd.Parameters.AddWithValue("@ReturnStatus", data.Status);
            cmd.Parameters.AddWithValue("@DaysOverdue", data.DaysOverdue);
            cmd.Parameters.AddWithValue("@FineAmount", data.FineAmount);
        }

        private void EnsureReturnRecordsTable(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.ReturnRecords', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReturnRecords
    (
        ReturnID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        BorrowID INT NULL,
        MemberName NVARCHAR(150) NOT NULL,
        BookTitle NVARCHAR(250) NOT NULL,
        ReturnDate DATE NOT NULL,
        BookCondition NVARCHAR(40) NOT NULL,
        ReturnStatus NVARCHAR(40) NOT NULL,
        DaysOverdue INT NOT NULL DEFAULT 0,
        FineAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.ReturnRecords', 'ClientID') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD ClientID INT NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void ApplySearch(string keyword)
        {
            keyword = keyword.Trim().ToLowerInvariant();

            filteredReturnRecords = returnRecords
                .Where(row =>
                    string.IsNullOrWhiteSpace(keyword) ||
                    row.ReturnId.ToString().Contains(keyword) ||
                    row.MemberName.ToLowerInvariant().Contains(keyword) ||
                    row.BookTitle.ToLowerInvariant().Contains(keyword) ||
                    row.Condition.ToLowerInvariant().Contains(keyword) ||
                    row.Status.ToLowerInvariant().Contains(keyword))
                .ToList();

            totalPages = Math.Max(1, (int)Math.Ceiling(filteredReturnRecords.Count / (double)PageSize));

            if (currentPage > totalPages)
                currentPage = totalPages;

            if (currentPage < 1)
                currentPage = 1;

            RenderReturnPage();
        }

        private string GetCurrentSearchText()
        {
            if (txtSearch == null || txtSearch.Text == SearchPlaceholder)
                return "";

            return txtSearch.Text;
        }

        private void UpdateFooter(int totalFilteredCount, int visiblePageCount)
        {
            if (lblFooter == null)
                return;

            if (returnRecords.Count == 0 || totalFilteredCount == 0)
            {
                lblFooter.Text = $"SHOWING 0 OF {returnRecords.Count:N0} RECORDS";
                return;
            }

            int start = ((currentPage - 1) * PageSize) + 1;
            int end = Math.Min(start + visiblePageCount - 1, totalFilteredCount);

            lblFooter.Text = $"SHOWING {start:N0}-{end:N0} OF {totalFilteredCount:N0} RECORDS";
        }

        private void UpdateMetricCards()
        {
            int total = returnRecords.Count;
            int late = returnRecords.Count(row => row.Status.Contains("LATE", StringComparison.OrdinalIgnoreCase));
            int damaged = returnRecords.Count(row =>
                row.Condition.Equals("DAMAGED", StringComparison.OrdinalIgnoreCase) ||
                row.Condition.Equals("LOST", StringComparison.OrdinalIgnoreCase));
            int onTime = returnRecords.Count(row => row.Status.Contains("ON-TIME", StringComparison.OrdinalIgnoreCase));
            decimal accuracy = total == 0 ? 100M : (decimal)onTime / total * 100M;

            SetMetricValue(cardReturns, total.ToString("N0"));
            SetMetricValue(cardLate, late.ToString("N0"));
            SetMetricValue(cardDamaged, damaged.ToString("N0"));
            SetMetricValue(cardAccuracy, $"{accuracy:0.0}%");
        }

        private void SetMetricValue(Panel card, string value)
        {
            if (card.Tag is Control[] controls && controls.Length > 1 && controls[1] is Label label)
                label.Text = value;
        }

        private string InsertBookLineBreak(string text)
        {
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (words.Length <= 2)
                return text;

            int mid = words.Length / 2;
            return string.Join(" ", words.Take(mid)) + "\n" + string.Join(" ", words.Skip(mid));
        }

        private void ApplyColumnStyle()
        {
            dgvReturns.Columns["ReturnId"].FillWeight = 13;
            dgvReturns.Columns["MemberName"].FillWeight = 18;
            dgvReturns.Columns["BookTitle"].FillWeight = 24;
            dgvReturns.Columns["ReturnDate"].FillWeight = 14;
            dgvReturns.Columns["Condition"].FillWeight = 13;
            dgvReturns.Columns["Status"].FillWeight = 13;
            dgvReturns.Columns["Actions"].FillWeight = 10;

            dgvReturns.Columns["Actions"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvReturns.Columns["Actions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvReturns.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void DgvReturns_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0)
                return;

            string col = dgv.Columns[e.ColumnIndex].Name;
            string text = e.FormattedValue?.ToString() ?? "";

            if (col == "ReturnId")
            {
                e.PaintBackground(e.CellBounds, true);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Consolas", 9.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 18, e.CellBounds.Y + 27, e.CellBounds.Width - 24, 24),
                    AccentDeep,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "MemberName")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle avatar = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 23, 34, 34);
                bool gray = text.Contains("Amara") || text.Contains("Elena");

                using (SolidBrush b = new SolidBrush(gray ? ColorTranslator.FromHtml("#DDE4E6") : AccentSoft))
                    e.Graphics.FillEllipse(b, avatar);

                TextRenderer.DrawText(
                    e.Graphics,
                    GetInitials(text),
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    avatar,
                    gray ? PrimaryText : AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 58, e.CellBounds.Y + 25, e.CellBounds.Width - 64, 28),
                    PrimaryText,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (col == "Condition")
            {
                e.PaintBackground(e.CellBounds, true);

                bool damaged = text == "DAMAGED";
                Color bg = damaged ? ColorTranslator.FromHtml("#FDE8E4") : AccentSoft;
                Color fg = damaged ? AccentDanger : AccentDeep;

                Size sz = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 28, sz.Width + 22, 24);

                using (SolidBrush b = new SolidBrush(bg))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    badge,
                    fg,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                bool late = text.Contains("LATE");
                Color fg = late ? AccentDanger : AccentDeep;

                Rectangle dot = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 36, 6, 6);
                using (SolidBrush b = new SolidBrush(fg))
                    e.Graphics.FillEllipse(b, dot);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 24, e.CellBounds.Y + 22, e.CellBounds.Width - 30, e.CellBounds.Height - 20),
                    fg,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                var actionBounds = GetActionBounds(e.CellBounds);

                TextRenderer.DrawText(
                    e.Graphics,
                    "✎",
                    new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                    actionBounds.Edit,
                    AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    "📥",
                    new Font("Segoe UI Symbol", 11F, FontStyle.Bold),
                    actionBounds.Archive,
                    Color.Firebrick,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private (Rectangle Edit, Rectangle Archive) GetActionBounds(Rectangle cellBounds)
        {
            const int iconSize = 24;
            const int gap = 12;

            int totalWidth = (iconSize * 2) + gap;
            int left = cellBounds.X + Math.Max(0, (cellBounds.Width - totalWidth) / 2);
            int top = cellBounds.Y + Math.Max(0, (cellBounds.Height - iconSize) / 2);

            return (
                new Rectangle(left, top, iconSize, iconSize),
                new Rectangle(left + iconSize + gap, top, iconSize, iconSize)
            );
        }

        private void DgvReturns_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            if (dgv.Columns[hit.ColumnIndex].Name != "Actions")
                return;

            if (dgv.Rows[hit.RowIndex].Tag is not ReturnRow row)
                return;

            Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
            if (!cellBounds.Contains(e.Location))
                return;

            var actionBounds = GetActionBounds(cellBounds);

            if (actionBounds.Edit.Contains(e.Location) || e.X < cellBounds.Left + (cellBounds.Width / 2))
                EditReturnRecord(row);
            else if (actionBounds.Archive.Contains(e.Location) || e.X >= cellBounds.Left + (cellBounds.Width / 2))
                ArchiveReturnRecord(row);
        }

        private void DgvReturns_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
            bool overActions = false;

            if (hit.RowIndex >= 0 &&
                hit.ColumnIndex >= 0 &&
                dgv.Columns[hit.ColumnIndex].Name == "Actions")
            {
                Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
                overActions = cellBounds.Contains(e.Location);
            }

            dgv.Cursor = overActions ? Cursors.Hand : Cursors.Default;
        }

        private void EditReturnRecord(ReturnRow row)
        {
            ShowReturnDialog(row);
        }

        private void ArchiveReturnRecord(ReturnRow row)
        {
            DialogResult confirm = MessageBox.Show(
                "Archive this return record?",
                "Archive Return Record",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureReturnRecordsTable(conn);

                const string query = @"
UPDATE dbo.ReturnRecords
SET IsArchived = 1,
    UpdatedAt = SYSUTCDATETIME()
WHERE ReturnID = @ReturnID
  AND ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ReturnID", row.ReturnId);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                cmd.ExecuteNonQuery();

                LoadReturnData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Return record was not archived.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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
                RenderReturnPage();
            };

            btnNextPage.Click += (s, e) =>
            {
                if (currentPage >= totalPages) return;
                currentPage++;
                RenderReturnPage();
            };

            btnPage1.Click += (s, e) => GoToReturnPage(btnPage1);
            btnPage2.Click += (s, e) => GoToReturnPage(btnPage2);
            btnPage3.Click += (s, e) => GoToReturnPage(btnPage3);

            pagerPanel.Controls.Add(btnPrevPage);
            pagerPanel.Controls.Add(btnPage1);
            pagerPanel.Controls.Add(btnPage2);
            pagerPanel.Controls.Add(btnPage3);
            pagerPanel.Controls.Add(btnNextPage);

            UpdatePagerButtons();
        }

        private void GoToReturnPage(Button button)
        {
            if (button.Tag == null)
                return;

            if (int.TryParse(button.Tag.ToString(), out int page))
            {
                currentPage = page;
                RenderReturnPage();
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

            btn.FlatAppearance.BorderColor = LineSoft;
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

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 26;
            int width = Math.Max(980, canvas.ClientSize.Width - margin * 2);

            lblTitle.Location = new Point(margin, 34);
            lblSubtitle.Location = new Point(margin, 78);

            btnProcessReturn.Bounds = new Rectangle(width - 230 + margin, 44, 230, 52);

            int cardTop = 132;
            int cardHeight = 170;
            int cardWidth = (width - (gap * 3)) / 4;

            cardReturns.Bounds = new Rectangle(margin, cardTop, cardWidth, cardHeight);
            cardLate.Bounds = new Rectangle(cardReturns.Right + gap, cardTop, cardWidth, cardHeight);
            cardDamaged.Bounds = new Rectangle(cardLate.Right + gap, cardTop, cardWidth, cardHeight);
            cardAccuracy.Bounds = new Rectangle(cardDamaged.Right + gap, cardTop, cardWidth, cardHeight);

            LayoutMetricCard(cardReturns);
            LayoutMetricCard(cardLate);
            LayoutMetricCard(cardDamaged);
            LayoutMetricCard(cardAccuracy);

            tableCard.Bounds = new Rectangle(margin, 340, width, 560);

            // Title on LEFT
            lblTableTitle.Location = new Point(26, 28);

            // Search bar on RIGHT
            searchPanel.Bounds = new Rectangle(tableCard.Width - 306, 22, 280, 38);

            // Keep inner layout
            searchPanel.Controls[0].Location = new Point(12, 7);
            txtSearch.Location = new Point(42, 9);
            txtSearch.Width = searchPanel.Width - 54;

            dgvReturns.Location = new Point(0, 86);
            dgvReturns.Size = new Size(tableCard.Width, 376);

            lblFooter.Location = new Point(26, tableCard.Height - 44);
            pagerPanel.Location = new Point(tableCard.Width - pagerPanel.Width - 28, tableCard.Height - 52);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 30);
        }

        private void LayoutMetricCard(Panel card)
        {
            if (card.Tag is not Control[] c) return;

            if (card == cardAccuracy)
            {
                c[1].Location = new Point(28, 38);  // value
                c[2].Location = new Point(28, 84);  // label
                return;
            }

            c[0].Location = new Point(24, 24);   // icon
            c[1].Location = new Point(24, 72);   // value
            c[2].Location = new Point(24, 112);  // label
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

        private void ClearGridSelection()
        {
            if (dgvReturns == null) return;
            dgvReturns.ClearSelection();
            dgvReturns.CurrentCell = null;
        }

        private string GetInitials(string name)
        {
            string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
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
