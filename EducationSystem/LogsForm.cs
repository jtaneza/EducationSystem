using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class LogsForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F8FAFC");
        private readonly Color CardBack = Color.White;
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#F1F5F9");
        private readonly Color Outline = ColorTranslator.FromHtml("#CBD5E1");

        private readonly Color Primary = ColorTranslator.FromHtml("#00B894");
        private readonly Color TextMain = ColorTranslator.FromHtml("#1E293B");
        private readonly Color TextMuted = ColorTranslator.FromHtml("#64748B");
        private readonly Color Error = ColorTranslator.FromHtml("#DC2626");
        private readonly Color ErrorSoft = ColorTranslator.FromHtml("#FDE2E1");

        private Panel headerPanel = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Panel viewSwitchPanel = null!;
        private Button btnTableView = null!;
        private Button btnVisualizer = null!;

        private Panel metricsPanel = null!;
        private Panel cardEvents = null!;
        private Panel cardLogins = null!;
        private Panel cardAlerts = null!;
        private Label lblEventsValue = null!;
        private Label lblLoginsValue = null!;
        private Label lblAlertsValue = null!;

        private Panel logsCard = null!;
        private Panel controlsPanel = null!;
        private ComboBox cmbActivityType = null!;
        private DateTimePicker dtpLogDate = null!;
        private Button btnExportLogs = null!;

        private DataGridView dgvLogs = null!;
        private Panel paginationPanel = null!;
        private Label lblPagingInfo = null!;
        private Button btnPrev = null!;
        private Button btnPage1 = null!;
        private Button btnPage2 = null!;
        private Button btnPage3 = null!;
        private Button btnNext = null!;

        public LogsForm()
        {
            InitializeComponent();
            BuildUI();
            LoadLogData();

            Padding = new Padding(0, 0, 0, 24);
            Resize += LogsForm_Resize;
            AdjustLayout();
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = true;

            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Background
            };

            lblTitle = new Label
            {
                Text = "System Logs",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = TextMain,
                AutoSize = true,
                Location = new Point(34, 30)
            };

            lblSubtitle = new Label
            {
                Text = "View a simple history of all activity across the library system.",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(36, 78)
            };

            viewSwitchPanel = new Panel
            {
                Size = new Size(245, 42),
                BackColor = SurfaceLow
            };

            btnTableView = new Button
            {
                Text = "Table View",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(114, 32),
                BackColor = CardBack,
                ForeColor = Primary,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTableView.FlatAppearance.BorderSize = 0;

            btnVisualizer = new Button
            {
                Text = "Visualizer",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(114, 32),
                BackColor = SurfaceLow,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnVisualizer.FlatAppearance.BorderSize = 0;

            viewSwitchPanel.Controls.Add(btnTableView);
            viewSwitchPanel.Controls.Add(btnVisualizer);

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(viewSwitchPanel);

            metricsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 198,
                BackColor = Background
            };

            cardEvents = CreateMetricCard("⚡", Primary, "+12%", "TOTAL EVENTS TODAY", "1,284", out lblEventsValue, false);
            cardLogins = CreateMetricCard("👤", Primary, "Normal", "NEW LOGINS", "42", out lblLoginsValue, false);
            cardAlerts = CreateMetricCard("⚠", Error, "2 Critical", "SYSTEM ALERTS", "07", out lblAlertsValue, true);

            metricsPanel.Controls.Add(cardEvents);
            metricsPanel.Controls.Add(cardLogins);
            metricsPanel.Controls.Add(cardAlerts);

            logsCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBack,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(0),
                Margin = new Padding(0, 0, 0, 20)
            };
            logsCard.Paint += DrawSoftBorder;

            controlsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 108,
                BackColor = CardBack
            };

            cmbActivityType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = SurfaceLow,
                ForeColor = TextMain,
                FlatStyle = FlatStyle.Flat,
                Width = 155,
                Height = 36
            };
            cmbActivityType.Items.AddRange(new object[]
            {
                "All Activities",
                "Inventory",
                "Finance",
                "Circulation",
                "Members",
                "Security"
            });
            cmbActivityType.SelectedIndex = 0;
            cmbActivityType.SelectedIndexChanged += FilterLogsChanged;

            dtpLogDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Width = 176,
                Height = 36,
                CalendarMonthBackground = CardBack
            };
            dtpLogDate.ValueChanged += FilterLogsChanged;

            btnExportLogs = new Button
            {
                Text = "↓  Export Logs",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(172, 50),
                BackColor = Primary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportLogs.FlatAppearance.BorderSize = 0;
            btnExportLogs.Click += BtnExportLogs_Click;

            controlsPanel.Controls.Add(cmbActivityType);
            controlsPanel.Controls.Add(dtpLogDate);
            controlsPanel.Controls.Add(btnExportLogs);

            dgvLogs = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = CardBack,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 56,
                GridColor = Color.FromArgb(245, 247, 250),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ScrollBars = ScrollBars.Vertical
            };
            dgvLogs.RowTemplate.Height = 72;
            dgvLogs.DefaultCellStyle.Padding = new Padding(10, 8, 10, 8);
            dgvLogs.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 10, 10, 10);

            dgvLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(148, 163, 184);
            dgvLogs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvLogs.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
            dgvLogs.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(148, 163, 184);

            dgvLogs.DefaultCellStyle.BackColor = CardBack;
            dgvLogs.DefaultCellStyle.ForeColor = TextMain;
            dgvLogs.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvLogs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
            dgvLogs.DefaultCellStyle.SelectionForeColor = TextMain;
            dgvLogs.AlternatingRowsDefaultCellStyle.BackColor = CardBack;

            dgvLogs.Columns.Add("Time", "TIME");
            dgvLogs.Columns.Add("WhoDidIt", "WHO DID IT");
            dgvLogs.Columns.Add("WhatHappened", "WHAT HAPPENED");
            dgvLogs.Columns.Add("Details", "DETAILS");
            dgvLogs.Columns.Add("Status", "STATUS");
            dgvLogs.Columns.Add("Actions", "ACTIONS");

            dgvLogs.Columns["Time"].FillWeight = 17;
            dgvLogs.Columns["WhoDidIt"].FillWeight = 22;
            dgvLogs.Columns["WhatHappened"].FillWeight = 20;
            dgvLogs.Columns["Details"].FillWeight = 30;
            dgvLogs.Columns["Status"].FillWeight = 15;
            dgvLogs.Columns["Actions"].FillWeight = 10;

            foreach (DataGridViewColumn col in dgvLogs.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvLogs.CellPainting += DgvLogs_CellPainting;
            dgvLogs.CellClick += DgvLogs_CellClick;

            paginationPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 84,
                BackColor = Color.FromArgb(250, 251, 252)
            };

            lblPagingInfo = new Label
            {
                Text = "Showing 5 of 1,284 entries",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TextMuted,
                AutoSize = true
            };

            btnPrev = CreatePagerButton("Previous", false, 86);
            btnPage1 = CreatePagerButton("1", true, 36);
            btnPage2 = CreatePagerButton("2", false, 36);
            btnPage3 = CreatePagerButton("3", false, 36);
            btnNext = CreatePagerButton("Next", false, 70);

            paginationPanel.Controls.Add(lblPagingInfo);
            paginationPanel.Controls.Add(btnPrev);
            paginationPanel.Controls.Add(btnPage1);
            paginationPanel.Controls.Add(btnPage2);
            paginationPanel.Controls.Add(btnPage3);
            paginationPanel.Controls.Add(btnNext);

            logsCard.Controls.Add(dgvLogs);
            logsCard.Controls.Add(paginationPanel);
            logsCard.Controls.Add(controlsPanel);

            Controls.Add(logsCard);
            Controls.Add(metricsPanel);
            Controls.Add(headerPanel);
        }

        private Panel CreateMetricCard(string iconText, Color accentColor, string tagText, string title, string value, out Label valueLabel, bool isAlert)
        {
            Panel card = new Panel
            {
                BackColor = CardBack,
                BorderStyle = BorderStyle.None
            };
            card.Paint += DrawSoftBorder;

            Panel iconPanel = new Panel
            {
                Size = new Size(48, 48),
                BackColor = isAlert ? ErrorSoft : Color.FromArgb(230, 248, 244)
            };

            Label iconLabel = new Label
            {
                Text = iconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 16F, FontStyle.Bold),
                ForeColor = accentColor
            };
            iconPanel.Controls.Add(iconLabel);

            Label tag = new Label
            {
                Text = tagText,
                AutoSize = true,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = isAlert ? ErrorSoft : Color.FromArgb(230, 248, 244),
                ForeColor = accentColor,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };

            Label lblTitleSmall = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184)
            };

            valueLabel = new Label
            {
                Text = value,
                AutoSize = true,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                ForeColor = TextMain
            };

            card.Controls.Add(iconPanel);
            card.Controls.Add(tag);
            card.Controls.Add(lblTitleSmall);
            card.Controls.Add(valueLabel);

            iconPanel.Location = new Point(28, 22);
            tag.Location = new Point(182, 28);
            lblTitleSmall.Location = new Point(28, 88);
            valueLabel.Location = new Point(28, 104);

            return card;
        }

        private Button CreatePagerButton(string text, bool active, int width)
        {
            Button btn = new Button
            {
                Text = text,
                Width = width,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, active ? FontStyle.Bold : FontStyle.Regular),
                BackColor = active ? Primary : CardBack,
                ForeColor = active ? Color.White : TextMuted
            };
            btn.FlatAppearance.BorderSize = active ? 0 : 1;
            btn.FlatAppearance.BorderColor = Outline;
            return btn;
        }

        private void LoadLogData()
        {
            dgvLogs.Rows.Clear();

            dgvLogs.Rows.Add("14:20 PM | OCT 24, 2023", "AS|Astra", "Inventory Restock", "'The Great Gatsby' (Hardcover) - Stock Updated", "Reviewed", "View");
            dgvLogs.Rows.Add("13:55 PM | OCT 24, 2023", "AS|Astra", "Fine Transaction", "User ID: 0025 Fine Clear - $15.00 payment received", "Reviewed", "View");
            dgvLogs.Rows.Add("13:12 PM | OCT 24, 2023", "AS|Astra", "Book Return", "'Advanced Physics' Vol II - Returned by Member #1092", "Reviewed", "View");
            dgvLogs.Rows.Add("11:45 AM | OCT 24, 2023", "AS|Astra", "Member Update", "Student Registry #442 - Contact information updated", "Reviewed", "View");
            dgvLogs.Rows.Add("09:15 AM | OCT 24, 2023", "SA|System Auto", "Security Alert", "Multiple failed login attempts from IP 10.0.0.82", "Warning", "View");

            lblPagingInfo.Text = "Showing 5 of 1,284 entries";
        }

        private void FilterLogsChanged(object? sender, EventArgs e)
        {
            LoadLogData();
        }

        private void BtnExportLogs_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Logs exported successfully.", "Export Logs", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DgvLogs_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvLogs.Columns[e.ColumnIndex].Name == "Actions")
            {
                string action = dgvLogs.Rows[e.RowIndex].Cells["WhatHappened"].Value?.ToString() ?? "Log";
                MessageBox.Show($"Viewing details for: {action}", "Log Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DgvLogs_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvLogs.Columns[e.ColumnIndex].Name;

            if (columnName == "WhoDidIt")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string initials = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : raw;

                bool systemAuto = name == "System Auto";

                Rectangle avatar = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + (e.CellBounds.Height - 32) / 2, 32, 32);

                using (SolidBrush brush = new SolidBrush(systemAuto ? Color.FromArgb(241, 245, 249) : Color.FromArgb(230, 248, 244)))
                    e.Graphics.FillRectangle(brush, avatar);

                TextRenderer.DrawText(
                    e.Graphics,
                    initials,
                    new Font("Segoe UI", 8F, FontStyle.Bold),
                    avatar,
                    systemAuto ? TextMuted : Primary,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                Rectangle nameRect = new Rectangle(avatar.Right + 10, e.CellBounds.Y, e.CellBounds.Width - avatar.Width - 20, e.CellBounds.Height);
                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    nameRect,
                    TextMain,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (columnName == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                bool warning = text.Equals("Warning", StringComparison.OrdinalIgnoreCase);

                Color back = warning ? ErrorSoft : Color.FromArgb(230, 248, 244);
                Color fore = warning ? Error : Primary;

                Size textSize = TextRenderer.MeasureText(text.ToUpper(), new Font("Segoe UI", 8F, FontStyle.Bold));
                Rectangle badgeRect = new Rectangle(
                    e.CellBounds.X + 8,
                    e.CellBounds.Y + (e.CellBounds.Height - 28) / 2,
                    textSize.Width + 20,
                    28
                );

                using (SolidBrush brush = new SolidBrush(back))
                    e.Graphics.FillRectangle(brush, badgeRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    text.ToUpper(),
                    new Font("Segoe UI", 8F, FontStyle.Bold),
                    badgeRect,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (columnName == "Actions")
            {
                e.PaintBackground(e.CellBounds, true);

                Rectangle iconRect = new Rectangle(
                    e.CellBounds.X + (e.CellBounds.Width - 24) / 2,
                    e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                    24,
                    24
                );

                TextRenderer.DrawText(
                    e.Graphics,
                    "👁",
                    new Font("Segoe UI Emoji", 12F),
                    iconRect,
                    TextMuted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
        }

        private void DrawSoftBorder(object? sender, PaintEventArgs e)
        {
            if (sender is not Control c) return;
            using Pen pen = new Pen(Color.FromArgb(230, 235, 241), 1);
            e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
        }

        private void LogsForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int left = 34;
            int gap = 24;
            int width = ClientSize.Width - 68;

            viewSwitchPanel.Location = new Point(ClientSize.Width - viewSwitchPanel.Width - 40, 74);
            btnTableView.Location = new Point(5, 5);
            btnVisualizer.Location = new Point(126, 5);

            if (width >= 1100)
            {
                metricsPanel.Height = 198;
                int cardW = (width - (gap * 2)) / 3;

                cardEvents.Bounds = new Rectangle(left, 18, cardW, 156);
                cardLogins.Bounds = new Rectangle(cardEvents.Right + gap, 18, cardW, 156);
                cardAlerts.Bounds = new Rectangle(cardLogins.Right + gap, 18, cardW, 156);
            }
            else
            {
                metricsPanel.Height = 366;
                int cardW = (width - gap) / 2;

                cardEvents.Bounds = new Rectangle(left, 18, cardW, 156);
                cardLogins.Bounds = new Rectangle(cardEvents.Right + gap, 18, cardW, 156);
                cardAlerts.Bounds = new Rectangle(left, cardEvents.Bottom + gap, cardW, 156);
            }

            logsCard.Padding = new Padding(0, 0, 0, 12);

            controlsPanel.Height = 108;
            cmbActivityType.Location = new Point(30, 32);
            dtpLogDate.Location = new Point(cmbActivityType.Right + 16, 32);
            btnExportLogs.Location = new Point(logsCard.Width - btnExportLogs.Width - 30, 29);

            paginationPanel.Height = 84;
            lblPagingInfo.Location = new Point(30, 30);

            btnNext.Location = new Point(paginationPanel.Width - btnNext.Width - 24, 24);
            btnPage3.Location = new Point(btnNext.Left - btnPage3.Width - 8, 24);
            btnPage2.Location = new Point(btnPage3.Left - btnPage2.Width - 8, 24);
            btnPage1.Location = new Point(btnPage2.Left - btnPage1.Width - 8, 24);
            btnPrev.Location = new Point(btnPage1.Left - btnPrev.Width - 8, 24);
        }
    }
}