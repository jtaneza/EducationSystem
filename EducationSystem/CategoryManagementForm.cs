using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class CategoryManagementForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color CardSoft = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color CardSoftLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color FieldBack = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color AccentMint = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color MutedText = ColorTranslator.FromHtml("#BBCAC3");
        private readonly Color LineSoft = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color Outline = ColorTranslator.FromHtml("#BBCAC3");

        private Panel canvas = null!;

        private Label lblTitle = null!;
        private Label lblSubTitle = null!;
        private Button btnAddCategory = null!;

        private Panel cardTotalCategories = null!;
        private Panel cardRecent = null!;
        private Panel cardCoverage = null!;

        private Panel directoryShell = null!;
        private Panel directoryCard = null!;
        private Panel directoryHeader = null!;
        private Label lblDirectoryTitle = null!;
        private Panel searchBox = null!;
        private TextBox txtSearch = null!;
        private Label lblSearchIcon = null!;

        private Panel tableHeader = null!;
        private Panel rowsPanel = null!;
        private Label lblEmptyState = null!;
        private Label lblFooterInfo = null!;
        private Panel pagerPanel = null!;

        private readonly List<CategoryRow> categories = new List<CategoryRow>();

        private int currentPage = 1;
        private const int PageSize = 5;
        private Label pagerLeft = null!;
        private Label pagerText = null!;
        private Label pagerRight = null!;

        private sealed class CategoryRow
        {
            public string Id { get; }
            public string Name { get; set; }
            public string Genre { get; set; }
            public string Group { get; set; }
            public string Initials { get; set; }
            public string AddedBy { get; set; }
            public string Icon { get; set; }
            public string IconBack { get; set; }
            public bool DangerIcon { get; set; }
            public string Description { get; set; }

            public CategoryRow(string id, string name, string genre, string group, string initials, string addedBy, string icon, string iconBack, bool dangerIcon, string description = "")
            {
                Id = id;
                Name = name;
                Genre = genre;
                Group = group;
                Initials = initials;
                AddedBy = addedBy;
                Icon = icon;
                IconBack = iconBack;
                DangerIcon = dangerIcon;
                Description = description;
            }
        }

        public CategoryManagementForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Text = "CategoryManagementForm";
            MinimumSize = new Size(760, 520);
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            BuildInterface();

            Load += async (s, e) =>
            {
                ForceFillParent();

                if (directoryShell != null)
                    directoryShell.Visible = false;

                if (cardTotalCategories != null) cardTotalCategories.Visible = false;
                if (cardRecent != null) cardRecent.Visible = false;
                if (cardCoverage != null) cardCoverage.Visible = false;

                await LoadCategoriesFromDatabaseAsync();

                if (IsDisposed || Disposing)
                    return;

                if (cardTotalCategories != null) cardTotalCategories.Visible = true;
                if (cardRecent != null) cardRecent.Visible = true;
                if (cardCoverage != null) cardCoverage.Visible = true;
                if (directoryShell != null) directoryShell.Visible = true;

                SafeBeginAdjustLayout();
            };
            Resize += (s, e) =>
            {
                if (!IsDisposed && !Disposing)
                    AdjustLayout();
            };

            ParentChanged += (s, e) =>
            {
                if (IsDisposed || Disposing || Parent == null)
                    return;

                ForceFillParent();
                SafeBeginAdjustLayout();
            };

            VisibleChanged += (s, e) =>
            {
                if (IsDisposed || Disposing || !Visible)
                    return;

                ForceFillParent();
                SafeBeginAdjustLayout();
            };

            Shown += (s, e) =>
            {
                if (IsDisposed || Disposing)
                    return;

                ForceFillParent();
                SafeBeginAdjustLayout();
            };
        }
        private void SafeBeginAdjustLayout()
        {
            if (IsDisposed || Disposing)
                return;

            if (!IsHandleCreated)
                return;

            BeginInvoke(new Action(() =>
            {
                if (IsDisposed || Disposing)
                    return;

                ForceFillParent();
                AdjustLayout();
            }));
        }

        private void ForceFillParent()
        {
            if (Parent == null)
                return;

            Dock = DockStyle.Fill;
            Location = Point.Empty;
            Size = Parent.ClientSize;
            Bounds = Parent.ClientRectangle;
        }

        private void BuildInterface()
        {
            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = FormBack,
                AutoScroll = true,
                AutoScrollMargin = new Size(0, 0),
                AutoScrollMinSize = new Size(0, 0)
            };
            canvas.HorizontalScroll.Enabled = false;
            canvas.HorizontalScroll.Visible = false;
            Controls.Add(canvas);

            lblTitle = CreateLabel("Category Management", 30F, FontStyle.Bold, PrimaryText);
            lblSubTitle = CreateLabel("Organize and curate the library's intellectual mapping.", 12F, FontStyle.Regular, SecondaryText);

            btnAddCategory = new Button
            {
                Text = "+  Add New Category",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddCategory.FlatAppearance.BorderSize = 0;
            btnAddCategory.Click += async (s, e) => await ShowAddCategoryDialogAsync();

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubTitle);
            canvas.Controls.Add(btnAddCategory);

            BuildStatCards();
            BuildDirectory();
        }

        private void BuildStatCards()
        {
            cardTotalCategories = CreateCard(CardBack);
            cardRecent = CreateCard(CardBack);
            cardCoverage = CreateCard(CardBack);

            canvas.Controls.Add(cardTotalCategories);
            canvas.Controls.Add(cardRecent);
            canvas.Controls.Add(cardCoverage);

            BuildStatCard(cardTotalCategories, "⌘", "0", "TOTAL CATEGORIES", "", ColorTranslator.FromHtml("#B7EBD7"));
            BuildStatCard(cardRecent, "↺", "No category yet", "RECENTLY ADDED", "", ColorTranslator.FromHtml("#B7EBD7"));
            BuildStatCard(cardCoverage, "☑", "0%", "CLASSIFICATION COVERAGE", "", ColorTranslator.FromHtml("#E1F6EE"));
        }

        private void BuildStatCard(Panel card, string iconText, string valueText, string captionText, string badgeText, Color iconBack)
        {
            Panel iconBox = new Panel
            {
                Name = "IconBox",
                Size = new Size(56, 56),
                BackColor = iconBack
            };
            iconBox.Paint += RoundedPanelPaint;

            Label icon = new Label
            {
                Text = iconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 18F, FontStyle.Bold),
                ForeColor = AccentDeep,
                BackColor = Color.Transparent
            };
            iconBox.Controls.Add(icon);

            Label badge = new Label
            {
                Name = "Badge",
                Text = badgeText,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(35, 109, 250, 210),
                ForeColor = AccentDeep,
                Visible = !string.IsNullOrWhiteSpace(badgeText)
            };

            Label value = CreateLabel(valueText, valueText.Length > 8 ? 20F : 30F, FontStyle.Bold, PrimaryText);
            value.Name = "Value";

            Label caption = CreateLabel(captionText, 10.5F, FontStyle.Bold, SecondaryText);
            caption.Name = "Caption";

            card.Controls.Add(iconBox);
            card.Controls.Add(badge);
            card.Controls.Add(value);
            card.Controls.Add(caption);
        }

        private void BuildDirectory()
        {
            directoryShell = new Panel { BackColor = Color.Transparent };
            directoryCard = CreateCard(CardBack);
            directoryHeader = new Panel { BackColor = CardBack };

            lblDirectoryTitle = CreateLabel("▤  Category Directory", 16F, FontStyle.Bold, PrimaryText);

            searchBox = new Panel
            {
                BackColor = FieldBack
            };
            searchBox.Paint += RoundedPanelPaint;

            lblSearchIcon = new Label
            {
                Text = "⌕",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 11F),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = FieldBack,
                ForeColor = SecondaryText,
                Font = new Font("Segoe UI", 11F),
                Text = "Search Categories..."
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search Categories...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = PrimaryText;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search Categories...";
                    txtSearch.ForeColor = SecondaryText;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                if (!txtSearch.Focused && txtSearch.Text == "Search Categories...")
                    return;

                currentPage = 1;
                RebuildRows(txtSearch.Text);
                AdjustLayout();
            };

            searchBox.Controls.Add(lblSearchIcon);
            searchBox.Controls.Add(txtSearch);

            directoryHeader.Controls.Add(lblDirectoryTitle);
            directoryHeader.Controls.Add(searchBox);

            tableHeader = new Panel
            {
                BackColor = CardSoftLow
            };

            rowsPanel = new Panel
            {
                BackColor = CardBack
            };

            lblEmptyState = CreateLabel("No categories yet. Click Add New Category to create one.", 11F, FontStyle.Regular, SecondaryText);
            lblEmptyState.Name = "EmptyState";
            lblEmptyState.Visible = false;
            rowsPanel.Controls.Add(lblEmptyState);

            lblFooterInfo = CreateLabel("SHOWING 0 OF 0 CATEGORIES", 9.5F, FontStyle.Bold, SecondaryText);

            pagerPanel = new Panel
            {
                BackColor = Color.Transparent
            };
            BuildPager();

            directoryCard.Controls.Add(directoryHeader);
            directoryCard.Controls.Add(tableHeader);
            directoryCard.Controls.Add(rowsPanel);
            directoryCard.Controls.Add(lblFooterInfo);
            directoryCard.Controls.Add(pagerPanel);

            directoryShell.Controls.Add(directoryCard);
            canvas.Controls.Add(directoryShell);

            BuildTableHeader();
        }

        private void BuildTableHeader()
        {
            tableHeader.Controls.Clear();

            string[] headers = { "CATEGORY ID", "CATEGORY NAME", "GENRE", "GROUP", "ADDED BY", "ACTIONS" };
            foreach (string header in headers)
            {
                tableHeader.Controls.Add(new Label
                {
                    Name = "Header" + header.Replace(" ", ""),
                    Text = header,
                    AutoSize = false,
                    TextAlign = header == "ACTIONS" ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
                    ForeColor = SecondaryText,
                    BackColor = Color.Transparent
                });
            }
        }

        private void RebuildRows(string keyword)
        {
            rowsPanel.Controls.Clear();

            if (lblEmptyState != null)
                rowsPanel.Controls.Add(lblEmptyState);

            string q = keyword == "Search Categories..." ? "" : keyword.Trim().ToLowerInvariant();

            List<CategoryRow> filteredRows = categories
                .Where(row =>
                {
                    string haystack = $"{row.Id} {row.Name} {row.Genre} {row.Group} {row.Initials} {row.AddedBy}".ToLowerInvariant();
                    return string.IsNullOrWhiteSpace(q) || haystack.Contains(q);
                })
                .ToList();

            int totalPages = Math.Max(1, (int)Math.Ceiling(filteredRows.Count / (double)PageSize));

            if (currentPage > totalPages)
                currentPage = totalPages;

            if (currentPage < 1)
                currentPage = 1;

            List<CategoryRow> pageRows = filteredRows
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            int index = 0;
            foreach (CategoryRow row in pageRows)
            {
                Panel rowPanel = BuildCategoryRow(row);
                rowPanel.Name = "Row" + index;
                rowPanel.Tag = "CategoryRow";
                rowsPanel.Controls.Add(rowPanel);
                index++;
            }

            if (lblEmptyState != null)
                lblEmptyState.Visible = filteredRows.Count == 0;

            int startItem = filteredRows.Count == 0 ? 0 : ((currentPage - 1) * PageSize) + 1;
            int endItem = filteredRows.Count == 0 ? 0 : Math.Min(filteredRows.Count, startItem + pageRows.Count - 1);

            lblFooterInfo.Text = filteredRows.Count == 0
                ? "SHOWING 0 OF 0 CATEGORIES"
                : $"SHOWING {startItem}-{endItem} OF {filteredRows.Count} CATEGORIES";

            UpdatePager(totalPages);
        }

        private async Task ShowAddCategoryDialogAsync()
        {
            string addedBy = GetCurrentUserName();

            using EducationSystem.AddCategoryDialog dialog = new EducationSystem.AddCategoryDialog(
                GenerateCategoryId(),
                addedBy,
                FormBack,
                CardBack,
                FieldBack,
                AccentEmerald,
                AccentDeep,
                PrimaryText,
                SecondaryText
            );

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            CategoryRow row = new CategoryRow(
                dialog.CategoryId,
                dialog.CategoryName,
                dialog.Genre,
                dialog.GroupName.ToUpperInvariant(),
                GetInitials(addedBy),
                addedBy,
                GetCategoryIcon(dialog.CategoryName, dialog.Genre, dialog.GroupName),
                GetCategoryIconBack(dialog.GroupName),
                dialog.GroupName.Contains("history", StringComparison.OrdinalIgnoreCase) ||
                dialog.Genre.Contains("history", StringComparison.OrdinalIgnoreCase),
                dialog.Description
            );

            btnAddCategory.Enabled = false;
            btnAddCategory.Text = "Saving...";

            try
            {
                await SaveCategoryToDatabaseAsync(row);

                categories.Insert(0, row);
                currentPage = 1;
                RebuildRows(GetCurrentSearchText());
                UpdateSummaryCards(row);
                AdjustLayout();

                MessageBox.Show(
                    "Category saved to database.",
                    "Category Added",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Category was not saved to the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnAddCategory.Text = "+  Add New Category";
                btnAddCategory.Enabled = true;
            }
        }

        private async Task LoadCategoriesFromDatabaseAsync()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                await conn.OpenAsync();

                await EnsureCategoriesTableAsync(conn);

                const string selectQuery = @"
SELECT CategoryID, CategoryName, Genre, GroupName, Description, AddedBy
FROM dbo.Categories
WHERE IsArchived = 0
  AND ClientID = @ClientID
ORDER BY CreatedAt DESC;";

                using SqlCommand cmd = new SqlCommand(selectQuery, conn);
                cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                categories.Clear();

                while (await reader.ReadAsync())
                {
                    string id = reader["CategoryID"]?.ToString() ?? "";
                    string name = reader["CategoryName"]?.ToString() ?? "";
                    string genre = reader["Genre"]?.ToString() ?? "";
                    string group = reader["GroupName"]?.ToString() ?? "";
                    string description = reader["Description"] == DBNull.Value ? "" : reader["Description"]?.ToString() ?? "";
                    string addedBy = reader["AddedBy"] == DBNull.Value ? "Admin-01" : reader["AddedBy"]?.ToString() ?? "Admin-01";

                    categories.Add(new CategoryRow(
                        id,
                        name,
                        genre,
                        group,
                        GetInitials(addedBy),
                        addedBy,
                        GetCategoryIcon(name, genre, group),
                        GetCategoryIconBack(group),
                        group.Equals("HISTORY", StringComparison.OrdinalIgnoreCase) ||
                        genre.Contains("History", StringComparison.OrdinalIgnoreCase),
                        description
                    ));
                }

                RebuildRows(GetCurrentSearchText());
                RefreshSummaryCards();
                AdjustLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Categories could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void RefreshSummaryCards()
        {
            if (cardTotalCategories.Controls["Value"] is Label totalValue)
                totalValue.Text = categories.Count.ToString();

            if (cardRecent.Controls["Value"] is Label recentValue)
                recentValue.Text = categories.Count > 0 ? categories[0].Name : "No category yet";

            Label? coverageLabel = cardCoverage.Controls["Value"] as Label;
            if (coverageLabel != null)
                coverageLabel.Text = categories.Count > 0 ? "100%" : "0%";

            LayoutStatCard(cardTotalCategories);
            LayoutStatCard(cardRecent);
            LayoutStatCard(cardCoverage);
        }

        private string GetCategoryIcon(string name, string genre, string group)
        {
            string value = (name + " " + genre + " " + group).ToLowerInvariant();

            if (value.Contains("history")) return "✎";
            if (value.Contains("architecture")) return "✿";
            if (value.Contains("science")) return "⚗";
            if (value.Contains("math")) return "∑";
            if (value.Contains("art")) return "◈";
            if (value.Contains("law")) return "§";

            return "✦";
        }

        private string GetCategoryIconBack(string group)
        {
            if (group.Contains("history", StringComparison.OrdinalIgnoreCase))
                return "#FFDAD6";

            if (group.Contains("modern", StringComparison.OrdinalIgnoreCase))
                return "#E1F6EE";

            return "#B7EBD7";
        }

        private async Task ShowEditCategoryDialogAsync(CategoryRow row)
        {
            using EducationSystem.AddCategoryDialog dialog = new EducationSystem.AddCategoryDialog(
                row.Id,
                row.AddedBy,
                FormBack,
                CardBack,
                FieldBack,
                AccentEmerald,
                AccentDeep,
                PrimaryText,
                SecondaryText
            );

            PrefillDialogTextBoxes(dialog, row);

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            row.Name = dialog.CategoryName;
            row.Genre = dialog.Genre;
            row.Group = dialog.GroupName.ToUpperInvariant();
            row.Icon = GetCategoryIcon(row.Name, row.Genre, row.Group);
            row.IconBack = GetCategoryIconBack(row.Group);
            row.DangerIcon =
                row.Group.Contains("history", StringComparison.OrdinalIgnoreCase) ||
                row.Genre.Contains("history", StringComparison.OrdinalIgnoreCase);
            row.Description = dialog.Description;

            try
            {
                await UpdateCategoryInDatabaseAsync(row);

                RebuildRows(GetCurrentSearchText());
                RefreshSummaryCards();
                AdjustLayout();

                MessageBox.Show(
                    "Category updated.",
                    "Category Updated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Category was not updated.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void PrefillDialogTextBoxes(Form dialog, CategoryRow row)
        {
            foreach (Control control in GetAllControls(dialog))
            {
                if (control is TextBox box)
                {
                    if (box.Text == row.Id)
                    {
                        box.Text = row.Id;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(box.Text) && box.PlaceholderText.Contains("Science Fiction"))
                        box.Text = row.Name;
                    else if (string.IsNullOrWhiteSpace(box.Text) && box.PlaceholderText.Contains("Academic"))
                        box.Text = row.Genre;
                    else if (string.IsNullOrWhiteSpace(box.Text) && box.PlaceholderText.Contains("Humanities"))
                        box.Text = row.Group;
                    else if (string.IsNullOrWhiteSpace(box.Text) && box.PlaceholderText.Contains("Briefly"))
                        box.Text = row.Description;
                }
            }
        }

        private IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;

                foreach (Control grandChild in GetAllControls(child))
                    yield return grandChild;
            }
        }

        private async Task UpdateCategoryInDatabaseAsync(CategoryRow row)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            await conn.OpenAsync();

            await EnsureCategoriesTableAsync(conn);

            const string updateQuery = @"
UPDATE dbo.Categories
SET CategoryName = @CategoryName,
    Genre = @Genre,
    GroupName = @GroupName,
    Description = @Description
WHERE CategoryID = @CategoryID
  AND ClientID = @ClientID;";

            using SqlCommand cmd = new SqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            cmd.Parameters.AddWithValue("@CategoryID", row.Id);
            cmd.Parameters.AddWithValue("@CategoryName", row.Name);
            cmd.Parameters.AddWithValue("@Genre", row.Genre);
            cmd.Parameters.AddWithValue("@GroupName", row.Group);
            cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(row.Description) ? (object)DBNull.Value : row.Description);

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task ArchiveCategoryAsync(CategoryRow row)
        {
            DialogResult result = MessageBox.Show(
                "Archive this category?",
                "Archive Category",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                await conn.OpenAsync();

                await EnsureCategoriesTableAsync(conn);
                await EnsureCategoryArchiveTableAsync(conn);

                using SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    const string archiveQuery = @"
INSERT INTO dbo.CategoryArchive
(ClientID, CategoryID, CategoryName, Genre, GroupName, Description, AddedBy, ArchivedBy, ArchivedAt)
VALUES
(@ClientID, @CategoryID, @CategoryName, @Genre, @GroupName, @Description, @AddedBy, @ArchivedBy, SYSUTCDATETIME());";

                    using SqlCommand archiveCmd = new SqlCommand(archiveQuery, conn, transaction);
                    archiveCmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                    archiveCmd.Parameters.AddWithValue("@CategoryID", row.Id);
                    archiveCmd.Parameters.AddWithValue("@CategoryName", row.Name);
                    archiveCmd.Parameters.AddWithValue("@Genre", row.Genre);
                    archiveCmd.Parameters.AddWithValue("@GroupName", row.Group);
                    archiveCmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(row.Description) ? (object)DBNull.Value : row.Description);
                    archiveCmd.Parameters.AddWithValue("@AddedBy", row.AddedBy);
                    archiveCmd.Parameters.AddWithValue("@ArchivedBy", GetCurrentUserName());
                    await archiveCmd.ExecuteNonQueryAsync();

                    const string updateQuery = @"
UPDATE dbo.Categories
SET IsArchived = 1
WHERE CategoryID = @CategoryID
  AND ClientID = @ClientID;";

                    using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction);
                    updateCmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
                    updateCmd.Parameters.AddWithValue("@CategoryID", row.Id);
                    await updateCmd.ExecuteNonQueryAsync();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

                categories.RemoveAll(c => c.Id.Equals(row.Id, StringComparison.OrdinalIgnoreCase));

                RebuildRows(GetCurrentSearchText());
                RefreshSummaryCards();
                AdjustLayout();

                MessageBox.Show(
                    "Category moved to archive.",
                    "Archived",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Category was not archived.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async Task EnsureCategoryArchiveTableAsync(SqlConnection conn)
        {
            const string createArchiveTableQuery = @"
IF OBJECT_ID('dbo.CategoryArchive', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CategoryArchive
    (
        ArchiveID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        CategoryID NVARCHAR(20) NOT NULL,
        CategoryName NVARCHAR(150) NOT NULL,
        Genre NVARCHAR(100) NOT NULL,
        GroupName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        AddedBy NVARCHAR(150) NULL,
        ArchivedBy NVARCHAR(150) NULL,
        ArchivedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF COL_LENGTH('dbo.CategoryArchive', 'ClientID') IS NULL
    ALTER TABLE dbo.CategoryArchive ADD ClientID INT NULL;";

            using SqlCommand cmd = new SqlCommand(createArchiveTableQuery, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task SaveCategoryToDatabaseAsync(CategoryRow row)
        {
            using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
            await conn.OpenAsync();

            await EnsureCategoriesTableAsync(conn);

            const string insertQuery = @"
INSERT INTO dbo.Categories
    (ClientID, CategoryID, CategoryName, Genre, GroupName, Description, AddedBy, CreatedAt, IsArchived)
VALUES
    (@ClientID, @CategoryID, @CategoryName, @Genre, @GroupName, @Description, @AddedBy, SYSUTCDATETIME(), 0);";

            using SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@ClientID", ClientSession.ClientId);
            cmd.Parameters.AddWithValue("@CategoryID", row.Id);
            cmd.Parameters.AddWithValue("@CategoryName", row.Name);
            cmd.Parameters.AddWithValue("@Genre", row.Genre);
            cmd.Parameters.AddWithValue("@GroupName", row.Group);
            cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(row.Description) ? (object)DBNull.Value : row.Description);
            cmd.Parameters.AddWithValue("@AddedBy", row.AddedBy);

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task EnsureCategoriesTableAsync(SqlConnection conn)
        {
            const string createTableQuery = @"
IF OBJECT_ID('dbo.Categories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        CategoryID NVARCHAR(20) NOT NULL PRIMARY KEY,
        ClientID INT NULL,
        CategoryName NVARCHAR(150) NOT NULL,
        Genre NVARCHAR(100) NOT NULL,
        GroupName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        AddedBy NVARCHAR(150) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.Categories', 'ClientID') IS NULL
    ALTER TABLE dbo.Categories ADD ClientID INT NULL;";

            using SqlCommand cmd = new SqlCommand(createTableQuery, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        private string GenerateCategoryId()
        {
            string id;

            do
            {
                id = "CAT-" + Random.Shared.Next(1000, 10000);
            }
            while (categories.Any(category => category.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));

            return id;
        }

        private string GetCurrentUserName()
        {
            if (!string.IsNullOrWhiteSpace(ClientSession.Username))
                return ClientSession.Username!;

            if (!string.IsNullOrWhiteSpace(UserSession.Username))
                return UserSession.Username!;

            return "Admin-01";
        }

        private string GetInitials(string name)
        {
            string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "AD";

            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();

            return (parts[0][0].ToString() + parts[1][0]).ToUpperInvariant();
        }

        private string GetCurrentSearchText()
        {
            if (txtSearch == null || txtSearch.Text == "Search Categories...")
                return "";

            return txtSearch.Text;
        }

        private void UpdateSummaryCards(CategoryRow row)
        {
            if (cardTotalCategories.Controls["Value"] is Label totalValue)
                totalValue.Text = categories.Count.ToString();

            if (cardRecent.Controls["Value"] is Label recentValue)
                recentValue.Text = row.Name;

            Label? coverageLabel = cardCoverage.Controls["Value"] as Label;
            if (coverageLabel != null)
                coverageLabel.Text = categories.Count > 0 ? "100%" : "0%";

            LayoutStatCard(cardTotalCategories);
            LayoutStatCard(cardRecent);
            LayoutStatCard(cardCoverage);
        }

        private Panel BuildCategoryRow(CategoryRow data)
        {
            Panel row = new Panel
            {
                BackColor = CardBack
            };

            Label id = CreateLabel(data.Id, 9.5F, FontStyle.Bold, PrimaryText);
            id.Name = "Id";

            Panel iconBox = new Panel
            {
                Name = "IconBox",
                Size = new Size(34, 34),
                BackColor = ColorTranslator.FromHtml(data.IconBack)
            };
            iconBox.Paint += RoundedPanelPaint;

            Label icon = new Label
            {
                Text = data.Icon,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 10F, FontStyle.Bold),
                ForeColor = data.DangerIcon ? AccentDanger : AccentDeep,
                BackColor = Color.Transparent
            };
            iconBox.Controls.Add(icon);

            Label name = CreateLabel(data.Name, 12F, FontStyle.Bold, PrimaryText);
            name.Name = "Name";

            Label genre = CreateLabel(data.Genre, 11F, FontStyle.Regular, PrimaryText);
            genre.Name = "Genre";

            Label group = new Label
            {
                Name = "Group",
                Text = data.Group,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = AccentDeep,
                BackColor = ColorTranslator.FromHtml("#B7EBD7")
            };

            Panel avatar = new Panel
            {
                Name = "Avatar",
                Size = new Size(26, 26),
                BackColor = FieldBack
            };
            avatar.Paint += CirclePaint;

            Label avatarText = new Label
            {
                Text = data.Initials,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };
            avatar.Controls.Add(avatarText);

            Label addedBy = CreateLabel(data.AddedBy, 9.5F, FontStyle.Regular, SecondaryText);
            addedBy.Name = "AddedBy";

            Label edit = new Label
            {
                Name = "Edit",
                Text = "✎",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 13F, FontStyle.Bold),
                ForeColor = AccentDeep,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            edit.Click += async (s, e) => await ShowEditCategoryDialogAsync(data);

            Label archive = new Label
            {
                Name = "Archive",
                Text = "📥",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Symbol", 13F, FontStyle.Bold),
                ForeColor = AccentDanger,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            archive.Click += async (s, e) => await ArchiveCategoryAsync(data);

            row.Controls.Add(id);
            row.Controls.Add(iconBox);
            row.Controls.Add(name);
            row.Controls.Add(genre);
            row.Controls.Add(group);
            row.Controls.Add(avatar);
            row.Controls.Add(addedBy);
            row.Controls.Add(edit);
            row.Controls.Add(archive);

            return row;
        }

        private void BuildPager()
        {
            pagerPanel.Controls.Clear();

            pagerLeft = new Label
            {
                Text = "‹",
                AutoSize = false,
                Size = new Size(32, 32),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            pagerLeft.Click += (s, e) =>
            {
                if (currentPage <= 1) return;

                currentPage--;
                RebuildRows(GetCurrentSearchText());
                AdjustLayout();
            };

            pagerText = new Label
            {
                Text = "Page 1 of 1",
                AutoSize = false,
                Size = new Size(92, 32),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            pagerRight = new Label
            {
                Text = "›",
                AutoSize = false,
                Size = new Size(32, 32),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            pagerRight.Click += (s, e) =>
            {
                int totalPages = Math.Max(1, (int)Math.Ceiling(categories.Count / (double)PageSize));

                if (currentPage >= totalPages) return;

                currentPage++;
                RebuildRows(GetCurrentSearchText());
                AdjustLayout();
            };

            pagerLeft.Location = new Point(0, 0);
            pagerText.Location = new Point(36, 0);
            pagerRight.Location = new Point(132, 0);

            pagerPanel.Controls.Add(pagerLeft);
            pagerPanel.Controls.Add(pagerText);
            pagerPanel.Controls.Add(pagerRight);
            pagerPanel.Size = new Size(164, 32);
        }

        private void UpdatePager(int totalPages)
        {
            if (pagerText == null || pagerLeft == null || pagerRight == null)
                return;

            pagerText.Text = $"Page {currentPage} of {totalPages}";

            bool canGoLeft = currentPage > 1;
            bool canGoRight = currentPage < totalPages;

            pagerLeft.Enabled = canGoLeft;
            pagerRight.Enabled = canGoRight;

            pagerLeft.ForeColor = canGoLeft ? PrimaryText : Color.FromArgb(160, 160, 160);
            pagerRight.ForeColor = canGoRight ? PrimaryText : Color.FromArgb(160, 160, 160);
        }

        private void AdjustLayout()
        {
            if (canvas == null) return;

            bool compact = canvas.ClientSize.Width < 1050;
            bool narrow = canvas.ClientSize.Width < 760;
            bool tiny = canvas.ClientSize.Width < 560;

            int layoutWidth = canvas.ClientSize.Width;

            if (Parent != null && Parent.ClientSize.Width > layoutWidth)
                layoutWidth = Parent.ClientSize.Width;

            bool wide = layoutWidth >= 1200;
            compact = layoutWidth < 1050;
            narrow = layoutWidth < 760;
            tiny = layoutWidth < 560;

            int margin = tiny ? 14 : narrow ? 18 : compact ? 26 : 40;
            int gap = compact ? 16 : 24;
            int usableWidth = Math.Max(320, layoutWidth - (margin * 2) - 8);

            lblTitle.Font = new Font("Segoe UI", tiny ? 20F : narrow ? 23F : compact ? 26F : 30F, FontStyle.Bold);
            lblTitle.Location = new Point(margin, compact ? 24 : 34);

            lblSubTitle.Location = new Point(margin, lblTitle.Bottom + 6);
            lblSubTitle.MaximumSize = new Size(usableWidth, 0);

            if (narrow)
                btnAddCategory.Bounds = new Rectangle(margin, lblSubTitle.Bottom + 18, usableWidth, 48);
            else
                btnAddCategory.Bounds = new Rectangle(margin + usableWidth - 250, lblTitle.Top + 8, 250, 48);

            int cardsTop = Math.Max(lblSubTitle.Bottom + 30, btnAddCategory.Bottom + (narrow ? 18 : 0));
            int cardHeight = compact ? 190 : 190;

            if (narrow)
            {
                cardTotalCategories.Bounds = new Rectangle(margin, cardsTop, usableWidth, cardHeight);
                cardRecent.Bounds = new Rectangle(margin, cardTotalCategories.Bottom + gap, usableWidth, cardHeight);
                cardCoverage.Bounds = new Rectangle(margin, cardRecent.Bottom + gap, usableWidth, cardHeight);
            }
            else
            {
                int cardWidth = (usableWidth - (gap * 2)) / 3;
                cardTotalCategories.Bounds = new Rectangle(margin, cardsTop, cardWidth, cardHeight);
                cardRecent.Bounds = new Rectangle(cardTotalCategories.Right + gap, cardsTop, cardWidth, cardHeight);
                cardCoverage.Bounds = new Rectangle(cardRecent.Right + gap, cardsTop, usableWidth - (cardWidth * 2) - (gap * 2), cardHeight);
            }

            if (cardCoverage.Controls["Value"] is Label coverageValue)
                coverageValue.Text = categories.Count > 0 ? "100%" : "0%";

            LayoutStatCard(cardTotalCategories);
            LayoutStatCard(cardRecent);
            LayoutStatCard(cardCoverage);

            int directoryTop = Math.Max(cardTotalCategories.Bottom, Math.Max(cardRecent.Bottom, cardCoverage.Bottom)) + (compact ? 28 : 34);
            int rowHeight = tiny ? 132 : narrow ? 112 : 86;

            int visibleRowCount = 0;
            foreach (Control ctrl in rowsPanel.Controls)
            {
                if (Convert.ToString(ctrl.Tag) == "CategoryRow")
                    visibleRowCount++;
            }

            int emptyStateHeight = visibleRowCount == 0 ? 120 : 0;
            int directoryHeight = (narrow ? 116 : 92) + 54 + (visibleRowCount * rowHeight) + emptyStateHeight + 96;

            directoryShell.Bounds = new Rectangle(margin, directoryTop, usableWidth, directoryHeight);
            directoryCard.Bounds = new Rectangle(0, 0, Math.Max(1, directoryShell.Width), Math.Max(1, directoryShell.Height));

            LayoutDirectory(narrow, tiny, rowHeight);

            canvas.AutoScrollMinSize = new Size(0, directoryShell.Bottom + 42);
            canvas.HorizontalScroll.Enabled = false;
            canvas.HorizontalScroll.Visible = false;
        }

        private void LayoutStatCard(Panel card)
        {
            if (card == null || card.IsDisposed) return;

            bool small = card.Width < 300;

            var iconBox = card.Controls.Find("IconBox", false).FirstOrDefault();
            var value = card.Controls.Find("Value", false).FirstOrDefault() as Label;
            var caption = card.Controls.Find("Caption", false).FirstOrDefault() as Label;

            // 🚫 prevent crash
            if (iconBox == null || value == null || caption == null)
                return;

            iconBox.Size = small ? new Size(50, 50) : new Size(56, 56);
            iconBox.Location = new Point(26, 26);

            if (card.Controls.ContainsKey("Badge"))
                card.Controls["Badge"].Bounds = new Rectangle(card.Width - 94, 30, 70, 28);

            bool longValue = value.Text.Length > 8;

            value.AutoSize = false;
            caption.AutoSize = false;

            value.Font = new Font("Segoe UI",
                longValue ? (small ? 15F : 18F) : (small ? 25F : 28F),
                FontStyle.Bold);

            caption.Font = new Font("Segoe UI",
                small ? 8.4F : 9.2F,
                FontStyle.Bold);

            int left = 26;
            int width = Math.Max(120, card.Width - 52);

            value.Bounds = new Rectangle(left, 88, width, 50);
            caption.Bounds = new Rectangle(left, 148, width, 24);
        }
        private void LayoutDirectory(bool narrow, bool tiny, int rowHeight)
        {
            directoryHeader.Bounds = new Rectangle(0, 0, directoryCard.Width, narrow ? 116 : 92);
            lblDirectoryTitle.Location = new Point(28, 30);

            if (narrow)
                searchBox.Bounds = new Rectangle(28, lblDirectoryTitle.Bottom + 18, directoryCard.Width - 56, 42);
            else
                searchBox.Bounds = new Rectangle(directoryCard.Width - 280 - 28, 26, 280, 42);

            lblSearchIcon.Bounds = new Rectangle(10, 0, 28, searchBox.Height);
            txtSearch.Location = new Point(42, 12);
            txtSearch.Width = searchBox.Width - 54;

            tableHeader.Bounds = new Rectangle(0, directoryHeader.Bottom, directoryCard.Width, 54);
            LayoutTableHeader(narrow, tiny);

            rowsPanel.Bounds = new Rectangle(0, tableHeader.Bottom, directoryCard.Width, directoryCard.Height - tableHeader.Bottom - 82);

            if (lblEmptyState != null && lblEmptyState.Visible)
            {
                lblEmptyState.Location = new Point(28, 42);
                lblEmptyState.MaximumSize = new Size(Math.Max(120, rowsPanel.Width - 56), 0);
            }

            int y = 0;
            foreach (Control row in rowsPanel.Controls)
            {
                if (Convert.ToString(row.Tag) != "CategoryRow")
                    continue;

                row.Bounds = new Rectangle(0, y, rowsPanel.Width, rowHeight);
                LayoutCategoryRow(row, narrow, tiny);
                y += rowHeight;
            }

            lblFooterInfo.Location = new Point(28, directoryCard.Height - 58);
            lblFooterInfo.Font = new Font("Segoe UI", tiny ? 8.2F : 9.5F, FontStyle.Bold);

            pagerPanel.Visible = !tiny;
            pagerPanel.Location = new Point(Math.Max(28, directoryCard.Width - pagerPanel.Width - 42), directoryCard.Height - 62);
        }

        private void LayoutTableHeader(bool narrow, bool tiny)
        {
            int w = tableHeader.Width;

            int idW = tiny ? 96 : narrow ? 120 : (int)(w * 0.15);
            int actionW = tiny ? 86 : narrow ? 106 : 130;
            int genreW = narrow ? 0 : (int)(w * 0.17);
            int groupW = narrow ? 0 : (int)(w * 0.15);
            int addedW = narrow ? 0 : (int)(w * 0.18);
            int nameW = w - idW - actionW - genreW - groupW - addedW;

            int x = 0;

            tableHeader.Controls[0].Bounds = new Rectangle(28, 0, Math.Max(40, idW - 28), tableHeader.Height);
            x += idW;

            tableHeader.Controls[1].Bounds = new Rectangle(x, 0, Math.Max(80, nameW), tableHeader.Height);
            x += nameW;

            tableHeader.Controls[2].Visible = !narrow;
            tableHeader.Controls[3].Visible = !narrow;
            tableHeader.Controls[4].Visible = !narrow;

            if (!narrow)
            {
                tableHeader.Controls[2].Bounds = new Rectangle(x, 0, genreW, tableHeader.Height);
                x += genreW;

                tableHeader.Controls[3].Bounds = new Rectangle(x, 0, groupW, tableHeader.Height);
                x += groupW;

                tableHeader.Controls[4].Bounds = new Rectangle(x, 0, addedW, tableHeader.Height);
                x += addedW;
            }

            tableHeader.Controls[5].Bounds = new Rectangle(w - actionW - 28, 0, actionW, tableHeader.Height);
        }

        private void LayoutCategoryRow(Control row, bool narrow, bool tiny)
        {
            int w = row.Width;

            int idW = tiny ? 96 : narrow ? 120 : (int)(w * 0.15);
            int actionW = tiny ? 86 : narrow ? 106 : 130;
            int genreW = narrow ? 0 : (int)(w * 0.17);
            int groupW = narrow ? 0 : (int)(w * 0.15);
            int addedW = narrow ? 0 : (int)(w * 0.18);
            int nameW = w - idW - actionW - genreW - groupW - addedW;

            int x = 0;

            row.Controls["Id"].Location = new Point(28, (row.Height - 22) / 2);
            x += idW;

            row.Controls["IconBox"].Bounds = new Rectangle(x, (row.Height - 34) / 2, 34, 34);
            row.Controls["Name"].Location = new Point(x + 48, row.Height / 2 - 12);
            row.Controls["Name"].MaximumSize = new Size(Math.Max(60, nameW - 54), 0);
            x += nameW;

            row.Controls["Genre"].Visible = !narrow;
            row.Controls["Group"].Visible = !narrow;
            row.Controls["Avatar"].Visible = !narrow;
            row.Controls["AddedBy"].Visible = !narrow;

            if (!narrow)
            {
                row.Controls["Genre"].Location = new Point(x, row.Height / 2 - 12);
                row.Controls["Genre"].MaximumSize = new Size(Math.Max(40, genreW - 12), 0);
                x += genreW;

                row.Controls["Group"].Bounds = new Rectangle(x, row.Height / 2 - 13, Math.Min(92, Math.Max(70, groupW - 20)), 26);
                x += groupW;

                row.Controls["Avatar"].Bounds = new Rectangle(x, row.Height / 2 - 13, 26, 26);
                row.Controls["AddedBy"].Location = new Point(x + 34, row.Height / 2 - 10);
                row.Controls["AddedBy"].MaximumSize = new Size(Math.Max(50, addedW - 42), 0);
                x += addedW;
            }

            int actionStart = w - actionW;
            row.Controls["Edit"].Bounds = new Rectangle(actionStart + 28, (row.Height - 28) / 2, 28, 28);
            row.Controls["Archive"].Bounds = new Rectangle(actionStart + 72, (row.Height - 28) / 2, 28, 28);
        }

        private sealed class AddCategoryDialog : Form
        {
            private readonly Color cardBack;
            private readonly Color fieldBack;
            private readonly Color accent;
            private readonly Color accentDeep;
            private readonly Color primaryText;
            private readonly Color secondaryText;

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
                ClientSize = new Size(640, 720);
                FormBorderStyle = FormBorderStyle.None;
                StartPosition = FormStartPosition.CenterParent;
                BackColor = formBack;
                ShowInTaskbar = false;
                MaximizeBox = false;
                MinimizeBox = false;

                BuildDialog(categoryId, addedBy);
            }

            private void BuildDialog(string categoryId, string addedBy)
            {
                Panel card = new Panel
                {
                    BackColor = cardBack,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(40, 34, 40, 34)
                };
                card.Paint += RoundedPanelPaint;
                Controls.Add(card);

                Label title = CreateLabel("Add New Category", 22F, FontStyle.Bold, primaryText);
                Label subtitle = CreateLabel("Define metadata for library classification.", 10.5F, FontStyle.Regular, secondaryText);

                card.Controls.Add(title);
                card.Controls.Add(subtitle);

                int left = 40;
                int y = 36;
                int fieldWidth = 560;

                title.Location = new Point(left, y);
                y = title.Bottom + 6;

                subtitle.Location = new Point(left, y);
                y = subtitle.Bottom + 34;

                txtCategoryId.Text = categoryId;
                txtCategoryId.ReadOnly = true;
                AddField(card, "CATEGORY ID", txtCategoryId, "Auto-Generated", left, ref y, fieldWidth, 44);

                AddField(card, "CATEGORY NAME", txtCategoryName, "e.g., Science Fiction", left, ref y, fieldWidth, 44);
                AddField(card, "GENRE", txtGenre, "e.g., Academic, Fiction, Research", left, ref y, fieldWidth, 44);
                AddField(card, "GROUP", txtGroup, "e.g., Humanities, Sciences, Arts", left, ref y, fieldWidth, 44);
                AddField(card, "DESCRIPTION", txtDescription, "Briefly describe the contents and scope of this category...", left, ref y, fieldWidth, 104);

                Panel addedByPanel = new Panel
                {
                    BackColor = cardBack,
                    Bounds = new Rectangle(left, y + 6, fieldWidth, 34)
                };

                Label addedByText = CreateLabel("✓  Added By: " + addedBy + " (Current Session)", 9.5F, FontStyle.Bold, accentDeep);
                addedByText.Location = new Point(0, 8);
                addedByText.BackColor = cardBack;
                addedByPanel.Controls.Add(addedByText);
                card.Controls.Add(addedByPanel);

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Size = new Size(112, 44),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = cardBack,
                    ForeColor = primaryText,
                    Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                    Cursor = Cursors.Hand
                };
                btnCancel.FlatAppearance.BorderSize = 0;
                btnCancel.Click += (s, e) =>
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                };

                Button btnSave = new Button
                {
                    Text = "Save Category",
                    Size = new Size(188, 48),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = accent,
                    ForeColor = Color.FromArgb(0, 66, 51),
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnSave.FlatAppearance.BorderSize = 0;
                btnSave.Click += Save_Click;

                btnCancel.Location = new Point(ClientSize.Width - 40 - btnSave.Width - 20 - btnCancel.Width, ClientSize.Height - 86);
                btnSave.Location = new Point(ClientSize.Width - 40 - btnSave.Width, ClientSize.Height - 88);
                btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                card.Controls.Add(btnCancel);
                card.Controls.Add(btnSave);

                AcceptButton = btnSave;
                CancelButton = btnCancel;
            }

            private void AddField(Panel parent, string labelText, TextBox textBox, string placeholder, int left, ref int y, int width, int height)
            {
                Label label = CreateLabel(labelText, 9.5F, FontStyle.Bold, secondaryText);
                label.Location = new Point(left, y);
                parent.Controls.Add(label);

                y = label.Bottom + 8;

                Panel host = new Panel
                {
                    BackColor = fieldBack,
                    Bounds = new Rectangle(left, y, width, height)
                };
                host.Paint += RoundedPanelPaint;

                textBox.BorderStyle = BorderStyle.None;
                textBox.BackColor = fieldBack;
                textBox.ForeColor = primaryText;
                textBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                textBox.PlaceholderText = placeholder;
                textBox.Multiline = true;
                textBox.Location = new Point(20, height > 60 ? 16 : 13);
                textBox.Size = new Size(width - 40, height - (height > 60 ? 28 : 18));

                if (textBox.ReadOnly)
                    textBox.ForeColor = secondaryText;

                host.Controls.Add(textBox);
                parent.Controls.Add(host);

                y = host.Bottom + 26;
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
                    BackColor = Color.Transparent
                };
            }

            private void RoundedPanelPaint(object? sender, PaintEventArgs e)
            {
                if (sender is not Panel panel) return;
                if (panel.Width <= 1 || panel.Height <= 1) return;

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using SolidBrush brush = new SolidBrush(panel.BackColor);
                using System.Drawing.Drawing2D.GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 14);
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

        private Panel CreateCard(Color backColor)
        {
            Panel panel = new Panel { BackColor = backColor };
            panel.Paint += RoundedPanelPaint;
            return panel;
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
            if (sender is not Panel panel) return;
            if (panel.Width <= 1 || panel.Height <= 1) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(panel.BackColor);
            using System.Drawing.Drawing2D.GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 14);
            e.Graphics.FillPath(brush, path);

        }

        private void CirclePaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using SolidBrush brush = new SolidBrush(panel.BackColor);
            e.Graphics.FillEllipse(brush, 0, 0, panel.Width - 1, panel.Height - 1);
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
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