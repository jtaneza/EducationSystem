using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class FinesViewForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = Color.White;
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceContainer = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color OnTertiaryContainer = ColorTranslator.FromHtml("#6E1B0F");
        private readonly Color InverseSurface = ColorTranslator.FromHtml("#2B3234");

        private Panel canvas = null!;
        private Panel titlePanel = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Panel heroPanel = null!;
        private Panel cardOutstanding = null!;
        private Panel cardPaid = null!;
        private Panel cardUnpaid = null!;
        private Label lblOutstandingValue = null!;
        private Label lblPaidValue = null!;
        private Label lblUnpaidValue = null!;
        private Panel paidProgressTrack = null!;
        private Panel paidProgressFill = null!;
        private Panel tableWrap = null!;
        private Panel tableCard = null!;
        private Panel tableHeader = null!;
        private ComboBox cboSchoolFilter = null!;
        private Panel filterHost = null!;
        private Panel searchHost = null!;
        private Label lblSearchIcon = null!;
        private TextBox txtSearch = null!;
        private Button btnFilter = null!;
        private DataGridView dgvFines = null!;
        private Label lblFooter = null!;
        private List<FinesRowItem> allRows = new List<FinesRowItem>();
        private List<FinesRowItem> currentPageRows = new List<FinesRowItem>();
        private int currentPage = 1;
        private int totalPages = 1;
        private const int PageSize = 5;

        public FinesViewForm()
        {
            InitializeComponent();
            BuildUI();
            LoadFinesFromDatabase();
            LoadSchoolFilter();
            LoadFinesData();
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            canvas = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Background, Padding = new Padding(0, 0, 0, 120) };
            canvas.HorizontalScroll.Enabled = false;
            canvas.HorizontalScroll.Visible = false;

            titlePanel = new Panel { BackColor = Background, Height = 118 };
            lblTitle = new Label
            {
                Text = "Global Fines Registry",
                Font = new Font("Segoe UI", 25F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };
            lblSubTitle = new Label
            {
                Text = "Real-time monitoring across all institutions",
                Font = new Font("Segoe UI", 12F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };
            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(lblSubTitle);

            heroPanel = new Panel { BackColor = Background, Height = 190 };
            cardOutstanding = new Panel { BackColor = SurfaceLow };
            cardPaid = new Panel { BackColor = Surface };
            cardUnpaid = new Panel { BackColor = InverseSurface };

            cardOutstanding.Controls.Add(new Label { Text = "Total Outstanding Fines", Font = new Font("Segoe UI", 12F), ForeColor = OnSurfaceVariant, AutoSize = true, Location = new Point(24, 22) });
            lblOutstandingValue = new Label { Text = "₱0.00", Font = new Font("Segoe UI", 40F, FontStyle.Bold), ForeColor = OnSurface, AutoSize = true, Location = new Point(24, 48) };
            cardOutstanding.Controls.Add(lblOutstandingValue);
            cardOutstanding.Controls.Add(new Label { Text = "Global across all client libraries", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = OnTertiaryContainer, BackColor = ColorTranslator.FromHtml("#F7816D"), AutoSize = true, Padding = new Padding(10, 6, 10, 6), Location = new Point(24, 122) });

            cardPaid.Controls.Add(new Label { Text = "PAID FINES", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = OnSurfaceVariant, AutoSize = true, Location = new Point(24, 22) });
            lblPaidValue = new Label { Text = "₱0", Font = new Font("Segoe UI", 28F, FontStyle.Bold), ForeColor = Primary, AutoSize = true, Location = new Point(24, 52) };
            paidProgressTrack = new Panel { BackColor = SurfaceHigh, Height = 4 };
            paidProgressFill = new Panel { BackColor = Primary, Height = 4 };
            cardPaid.Controls.Add(lblPaidValue);
            cardPaid.Controls.Add(paidProgressTrack);
            cardPaid.Controls.Add(paidProgressFill);

            cardUnpaid.Controls.Add(new Label { Text = "UNPAID ACCOUNTS", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#BBCAC3"), AutoSize = true, Location = new Point(24, 22) });
            lblUnpaidValue = new Label { Text = "0", Font = new Font("Segoe UI", 28F, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(24, 52) };
            cardUnpaid.Controls.Add(lblUnpaidValue);
            cardUnpaid.Controls.Add(new Label { Text = "Requires oversight", Font = new Font("Segoe UI", 10F), ForeColor = ColorTranslator.FromHtml("#6DFAD2"), AutoSize = true, Location = new Point(24, 130) });

            heroPanel.Controls.Add(cardOutstanding);
            heroPanel.Controls.Add(cardPaid);
            heroPanel.Controls.Add(cardUnpaid);

            tableWrap = new Panel { BackColor = Background, Height = 610 };
            tableCard = new Panel { BackColor = Background, BorderStyle = BorderStyle.None };
            tableHeader = new Panel { Height = 64, Dock = DockStyle.Top, BackColor = Background, Padding = new Padding(0) };

            filterHost = new Panel { BackColor = SurfaceContainer, Size = new Size(220, 38) };
            cboSchoolFilter = new ComboBox { FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = SurfaceContainer, ForeColor = OnSurfaceVariant, Font = new Font("Segoe UI", 10F), Width = 200 };
            cboSchoolFilter.SelectedIndexChanged += (s, e) =>
            {
                currentPage = 1;
                LoadFinesData();
            };
            filterHost.Controls.Add(cboSchoolFilter);

            searchHost = new Panel { BackColor = SurfaceContainer, Size = new Size(290, 38) };
            lblSearchIcon = new Label { Text = "⌕", Font = new Font("Segoe UI Symbol", 16F), ForeColor = OnSurfaceVariant, AutoSize = true };
            txtSearch = new TextBox { BorderStyle = BorderStyle.None, BackColor = SurfaceContainer, ForeColor = OnSurfaceVariant, Font = new Font("Segoe UI", 10F), Text = "Search by member, fine, or school..." };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search by member, fine, or school...") { txtSearch.Text = ""; txtSearch.ForeColor = OnSurface; } };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search by member, fine, or school..."; txtSearch.ForeColor = OnSurfaceVariant; } };
            txtSearch.TextChanged += (s, e) =>
            {
                currentPage = 1;
                LoadFinesData();
            };
            searchHost.Controls.Add(lblSearchIcon);
            searchHost.Controls.Add(txtSearch);

            btnFilter = new Button { Text = "☰", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Width = 44, Height = 38, FlatStyle = FlatStyle.Flat, BackColor = Surface, ForeColor = OnSurface };
            btnFilter.FlatAppearance.BorderColor = Color.FromArgb(225, 231, 232);

            tableHeader.Controls.Add(filterHost);
            tableHeader.Controls.Add(searchHost);
            tableHeader.Controls.Add(btnFilter);

            dgvFines = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Surface, BorderStyle = BorderStyle.FixedSingle, RowHeadersVisible = false, AllowUserToAddRows = false, AllowUserToResizeRows = false, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, GridColor = Color.FromArgb(230, 235, 236), CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ScrollBars = ScrollBars.Vertical };
            dgvFines.RowTemplate.Height = 72;
            dgvFines.ColumnHeadersDefaultCellStyle.BackColor = SurfaceContainer;
            dgvFines.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvFines.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvFines.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceContainer;
            dgvFines.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;
            dgvFines.DefaultCellStyle.BackColor = Surface;
            dgvFines.DefaultCellStyle.ForeColor = OnSurface;
            dgvFines.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvFines.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvFines.DefaultCellStyle.SelectionForeColor = OnSurface;
            dgvFines.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvFines.RowsDefaultCellStyle.SelectionForeColor = OnSurface;
            dgvFines.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 250, 248);
            dgvFines.AlternatingRowsDefaultCellStyle.SelectionForeColor = OnSurface;
            dgvFines.Columns.Add("FineId", "FINE ID");
            dgvFines.Columns.Add("SchoolName", "SCHOOL NAME");
            dgvFines.Columns.Add("MemberName", "MEMBER NAME");
            dgvFines.Columns.Add("Reason", "REASON");
            dgvFines.Columns.Add("Amount", "AMOUNT");
            dgvFines.Columns.Add("Status", "STATUS");
            foreach (DataGridViewColumn col in dgvFines.Columns) col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvFines.CellPainting += dgvFines_CellPainting;

            Panel footerPanel = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = Color.FromArgb(250, 252, 252) };
            lblFooter = new Label { Text = "Showing 0 of 0 records across the global network", Font = new Font("Segoe UI", 9.5F), ForeColor = OnSurfaceVariant, AutoSize = true, Location = new Point(24, 20) };
            footerPanel.Controls.Add(lblFooter);

            tableCard.Controls.Add(dgvFines);
            tableCard.Controls.Add(footerPanel);
            tableCard.Controls.Add(tableHeader);
            tableWrap.Controls.Add(tableCard);
            canvas.Controls.Add(tableWrap);
            canvas.Controls.Add(heroPanel);
            canvas.Controls.Add(titlePanel);
            Controls.Add(canvas);

            Resize += (s, e) => AdjustLayout();
            AdjustLayout();
        }

        private void LoadFinesFromDatabase()
        {
            allRows.Clear();
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureFinesViewSchema(conn);

                const string query = @"
SELECT fr.FineID,
       ISNULL(NULLIF(cl.LibraryName, ''), 'Unknown School') AS SchoolName,
       ISNULL(NULLIF(fr.MemberName, ''), 'Unknown Member') AS MemberName,
       ISNULL(NULLIF(fr.Reason, ''), 'Fine') AS Reason,
       ISNULL(fr.Amount, 0) AS Amount,
       ISNULL(NULLIF(fr.Status, ''), 'UNPAID') AS Status
FROM dbo.FineRecords fr
LEFT JOIN dbo.ClientLibraries cl ON cl.ClientID = fr.ClientID
WHERE ISNULL(fr.IsArchived, 0) = 0
ORDER BY COALESCE(fr.CreatedAt, SYSUTCDATETIME()) DESC, fr.FineID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int fineId = Convert.ToInt32(reader["FineID"]);
                    string member = SafeText(reader["MemberName"], "Unknown Member");
                    string status = SafeText(reader["Status"], "UNPAID").ToUpperInvariant();
                    bool paid = status == "PAID";

                    allRows.Add(new FinesRowItem("#FIN-" + fineId.ToString("00000"), SafeText(reader["SchoolName"], "Unknown School"), GetInitials(member), member, paid ? "#B7EBD7" : "#DDE4E6", paid ? "#3B6B5C" : "#161D1F", SafeText(reader["Reason"], "Fine"), reader["Amount"] == DBNull.Value ? 0M : Convert.ToDecimal(reader["Amount"]), paid ? "PAID" : "UNPAID", paid ? "#B7EBD7" : "#F7816D", paid ? "#3B6B5C" : "#6E1B0F"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fine records could not be loaded from the database.\n\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void LoadFinesData()
        {
            dgvFines.Rows.Clear();

            string selectedSchool = cboSchoolFilter.SelectedItem?.ToString() ?? "All Schools";
            string term = txtSearch.Text.Trim();
            bool useSearch = !string.IsNullOrWhiteSpace(term) && term != "Search by member, fine, or school...";

            IEnumerable<FinesRowItem> filtered = allRows;

            if (selectedSchool != "All Schools")
                filtered = filtered.Where(x => x.SchoolName.Equals(selectedSchool, StringComparison.OrdinalIgnoreCase));

            if (useSearch)
            {
                filtered = filtered.Where(x =>
                    x.SchoolName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.MemberName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Reason.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.FineId.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            List<FinesRowItem> results = filtered.ToList();

            totalPages = Math.Max(1, (int)Math.Ceiling(results.Count / (double)PageSize));
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;

            currentPageRows = results.Skip((currentPage - 1) * PageSize).Take(PageSize).ToList();

            foreach (var row in currentPageRows)
            {
                dgvFines.Rows.Add(
                    row.FineId,
                    row.SchoolName,
                    $"{row.Initials}|{row.MemberName}|{row.MemberBack}|{row.MemberFore}",
                    InsertReasonLineBreak(row.Reason),
                    "₱" + row.Amount.ToString("0.00"),
                    $"{row.Status}|{row.StatusBack}|{row.StatusFore}");
            }

            dgvFines.ClearSelection();

            if (results.Count == 0)
            {
                lblFooter.Text = "Showing 0 of 0 records across the global network";
            }
            else
            {
                int start = ((currentPage - 1) * PageSize) + 1;
                int end = Math.Min(start + currentPageRows.Count - 1, results.Count);
                lblFooter.Text = $"Showing {start}-{end} of {results.Count} records across the global network";
            }

            decimal total = results.Sum(x => x.Amount);
            decimal paid = results.Where(x => x.Status == "PAID").Sum(x => x.Amount);

            lblOutstandingValue.Text = "₱" + total.ToString("N2");
            lblPaidValue.Text = "₱" + paid.ToString("N0");
            lblUnpaidValue.Text = results.Count(x => x.Status == "UNPAID").ToString("N0");

            if (paidProgressFill != null && paidProgressTrack != null)
                paidProgressFill.Width = total <= 0 ? 0 : (int)(paidProgressTrack.Width * Math.Min(1M, paid / total));
        }

        private string SafeText(object value, string fallback = "") { if (value == null || value == DBNull.Value) return fallback; string text = Convert.ToString(value) ?? ""; return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim(); }
        private string GetInitials(string name) { string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries); if (parts.Length == 0) return "U"; if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant(); return (parts[0][0].ToString() + parts[^1][0]).ToUpperInvariant(); }

        private void EnsureFinesViewSchema(SqlConnection conn)
        {
            const string query = @"
IF COL_LENGTH('dbo.FineRecords', 'ClientID') IS NULL ALTER TABLE dbo.FineRecords ADD ClientID INT NULL;
IF COL_LENGTH('dbo.FineRecords', 'IsArchived') IS NULL ALTER TABLE dbo.FineRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_FineRecords_View_IsArchived DEFAULT 0;
IF COL_LENGTH('dbo.FineRecords', 'CreatedAt') IS NULL ALTER TABLE dbo.FineRecords ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_FineRecords_View_CreatedAt DEFAULT SYSUTCDATETIME();";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private string InsertReasonLineBreak(string text) { string[] words = text.Split(' '); if (words.Length <= 2) return text; int mid = words.Length / 2; return string.Join(" ", words.Take(mid)) + "\n" + string.Join(" ", words.Skip(mid)); }

        private void dgvFines_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string column = dgvFines.Columns[e.ColumnIndex].Name;

            if (column == "MemberName")
            {
                e.PaintBackground(e.CellBounds, true);
                string[] parts = (e.FormattedValue?.ToString() ?? "").Split('|');
                string initials = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : "";
                Color circleBack = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : SurfaceHigh;
                Color circleFore = parts.Length > 3 ? ColorTranslator.FromHtml(parts[3]) : OnSurface;
                Rectangle circle = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 20, 28, 28);
                using (SolidBrush brush = new SolidBrush(circleBack)) e.Graphics.FillEllipse(brush, circle);
                TextRenderer.DrawText(e.Graphics, initials, new Font("Segoe UI", 8.5F, FontStyle.Bold), circle, circleFore, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(e.Graphics, name, new Font("Segoe UI", 10F), new Rectangle(e.CellBounds.X + 48, e.CellBounds.Y + 10, e.CellBounds.Width - 56, e.CellBounds.Height - 16), OnSurface, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                e.Handled = true;
            }
            else if (column == "Reason")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(e.Graphics, e.FormattedValue?.ToString() ?? "", new Font("Segoe UI", 10F, FontStyle.Italic), new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 10, e.CellBounds.Width - 18, e.CellBounds.Height - 16), OnSurfaceVariant, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                e.Handled = true;
            }
            else if (column == "Status")
            {
                e.PaintBackground(e.CellBounds, true);
                string[] parts = (e.FormattedValue?.ToString() ?? "").Split('|');
                string text = parts[0];
                Color back = parts.Length > 1 ? ColorTranslator.FromHtml(parts[1]) : SurfaceHigh;
                Color fore = parts.Length > 2 ? ColorTranslator.FromHtml(parts[2]) : OnSurfaceVariant;
                Size textSize = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(e.CellBounds.X + (e.CellBounds.Width - textSize.Width - 22) / 2, e.CellBounds.Y + (e.CellBounds.Height - 28) / 2, textSize.Width + 22, 28);
                using (SolidBrush brush = new SolidBrush(back)) e.Graphics.FillEllipse(brush, badge);
                TextRenderer.DrawText(e.Graphics, text, new Font("Segoe UI", 8.5F, FontStyle.Bold), badge, fore, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                e.Handled = true;
            }
        }

        private void AdjustLayout()
        {
            if (canvas == null) return;

            int margin = 34;
            int gap = 24;
            int clientWidth = canvas.ClientSize.Width;
            int width = Math.Max(900, clientWidth - (margin * 2));

            int y = 0;

            titlePanel.SetBounds(0, y, clientWidth, 118);
            lblTitle.Location = new Point(margin, 28);
            lblSubTitle.Location = new Point(margin, 76);
            y += titlePanel.Height;

            heroPanel.SetBounds(0, y, clientWidth, 180);

            int leftWidth = (int)(width * 0.46);
            int middleWidth = (int)(width * 0.22);
            int rightWidth = width - leftWidth - middleWidth - (gap * 2);

            cardOutstanding.Bounds = new Rectangle(margin, 18, leftWidth, 150);
            cardPaid.Bounds = new Rectangle(cardOutstanding.Right + gap, 18, middleWidth, 150);
            cardUnpaid.Bounds = new Rectangle(cardPaid.Right + gap, 18, rightWidth, 150);

            paidProgressTrack.Bounds = new Rectangle(24, cardPaid.Height - 34, cardPaid.Width - 48, 4);
            paidProgressFill.Bounds = new Rectangle(24, cardPaid.Height - 34, Math.Min(paidProgressFill.Width, paidProgressTrack.Width), 4);

            y += heroPanel.Height;

            int tableHeight = 500;
            tableWrap.SetBounds(0, y, clientWidth, tableHeight);
            tableCard.SetBounds(margin, 0, width, tableHeight);

            btnFilter.Location = new Point(tableHeader.Width - btnFilter.Width - 24, 13);
            searchHost.Location = new Point(btnFilter.Left - 300, 13);
            filterHost.Location = new Point(searchHost.Left - 230, 13);

            cboSchoolFilter.Location = new Point(10, 6);
            cboSchoolFilter.Width = filterHost.Width - 20;

            lblSearchIcon.Location = new Point(10, 8);
            txtSearch.Location = new Point(42, 11);
            txtSearch.Width = searchHost.Width - 54;

            dgvFines.ScrollBars = ScrollBars.Vertical;

            canvas.AutoScrollMinSize = new Size(0, tableWrap.Bottom + 60);
        }
    }

    public class FinesRowItem
    {
        public string FineId { get; set; }
        public string SchoolName { get; set; }
        public string Initials { get; set; }
        public string MemberName { get; set; }
        public string MemberBack { get; set; }
        public string MemberFore { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string StatusBack { get; set; }
        public string StatusFore { get; set; }

        public FinesRowItem(string fineId, string schoolName, string initials, string memberName, string memberBack, string memberFore, string reason, decimal amount, string status, string statusBack, string statusFore)
        {
            FineId = fineId;
            SchoolName = schoolName;
            Initials = initials;
            MemberName = memberName;
            MemberBack = memberBack;
            MemberFore = memberFore;
            Reason = reason;
            Amount = amount;
            Status = status;
            StatusBack = statusBack;
            StatusFore = statusFore;
        }
    }
}