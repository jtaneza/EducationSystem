using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class LibrarySetupForm : Form
    {
        private readonly Color FormBack = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color CardBack = Color.White;
        private readonly Color FieldBack = ColorTranslator.FromHtml("#DDE6EA");

        private readonly Color SidebarDark = ColorTranslator.FromHtml("#2B3234");
        private readonly Color AccentMint = ColorTranslator.FromHtml("#6DFAD2");
        private readonly Color AccentEmerald = ColorTranslator.FromHtml("#00B894");
        private readonly Color AccentDeep = ColorTranslator.FromHtml("#006B55");
        private readonly Color AccentDanger = ColorTranslator.FromHtml("#A03F30");

        private readonly Color PrimaryText = ColorTranslator.FromHtml("#161D1F");
        private readonly Color SecondaryText = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color MutedText = ColorTranslator.FromHtml("#BBCAC3");

        private Panel canvas = null!;

        private Label lblKicker = null!;
        private Label lblTitle = null!;
        private Label lblSubTitle = null!;

        private Panel brandingCard = null!;
        private Panel hoursCard = null!;
        private Panel locationCard = null!;
        private Panel rulesCard = null!;

        private Panel fieldLibraryName = null!;
        private Panel fieldEmail = null!;
        private Panel fieldPhone = null!;
        private Panel fieldBlock = null!;
        private Panel fieldWing = null!;
        private Panel fieldZip = null!;

        private Label lblSundayClosed = null!;
        private Panel mapPanel = null!;
        private Label lblMapBadge = null!;

        private Panel guestToggle = null!;
        private Panel guestToggleKnob = null!;
        private bool guestBrowsingEnabled = true;

        private TrackBar trackLoanDays = null!;
        private Label lblLoanDays = null!;

        private ComboBox cboMaxBooks = null!;
        private Button btnSavePreferences = null!;

        public LibrarySetupForm()
        {
            InitializeComponent();

            BackColor = FormBack;
            FormBorderStyle = FormBorderStyle.None;
            Text = "LibrarySetupForm";

            BuildInterface();
            AdjustLayout();

            Load += LibrarySetupForm_Load;
            Resize += (s, e) => AdjustLayout();
        }

        private void LibrarySetupForm_Load(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void BuildInterface()
        {
            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = FormBack,
                AutoScroll = true
            };
            Controls.Add(canvas);

            lblKicker = new Label
            {
                Text = "✦ INSTITUTIONAL CONFIGURATION",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = AccentDeep,
                BackColor = Color.Transparent
            };

            lblTitle = new Label
            {
                Text = "Library Setup",
                AutoSize = true,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = PrimaryText,
                BackColor = Color.Transparent
            };

            lblSubTitle = new Label
            {
                Text = "Configure the core identity, operational rhythms, and institutional standards for the ABC School Library ecosystem.",
                AutoSize = false,
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };

            canvas.Controls.Add(lblKicker);
            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubTitle);

            BuildBrandingCard();
            BuildHoursCard();
            BuildLocationCard();
            BuildRulesCard();
        }

        private void BuildBrandingCard()
        {
            brandingCard = CreateLightCard();
            canvas.Controls.Add(brandingCard);

            var title = CreateCardTitle("Institutional Branding");
            var subtitle = CreateCardSubTitle("Global identification metadata");
            var icon = CreateCornerIcon("▣");

            var lblName = CreateFieldLabel("OFFICIAL LIBRARY NAME");
            fieldLibraryName = CreateValueField("ABC School Library");

            var lblEmail = CreateFieldLabel("CONTACT EMAIL");
            fieldEmail = CreateValueField("admin@abcschool.edu");

            var lblPhone = CreateFieldLabel("PHONE SYSTEM");
            fieldPhone = CreateValueField("+1 (555) 012-3456");

            brandingCard.Controls.Add(title);
            brandingCard.Controls.Add(subtitle);
            brandingCard.Controls.Add(icon);
            brandingCard.Controls.Add(lblName);
            brandingCard.Controls.Add(fieldLibraryName);
            brandingCard.Controls.Add(lblEmail);
            brandingCard.Controls.Add(fieldEmail);
            brandingCard.Controls.Add(lblPhone);
            brandingCard.Controls.Add(fieldPhone);

            brandingCard.Tag = new Control[] { title, subtitle, icon, lblName, fieldLibraryName, lblEmail, fieldEmail, lblPhone, fieldPhone };
        }

        private void BuildHoursCard()
        {
            hoursCard = CreateLightCard();
            canvas.Controls.Add(hoursCard);

            var title = CreateCardTitle("Operating Hours");
            var subtitle = CreateCardSubTitle("Library access schedule");
            var icon = CreateCornerIcon("◔");

            Panel row1 = CreateHoursRow("Mon - Fri", "08:00 AM", "06:00 PM");
            Panel row2 = CreateHoursRow("Saturday", "10:00 AM", "02:00 PM");
            Panel row3 = new Panel
            {
                BackColor = CardBack,
                Height = 62
            };

            Label sunday = new Label
            {
                Text = "Sunday",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = PrimaryText,
                BackColor = CardBack
            };

            lblSundayClosed = new Label
            {
                Text = "CLOSED",
                AutoSize = false,
                Size = new Size(84, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 218, 212),
                ForeColor = AccentDanger
            };

            row3.Controls.Add(sunday);
            row3.Controls.Add(lblSundayClosed);

            hoursCard.Controls.Add(title);
            hoursCard.Controls.Add(subtitle);
            hoursCard.Controls.Add(icon);
            hoursCard.Controls.Add(row1);
            hoursCard.Controls.Add(row2);
            hoursCard.Controls.Add(row3);

            hoursCard.Tag = new Control[] { title, subtitle, icon, row1, row2, row3, sunday };
        }

        private void BuildLocationCard()
        {
            locationCard = CreateLightCard();
            canvas.Controls.Add(locationCard);

            var title = CreateCardTitle("Campus Location");
            var subtitle = CreateCardSubTitle("Physical repository address");

            var lblBlock = CreateFieldLabel("BLOCK/BUILDING");
            fieldBlock = CreateValueField("Main Academic Block C");

            var lblWing = CreateFieldLabel("ROOM/WING");
            fieldWing = CreateValueField("Librarium Floor 2, East Wing");

            var lblZip = CreateFieldLabel("ZIP CODE");
            fieldZip = CreateValueField("90210");

            mapPanel = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#E3E8EA")
            };
            mapPanel.Paint += DrawMapTexture;

            Panel badge = new Panel
            {
                BackColor = Color.FromArgb(245, 248, 249)
            };
            badge.Paint += (s, e) =>
            {
                using Pen pen = new Pen(Color.FromArgb(220, 226, 228));
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, badge.Width - 1, badge.Height - 1));
            };

            Label pin = new Label
            {
                Text = "📍",
                AutoSize = true,
                Font = new Font("Segoe UI Emoji", 16F),
                Location = new Point(16, 13),
                BackColor = Color.Transparent
            };

            lblMapBadge = new Label
            {
                Text = "Primary Repository: Academic Block C, Floor 2",
                AutoSize = false,
                Font = new Font("Segoe UI", 10F),
                ForeColor = PrimaryText,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            badge.Controls.Add(pin);
            badge.Controls.Add(lblMapBadge);
            mapPanel.Controls.Add(badge);

            locationCard.Controls.Add(title);
            locationCard.Controls.Add(subtitle);
            locationCard.Controls.Add(lblBlock);
            locationCard.Controls.Add(fieldBlock);
            locationCard.Controls.Add(lblWing);
            locationCard.Controls.Add(fieldWing);
            locationCard.Controls.Add(lblZip);
            locationCard.Controls.Add(fieldZip);
            locationCard.Controls.Add(mapPanel);

            locationCard.Tag = new Control[] { title, subtitle, lblBlock, fieldBlock, lblWing, fieldWing, lblZip, fieldZip, mapPanel, badge };
        }

        private void BuildRulesCard()
        {
            rulesCard = new Panel
            {
                BackColor = SidebarDark
            };
            rulesCard.Paint += RoundedPanelPaint;
            canvas.Controls.Add(rulesCard);

            Label title = new Label
            {
                Text = "System Rules",
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = AccentMint,
                BackColor = Color.Transparent
            };

            Label subtitle = new Label
            {
                Text = "Institutional Preferences",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = MutedText,
                BackColor = Color.Transparent
            };

            Label icon = new Label
            {
                Text = "⬢",
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 16F, FontStyle.Bold),
                ForeColor = AccentMint,
                BackColor = Color.Transparent
            };

            Label lblGuest = new Label
            {
                Text = "Guest Browsing",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            guestToggle = new Panel
            {
                Size = new Size(48, 24),
                BackColor = AccentDeep,
                Cursor = Cursors.Hand
            };
            guestToggle.Paint += RoundedPanelPaint;

            guestToggleKnob = new Panel
            {
                Size = new Size(18, 18),
                BackColor = Color.White
            };
            guestToggleKnob.Paint += RoundedPanelPaint;
            guestToggle.Controls.Add(guestToggleKnob);

            guestToggle.Click += ToggleGuestBrowsing;
            guestToggleKnob.Click += ToggleGuestBrowsing;

            Label lblLoan = new Label
            {
                Text = "Default Loan Period",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            lblLoanDays = new Label
            {
                Text = "14 Days",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = AccentMint,
                BackColor = Color.Transparent
            };

            trackLoanDays = new TrackBar
            {
                Minimum = 1,
                Maximum = 30,
                Value = 14,
                TickStyle = TickStyle.None,
                BackColor = SidebarDark
            };
            trackLoanDays.ValueChanged += (s, e) => lblLoanDays.Text = $"{trackLoanDays.Value} Days";

            Label lblMax = new Label
            {
                Text = "MAX BOOKS PER USER",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = MutedText,
                BackColor = Color.Transparent
            };

            cboMaxBooks = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F),
                BackColor = ColorTranslator.FromHtml("#44524D"),
                ForeColor = Color.White
            };
            cboMaxBooks.Items.AddRange(new object[]
            {
                "5 Books",
                "10 Books",
                "15 Books",
                "20 Books",
                "Unlimited"
            });
            cboMaxBooks.SelectedIndex = 1;

            btnSavePreferences = new Button
            {
                Text = "SAVE ALL PREFERENCES",
                FlatStyle = FlatStyle.Flat,
                BackColor = AccentEmerald,
                ForeColor = Color.FromArgb(0, 66, 51),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSavePreferences.FlatAppearance.BorderSize = 0;

            rulesCard.Controls.Add(title);
            rulesCard.Controls.Add(subtitle);
            rulesCard.Controls.Add(icon);
            rulesCard.Controls.Add(lblGuest);
            rulesCard.Controls.Add(guestToggle);
            rulesCard.Controls.Add(lblLoan);
            rulesCard.Controls.Add(lblLoanDays);
            rulesCard.Controls.Add(trackLoanDays);
            rulesCard.Controls.Add(lblMax);
            rulesCard.Controls.Add(cboMaxBooks);
            rulesCard.Controls.Add(btnSavePreferences);

            rulesCard.Tag = new Control[]
            {
                title, subtitle, icon, lblGuest, guestToggle, lblLoan,
                lblLoanDays, trackLoanDays, lblMax, cboMaxBooks, btnSavePreferences
            };

            UpdateGuestToggleVisual();
        }

        private void ToggleGuestBrowsing(object? sender, EventArgs e)
        {
            guestBrowsingEnabled = !guestBrowsingEnabled;
            UpdateGuestToggleVisual();
        }

        private void UpdateGuestToggleVisual()
        {
            guestToggle.BackColor = guestBrowsingEnabled
                ? AccentDeep
                : ColorTranslator.FromHtml("#64706B");

            guestToggleKnob.Location = guestBrowsingEnabled
                ? new Point(26, 3)
                : new Point(4, 3);

            guestToggle.Invalidate();
            guestToggleKnob.Invalidate();
        }

        private Panel CreateLightCard()
        {
            Panel p = new Panel
            {
                BackColor = CardBack
            };
            p.Paint += RoundedPanelPaint;
            return p;
        }

        private Label CreateCardTitle(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = PrimaryText,
                Location = new Point(32, 28),
                BackColor = Color.Transparent
            };
        }

        private Label CreateCardSubTitle(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                ForeColor = SecondaryText,
                Location = new Point(32, 64),
                BackColor = Color.Transparent
            };
        }

        private Label CreateCornerIcon(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI Symbol", 16F, FontStyle.Bold),
                ForeColor = AccentEmerald,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };
        }

        private Label CreateFieldLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = SecondaryText,
                BackColor = Color.Transparent
            };
        }

        private Panel CreateValueField(string value)
        {
            Panel p = new Panel
            {
                BackColor = FieldBack
            };

            Label lbl = new Label
            {
                Text = value,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                ForeColor = PrimaryText,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 16, 0),
                BackColor = FieldBack
            };

            p.Controls.Add(lbl);
            return p;
        }

        private Panel CreateHoursRow(string day, string start, string end)
        {
            Panel row = new Panel
            {
                BackColor = CardBack,
                Height = 60
            };

            Label lblDay = new Label
            {
                Text = day,
                AutoSize = true,
                Font = new Font("Segoe UI", 12F),
                ForeColor = PrimaryText,
                BackColor = CardBack
            };

            Panel lblStart = CreateHourChip(start);

            Panel separator = new Panel
            {
                Size = new Size(20, 2),
                BackColor = SecondaryText
            };

            Panel lblEnd = CreateHourChip(end);

            row.Controls.Add(lblDay);
            row.Controls.Add(lblStart);
            row.Controls.Add(separator);
            row.Controls.Add(lblEnd);

            row.Tag = new Control[] { lblDay, lblStart, separator, lblEnd };
            return row;
        }

        private Panel CreateHourChip(string text)
        {
            Panel chip = new Panel
            {
                Size = new Size(86, 30),
                BackColor = FieldBack
            };

            Label lbl = new Label
            {
                Dock = DockStyle.Fill,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F),
                ForeColor = PrimaryText,
                BackColor = FieldBack
            };

            chip.Controls.Add(lbl);
            return chip;
        }

        private void AdjustLayout()
        {
            int margin = 40;
            int gap = 28;
            int usableWidth = Math.Max(1180, canvas.ClientSize.Width - (margin * 2));

            lblKicker.Location = new Point(margin, 28);
            lblTitle.Location = new Point(margin, 54);
            lblSubTitle.Location = new Point(margin, 108);
            lblSubTitle.Size = new Size(Math.Min(920, usableWidth), 84);

            int topCardsY = 202;
            int leftWidth = (int)(usableWidth * 0.56);
            int rightWidth = usableWidth - leftWidth - gap;

            int topCardHeight = 338;
            brandingCard.Bounds = new Rectangle(margin, topCardsY, leftWidth, topCardHeight);
            hoursCard.Bounds = new Rectangle(brandingCard.Right + gap, topCardsY, rightWidth, topCardHeight);

            int bottomY = topCardsY + topCardHeight + 30;
            int bottomLeftWidth = (int)(usableWidth * 0.64);
            int bottomRightWidth = usableWidth - bottomLeftWidth - gap;

            int lowerCardHeight = 520;
            locationCard.Bounds = new Rectangle(margin, bottomY, bottomLeftWidth, lowerCardHeight);
            rulesCard.Bounds = new Rectangle(locationCard.Right + gap, bottomY, bottomRightWidth, lowerCardHeight);

            LayoutBrandingCard();
            LayoutHoursCard();
            LayoutLocationCard();
            LayoutRulesCard();

            canvas.AutoScrollMinSize = new Size(0, rulesCard.Bottom + 80);
        }

        private void LayoutBrandingCard()
        {
            var c = (Control[])brandingCard.Tag;
            Label icon = (Label)c[2];
            Label lblName = (Label)c[3];
            Panel valueName = (Panel)c[4];
            Label lblEmail = (Label)c[5];
            Panel valueEmail = (Panel)c[6];
            Label lblPhone = (Label)c[7];
            Panel valuePhone = (Panel)c[8];

            icon.Location = new Point(brandingCard.Width - 50, 30);

            lblName.Location = new Point(32, 122);
            valueName.Bounds = new Rectangle(32, 148, brandingCard.Width - 64, 50);

            int halfGap = 20;
            int halfWidth = (brandingCard.Width - 64 - halfGap) / 2;

            lblEmail.Location = new Point(32, 232);
            valueEmail.Bounds = new Rectangle(32, 258, halfWidth, 50);

            lblPhone.Location = new Point(32 + halfWidth + halfGap, 232);
            valuePhone.Bounds = new Rectangle(32 + halfWidth + halfGap, 258, halfWidth, 50);
        }

        private void LayoutHoursCard()
        {
            var c = (Control[])hoursCard.Tag;
            Label icon = (Label)c[2];
            Panel row1 = (Panel)c[3];
            Panel row2 = (Panel)c[4];
            Panel row3 = (Panel)c[5];
            Label sunday = (Label)c[6];

            icon.Location = new Point(hoursCard.Width - 50, 30);

            row1.Bounds = new Rectangle(28, 118, hoursCard.Width - 56, 60);
            row2.Bounds = new Rectangle(28, 184, hoursCard.Width - 56, 60);
            row3.Bounds = new Rectangle(28, 250, hoursCard.Width - 56, 60);

            LayoutHoursRow(row1);
            LayoutHoursRow(row2);

            sunday.Location = new Point(12, 16);
            lblSundayClosed.Location = new Point(row3.Width - 94, 14);
        }

        private void LayoutHoursRow(Panel row)
        {
            var items = (Control[])row.Tag;
            Label lblDay = (Label)items[0];
            Panel start = (Panel)items[1];
            Panel separator = (Panel)items[2];
            Panel end = (Panel)items[3];

            lblDay.Location = new Point(12, 18);
            end.Location = new Point(row.Width - end.Width - 12, 15);
            separator.Location = new Point(end.Left - 30, 29);
            start.Location = new Point(separator.Left - start.Width - 10, 15);
        }

        private void LayoutLocationCard()
        {
            var c = (Control[])locationCard.Tag;
            Label lblBlock = (Label)c[2];
            Panel valueBlock = (Panel)c[3];
            Label lblWing = (Label)c[4];
            Panel valueWing = (Panel)c[5];
            Label lblZip = (Label)c[6];
            Panel valueZip = (Panel)c[7];
            Panel map = (Panel)c[8];
            Panel badge = (Panel)c[9];

            int formWidth = (int)(locationCard.Width * 0.47);
            int mapWidth = locationCard.Width - formWidth;

            lblBlock.Location = new Point(32, 124);
            valueBlock.Bounds = new Rectangle(32, 150, formWidth - 64, 42);

            lblWing.Location = new Point(32, 210);
            valueWing.Bounds = new Rectangle(32, 236, formWidth - 64, 42);

            lblZip.Location = new Point(32, 296);
            valueZip.Bounds = new Rectangle(32, 322, formWidth - 64, 42);

            map.Bounds = new Rectangle(formWidth, 0, mapWidth, locationCard.Height);

            badge.Bounds = new Rectangle(
                42,
                map.Height - 86,
                Math.Min(290, map.Width - 84),
                58
            );

            lblMapBadge.Location = new Point(54, 10);
            lblMapBadge.Size = new Size(badge.Width - 70, 40);
        }

        private void LayoutRulesCard()
        {
            var c = (Control[])rulesCard.Tag;
            Label title = (Label)c[0];
            Label subtitle = (Label)c[1];
            Label icon = (Label)c[2];
            Label lblGuest = (Label)c[3];
            Panel toggle = (Panel)c[4];
            Label lblLoan = (Label)c[5];
            Label lblDays = (Label)c[6];
            TrackBar track = (TrackBar)c[7];
            Label lblMax = (Label)c[8];
            ComboBox cbo = (ComboBox)c[9];
            Button btnSave = (Button)c[10];

            title.Location = new Point(32, 34);
            subtitle.Location = new Point(32, 70);
            icon.Location = new Point(rulesCard.Width - 50, 34);

            lblGuest.Location = new Point(32, 136);
            toggle.Location = new Point(rulesCard.Width - 86, 134);

            lblLoan.Location = new Point(32, 208);
            lblDays.Location = new Point(rulesCard.Width - lblDays.PreferredWidth - 32, 210);
            track.Bounds = new Rectangle(28, 246, rulesCard.Width - 56, 34);

            lblMax.Location = new Point(32, 314);
            cbo.Bounds = new Rectangle(32, 342, rulesCard.Width - 64, 42);

            btnSave.Bounds = new Rectangle(32, rulesCard.Height - 88, rulesCard.Width - 64, 52);
        }

        private void DrawMapTexture(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;

            using Pen p1 = new Pen(Color.FromArgb(235, 240, 241), 1);
            using Pen p2 = new Pen(Color.FromArgb(220, 226, 228), 1);

            for (int x = 20; x < p.Width; x += 38)
                e.Graphics.DrawLine(p1, x, 0, x + 50, p.Height);

            for (int y = 15; y < p.Height; y += 34)
                e.Graphics.DrawLine(p2, 0, y, p.Width, y + 25);
        }

        private void RoundedPanelPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control p) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var brush = new SolidBrush(p.BackColor);
            using var path = GetRoundedRectPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 14);
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