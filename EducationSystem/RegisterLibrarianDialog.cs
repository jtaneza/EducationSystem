using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class RegisterLibrarianDialog : Form
    {
        private readonly Color FieldBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color FooterBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color Accent = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentSoft = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private TextBox txtFullName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPassword = null!;
        private ComboBox cmbRole = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public int EditingUserId { get; set; } = 0;

        public RegisterLibrarianDialog()
        {
            InitializeComponent();

            Text = "Register New Librarian";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(560, 650);
            BackColor = Color.White;

            BuildUI();
        }

        public void LoadForEdit(int userId, string fullName, string email, string role)
        {
            EditingUserId = userId;
            Text = "Edit Librarian";

            txtFullName.Text = fullName;
            txtEmail.Text = email;
            txtPassword.Text = "";

            if (cmbRole.Items.Contains(role))
                cmbRole.SelectedItem = role;

            btnSave.Text = "Save Changes";
        }

        private void BuildUI()
        {
            Panel iconBox = new Panel
            {
                Size = new Size(52, 52),
                Location = new Point(38, 34),
                BackColor = AccentSoft
            };
            iconBox.Paint += RoundedPanelPaint;

            Label icon = new Label
            {
                Text = "👥",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 20F),
                BackColor = Color.Transparent
            };
            iconBox.Controls.Add(icon);

            Label title = new Label
            {
                Text = "Register New Librarian",
                AutoSize = true,
                Location = new Point(38, 118),
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                ForeColor = PrimaryText
            };

            Label sub = new Label
            {
                Text = "Provide access credentials for the new library staff member.",
                AutoSize = true,
                Location = new Point(40, 158),
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = SecondaryText
            };

            txtFullName = CreateTextBox("e.g. Dr. Julian Thorne", 40, 225, 480);
            txtEmail = CreateTextBox("j.thorne@abcschool.edu", 40, 325, 480);
            txtPassword = CreateTextBox("", 40, 425, 230);
            txtPassword.UseSystemPasswordChar = true;

            cmbRole = new ComboBox
            {
                Location = new Point(305, 425),
                Size = new Size(220, 40),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F),
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                FlatStyle = FlatStyle.Flat
            };

            cmbRole.Items.AddRange(new object[]
            {
                "Head Librarian",
                "Librarian",
                "Assistant"
            });
            cmbRole.SelectedIndex = 1;

            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 44),
                Location = new Point(260, 540),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            btnSave = new Button
            {
                Text = "Register Librarian",
                Size = new Size(165, 44),
                Location = new Point(370, 540),
                FlatStyle = FlatStyle.Flat,
                BackColor = Accent,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            Panel footer = new Panel
            {
                BackColor = FooterBack,
                Location = new Point(0, ClientSize.Height - 56),
                Size = new Size(ClientSize.Width, 56)
            };

            Label footerText = new Label
            {
                Text = "The librarian can login using this email and password.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = SecondaryText
            };
            footer.Controls.Add(footerText);

            Controls.Add(iconBox);
            Controls.Add(title);
            Controls.Add(sub);

            Controls.Add(CreateLabel("FULL NAME", 40, 198));
            Controls.Add(txtFullName);

            Controls.Add(CreateLabel("EMAIL ADDRESS", 40, 298));
            Controls.Add(txtEmail);

            Controls.Add(CreateLabel("TEMPORARY PASSWORD", 40, 398));
            Controls.Add(txtPassword);

            Controls.Add(CreateLabel("ROLE SELECTION", 305, 398));
            Controls.Add(cmbRole);

            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(footer);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = SecondaryText
            };
        }

        private TextBox CreateTextBox(string placeholder, int x, int y, int width)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 40),
                Font = new Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                PlaceholderText = placeholder
            };
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = cmbRole.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please complete all required fields.", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (EditingUserId == 0 && string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a temporary password.", "Missing Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                string checkQuery = @"
SELECT COUNT(*)
FROM dbo.Users
WHERE Email = @Email
  AND IsArchived = 0
  AND UserID <> @UserID;";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    checkCmd.Parameters.AddWithValue("@UserID", EditingUserId);

                    int existing = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existing > 0)
                    {
                        MessageBox.Show("This email already exists.", "Duplicate Email",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (EditingUserId == 0)
                {
                    string insertQuery = @"
INSERT INTO dbo.Users
(ClientID, FullName, Email, PasswordText, Role, IsActive, IsArchived, ImagePath, CreatedAt)
VALUES
(@ClientID, @FullName, @Email, @PasswordText, @Role, 1, 0, @ImagePath, GETDATE());";

                    using SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@ClientID", 1);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordText", password);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@ImagePath", "Assets\\user.png");
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Librarian registered successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string updateQuery = @"
UPDATE dbo.Users
SET FullName = @FullName,
    Email = @Email,
    Role = @Role
WHERE UserID = @UserID;";

                    using SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@UserID", EditingUserId);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.ExecuteNonQuery();

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        string passQuery = @"
UPDATE dbo.Users
SET PasswordText = @PasswordText
WHERE UserID = @UserID;";

                        using SqlCommand passCmd = new SqlCommand(passQuery, conn);
                        passCmd.Parameters.AddWithValue("@UserID", EditingUserId);
                        passCmd.Parameters.AddWithValue("@PasswordText", password);
                        passCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Librarian updated successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to save librarian.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control c) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(c.BackColor);
            using System.Drawing.Drawing2D.GraphicsPath path =
                GetRoundedRectPath(new Rectangle(0, 0, c.Width - 1, c.Height - 1), 10);

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