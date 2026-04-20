using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class MonitoringForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color SurfaceLowest = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#E2E9EC");
        private readonly Color SurfaceVariant = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color PrimaryFixed = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Tertiary = ColorTranslator.FromHtml("#A03F30");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");

        private readonly Color InverseSurface = ColorTranslator.FromHtml("#2B3234");
        private readonly Color InverseOnSurface = ColorTranslator.FromHtml("#EBF2F4");

        private Panel canvasPanel = null!;

        private Label lblPageTitle = null!;
        private Label lblPageSub = null!;
        private Button btnFilterLog = null!;

        private Panel activityPanel = null!;
        private DataGridView dgvActivity = null!;

        private Label lblVitalsTitle = null!;
        private Label lblVitalsSub = null!;

        private Panel cardUsers = null!;
        private Panel cardStock = null!;
        private Panel cardFees = null!;

        private Label lblUsersValue = null!;
        private Label lblUsersTag = null!;
        private Label lblStockValue = null!;
        private Label lblFeesValue = null!;

        private Panel usersBars = null!;
        private Panel stockProgress = null!;

        private Panel reportPanel = null!;
        private Panel reportImage = null!;
        private Label lblReportTitle = null!;
        private Label lblReportBody = null!;
        private Button btnDeepDive = null!;

        private Panel maintenancePanel = null!;
        private Label lblMaintenanceTitle = null!;
        private Label lblMaintenanceBody = null!;
        private Label lblMaintenanceChip1 = null!;
        private Label lblMaintenanceChip2 = null!;
        private Label lblMaintenanceIcon = null!;

        public MonitoringForm()
        {
            InitializeComponent();
            BuildMonitoringUI();
            LoadMonitoringData();
        }

        private void BuildMonitoringUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = true;

            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background
            };
            Controls.Add(canvasPanel);

            lblPageTitle = new Label
            {
                Text = "Activity Feed",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblPageSub = new Label
            {
                Text = "Real-time book transactions across all connected libraries",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            btnFilterLog = new Button
            {
                Text = "☰  Filter Log",
                FlatStyle = FlatStyle.Flat,
                BackColor = SurfaceHigh,
                ForeColor = Primary,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFilterLog.FlatAppearance.BorderSize = 0;

            activityPanel = CreateCardPanel();
            dgvActivity = new DataGridView
            {
                BackgroundColor = SurfaceLowest,
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
                ColumnHeadersHeight = 48,
                GridColor = SurfaceVariant,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                Dock = DockStyle.Fill
            };
            dgvActivity.RowTemplate.Height = 62;

            dgvActivity.ColumnHeadersDefaultCellStyle.BackColor = SurfaceLow;
            dgvActivity.ColumnHeadersDefaultCellStyle.ForeColor = OnSurfaceVariant;
            dgvActivity.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvActivity.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceLow;
            dgvActivity.ColumnHeadersDefaultCellStyle.SelectionForeColor = OnSurfaceVariant;

            dgvActivity.DefaultCellStyle.BackColor = SurfaceLowest;
            dgvActivity.DefaultCellStyle.ForeColor = OnSurface;
            dgvActivity.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvActivity.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 250, 247);
            dgvActivity.DefaultCellStyle.SelectionForeColor = OnSurface;

            dgvActivity.Columns.Add("Time", "TIME");
            dgvActivity.Columns.Add("Member", "MEMBER");
            dgvActivity.Columns.Add("BookTitle", "BOOK TITLE");
            dgvActivity.Columns.Add("Activity", "ACTIVITY");
            dgvActivity.Columns.Add("Status", "STATUS");

            dgvActivity.Columns["Time"].FillWeight = 14;
            dgvActivity.Columns["Member"].FillWeight = 24;
            dgvActivity.Columns["BookTitle"].FillWeight = 28;
            dgvActivity.Columns["Activity"].FillWeight = 18;
            dgvActivity.Columns["Status"].FillWeight = 16;

            foreach (DataGridViewColumn col in dgvActivity.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvActivity.CellPainting += dgvActivity_CellPainting;
            activityPanel.Controls.Add(dgvActivity);

            lblVitalsTitle = new Label
            {
                Text = "System Vitals",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblVitalsSub = new Label
            {
                Text = "Key performance indicators for the current session",
                Font = new Font("Segoe UI", 11F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            cardUsers = CreateCardPanel();
            cardStock = CreateCardPanel();
            cardFees = new Panel
            {
                BackColor = InverseSurface,
                BorderStyle = BorderStyle.FixedSingle
            };

            BuildUsersCard();
            BuildStockCard();
            BuildFeesCard();
            BuildBottomPanels();

            canvasPanel.Controls.Add(lblPageTitle);
            canvasPanel.Controls.Add(lblPageSub);
            canvasPanel.Controls.Add(btnFilterLog);
            canvasPanel.Controls.Add(activityPanel);
            canvasPanel.Controls.Add(lblVitalsTitle);
            canvasPanel.Controls.Add(lblVitalsSub);
            canvasPanel.Controls.Add(cardUsers);
            canvasPanel.Controls.Add(cardStock);
            canvasPanel.Controls.Add(cardFees);
            canvasPanel.Controls.Add(reportPanel);
            canvasPanel.Controls.Add(maintenancePanel);

            Resize += MonitoringForm_Resize;
            AdjustLayout();
        }

        private void BuildUsersCard()
        {
            Panel icon = CreateIconPanel("👥", Color.FromArgb(235, 250, 245), Primary);
            icon.Location = new Point(24, 26);

            lblUsersTag = new Label
            {
                Text = "+12% TODAY",
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 140, 100),
                BackColor = Color.FromArgb(235, 250, 245),
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(205, 28)
            };

            Label title = new Label
            {
                Text = "HOW MANY USERS",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                Location = new Point(24, 108)
            };

            lblUsersValue = new Label
            {
                Text = "1,284",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                Location = new Point(24, 128)
            };

            usersBars = new Panel
            {
                BackColor = SurfaceLowest,
                Location = new Point(24, 196),
                Size = new Size(260, 14)
            };

            cardUsers.Controls.Add(icon);
            cardUsers.Controls.Add(lblUsersTag);
            cardUsers.Controls.Add(title);
            cardUsers.Controls.Add(lblUsersValue);
            cardUsers.Controls.Add(usersBars);
        }

        private void BuildStockCard()
        {
            Panel icon = CreateIconPanel("🗂", Color.FromArgb(235, 250, 245), Primary);
            icon.Location = new Point(24, 26);

            Label title = new Label
            {
                Text = "BOOK STOCK",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                Location = new Point(24, 108)
            };

            lblStockValue = new Label
            {
                Text = "68%",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                Location = new Point(24, 126)
            };

            Label lblSuffix = new Label
            {
                Text = "Checked out",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = OnSurface,
                Location = new Point(122, 145)
            };

            stockProgress = new Panel
            {
                BackColor = SurfaceHigh,
                Location = new Point(24, 202),
                Size = new Size(270, 12)
            };

            cardStock.Controls.Add(icon);
            cardStock.Controls.Add(title);
            cardStock.Controls.Add(lblStockValue);
            cardStock.Controls.Add(lblSuffix);
            cardStock.Controls.Add(stockProgress);
        }

        private void BuildFeesCard()
        {
            Panel glow = new Panel
            {
                BackColor = Color.FromArgb(25, 0, 184, 148),
                Size = new Size(120, 120),
                Location = new Point(220, 130)
            };
            glow.Paint += (s, e) =>
            {
                using SolidBrush b = new SolidBrush(Color.FromArgb(22, 0, 184, 148));
                e.Graphics.FillEllipse(b, 0, 0, glow.Width, glow.Height);
            };

            Panel icon = CreateIconPanel("💵", Color.FromArgb(24, 0, 184, 148), PrimaryFixed);
            icon.Location = new Point(24, 26);

            Label lblLive = new Label
            {
                Text = "↗ Live",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = PrimaryFixed,
                Location = new Point(250, 34)
            };

            Label title = new Label
            {
                Text = "FEES CALCULATED TODAY",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(135, 150, 160),
                Location = new Point(24, 110)
            };

            lblFeesValue = new Label
            {
                Text = "$4,290.50",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryFixed,
                Location = new Point(24, 132)
            };

            Label footer = new Label
            {
                Text = "UPDATED 2 MINS AGO  ⟳",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 130, 140),
                Location = new Point(24, 204)
            };

            cardFees.Controls.Add(glow);
            cardFees.Controls.Add(icon);
            cardFees.Controls.Add(lblLive);
            cardFees.Controls.Add(title);
            cardFees.Controls.Add(lblFeesValue);
            cardFees.Controls.Add(footer);
            glow.SendToBack();
        }

        private void BuildBottomPanels()
        {
            reportPanel = new Panel
            {
                BackColor = SurfaceLow,
                BorderStyle = BorderStyle.FixedSingle
            };

            reportImage = new Panel
            {
                BackColor = InverseSurface,
                BorderStyle = BorderStyle.FixedSingle
            };
            reportImage.Paint += (s, e) =>
            {
                using SolidBrush b = new SolidBrush(Color.FromArgb(12, 0, 184, 148));
                e.Graphics.FillRectangle(b, 0, 0, reportImage.Width, reportImage.Height);

                using Pen p = new Pen(Color.FromArgb(40, 109, 250, 210), 2);
                e.Graphics.DrawRectangle(p, 20, 20, reportImage.Width - 40, reportImage.Height - 40);
                e.Graphics.DrawLine(p, 20, reportImage.Height - 30, reportImage.Width - 20, reportImage.Height - 30);
                e.Graphics.DrawLine(p, 45, 30, 45, reportImage.Height - 30);
            };

            lblReportTitle = new Label
            {
                Text = "Weekly Intelligence\nReport",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = OnSurface
            };

            lblReportBody = new Label
            {
                Text = "System performance is currently\noptimal with a 99.9% uptime\nrecord over the last 7 days.",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = OnSurfaceVariant
            };

            btnDeepDive = new Button
            {
                Text = "VIEW DEEP DIVE",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Primary,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDeepDive.FlatAppearance.BorderSize = 0;

            reportPanel.Controls.Add(reportImage);
            reportPanel.Controls.Add(lblReportTitle);
            reportPanel.Controls.Add(lblReportBody);
            reportPanel.Controls.Add(btnDeepDive);

            maintenancePanel = new Panel
            {
                BackColor = PrimaryContainer,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblMaintenanceTitle = new Label
            {
                Text = "Automated Maintenance",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#004233")
            };

            lblMaintenanceBody = new Label
            {
                Text = "Next scheduled indexing is tonight at 02:00 AM. No\nadministrator action required.",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = ColorTranslator.FromHtml("#004233")
            };

            lblMaintenanceChip1 = CreateChip("DATABASE");
            lblMaintenanceChip2 = CreateChip("OPTIMIZATION");

            lblMaintenanceIcon = new Label
            {
                Text = "✦",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 60F),
                ForeColor = Color.FromArgb(20, 0, 66, 51)
            };

            maintenancePanel.Controls.Add(lblMaintenanceTitle);
            maintenancePanel.Controls.Add(lblMaintenanceBody);
            maintenancePanel.Controls.Add(lblMaintenanceChip1);
            maintenancePanel.Controls.Add(lblMaintenanceChip2);
            maintenancePanel.Controls.Add(lblMaintenanceIcon);
        }

        private Panel CreateCardPanel()
        {
            return new Panel
            {
                BackColor = SurfaceLowest,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Panel CreateIconPanel(string text, Color back, Color fore)
        {
            Panel panel = new Panel
            {
                Size = new Size(52, 52),
                BackColor = back
            };

            Label label = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 18F),
                ForeColor = fore
            };

            panel.Controls.Add(label);
            return panel;
        }

        private Label CreateChip(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#004233"),
                BackColor = Color.FromArgb(35, 0, 66, 51),
                Padding = new Padding(10, 5, 10, 5)
            };
        }

        private void LoadMonitoringData()
        {
            dgvActivity.Rows.Clear();
            dgvActivity.Rows.Add("10:42 AM", "JS|Jordan Smith", "The Great Gatsby", "Borrowed", "Success");
            dgvActivity.Rows.Add("10:38 AM", "ML|Maria Lopez", "Introduction to Algorithms", "Returned", "Processed");
            dgvActivity.Rows.Add("10:31 AM", "AR|Alex Reed", "Dune: Part One", "Renewed", "Success");

            BuildUsersMeter();
            BuildStockMeter();
        }

        private void BuildUsersMeter()
        {
            usersBars.Controls.Clear();

            int gap = 6;
            int y = 4;
            int h = 4;
            int[] widths = { 60, 60, 60, 60 };

            for (int i = 0; i < widths.Length; i++)
            {
                Panel bar = new Panel
                {
                    Height = h,
                    Width = widths[i],
                    Left = i * (widths[i] + gap),
                    Top = y,
                    BackColor = i < 3 ? Primary : Color.FromArgb(225, 230, 232)
                };
                usersBars.Controls.Add(bar);
            }
        }

        private void BuildStockMeter()
        {
            stockProgress.Controls.Clear();

            Panel fill = new Panel
            {
                BackColor = PrimaryContainer,
                Width = (int)(stockProgress.Width * 0.68),
                Height = stockProgress.Height,
                Left = 0,
                Top = 0
            };
            stockProgress.Controls.Add(fill);
        }

        private void dgvActivity_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvActivity.Columns[e.ColumnIndex].Name;

            if (col == "Member")
            {
                e.PaintBackground(e.CellBounds, true);

                string raw = e.FormattedValue?.ToString() ?? "";
                string[] parts = raw.Split('|');
                string initials = parts.Length > 0 ? parts[0] : "";
                string name = parts.Length > 1 ? parts[1] : raw;

                Color bubbleColor = initials switch
                {
                    "JS" => SecondaryContainer,
                    "ML" => Color.FromArgb(210, 245, 232),
                    _ => Color.FromArgb(250, 225, 220)
                };

                Color textColor = initials switch
                {
                    "AR" => Tertiary,
                    _ => Primary
                };

                Rectangle bubble = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 13, 34, 34);

                using (SolidBrush brush = new SolidBrush(bubbleColor))
                    e.Graphics.FillEllipse(brush, bubble);

                TextRenderer.DrawText(
                    e.Graphics,
                    initials,
                    new Font("Segoe UI", 9F, FontStyle.Bold),
                    bubble,
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                Rectangle nameRect = new Rectangle(bubble.Right + 12, e.CellBounds.Y, e.CellBounds.Width - 56, e.CellBounds.Height);

                TextRenderer.DrawText(
                    e.Graphics,
                    name,
                    new Font("Segoe UI", 10F, FontStyle.Bold),
                    nameRect,
                    OnSurface,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (col == "Activity")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Color back = text switch
                {
                    "Borrowed" => SecondaryContainer,
                    "Returned" => SurfaceHigh,
                    _ => Color.FromArgb(220, 248, 238)
                };

                Color fore = text switch
                {
                    "Returned" => OnSurfaceVariant,
                    _ => Primary
                };

                Size size = TextRenderer.MeasureText(text.ToUpper(), new Font("Segoe UI", 8.5F, FontStyle.Bold));
                Rectangle badge = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                    size.Width + 18,
                    24
                );

                using (SolidBrush brush = new SolidBrush(back))
                    e.Graphics.FillRectangle(brush, badge);

                TextRenderer.DrawText(
                    e.Graphics,
                    text.ToUpper(),
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    badge,
                    fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
            else if (col == "Status")
            {
                e.PaintBackground(e.CellBounds, true);

                string text = e.FormattedValue?.ToString() ?? "";
                Color dot = Color.FromArgb(16, 180, 110);
                Color fore = Color.FromArgb(16, 125, 88);

                Rectangle dotRect = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + (e.CellBounds.Height / 2) - 4, 8, 8);

                using (SolidBrush brush = new SolidBrush(dot))
                    e.Graphics.FillEllipse(brush, dotRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    new Font("Segoe UI", 10F, FontStyle.Regular),
                    new Rectangle(dotRect.Right + 8, e.CellBounds.Y, e.CellBounds.Width - 22, e.CellBounds.Height),
                    fore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
        }

        private void MonitoringForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 28;
            int width = ClientSize.Width - (margin * 2);

            lblPageTitle.Location = new Point(margin, 28);
            lblPageSub.Location = new Point(margin, 66);

            btnFilterLog.Size = new Size(126, 40);
            btnFilterLog.Location = new Point(ClientSize.Width - btnFilterLog.Width - margin, 38);

            activityPanel.Bounds = new Rectangle(margin, 112, width, 292);

            lblVitalsTitle.Location = new Point(margin, activityPanel.Bottom + 38);
            lblVitalsSub.Location = new Point(margin, lblVitalsTitle.Bottom + 6);

            int cardTop = lblVitalsSub.Bottom + 22;
            int cardHeight = 232;

            if (width >= 980)
            {
                int cardWidth = (width - (gap * 2)) / 3;

                cardUsers.Bounds = new Rectangle(margin, cardTop, cardWidth, cardHeight);
                cardStock.Bounds = new Rectangle(cardUsers.Right + gap, cardTop, cardWidth, cardHeight);
                cardFees.Bounds = new Rectangle(cardStock.Right + gap, cardTop, cardWidth, cardHeight);

                int bottomTop = cardUsers.Bottom + 36;
                int bottomWidth = (width - gap) / 2;

                reportPanel.Bounds = new Rectangle(margin, bottomTop, bottomWidth, 258);
                maintenancePanel.Bounds = new Rectangle(reportPanel.Right + gap, bottomTop, bottomWidth, 258);
            }
            else
            {
                int cardWidth = width;

                cardUsers.Bounds = new Rectangle(margin, cardTop, cardWidth, cardHeight);
                cardStock.Bounds = new Rectangle(margin, cardUsers.Bottom + gap, cardWidth, cardHeight);
                cardFees.Bounds = new Rectangle(margin, cardStock.Bottom + gap, cardWidth, cardHeight);

                int bottomTop = cardFees.Bottom + 36;

                reportPanel.Bounds = new Rectangle(margin, bottomTop, width, 258);
                maintenancePanel.Bounds = new Rectangle(margin, reportPanel.Bottom + gap, width, 258);
            }

            LayoutActivityPanel();
            LayoutBottomPanels();

            canvasPanel.AutoScrollMinSize = new Size(0, maintenancePanel.Bottom + 40);
        }

        private void LayoutActivityPanel()
        {
            dgvActivity.Location = new Point(0, 0);
            dgvActivity.Size = new Size(activityPanel.Width - 2, activityPanel.Height - 2);
        }

        private void LayoutBottomPanels()
        {
            reportImage.Bounds = new Rectangle(36, 34, 138, 138);
            lblReportTitle.Location = new Point(reportImage.Right + 28, 38);
            lblReportBody.Location = new Point(reportImage.Right + 28, lblReportTitle.Bottom + 14);
            btnDeepDive.Location = new Point(reportImage.Right + 24, 188);
            btnDeepDive.Size = new Size(170, 34);

            lblMaintenanceTitle.Location = new Point(34, 36);
            lblMaintenanceBody.Location = new Point(34, 82);
            lblMaintenanceChip1.Location = new Point(34, 152);
            lblMaintenanceChip2.Location = new Point(lblMaintenanceChip1.Right + 10, 152);
            lblMaintenanceIcon.Location = new Point(maintenancePanel.Width - 110, 138);
        }
    }
}