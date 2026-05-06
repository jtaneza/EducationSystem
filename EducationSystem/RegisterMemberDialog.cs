using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class RegisterMemberDialog : Form
    {
        private readonly Color FieldBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color FooterBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color Accent = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentSoft = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private TextBox txtFullName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtContact = null!;
        private TextBox txtAddress = null!;
        private TextBox txtPassword = null!;
        private ComboBox cmbRole = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public int EditingUserId { get; set; } = 0;

        public RegisterMemberDialog()
        {
            InitializeComponent();

            Text = "Register New Member";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(600, 720);
            BackColor = Color.White;

            BuildUI();
        }

        private void BuildUI()
        {
            Panel header = new Panel
            {
                BackColor = FooterBack,
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width, 120)
            };

            Panel iconBox = new Panel
            {
                Size = new Size(52, 52),
                Location = new Point(36, 34),
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
                Text = "Register New Member",
                AutoSize = true,
                Location = new Point(104, 34),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = PrimaryText
            };

            Label sub = new Label
            {
                Text = "Add student or teacher details to the library registry.",
                AutoSize = true,
                Location = new Point(106, 70),
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = SecondaryText
            };

            header.Controls.Add(iconBox);
            header.Controls.Add(title);
            header.Controls.Add(sub);
            Controls.Add(header);

            txtFullName = CreateTextBox("e.g. Aria Montgomery", 40, 175, 520);
            txtEmail = CreateTextBox("aria.m@school.edu", 40, 275, 250);
            txtContact = CreateTextBox("+1 (555) 000-0000", 310, 275, 250);
            txtAddress = CreateTextBox("Street, City, State", 40, 375, 520);
            txtPassword = CreateTextBox("", 40, 475, 520);
            txtPassword.UseSystemPasswordChar = true;

            cmbRole = new ComboBox
            {
                Location = new Point(40, 575),
                Size = new Size(520, 40),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F),
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                FlatStyle = FlatStyle.Flat
            };
            cmbRole.Items.AddRange(new object[] { "Student", "Teacher" });
            cmbRole.SelectedIndex = 0;

            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(110, 44),
                Location = new Point(300, 650),
                FlatStyle = FlatStyle.Flat,
                BackColor = FooterBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            btnSave = new Button
            {
                Text = "Register Member",
                Size = new Size(160, 44),
                Location = new Point(420, 650),
                FlatStyle = FlatStyle.Flat,
                BackColor = Accent,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            Controls.Add(CreateLabel("FULL NAME", 40, 148));
            Controls.Add(txtFullName);

            Controls.Add(CreateLabel("EMAIL ADDRESS", 40, 248));
            Controls.Add(txtEmail);

            Controls.Add(CreateLabel("CONTACT NUMBER", 310, 248));
            Controls.Add(txtContact);

            Controls.Add(CreateLabel("PHYSICAL ADDRESS", 40, 348));
            Controls.Add(txtAddress);

            Controls.Add(CreateLabel("TEMPORARY PASSWORD", 40, 448));
            Controls.Add(txtPassword);

            Controls.Add(CreateLabel("ROLE SELECTION", 40, 548));
            Controls.Add(cmbRole);

            Controls.Add(btnCancel);
            Controls.Add(btnSave);
        }

        public void LoadForEdit(int userId, string fullName, string email, string contact, string address, string role)
        {
            EditingUserId = userId;
            Text = "Edit Member";
            btnSave.Text = "Save Changes";

            txtFullName.Text = fullName;
            txtEmail.Text = email;
            txtContact.Text = contact;
            txtAddress.Text = address;
            txtPassword.Text = "";

            if (cmbRole.Items.Contains(role))
                cmbRole.SelectedItem = role;
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
            string contact = txtContact.Text.Trim();
            string address = txtAddress.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = cmbRole.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please complete the required fields.", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                if (EditingUserId == 0)
                {
                    string insertQuery = @"
INSERT INTO dbo.Users
(ClientID, FullName, Email, PasswordText, Role, IsActive, IsArchived, ImagePath, CreatedAt, ContactNumber, Address)
VALUES
(@ClientID, @FullName, @Email, @PasswordText, @Role, 1, 0, @ImagePath, GETDATE(), @ContactNumber, @Address);";

                    using SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@ClientID", 1);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordText", string.IsNullOrWhiteSpace(password) ? "123456" : password);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@ImagePath", "Assets\\user.png");
                    cmd.Parameters.AddWithValue("@ContactNumber", contact);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string updateQuery = @"
UPDATE dbo.Users
SET FullName = @FullName,
    Email = @Email,
    Role = @Role,
    ContactNumber = @ContactNumber,
    Address = @Address
WHERE UserID = @UserID;";

                    using SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@UserID", EditingUserId);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@ContactNumber", contact);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.ExecuteNonQuery();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error:\n\n" + ex.Message);
            }
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control c) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(c.BackColor);
            using var path = GetRoundedRectPath(new Rectangle(0, 0, c.Width - 1, c.Height - 1), 10);
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