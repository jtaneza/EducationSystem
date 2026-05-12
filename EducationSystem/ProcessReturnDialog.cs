using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public sealed class ReturnLoanOption
    {
        public int BorrowId { get; }
        public string MemberName { get; }
        public string BookTitle { get; }
        public DateTime DueDate { get; }

        public ReturnLoanOption(int borrowId, string memberName, string bookTitle, DateTime dueDate)
        {
            BorrowId = borrowId;
            MemberName = memberName;
            BookTitle = bookTitle;
            DueDate = dueDate.Date;
        }

        public override string ToString()
        {
            return $"{MemberName} - {BookTitle}";
        }
    }

    public sealed class ReturnDialogData
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

    public sealed class ProcessReturnDialog : Form
    {
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color FieldBack = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color SoftBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color CardBack = Color.White;
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private readonly List<ReturnLoanOption> loanOptions;
        private readonly ReturnDialogData? existingReturn;

        private ComboBox cboMember = null!;
        private TextBox txtBookTitle = null!;
        private DateTimePicker dtpReturnDate = null!;
        private ComboBox cboCondition = null!;
        private Label lblDaysOverdue = null!;
        private Label lblFineReason = null!;
        private Label lblFineAmount = null!;
        private Button btnComplete = null!;

        private DateTime? selectedDueDate;
        private int currentDaysOverdue;
        private string currentFineReason = "NONE";
        private decimal currentFineAmount;

        public ReturnDialogData ReturnData { get; private set; } = new ReturnDialogData();

        public ProcessReturnDialog(IEnumerable<ReturnLoanOption> loanOptions, ReturnDialogData? existingReturn = null)
        {
            this.loanOptions = loanOptions.ToList();
            this.existingReturn = existingReturn;

            Text = existingReturn == null ? "Process Book Return" : "Edit Book Return";
            ClientSize = new Size(640, 610);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = CardBack;
            ShowInTaskbar = false;
            DoubleBuffered = true;

            BuildDialog();
            LoadValues();
            RecalculateFine();
        }

        private void BuildDialog()
        {
            Panel header = new Panel
            {
                BackColor = HeaderBack,
                Bounds = new Rectangle(0, 0, ClientSize.Width, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label title = CreateLabel(
                existingReturn == null ? "Process Book Return" : "Edit Book Return",
                20F,
                FontStyle.Bold,
                PrimaryText);
            title.Location = new Point(36, 24);

            Label subtitle = CreateLabel("ABC School Library - Returns Management", 10F, FontStyle.Regular, SecondaryText);
            subtitle.Location = new Point(36, 58);

            Label close = new Label
            {
                Text = "x",
                AutoSize = false,
                Size = new Size(34, 34),
                Location = new Point(ClientSize.Width - 56, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Regular),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            close.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            header.Controls.Add(close);
            Controls.Add(header);

            int left = 36;
            int top = 122;
            int fieldWidth = 265;
            int fieldHeight = 30;
            int gap = 28;
            int right = left + fieldWidth + gap;

            AddLabel("MEMBER NAME / ID", left, top);
            cboMember = new ComboBox
            {
                Bounds = new Rectangle(left, top + 28, fieldWidth, fieldHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDown
            };
            cboMember.Items.AddRange(loanOptions.Cast<object>().ToArray());
            cboMember.SelectedIndexChanged += (s, e) => ApplySelectedLoan();
            Controls.Add(cboMember);

            AddLabel("BOOK TITLE / ID", right, top);
            txtBookTitle = CreateTextBox("Search book title...");
            txtBookTitle.Bounds = new Rectangle(right, top + 28, fieldWidth, fieldHeight);
            Controls.Add(txtBookTitle);

            top += 92;

            AddLabel("RETURN DATE", left, top);
            dtpReturnDate = new DateTimePicker
            {
                Bounds = new Rectangle(left, top + 28, fieldWidth, fieldHeight),
                CalendarForeColor = PrimaryText,
                CalendarMonthBackground = FieldBack,
                Font = new Font("Segoe UI", 11F),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "MMM dd, yyyy",
                Value = DateTime.Today
            };
            dtpReturnDate.ValueChanged += (s, e) => RecalculateFine();
            Controls.Add(dtpReturnDate);

            AddLabel("BOOK CONDITION", right, top);
            cboCondition = new ComboBox
            {
                Bounds = new Rectangle(right, top + 28, fieldWidth, fieldHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboCondition.Items.AddRange(new object[] { "GOOD", "DAMAGED", "LOST" });
            cboCondition.SelectedIndex = 0;
            cboCondition.SelectedIndexChanged += (s, e) => RecalculateFine();
            Controls.Add(cboCondition);

            Panel summary = new Panel
            {
                BackColor = SoftBack,
                Bounds = new Rectangle(left, 326, ClientSize.Width - 72, 150)
            };
            summary.Paint += RoundedPanelPaint;

            Label summaryIcon = new Label
            {
                Text = "!",
                AutoSize = false,
                Bounds = new Rectangle(28, 28, 44, 44),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = ColorTranslator.FromHtml("#B7EBD7"),
                ForeColor = AccentDeep,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold)
            };

            Label summaryTitle = CreateLabel("Fine Summary", 15F, FontStyle.Bold, PrimaryText);
            summaryTitle.Location = new Point(92, 28);

            Label daysLabel = CreateLabel("Days Overdue", 10.5F, FontStyle.Regular, SecondaryText);
            daysLabel.Location = new Point(92, 60);

            lblDaysOverdue = CreateLabel("0 days", 10.5F, FontStyle.Bold, PrimaryText);
            lblDaysOverdue.AutoSize = false;
            lblDaysOverdue.TextAlign = ContentAlignment.MiddleRight;
            lblDaysOverdue.Bounds = new Rectangle(summary.Width - 170, 56, 138, 24);

            Label amountLabel = CreateLabel("Estimated Fine", 10.5F, FontStyle.Regular, SecondaryText);
            amountLabel.Location = new Point(92, 94);

            lblFineAmount = CreateLabel("₱0.00", 10.5F, FontStyle.Bold, AccentDeep);
            lblFineAmount.AutoSize = false;
            lblFineAmount.TextAlign = ContentAlignment.MiddleRight;
            lblFineAmount.Bounds = new Rectangle(summary.Width - 170, 104, 138, 24);

            lblFineReason = CreateLabel("Reason: No fine", 9.5F, FontStyle.Bold, AccentDeep);
            lblFineReason.AutoSize = false;
            lblFineReason.TextAlign = ContentAlignment.MiddleLeft;
            lblFineReason.Bounds = new Rectangle(92, 122, summary.Width - 265, 24);

            summary.Controls.Add(summaryIcon);
            summary.Controls.Add(summaryTitle);
            summary.Controls.Add(lblFineReason);
            summary.Controls.Add(daysLabel);
            summary.Controls.Add(lblDaysOverdue);
            summary.Controls.Add(amountLabel);
            summary.Controls.Add(lblFineAmount);
            Controls.Add(summary);

            Panel footer = new Panel
            {
                BackColor = HeaderBack,
                Bounds = new Rectangle(0, ClientSize.Height - 78, ClientSize.Width, 78),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(112, 42),
                Location = new Point(ClientSize.Width - 36 - 170 - 14 - 112, 18),
                FlatStyle = FlatStyle.Flat,
                BackColor = HeaderBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            btnComplete = new Button
            {
                Text = existingReturn == null ? "Complete Return" : "Save Changes",
                Size = new Size(170, 42),
                Location = new Point(ClientSize.Width - 36 - 170, 18),
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnComplete.FlatAppearance.BorderSize = 0;
            btnComplete.Click += Complete_Click;

            footer.Controls.Add(btnCancel);
            footer.Controls.Add(btnComplete);
            Controls.Add(footer);

            AcceptButton = btnComplete;
            CancelButton = btnCancel;
        }

        private void LoadValues()
        {
            if (existingReturn == null)
                return;

            ReturnLoanOption? selected = existingReturn.BorrowId.HasValue
                ? loanOptions.FirstOrDefault(option => option.BorrowId == existingReturn.BorrowId.Value)
                : null;

            if (selected != null)
            {
                cboMember.SelectedItem = selected;
                selectedDueDate = selected.DueDate;
            }
            else
            {
                cboMember.Text = existingReturn.MemberName;
            }

            txtBookTitle.Text = existingReturn.BookTitle;
            dtpReturnDate.Value = existingReturn.ReturnDate.Date;

            int conditionIndex = cboCondition.Items.IndexOf(existingReturn.Condition.ToUpperInvariant());
            cboCondition.SelectedIndex = conditionIndex >= 0 ? conditionIndex : 0;
        }

        private void ApplySelectedLoan()
        {
            if (cboMember.SelectedItem is not ReturnLoanOption selected)
                return;

            txtBookTitle.Text = selected.BookTitle;
            selectedDueDate = selected.DueDate;
            RecalculateFine();
        }

        private void RecalculateFine()
        {
            currentDaysOverdue = 0;

            if (selectedDueDate.HasValue)
                currentDaysOverdue = Math.Max(0, (dtpReturnDate.Value.Date - selectedDueDate.Value.Date).Days);

            string condition = cboCondition == null ? "GOOD" : cboCondition.Text.Trim().ToUpperInvariant();

            currentFineReason = "NONE";
            currentFineAmount = 0M;

            if (condition == "LOST")
            {
                currentFineReason = "LOST";
                currentFineAmount = GetFinePolicyAmount("LOST", 500M);
            }
            else if (condition == "DAMAGED")
            {
                currentFineReason = "DAMAGED";
                currentFineAmount = GetFinePolicyAmount("DAMAGED", 200M);
            }
            else if (currentDaysOverdue > 0)
            {
                currentFineReason = "LATE RETURN";
                decimal latePolicyAmount = GetFinePolicyAmount("LATE RETURN", 50M);
                currentFineAmount = latePolicyAmount * currentDaysOverdue;
            }

            if (lblDaysOverdue != null)
                lblDaysOverdue.Text = currentDaysOverdue == 1 ? "1 day" : $"{currentDaysOverdue} days";

            if (lblFineReason != null)
                lblFineReason.Text = currentFineReason == "NONE" ? "Reason: No fine" : $"Reason: {currentFineReason}";

            if (lblFineAmount != null)
                lblFineAmount.Text = currentFineAmount.ToString("₱#,##0.00");
        }

        private decimal GetFinePolicyAmount(string fineType, decimal fallbackAmount)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureFinePoliciesTable(conn);

                const string query = @"
SELECT TOP 1 DefaultAmount
FROM dbo.FinePolicies
WHERE IsArchived = 0
  AND UPPER(FineType) = @FineType
ORDER BY PolicyID DESC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FineType", fineType.ToUpperInvariant());

                object? result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    return Convert.ToDecimal(result);
            }
            catch
            {
                // keep fallback amount if policy table cannot be reached
            }

            return fallbackAmount;
        }

        private void EnsureFinePoliciesTable(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.FinePolicies', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.FinePolicies
    (
        PolicyID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        FineType NVARCHAR(50) NOT NULL,
        DefaultAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
        Description NVARCHAR(500) NULL,
        AddedBy NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.FinePolicies WHERE FineType = 'LATE RETURN' AND IsArchived = 0)
    INSERT INTO dbo.FinePolicies (FineType, DefaultAmount, Description, AddedBy)
    VALUES ('LATE RETURN', 50.00, 'Default per-day fine for late returns.', 'System');

IF NOT EXISTS (SELECT 1 FROM dbo.FinePolicies WHERE FineType = 'DAMAGED' AND IsArchived = 0)
    INSERT INTO dbo.FinePolicies (FineType, DefaultAmount, Description, AddedBy)
    VALUES ('DAMAGED', 200.00, 'Default fine for damaged books.', 'System');

IF NOT EXISTS (SELECT 1 FROM dbo.FinePolicies WHERE FineType = 'LOST' AND IsArchived = 0)
    INSERT INTO dbo.FinePolicies (FineType, DefaultAmount, Description, AddedBy)
    VALUES ('LOST', 500.00, 'Default fine for lost books.', 'System');";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void Complete_Click(object? sender, EventArgs e)
        {
            string memberName = cboMember.Text.Trim();
            string bookTitle = txtBookTitle.Text.Trim();

            if (string.IsNullOrWhiteSpace(memberName))
            {
                MessageBox.Show("Please enter or select a member.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMember.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(bookTitle))
            {
                MessageBox.Show("Please enter a book title.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBookTitle.Focus();
                return;
            }

            int daysOverdue = 0;
            if (selectedDueDate.HasValue)
                daysOverdue = Math.Max(0, (dtpReturnDate.Value.Date - selectedDueDate.Value.Date).Days);

            string condition = cboCondition.Text.Trim().ToUpperInvariant();
            RecalculateFine();
            decimal fine = currentFineAmount;

            ReturnLoanOption? selectedLoan = cboMember.SelectedItem as ReturnLoanOption;

            ReturnData = new ReturnDialogData
            {
                ReturnId = existingReturn?.ReturnId ?? 0,
                BorrowId = selectedLoan?.BorrowId ?? existingReturn?.BorrowId,
                MemberName = selectedLoan?.MemberName ?? memberName,
                BookTitle = bookTitle,
                ReturnDate = dtpReturnDate.Value.Date,
                Condition = condition,
                Status = daysOverdue > 0 ? "LATE" : "ON-TIME",
                DaysOverdue = daysOverdue,
                FineAmount = fine
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private TextBox CreateTextBox(string placeholder)
        {
            return new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F),
                PlaceholderText = placeholder,
                Multiline = false,
                Padding = new Padding(12)
            };
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = CreateLabel(text, 9.5F, FontStyle.Bold, SecondaryText);
            label.Location = new Point(x, y);
            Controls.Add(label);
        }

        private Label CreateLabel(string text, float size, FontStyle style, Color color)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control control) return;
            if (control.Width <= 1 || control.Height <= 1) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(control.BackColor);
            using System.Drawing.Drawing2D.GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, control.Width - 1, control.Height - 1), 14);
            e.Graphics.FillPath(brush, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius * 2;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
