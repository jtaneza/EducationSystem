using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace EducationSystem
{
    public sealed class FinePolicyDialogData
    {
        public int PolicyId { get; set; }
        public string FineType { get; set; } = "";
        public decimal DefaultAmount { get; set; }
        public string Description { get; set; } = "";
        public string AddedBy { get; set; } = "";
    }

    public partial class FinePolicyDialog : Form
    {
        private readonly Color FieldBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color HeaderBack = ColorTranslator.FromHtml("#2B3234");
        private readonly Color FooterBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color MutedText = ColorTranslator.FromHtml("#6C7A74");

        private readonly FinePolicyDialogData? existingPolicy;
        private readonly string addedBy;

        private ComboBox cboFineType = null!;
        private TextBox txtAmount = null!;
        private TextBox txtDescription = null!;
        private TextBox txtAddedBy = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private Button btnClose = null!;

        public FinePolicyDialogData PolicyData { get; private set; } = new FinePolicyDialogData();

        public FinePolicyDialog(string addedBy, FinePolicyDialogData? existingPolicy = null)
        {
            this.addedBy = addedBy;
            this.existingPolicy = existingPolicy;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            ClientSize = new Size(620, 540);
            DoubleBuffered = true;
            ShowInTaskbar = false;

            BuildDialog();
            LoadValues();
        }

        private void BuildDialog()
        {
            Panel header = new Panel
            {
                BackColor = HeaderBack,
                Bounds = new Rectangle(0, 0, ClientSize.Width, 112)
            };
            Controls.Add(header);

            Label title = new Label
            {
                Text = existingPolicy == null ? "Create New Fine Policy" : "Edit Fine Policy",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 28),
                BackColor = Color.Transparent
            };
            header.Controls.Add(title);

            Label subtitle = new Label
            {
                Text = "LIBRAFLOW ACADEMIC SYSTEM",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = ColorTranslator.FromHtml("#BBCAC3"),
                Location = new Point(42, 62),
                BackColor = Color.Transparent
            };
            header.Controls.Add(subtitle);

            btnClose = new Button
            {
                Text = "X",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F),
                Bounds = new Rectangle(ClientSize.Width - 58, 30, 36, 36),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => CancelDialog();
            header.Controls.Add(btnClose);

            int left = 40;
            int right = 325;
            int top = 150;
            int fieldWidth = 245;
            int fieldHeight = 40;

            AddLabel("FINE TYPE", left, top);
            cboFineType = new ComboBox
            {
                Bounds = new Rectangle(left, top + 26, fieldWidth, fieldHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboFineType.Items.AddRange(new object[] { "LATE RETURN", "DAMAGED", "LOST" });
            cboFineType.SelectedIndexChanged += (s, e) => ApplyDefaultAmount();
            Controls.Add(cboFineType);

            AddLabel("DEFAULT FINE AMOUNT", right, top);
            txtAmount = CreateTextBox();
            txtAmount.PlaceholderText = "0.00";
            txtAmount.Bounds = new Rectangle(right, top + 26, fieldWidth, fieldHeight);
            Controls.Add(txtAmount);

            top += 112;

            AddLabel("DESCRIPTION", left, top);
            txtDescription = CreateTextBox();
            txtDescription.Multiline = true;
            txtDescription.PlaceholderText = "Enter detailed notes about this fine policy...";
            txtDescription.Bounds = new Rectangle(left, top + 26, ClientSize.Width - 80, 92);
            Controls.Add(txtDescription);

            top += 142;

            AddLabel("ADDED BY", left, top);
            txtAddedBy = CreateTextBox();
            txtAddedBy.ReadOnly = true;
            txtAddedBy.ForeColor = MutedText;
            txtAddedBy.Bounds = new Rectangle(left, top + 26, ClientSize.Width - 80, fieldHeight);
            Controls.Add(txtAddedBy);

            Panel footer = new Panel
            {
                BackColor = FooterBack,
                Bounds = new Rectangle(0, ClientSize.Height - 86, ClientSize.Width, 86)
            };
            Controls.Add(footer);

            btnCancel = new Button
            {
                Text = "Cancel",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Bounds = new Rectangle(ClientSize.Width - 270, 22, 95, 42),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => CancelDialog();
            footer.Controls.Add(btnCancel);

            btnSave = new Button
            {
                Text = "Save Fine Type",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Bounds = new Rectangle(ClientSize.Width - 170, 18, 130, 50),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SaveDialog;
            footer.Controls.Add(btnSave);
        }

        private void LoadValues()
        {
            txtAddedBy.Text = addedBy;

            if (existingPolicy == null)
            {
                cboFineType.SelectedItem = "LATE RETURN";
                txtDescription.Text = "";
                return;
            }

            int typeIndex = cboFineType.Items.IndexOf(existingPolicy.FineType);
            cboFineType.SelectedIndex = typeIndex >= 0 ? typeIndex : 0;
            txtAmount.Text = existingPolicy.DefaultAmount.ToString("0.00", CultureInfo.InvariantCulture);
            txtDescription.Text = existingPolicy.Description;
            txtAddedBy.Text = string.IsNullOrWhiteSpace(existingPolicy.AddedBy) ? addedBy : existingPolicy.AddedBy;
        }

        private void ApplyDefaultAmount()
        {
            if (existingPolicy != null)
                return;

            string type = Convert.ToString(cboFineType.SelectedItem) ?? "";

            if (type == "LOST")
                txtAmount.Text = "500.00";
            else if (type == "DAMAGED")
                txtAmount.Text = "200.00";
            else
                txtAmount.Text = "50.00";
        }

        private void SaveDialog(object? sender, EventArgs e)
        {
            string fineType = Convert.ToString(cboFineType.SelectedItem) ?? "";
            string amountText = txtAmount.Text.Trim().Replace("₱", "");

            if (string.IsNullOrWhiteSpace(fineType))
            {
                MessageBox.Show("Please select a fine type.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboFineType.Focus();
                return;
            }

            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) || amount < 0)
            {
                MessageBox.Show("Please enter a valid amount.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            PolicyData = new FinePolicyDialogData
            {
                PolicyId = existingPolicy?.PolicyId ?? 0,
                FineType = fineType,
                DefaultAmount = amount,
                Description = txtDescription.Text.Trim(),
                AddedBy = txtAddedBy.Text.Trim()
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelDialog()
        {
            DialogResult = DialogResult.Cancel;
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
                Padding = new Padding(8)
            };
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = SecondaryText,
                Location = new Point(x, y),
                BackColor = Color.Transparent
            };

            Controls.Add(label);
        }
    }
}
