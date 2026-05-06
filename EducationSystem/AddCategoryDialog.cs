using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public sealed class AddCategoryDialog : Form
    {
        private readonly Color cardBack;
        private readonly Color fieldBack;
        private readonly Color accent;
        private readonly Color accentDeep;
        private readonly Color primaryText;
        private readonly Color secondaryText;
        private readonly Color outline = ColorTranslator.FromHtml("#DDE4E6");

        private readonly TextBox txtCategoryId = new TextBox();
        private readonly TextBox txtCategoryName = new TextBox();
        private readonly TextBox txtGenre = new TextBox();
        private readonly TextBox txtGroup = new TextBox();
        private readonly TextBox txtDescription = new TextBox();

        public string CategoryId => txtCategoryId.Text.Trim();
        public string CategoryName => txtCategoryName.Text.Trim();
        public string Genre => txtGenre.Text.Trim();
        public string GroupName => txtGroup.Text.Trim();
        public string Description => txtDescription.Text.Trim();

        public AddCategoryDialog(
            string categoryId,
            string addedBy,
            Color formBack,
            Color cardBack,
            Color fieldBack,
            Color accent,
            Color accentDeep,
            Color primaryText,
            Color secondaryText)
        {
            this.cardBack = cardBack;
            this.fieldBack = fieldBack;
            this.accent = accent;
            this.accentDeep = accentDeep;
            this.primaryText = primaryText;
            this.secondaryText = secondaryText;

            Text = "Add New Category";
            ClientSize = new Size(620, 700);
            BackColor = cardBack;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            DoubleBuffered = true;
            Padding = new Padding(44, 34, 44, 28);

            BuildDialog(categoryId, addedBy);

            Shown += (s, e) =>
            {
                txtCategoryId.DeselectAll();
                txtCategoryName.Focus();
            };
        }

        private void BuildDialog(string categoryId, string addedBy)
        {
            Label title = CreateLabel("Add New Category", 22F, FontStyle.Bold, primaryText);
            Label subtitle = CreateLabel("Define metadata for library classification.", 10.5F, FontStyle.Regular, secondaryText);

            Controls.Add(title);
            Controls.Add(subtitle);

            int left = Padding.Left;
            int width = ClientSize.Width - Padding.Left - Padding.Right;
            int y = Padding.Top;

            title.Location = new Point(left, y);
            y = title.Bottom + 6;

            subtitle.Location = new Point(left, y);
            y = subtitle.Bottom + 28;

            txtCategoryId.Text = categoryId;
            txtCategoryId.ReadOnly = true;
            txtCategoryId.TabStop = false;

            AddField("CATEGORY ID", txtCategoryId, "Auto-Generated", left, ref y, width, 42, false);
            AddField("CATEGORY NAME", txtCategoryName, "e.g., Science Fiction", left, ref y, width, 42, false);
            AddField("GENRE", txtGenre, "e.g., Academic, Fiction, Research", left, ref y, width, 42, false);
            AddField("GROUP", txtGroup, "e.g., Humanities, Sciences, Arts", left, ref y, width, 42, false);
            AddField("DESCRIPTION", txtDescription, "Briefly describe the contents and scope of this category...", left, ref y, width, 86, true);

            Label addedByText = CreateLabel("✓  Added By: " + addedBy + " (Current Session)", 9.2F, FontStyle.Bold, accentDeep);
            addedByText.Location = new Point(left, y + 13);
            addedByText.BackColor = cardBack;
            Controls.Add(addedByText);

            Button btnCancel = CreateTextButton("Cancel");
            Button btnSave = CreatePrimaryButton("Save Category");

            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            btnSave.Click += Save_Click;

            btnSave.Location = new Point(ClientSize.Width - Padding.Right - btnSave.Width, ClientSize.Height - 74);
            btnCancel.Location = new Point(btnSave.Left - btnCancel.Width - 18, btnSave.Top + 2);
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Controls.Add(btnCancel);
            Controls.Add(btnSave);

            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        private void AddField(string labelText, TextBox textBox, string placeholder, int left, ref int y, int width, int height, bool multiline)
        {
            Label label = CreateLabel(labelText, 9F, FontStyle.Bold, secondaryText);
            label.Location = new Point(left, y);
            Controls.Add(label);

            y = label.Bottom + 6;

            Panel host = new Panel
            {
                BackColor = fieldBack,
                Bounds = new Rectangle(left, y, width, height)
            };
            host.Paint += RoundedPanelPaint;

            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = fieldBack;
            textBox.ForeColor = textBox.ReadOnly ? secondaryText : primaryText;
            textBox.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            textBox.PlaceholderText = placeholder;
            textBox.Multiline = multiline;
            textBox.ScrollBars = ScrollBars.None;
            textBox.Location = new Point(18, multiline ? 12 : 11);
            textBox.Size = new Size(width - 36, multiline ? height - 22 : 24);

            host.Controls.Add(textBox);
            Controls.Add(host);

            y = host.Bottom + 16;
        }

        private Button CreateTextButton(string text)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(112, 44),
                FlatStyle = FlatStyle.Flat,
                BackColor = cardBack,
                ForeColor = primaryText,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private Button CreatePrimaryButton(string text)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(178, 46),
                FlatStyle = FlatStyle.Flat,
                BackColor = accent,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void Save_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                ShowValidation("Please enter a category name.", txtCategoryName);
                return;
            }

            if (string.IsNullOrWhiteSpace(Genre))
            {
                ShowValidation("Please enter a genre.", txtGenre);
                return;
            }

            if (string.IsNullOrWhiteSpace(GroupName))
            {
                ShowValidation("Please enter a group.", txtGroup);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowValidation(string message, Control focusTarget)
        {
            MessageBox.Show(message, "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            focusTarget.Focus();
        }

        private Label CreateLabel(string text, float size, FontStyle style, Color color)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                BackColor = cardBack
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using Pen border = new Pen(outline, 1);
            e.Graphics.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel) return;
            if (panel.Width <= 1 || panel.Height <= 1) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(panel.BackColor);
            using System.Drawing.Drawing2D.GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 10);
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
