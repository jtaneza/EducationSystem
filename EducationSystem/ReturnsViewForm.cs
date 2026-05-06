using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class ReturnsViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color Surface = Color.White;
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");

        private Panel pageCanvas = null!;
        private Panel titlePanel = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Panel heroPanel = null!;
        private Panel totalCard = null!;
        private Label lblBigNumber = null!;
        private Label lblHeroSub = null!;
        private Panel summaryCard = null!;
        private Label lblOnTimeValue = null!;
        private Label lblSchoolsValue = null!;
        private Panel filterHost = null!;
        private ComboBox cboSchoolFilter = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;
        private Panel filterRow = null!;
        private Panel tableWrap = null!;
        private Panel tableCard = null!;
        private Panel gridHost = null!;
        private DataGridView dgvReturns = null!;
        private Label lblFooter = null!;
        private List<ReturnsRowItem> allRows = new List<ReturnsRowItem>();

        public ReturnsViewForm()
        {
            InitializeComponent();
            BuildUI();
            LoadReturnsFromDatabase();
            LoadSchoolFilter();
            LoadReturnsData();
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

            titlePanel = new Panel
            {
                BackColor = Background
            };

            lblTitle = new Label
            {
                Text = "Global Return Records",
                Font = new Font("Segoe UI", 25F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubTitle = new Label
            {
                Text = "Comprehensive oversight of returned books across all schools.",
                Font = new Font("Segoe UI", 12F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(lblSubTitle);

            heroPanel = new Panel { BackColor = Background };

            totalCard = new Panel { BackColor = SurfaceLow, Size = new Size(360, 106) };
            lblBigNumber = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 34F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(28, 18)
            };
            lblHeroSub = new Label
            {
                Text = "Total Books Returned Across All Schools",
                Font = new Font("Segoe UI", 11.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(28, 72)
            };
            totalCard.Controls.Add(lblBigNumber);
            totalCard.Controls.Add(lblHeroSub);

            summaryCard = new Panel { BackColor = SurfaceLow, Size = new Size(360, 106) };
            summaryCard.Controls.Add(new Label { Text = "ON TIME RATE", Font = new Font("Segoe UI", 10F), ForeColor = OnSurfaceVariant, AutoSize = true, Location = new Point(28, 22) });
            lblOnTimeValue = new Label { Text = "0.0%", Font = new Font("Segoe UI", 24F, FontStyle.Bold | FontStyle.Italic), ForeColor = Primary, AutoSize = true, Location = new Point(28, 48) };
            summaryCard.Controls.Add(lblOnTimeValue);
            summaryCard.Controls.Add(new Label { Text = "TOTAL SCHOOLS", Font = new Font("Segoe UI", 10F), ForeColor = OnSurfaceVariant, AutoSize = true, Location = new Point(192, 22) });
            lblSchoolsValue = new Label { Text = "0", Font = new Font("Segoe UI", 24F, FontStyle.Bold), ForeColor = OnSurface, AutoSize = true, Location = new Point(192, 48) };
            summaryCard.Controls.Add(lblSchoolsValue);

            filterHost = new Panel { BackColor = SurfaceContainer, Size = new Size(220, 38) };
            cboSchoolFilter = new ComboBox { FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = SurfaceContainer, ForeColor = OnSurfaceVariant, Font = new Font("Segoe UI", 10F), Width = 200 };
            cboSchoolFilter.SelectedIndexChanged += (s, e) => LoadReturnsData();
            filterHost.Controls.Add(cboSchoolFilter);

            searchHost = new Panel { BackColor = SurfaceContainer, Size = new Size(280, 38) };
            lblSearchIcon = new Label { Text = "⌕", Font = new Font("Segoe UI Symbol", 16F), ForeColor = OnSurfaceVariant, AutoSize = true };
            txtSearch = new TextBox { BorderStyle = BorderStyle.None, BackColor = SurfaceContainer, ForeColor = OnSurfaceVariant, Font = new Font("Segoe UI", 10F), Text = "Search by member, book, or school..." };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search by member, book, or school...") { txtSearch.Text = ""; txtSearch.ForeColor = OnSurface; } };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search by member, book, or school..."; txtSearch.ForeColor = OnSurfaceVariant; } };
            txtSearch.TextChanged += (s, e) => LoadReturnsData();
            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            btnFilter = new Button { Text = "☰", Font = new Font("Segoe UI Symbol", 12F, FontStyle.Bold), Size = new Size(38, 38), FlatStyle = FlatStyle.Flat, BackColor = SurfaceHigh, ForeColor = OnSurfaceVariant };
            btnFilter.FlatAppearance.BorderSize = 0;

            heroPanel.Controls.Add(totalCard);
            heroPanel.Controls.Add(summaryCard);

            filterRow = new Panel
            {
                Height = 64,
                BackColor = Background
            };
            filterRow.Controls.Add(filterHost);
            filterRow.Controls.Add(searchHost);
            filterRow.Controls.Add(btnFilter);

            tableWrap = new Panel { BackColor = Background, Padding = new Padding(34, 0, 34, 24) };
            tableCard = new Panel { Dock = DockStyle.Fill, BackColor = Surface, BorderStyle = BorderStyle.FixedSingle };

            Panel header = new Panel { Dock = DockStyle.Top, Height = 0, BackColor = Surface };

            dgvReturns = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Surface,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 64,
                GridColor = Color.FromArgb(220, 228, 230),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ScrollBars = ScrollBars.Vertical,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                TabStop = false
            };
            dgvReturns.RowTemplate.Height = 86;
            dgvReturns.ColumnHeadersDefaultCellStyle.BackColor = SurfaceContainer;
            dgvReturns.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvReturns.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvReturns.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceContainer;
            dgvReturns.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;
            dgvReturns.DefaultCellStyle.BackColor = Surface;
            dgvReturns.DefaultCellStyle.ForeColor = OnSurface;
            dgvReturns.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvReturns.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvReturns.DefaultCellStyle.SelectionForeColor = OnSurface;
            dgvReturns.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvReturns.RowsDefaultCellStyle.SelectionForeColor = OnSurface;
            dgvReturns.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvReturns.AlternatingRowsDefaultCellStyle.SelectionForeColor = OnSurface;

            dgvReturns.Columns.Add("TransactionId", "TRANSACTION\r\nID");
            dgvReturns.Columns.Add("SchoolName", "SCHOOL\r\nNAME");
            dgvReturns.Columns.Add("MemberName", "MEMBER\r\nNAME");
            dgvReturns.Columns.Add("BookTitle", "BOOK TITLE");
            dgvReturns.Columns.Add("ReturnDate", "RETURN\r\nDATE");
            dgvReturns.Columns.Add("Status", "STATUS");

            dgvReturns.Columns["TransactionId"].FillWeight = 15;
            dgvReturns.Columns["SchoolName"].FillWeight = 18;
            dgvReturns.Columns["MemberName"].FillWeight = 18;
            dgvReturns.Columns["BookTitle"].FillWeight = 25;
            dgvReturns.Columns["ReturnDate"].FillWeight = 18;
            dgvReturns.Columns["Status"].FillWeight = 16;

            foreach (DataGridViewColumn col in dgvReturns.Columns) col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvReturns.CellPainting += dgvReturns_CellPainting;
            dgvReturns.SelectionChanged += (s, e) =>
            {
                if (dgvReturns.SelectedCells.Count > 0 || dgvReturns.SelectedRows.Count > 0)
                    dgvReturns.ClearSelection();
            };

            Panel footer = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = SurfaceContainer };
            lblFooter = new Label { Text = "Showing 0 of 0 returns", Font = new Font("Segoe UI", 9.5F), ForeColor = OnSurfaceVariant, AutoSize = true, Location = new Point(36, 18) };
            footer.Controls.Add(lblFooter);

            gridHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Surface
            };
            gridHost.Controls.Add(dgvReturns);

            tableCard.Controls.Add(footer);
            tableCard.Controls.Add(gridHost);
            tableCard.Controls.Add(header);
            footer.BringToFront();

            tableWrap.Controls.Add(tableCard);

            pageCanvas.Controls.Add(tableWrap);
            pageCanvas.Controls.Add(filterRow);
            pageCanvas.Controls.Add(heroPanel);
            pageCanvas.Controls.Add(titlePanel);

            Resize += (s, e) => AdjustLayout();
            AdjustLayout();
        }

        private void LoadReturnsFromDatabase()
        {
            allRows.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureReturnsViewSchema(conn);

                const string query = @"
SELECT rr.ReturnID,
       ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown School') AS SchoolName,
       ISNULL(NULLIF(rr.MemberName, ''), 'Unknown Member') AS MemberName,
       ISNULL(NULLIF(rr.BookTitle, ''), 'Unknown Book') AS BookTitle,
       rr.ReturnDate,
       ISNULL(rr.DaysOverdue, 0) AS DaysOverdue,
       ISNULL(NULLIF(rr.ReturnStatus, ''), 'ON TIME') AS ReturnStatus
FROM dbo.ReturnRecords rr
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = rr.ClientID
WHERE ISNULL(rr.IsArchived, 0) = 0
ORDER BY COALESCE(rr.CreatedAt, CAST(rr.ReturnDate AS DATETIME2), SYSUTCDATETIME()) DESC, rr.ReturnID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int returnId = Convert.ToInt32(reader["ReturnID"]);
                    int daysOverdue = Convert.ToInt32(reader["DaysOverdue"]);
                    string rawStatus = SafeText(reader["ReturnStatus"], "ON TIME").ToUpperInvariant();
                    bool late = daysOverdue > 0 || rawStatus.Contains("LATE");
                    DateTime returnDate = reader["ReturnDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ReturnDate"]);

                    allRows.Add(new ReturnsRowItem("#TX-" + returnId.ToString("00000"), SafeText(reader["SchoolName"], "Unknown School"), SafeText(reader["MemberName"], "Unknown Member"), SafeText(reader["BookTitle"], "Unknown Book"), returnDate.ToString("MMM dd, yyyy"), late ? "LATE" : "ON TIME", late ? "#F7816D" : "#B7EBD7", late ? "#6E1B0F" : "#3B6B5C"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Return records could not be loaded from the database.\n\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void LoadReturnsData()
        {
            dgvReturns.Rows.Clear();
            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string term = txtSearch.Text.Trim();
            bool useSearch = !string.IsNullOrWhiteSpace(term) && term != "Search by member, book, or school...";

            IEnumerable<ReturnsRowItem> filtered = allRows;
            if (selectedSchool != "All Schools") filtered = filtered.Where(x => x.SchoolName.Equals(selectedSchool, StringComparison.OrdinalIgnoreCase));
            if (useSearch) filtered = filtered.Where(x => x.SchoolName.Contains(term, StringComparison.OrdinalIgnoreCase) || x.MemberName.Contains(term, StringComparison.OrdinalIgnoreCase) || x.BookTitle.Contains(term, StringComparison.OrdinalIgnoreCase) || x.TransactionId.Contains(term, StringComparison.OrdinalIgnoreCase));

            List<ReturnsRowItem> results = filtered.ToList();
            foreach (var row in results)
                dgvReturns.Rows.Add(row.TransactionId, ReplaceFirst(row.SchoolName, " ", "\n"), ReplaceFirst(row.MemberName, " ", "\n"), InsertBookLineBreak(row.BookTitle), InsertDateLineBreak(row.ReturnDate), $"{row.Status}|{row.StatusBack}|{row.StatusFore}");

            dgvReturns.ClearSelection();
            lblFooter.Text = results.Count == 0 ? "Showing 0 of 0 returns" : $"Showing 1-{results.Count} of {results.Count} returns";
            lblBigNumber.Text = results.Count.ToString("N0");
            lblSchoolsValue.Text = results.Select(x => x.SchoolName).Distinct().Count().ToString("N0");
            int onTime = results.Count(x => x.Status == "ON TIME");
            lblOnTimeValue.Text = results.Count == 0 ? "0.0%" : $"{(decimal)onTime / results.Count * 100:0.0}%";
        }

        private string SafeText(object value, string fallback = "") { if (value == null || value == DBNull.Value) return fallback; string text = Convert.ToString(value) ?? ""; return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim(); }

        private void EnsureReturnsViewSchema(SqlConnection conn)
        {
            const string query = @"
IF COL_LENGTH('dbo.ReturnRecords', 'ClientID') IS NULL ALTER TABLE dbo.ReturnRecords ADD ClientID INT NULL;
IF COL_LENGTH('dbo.ReturnRecords', 'IsArchived') IS NULL ALTER TABLE dbo.ReturnRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_ReturnRecords_View_IsArchived DEFAULT 0;
IF COL_LENGTH('dbo.ReturnRecords', 'CreatedAt') IS NULL ALTER TABLE dbo.ReturnRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_ReturnRecords_View_CreatedAt DEFAULT SYSUTCDATETIME();";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private string InsertBookLineBreak(string text) { string[] words = text.Split(' '); if (words.Length <= 2) return text; int mid = words.Length / 2; return string.Join(" ", words.Take(mid)) + "\n" + string.Join(" ", words.Skip(mid)); }
        private string InsertDateLineBreak(string text) => text.Replace(", ", ",\n");
        private static string ReplaceFirst(string input, string oldValue, string newValue) { int index = input.IndexOf(oldValue, StringComparison.Ordinal); return index < 0 ? input : input.Substring(0, index) + newValue + input.Substring(index + oldValue.Length); }

        private void dgvReturns_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string column = dgvReturns.Columns[e.ColumnIndex].Name;
            if (column == "BookTitle")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(e.Graphics, e.FormattedValue?.ToString() ?? "", new Font("Segoe UI", 10F, FontStyle.Italic), new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16), OnSurface, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                e.Handled = true;
            }
            else if (column == "Status")
            {
                e.PaintBackground(e.CellBounds, true);
                string[] parts = (e.FormattedValue?.ToString() ?? "").Split('|');
                string text = parts[0];
                Color back = parts.Length > 1 ? ColorTranslator.FromHtml(parts[1]) : SurfaceHigh;
                Color fore = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : OnSurfaceVariant;
                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 9F, FontStyle.Bold));
                Rectangle badge = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + (e.CellBounds.Height - 42) / 2, textSize.Width + 28, 42);
                using (SolidBrush brush = new SolidBrush(back)) e.Graphics.FillEllipse(brush, badge);
                TextRenderer.DrawText(e.Graphics, text, new Font("Segoe UI", 9F, FontStyle.Bold), badge, fore, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                e.Handled = true;
            }
        }

        private void AdjustLayout()
        {
            if (pageCanvas == null) return;

            int margin = 34;
            int gap = 24;
            int clientWidth = pageCanvas.ClientSize.Width;
            int width = Math.Max(820, clientWidth - (margin * 2));
            int y = 0;

            titlePanel.Bounds = new Rectangle(0, y, clientWidth, 118);
            lblTitle.Location = new Point(margin, 28);
            lblSubTitle.Location = new Point(margin, 76);
            y += titlePanel.Height;

            heroPanel.Bounds = new Rectangle(0, y, clientWidth, 150);

            int cardTop = 18;
            int cardHeight = 106;
            int totalWidth = Math.Max(360, (width - gap) / 2);
            int summaryWidth = Math.Max(360, width - totalWidth - gap);

            totalCard.Bounds = new Rectangle(margin, cardTop, totalWidth, cardHeight);
            summaryCard.Bounds = new Rectangle(totalCard.Right + gap, cardTop, summaryWidth, cardHeight);

            lblBigNumber.Location = new Point(28, 14);
            lblHeroSub.Location = new Point(28, 72);

            Control onTimeCaption = summaryCard.Controls[0];
            Control schoolCaption = summaryCard.Controls[2];

            int blockGap = 52;
            int blockWidth = (summaryCard.Width - 56 - blockGap) / 2;

            onTimeCaption.Location = new Point(28, 22);
            lblOnTimeValue.Location = new Point(28, 48);

            schoolCaption.Location = new Point(28 + blockWidth + blockGap, 22);
            lblSchoolsValue.Location = new Point(28 + blockWidth + blockGap, 48);

            y += heroPanel.Height;

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

            int tableHeight = 620;
            tableWrap.Bounds = new Rectangle(0, y, clientWidth, tableHeight);
            tableWrap.Padding = new Padding(margin, 0, margin, 24);

            dgvReturns.ScrollBars = ScrollBars.Vertical;
            dgvReturns.ClearSelection();

            pageCanvas.AutoScrollMinSize = new Size(0, y + tableHeight + 32);
        }
    }

    public class ReturnsRowItem
    {
        public string TransactionId { get; set; }
        public string SchoolName { get; set; }
        public string MemberName { get; set; }
        public string BookTitle { get; set; }
        public string ReturnDate { get; set; }
        public string Status { get; set; }
        public string StatusBack { get; set; }
        public string StatusFore { get; set; }

        public ReturnsRowItem(string transactionId, string schoolName, string memberName, string bookTitle, string returnDate, string status, string statusBack, string statusFore)
        {
            TransactionId = transactionId;
            SchoolName = schoolName;
            MemberName = memberName;
            BookTitle = bookTitle;
            ReturnDate = returnDate;
            Status = status;
            StatusBack = statusBack;
            StatusFore = statusFore;
        }
    }
}