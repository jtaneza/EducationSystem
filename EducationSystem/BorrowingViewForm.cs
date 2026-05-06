using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class BorrowingViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = Color.White;
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color Outline = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color WarningText = ColorTranslator.FromHtml("#6E1B0F");

        private Panel pageCanvas = null!;
        private Panel heroPanel = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Panel filterHost = null!;
        private ComboBox cboSchoolFilter = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;

        private Panel statsPanel = null!;
        private Panel cardTotal = null!;
        private Panel cardOverview = null!;
        private Panel dueBlock = null!;
        private Panel overdueBlock = null!;
        private Panel borrowersBlock = null!;

        private Panel filterRow = null!;
        private Panel tableWrap = null!;
        private Panel tableShell = null!;
        private Panel gridHost = null!;
        private DataGridView dgvBorrowing = null!;
        private Panel footerPanel = null!;
        private Label lblFooter = null!;

        private List<BorrowingRowItem> allRows = new List<BorrowingRowItem>();

        public BorrowingViewForm()
        {
            InitializeComponent();
            BuildUI();
            LoadBorrowingFromDatabase();
            LoadSchoolFilter();
            LoadBorrowingData();
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = false;

            pageCanvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background,
                AutoScroll = true
            };
            pageCanvas.HorizontalScroll.Enabled = false;
            pageCanvas.HorizontalScroll.Visible = false;
            Controls.Add(pageCanvas);

            heroPanel = new Panel { BackColor = Background };

            lblTitle = new Label
            {
                Text = "Global Borrowing Records",
                Font = new Font("Segoe UI", 25F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "Comprehensive oversight of all active book circulations.",
                Font = new Font("Segoe UI", 12F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            filterHost = new Panel { BackColor = SurfaceContainer, Size = new Size(220, 38) };
            cboSchoolFilter = new ComboBox
            {
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = SurfaceContainer,
                ForeColor = OnSurfaceVariant,
                Font = new Font("Segoe UI", 10F),
                Width = 200
            };
            cboSchoolFilter.SelectedIndexChanged += (s, e) => LoadBorrowingData();
            filterHost.Controls.Add(cboSchoolFilter);

            searchHost = new Panel { BackColor = SurfaceContainer, Size = new Size(280, 38) };
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
                Text = "Search by member, book, or school..."
            };
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by member, book, or school...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = OnSurface;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by member, book, or school...";
                    txtSearch.ForeColor = OnSurfaceVariant;
                }
            };
            txtSearch.TextChanged += (s, e) => LoadBorrowingData();

            btnFilter = new Button
            {
                Text = "☰",
                Font = new Font("Segoe UI Symbol", 12F, FontStyle.Bold),
                Size = new Size(38, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Surface,
                ForeColor = Primary,
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 230);
            btnFilter.FlatAppearance.BorderSize = 1;

            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);
            heroPanel.Controls.Add(lblTitle);
            heroPanel.Controls.Add(lblSubTitle);

            statsPanel = new Panel { Height = 156, BackColor = Background };
            cardTotal = new Panel { BackColor = SurfaceLow };
            Label lblTotalCaption = new Label
            {
                Text = "TOTAL ACTIVE LOANS",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(24, 24)
            };
            Label lblTotalValue = new Label
            {
                Name = "lblTotalValue",
                Text = "0",
                Font = new Font("Segoe UI", 30F, FontStyle.Bold),
                ForeColor = Primary,
                AutoSize = true,
                Location = new Point(24, 48)
            };
            cardTotal.Controls.Add(lblTotalCaption);
            cardTotal.Controls.Add(lblTotalValue);

            cardOverview = new Panel { BackColor = Surface };
            dueBlock = CreateOverviewBlock("Upcoming Due", "0", OnSurface);
            overdueBlock = CreateOverviewBlock("Overdue Notices", "0", WarningText);
            borrowersBlock = CreateOverviewBlock("Unique Borrowers", "0", OnSurface);
            cardOverview.Controls.Add(dueBlock);
            cardOverview.Controls.Add(overdueBlock);
            cardOverview.Controls.Add(borrowersBlock);
            statsPanel.Controls.Add(cardTotal);
            statsPanel.Controls.Add(cardOverview);

            filterRow = new Panel
            {
                Height = 64,
                BackColor = Background
            };
            filterRow.Controls.Add(filterHost);
            filterRow.Controls.Add(searchHost);
            filterRow.Controls.Add(btnFilter);

            tableWrap = new Panel { BackColor = Background, Padding = new Padding(34, 0, 34, 24) };
            tableShell = new Panel { Dock = DockStyle.Fill, BackColor = Surface, BorderStyle = BorderStyle.FixedSingle };
            dgvBorrowing = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Surface,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 58,
                GridColor = Outline,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ScrollBars = ScrollBars.Vertical,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                TabStop = false
            };
            dgvBorrowing.RowTemplate.Height = 78;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.BackColor = SurfaceContainer;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvBorrowing.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceContainer;
            dgvBorrowing.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;
            dgvBorrowing.DefaultCellStyle.BackColor = Surface;
            dgvBorrowing.DefaultCellStyle.ForeColor = OnSurface;
            dgvBorrowing.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvBorrowing.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvBorrowing.DefaultCellStyle.SelectionForeColor = OnSurface;
            dgvBorrowing.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvBorrowing.RowsDefaultCellStyle.SelectionForeColor = OnSurface;
            dgvBorrowing.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvBorrowing.AlternatingRowsDefaultCellStyle.SelectionForeColor = OnSurface;

            dgvBorrowing.Columns.Add("TransactionId", "TRANSACTION\r\nID");
            dgvBorrowing.Columns.Add("SchoolName", "SCHOOL NAME");
            dgvBorrowing.Columns.Add("MemberName", "MEMBER\r\nNAME");
            dgvBorrowing.Columns.Add("BookTitle", "BOOK TITLE");
            dgvBorrowing.Columns.Add("BorrowDate", "BORROW\r\nDATE");
            dgvBorrowing.Columns.Add("DueDate", "DUE\r\nDATE");
            dgvBorrowing.Columns.Add("Status", "STATUS");

            dgvBorrowing.Columns["TransactionId"].FillWeight = 14;
            dgvBorrowing.Columns["SchoolName"].FillWeight = 18;
            dgvBorrowing.Columns["MemberName"].FillWeight = 14;
            dgvBorrowing.Columns["BookTitle"].FillWeight = 22;
            dgvBorrowing.Columns["BorrowDate"].FillWeight = 12;
            dgvBorrowing.Columns["DueDate"].FillWeight = 12;
            dgvBorrowing.Columns["Status"].FillWeight = 12;

            foreach (DataGridViewColumn col in dgvBorrowing.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvBorrowing.CellPainting += dgvBorrowing_CellPainting;
            dgvBorrowing.SelectionChanged += (s, e) =>
            {
                if (dgvBorrowing.SelectedCells.Count > 0 || dgvBorrowing.SelectedRows.Count > 0)
                    dgvBorrowing.ClearSelection();
            };

            footerPanel = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = SurfaceLow };
            lblFooter = new Label
            {
                Text = "Showing 0 of 0 entries",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(20, 18)
            };
            footerPanel.Controls.Add(lblFooter);

            gridHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Surface,
                Padding = new Padding(0, 0, 0, 0)
            };
            gridHost.Controls.Add(dgvBorrowing);

            tableShell.Controls.Add(footerPanel);
            tableShell.Controls.Add(gridHost);
            footerPanel.BringToFront();

            tableWrap.Controls.Add(tableShell);

            pageCanvas.Controls.Add(tableWrap);
            pageCanvas.Controls.Add(filterRow);
            pageCanvas.Controls.Add(statsPanel);
            pageCanvas.Controls.Add(heroPanel);

            Resize += (s, e) => AdjustLayout();
            AdjustLayout();
        }

        private Panel CreateOverviewBlock(string caption, string value, Color valueColor)
        {
            Panel panel = new Panel { Size = new Size(130, 68), BackColor = Color.Transparent };

            panel.Controls.Add(new Label
            {
                Text = caption,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(0, 0)
            });

            panel.Controls.Add(new Label
            {
                Name = "lblValue",
                Text = value,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = valueColor,
                AutoSize = true,
                Location = new Point(0, 18)
            });

            return panel;
        }

        private void LoadBorrowingFromDatabase()
        {
            allRows.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureBorrowingViewSchema(conn);

                const string query = @"
SELECT
    br.BorrowID,
    ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown School') AS SchoolName,
    ISNULL(NULLIF(u.FullName, ''), 'Unknown Member') AS MemberName,
    ISNULL(NULLIF(br.BookTitle, ''), 'Unknown Book') AS BookTitle,
    br.IssueDate,
    br.DueDate,
    ISNULL(NULLIF(br.Status, ''), 'ACTIVE') AS Status
FROM dbo.BorrowingRecords br
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = br.ClientID
LEFT JOIN dbo.Users u ON u.UserID = br.MemberID
WHERE ISNULL(br.IsArchived, 0) = 0
ORDER BY COALESCE(br.CreatedAt, CAST(br.IssueDate AS DATETIME2), SYSUTCDATETIME()) DESC, br.BorrowID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int borrowId = Convert.ToInt32(reader["BorrowID"]);
                    DateTime issueDate = reader["IssueDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["IssueDate"]);
                    DateTime dueDate = reader["DueDate"] == DBNull.Value ? issueDate : Convert.ToDateTime(reader["DueDate"]);
                    string rawStatus = SafeText(reader["Status"], "ACTIVE").ToUpperInvariant();
                    bool isOverdue = dueDate.Date < DateTime.Today &&
                                     (rawStatus == "ACTIVE" || rawStatus == "BORROWED" || rawStatus == "OVERDUE");

                    allRows.Add(new BorrowingRowItem(
                        "TX-" + borrowId.ToString("00000"),
                        SafeText(reader["SchoolName"], "Unknown School"),
                        SafeText(reader["MemberName"], "Unknown Member"),
                        SafeText(reader["BookTitle"], "Unknown Book"),
                        issueDate.ToString("MMM dd, yyyy"),
                        dueDate.ToString("MMM dd, yyyy"),
                        isOverdue,
                        isOverdue ? "Overdue" : rawStatus == "RETURNED" ? "Returned Today" : "In Progress",
                        isOverdue ? "#F7816D" : rawStatus == "RETURNED" ? "#DDE4E6" : "#B7EBD7",
                        isOverdue ? "#6E1B0F" : rawStatus == "RETURNED" ? "#3C4A44" : "#3B6B5C"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Borrowing records could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSchoolFilter()
        {
            cboSchoolFilter.Items.Clear();
            cboSchoolFilter.Items.Add("All Schools");

            foreach (string school in allRows.Select(x => x.SchoolName).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x))
                cboSchoolFilter.Items.Add(school);

            cboSchoolFilter.SelectedIndex = 0;
        }

        private void LoadBorrowingData()
        {
            dgvBorrowing.Rows.Clear();

            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string term = txtSearch.Text.Trim();
            bool useSearch = !string.IsNullOrWhiteSpace(term) && term != "Search by member, book, or school...";

            IEnumerable<BorrowingRowItem> filtered = allRows;

            if (selectedSchool != "All Schools")
                filtered = filtered.Where(x => x.SchoolName.Equals(selectedSchool, StringComparison.OrdinalIgnoreCase));

            if (useSearch)
            {
                filtered = filtered.Where(x =>
                    x.SchoolName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.MemberName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.BookTitle.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.TransactionId.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            List<BorrowingRowItem> results = filtered.ToList();

            foreach (var row in results)
            {
                dgvBorrowing.Rows.Add(
                    row.TransactionId,
                    ReplaceFirst(row.SchoolName, " ", "\n"),
                    ReplaceFirst(row.MemberName, " ", "\n"),
                    InsertBookLineBreak(row.BookTitle),
                    InsertDateLineBreak(row.BorrowDate),
                    row.IsDueOverdue ? InsertDateLineBreak(row.DueDate) + "|overdue" : InsertDateLineBreak(row.DueDate),
                    $"{row.Status}|{row.StatusBack}|{row.StatusFore}");
            }

            dgvBorrowing.ClearSelection();
            lblFooter.Text = $"Showing {results.Count} of {results.Count} entries";
            UpdateSummary(results);
        }

        private void UpdateSummary(List<BorrowingRowItem> rows)
        {
            Label? total = cardTotal.Controls.Find("lblTotalValue", true).FirstOrDefault() as Label;
            if (total != null) total.Text = rows.Count.ToString("N0");

            Label? due = dueBlock.Controls.Find("lblValue", true).FirstOrDefault() as Label;
            if (due != null) due.Text = rows.Count(x => x.Status == "In Progress").ToString("N0");

            Label? overdue = overdueBlock.Controls.Find("lblValue", true).FirstOrDefault() as Label;
            if (overdue != null) overdue.Text = rows.Count(x => x.Status == "Overdue").ToString("N0");

            Label? borrowers = borrowersBlock.Controls.Find("lblValue", true).FirstOrDefault() as Label;
            if (borrowers != null) borrowers.Text = rows.Select(x => x.MemberName).Distinct().Count().ToString("N0");
        }

        private string SafeText(object value, string fallback = "")
        {
            if (value == null || value == DBNull.Value) return fallback;
            string text = Convert.ToString(value) ?? "";
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        }

        private void EnsureBorrowingViewSchema(SqlConnection conn)
        {
            const string query = @"
IF COL_LENGTH('dbo.BorrowingRecords', 'ClientID') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD ClientID INT NULL;
IF COL_LENGTH('dbo.BorrowingRecords', 'IsArchived') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_BorrowingRecords_View_IsArchived DEFAULT 0;
IF COL_LENGTH('dbo.BorrowingRecords', 'CreatedAt') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_BorrowingRecords_View_CreatedAt DEFAULT SYSUTCDATETIME();
IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
    ALTER TABLE dbo.Users ADD ClientID INT NULL;";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private string InsertBookLineBreak(string text)
        {
            string[] words = text.Split(' ');
            if (words.Length <= 2) return text;
            int mid = words.Length / 2;
            return string.Join(" ", words.Take(mid)) + "\n" + string.Join(" ", words.Skip(mid));
        }

        private string InsertDateLineBreak(string text) => text.Replace(", ", ",\n");

        private static string ReplaceFirst(string input, string oldValue, string newValue)
        {
            int index = input.IndexOf(oldValue, StringComparison.Ordinal);
            return index < 0 ? input : input.Substring(0, index) + newValue + input.Substring(index + oldValue.Length);
        }

        private void dgvBorrowing_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string column = dgvBorrowing.Columns[e.ColumnIndex].Name;

            if (column == "BookTitle")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(e.Graphics, e.FormattedValue?.ToString() ?? "", new Font("Segoe UI", 10F, FontStyle.Italic),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 16, e.CellBounds.Height - 16),
                    OnSurface, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                e.Handled = true;
            }
            else if (column == "DueDate")
            {
                e.PaintBackground(e.CellBounds, true);
                string raw = e.FormattedValue?.ToString() ?? "";
                bool overdue = raw.Contains("|overdue");
                string text = raw.Replace("|overdue", "");
                TextRenderer.DrawText(e.Graphics, text, new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 16, e.CellBounds.Height - 16),
                    overdue ? WarningText : OnSurface, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                e.Handled = true;
            }
            else if (column == "Status")
            {
                e.PaintBackground(e.CellBounds, true);
                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string text = parts[0];
                Color back = parts.Length > 1 ? ColorTranslator.FromHtml(parts[1]) : SurfaceContainer;
                Color fore = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : OnSurfaceVariant;
                Size textSize = TextRenderer.MeasureText(text.ToUpper(), new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + (e.CellBounds.Height - 28) / 2, textSize.Width + 20, 28);
                using (SolidBrush brush = new SolidBrush(back)) e.Graphics.FillRectangle(brush, badge);
                TextRenderer.DrawText(e.Graphics, text.ToUpper(), new Font("Segoe UI", 8.5F, FontStyle.Bold), badge, fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                e.Handled = true;
            }
        }

        private void AdjustLayout()
        {
            if (pageCanvas == null) return;

            int margin = 34;
            int gap = 24;
            int clientWidth = pageCanvas.ClientSize.Width;
            int width = Math.Max(800, clientWidth - (margin * 2));

            int y = 0;

            heroPanel.Bounds = new Rectangle(0, y, clientWidth, 150);
            lblTitle.Location = new Point(margin, 28);
            lblSubTitle.Location = new Point(margin, 76);
            y += heroPanel.Height;

            statsPanel.Bounds = new Rectangle(0, y, clientWidth, 156);

            int cardHeight = 118;
            int leftWidth = (int)(width * 0.34);
            int rightWidth = width - leftWidth - gap;

            cardTotal.Bounds = new Rectangle(margin, 18, leftWidth, cardHeight);
            cardOverview.Bounds = new Rectangle(cardTotal.Right + gap, 18, rightWidth, cardHeight);

            int overviewPadding = 28;
            int overviewGap = 24;
            int blockWidth = Math.Max(130, (cardOverview.Width - (overviewPadding * 2) - (overviewGap * 2)) / 3);

            dueBlock.Bounds = new Rectangle(overviewPadding, 28, blockWidth, 68);
            overdueBlock.Bounds = new Rectangle(dueBlock.Right + overviewGap, 28, blockWidth, 68);
            borrowersBlock.Bounds = new Rectangle(overdueBlock.Right + overviewGap, 28, blockWidth, 68);

            y += statsPanel.Height;

            filterRow.Bounds = new Rectangle(0, y, clientWidth, 64);

            btnFilter.Location = new Point(clientWidth - btnFilter.Width - margin, 12);
            searchHost.Location = new Point(btnFilter.Left - searchHost.Width - 10, 12);
            filterHost.Location = new Point(searchHost.Left - filterHost.Width - 10, 12);

            cboSchoolFilter.Location = new Point(10, 6);
            cboSchoolFilter.Width = filterHost.Width - 20;

            lblSearchIcon.Location = new Point(10, 8);
            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = searchHost.Width - 54;

            y += filterRow.Height;

            int tableHeight = 620; // page scroll will reveal the bottom if the screen is short
            tableWrap.Bounds = new Rectangle(0, y, clientWidth, tableHeight);
            tableWrap.Padding = new Padding(margin, 0, margin, 24);

            dgvBorrowing.ScrollBars = ScrollBars.Vertical;
            dgvBorrowing.ClearSelection();

            pageCanvas.AutoScrollMinSize = new Size(0, y + tableHeight + 28);
        }
    }

    public class BorrowingRowItem
    {
        public string TransactionId { get; set; }
        public string SchoolName { get; set; }
        public string MemberName { get; set; }
        public string BookTitle { get; set; }
        public string BorrowDate { get; set; }
        public string DueDate { get; set; }
        public bool IsDueOverdue { get; set; }
        public string Status { get; set; }
        public string StatusBack { get; set; }
        public string StatusFore { get; set; }

        public BorrowingRowItem(string transactionId, string schoolName, string memberName, string bookTitle, string borrowDate, string dueDate, bool isDueOverdue, string status, string statusBack, string statusFore)
        {
            TransactionId = transactionId;
            SchoolName = schoolName;
            MemberName = memberName;
            BookTitle = bookTitle;
            BorrowDate = borrowDate;
            DueDate = dueDate;
            IsDueOverdue = isDueOverdue;
            Status = status;
            StatusBack = statusBack;
            StatusFore = statusFore;
        }
    }
}