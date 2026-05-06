using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class FinesForm : Form
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
        private readonly Color DarkCard = ColorTranslator.FromHtml("#2B3234");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Button btnCreateFine = null!;
        private Button btnApplyFine = null!;

        private Panel cardUnpaid = null!;
        private Panel cardCritical = null!;
        private Panel cardEfficiency = null!;

        private Panel tableCard = null!;
        private Label lblTableTitle = null!;
        private Panel searchPanel = null!;
        private TextBox txtSearch = null!;
        private DataGridView dgvFines = null!;
        private Label lblFooter = null!;
        private Panel pagerPanel = null!;

        private Panel policyCard = null!;
        private Label lblPolicyTitle = null!;
        private DataGridView dgvPolicies = null!;
        private Label lblPolicyFooter = null!;

        private readonly List<FineRow> fineRecords = new List<FineRow>();
        private List<FineRow> filteredFineRecords = new List<FineRow>();

        private int currentPage = 1;
        private int totalPages = 1;
        private const int PageSize = 4;

        private Button btnPrevPage = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNextPage = null!;

        private readonly List<FinePolicyRow> finePolicies = new List<FinePolicyRow>();
        private readonly List<FineMemberOption> memberOptions = new List<FineMemberOption>();
        private readonly List<FineBookOption> bookOptions = new List<FineBookOption>();
        private const string SearchPlaceholder = "Search fines...";

        private sealed class FineRow
        {
            public int FineId { get; set; }
            public int? BorrowId { get; set; }
            public int? MemberId { get; set; }
            public string MemberName { get; set; } = "";
            public string BookId { get; set; } = "";
            public string BookTitle { get; set; } = "";
            public string Reason { get; set; } = "";
            public decimal Amount { get; set; }
            public string Status { get; set; } = "Unpaid";
            public string Remarks { get; set; } = "";
            public string RecordedBy { get; set; } = "";
            public DateTime CreatedAt { get; set; }
        }

        private sealed class FinePolicyRow
        {
            public int PolicyId { get; set; }
            public string FineType { get; set; } = "";
            public decimal DefaultAmount { get; set; }
            public string Description { get; set; } = "";
            public string AddedBy { get; set; } = "";
            public DateTime CreatedAt { get; set; }
        }

        public FinesForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            BuildUI();

            Load += (s, e) =>
            {
                AdjustLayout();
                LoadFineData();
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
                Text = "Fine Management",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubtitle = new Label
            {
                Text = "Review and resolve academic resource penalties.",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            btnCreateFine = new Button
            {
                Text = "+   Create Fine",
                FlatStyle = FlatStyle.Flat,
                BackColor = CardBack,
                ForeColor = AccentDeep,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreateFine.FlatAppearance.BorderColor = AccentEmerald;
            btnCreateFine.FlatAppearance.BorderSize = 1;
            btnCreateFine.Visible = true;
            btnCreateFine.Click += (s, e) => ShowFinePolicyDialog(null);

            btnApplyFine = new Button
            {
                Text = "+   Apply Fine",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnApplyFine.FlatAppearance.BorderSize = 0;
            btnApplyFine.Visible = true;
            btnApplyFine.Click += (s, e) => ShowFineDialog(null);

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubtitle);
            canvas.Controls.Add(btnCreateFine);
            canvas.Controls.Add(btnApplyFine);

            BuildCards();
            BuildTable();
            BuildPolicyTable();
        }

        private void BuildCards()
        {
            cardUnpaid = CreateRoundedCard();
            cardCritical = CreateRoundedCard();
            cardEfficiency = CreateRoundedCard();
            cardEfficiency.BackColor = DarkCard;

            canvas.Controls.Add(cardUnpaid);
            canvas.Controls.Add(cardCritical);
            canvas.Controls.Add(cardEfficiency);

            BuildUnpaidCard();
            BuildCriticalCard();
            BuildEfficiencyCard();
        }

        private void BuildUnpaidCard()
        {
            Label title = CreateCardTitle("TOTAL UNPAID");
            Label value = CreateBigValue("₱0.00", AccentDeep);

            Label trend = new Label
            {
                Text = "Live database total",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = AccentDeep,
                BackColor = Color.Transparent
            };

            cardUnpaid.Controls.Add(title);
            cardUnpaid.Controls.Add(value);
            cardUnpaid.Controls.Add(trend);
            cardUnpaid.Tag = new Control[] { title, value, trend };
        }

        private void BuildCriticalCard()
        {
            Label title = CreateCardTitle("CRITICAL ALERTS");
            Label value = CreateBigValue("0", AccentDanger);

            Label note = new Label
            {
                Text = "Unpaid fines older than 60 days",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            cardCritical.Controls.Add(title);
            cardCritical.Controls.Add(value);
            cardCritical.Controls.Add(note);
            cardCritical.Tag = new Control[] { title, value, note };
        }

        private void BuildEfficiencyCard()
        {
            Label title = new Label
            {
                Text = "COLLECTION EFFICIENCY",
                AutoSize = true,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#BBCAC3"),
                BackColor = Color.Transparent
            };

            Label value = new Label
            {
                Text = "0%",
                AutoSize = true,
                Font = new Font("Segoe UI", 30F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#6DFAD2"),
                BackColor = Color.Transparent
            };

            Panel barBack = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#3C4A44")
            };

            Panel barFill = new Panel
            {
                BackColor = AccentEmerald
            };

            cardEfficiency.Controls.Add(title);
            cardEfficiency.Controls.Add(value);
            cardEfficiency.Controls.Add(barBack);
            cardEfficiency.Controls.Add(barFill);
            cardEfficiency.Tag = new Control[] { title, value, barBack, barFill };
        }

        private Label CreateCardTitle(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };
        }

        private Label CreateBigValue(string text, Color color)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 30F, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private void BuildTable()
        {
            tableCard = CreateRoundedCard();
            canvas.Controls.Add(tableCard);

            lblTableTitle = new Label
            {
                Text = "Recent Fine Records",
                AutoSize = true,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

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
                RebuildFineRows(GetCurrentSearchText());
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(txtSearch);

            dgvFines = new DataGridView
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
                ColumnHeadersHeight = 54,
                RowTemplate = { Height = 76 },
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical,
                ShowCellToolTips = false
            };

            dgvFines.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvFines.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvFines.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvFines.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvFines.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvFines.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgvFines.DefaultCellStyle.BackColor = CardBack;
            dgvFines.DefaultCellStyle.ForeColor = PrimaryText;
            dgvFines.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            dgvFines.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvFines.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvFines.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            dgvFines.RowsDefaultCellStyle.BackColor = CardBack;
            dgvFines.RowsDefaultCellStyle.SelectionBackColor = CardBack;
            dgvFines.RowsDefaultCellStyle.SelectionForeColor = PrimaryText;

            dgvFines.Columns.Add("FineId", "FINE ID");
            dgvFines.Columns.Add("MemberName", "MEMBER NAME");
            dgvFines.Columns.Add("BookInfo", "BOOK ID / TITLE");
            dgvFines.Columns.Add("Reason", "REASON");
            dgvFines.Columns.Add("Remarks", "REMARKS");
            dgvFines.Columns.Add("Amount", "AMOUNT");
            dgvFines.Columns.Add("Status", "STATUS");
            dgvFines.Columns.Add("Actions", "ACTIONS");

            ApplyColumnStyle();

            dgvFines.CellPainting += DgvFines_CellPainting;
            dgvFines.MouseClick += DgvFines_MouseClick;
            dgvFines.MouseMove += DgvFines_MouseMove;
            dgvFines.SelectionChanged += (s, e) => ClearGridSelection();

            lblFooter = new Label
            {
                Text = "Showing 0 of 0 entries",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            pagerPanel = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(210, 36)
            };
            BuildPager();

            tableCard.Controls.Add(lblTableTitle);
            tableCard.Controls.Add(searchPanel);
            tableCard.Controls.Add(dgvFines);
            tableCard.Controls.Add(lblFooter);
            tableCard.Controls.Add(pagerPanel);
        }


        private void BuildPolicyTable()
        {
            policyCard = CreateRoundedCard();
            canvas.Controls.Add(policyCard);

            lblPolicyTitle = new Label
            {
                Text = "Fine Policy",
                AutoSize = true,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            dgvPolicies = new DataGridView
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
                ColumnHeadersHeight = 52,
                RowTemplate = { Height = 68 },
                GridColor = LineSoft,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical,
                ShowCellToolTips = false
            };

            dgvPolicies.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            dgvPolicies.ColumnHeadersDefaultCellStyle.ForeColor = SecondaryText;
            dgvPolicies.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPolicies.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBack;
            dgvPolicies.ColumnHeadersDefaultCellStyle.SelectionForeColor = SecondaryText;
            dgvPolicies.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgvPolicies.DefaultCellStyle.BackColor = CardBack;
            dgvPolicies.DefaultCellStyle.ForeColor = PrimaryText;
            dgvPolicies.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            dgvPolicies.DefaultCellStyle.SelectionBackColor = CardBack;
            dgvPolicies.DefaultCellStyle.SelectionForeColor = PrimaryText;
            dgvPolicies.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            dgvPolicies.Columns.Add("PolicyId", "POLICY ID");
            dgvPolicies.Columns.Add("FineType", "FINE TYPE");
            dgvPolicies.Columns.Add("DefaultAmount", "DEFAULT AMOUNT");
            dgvPolicies.Columns.Add("Description", "DESCRIPTION");
            dgvPolicies.Columns.Add("AddedBy", "ADDED BY");
            dgvPolicies.Columns.Add("Actions", "ACTIONS");

            dgvPolicies.Columns["PolicyId"].FillWeight = 12;
            dgvPolicies.Columns["FineType"].FillWeight = 14;
            dgvPolicies.Columns["DefaultAmount"].FillWeight = 16;
            dgvPolicies.Columns["Description"].FillWeight = 34;
            dgvPolicies.Columns["AddedBy"].FillWeight = 16;
            dgvPolicies.Columns["Actions"].FillWeight = 10;

            foreach (DataGridViewColumn col in dgvPolicies.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPolicies.CellPainting += DgvPolicies_CellPainting;
            dgvPolicies.MouseClick += DgvPolicies_MouseClick;
            dgvPolicies.MouseMove += DgvPolicies_MouseMove;
            dgvPolicies.SelectionChanged += (s, e) => ClearPolicyGridSelection();

            lblPolicyFooter = new Label
            {
                Text = "Showing 0 fine policies",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            policyCard.Controls.Add(lblPolicyTitle);
            policyCard.Controls.Add(dgvPolicies);
            policyCard.Controls.Add(lblPolicyFooter);
        }

        private void LoadFinePolicyData(SqlConnection conn)
        {
            finePolicies.Clear();

            const string query = @"
SELECT PolicyID, FineType, DefaultAmount, Description, AddedBy, CreatedAt
FROM dbo.FinePolicies
WHERE IsArchived = 0
  AND ClientID = @ClientID
ORDER BY PolicyID DESC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                finePolicies.Add(new FinePolicyRow
                {
                    PolicyId = Convert.ToInt32(reader["PolicyID"]),
                    FineType = Convert.ToString(reader["FineType"]) ?? "",
                    DefaultAmount = reader["DefaultAmount"] == DBNull.Value ? 0M : Convert.ToDecimal(reader["DefaultAmount"]),
                    Description = Convert.ToString(reader["Description"]) ?? "",
                    AddedBy = Convert.ToString(reader["AddedBy"]) ?? "",
                    CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            RebuildPolicyRows();
        }

        private void RebuildPolicyRows()
        {
            if (dgvPolicies == null)
                return;

            dgvPolicies.Rows.Clear();

            foreach (FinePolicyRow policy in finePolicies)
            {
                int rowIndex = dgvPolicies.Rows.Add(
                    "FP-" + policy.PolicyId.ToString("0000"),
                    policy.FineType,
                    policy.DefaultAmount.ToString("₱#,##0.00"),
                    policy.Description,
                    policy.AddedBy,
                    ""
                );

                dgvPolicies.Rows[rowIndex].Tag = policy;
            }

            lblPolicyFooter.Text = $"Showing {finePolicies.Count:N0} fine policies";
            ClearPolicyGridSelection();
        }

        private void ShowFinePolicyDialog(FinePolicyRow? row)
        {
            FinePolicyDialogData? existing = row == null ? null : new FinePolicyDialogData
            {
                PolicyId = row.PolicyId,
                FineType = row.FineType,
                DefaultAmount = row.DefaultAmount,
                Description = row.Description,
                AddedBy = row.AddedBy
            };

            using FinePolicyDialog dialog = new FinePolicyDialog(GetRecordedBy(), existing);

            if (ShowFinePolicyDialogCentered(dialog) != DialogResult.OK)
                return;

            try
            {
                if (row == null)
                    InsertFinePolicy(dialog.PolicyData);
                else
                    UpdateFinePolicy(dialog.PolicyData);

                LoadFineData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Fine policy could not be saved.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private DialogResult ShowFinePolicyDialogCentered(FinePolicyDialog dialog)
        {
            Form? owner = FindForm();

            if (owner == null)
                return dialog.ShowDialog(this);

            dialog.StartPosition = FormStartPosition.CenterParent;
            return dialog.ShowDialog(owner);
        }

        private void InsertFinePolicy(FinePolicyDialogData data)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();
            EnsureFinePoliciesTable(conn);

            const string query = @"
INSERT INTO dbo.FinePolicies
    (ClientID, FineType, DefaultAmount, Description, AddedBy, CreatedAt, IsArchived)
VALUES
    (@ClientID, @FineType, @DefaultAmount, @Description, @AddedBy, SYSUTCDATETIME(), 0);";

            using SqlCommand cmd = new SqlCommand(query, conn);
            AddFinePolicyParameters(cmd, data);
            cmd.ExecuteNonQuery();
        }

        private void UpdateFinePolicy(FinePolicyDialogData data)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();
            EnsureFinePoliciesTable(conn);

            const string query = @"
UPDATE dbo.FinePolicies
SET FineType = @FineType,
    DefaultAmount = @DefaultAmount,
    Description = @Description,
    AddedBy = @AddedBy,
    UpdatedAt = SYSUTCDATETIME()
WHERE PolicyID = @PolicyID
  AND ClientID = @ClientID
  AND IsArchived = 0;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            AddFinePolicyParameters(cmd, data);
            cmd.Parameters.AddWithValue("@PolicyID", data.PolicyId);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            cmd.ExecuteNonQuery();
        }

        private void ArchiveFinePolicy(FinePolicyRow row)
        {
            DialogResult confirm = MessageBox.Show(
                $"Archive fine policy {row.FineType}?",
                "Archive Fine Policy",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureFinePoliciesTable(conn);

                const string query = @"
UPDATE dbo.FinePolicies
SET IsArchived = 1,
    UpdatedAt = SYSUTCDATETIME()
WHERE PolicyID = @PolicyID
  AND ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PolicyID", row.PolicyId);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                cmd.ExecuteNonQuery();

                LoadFineData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Fine policy was not archived.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AddFinePolicyParameters(SqlCommand cmd, FinePolicyDialogData data)
        {
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            cmd.Parameters.AddWithValue("@FineType", data.FineType);
            cmd.Parameters.AddWithValue("@DefaultAmount", data.DefaultAmount);
            cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(data.Description) ? (object)DBNull.Value : data.Description);
            cmd.Parameters.AddWithValue("@AddedBy", data.AddedBy);
        }

        private void LoadFineData()
        {
            fineRecords.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureFineRecordsTable(conn);
                EnsureBooksTable(conn);
                EnsureReturnRecordsTable(conn);
                EnsureFinePoliciesTable(conn);
                LoadDialogOptions(conn);

                const string query = @"
SELECT FineID, BorrowID, MemberID, MemberName, BookID, BookTitle, Reason, Amount, Status, Remarks, RecordedBy, CreatedAt
FROM dbo.FineRecords
WHERE IsArchived = 0
  AND ClientID = @ClientID
  AND (
        UPPER(Reason) IN ('DAMAGED', 'LOST', 'LATE RETURN')
        OR UPPER(Reason) LIKE '%DAMAGE%'
        OR UPPER(Reason) LIKE '%DAMAGED%'
        OR UPPER(Reason) LIKE '%LOST%'
        OR UPPER(Reason) LIKE '%LATE%'
      )
ORDER BY FineID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    fineRecords.Add(new FineRow
                    {
                        FineId = Convert.ToInt32(reader["FineID"]),
                        BorrowId = reader["BorrowID"] == DBNull.Value ? null : Convert.ToInt32(reader["BorrowID"]),
                        MemberId = reader["MemberID"] == DBNull.Value ? null : Convert.ToInt32(reader["MemberID"]),
                        MemberName = Convert.ToString(reader["MemberName"]) ?? "",
                        BookId = Convert.ToString(reader["BookID"]) ?? "",
                        BookTitle = Convert.ToString(reader["BookTitle"]) ?? "",
                        Reason = Convert.ToString(reader["Reason"]) ?? "",
                        Amount = reader["Amount"] == DBNull.Value ? 0M : Convert.ToDecimal(reader["Amount"]),
                        Status = Convert.ToString(reader["Status"]) ?? "Unpaid",
                        Remarks = Convert.ToString(reader["Remarks"]) ?? "",
                        RecordedBy = Convert.ToString(reader["RecordedBy"]) ?? "",
                        CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(reader["CreatedAt"])
                    });
                }

                RebuildFineRows(GetCurrentSearchText());
                LoadFinePolicyData(conn);
                UpdateMetricCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Fine records could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadDialogOptions(SqlConnection conn)
        {
            memberOptions.Clear();
            bookOptions.Clear();

            const string query = @"
SELECT
    r.ReturnID,
    r.BorrowID,
    ISNULL(b.MemberID, 0) AS MemberID,
    r.MemberName,
    ISNULL(u.Role, 'Member') AS Role,
    COALESCE(NULLIF(b.BookID, ''), NULLIF(rb.BookID, ''), NULLIF(bookMatch.BookID, ''), '') AS BookID,
    r.BookTitle,
    ISNULL(r.BookCondition, '') AS BookCondition,
    ISNULL(r.ReturnStatus, '') AS ReturnStatus,
    ISNULL(r.DaysOverdue, 0) AS DaysOverdue,
    ISNULL(r.FineAmount, 0) AS FineAmount
FROM dbo.ReturnRecords r
LEFT JOIN dbo.BorrowingRecords b ON r.BorrowID = b.BorrowID AND b.ClientID = @ClientID
LEFT JOIN dbo.Users u ON b.MemberID = u.UserID
OUTER APPLY
(
    SELECT TOP 1 br.BookID
    FROM dbo.BorrowingRecords br
    WHERE br.BookTitle = r.BookTitle
      AND br.MemberID = b.MemberID
      AND br.ClientID = @ClientID
      AND NULLIF(br.BookID, '') IS NOT NULL
    ORDER BY br.BorrowID DESC
) rb
OUTER APPLY
(
    SELECT TOP 1 bk.BookID
    FROM dbo.Books bk
    WHERE bk.BookTitle = r.BookTitle
      AND bk.ClientID = @ClientID
      AND ISNULL(bk.IsArchived, 0) = 0
      AND NULLIF(bk.BookID, '') IS NOT NULL
    ORDER BY bk.BookID DESC
) bookMatch
WHERE ISNULL(r.IsArchived, 0) = 0
  AND r.ClientID = @ClientID
  AND (
        UPPER(ISNULL(r.BookCondition, '')) IN ('DAMAGED', 'DAMAGE', 'LOST')
        OR UPPER(ISNULL(r.BookCondition, '')) LIKE '%DAMAGE%'
        OR UPPER(ISNULL(r.BookCondition, '')) LIKE '%LOST%'
        OR UPPER(ISNULL(r.ReturnStatus, '')) LIKE '%LATE%'
        OR UPPER(ISNULL(r.ReturnStatus, '')) LIKE '%OVERDUE%'
        OR ISNULL(r.DaysOverdue, 0) > 0
      )
  AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.FineRecords f
          WHERE ISNULL(f.IsArchived, 0) = 0
            AND f.BorrowID = r.BorrowID
      )
ORDER BY r.ReturnDate DESC, r.ReturnID DESC;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int? borrowId = reader["BorrowID"] == DBNull.Value ? null : Convert.ToInt32(reader["BorrowID"]);
                int memberId = reader["MemberID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MemberID"]);

                string memberName = Convert.ToString(reader["MemberName"]) ?? "";
                string role = Convert.ToString(reader["Role"]) ?? "Member";
                string bookId = Convert.ToString(reader["BookID"]) ?? "";
                string bookTitle = Convert.ToString(reader["BookTitle"]) ?? "";
                string condition = Convert.ToString(reader["BookCondition"]) ?? "";
                string returnStatus = Convert.ToString(reader["ReturnStatus"]) ?? "";
                int daysOverdue = reader["DaysOverdue"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DaysOverdue"]);
                decimal storedFine = reader["FineAmount"] == DBNull.Value ? 0M : Convert.ToDecimal(reader["FineAmount"]);

                string reason = GetFineReasonFromReturn(condition, returnStatus, daysOverdue);
                decimal amount = GetSuggestedFineAmount(reason, daysOverdue, storedFine);

                memberOptions.Add(new FineMemberOption(
                    memberId,
                    memberName,
                    role,
                    borrowId,
                    bookId,
                    bookTitle,
                    reason,
                    amount,
                    true
                ));

                if (!string.IsNullOrWhiteSpace(bookId) &&
                    !bookOptions.Any(book => book.BookId.Equals(bookId, StringComparison.OrdinalIgnoreCase)))
                {
                    bookOptions.Add(new FineBookOption(bookId, bookTitle));
                }
            }

            const string allBooksQuery = @"
SELECT BookID, BookTitle
FROM dbo.Books
WHERE ISNULL(IsArchived, 0) = 0
  AND ClientID = @ClientID
ORDER BY BookTitle ASC;";

            using SqlCommand booksCmd = new SqlCommand(allBooksQuery, conn);
            booksCmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            using SqlDataReader booksReader = booksCmd.ExecuteReader();

            while (booksReader.Read())
            {
                string bookId = Convert.ToString(booksReader["BookID"]) ?? "";
                string bookTitle = Convert.ToString(booksReader["BookTitle"]) ?? "";

                if (!string.IsNullOrWhiteSpace(bookId) &&
                    !bookOptions.Any(book => book.BookId.Equals(bookId, StringComparison.OrdinalIgnoreCase)))
                {
                    bookOptions.Add(new FineBookOption(bookId, bookTitle));
                }
            }
        }

        private string GetFineReasonFromReturn(string condition, string returnStatus, int daysOverdue)
        {
            string c = (condition ?? "").Trim().ToUpperInvariant();
            string s = (returnStatus ?? "").Trim().ToUpperInvariant();

            if (c.Contains("LOST"))
                return "LOST";

            if (c.Contains("DAMAGE") || c.Contains("DAMAGED"))
                return "DAMAGED";

            if (daysOverdue > 0 || s.Contains("LATE") || s.Contains("OVERDUE"))
                return "LATE RETURN";

            return "LATE RETURN";
        }

        private decimal GetSuggestedFineAmount(string reason, int daysOverdue, decimal storedFine)
        {
            if (storedFine > 0)
                return storedFine;

            if (reason.Equals("LOST", StringComparison.OrdinalIgnoreCase))
                return 500M;

            if (reason.Equals("DAMAGED", StringComparison.OrdinalIgnoreCase))
                return 200M;

            return Math.Max(1, daysOverdue) * 10M;
        }

        private void ShowFineDialog(FineRow? row)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureFineRecordsTable(conn);
                EnsureBooksTable(conn);
                EnsureReturnRecordsTable(conn);
                EnsureFinePoliciesTable(conn);
                LoadDialogOptions(conn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Fine form choices could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            FineDialogData? existing = row == null ? null : new FineDialogData
            {
                FineId = row.FineId,
                BorrowId = row.BorrowId,
                MemberId = row.MemberId,
                MemberName = row.MemberName,
                BookId = row.BookId,
                BookTitle = row.BookTitle,
                Reason = row.Reason,
                Amount = row.Amount,
                Status = row.Status,
                Remarks = row.Remarks,
                RecordedBy = row.RecordedBy
            };

            using ApplyFineDialog dialog = new ApplyFineDialog(
                row == null ? "FN-[Auto]" : FormatFineCode(row.FineId),
                GetRecordedBy(),
                memberOptions,
                bookOptions,
                existing
            );

            if (ShowFineDialogWithOverlay(dialog) != DialogResult.OK)
                return;

            try
            {
                if (row == null)
                    InsertFineRecord(dialog.FineData);
                else
                    UpdateFineRecord(dialog.FineData);

                LoadFineData();
                ClearGridSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Fine record was not saved to the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private DialogResult ShowFineDialogWithOverlay(ApplyFineDialog dialog)
        {
            Form? owner = FindForm();

            if (owner == null)
                return dialog.ShowDialog(this);

            dialog.StartPosition = FormStartPosition.CenterParent;
            return dialog.ShowDialog(owner);
        }

        private void InsertFineRecord(FineDialogData data)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();
            EnsureFineRecordsTable(conn);

            const string query = @"
INSERT INTO dbo.FineRecords
    (ClientID, BorrowID, MemberID, MemberName, BookID, BookTitle, Reason, Amount, Status, Remarks, RecordedBy, CreatedAt, IsArchived)
VALUES
    (@ClientID, @BorrowID, @MemberID, @MemberName, @BookID, @BookTitle, @Reason, @Amount, @Status, @Remarks, @RecordedBy, SYSUTCDATETIME(), 0);";

            using SqlCommand cmd = new SqlCommand(query, conn);
            AddFineParameters(cmd, data);
            cmd.ExecuteNonQuery();
        }

        private void UpdateFineRecord(FineDialogData data)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();
            EnsureFineRecordsTable(conn);

            const string query = @"
UPDATE dbo.FineRecords
SET BorrowID = @BorrowID,
    MemberID = @MemberID,
    MemberName = @MemberName,
    BookID = @BookID,
    BookTitle = @BookTitle,
    Reason = @Reason,
    Amount = @Amount,
    Status = @Status,
    Remarks = @Remarks,
    RecordedBy = @RecordedBy,
    UpdatedAt = SYSUTCDATETIME()
WHERE FineID = @FineID
  AND ClientID = @ClientID
  AND IsArchived = 0;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            AddFineParameters(cmd, data);
            cmd.Parameters.AddWithValue("@FineID", data.FineId);
            cmd.ExecuteNonQuery();
        }

        private void AddFineParameters(SqlCommand cmd, FineDialogData data)
        {
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            cmd.Parameters.AddWithValue("@BorrowID", data.BorrowId.HasValue ? data.BorrowId.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MemberID", data.MemberId.HasValue ? data.MemberId.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MemberName", data.MemberName);
            cmd.Parameters.AddWithValue("@BookID", string.IsNullOrWhiteSpace(data.BookId) ? (object)DBNull.Value : data.BookId);
            cmd.Parameters.AddWithValue("@BookTitle", string.IsNullOrWhiteSpace(data.BookTitle) ? (object)DBNull.Value : data.BookTitle);
            cmd.Parameters.AddWithValue("@Reason", data.Reason);
            cmd.Parameters.AddWithValue("@Amount", data.Amount);
            cmd.Parameters.AddWithValue("@Status", data.Status);
            cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(data.Remarks) ? (object)DBNull.Value : data.Remarks);
            cmd.Parameters.AddWithValue("@RecordedBy", data.RecordedBy);
        }


        private void EnsureFinePoliciesTable(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.FinePolicies', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.FinePolicies
    (
        PolicyID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        FineType NVARCHAR(50) NOT NULL,
        DefaultAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
        Description NVARCHAR(500) NULL,
        AddedBy NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.FinePolicies', 'ClientID') IS NULL
    ALTER TABLE dbo.FinePolicies ADD ClientID INT NULL;

IF COL_LENGTH('dbo.FinePolicies', 'FineType') IS NULL
    ALTER TABLE dbo.FinePolicies ADD FineType NVARCHAR(50) NOT NULL CONSTRAINT DF_FinePolicies_FineType DEFAULT 'Late';

IF COL_LENGTH('dbo.FinePolicies', 'DefaultAmount') IS NULL
    ALTER TABLE dbo.FinePolicies ADD DefaultAmount DECIMAL(10,2) NOT NULL CONSTRAINT DF_FinePolicies_DefaultAmount DEFAULT 0;

IF COL_LENGTH('dbo.FinePolicies', 'Description') IS NULL
    ALTER TABLE dbo.FinePolicies ADD Description NVARCHAR(500) NULL;

IF COL_LENGTH('dbo.FinePolicies', 'AddedBy') IS NULL
    ALTER TABLE dbo.FinePolicies ADD AddedBy NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.FinePolicies', 'CreatedAt') IS NULL
    ALTER TABLE dbo.FinePolicies ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_FinePolicies_CreatedAt DEFAULT SYSUTCDATETIME();

IF COL_LENGTH('dbo.FinePolicies', 'UpdatedAt') IS NULL
    ALTER TABLE dbo.FinePolicies ADD UpdatedAt DATETIME2 NULL;

IF COL_LENGTH('dbo.FinePolicies', 'IsArchived') IS NULL
    ALTER TABLE dbo.FinePolicies ADD IsArchived BIT NOT NULL CONSTRAINT DF_FinePolicies_IsArchived DEFAULT 0;

IF NOT EXISTS (SELECT 1 FROM dbo.FinePolicies WHERE ClientID = @ClientID AND FineType = 'LATE RETURN' AND IsArchived = 0)
    INSERT INTO dbo.FinePolicies (ClientID, FineType, DefaultAmount, Description, AddedBy)
    VALUES (@ClientID, 'LATE RETURN', 50.00, 'Default fine policy for late returns.', 'System');

IF NOT EXISTS (SELECT 1 FROM dbo.FinePolicies WHERE ClientID = @ClientID AND FineType = 'DAMAGED' AND IsArchived = 0)
    INSERT INTO dbo.FinePolicies (ClientID, FineType, DefaultAmount, Description, AddedBy)
    VALUES (@ClientID, 'DAMAGED', 200.00, 'Default fine policy for damaged books.', 'System');

IF NOT EXISTS (SELECT 1 FROM dbo.FinePolicies WHERE ClientID = @ClientID AND FineType = 'LOST' AND IsArchived = 0)
    INSERT INTO dbo.FinePolicies (ClientID, FineType, DefaultAmount, Description, AddedBy)
    VALUES (@ClientID, 'LOST', 500.00, 'Default fine policy for lost books.', 'System');";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            cmd.ExecuteNonQuery();
        }

        private void EnsureFineRecordsTable(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.FineRecords', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.FineRecords
    (
        FineID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        BorrowID INT NULL,
        MemberID INT NULL,
        MemberName NVARCHAR(150) NOT NULL,
        BookID NVARCHAR(20) NULL,
        BookTitle NVARCHAR(250) NULL,
        Reason NVARCHAR(150) NOT NULL,
        Amount DECIMAL(10,2) NOT NULL DEFAULT 0,
        Status NVARCHAR(40) NOT NULL DEFAULT 'Unpaid',
        Remarks NVARCHAR(500) NULL,
        RecordedBy NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.FineRecords', 'ClientID') IS NULL
    ALTER TABLE dbo.FineRecords ADD ClientID INT NULL;

IF COL_LENGTH('dbo.FineRecords', 'BorrowID') IS NULL
    ALTER TABLE dbo.FineRecords ADD BorrowID INT NULL;

IF COL_LENGTH('dbo.FineRecords', 'BookID') IS NULL
    ALTER TABLE dbo.FineRecords ADD BookID NVARCHAR(20) NULL;

IF COL_LENGTH('dbo.FineRecords', 'BookTitle') IS NULL
    ALTER TABLE dbo.FineRecords ADD BookTitle NVARCHAR(250) NULL;

IF COL_LENGTH('dbo.FineRecords', 'Remarks') IS NULL
    ALTER TABLE dbo.FineRecords ADD Remarks NVARCHAR(500) NULL;

IF COL_LENGTH('dbo.FineRecords', 'RecordedBy') IS NULL
    ALTER TABLE dbo.FineRecords ADD RecordedBy NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.FineRecords', 'UpdatedAt') IS NULL
    ALTER TABLE dbo.FineRecords ADD UpdatedAt DATETIME2 NULL;

IF COL_LENGTH('dbo.FineRecords', 'IsArchived') IS NULL
    ALTER TABLE dbo.FineRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_FineRecords_IsArchived DEFAULT 0;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
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
        BookCondition NVARCHAR(40) NOT NULL DEFAULT 'GOOD',
        ReturnStatus NVARCHAR(40) NOT NULL DEFAULT 'RETURNED',
        DaysOverdue INT NOT NULL DEFAULT 0,
        FineAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.ReturnRecords', 'ClientID') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD ClientID INT NULL;

IF COL_LENGTH('dbo.ReturnRecords', 'BookCondition') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD BookCondition NVARCHAR(40) NOT NULL CONSTRAINT DF_ReturnRecords_BookCondition DEFAULT 'GOOD';

IF COL_LENGTH('dbo.ReturnRecords', 'ReturnStatus') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD ReturnStatus NVARCHAR(40) NOT NULL CONSTRAINT DF_ReturnRecords_ReturnStatus DEFAULT 'RETURNED';

IF COL_LENGTH('dbo.ReturnRecords', 'DaysOverdue') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD DaysOverdue INT NOT NULL CONSTRAINT DF_ReturnRecords_DaysOverdue DEFAULT 0;

IF COL_LENGTH('dbo.ReturnRecords', 'FineAmount') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD FineAmount DECIMAL(10,2) NOT NULL CONSTRAINT DF_ReturnRecords_FineAmount DEFAULT 0;

IF COL_LENGTH('dbo.ReturnRecords', 'UpdatedAt') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD UpdatedAt DATETIME2 NULL;

IF COL_LENGTH('dbo.ReturnRecords', 'IsArchived') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD IsArchived BIT NOT NULL CONSTRAINT DF_ReturnRecords_IsArchived DEFAULT 0;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void EnsureBooksTable(SqlConnection conn)
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
            cmd.ExecuteNonQuery();
        }

        private void ArchiveFineRecord(FineRow row)
        {
            DialogResult confirm = MessageBox.Show(
                $"Archive fine {FormatFineCode(row.FineId)}?",
                "Archive Fine",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();
                EnsureFineRecordsTable(conn);

                const string query = @"
UPDATE dbo.FineRecords
SET IsArchived = 1,
    UpdatedAt = SYSUTCDATETIME()
WHERE FineID = @FineID
  AND ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FineID", row.FineId);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                cmd.ExecuteNonQuery();

                LoadFineData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Fine record was not archived.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void RebuildFineRows(string keyword)
        {
            if (dgvFines == null)
                return;

            string q = keyword.Trim().ToLowerInvariant();

            filteredFineRecords = fineRecords
                .Where(row =>
                    string.IsNullOrWhiteSpace(q) ||
                    $"{row.FineId} {row.MemberName} {row.BookId} {row.BookTitle} {row.Reason} {row.Remarks} {row.Amount} {row.Status}"
                        .ToLowerInvariant()
                        .Contains(q))
                .ToList();

            totalPages = Math.Max(1, (int)Math.Ceiling(filteredFineRecords.Count / (double)PageSize));

            if (currentPage > totalPages)
                currentPage = totalPages;

            if (currentPage < 1)
                currentPage = 1;

            RenderFinePage();
        }

        private void RenderFinePage()
        {
            dgvFines.Rows.Clear();

            List<FineRow> pageRows = filteredFineRecords
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (FineRow row in pageRows)
            {
                int rowIndex = dgvFines.Rows.Add(
                    FormatFineCode(row.FineId),
                    row.MemberName,
                    $"{row.BookId}\n{row.BookTitle}".Trim(),
                    row.Reason,
                    string.IsNullOrWhiteSpace(row.Remarks) ? "—" : row.Remarks,
                    row.Amount.ToString("₱#,##0.00"),
                    row.Status,
                    ""
                );

                dgvFines.Rows[rowIndex].Tag = row;
            }

            UpdateFineFooter(filteredFineRecords.Count, pageRows.Count);
            UpdatePagerButtons();
            ClearGridSelection();
        }

        private void UpdateFineFooter(int totalFilteredCount, int visiblePageCount)
        {
            if (lblFooter == null)
                return;

            if (fineRecords.Count == 0 || totalFilteredCount == 0)
            {
                lblFooter.Text = $"Showing 0 of {fineRecords.Count:N0} entries";
                return;
            }

            int start = ((currentPage - 1) * PageSize) + 1;
            int end = Math.Min(start + visiblePageCount - 1, totalFilteredCount);

            lblFooter.Text = $"Showing {start:N0} - {end:N0} of {totalFilteredCount:N0} entries";
        }

        private void UpdateMetricCards()
        {
            decimal unpaidTotal = fineRecords
                .Where(row => row.Status.Equals("Unpaid", StringComparison.OrdinalIgnoreCase))
                .Sum(row => row.Amount);

            int critical = fineRecords.Count(row =>
                row.Status.Equals("Unpaid", StringComparison.OrdinalIgnoreCase) &&
                row.CreatedAt.Date <= DateTime.Today.AddDays(-60));

            int paid = fineRecords.Count(row => row.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase));
            decimal efficiency = fineRecords.Count == 0 ? 0M : (decimal)paid / fineRecords.Count * 100M;

            SetMetricValue(cardUnpaid, unpaidTotal.ToString("₱#,##0.00"));
            SetMetricValue(cardCritical, critical.ToString("N0"));

            if (cardEfficiency.Tag is Control[] controls && controls.Length >= 4)
            {
                ((Label)controls[1]).Text = $"{efficiency:N1}%";

                Panel barFill = (Panel)controls[3];
                int fullWidth = Math.Max(0, cardEfficiency.Width - 52);
                barFill.Width = (int)(fullWidth * Math.Min(100M, efficiency) / 100M);
            }

            LayoutCards();
        }

        private void SetMetricValue(Panel card, string text)
        {
            if (card.Tag is Control[] controls && controls.Length > 1 && controls[1] is Label value)
                value.Text = text;
        }

        private string GetCurrentSearchText()
        {
            if (txtSearch == null || txtSearch.Text == SearchPlaceholder)
                return "";

            return txtSearch.Text;
        }

        private string GetRecordedBy()
        {
            if (!string.IsNullOrWhiteSpace(ClientSession.Username))
                return ClientSession.Username!;

            if (!string.IsNullOrWhiteSpace(UserSession.Username))
                return UserSession.Username!;

            return "ADM-088";
        }

        private static string FormatFineCode(int fineId)
        {
            return "#FN-" + fineId.ToString("0000");
        }

        private void ApplySearch(string keyword)
        {
            RebuildFineRows(keyword);
        }

        private void ApplyColumnStyle()
        {
            dgvFines.Columns["FineId"].FillWeight = 12;
            dgvFines.Columns["MemberName"].FillWeight = 18;
            dgvFines.Columns["BookInfo"].FillWeight = 20;
            dgvFines.Columns["Reason"].FillWeight = 14;
            dgvFines.Columns["Remarks"].FillWeight = 22;
            dgvFines.Columns["Amount"].FillWeight = 12;
            dgvFines.Columns["Status"].FillWeight = 11;
            dgvFines.Columns["Actions"].FillWeight = 9;

            dgvFines.Columns["Amount"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvFines.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvFines.Columns["Status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvFines.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvFines.Columns["Actions"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvFines.Columns["Actions"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvFines.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void DgvFines_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0)
                return;

            string col = dgv.Columns[e.ColumnIndex].Name;
            string text = e.FormattedValue?.ToString() ?? "";

            if (col == "FineId")
            {
                e.PaintBackground(e.CellBounds, true);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 18, e.CellBounds.Y + 25, e.CellBounds.Width - 24, 26),
                    AccentDeep,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "MemberName")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle avatar = new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 22, 34, 34);

                using (SolidBrush b = new SolidBrush(HeaderBack))
                    e.Graphics.FillEllipse(b, avatar);

                TextRenderer.DrawText(
                    e.Graphics,
                    GetInitials(text),
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    avatar,
                    SecondaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 58, e.CellBounds.Y + 25, e.CellBounds.Width - 64, 26),
                    PrimaryText,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (col == "BookInfo")
            {
                e.PaintBackground(e.CellBounds, true);

                string[] parts = text.Split('\n');
                string bookId = parts.Length > 0 ? parts[0] : "";
                string title = parts.Length > 1 ? parts[1] : "";

                TextRenderer.DrawText(
                    e.Graphics,
                    bookId,
                    new Font("Segoe UI", 9F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 17, e.CellBounds.Width - 24, 24),
                    AccentDeep,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                TextRenderer.DrawText(
                    e.Graphics,
                    title,
                    new Font("Segoe UI", 9.5F),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 40, e.CellBounds.Width - 24, 24),
                    SecondaryText,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (col == "Remarks")
            {
                e.PaintBackground(e.CellBounds, true);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 9.5F),
                    new Rectangle(e.CellBounds.X + 12, e.CellBounds.Y + 18, e.CellBounds.Width - 24, e.CellBounds.Height - 28),
                    SecondaryText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                e.Handled = true;
            }
            else if (col == "Amount")
            {
                e.PaintBackground(e.CellBounds, true);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 11F, FontStyle.Bold),
                    e.CellBounds,
                    PrimaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                bool unpaid = text.Equals("Unpaid", StringComparison.OrdinalIgnoreCase);
                Color bg = unpaid ? AccentDangerBg : AccentSoft;
                Color fg = unpaid ? AccentDanger : AccentDeep;

                Size sz = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - (sz.Width + 24)) / 2,
                    e.CellBounds.Y + 27,
                    sz.Width + 24,
                    24
                );

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
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);
                var actionBounds = GetActionIconBounds(e.CellBounds);

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
                    SecondaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
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

        private void DgvFines_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            if (dgv.Columns[hit.ColumnIndex].Name != "Actions")
                return;

            if (dgv.Rows[hit.RowIndex].Tag is not FineRow row)
                return;

            Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
            if (!cellBounds.Contains(e.Location))
                return;

            var actionBounds = GetActionIconBounds(cellBounds);
            if (actionBounds.Edit.Contains(e.Location))
                ShowFineDialog(row);
            else if (actionBounds.Archive.Contains(e.Location))
                ArchiveFineRecord(row);
        }

        private void DgvFines_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
            bool overActionIcon = false;

            if (hit.RowIndex >= 0 &&
                hit.ColumnIndex >= 0 &&
                dgv.Columns[hit.ColumnIndex].Name == "Actions")
            {
                Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
                var actionBounds = GetActionIconBounds(cellBounds);
                overActionIcon = actionBounds.Edit.Contains(e.Location) ||
                                 actionBounds.Archive.Contains(e.Location);
            }

            dgv.Cursor = overActionIcon ? Cursors.Hand : Cursors.Default;
        }


        private void DgvPolicies_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (sender is not DataGridView dgv || e.RowIndex < 0)
                return;

            string col = dgv.Columns[e.ColumnIndex].Name;
            string text = e.FormattedValue?.ToString() ?? "";

            if (col == "PolicyId")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    new Rectangle(e.CellBounds.X + 18, e.CellBounds.Y + 21, e.CellBounds.Width - 24, 26),
                    AccentDeep,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                e.Handled = true;
            }
            else if (col == "FineType")
            {
                e.PaintBackground(e.CellBounds, true);
                Color bg = HeaderBack;
                Color fg = AccentDeep;

                Size sz = TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(e.CellBounds.X + 14, e.CellBounds.Y + 22, Math.Min(e.CellBounds.Width - 20, sz.Width + 24), 26);

                using (SolidBrush b = new SolidBrush(bg))
                    e.Graphics.FillRectangle(b, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    badge,
                    fg,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                e.Handled = true;
            }
            else if (col == "DefaultAmount")
            {
                e.PaintBackground(e.CellBounds, true);
                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    e.CellBounds,
                    PrimaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                e.Handled = true;
            }
            else if (col == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);
                var actionBounds = GetActionIconBounds(e.CellBounds);

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
                    SecondaryText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private void DgvPolicies_MouseClick(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            if (dgv.Columns[hit.ColumnIndex].Name != "Actions")
                return;

            if (dgv.Rows[hit.RowIndex].Tag is not FinePolicyRow row)
                return;

            Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
            var actionBounds = GetActionIconBounds(cellBounds);

            if (actionBounds.Edit.Contains(e.Location))
                ShowFinePolicyDialog(row);
            else if (actionBounds.Archive.Contains(e.Location))
                ArchiveFinePolicy(row);
        }

        private void DgvPolicies_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
                return;

            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);
            bool overActionIcon = false;

            if (hit.RowIndex >= 0 &&
                hit.ColumnIndex >= 0 &&
                dgv.Columns[hit.ColumnIndex].Name == "Actions")
            {
                Rectangle cellBounds = dgv.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, false);
                var actionBounds = GetActionIconBounds(cellBounds);
                overActionIcon = actionBounds.Edit.Contains(e.Location) ||
                                 actionBounds.Archive.Contains(e.Location);
            }

            dgv.Cursor = overActionIcon ? Cursors.Hand : Cursors.Default;
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
                RenderFinePage();
            };

            btnNextPage.Click += (s, e) =>
            {
                if (currentPage >= totalPages) return;
                currentPage++;
                RenderFinePage();
            };

            btnPage1.Click += (s, e) => GoToFinePage(btnPage1);
            btnPage2.Click += (s, e) => GoToFinePage(btnPage2);
            btnPage3.Click += (s, e) => GoToFinePage(btnPage3);

            pagerPanel.Controls.Add(btnPrevPage);
            pagerPanel.Controls.Add(btnPage1);
            pagerPanel.Controls.Add(btnPage2);
            pagerPanel.Controls.Add(btnPage3);
            pagerPanel.Controls.Add(btnNextPage);

            UpdatePagerButtons();
        }

        private void GoToFinePage(Button button)
        {
            if (button.Tag == null)
                return;

            if (int.TryParse(button.Tag.ToString(), out int page))
            {
                currentPage = page;
                RenderFinePage();
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
            button.ForeColor = active ? Color.White : enabled ? PrimaryText : Color.FromArgb(150, 165, 170);
            button.FlatAppearance.BorderSize = active ? 0 : 1;
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 26;
            int width = Math.Max(980, canvas.ClientSize.Width - margin * 2);

            lblTitle.Location = new Point(margin, 34);
            lblSubtitle.Location = new Point(margin, 82);

            btnApplyFine.Bounds = new Rectangle(width - 175 + margin, 50, 175, 52);
            btnCreateFine.Bounds = new Rectangle(btnApplyFine.Left - 190, 50, 175, 52);

            int cardTop = 132;
            int cardHeight = 170;
            int totalWidth = width - (gap * 2);
            int cardUnpaidWidth = (int)(totalWidth * 0.30);
            int cardCriticalWidth = (int)(totalWidth * 0.30);
            int cardEfficiencyWidth = totalWidth - cardUnpaidWidth - cardCriticalWidth;

            cardUnpaid.Bounds = new Rectangle(margin, cardTop, cardUnpaidWidth, cardHeight);
            cardCritical.Bounds = new Rectangle(cardUnpaid.Right + gap, cardTop, cardCriticalWidth, cardHeight);
            cardEfficiency.Bounds = new Rectangle(cardCritical.Right + gap, cardTop, cardEfficiencyWidth, cardHeight);

            LayoutCards();

            tableCard.Bounds = new Rectangle(margin, 340, width, 585);

            lblTableTitle.Location = new Point(26, 28);

            searchPanel.Bounds = new Rectangle(tableCard.Width - 300, 22, 270, 40);
            searchPanel.Controls[0].Location = new Point(12, 8);
            txtSearch.Location = new Point(42, 10);
            txtSearch.Width = searchPanel.Width - 54;

            dgvFines.Location = new Point(0, 82);
            dgvFines.Size = new Size(tableCard.Width, 358);

            lblFooter.Location = new Point(26, tableCard.Height - 48);
            pagerPanel.Location = new Point(tableCard.Width - pagerPanel.Width - 28, tableCard.Height - 56);

            policyCard.Bounds = new Rectangle(margin, tableCard.Bottom + 28, width, 360);
            lblPolicyTitle.Location = new Point(26, 24);
            dgvPolicies.Location = new Point(0, 76);
            dgvPolicies.Size = new Size(policyCard.Width, 230);
            lblPolicyFooter.Location = new Point(26, policyCard.Height - 42);

            canvas.AutoScrollMinSize = new Size(0, policyCard.Bottom + 30);
        }

        private void LayoutCards()
        {
            if (cardUnpaid.Tag is Control[] u)
            {
                u[0].Location = new Point(26, 26);
                u[1].Location = new Point(26, 62);
                u[2].Location = new Point(26, 126);
            }

            if (cardCritical.Tag is Control[] c)
            {
                c[0].Location = new Point(26, 26);
                c[1].Location = new Point(26, 62);
                c[2].Location = new Point(26, 126);
            }

            if (cardEfficiency.Tag is Control[] e)
            {
                e[0].Location = new Point(26, 36);
                e[1].Location = new Point(26, 72);

                Panel barBack = (Panel)e[2];
                Panel barFill = (Panel)e[3];

                int fullWidth = Math.Max(0, cardEfficiency.Width - 52);
                decimal efficiency = fineRecords.Count == 0
                    ? 0M
                    : (decimal)fineRecords.Count(row => row.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase)) / fineRecords.Count * 100M;

                barBack.Bounds = new Rectangle(26, 130, fullWidth, 8);
                barFill.Bounds = new Rectangle(26, 130, (int)(fullWidth * Math.Min(100M, efficiency) / 100M), 8);
            }
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
            if (dgvFines == null) return;
            dgvFines.ClearSelection();
            dgvFines.CurrentCell = null;
        }

        private void ClearPolicyGridSelection()
        {
            if (dgvPolicies == null) return;
            dgvPolicies.ClearSelection();
            dgvPolicies.CurrentCell = null;
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
