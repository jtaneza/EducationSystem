using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public sealed class BookCategoryOption
    {
        public string CategoryName { get; }
        public string Genre { get; }
        public string GroupName { get; }

        public BookCategoryOption(string categoryName, string genre, string groupName)
        {
            CategoryName = categoryName;
            Genre = genre;
            GroupName = groupName;
        }

        public override string ToString()
        {
            return CategoryName;
        }
    }

    public sealed class BookDialogData
    {
        public string BookId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Category { get; set; } = "";
        public string Genre { get; set; } = "";
        public string GroupName { get; set; } = "";
        public int? PublishYear { get; set; }
        public int Quantity { get; set; } = 1;
        public string Status { get; set; } = "In Stock";
    }

    public sealed class AddBookDialog : Form
    {
        private readonly Color formBack = Color.White;
        private readonly Color frameBack = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color headerBack = ColorTranslator.FromHtml("#2B3234");
        private readonly Color footerBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color fieldBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color primary = ColorTranslator.FromHtml("#00B894");
        private readonly Color primaryDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color primaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color secondaryText = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color mutedText = ColorTranslator.FromHtml("#6C7A74");

        private readonly List<BookCategoryOption> categoryOptions;
        private readonly bool editMode;

        private Panel headerPanel = null!;
        private Panel bodyPanel = null!;
        private Panel footerPanel = null!;

        private TextBox txtBookId = null!;
        private TextBox txtRecordedBy = null!;
        private TextBox txtTitle = null!;
        private TextBox txtAuthor = null!;
        private ComboBox cmbCategory = null!;
        private ComboBox cmbGenre = null!;
        private ComboBox cmbGroup = null!;
        private NumericUpDown numYear = null!;
        private NumericUpDown numQuantity = null!;
        private ComboBox cmbStatus = null!;
        private Button btnCancel = null!;
        private Button btnSave = null!;

        public BookDialogData BookData => new BookDialogData
        {
            BookId = txtBookId.Text.Trim(),
            Title = txtTitle.Text.Trim(),
            Author = txtAuthor.Text.Trim(),
            Category = cmbCategory.SelectedItem is BookCategoryOption selectedCategory
                ? selectedCategory.CategoryName
                : Convert.ToString(cmbCategory.SelectedItem) ?? "",
            Genre = Convert.ToString(cmbGenre.SelectedItem) ?? "",
            GroupName = Convert.ToString(cmbGroup.SelectedItem) ?? "",
            PublishYear = numYear.Value <= 0 ? null : (int)numYear.Value,
            Quantity = (int)numQuantity.Value,
            Status = Convert.ToString(cmbStatus.SelectedItem) ?? "In Stock"
        };

        public AddBookDialog(
            string bookId,
            string recordedBy,
            IEnumerable<BookCategoryOption> categories,
            BookDialogData? existingBook = null)
        {
            categoryOptions = categories
                .Where(item => !string.IsNullOrWhiteSpace(item.CategoryName))
                .GroupBy(item => item.CategoryName, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(item => item.CategoryName)
                .ToList();

            editMode = existingBook != null;

            Text = editMode ? "Edit Book" : "Add New Book";
            ClientSize = new Size(660, 560);
            MinimumSize = new Size(620, 520);
            MaximumSize = new Size(700, 600);
            AutoScaleMode = AutoScaleMode.None;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = frameBack;
            Padding = new Padding(1);
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            DoubleBuffered = true;

            BuildDialog(bookId, recordedBy);
            LoadValues(existingBook);
        }

        private void BuildDialog(string bookId, string recordedBy)
        {
            headerPanel = new Panel
            {
                BackColor = headerBack,
                Bounds = new Rectangle(1, 1, ClientSize.Width - 2, 74),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            Label title = new Label
            {
                Text = editMode ? "Edit Book Details" : "Add New Book to Catalog",
                AutoSize = true,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(32, 22)
            };

            Button close = new Button
            {
                Text = "x",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(34, 34),
                Location = new Point(headerPanel.Width - 48, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = headerBack,
                ForeColor = ColorTranslator.FromHtml("#BBCAC3"),
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            close.FlatAppearance.BorderSize = 0;
            close.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            headerPanel.Controls.Add(title);
            headerPanel.Controls.Add(close);

            footerPanel = new Panel
            {
                BackColor = footerBack,
                Bounds = new Rectangle(1, ClientSize.Height - 70, ClientSize.Width - 2, 69),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            bodyPanel = new Panel
            {
                BackColor = formBack,
                Bounds = new Rectangle(1, headerPanel.Bottom, ClientSize.Width - 2, footerPanel.Top - headerPanel.Bottom),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                Padding = new Padding(32, 22, 32, 22),
                AutoScroll = false
            };

            Controls.Add(headerPanel);
            Controls.Add(bodyPanel);
            Controls.Add(footerPanel);

            BuildBody(bookId, recordedBy);
            BuildFooter();
        }

        private void BuildBody(string bookId, string recordedBy)
        {
            int left = 32;
            int top = 20;
            int gap = 26;
            int fullWidth = bodyPanel.Width - 64;
            int colWidth = (fullWidth - gap) / 2;

            txtBookId = CreateTextBox(bookId, true);
            txtRecordedBy = CreateTextBox(recordedBy, true);
            AddLabeledControl(bodyPanel, "BOOK ID (SYSTEM GENERATED)", txtBookId, left, top, colWidth, 42);
            AddLabeledControl(bodyPanel, "RECORDED BY", txtRecordedBy, left + colWidth + gap, top, colWidth, 42);

            top += 76;
            txtTitle = CreateTextBox("", false);
            txtTitle.PlaceholderText = "Enter full publication title...";
            txtTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            AddLabeledControl(bodyPanel, "BOOK TITLE", txtTitle, left, top, fullWidth, 48);

            top += 78;
            txtAuthor = CreateTextBox("", false);
            txtAuthor.PlaceholderText = "Full name of primary author...";
            AddLabeledControl(bodyPanel, "AUTHOR NAME", txtAuthor, left, top, fullWidth, 46);

            top += 76;
            int selectWidth = (fullWidth - 32) / 3;

            cmbCategory = CreateComboBox();
            cmbGenre = CreateComboBox();
            cmbGroup = CreateComboBox();

            AddLabeledControl(bodyPanel, "CATEGORY", cmbCategory, left, top, selectWidth, 42);
            AddLabeledControl(bodyPanel, "GENRE", cmbGenre, left + selectWidth + 16, top, selectWidth, 42);
            AddLabeledControl(bodyPanel, "GROUP", cmbGroup, left + (selectWidth + 16) * 2, top, selectWidth, 42);

            top += 72;

            Panel inventoryPanel = new Panel
            {
                BackColor = footerBack,
                Bounds = new Rectangle(left, top, fullWidth, 74)
            };
            inventoryPanel.Paint += RoundedPanelPaint;
            bodyPanel.Controls.Add(inventoryPanel);

            int inventoryLeft = 18;
            int inventoryGap = 24;
            int inventoryWidth = (inventoryPanel.Width - 36 - inventoryGap * 2) / 3;

            Label lblYear = CreateFieldLabel("YEAR");
            lblYear.Location = new Point(inventoryLeft, 14);

            numYear = new NumericUpDown
            {
                Minimum = 0,
                Maximum = DateTime.Now.Year + 1,
                Value = 0,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = formBack,
                ForeColor = primaryText,
                BorderStyle = BorderStyle.None,
                Location = new Point(inventoryLeft, 38),
                Size = new Size(inventoryWidth, 24),
                TextAlign = HorizontalAlignment.Center
            };

            int quantityX = inventoryLeft + inventoryWidth + inventoryGap;
            Label lblQuantity = CreateFieldLabel("QUANTITY (QTY)");
            lblQuantity.Location = new Point(quantityX, 14);

            numQuantity = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100000,
                Value = 1,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = formBack,
                ForeColor = primaryText,
                BorderStyle = BorderStyle.None,
                Location = new Point(quantityX, 38),
                Size = new Size(inventoryWidth, 24),
                TextAlign = HorizontalAlignment.Center
            };

            int statusX = inventoryLeft + (inventoryWidth + inventoryGap) * 2;
            Label lblStatus = CreateFieldLabel("AVAILABILITY STATUS");
            lblStatus.Location = new Point(statusX, 14);

            cmbStatus = CreateComboBox();
            cmbStatus.Location = new Point(statusX, 36);
            cmbStatus.Size = new Size(inventoryWidth, 28);
            cmbStatus.Items.AddRange(new object[] { "In Stock", "Low Stock", "Out of Stock" });
            cmbStatus.SelectedIndex = 0;

            inventoryPanel.Controls.Add(lblYear);
            inventoryPanel.Controls.Add(numYear);
            inventoryPanel.Controls.Add(lblQuantity);
            inventoryPanel.Controls.Add(numQuantity);
            inventoryPanel.Controls.Add(lblStatus);
            inventoryPanel.Controls.Add(cmbStatus);

            LoadCategoryDropdowns();

            cmbCategory.SelectedIndexChanged += (s, e) => ApplyCategorySelection();
        }

        private void BuildFooter()
        {
            btnCancel = new Button
            {
                Text = "Cancel",
                FlatStyle = FlatStyle.Flat,
                BackColor = footerBack,
                ForeColor = primaryText,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Size = new Size(112, 40),
                Location = new Point(footerPanel.Width - 292, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            btnSave = new Button
            {
                Text = editMode ? "Save Changes" : "Save Book",
                FlatStyle = FlatStyle.Flat,
                BackColor = primary,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Size = new Size(150, 40),
                Location = new Point(footerPanel.Width - 168, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SaveClicked;

            footerPanel.Controls.Add(btnCancel);
            footerPanel.Controls.Add(btnSave);
        }

        private void LoadCategoryDropdowns()
        {
            cmbCategory.Items.Clear();

            foreach (BookCategoryOption option in categoryOptions)
                cmbCategory.Items.Add(option);

            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        private void ApplyCategorySelection()
        {
            if (cmbCategory.SelectedItem is not BookCategoryOption selected)
                return;

            cmbGenre.Items.Clear();
            cmbGroup.Items.Clear();

            IEnumerable<BookCategoryOption> matches = categoryOptions
                .Where(item => string.Equals(item.CategoryName, selected.CategoryName, StringComparison.OrdinalIgnoreCase));

            foreach (string genre in matches
                .Select(item => item.Genre)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value))
            {
                cmbGenre.Items.Add(genre);
            }

            foreach (string groupName in matches
                .Select(item => item.GroupName)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value))
            {
                cmbGroup.Items.Add(groupName);
            }

            if (cmbGenre.Items.Count > 0)
                cmbGenre.SelectedIndex = 0;

            if (cmbGroup.Items.Count > 0)
                cmbGroup.SelectedIndex = 0;
        }

        private void LoadValues(BookDialogData? existingBook)
        {
            if (existingBook == null)
            {
                ApplyCategorySelection();
                return;
            }

            txtBookId.Text = existingBook.BookId;
            txtTitle.Text = existingBook.Title;
            txtAuthor.Text = existingBook.Author;
            numYear.Value = existingBook.PublishYear.HasValue
                ? Math.Min(numYear.Maximum, Math.Max(numYear.Minimum, existingBook.PublishYear.Value))
                : 0;
            numQuantity.Value = Math.Min(numQuantity.Maximum, Math.Max(numQuantity.Minimum, existingBook.Quantity));

            for (int i = 0; i < cmbCategory.Items.Count; i++)
            {
                if (cmbCategory.Items[i] is BookCategoryOption option &&
                    string.Equals(option.CategoryName, existingBook.Category, StringComparison.OrdinalIgnoreCase))
                {
                    cmbCategory.SelectedIndex = i;
                    break;
                }
            }

            ApplyCategorySelection();

            SelectComboValue(cmbGenre, existingBook.Genre);
            SelectComboValue(cmbGroup, existingBook.GroupName);
            SelectComboValue(cmbStatus, existingBook.Status);
        }

        private static void SelectComboValue(ComboBox comboBox, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                string itemText = Convert.ToString(comboBox.Items[i]) ?? "";

                if (string.Equals(itemText, value, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }

            comboBox.Items.Add(value);
            comboBox.SelectedItem = value;
        }

        private void SaveClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter the book title.", "Missing Title", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Please enter the author name.", "Missing Author", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAuthor.Focus();
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Missing Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private TextBox CreateTextBox(string value, bool readOnly)
        {
            return new TextBox
            {
                Text = value,
                ReadOnly = readOnly,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F, readOnly ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = readOnly ? primaryDeep : primaryText,
                BackColor = fieldBack,
                Multiline = true,
                Margin = Padding.Empty
            };
        }

        private ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F),
                BackColor = fieldBack,
                ForeColor = primaryText
            };
        }

        private Label CreateFieldLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = secondaryText,
                BackColor = Color.Transparent
            };
        }

        private void AddLabeledControl(Control parent, string labelText, Control input, int x, int y, int width, int height)
        {
            Label label = CreateFieldLabel(labelText);
            label.Location = new Point(x, y);
            parent.Controls.Add(label);

            input.Location = new Point(x, y + 24);
            input.Size = new Size(width, height);
            parent.Controls.Add(input);
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control control)
                return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(control.BackColor);
            using System.Drawing.Drawing2D.GraphicsPath path = GetRoundedRectanglePath(
                new Rectangle(0, 0, control.Width - 1, control.Height - 1),
                14);

            e.Graphics.FillPath(brush, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
