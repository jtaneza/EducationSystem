using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public sealed class FineMemberOption
    {
        public int UserId { get; }
        public string FullName { get; }
        public string Role { get; }
        public int? BorrowId { get; }
        public string BookId { get; }
        public string BookTitle { get; }
        public string FineReason { get; }
        public decimal SuggestedAmount { get; }
        public bool HasReturnFineSource { get; }

        public FineMemberOption(int userId, string fullName, string role)
            : this(userId, fullName, role, null, "", "", "", 0M, false)
        {
        }

        public FineMemberOption(int userId, string fullName, string role, int? borrowId, string bookId, string bookTitle, string fineReason, decimal suggestedAmount, bool hasReturnFineSource)
        {
            UserId = userId;
            FullName = fullName;
            Role = role;
            BorrowId = borrowId;
            BookId = bookId;
            BookTitle = bookTitle;
            FineReason = fineReason;
            SuggestedAmount = suggestedAmount;
            HasReturnFineSource = hasReturnFineSource;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Role) ? FullName : $"{FullName} ({Role})";
        }
    }

    public sealed class FineBookOption
    {
        public string BookId { get; }
        public string BookTitle { get; }

        public FineBookOption(string bookId, string bookTitle)
        {
            BookId = bookId;
            BookTitle = bookTitle;
        }

        public override string ToString()
        {
            return BookId;
        }
    }

    public sealed class FineDialogData
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
    }

    public sealed class ApplyFineDialog : Form
    {
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#2B3234");
        private readonly Color FooterBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color FieldBack = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color CardBack = Color.White;
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color MutedText = ColorTranslator.FromHtml("#6C7A74");

        private readonly List<FineMemberOption> memberOptions;
        private readonly List<FineBookOption> bookOptions;
        private readonly FineDialogData? existingFine;
        private readonly string recordedBy;

        private TextBox txtFineCode = null!;
        private TextBox txtRecordedBy = null!;
        private ComboBox cboMember = null!;
        private TextBox txtBookId = null!;
        private TextBox txtBookTitle = null!;
        private ComboBox cboReason = null!;
        private TextBox txtAmount = null!;
        private ComboBox cboStatus = null!;
        private TextBox txtRemarks = null!;
        private Button btnApply = null!;
        private bool suppressReasonAutoAmount;

        public FineDialogData FineData { get; private set; } = new FineDialogData();

        public ApplyFineDialog(
            string fineCode,
            string recordedBy,
            IEnumerable<FineMemberOption> memberOptions,
            IEnumerable<FineBookOption> bookOptions,
            FineDialogData? existingFine = null)
        {
            this.recordedBy = recordedBy;
            this.memberOptions = memberOptions.ToList();
            this.bookOptions = bookOptions.ToList();
            this.existingFine = existingFine;

            Text = existingFine == null ? "Apply New Fine" : "Edit Fine";
            ClientSize = new Size(680, 650);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = CardBack;
            ShowInTaskbar = false;
            DoubleBuffered = true;

            BuildDialog(fineCode);
            LoadValues(fineCode);
        }

        private IEnumerable<FineMemberOption> GetMemberOnlyOptions()
        {
            return memberOptions
                .Where(member => member.HasReturnFineSource)
                .OrderBy(member => member.FullName);
        }

        private void LoadMemberChoices()
        {
            if (cboMember == null)
                return;

            cboMember.BeginUpdate();
            cboMember.Items.Clear();
            cboMember.Items.AddRange(GetMemberOnlyOptions().Cast<object>().ToArray());
            cboMember.EndUpdate();

            cboMember.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboMember.AutoCompleteSource = AutoCompleteSource.ListItems;

            if (cboMember.Items.Count > 0)
                cboMember.SelectedIndex = 0;
            else
                cboMember.Text = "";
        }
        private void ApplySelectedMemberFineSource()
        {
            if (cboMember == null || txtBookId == null || txtBookTitle == null || cboReason == null || txtAmount == null || txtRemarks == null)
                return;

            if (cboMember.SelectedItem is not FineMemberOption selected)
                return;

            // Only apply if coming from return (damage/lost/late)
            if (!selected.HasReturnFineSource)
                return;

            // Auto-fill fields from the selected return record
            string autoBookId = selected.BookId;

            if (string.IsNullOrWhiteSpace(autoBookId))
            {
                FineBookOption? matchedBook = bookOptions.FirstOrDefault(book =>
                    book.BookTitle.Equals(selected.BookTitle, StringComparison.OrdinalIgnoreCase));

                if (matchedBook != null)
                    autoBookId = matchedBook.BookId;
            }

            txtBookId.Text = autoBookId;
            txtBookTitle.Text = selected.BookTitle;
            // Normalize reason
            string reason = NormalizeReason(selected.FineReason);
            if (!string.IsNullOrWhiteSpace(reason))
            {
                suppressReasonAutoAmount = true;
                cboReason.SelectedItem = reason;
                suppressReasonAutoAmount = false;
            }

            // Auto amount (peso) from return record
            if (selected.SuggestedAmount > 0)
                txtAmount.Text = selected.SuggestedAmount.ToString("0.00", CultureInfo.InvariantCulture);
            else
                txtAmount.Text = GetDefaultAmount(reason).ToString("0.00", CultureInfo.InvariantCulture);

            // Optional remarks
            if (string.IsNullOrWhiteSpace(txtRemarks.Text))
                txtRemarks.Text = $" ";
        }

        private string NormalizeReason(string reason)
        {
            string value = (reason ?? "").ToUpper();

            if (value.Contains("LOST"))
                return "LOST";

            if (value.Contains("DAMAGE"))
                return "DAMAGED";

            if (value.Contains("LATE") || value.Contains("OVERDUE"))
                return "LATE RETURN";

            return "";
        }
        private void BuildDialog(string fineCode)
        {
            Panel header = new Panel
            {
                BackColor = HeaderBack,
                Bounds = new Rectangle(0, 0, ClientSize.Width, 88),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            Label title = CreateLabel(existingFine == null ? "Apply New Fine" : "Edit Fine", 18F, FontStyle.Bold, Color.White);
            title.Location = new Point(40, 18);

            Label subtitle = CreateLabel("Record a penalty for a library member.", 10.5F, FontStyle.Regular, ColorTranslator.FromHtml("#BBCAC3"));
            subtitle.Location = new Point(40, 46);

            Label close = new Label
            {
                Text = "x",
                AutoSize = false,
                Size = new Size(34, 34),
                Location = new Point(ClientSize.Width - 58, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18F, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#BBCAC3"),
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

            int left = 40;
            int top = 110;
            int gap = 30;
            int fieldWidth = 280;
            int fieldHeight = 38;
            int right = left + fieldWidth + gap;

            AddLabel("FINE ID", left, top);
            txtFineCode = CreateTextBox();
            txtFineCode.Text = fineCode;
            txtFineCode.ReadOnly = true;
            txtFineCode.ForeColor = MutedText;
            txtFineCode.Bounds = new Rectangle(left, top + 26, fieldWidth, fieldHeight);
            Controls.Add(txtFineCode);

            AddLabel("RECORDED BY", right, top);
            txtRecordedBy = CreateTextBox();
            txtRecordedBy.Text = recordedBy;
            txtRecordedBy.ReadOnly = true;
            txtRecordedBy.ForeColor = MutedText;
            txtRecordedBy.Bounds = new Rectangle(right, top + 26, fieldWidth, fieldHeight);
            Controls.Add(txtRecordedBy);

            top += 80;

            AddLabel("MEMBER NAME / ID", left, top);
            cboMember = new ComboBox
            {
                Bounds = new Rectangle(left, top + 26, ClientSize.Width - 80, fieldHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            cboMember.SelectedIndexChanged += (s, e) => ApplySelectedMemberFineSource();
            Controls.Add(cboMember);
            LoadMemberChoices();

            top += 80;

            AddLabel("BOOK ID", left, top);
            txtBookId = CreateTextBox();
            txtBookId.PlaceholderText = "Automatic book ID...";
            txtBookId.ReadOnly = true;
            txtBookId.ForeColor = MutedText;
            txtBookId.Bounds = new Rectangle(left, top + 26, fieldWidth, fieldHeight);
            Controls.Add(txtBookId);

            AddLabel("BOOK TITLE", right, top);
            txtBookTitle = CreateTextBox();
            txtBookTitle.PlaceholderText = "Automatic title retrieval...";
            txtBookTitle.ReadOnly = true;
            txtBookTitle.ForeColor = MutedText;
            txtBookTitle.Bounds = new Rectangle(right, top + 26, fieldWidth, fieldHeight);
            Controls.Add(txtBookTitle);

            top += 80;

            AddLabel("FINE REASON", left, top);
            cboReason = new ComboBox
            {
                Bounds = new Rectangle(left, top + 26, fieldWidth, fieldHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboReason.Items.AddRange(new object[]
            {
                "DAMAGED",
                "LOST",
                "LATE RETURN"
            });
            cboReason.SelectedIndexChanged += (s, e) => ApplyReasonDefaultAmount();
            Controls.Add(cboReason);

            AddLabel("FINE AMOUNT", right, top);
            txtAmount = CreateTextBox();
            txtAmount.PlaceholderText = "0.00";
            txtAmount.Bounds = new Rectangle(right, top + 26, fieldWidth, fieldHeight);
            Controls.Add(txtAmount);

            top += 80;

            AddLabel("STATUS", left, top);
            cboStatus = new ComboBox
            {
                Bounds = new Rectangle(left, top + 26, fieldWidth, fieldHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new object[] { "Unpaid", "Paid" });
            cboStatus.SelectedIndex = 0;
            Controls.Add(cboStatus);

            AddLabel("REMARKS", right, top);
            txtRemarks = CreateTextBox();
            txtRemarks.Multiline = true;
            txtRemarks.PlaceholderText = "Add details...";
            txtRemarks.Bounds = new Rectangle(right, top + 26, fieldWidth, 48);
            Controls.Add(txtRemarks);

            Panel footer = new Panel
            {
                BackColor = FooterBack,
                Bounds = new Rectangle(0, ClientSize.Height - 70, ClientSize.Width, 70),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(120, 42),
                Location = new Point(ClientSize.Width - 40 - 190 - 20 - 120, 14),
                FlatStyle = FlatStyle.Flat,
                BackColor = FooterBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            btnApply = new Button
            {
                Text = existingFine == null ? "Apply Fine" : "Save Changes",
                Size = new Size(190, 44),
                Location = new Point(ClientSize.Width - 40 - 190, 13),
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += Apply_Click;

            footer.Controls.Add(btnCancel);
            footer.Controls.Add(btnApply);
            Controls.Add(footer);
            footer.BringToFront();

            AcceptButton = btnApply;
            CancelButton = btnCancel;
        }

        private void LoadValues(string fineCode)
        {
            txtFineCode.Text = fineCode;
            txtRecordedBy.Text = recordedBy;
            LoadMemberChoices();

            if (existingFine == null)
            {
                cboStatus.SelectedItem = "Unpaid";

                if (cboMember.SelectedItem is FineMemberOption selected && selected.HasReturnFineSource)
                {
                    ApplySelectedMemberFineSource();
                }
                else
                {
                    cboReason.SelectedItem = "LATE RETURN";
                    txtAmount.Text = "50.00";
                }

                return;
            }

            FineMemberOption? selectedMember = existingFine.MemberId.HasValue
                ? memberOptions.FirstOrDefault(member => member.UserId == existingFine.MemberId.Value)
                : null;

            if (selectedMember != null &&
                (selectedMember.Role.Equals("Member", StringComparison.OrdinalIgnoreCase) ||
                 selectedMember.Role.Equals("Student", StringComparison.OrdinalIgnoreCase)))
            {
                cboMember.SelectedItem = cboMember.Items.Cast<object>()
                    .OfType<FineMemberOption>()
                    .FirstOrDefault(member => member.UserId == selectedMember.UserId);
            }
            else
            {
                cboMember.Text = existingFine.MemberName;
            }

            txtBookId.Text = existingFine.BookId;
            txtBookTitle.Text = existingFine.BookTitle;

            string reason = existingFine.Reason.ToUpperInvariant();
            if (reason.Contains("LOST"))
                cboReason.SelectedItem = "LOST";
            else if (reason.Contains("LATE"))
                cboReason.SelectedItem = "LATE RETURN";
            else
                cboReason.SelectedItem = "DAMAGED";

            txtAmount.Text = existingFine.Amount.ToString("0.00", CultureInfo.InvariantCulture);

            int statusIndex = cboStatus.Items.IndexOf(existingFine.Status);
            cboStatus.SelectedIndex = statusIndex >= 0 ? statusIndex : 0;

            txtRemarks.Text = existingFine.Remarks;
        }

        private void ApplyReasonDefaultAmount()
        {
            if (existingFine != null || suppressReasonAutoAmount)
                return;

            string reason = Convert.ToString(cboReason.SelectedItem) ?? "";
            txtAmount.Text = GetDefaultAmount(reason).ToString("0.00", CultureInfo.InvariantCulture);
        }

        private decimal GetDefaultAmount(string reason)
        {
            if (reason.Equals("LOST", StringComparison.OrdinalIgnoreCase))
                return 500M;

            if (reason.Equals("DAMAGED", StringComparison.OrdinalIgnoreCase))
                return 200M;

            if (reason.Equals("LATE RETURN", StringComparison.OrdinalIgnoreCase))
                return 50M;

            return 0M;
        }

        private void Apply_Click(object? sender, EventArgs e)
        {
            string memberName = cboMember.Text.Trim();
            string bookId = txtBookId.Text.Trim();
            string bookTitle = txtBookTitle.Text.Trim();
            string reason = (Convert.ToString(cboReason.SelectedItem) ?? "").Trim();
            string amountText = txtAmount.Text.Trim().Replace("$", "").Replace("₱", "");

            if (string.IsNullOrWhiteSpace(memberName))
            {
                MessageBox.Show("Please enter or select a member.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMember.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(bookId))
            {
                MessageBox.Show("This return record has no Book ID. Please check BorrowingRecords.BookID or make sure the book title exists in Books.", "Missing Book ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBookId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(bookTitle))
            {
                MessageBox.Show("Please enter the book title.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBookTitle.Focus();
                return;
            }

            if (reason != "DAMAGED" && reason != "LOST" && reason != "LATE RETURN")
            {
                MessageBox.Show("Fine reason must be DAMAGED, LOST, or LATE RETURN.", "Invalid Reason", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboReason.Focus();
                return;
            }

            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) || amount < 0)
            {
                MessageBox.Show("Please enter a valid fine amount.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            FineMemberOption? selectedMember = cboMember.SelectedItem as FineMemberOption;
            FineData = new FineDialogData
            {
                FineId = existingFine?.FineId ?? 0,
                BorrowId = selectedMember?.BorrowId ?? existingFine?.BorrowId,
                MemberId = selectedMember?.UserId ?? existingFine?.MemberId,
                MemberName = selectedMember?.FullName ?? memberName,
                BookId = bookId,
                BookTitle = bookTitle,
                Reason = reason,
                Amount = amount,
                Status = Convert.ToString(cboStatus.SelectedItem) ?? "Unpaid",
                Remarks = txtRemarks.Text.Trim(),
                RecordedBy = recordedBy
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F),
                Multiline = false
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
    }
}
