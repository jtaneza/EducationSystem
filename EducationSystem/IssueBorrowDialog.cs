using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class IssueBorrowDialog : Form
    {
        private readonly Color FormBack = Color.White;
        private readonly Color FooterBack = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color FieldBack = ColorTranslator.FromHtml("#E8EFF1");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");

        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Label lblMember = null!;
        private ComboBox cboMember = null!;
        private Label lblBook = null!;
        private ComboBox cboBook = null!;
        private Label lblIssueDate = null!;
        private DateTimePicker dtpIssueDate = null!;
        private Label lblDueDate = null!;
        private DateTimePicker dtpDueDate = null!;
        private Label lblStatus = null!;
        private ComboBox cboStatus = null!;
        private Panel footer = null!;
        private Button btnCancel = null!;
        private Button btnSave = null!;

        private int editingBorrowId = 0;
        private string originalBookId = "";

        private sealed class MemberOption
        {
            public int UserId { get; set; }
            public string FullName { get; set; } = "";
            public string Role { get; set; } = "";

            public override string ToString()
            {
                return $"{FullName} ({Role})";
            }
        }

        private sealed class BookOption
        {
            public string BookId { get; set; } = "";
            public string BookTitle { get; set; } = "";
            public string Author { get; set; } = "";
            public int Quantity { get; set; }

            public override string ToString()
            {
                return $"{BookTitle} - {Author} | Available: {Quantity}";
            }
        }

        public IssueBorrowDialog()
        {
            InitializeComponent();

            Text = "Issue New Book";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = FormBack;
            ClientSize = new Size(800, 650);

            BuildUI();

            Load += (s, e) =>
            {
                LoadMembers();
                LoadAvailableBooks();
            };
        }

        private void BuildUI()
        {
            lblTitle = new Label
            {
                Text = "Issue New Book",
                AutoSize = true,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = PrimaryText,
                Location = new Point(55, 55)
            };

            lblSubtitle = new Label
            {
                Text = "Initialize a new library circulation record.",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = SecondaryText,
                Location = new Point(58, 103)
            };

            lblMember = CreateFieldLabel("MEMBER DETAILS", 58, 155);
            cboMember = CreateComboBox(58, 188, 684);

            lblBook = CreateFieldLabel("BOOK SELECTION", 58, 250);
            cboBook = CreateComboBox(58, 283, 684);

            lblIssueDate = CreateFieldLabel("ISSUE DATE", 58, 350);
            dtpIssueDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10.5F),
                Location = new Point(58, 383),
                Size = new Size(270, 32),
                Value = DateTime.Today
            };

            lblDueDate = CreateFieldLabel("DUE DATE", 385, 350);
            dtpDueDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10.5F),
                Location = new Point(385, 383),
                Size = new Size(270, 32),
                Value = DateTime.Today.AddDays(7)
            };

            lblStatus = CreateFieldLabel("STATUS", 58, 450);
            cboStatus = CreateComboBox(58, 483, 300);
            cboStatus.Items.Add("ACTIVE");
            cboStatus.SelectedIndex = 0;
            cboStatus.Enabled = false;

            footer = new Panel
            {
                BackColor = FooterBack,
                Dock = DockStyle.Bottom,
                Height = 92
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                FlatStyle = FlatStyle.Flat,
                BackColor = FooterBack,
                ForeColor = PrimaryText,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Size = new Size(120, 44),
                Location = new Point(470, 24)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            btnSave = new Button
            {
                Text = "Issue Book",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Size = new Size(160, 44),
                Location = new Point(600, 24)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            footer.Controls.Add(btnCancel);
            footer.Controls.Add(btnSave);

            Controls.Add(lblTitle);
            Controls.Add(lblSubtitle);
            Controls.Add(lblMember);
            Controls.Add(cboMember);
            Controls.Add(lblBook);
            Controls.Add(cboBook);
            Controls.Add(lblIssueDate);
            Controls.Add(dtpIssueDate);
            Controls.Add(lblDueDate);
            Controls.Add(dtpDueDate);
            Controls.Add(lblStatus);
            Controls.Add(cboStatus);
            Controls.Add(footer);
        }

        private Label CreateFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = SecondaryText,
                Location = new Point(x, y)
            };
        }

        private ComboBox CreateComboBox(int x, int y, int width)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10.5F),
                BackColor = FieldBack,
                ForeColor = PrimaryText,
                Location = new Point(x, y),
                Size = new Size(width, 32)
            };
        }

        private int GetCurrentClientId()
        {
            if (int.TryParse(ClientSession.ClientId.ToString(), out int clientId))
                return clientId;

            return 0;
        }

        private void LoadMembers()
        {
            cboMember.Items.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureBorrowingSchema(conn);

                const string query = @"
SELECT UserID, FullName, Role
FROM dbo.Users
WHERE ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0
  AND ISNULL(IsActive, 1) = 1
  AND Role IN ('Member', 'Student', 'Teacher')
ORDER BY FullName ASC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cboMember.Items.Add(new MemberOption
                    {
                        UserId = Convert.ToInt32(reader["UserID"]),
                        FullName = Convert.ToString(reader["FullName"]) ?? "",
                        Role = Convert.ToString(reader["Role"]) ?? ""
                    });
                }

                if (cboMember.Items.Count > 0)
                    cboMember.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Members could not be loaded.\n\n" + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAvailableBooks()
        {
            cboBook.Items.Clear();

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureBorrowingSchema(conn);

                const string query = @"
SELECT BookID, BookTitle, Author, Quantity
FROM dbo.Books
WHERE ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0
  AND Quantity > 0
  AND Status IN ('In Stock', 'Low Stock')
ORDER BY BookTitle ASC;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cboBook.Items.Add(new BookOption
                    {
                        BookId = Convert.ToString(reader["BookID"]) ?? "",
                        BookTitle = Convert.ToString(reader["BookTitle"]) ?? "",
                        Author = Convert.ToString(reader["Author"]) ?? "",
                        Quantity = Convert.ToInt32(reader["Quantity"])
                    });
                }

                if (cboBook.Items.Count > 0)
                    cboBook.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Available books could not be loaded.\n\n" + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadForEdit(int borrowId)
        {
            editingBorrowId = borrowId;
            btnSave.Text = "Update Borrow";

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureBorrowingSchema(conn);

                const string query = @"
SELECT BorrowID, MemberID, BookID, BookTitle, IssueDate, DueDate, Status
FROM dbo.BorrowingRecords
WHERE BorrowID = @BorrowID
  AND ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BorrowID", borrowId);
                cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return;

                int memberId = Convert.ToInt32(reader["MemberID"]);
                originalBookId = Convert.ToString(reader["BookID"]) ?? "";

                dtpIssueDate.Value = Convert.ToDateTime(reader["IssueDate"]);
                dtpDueDate.Value = Convert.ToDateTime(reader["DueDate"]);

                cboStatus.SelectedItem = "ACTIVE";

                for (int i = 0; i < cboMember.Items.Count; i++)
                {
                    if (cboMember.Items[i] is MemberOption member && member.UserId == memberId)
                    {
                        cboMember.SelectedIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < cboBook.Items.Count; i++)
                {
                    if (cboBook.Items[i] is BookOption book && book.BookId == originalBookId)
                    {
                        cboBook.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Borrowing record could not be loaded.\n\n" + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (cboMember.SelectedItem is not MemberOption member)
            {
                MessageBox.Show("Please select a member.", "Missing Member",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboBook.SelectedItem is not BookOption book)
            {
                MessageBox.Show("Please select an available book.", "Missing Book",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpDueDate.Value.Date < dtpIssueDate.Value.Date)
            {
                MessageBox.Show("Due date cannot be earlier than issue date.", "Invalid Date",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureBorrowingSchema(conn);

                using SqlTransaction tx = conn.BeginTransaction();

                try
                {
                    if (editingBorrowId == 0)
                    {
                        InsertBorrow(conn, tx, member, book);
                        DecreaseBookQuantity(conn, tx, book.BookId);
                    }
                    else
                    {
                        UpdateBorrow(conn, tx, member, book);

                        if (!string.Equals(originalBookId, book.BookId, StringComparison.OrdinalIgnoreCase))
                        {
                            IncreaseBookQuantity(conn, tx, originalBookId);
                            DecreaseBookQuantity(conn, tx, book.BookId);
                        }
                    }

                    UpdateBookStockStatus(conn, tx, book.BookId);

                    if (!string.IsNullOrWhiteSpace(originalBookId) &&
                        !string.Equals(originalBookId, book.BookId, StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateBookStockStatus(conn, tx, originalBookId);
                    }

                    tx.Commit();

                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Borrowing record was not saved.\n\n" + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertBorrow(SqlConnection conn, SqlTransaction tx, MemberOption member, BookOption book)
        {
            const string query = @"
INSERT INTO dbo.BorrowingRecords
    (ClientID, MemberID, MemberName, BookID, BookTitle, IssueDate, DueDate, Status, CreatedAt, IsArchived)
VALUES
    (@ClientID, @MemberID, @MemberName, @BookID, @BookTitle, @IssueDate, @DueDate, @Status, SYSUTCDATETIME(), 0);";

            using SqlCommand cmd = new SqlCommand(query, conn, tx);
            AddBorrowParameters(cmd, member, book);
            cmd.ExecuteNonQuery();
        }

        private void UpdateBorrow(SqlConnection conn, SqlTransaction tx, MemberOption member, BookOption book)
        {
            const string query = @"
UPDATE dbo.BorrowingRecords
SET MemberID = @MemberID,
    MemberName = @MemberName,
    BookID = @BookID,
    BookTitle = @BookTitle,
    IssueDate = @IssueDate,
    DueDate = @DueDate,
    Status = @Status
WHERE BorrowID = @BorrowID
  AND ClientID = @ClientID;";

            using SqlCommand cmd = new SqlCommand(query, conn, tx);
            AddBorrowParameters(cmd, member, book);
            cmd.Parameters.AddWithValue("@BorrowID", editingBorrowId);
            cmd.ExecuteNonQuery();
        }

        private void AddBorrowParameters(SqlCommand cmd, MemberOption member, BookOption book)
        {
            cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());
            cmd.Parameters.AddWithValue("@MemberID", member.UserId);
            cmd.Parameters.AddWithValue("@MemberName", member.FullName);
            cmd.Parameters.AddWithValue("@BookID", book.BookId);
            cmd.Parameters.AddWithValue("@BookTitle", book.BookTitle);
            cmd.Parameters.AddWithValue("@IssueDate", dtpIssueDate.Value.Date);
            cmd.Parameters.AddWithValue("@DueDate", dtpDueDate.Value.Date);
            cmd.Parameters.AddWithValue("@Status", "ACTIVE");
        }

        private void DecreaseBookQuantity(SqlConnection conn, SqlTransaction tx, string bookId)
        {
            const string query = @"
UPDATE dbo.Books
SET Quantity = Quantity - 1,
    UpdatedAt = SYSUTCDATETIME()
WHERE BookID = @BookID
  AND ClientID = @ClientID
  AND Quantity > 0
  AND ISNULL(IsArchived, 0) = 0;";

            using SqlCommand cmd = new SqlCommand(query, conn, tx);
            cmd.Parameters.AddWithValue("@BookID", bookId);
            cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());

            int affected = cmd.ExecuteNonQuery();

            if (affected == 0)
                throw new InvalidOperationException("This book has no available stock for this school.");
        }

        private void IncreaseBookQuantity(SqlConnection conn, SqlTransaction tx, string bookId)
        {
            if (string.IsNullOrWhiteSpace(bookId))
                return;

            const string query = @"
UPDATE dbo.Books
SET Quantity = Quantity + 1,
    UpdatedAt = SYSUTCDATETIME()
WHERE BookID = @BookID
  AND ClientID = @ClientID
  AND ISNULL(IsArchived, 0) = 0;";

            using SqlCommand cmd = new SqlCommand(query, conn, tx);
            cmd.Parameters.AddWithValue("@BookID", bookId);
            cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());
            cmd.ExecuteNonQuery();
        }

        private void UpdateBookStockStatus(SqlConnection conn, SqlTransaction tx, string bookId)
        {
            const string query = @"
UPDATE dbo.Books
SET Status =
    CASE
        WHEN Quantity <= 0 THEN 'Out of Stock'
        WHEN Quantity <= 2 THEN 'Low Stock'
        ELSE 'In Stock'
    END,
    UpdatedAt = SYSUTCDATETIME()
WHERE BookID = @BookID
  AND ClientID = @ClientID;";

            using SqlCommand cmd = new SqlCommand(query, conn, tx);
            cmd.Parameters.AddWithValue("@BookID", bookId);
            cmd.Parameters.AddWithValue("@ClientID", GetCurrentClientId());
            cmd.ExecuteNonQuery();
        }

        private void EnsureBorrowingSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.BorrowingRecords', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BorrowingRecords
    (
        BorrowID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClientID INT NOT NULL,
        MemberID INT NOT NULL,
        MemberName NVARCHAR(150) NULL,
        BookID NVARCHAR(20) NULL,
        BookTitle NVARCHAR(250) NOT NULL,
        IssueDate DATE NOT NULL,
        DueDate DATE NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'ACTIVE',
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        IsArchived BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.BorrowingRecords', 'ClientID') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD ClientID INT NULL;

IF COL_LENGTH('dbo.BorrowingRecords', 'MemberID') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD MemberID INT NULL;

IF COL_LENGTH('dbo.BorrowingRecords', 'MemberName') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD MemberName NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.BorrowingRecords', 'BookID') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD BookID NVARCHAR(20) NULL;

IF COL_LENGTH('dbo.BorrowingRecords', 'BookTitle') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD BookTitle NVARCHAR(250) NULL;

IF COL_LENGTH('dbo.BorrowingRecords', 'IsArchived') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD IsArchived BIT NOT NULL DEFAULT 0;

IF COL_LENGTH('dbo.BorrowingRecords', 'CreatedAt') IS NULL
    ALTER TABLE dbo.BorrowingRecords ADD CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME();

IF COL_LENGTH('dbo.Books', 'ClientID') IS NULL
    ALTER TABLE dbo.Books ADD ClientID INT NULL;

IF COL_LENGTH('dbo.Users', 'ClientID') IS NULL
    ALTER TABLE dbo.Users ADD ClientID INT NULL;

IF COL_LENGTH('dbo.Users', 'IsActive') IS NULL
    ALTER TABLE dbo.Users ADD IsActive BIT NOT NULL DEFAULT 1;

IF COL_LENGTH('dbo.Users', 'IsArchived') IS NULL
    ALTER TABLE dbo.Users ADD IsArchived BIT NOT NULL DEFAULT 0;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }
    }
}