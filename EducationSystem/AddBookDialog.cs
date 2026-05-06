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
            ClientSize = new Size(760, 680);
            MinimumSize = new Size(760, 680);
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
                Bounds = new Rectangle(1, 1, ClientSize.Width - 2, 86),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            Label title = new Label
            {
                Text = editMode ? "Edit Book Details" : "Add New Book to Catalog",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(36, 27)
            };

            Button close = new Button
            {
                Text = "x",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(38, 38),
                Location = new Point(headerPanel.Width - 54, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = headerBack,
                ForeColor = ColorTranslator.FromHtml("#BBCAC3"),
                Font = new Font("Segoe UI", 16F, FontStyle.Regular),
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

            bodyPanel = new Panel
            {
                BackColor = formBack,
                Bounds = new Rectangle(1, headerPanel.Bottom, ClientSize.Width - 2, 496),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                Padding = new Padding(36, 30, 36, 30)
            };

            footerPanel = new Panel
            {
                BackColor = footerBack,
                Bounds = new Rectangle(1, bodyPanel.Bottom, ClientSize.Width - 2, ClientSize.Height - bodyPanel.Bottom - 1),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            Controls.Add(headerPanel);
            Controls.Add(bodyPanel);
            Controls.Add(footerPanel);

            BuildBody(bookId, recordedBy);
            BuildFooter();
        }

        private void BuildBody(string bookId, string recordedBy)
        {
            int left = 36;
            int top = 24;
            int gap = 30;
            int colWidth = (ClientSize.Width - 72 - gap) / 2;
            int fullWidth = ClientSize.Width - 72;

            txtBookId = CreateTextBox(bookId, true);
            txtRecordedBy = CreateTextBox(recordedBy, true);
            AddLabeledControl(bodyPanel, "BOOK ID (SYSTEM GENERATED)", txtBookId, left, top, colWidth, 48);
            AddLabeledControl(bodyPanel, "RECORDED BY", txtRecordedBy, left + colWidth + gap, top, colWidth, 48);

            top += 92;
            txtTitle = CreateTextBox("", false);
            txtTitle.PlaceholderText = "Enter full publication title...";
            txtTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            AddLabeledControl(bodyPanel, "BOOK TITLE", txtTitle, left, top, fullWidth, 54);

            top += 92;
            txtAuthor = CreateTextBox("", false);
            txtAuthor.PlaceholderText = "Full name of primary author...";
            AddLabeledControl(bodyPanel, "AUTHOR NAME", txtAuthor, left, top, fullWidth, 50);

            top += 88;
            int selectWidth = (fullWidth - 40) / 3;
            cmbCategory = CreateComboBox();
            cmbGenre = CreateComboBox();
            cmbGroup = CreateComboBox();

            AddLabeledControl(bodyPanel, "CATEGORY", cmbCategory, left, top, selectWidth, 46);
            AddLabeledControl(bodyPanel, "GENRE", cmbGenre, left + selectWidth + 20, top, selectWidth, 46);
            AddLabeledControl(bodyPanel, "GROUP", cmbGroup, left + (selectWidth + 20) * 2, top, selectWidth, 46);

            top += 86;
            Panel inventoryPanel = new Panel
            {
                BackColor = footerBack,
                Bounds = new Rectangle(left, top, fullWidth, 98)
            };
            inventoryPanel.Paint += RoundedPanelPaint;
            bodyPanel.Controls.Add(inventoryPanel);

            numYear = new NumericUpDown
            {
                Minimum = 0,
                Maximum = DateTime.Today.Year + 5,
                Value = 0,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = primaryText,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(18, 48),
                Size = new Size(150, 34)
            };

            numQuantity = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 99999,
                Value = 1,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = primaryText,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(192, 48),
                Size = new Size(150, 34)
            };

            cmbStatus = CreateComboBox();
            cmbStatus.Items.AddRange(new object[] { "In Stock", "Reserved", "On Order", "Restricted" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.Location = new Point(366, 46);
            cmbStatus.Size = new Size(fullWidth - 386, 34);

            Label yearLabel = CreateSectionLabel("YEAR", 18, 20);
            Label qtyLabel = CreateSectionLabel("QUANTITY (QTY)", 192, 20);
            Label statusLabel = CreateSectionLabel("AVAILABILITY STATUS", 366, 20);

            inventoryPanel.Controls.Add(yearLabel);
            inventoryPanel.Controls.Add(qtyLabel);
            inventoryPanel.Controls.Add(statusLabel);
            inventoryPanel.Controls.Add(numYear);
            inventoryPanel.Controls.Add(numQuantity);
            inventoryPanel.Controls.Add(cmbStatus);

            BindClassificationChoices();
        }

        private void BuildFooter()
        {
            btnCancel = new Button
            {
                Text = "Cancel",
                FlatStyle = FlatStyle.Flat,
                BackColor = footerBack,
                ForeColor = mutedText,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Size = new Size(112, 48),
                Cursor = Cursors.Hand,
                Location = new Point(ClientSize.Width - 36 - 210 - 18 - 112, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            btnSave = new Button
            {
                Text = editMode ? "Update Book" : "Save Book",
                FlatStyle = FlatStyle.Flat,
                BackColor = primary,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Size = new Size(210, 50),
                Cursor = Cursors.Hand,
                Location = new Point(ClientSize.Width - 36 - 210, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += Save_Click;

            footerPanel.Controls.Add(btnCancel);
            footerPanel.Controls.Add(btnSave);

            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        private void BindClassificationChoices()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("Select Category");
            cmbCategory.Items.AddRange(categoryOptions.Cast<object>().ToArray());
            cmbCategory.SelectedIndex = 0;

            cmbGenre.Items.Clear();
            cmbGenre.Items.Add("Select Genre");
            cmbGenre.Items.AddRange(categoryOptions
                .Select(item => item.Genre)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value)
                .Cast<object>()
                .ToArray());
            cmbGenre.SelectedIndex = 0;

            cmbGroup.Items.Clear();
            cmbGroup.Items.Add("Select Group");
            cmbGroup.Items.AddRange(categoryOptions
                .Select(item => item.GroupName)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value)
                .Cast<object>()
                .ToArray());
            cmbGroup.SelectedIndex = 0;

            cmbCategory.SelectedIndexChanged += (s, e) =>
            {
                if (cmbCategory.SelectedItem is BookCategoryOption selected)
                {
                    SelectComboValue(cmbGenre, selected.Genre);
                    SelectComboValue(cmbGroup, selected.GroupName);
                }
            };
        }

        private void LoadValues(BookDialogData? existingBook)
        {
            if (existingBook == null)
                return;

            txtBookId.Text = existingBook.BookId;
            txtTitle.Text = existingBook.Title;
            txtAuthor.Text = existingBook.Author;
            numYear.Value = existingBook.PublishYear.HasValue
                ? Math.Min(numYear.Maximum, Math.Max(numYear.Minimum, existingBook.PublishYear.Value))
                : 0;
            numQuantity.Value = Math.Min(numQuantity.Maximum, Math.Max(numQuantity.Minimum, existingBook.Quantity));
            SelectCategory(existingBook.Category, existingBook.Genre, existingBook.GroupName);
            SelectComboValue(cmbGenre, existingBook.Genre);
            SelectComboValue(cmbGroup, existingBook.GroupName);
            SelectComboValue(cmbStatus, existingBook.Status);
        }

        private void SelectCategory(string category, string genre, string group)
        {
            for (int i = 0; i < cmbCategory.Items.Count; i++)
            {
                if (cmbCategory.Items[i] is BookCategoryOption item &&
                    item.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase))
                {
                    cmbCategory.SelectedIndex = i;
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                BookCategoryOption option = new BookCategoryOption(category, genre, group);
                cmbCategory.Items.Insert(0, option);
                cmbCategory.SelectedIndex = 0;
            }
        }

        private void SelectComboValue(ComboBox comboBox, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (string.Equals(Convert.ToString(comboBox.Items[i]), value, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }

            comboBox.Items.Insert(0, value);
            comboBox.SelectedIndex = 0;
        }

        private TextBox CreateTextBox(string text, bool readOnly)
        {
            return new TextBox
            {
                Text = text,
                ReadOnly = readOnly,
                BorderStyle = BorderStyle.None,
                BackColor = fieldBack,
                ForeColor = readOnly ? primaryDeep : primaryText,
                Font = new Font("Segoe UI", 11F, readOnly ? FontStyle.Bold : FontStyle.Regular),
                Multiline = true
            };
        }

        private ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = footerBack,
                ForeColor = primaryText,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                IntegralHeight = false,
                DropDownHeight = 160
            };
        }

        private Label CreateSectionLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = secondaryText,
                BackColor = Color.Transparent,
                Location = new Point(x, y)
            };
        }

        private void AddLabeledControl(Control parent, string labelText, Control input, int x, int y, int width, int height)
        {
            Label label = CreateSectionLabel(labelText, x, y);
            Panel host = new Panel
            {
                BackColor = input is ComboBox ? footerBack : fieldBack,
                Bounds = new Rectangle(x, y + 26, width, height)
            };
            host.Paint += RoundedPanelPaint;

            input.Location = new Point(18, input is ComboBox ? 11 : height > 54 ? 17 : 14);
            input.Size = new Size(width - 36, height - 18);
            input.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            host.Controls.Add(input);
            parent.Controls.Add(label);
            parent.Controls.Add(host);
        }

        private void Save_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                ShowValidation("Please enter a book title.", txtTitle);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                ShowValidation("Please enter an author name.", txtAuthor);
                return;
            }

            if (numYear.Value > 0 && numYear.Value < 1000)
            {
                ShowValidation("Please enter a valid publication year or leave it as 0.", numYear);
                return;
            }

            if (cmbCategory.SelectedItem is not BookCategoryOption)
            {
                ShowValidation("Please select a category.", cmbCategory);
                return;
            }

            if (cmbGenre.SelectedItem == null ||
                string.Equals(Convert.ToString(cmbGenre.SelectedItem), "Select Genre", StringComparison.OrdinalIgnoreCase))
            {
                ShowValidation("Please select a genre.", cmbGenre);
                return;
            }

            if (cmbGroup.SelectedItem == null ||
                string.Equals(Convert.ToString(cmbGroup.SelectedItem), "Select Group", StringComparison.OrdinalIgnoreCase))
            {
                ShowValidation("Please select a group.", cmbGroup);
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

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control control || control.Width <= 1 || control.Height <= 1)
                return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(control.BackColor);
            using System.Drawing.Drawing2D.GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, control.Width - 1, control.Height - 1), 9);
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
