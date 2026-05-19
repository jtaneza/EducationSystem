using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public sealed class ClientDialogForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = Color.White;
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color Border = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color TextDark = ColorTranslator.FromHtml("#161D1F");
        private readonly Color TextMuted = ColorTranslator.FromHtml("#64748B");
        private readonly Color Accent = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");

        private TextBox txtClientId = null!;
        private TextBox txtLibraryName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPassword = null!;
        private TextBox txtConfirmPassword = null!;
        private ComboBox cboStatus = null!;
        private ComboBox cboSubscriptionPlan = null!;
        private Button btnEyePassword = null!;
        private Button btnEyeConfirm = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        private bool showPassword = false;
        private bool showConfirmPassword = false;

        public string ClientIDValue => txtClientId.Text.Trim();
        public string LibraryNameValue => txtLibraryName.Text.Trim();
        public string EmailValue => txtEmail.Text.Trim();
        public string PasswordValue => txtPassword.Text.Trim();
        public string StatusValue => Convert.ToString(cboStatus.SelectedItem) ?? "Active";
        public string SubscriptionPlanValue => Convert.ToString(cboSubscriptionPlan.SelectedItem) ?? "One-Time Payment";

        public ClientDialogForm(string title, string clientId)
            : this(title, clientId, null, "One-Time Payment")
        {
        }

        public ClientDialogForm(string title, string clientId, ClientItem? existingClient)
            : this(title, clientId, existingClient, "One-Time Payment")
        {
        }

        public ClientDialogForm(string title, string clientId, ClientItem? existingClient, string subscriptionPlan)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Background;
            ClientSize = new Size(590, 540);
            AutoScaleMode = AutoScaleMode.None;

            BuildUi(title);
            LoadValues(clientId, existingClient, subscriptionPlan);
        }

        private void BuildUi(string title)
        {
            Panel card = new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(18, 18),
                Size = new Size(ClientSize.Width - 36, ClientSize.Height - 36),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(34, 26)
            };

            int labelX = 34;
            int inputX = 205;
            int top = 88;
            int rowGap = 50;
            int inputW = 320;

            txtClientId = CreateTextBox(true);
            AddRow(card, "Client ID", txtClientId, labelX, inputX, top, inputW);

            top += rowGap;
            txtLibraryName = CreateTextBox(false);
            AddRow(card, "Library Name", txtLibraryName, labelX, inputX, top, inputW);

            top += rowGap;
            txtEmail = CreateTextBox(false);
            AddRow(card, "Email", txtEmail, labelX, inputX, top, inputW);

            top += rowGap;
            txtPassword = CreateTextBox(false);
            txtPassword.UseSystemPasswordChar = true;
            AddRow(card, "Password", txtPassword, labelX, inputX, top, inputW - 42);

            btnEyePassword = CreateEyeButton(inputX + inputW - 36, top - 1);
            btnEyePassword.Click += (s, e) =>
            {
                showPassword = !showPassword;
                txtPassword.UseSystemPasswordChar = !showPassword;
            };
            card.Controls.Add(btnEyePassword);

            top += rowGap;
            txtConfirmPassword = CreateTextBox(false);
            txtConfirmPassword.UseSystemPasswordChar = true;
            AddRow(card, "Confirm Password", txtConfirmPassword, labelX, inputX, top, inputW - 42);

            btnEyeConfirm = CreateEyeButton(inputX + inputW - 36, top - 1);
            btnEyeConfirm.Click += (s, e) =>
            {
                showConfirmPassword = !showConfirmPassword;
                txtConfirmPassword.UseSystemPasswordChar = !showConfirmPassword;
            };
            card.Controls.Add(btnEyeConfirm);

            top += rowGap;
            cboStatus = CreateComboBox();
            cboStatus.Items.AddRange(new object[] { "Active", "Inactive" });
            cboStatus.SelectedIndex = 0;
            AddRow(card, "Status", cboStatus, labelX, inputX, top, inputW);

            top += rowGap;
            cboSubscriptionPlan = CreateComboBox();
            cboSubscriptionPlan.Items.AddRange(new object[] { "One-Time Payment", "Yearly Subscription" });
            cboSubscriptionPlan.SelectedIndex = 0;
            AddRow(card, "Subscription Plan", cboSubscriptionPlan, labelX, inputX, top, inputW);

            btnSave = new Button
            {
                Text = "Save",
                BackColor = Accent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(90, 36),
                Location = new Point(card.Width - 220, card.Height - 58),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SaveClicked;

            btnCancel = new Button
            {
                Text = "Cancel",
                BackColor = ColorTranslator.FromHtml("#7B8794"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(90, 36),
                Location = new Point(card.Width - 120, card.Height - 58),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(btnSave);
            card.Controls.Add(btnCancel);

            Controls.Add(card);
        }

        private void LoadValues(string clientId, ClientItem? existingClient, string subscriptionPlan)
        {
            txtClientId.Text = clientId;

            if (existingClient != null)
            {
                txtLibraryName.Text = existingClient.LibraryName;
                txtEmail.Text = existingClient.Email;
                txtPassword.Text = existingClient.Password;
                txtConfirmPassword.Text = existingClient.Password;

                SelectComboValue(cboStatus, existingClient.Status);
            }

            SelectComboValue(cboSubscriptionPlan, subscriptionPlan);
        }

        private TextBox CreateTextBox(bool readOnly)
        {
            return new TextBox
            {
                ReadOnly = readOnly,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = readOnly ? SurfaceLow : Color.White,
                ForeColor = readOnly ? AccentDeep : TextDark,
                Font = new Font("Segoe UI", 10F),
                Height = 28
            };
        }

        private ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = TextDark,
                Font = new Font("Segoe UI", 10F),
                Height = 30
            };
        }

        private Button CreateEyeButton(int x, int y)
        {
            Button button = new Button
            {
                Text = "👁",
                Size = new Size(34, 30),
                Location = new Point(x, y),
                BackColor = SurfaceLow,
                ForeColor = Color.FromArgb(83, 111, 142),
                Font = new Font("Segoe UI Symbol", 9F),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.BorderSize = 1;

            return button;
        }

        private void AddRow(Control parent, string labelText, Control input, int labelX, int inputX, int y, int inputW)
        {
            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 10F),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(labelX, y + 4)
            };

            input.Location = new Point(inputX, y);
            input.Size = new Size(inputW, 28);

            parent.Controls.Add(label);
            parent.Controls.Add(input);
        }

        private void SelectComboValue(ComboBox combo, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            for (int i = 0; i < combo.Items.Count; i++)
            {
                string itemText = Convert.ToString(combo.Items[i]) ?? "";

                if (string.Equals(itemText, value, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        private void SaveClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LibraryNameValue))
            {
                MessageBox.Show("Please enter the library name.", "Missing Library Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLibraryName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailValue))
            {
                MessageBox.Show("Please enter the email address.", "Missing Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordValue))
            {
                MessageBox.Show("Please enter the password.", "Missing Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (!string.Equals(txtPassword.Text, txtConfirmPassword.Text, StringComparison.Ordinal))
            {
                MessageBox.Show("Password and confirm password do not match.", "Password Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
