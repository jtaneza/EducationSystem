using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class BorrowingForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentSoft = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");
        private readonly Color AccentDangerBg = ColorTranslator.FromHtml("#F7816D");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Button btnNewBorrow = null!;

        private Panel cardActiveLoans = null!;
        private Panel cardOverdue = null!;
        private Panel cardDueToday = null!;
        private Panel cardBorrowingRate = null!;

        private Panel tableCard = null!;
        private Panel searchPanel = null!;
        private TextBox txtSearch = null!;
        private DataGridView dgvBorrowing = null!;
        private Label lblFooter = null!;
        private Panel pagerPanel = null!;

        private readonly List<BorrowRowItem> allBorrowRows = new List<BorrowRowItem>();
        private List<BorrowRowItem> filteredBorrowRows = new List<BorrowRowItem>();
        private int currentPage = 1;
        private int totalPages = 1;
        private const int PageSize = 4;

        private Button btnPrevPage = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNextPage = null!;

        public BorrowingForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            BuildUI();

            Load += (s, e) =>
            {
                AdjustLayout();
                ClearGridSelection();

                BeginInvoke(new Action(() =>
                {
                    LoadBorrowingData();
                    ClearGridSelection();
                }));
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
                Text = "Borrowing Management",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubtitle = new Label
            {
                Text = "Track, monitor, and update active book loans for School Library.",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            btnNewBorrow = new Button
            {
                Text = "+  New Borrow",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNewBorrow.FlatAppearance.BorderSize = 0;
            btnNewBorrow.Click += BtnNewBorrow_Click;

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubtitle);
            canvas.Controls.Add(btnNewBorrow);

            BuildCards();
            BuildTable();
        }


        /*
         * IMPORTANT:
         * The member dropdown is inside IssueBorrowDialog, not this form.
         * In IssueBorrowDialog, the member ComboBox query must use this filter:
         *
         * SELECT UserID, FullName, Role
         * FROM dbo.Users
         * WHERE ClientID = @ClientID
         *   AND ISNULL(IsArchived, 0) = 0
         *   AND ISNULL(IsActive, 1) = 1
         *   AND Role IN ('Student', 'Teacher', 'Member')
         * ORDER BY FullName;
         *
         * And every INSERT/UPDATE to BorrowingRecords must save:
         * @ClientID = ClientSession.ClientId
         * @MemberID = selected member UserID
         * @MemberName = selected member FullName
         */

        private void BtnNewBorrow_Click(object? sender, EventArgs e)
        {
            using IssueBorrowDialog dlg = new IssueBorrowDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
                LoadBorrowingData();
        }

        private void BuildCards()
        {
            cardActiveLoans = CreateRoundedCard();
            cardOverdue = CreateRoundedCard();
            cardDueToday = CreateRoundedCard();
            cardBorrowingRate = CreateRoundedCard();

            canvas.Controls.Add(cardActiveLoans);
            canvas.Controls.Add(cardOverdue);
            canvas.Controls.Add(cardDueToday);
            canvas.Controls.Add(cardBorrowingRate);

            BuildMetricCard(cardActiveLoans, "📖", "+12% THIS WEEK", "ACTIVE LOANS", "0", AccentSoft, AccentDeep);
            BuildMetricCard(cardOverdue, "⟳", "REQUIRES ATTENTION", "OVERDUE", "0", AccentDangerBg, AccentDanger);
            BuildMetricCard(cardDueToday, "📅", "TODAY'S DEADLINE", "DUE TODAY", "0", AccentSoft, AccentDeep);
            BuildMetricCard(cardBorrowingRate, "↗", "+2.4% FROM LAST WEEK", "BORROWING RATE", "84.5%", AccentSoft, AccentDeep);
        }

        private void BuildMetricCard(Panel card, string iconText, string topText, string labelText, string valueText, Color iconBack, Color accentColor)
        {
            Panel iconBox = new Panel
            {
                BackColor = iconBack,
                Size = new Size(48, 48)
            };
            iconBox.Paint += RoundedPanelPaint;

            Label icon = new Label
            {
                Text = iconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 18F),
                BackColor = Color.Transparent,
                ForeColor = accentColor
            };
            iconBox.Controls.Add(icon);

            Label top = new Label
            {
                Text = topText,
                AutoSize = false,
                Size = new Size(120, 32),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = accentColor,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopRight
            };

            Label label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            Label value = new Label
            {
                Text = valueText,
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            card.Controls.Add(iconBox);
            card.Controls.Add(top);
            card.Controls.Add(label);
            card.Controls.Add(value);

            card.Tag = new Control[] { iconBox, top, label, value };
        }

        private void BuildTable()
        {
            tableCard = CreateRoundedCard();
            canvas.Controls.Add(tableCard);

            searchPanel = new Panel { BackColor = HeaderBack };
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
                Text = "Search active loans...",
                BorderStyle = BorderStyle.None,
                BackColor = HeaderBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 11F)
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search active loans...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = PrimaryText;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search active loans...";
                    txtSearch.ForeColor = SecondaryText;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                if (!txtSearch.Focused && txtSearch.Text == "Search active loans...") return;
                currentPage = 1;
                ApplySearch(txtSearch.Text);
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(txtSearch);

            dgvBorrowing = new DataGridView
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 64,
                RowTemplate = { Height = 88 },
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical,
                ShowCellToolTips = false
            };

            dgvBorrowing.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvBorrowing.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgvBorrowing.DefaultCellStyle.BackColor = CardBack;
            dgvBorrowing.DefaultCellStyle.ForeColor = PrimaryText;
            dgvBorrowing.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            dgvBorrowing.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvBorrowing.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvBorrowing.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            dgvBorrowing.RowsDefaultCellStyle.BackColor = CardBack;
            dgvBorrowing.RowsDefaultCellStyle.SelectionBackColor = CardBack;
            dgvBorrowing.RowsDefaultCellStyle.SelectionForeColor = PrimaryText;

            dgvBorrowing.Columns.Add("BorrowId", "BORROW\r\nID");
            dgvBorrowing.Columns.Add("MemberName", "MEMBER NAME");
            dgvBorrowing.Columns.Add("BookTitle", "BOOK TITLE");
            dgvBorrowing.Columns.Add("IssueDate", "ISSUE\r\nDATE");
            dgvBorrowing.Columns.Add("DueDate", "DUE DATE");
            dgvBorrowing.Columns.Add("Status", "STATUS");
            dgvBorrowing.Columns.Add("Actions", "ACTIONS");

            ApplyColumnStyle();

            dgvBorrowing.CellPainting += DgvBorrowing_CellPainting;
            dgvBorrowing.MouseClick += DgvBorrowing_MouseClick;
            dgvBorrowing.MouseMove += DgvBorrowing_MouseMove;
            dgvBorrowing.SelectionChanged += (s, e) => ClearGridSelection();

            lblFooter = new Label
            {
                Text = "Showing 0 entries",
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

            tableCard.Controls.Add(searchPanel);
            tableCard.Controls.Add(dgvBorrowing);
            tableCard.Controls.Add(lblFooter);
            tableCard.Controls.Add(pagerPanel);
        }

        private void LoadBorrowingData()
        {
            allBorrowRows.Clear();

            int activeCount = 0;
            int overdueCount = 0;
            int dueTodayCount = 0;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureBorrowingClientSchema(conn);

                string query = @"
SELECT 
    b.BorrowID,
    b.MemberID,
    COALESCE(NULLIF(b.MemberName, ''), u.FullName, 'Unknown Member') AS MemberName,
    ISNULL(u.Role, 'Member') AS Role,
    b.BookTitle,
    b.IssueDate,
    b.DueDate,
    ISNULL(b.Status, 'ACTIVE') AS Status
FROM dbo.BorrowingRecords b
LEFT JOIN dbo.Users u
    ON b.MemberID = u.UserID
   AND u.ClientID = b.ClientID
WHERE b.ClientID = @ClientID
  AND ISNULL(b.IsArchived, 0) = 0
  AND UPPER(ISNULL(b.Status, 'ACTIVE')) IN ('ACTIVE', 'BORROWED', 'IN PROGRESS')
ORDER BY b.IssueDate DESC, b.BorrowID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int borrowId = Convert.ToInt32(reader["BorrowID"]);
                    string memberName = reader["MemberName"]?.ToString() ?? "Unknown Member";
                    string role = reader["Role"]?.ToString() ?? "Member";
                    string bookTitle = reader["BookTitle"]?.ToString() ?? "";
                    DateTime issueDate = reader["IssueDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["IssueDate"]);
                    DateTime dueDate = reader["DueDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["DueDate"]);
                    string status = reader["Status"]?.ToString() ?? "ACTIVE";

                    string displayStatus = dueDate.Date < DateTime.Today ? "OVERDUE" : status.ToUpperInvariant();

                    if (displayStatus.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase) ||
                        displayStatus.Equals("BORROWED", StringComparison.OrdinalIgnoreCase) ||
                        displayStatus.Equals("IN PROGRESS", StringComparison.OrdinalIgnoreCase))
                    {
                        activeCount++;
                    }

                    if (displayStatus.Equals("OVERDUE", StringComparison.OrdinalIgnoreCase))
                        overdueCount++;

                    if (dueDate.Date == DateTime.Today)
                        dueTodayCount++;

                    allBorrowRows.Add(new BorrowRowItem
                    {
                        BorrowId = borrowId,
                        MemberName = memberName,
                        Role = role,
                        BookTitle = bookTitle,
                        IssueDate = issueDate,
                        DueDate = dueDate,
                        Status = displayStatus
                    });
                }

                SetMetricValue(cardActiveLoans, activeCount.ToString());
                SetMetricValue(cardOverdue, overdueCount.ToString());
                SetMetricValue(cardDueToday, dueTodayCount.ToString());

                ApplySearch(GetCurrentSearchText());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
    "Failed to load borrowing records.\n\n" + ex.Message,
    "Database Error",
    MessageBoxButtons.OK,
    MessageBoxIcon.Error
);
            }

            ClearGridSelection();
        }

        private int GetCurrentClientId()
        {
            if (ClientSession.ClientId != null && int.TryParse(ClientSession.ClientId.ToString(), out int clientId))
                return clientId;

            return 0;
        }

        private void EnsureBorrowingClientSchema(SqlConnection conn)
        {
            const string query = @"
IF COL_LENGTH('dbo.BorrowingRecords', 'ClientID') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD ClientID INT NULL;

IF COL_LENGTH('dbo.BorrowingRecords', 'MemberID') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD MemberID INT NULL;

IF COL_LENGTH('dbo.BorrowingRecords', 'MemberName') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD MemberName NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.BorrowingRecords', 'IsArchived') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_BorrowingRecords_IsArchived_Client DEFAULT 0;

IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
    ALTER TABLE dbo.Users ADD ClientID INT NULL;

IF COL_LENGTH('dbo.Users', 'IsArchived') IS NULL
    ALTER TABLE dbo.Users ADD IsArchived BIT NOT NULL CONSTRAINT DF_Users_IsArchived_Borrow DEFAULT 0;

IF COL_LENGTH('dbo.Users', 'IsActive') IS NULL
    ALTER TABLE dbo.Users ADD IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive_Borrow DEFAULT 1;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void SetMetricValue(Panel card, string value)
        {
            if (card.Tag is not Control[] c) return;
            ((Label)c[3]).Text = value;
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

        private void DgvBorrowing_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            if (dgv.Columns[hit.ColumnIndex].Name != "Actions")
                return;

            int borrowId = dgv.Rows[hit.RowIndex].Tag is int id ? id : 0;
            if (borrowId == 0) return;

            Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
            if (!cellBounds.Contains(e.Location))
                return;

            var actionBounds = GetActionBounds(cellBounds);

            if (actionBounds.Edit.Contains(e.Location) || e.X < cellBounds.Left + (cellBounds.Width / 2))
                EditBorrowRecord(borrowId);
            else if (actionBounds.Archive.Contains(e.Location) || e.X >= cellBounds.Left + (cellBounds.Width / 2))
                ArchiveBorrowRecord(borrowId);
        }

        private void DgvBorrowing_MouseMove(object? sender, MouseEventArgs e)
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

        private void EditBorrowRecord(int borrowId)
        {
            try
            {
                using IssueBorrowDialog dlg = new IssueBorrowDialog();
                dlg.LoadForEdit(borrowId);

                if (dlg.ShowDialog(this) == DialogResult.OK)
                    LoadBorrowingData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Borrowing record could not be opened for editing.\n\n" + ex.Message,
                    "Edit Borrow Record",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ArchiveBorrowRecord(int borrowId)
        {
            DialogResult confirm = MessageBox.Show(
                "Archive this borrowing record?",
                "Archive Borrow Record",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                const string query = @"
UPDATE dbo.BorrowingRecords
SET IsArchived = 1
WHERE BorrowID = @BorrowID
  AND ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BorrowID", borrowId);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                cmd.ExecuteNonQuery();

                LoadBorrowingData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Borrowing record was not archived.\n\n" + ex.Message,
                    "Archive Borrow Record",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ApplySearch(string keyword)
        {
            keyword = keyword.Trim().ToLowerInvariant();

            filteredBorrowRows = allBorrowRows
                .Where(row =>
                    string.IsNullOrWhiteSpace(keyword) ||
                    row.BorrowId.ToString().Contains(keyword) ||
                    row.MemberName.ToLowerInvariant().Contains(keyword) ||
                    row.Role.ToLowerInvariant().Contains(keyword) ||
                    row.BookTitle.ToLowerInvariant().Contains(keyword) ||
                    row.Status.ToLowerInvariant().Contains(keyword))
                .ToList();

            totalPages = Math.Max(1, (int)Math.Ceiling(filteredBorrowRows.Count / (double)PageSize));

            if (currentPage > totalPages)
                currentPage = totalPages;

            if (currentPage < 1)
                currentPage = 1;

            RenderBorrowingPage();
        }

        private string GetCurrentSearchText()
        {
            if (txtSearch == null || txtSearch.Text == "Search active loans...")
                return "";

            return txtSearch.Text;
        }

        private void RenderBorrowingPage()
        {
            dgvBorrowing.Rows.Clear();

            List<BorrowRowItem> pageRows = filteredBorrowRows
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (BorrowRowItem row in pageRows)
            {
                int rowIndex = dgvBorrowing.Rows.Add(
                    "#LIB-\n" + row.BorrowId.ToString("00000"),
                    row.MemberName + "\n" + row.Role,
                    row.BookTitle,
                    row.IssueDate.ToString("MMM dd,\nyyyy"),
                    row.DueDate.ToString("MMM dd,\nyyyy"),
                    row.Status,
                    ""
                );

                dgvBorrowing.Rows[rowIndex].Tag = row.BorrowId;
            }

            if (filteredBorrowRows.Count == 0)
            {
                lblFooter.Text = "Showing 0 entries";
            }
            else
            {
                int start = ((currentPage - 1) * PageSize) + 1;
                int end = Math.Min(start + pageRows.Count - 1, filteredBorrowRows.Count);
                lblFooter.Text = $"Showing {start} - {end} of {filteredBorrowRows.Count} entries";
            }

            UpdatePagerButtons();
            ClearGridSelection();
        }

        private void ApplyColumnStyle()
        {
            dgvBorrowing.Columns["BorrowId"].FillWeight = 13;
            dgvBorrowing.Columns["MemberName"].FillWeight = 21;
            dgvBorrowing.Columns["BookTitle"].FillWeight = 24;
            dgvBorrowing.Columns["IssueDate"].FillWeight = 12;
            dgvBorrowing.Columns["DueDate"].FillWeight = 12;
            dgvBorrowing.Columns["Status"].FillWeight = 13;
            dgvBorrowing.Columns["Actions"].FillWeight = 10;

            dgvBorrowing.Columns["Actions"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBorrowing.Columns["Actions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvBorrowing.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void DgvBorrowing_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0)
                return;

            string col = dgv.Columns[e.ColumnIndex].Name;
            string text = e.FormattedValue?.ToString() ?? "";

            if (col == "BorrowId")
            {
                e.PaintBackground(e.CellBounds, true);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Consolas", 9.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 18, e.CellBounds.Y + 24, e.CellBounds.Width - 24, e.CellBounds.Height - 24),
                    AccentDeep,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

                e.Handled = true;
            }
            else if (col == "MemberName")
            {
                e.PaintBackground(e.CellBounds, true);

                string[] parts = text.Split('\n');
                string name = parts.Length > 0 ? parts[0] : "";
                string detail = parts.Length > 1 ? parts[1] : "";

                Rectangle avatar = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 27, 34, 34);

                using (SolidBrush b = new SolidBrush(text.Contains("OVERDUE") ? ColorTranslator.FromHtml("#FDE8E4") : AccentSoft))
                    e.Graphics.FillEllipse(b, avatar);

                TextRenderer.DrawText(
                    e.Graphics,
                    GetInitials(name),
                    new Font("Segoe UI", 9F, FontStyle.Bold),
                    avatar,
                    AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 58, e.CellBounds.Y + 22, e.CellBounds.Width - 66, 28),
                    PrimaryText,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                TextRenderer.DrawText(
                    e.Graphics,
                    detail,
                    new Font("Segoe UI", 8.5F),
                    new Rectangle(e.CellBounds.X + 58, e.CellBounds.Y + 47, e.CellBounds.Width - 66, 24),
                    SecondaryText,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                bool overdue = text == "OVERDUE";
                Color bg = overdue ? AccentDangerBg : AccentSoft;
                Color fg = overdue ? AccentDanger : AccentDeep;

                Size sz = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 32, sz.Width + 22, 24);

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
            else if (col == "DueDate" && text.Contains(DateTime.Today.ToString("MMM dd")))
            {
                e.PaintBackground(e.CellBounds, true);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 22, e.CellBounds.Width - 20, e.CellBounds.Height - 22),
                    AccentDanger,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                var actionBounds = GetActionBounds(e.CellBounds);

                TextRenderer.DrawText(e.Graphics, "✎", new Font("Segoe UI Symbol", 12F, FontStyle.Bold), actionBounds.Edit, AccentDeep,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(e.Graphics, "📥", new Font("Segoe UI Emoji", 11F), actionBounds.Archive, AccentDanger,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
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
                RenderBorrowingPage();
            };

            btnNextPage.Click += (s, e) =>
            {
                if (currentPage >= totalPages) return;
                currentPage++;
                RenderBorrowingPage();
            };

            btnPage1.Click += (s, e) => GoToBorrowPage(btnPage1);
            btnPage2.Click += (s, e) => GoToBorrowPage(btnPage2);
            btnPage3.Click += (s, e) => GoToBorrowPage(btnPage3);

            pagerPanel.Controls.Add(btnPrevPage);
            pagerPanel.Controls.Add(btnPage1);
            pagerPanel.Controls.Add(btnPage2);
            pagerPanel.Controls.Add(btnPage3);
            pagerPanel.Controls.Add(btnNextPage);

            UpdatePagerButtons();
        }

        private void GoToBorrowPage(Button button)
        {
            if (button.Tag == null)
                return;

            if (int.TryParse(button.Tag.ToString(), out int page))
            {
                currentPage = page;
                RenderBorrowingPage();
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
            button.BackColor = active ? AccentDeep : Color.White;
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

            btnNewBorrow.Bounds = new Rectangle(width - 190 + margin, 46, 190, 50);

            int cardTop = 132;
            int cardHeight = 170;
            int cardWidth = (width - (gap * 3)) / 4;

            cardActiveLoans.Bounds = new Rectangle(margin, cardTop, cardWidth, cardHeight);
            cardOverdue.Bounds = new Rectangle(cardActiveLoans.Right + gap, cardTop, cardWidth, cardHeight);
            cardDueToday.Bounds = new Rectangle(cardOverdue.Right + gap, cardTop, cardWidth, cardHeight);
            cardBorrowingRate.Bounds = new Rectangle(cardDueToday.Right + gap, cardTop, cardWidth, cardHeight);

            LayoutMetricCard(cardActiveLoans);
            LayoutMetricCard(cardOverdue);
            LayoutMetricCard(cardDueToday);
            LayoutMetricCard(cardBorrowingRate);

            tableCard.Bounds = new Rectangle(margin, 340, width, 620);

            searchPanel.Bounds = new Rectangle(26, 26, 470, 42);
            searchPanel.Controls[0].Location = new Point(13, 9);

            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = searchPanel.Width - 56;

            dgvBorrowing.Location = new Point(0, 92);
            dgvBorrowing.Size = new Size(tableCard.Width, 416);

            lblFooter.Location = new Point(28, tableCard.Height - 45);
            pagerPanel.Location = new Point(tableCard.Width - pagerPanel.Width - 28, tableCard.Height - 54);

            canvas.AutoScrollMinSize = new Size(0, tableCard.Bottom + 30);
        }

        private void LayoutMetricCard(Panel card)
        {
            if (card.Tag is not Control[] c) return;

            c[0].Location = new Point(24, 26);
            c[1].Location = new Point(card.Width - 145, 32);
            c[2].Location = new Point(24, 92);
            c[3].Location = new Point(24, 116);
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


        private sealed class BorrowRowItem
        {
            public int BorrowId { get; set; }
            public string MemberName { get; set; } = "";
            public string Role { get; set; } = "";
            public string BookTitle { get; set; } = "";
            public DateTime IssueDate { get; set; }
            public DateTime DueDate { get; set; }
            public string Status { get; set; } = "";
        }

        private void ClearGridSelection()
        {
            if (dgvBorrowing == null) return;
            dgvBorrowing.ClearSelection();
            dgvBorrowing.CurrentCell = null;
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
