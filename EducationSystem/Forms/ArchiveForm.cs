using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class ArchiveForm : Form
    {
        private Label lblTitle = null!;
        private TextBox txtSearch = null!;
        private Button btnSearch = null!;
        private Button btnRefresh = null!;
        private DataGridView dgvArchiveHistory = null!;
        private Panel topPanel = null!;
        private Panel tableContainer = null!;

        public ArchiveForm()
        {
            InitializeComponent();
            BuildArchiveUI();
            LoadArchiveHistoryToGrid();
        }

        private void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(210, 210, 210);
            btn.UseVisualStyleBackColor = false;

            btn.BackColor = Color.Maroon;
            btn.ForeColor = Color.WhiteSmoke;

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(230, 230, 230);
                btn.ForeColor = Color.Black;
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.Maroon;
                btn.ForeColor = Color.WhiteSmoke;
            };
        }

        private void BuildArchiveUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            topPanel = new Panel();
            topPanel.BackColor = Color.Snow;
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 115;

            lblTitle = new Label();
            lblTitle.Text = "Archive History";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.Maroon;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(30, 15);

            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Size = new Size(250, 30);
            txtSearch.Location = new Point(30, 58);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.BackColor = Color.White;

            btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSearch.Size = new Size(90, 30);
            btnSearch.Location = new Point(290, 58);
            StyleButton(btnSearch);
            btnSearch.Click += BtnSearch_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Size = new Size(100, 32);
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StyleButton(btnRefresh);
            btnRefresh.Click += BtnRefresh_Click;

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);
            topPanel.Controls.Add(btnRefresh);

            dgvArchiveHistory = new DataGridView();
            dgvArchiveHistory.Dock = DockStyle.Fill;
            dgvArchiveHistory.BackgroundColor = Color.White;
            dgvArchiveHistory.BorderStyle = BorderStyle.None;
            dgvArchiveHistory.RowHeadersVisible = false;
            dgvArchiveHistory.AllowUserToAddRows = false;
            dgvArchiveHistory.AllowUserToResizeRows = false;
            dgvArchiveHistory.AllowUserToResizeColumns = false;
            dgvArchiveHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvArchiveHistory.MultiSelect = false;
            dgvArchiveHistory.ReadOnly = true;
            dgvArchiveHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvArchiveHistory.EnableHeadersVisualStyles = false;
            dgvArchiveHistory.ColumnHeadersHeight = 38;
            dgvArchiveHistory.RowTemplate.Height = 38;
            dgvArchiveHistory.GridColor = Color.Gainsboro;
            dgvArchiveHistory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvArchiveHistory.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvArchiveHistory.BackgroundColor = Color.WhiteSmoke;

            dgvArchiveHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvArchiveHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvArchiveHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvArchiveHistory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvArchiveHistory.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;
            dgvArchiveHistory.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            dgvArchiveHistory.DefaultCellStyle.BackColor = Color.White;
            dgvArchiveHistory.DefaultCellStyle.ForeColor = Color.Black;
            dgvArchiveHistory.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvArchiveHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvArchiveHistory.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvArchiveHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            dgvArchiveHistory.Columns.Add("ArchiveID", "Archive ID");
            dgvArchiveHistory.Columns.Add("Module", "Module");
            dgvArchiveHistory.Columns.Add("RecordID", "Record ID");
            dgvArchiveHistory.Columns.Add("ItemName", "Name / Title");
            dgvArchiveHistory.Columns.Add("ArchivedBy", "Archived By");
            dgvArchiveHistory.Columns.Add("ArchivedDate", "Archived Date");

            DataGridViewTextBoxColumn actionCol = new DataGridViewTextBoxColumn();
            actionCol.Name = "Actions";
            actionCol.HeaderText = "Actions";
            actionCol.Width = 170;
            actionCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            actionCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            actionCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvArchiveHistory.Columns.Add(actionCol);

            foreach (DataGridViewColumn col in dgvArchiveHistory.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dgvArchiveHistory.Columns["ArchiveID"].FillWeight = 14;
            dgvArchiveHistory.Columns["Module"].FillWeight = 14;
            dgvArchiveHistory.Columns["RecordID"].FillWeight = 14;
            dgvArchiveHistory.Columns["ItemName"].FillWeight = 26;
            dgvArchiveHistory.Columns["ArchivedBy"].FillWeight = 14;
            dgvArchiveHistory.Columns["ArchivedDate"].FillWeight = 18;
            dgvArchiveHistory.Columns["Actions"].FillWeight = 18;

            dgvArchiveHistory.CellClick += DgvArchiveHistory_CellClick;

            tableContainer = new Panel();
            tableContainer.Dock = DockStyle.Fill;
            tableContainer.Padding = new Padding(30, 0, 30, 20);
            tableContainer.BackColor = Color.Snow;
            tableContainer.Controls.Add(dgvArchiveHistory);

            Controls.Add(tableContainer);
            Controls.Add(topPanel);

            Resize += ArchiveForm_Resize;
            AdjustTopControls();
        }

        private void ArchiveForm_Resize(object? sender, EventArgs e)
        {
            AdjustTopControls();
        }

        private void AdjustTopControls()
        {
            if (btnRefresh == null) return;
            btnRefresh.Location = new Point(ClientSize.Width - btnRefresh.Width - 30, 57);
        }

        private void LoadArchiveHistoryToGrid()
        {
            dgvArchiveHistory.Rows.Clear();

            foreach (var item in ArchiveStore.ArchivedItems.OrderByDescending(x => x.ArchivedDate))
            {
                dgvArchiveHistory.Rows.Add(
                    item.ArchiveID,
                    item.Module,
                    item.RecordID,
                    item.ItemName,
                    item.ArchivedBy,
                    item.ArchivedDate.ToString("MM/dd/yyyy hh:mm tt"),
                    "↩ Restore    ✖ Delete"
                );
            }

            dgvArchiveHistory.ClearSelection();
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            dgvArchiveHistory.Rows.Clear();

            var filtered = ArchiveStore.ArchivedItems
                .Where(a =>
                    a.ArchiveID.ToLower().Contains(keyword) ||
                    a.Module.ToLower().Contains(keyword) ||
                    a.RecordID.ToLower().Contains(keyword) ||
                    a.ItemName.ToLower().Contains(keyword) ||
                    a.ArchivedBy.ToLower().Contains(keyword))
                .OrderByDescending(a => a.ArchivedDate)
                .ToList();

            foreach (var item in filtered)
            {
                dgvArchiveHistory.Rows.Add(
                    item.ArchiveID,
                    item.Module,
                    item.RecordID,
                    item.ItemName,
                    item.ArchivedBy,
                    item.ArchivedDate.ToString("MM/dd/yyyy hh:mm tt"),
                    "↩ Restore    ✖ Delete"
                );
            }

            dgvArchiveHistory.ClearSelection();
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadArchiveHistoryToGrid();
        }

        private void RestoreArchiveItem(string archiveId)
        {
            var item = ArchiveStore.ArchivedItems.FirstOrDefault(a => a.ArchiveID == archiveId);
            if (item == null) return;

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to restore '{item.ItemName}' from module '{item.Module}'?",
                "Restore Item",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            if (item.Module == "Clients")
            {
                var client = ClientArchiveStore.ArchivedClients.FirstOrDefault(c => c.ClientID == item.RecordID);
                if (client != null)
                {
                    client.Status = "Active";
                    ClientArchiveStore.ActiveClients.Add(client);
                    ClientArchiveStore.ArchivedClients.Remove(client);
                }
            }

            ArchiveStore.ArchivedItems.Remove(item);
            LoadArchiveHistoryToGrid();
        }

        private void DeleteArchiveItemPermanently(string archiveId)
        {
            var item = ArchiveStore.ArchivedItems.FirstOrDefault(a => a.ArchiveID == archiveId);
            if (item == null) return;

            DialogResult result = MessageBox.Show(
                $"This will permanently delete '{item.ItemName}' from archive history. Continue?",
                "Delete Permanently",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes) return;

            if (item.Module == "Clients")
            {
                var client = ClientArchiveStore.ArchivedClients.FirstOrDefault(c => c.ClientID == item.RecordID);
                if (client != null)
                {
                    ClientArchiveStore.ArchivedClients.Remove(client);
                }
            }

            ArchiveStore.ArchivedItems.Remove(item);
            LoadArchiveHistoryToGrid();
        }

        private void DgvArchiveHistory_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvArchiveHistory.Columns[e.ColumnIndex].Name == "Actions")
            {
                Rectangle cellRect = dgvArchiveHistory.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                int clickX = dgvArchiveHistory.PointToClient(Cursor.Position).X - cellRect.Left;
                string archiveId = dgvArchiveHistory.Rows[e.RowIndex].Cells["ArchiveID"].Value?.ToString() ?? "";

                if (clickX < cellRect.Width / 2)
                    RestoreArchiveItem(archiveId);
                else
                    DeleteArchiveItemPermanently(archiveId);
            }
        }
    }
}