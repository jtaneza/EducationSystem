using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Data.SqlClient;

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

        private TextBox txtLibraryName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPhone = null!;
        private TextBox txtBlock = null!;
        private TextBox txtWing = null!;
        private TextBox txtZip = null!;
        private TextBox txtMapTitle = null!;
        private TextBox txtMapAddress = null!;

        private Label lblSundayClosed = null!;
        private DateTimePicker dtMonFriOpen = null!;
        private DateTimePicker dtMonFriClose = null!;
        private DateTimePicker dtSaturdayOpen = null!;
        private DateTimePicker dtSaturdayClose = null!;
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
            LoadSettingsFromDatabase();
            AdjustLayout();
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
            txtLibraryName = CreateValueTextBox("ABC School Library");

            var lblEmail = CreateFieldLabel("CONTACT EMAIL");
            txtEmail = CreateValueTextBox("admin@abcschool.edu");

            var lblPhone = CreateFieldLabel("PHONE SYSTEM");
            txtPhone = CreateValueTextBox("+1 (555) 012-3456");

            brandingCard.Controls.Add(title);
            brandingCard.Controls.Add(subtitle);
            brandingCard.Controls.Add(icon);
            brandingCard.Controls.Add(lblName);
            brandingCard.Controls.Add(txtLibraryName);
            brandingCard.Controls.Add(lblEmail);
            brandingCard.Controls.Add(txtEmail);
            brandingCard.Controls.Add(lblPhone);
            brandingCard.Controls.Add(txtPhone);

            brandingCard.Tag = new Control[] { title, subtitle, icon, lblName, txtLibraryName, lblEmail, txtEmail, lblPhone, txtPhone };
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

            var title = CreateCardTitle("Library Location");
            var subtitle = CreateCardSubTitle("Set the Philippine address shown in the member portal and reports.");

            var lblBlock = CreateFieldLabel("SCHOOL / CAMPUS NAME");
            txtBlock = CreateValueTextBox("ABC School Library");

            var lblWing = CreateFieldLabel("STREET / BARANGAY");
            txtWing = CreateValueTextBox("Barangay Poblacion");

            var lblZip = CreateFieldLabel("POSTAL CODE");
            txtZip = CreateValueTextBox("8000");

            var lblMapTitle = CreateFieldLabel("BRANCH / LOCATION LABEL");
            txtMapTitle = CreateValueTextBox("Main Library Branch");

            var lblMapAddress = CreateFieldLabel("CITY / PROVINCE / COUNTRY");
            txtMapAddress = CreateValueTextBox("Davao City, Davao del Sur, Philippines");

            txtBlock.TextChanged += LocationTextChanged;
            txtWing.TextChanged += LocationTextChanged;
            txtZip.TextChanged += LocationTextChanged;
            txtMapTitle.TextChanged += LocationTextChanged;
            txtMapAddress.TextChanged += LocationTextChanged;

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
            locationCard.Controls.Add(txtBlock);
            locationCard.Controls.Add(lblWing);
            locationCard.Controls.Add(txtWing);
            locationCard.Controls.Add(lblZip);
            locationCard.Controls.Add(txtZip);
            locationCard.Controls.Add(lblMapTitle);
            locationCard.Controls.Add(txtMapTitle);
            locationCard.Controls.Add(lblMapAddress);
            locationCard.Controls.Add(txtMapAddress);
            locationCard.Controls.Add(mapPanel);

            locationCard.Tag = new Control[]
            {
                title, subtitle,
                lblBlock, txtBlock,
                lblWing, txtWing,
                lblZip, txtZip,
                lblMapTitle, txtMapTitle,
                lblMapAddress, txtMapAddress,
                mapPanel, badge
            };

            UpdateMapBadge();
        }

        private void LocationTextChanged(object? sender, EventArgs e)
        {
            UpdateMapBadge();
            mapPanel?.Invalidate();
        }

        private void UpdateMapBadge()
        {
            if (lblMapBadge == null)
                return;

            string title = string.IsNullOrWhiteSpace(txtMapTitle?.Text)
                ? "Main Library Branch"
                : txtMapTitle.Text.Trim();

            string address = string.IsNullOrWhiteSpace(txtMapAddress?.Text)
                ? "Davao City, Davao del Sur, Philippines"
                : txtMapAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(address))
                address = "Campus location not set";

            lblMapBadge.Text = $"{title}: {address}";
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
            btnSavePreferences.Click += SavePreferences_Click;

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

        private TextBox CreateValueTextBox(string value)
        {
            TextBox txt = new TextBox
            {
                Text = value,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11F),
                ForeColor = PrimaryText,
                BackColor = FieldBack,
                Multiline = true
            };

            return txt;
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

            DateTimePicker startPicker = CreateTimePicker(start);
            DateTimePicker endPicker = CreateTimePicker(end);

            Panel separator = new Panel
            {
                Size = new Size(20, 2),
                BackColor = SecondaryText
            };

            if (day == "Mon - Fri")
            {
                dtMonFriOpen = startPicker;
                dtMonFriClose = endPicker;
            }
            else if (day == "Saturday")
            {
                dtSaturdayOpen = startPicker;
                dtSaturdayClose = endPicker;
            }

            row.Controls.Add(lblDay);
            row.Controls.Add(startPicker);
            row.Controls.Add(separator);
            row.Controls.Add(endPicker);

            row.Tag = new Control[] { lblDay, startPicker, separator, endPicker };
            return row;
        }

        private DateTimePicker CreateTimePicker(string text)
        {
            DateTimePicker picker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "hh:mm tt",
                ShowUpDown = true,
                Font = new Font("Segoe UI", 9F),
                Size = new Size(86, 30),
                CalendarForeColor = PrimaryText,
                CalendarMonthBackground = FieldBack
            };

            if (DateTime.TryParse(text, out DateTime value))
                picker.Value = DateTime.Today.Add(value.TimeOfDay);

            return picker;
        }

        private void AdjustLayout()
        {
            if (canvas == null || canvas.ClientSize.Width <= 0)
                return;

            int margin = canvas.ClientSize.Width < 900 ? 18 : 40;
            int gap = canvas.ClientSize.Width < 900 ? 18 : 28;
            int usableWidth = Math.Max(320, canvas.ClientSize.Width - (margin * 2));

            lblKicker.Location = new Point(margin, 28);
            lblTitle.Location = new Point(margin, 54);
            lblSubTitle.Location = new Point(margin, 108);
            lblSubTitle.Size = new Size(Math.Min(920, usableWidth), 84);

            int topCardsY = 202;

            /*
             * Laptop fix:
             * The previous responsive version stacked the cards too early.
             * Most laptops still have enough content width to keep the PC-style layout:
             * Branding + Hours on top, Location + Rules below.
             */
            bool desktopStyle = usableWidth >= 960;

            if (desktopStyle)
            {
                int leftWidth = (int)(usableWidth * 0.56);
                int rightWidth = usableWidth - leftWidth - gap;

                int topCardHeight = leftWidth < 620 ? 430 : 338;
                brandingCard.Bounds = new Rectangle(margin, topCardsY, leftWidth, topCardHeight);
                hoursCard.Bounds = new Rectangle(brandingCard.Right + gap, topCardsY, rightWidth, topCardHeight);

                int bottomY = topCardsY + topCardHeight + 30;
                int bottomLeftWidth = (int)(usableWidth * 0.64);
                int bottomRightWidth = usableWidth - bottomLeftWidth - gap;

                int lowerCardHeight = 520;
                locationCard.Bounds = new Rectangle(margin, bottomY, bottomLeftWidth, lowerCardHeight);
                rulesCard.Bounds = new Rectangle(locationCard.Right + gap, bottomY, bottomRightWidth, lowerCardHeight);
            }
            else
            {
                int fullWidth = usableWidth;
                int topCardHeight = 338;
                int lowerCardHeight = 520;

                brandingCard.Bounds = new Rectangle(margin, topCardsY, fullWidth, topCardHeight);
                hoursCard.Bounds = new Rectangle(margin, brandingCard.Bottom + gap, fullWidth, topCardHeight);

                int bottomY = hoursCard.Bottom + gap;
                int compactLocationHeight = fullWidth < 820 ? 920 : lowerCardHeight;
                locationCard.Bounds = new Rectangle(margin, bottomY, fullWidth, compactLocationHeight);
                rulesCard.Bounds = new Rectangle(margin, locationCard.Bottom + gap, fullWidth, lowerCardHeight);
            }

            LayoutBrandingCard();
            LayoutHoursCard();
            LayoutLocationCard();
            LayoutRulesCard();

            int bottom = Math.Max(rulesCard.Bottom, locationCard.Bottom);
            canvas.AutoScrollMinSize = new Size(0, bottom + 80);
            canvas.HorizontalScroll.Enabled = false;
            canvas.HorizontalScroll.Visible = false;
        }

        private void LayoutBrandingCard()
        {
            var c = (Control[])brandingCard.Tag;
            Label icon = (Label)c[2];
            Label lblName = (Label)c[3];
            TextBox valueName = (TextBox)c[4];
            Label lblEmail = (Label)c[5];
            TextBox valueEmail = (TextBox)c[6];
            Label lblPhone = (Label)c[7];
            TextBox valuePhone = (TextBox)c[8];

            icon.Location = new Point(brandingCard.Width - 50, 30);

            lblName.Location = new Point(32, 122);
            valueName.Bounds = new Rectangle(32, 148, brandingCard.Width - 64, 50);

            int halfGap = 20;

            if (brandingCard.Width >= 620)
            {
                int halfWidth = (brandingCard.Width - 64 - halfGap) / 2;

                lblEmail.Location = new Point(32, 232);
                valueEmail.Bounds = new Rectangle(32, 258, halfWidth, 50);

                lblPhone.Location = new Point(32 + halfWidth + halfGap, 232);
                valuePhone.Bounds = new Rectangle(32 + halfWidth + halfGap, 258, halfWidth, 50);
            }
            else
            {
                int fullWidth = brandingCard.Width - 64;

                lblEmail.Location = new Point(32, 220);
                valueEmail.Bounds = new Rectangle(32, 246, fullWidth, 46);

                lblPhone.Location = new Point(32, 308);
                valuePhone.Bounds = new Rectangle(32, 334, fullWidth, 46);
            }
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
            DateTimePicker start = (DateTimePicker)items[1];
            Panel separator = (Panel)items[2];
            DateTimePicker end = (DateTimePicker)items[3];

            lblDay.Location = new Point(12, 18);
            end.Location = new Point(row.Width - end.Width - 12, 15);
            separator.Location = new Point(end.Left - 30, 29);
            start.Location = new Point(separator.Left - start.Width - 10, 15);
        }

        private void LayoutLocationCard()
        {
            var c = (Control[])locationCard.Tag;

            Label lblBlock = (Label)c[2];
            TextBox valueBlock = (TextBox)c[3];
            Label lblWing = (Label)c[4];
            TextBox valueWing = (TextBox)c[5];
            Label lblZip = (Label)c[6];
            TextBox valueZip = (TextBox)c[7];
            Label lblMapTitle = (Label)c[8];
            TextBox valueMapTitle = (TextBox)c[9];
            Label lblMapAddress = (Label)c[10];
            TextBox valueMapAddress = (TextBox)c[11];
            Panel map = (Panel)c[12];
            Panel badge = (Panel)c[13];

            // Keep the PC-style left form + right map as long as there is enough room.
            bool compact = locationCard.Width < 620;

            int formWidth = compact
                ? locationCard.Width
                : Math.Max(390, (int)(locationCard.Width * 0.54));

            c[0].Location = new Point(32, 34); // title
            c[1].Location = new Point(32, 70); // subtitle
            ((Label)c[1]).MaximumSize = new Size(Math.Max(260, formWidth - 64), 0);

            int inputWidth = Math.Max(220, formWidth - 64);

            lblBlock.Location = new Point(32, 128);
            valueBlock.Bounds = new Rectangle(32, 154, inputWidth, 42);

            lblWing.Location = new Point(32, 214);
            valueWing.Bounds = new Rectangle(32, 240, inputWidth, 42);

            lblZip.Location = new Point(32, 300);
            valueZip.Bounds = new Rectangle(32, 326, inputWidth, 42);

            lblMapTitle.Location = new Point(32, 386);
            valueMapTitle.Bounds = new Rectangle(32, 412, inputWidth, 42);

            lblMapAddress.Location = new Point(32, 472);
            valueMapAddress.Bounds = new Rectangle(32, 498, inputWidth, 42);

            if (compact)
            {
                map.Bounds = new Rectangle(
                    32,
                    558,
                    Math.Max(260, locationCard.Width - 64),
                    Math.Max(300, locationCard.Height - 590)
                );
            }
            else
            {
                int gap = 24;
                int mapX = formWidth + gap;
                int mapWidth = Math.Max(240, locationCard.Width - mapX);

                map.Bounds = new Rectangle(mapX, 0, mapWidth, locationCard.Height);
            }

            badge.Bounds = new Rectangle(
                42,
                Math.Max(20, map.Height - 96),
                Math.Min(380, Math.Max(220, map.Width - 84)),
                68
            );

            lblMapBadge.Location = new Point(54, 8);
            lblMapBadge.Size = new Size(Math.Max(120, badge.Width - 70), 52);

            UpdateMapBadge();
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


        private int GetCurrentClientId()
        {
            try
            {
                if (int.TryParse(Convert.ToString(ClientSession.ClientId), out int clientId) && clientId > 0)
                    return clientId;
            }
            catch
            {
                // fallback below
            }

            return 1;
        }

        private void EnsureLibrarySetupSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.ClientLibraries', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ClientLibraries
    (
        ClientID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        LibraryCode NVARCHAR(50) NULL,
        LibraryName NVARCHAR(200) NULL,
        Email NVARCHAR(150) NULL,
        PasswordText NVARCHAR(150) NULL,
        UserCount INT NOT NULL DEFAULT 0,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
        ImagePath NVARCHAR(250) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

IF COL_LENGTH('dbo.ClientLibraries', 'LibraryName') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD LibraryName NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'Email') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD Email NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'Phone') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD Phone NVARCHAR(50) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'BlockBuilding') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD BlockBuilding NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'RoomWing') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD RoomWing NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'ZipCode') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD ZipCode NVARCHAR(20) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'MapTitle') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD MapTitle NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'MapAddress') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD MapAddress NVARCHAR(300) NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'MondayFridayOpen') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD MondayFridayOpen TIME NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'MondayFridayClose') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD MondayFridayClose TIME NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'SaturdayOpen') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD SaturdayOpen TIME NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'SaturdayClose') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD SaturdayClose TIME NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'SundayClosed') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD SundayClosed BIT NOT NULL CONSTRAINT DF_ClientLibraries_SundayClosed DEFAULT 1;

IF COL_LENGTH('dbo.ClientLibraries', 'GuestBrowsingEnabled') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD GuestBrowsingEnabled BIT NOT NULL CONSTRAINT DF_ClientLibraries_GuestBrowsing DEFAULT 1;

IF COL_LENGTH('dbo.ClientLibraries', 'DefaultLoanDays') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD DefaultLoanDays INT NOT NULL CONSTRAINT DF_ClientLibraries_DefaultLoanDays DEFAULT 14;

IF COL_LENGTH('dbo.ClientLibraries', 'MaxBooksPerUser') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD MaxBooksPerUser INT NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'UpdatedAt') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD UpdatedAt DATETIME2 NULL;

IF COL_LENGTH('dbo.ClientLibraries', 'CreatedAt') IS NULL
    ALTER TABLE dbo.ClientLibraries ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_ClientLibraries_CreatedAt_Setup DEFAULT SYSUTCDATETIME();

IF OBJECT_ID('dbo.SystemConfigurations', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.SystemConfigurations', 'LibraryHoursMonFri') IS NULL
        ALTER TABLE dbo.SystemConfigurations ADD LibraryHoursMonFri NVARCHAR(50) NULL;

    IF COL_LENGTH('dbo.SystemConfigurations', 'LibraryHoursSaturday') IS NULL
        ALTER TABLE dbo.SystemConfigurations ADD LibraryHoursSaturday NVARCHAR(50) NULL;

    IF COL_LENGTH('dbo.SystemConfigurations', 'LibraryHoursSunday') IS NULL
        ALTER TABLE dbo.SystemConfigurations ADD LibraryHoursSunday NVARCHAR(50) NULL;

    IF COL_LENGTH('dbo.SystemConfigurations', 'LibraryBranchName') IS NULL
        ALTER TABLE dbo.SystemConfigurations ADD LibraryBranchName NVARCHAR(150) NULL;

    IF COL_LENGTH('dbo.SystemConfigurations', 'LibraryBranchLocation') IS NULL
        ALTER TABLE dbo.SystemConfigurations ADD LibraryBranchLocation NVARCHAR(150) NULL;
END;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void LoadSettingsFromDatabase()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureLibrarySetupSchema(conn);

                int clientId = GetCurrentClientId();

                const string query = @"
SELECT TOP 1
    LibraryName,
    Email,
    Phone,
    BlockBuilding,
    RoomWing,
    ZipCode,
    MapTitle,
    MapAddress,
    MondayFridayOpen,
    MondayFridayClose,
    SaturdayOpen,
    SaturdayClose,
    SundayClosed,
    GuestBrowsingEnabled,
    DefaultLoanDays,
    MaxBooksPerUser
FROM dbo.ClientLibraries
WHERE ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClientID", clientId);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return;

                txtLibraryName.Text = ReadText(reader["LibraryName"], "ABC School Library");
                txtEmail.Text = ReadText(reader["Email"], "admin@abcschool.edu");
                txtPhone.Text = ReadText(reader["Phone"], "+1 (555) 012-3456");
                txtBlock.Text = ReadText(reader["BlockBuilding"], "ABC School Library");
                txtWing.Text = ReadText(reader["RoomWing"], "Barangay Poblacion");
                txtZip.Text = ReadText(reader["ZipCode"], "8000");
                txtMapTitle.Text = ReadText(reader["MapTitle"], "Main Library Branch");
                txtMapAddress.Text = ReadText(reader["MapAddress"], "Davao City, Davao del Sur, Philippines");
                UpdateMapBadge();

                SetTimePicker(dtMonFriOpen, reader["MondayFridayOpen"], "08:00 AM");
                SetTimePicker(dtMonFriClose, reader["MondayFridayClose"], "06:00 PM");
                SetTimePicker(dtSaturdayOpen, reader["SaturdayOpen"], "10:00 AM");
                SetTimePicker(dtSaturdayClose, reader["SaturdayClose"], "02:00 PM");

                guestBrowsingEnabled = reader["GuestBrowsingEnabled"] == DBNull.Value || Convert.ToBoolean(reader["GuestBrowsingEnabled"]);
                UpdateGuestToggleVisual();

                int loanDays = reader["DefaultLoanDays"] == DBNull.Value ? 14 : Convert.ToInt32(reader["DefaultLoanDays"]);
                loanDays = Math.Max(trackLoanDays.Minimum, Math.Min(trackLoanDays.Maximum, loanDays));
                trackLoanDays.Value = loanDays;
                lblLoanDays.Text = $"{loanDays} Days";

                int maxBooks = reader["MaxBooksPerUser"] == DBNull.Value ? 10 : Convert.ToInt32(reader["MaxBooksPerUser"]);
                SelectMaxBooks(maxBooks);

                UpdateMapBadge();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Library setup could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string ReadText(object value, string fallback)
        {
            if (value == null || value == DBNull.Value)
                return fallback;

            string text = Convert.ToString(value) ?? "";
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        }

        private void SetTimePicker(DateTimePicker picker, object value, string fallback)
        {
            TimeSpan time;

            if (value == null || value == DBNull.Value)
            {
                DateTime.TryParse(fallback, out DateTime fallbackDate);
                time = fallbackDate.TimeOfDay;
            }
            else if (value is TimeSpan span)
            {
                time = span;
            }
            else
            {
                DateTime.TryParse(Convert.ToString(value), out DateTime parsed);
                time = parsed.TimeOfDay;
            }

            picker.Value = DateTime.Today.Add(time);
        }

        private void SelectMaxBooks(int maxBooks)
        {
            string value = maxBooks <= 0 ? "Unlimited" : $"{maxBooks} Books";

            int index = cboMaxBooks.Items.IndexOf(value);
            cboMaxBooks.SelectedIndex = index >= 0 ? index : 1;
        }

        private int GetSelectedMaxBooks()
        {
            string selected = cboMaxBooks.SelectedItem?.ToString() ?? "10 Books";

            if (selected.Equals("Unlimited", StringComparison.OrdinalIgnoreCase))
                return 0;

            string digits = new string(selected.Where(char.IsDigit).ToArray());

            return int.TryParse(digits, out int result) ? result : 10;
        }

        private void SavePreferences_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLibraryName.Text))
            {
                MessageBox.Show("Please enter the official library name.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLibraryName.Focus();
                return;
            }

            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureLibrarySetupSchema(conn);

                int clientId = GetCurrentClientId();

                const string query = @"
UPDATE dbo.ClientLibraries
SET
    LibraryName = @LibraryName,
    Email = @Email,
    Phone = @Phone,
    BlockBuilding = @BlockBuilding,
    RoomWing = @RoomWing,
    ZipCode = @ZipCode,
    MapTitle = @MapTitle,
    MapAddress = @MapAddress,
    MondayFridayOpen = @MondayFridayOpen,
    MondayFridayClose = @MondayFridayClose,
    SaturdayOpen = @SaturdayOpen,
    SaturdayClose = @SaturdayClose,
    SundayClosed = @SundayClosed,
    GuestBrowsingEnabled = @GuestBrowsingEnabled,
    DefaultLoanDays = @DefaultLoanDays,
    MaxBooksPerUser = @MaxBooksPerUser,
    UpdatedAt = SYSUTCDATETIME()
WHERE ClientID = @ClientID;";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ClientID", clientId);
                cmd.Parameters.AddWithValue("@LibraryName", txtLibraryName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@BlockBuilding", txtBlock.Text.Trim());
                cmd.Parameters.AddWithValue("@RoomWing", txtWing.Text.Trim());
                cmd.Parameters.AddWithValue("@ZipCode", txtZip.Text.Trim());
                cmd.Parameters.AddWithValue("@MapTitle", txtMapTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@MapAddress", txtMapAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@MondayFridayOpen", dtMonFriOpen.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@MondayFridayClose", dtMonFriClose.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@SaturdayOpen", dtSaturdayOpen.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@SaturdayClose", dtSaturdayClose.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@SundayClosed", true);
                cmd.Parameters.AddWithValue("@GuestBrowsingEnabled", guestBrowsingEnabled);
                cmd.Parameters.AddWithValue("@DefaultLoanDays", trackLoanDays.Value);

                int maxBooks = GetSelectedMaxBooks();
                if (maxBooks <= 0)
                    cmd.Parameters.AddWithValue("@MaxBooksPerUser", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@MaxBooksPerUser", maxBooks);

                int affected = cmd.ExecuteNonQuery();

                if (affected <= 0)
                    throw new InvalidOperationException("No client library record was updated. Please check the current ClientID.");

                SaveSystemConfigurationMirror(conn, clientId);
                UpdateMapBadge();

                MessageBox.Show(
                    "Library setup saved successfully. Super Admin View Clients will reflect the updated library name and email.",
                    "Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Library setup could not be saved.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void SaveSystemConfigurationMirror(SqlConnection conn, int clientId)
        {
            const string ensureConfig = @"
IF OBJECT_ID('dbo.SystemConfigurations', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SystemConfigurations WHERE ClientID = @ClientID)
    BEGIN
        INSERT INTO dbo.SystemConfigurations
        (
            ClientID,
            AdminFullAccess,
            StaffCanChangeRules,
            StudentFacultyAccess,
            GuestReadAccess,
            DailyLateFee,
            MaximumTotalFee,
            LostBookFee,
            AllowAutoRenewals,
            BlockOutstandingFees,
            CrossCampusBorrowing,
            CreatedAt,
            UpdatedAt
        )
        VALUES
        (
            @ClientID,
            1,
            1,
            1,
            1,
            2.50,
            100.00,
            500.00,
            1,
            0,
            0,
            SYSUTCDATETIME(),
            SYSUTCDATETIME()
        );
    END
END;";

            using (SqlCommand ensureCmd = new SqlCommand(ensureConfig, conn))
            {
                ensureCmd.Parameters.AddWithValue("@ClientID", clientId);
                ensureCmd.ExecuteNonQuery();
            }

            const string updateConfig = @"
IF OBJECT_ID('dbo.SystemConfigurations', 'U') IS NOT NULL
BEGIN
    UPDATE dbo.SystemConfigurations
    SET
        LibraryHoursMonFri = @LibraryHoursMonFri,
        LibraryHoursSaturday = @LibraryHoursSaturday,
        LibraryHoursSunday = @LibraryHoursSunday,
        LibraryBranchName = @LibraryBranchName,
        LibraryBranchLocation = @LibraryBranchLocation,
        DailyLateFee = ISNULL(DailyLateFee, 2.50),
        MaximumTotalFee = ISNULL(MaximumTotalFee, 100.00),
        UpdatedAt = SYSUTCDATETIME()
    WHERE ClientID = @ClientID;
END;";

            using SqlCommand cmd = new SqlCommand(updateConfig, conn);
            cmd.Parameters.AddWithValue("@ClientID", clientId);
            cmd.Parameters.AddWithValue("@LibraryHoursMonFri", FormatTimeRange(dtMonFriOpen, dtMonFriClose));
            cmd.Parameters.AddWithValue("@LibraryHoursSaturday", FormatTimeRange(dtSaturdayOpen, dtSaturdayClose));
            cmd.Parameters.AddWithValue("@LibraryHoursSunday", "Closed");
            cmd.Parameters.AddWithValue("@LibraryBranchName", string.IsNullOrWhiteSpace(txtMapTitle.Text) ? "Main Library Branch" : txtMapTitle.Text.Trim());
            cmd.Parameters.AddWithValue("@LibraryBranchLocation", string.IsNullOrWhiteSpace(txtMapAddress.Text) ? "Davao City, Davao del Sur, Philippines" : txtMapAddress.Text.Trim());
            cmd.ExecuteNonQuery();
        }

        private string FormatTimeRange(DateTimePicker openPicker, DateTimePicker closePicker)
        {
            return $"{openPicker.Value:HH:mm} - {closePicker.Value:HH:mm}";
        }


        private void DrawMapTexture(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel p) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using SolidBrush bg = new SolidBrush(ColorTranslator.FromHtml("#E7EFF1"));
            e.Graphics.FillRectangle(bg, p.ClientRectangle);

            using Pen grid = new Pen(Color.FromArgb(38, 255, 255, 255), 1);
            for (int x = -40; x < p.Width; x += 44)
                e.Graphics.DrawLine(grid, x, 0, x + 70, p.Height);

            for (int y = 8; y < p.Height; y += 40)
                e.Graphics.DrawLine(grid, 0, y, p.Width, y + 14);

            Rectangle mapArea = new Rectangle(
                Math.Max(20, p.Width / 2 - 150),
                42,
                Math.Min(300, p.Width - 40),
                Math.Max(250, p.Height - 172)
            );

            DrawPhilippinesMap(e.Graphics, mapArea);

            PointF marker = GetPhilippineMarkerPoint(mapArea);
            DrawLocationPin(e.Graphics, marker);

            string address = string.IsNullOrWhiteSpace(txtMapAddress?.Text)
                ? "Davao City, Davao del Sur, Philippines"
                : txtMapAddress.Text.Trim();

            using Font addressFont = new Font("Segoe UI", 8.8F, FontStyle.Bold);
            using SolidBrush addressBrush = new SolidBrush(Color.FromArgb(132, 22, 29, 31));

            SizeF addressSize = e.Graphics.MeasureString(address, addressFont);
            float xAddress = Math.Max(16, Math.Min(p.Width - addressSize.Width - 16, marker.X - addressSize.Width / 2));
            float yAddress = Math.Min(p.Height - 118, marker.Y + 28);
            e.Graphics.DrawString(address, addressFont, addressBrush, xAddress, yAddress);
        }

        private void DrawPhilippinesMap(Graphics g, Rectangle area)
        {
            Color islandFill = Color.FromArgb(165, 181, 235, 221);
            Color islandAccent = Color.FromArgb(205, 109, 250, 210);
            Color outline = Color.FromArgb(210, 0, 107, 85);

            using SolidBrush fill = new SolidBrush(islandFill);
            using SolidBrush accent = new SolidBrush(islandAccent);
            using Pen pen = new Pen(outline, 2.1F);

            PointF luzon = new PointF(area.Left + area.Width * 0.47F, area.Top + area.Height * 0.17F);
            PointF mindoro = new PointF(area.Left + area.Width * 0.52F, area.Top + area.Height * 0.34F);
            PointF visayas = new PointF(area.Left + area.Width * 0.54F, area.Top + area.Height * 0.47F);
            PointF mindanao = new PointF(area.Left + area.Width * 0.51F, area.Top + area.Height * 0.72F);

            DrawIsland(g, luzon, 42, 88, -16, accent, pen);
            DrawIsland(g, new PointF(luzon.X + 28, luzon.Y + 75), 24, 54, 7, fill, pen);
            DrawIsland(g, mindoro, 36, 30, 4, fill, pen);
            DrawIsland(g, visayas, 78, 34, 4, accent, pen);
            DrawIsland(g, new PointF(visayas.X + 55, visayas.Y + 20), 34, 24, 12, fill, pen);
            DrawIsland(g, mindanao, 98, 62, -8, accent, pen);

            using Font font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            using SolidBrush label = new SolidBrush(Color.FromArgb(105, 60, 74, 70));

            g.DrawString("LUZON", font, label, luzon.X - 24, luzon.Y - 61);
            g.DrawString("VISAYAS", font, label, visayas.X + 46, visayas.Y - 4);
            g.DrawString("MINDANAO", font, label, mindanao.X - 38, mindanao.Y + 48);
        }

        private void DrawIsland(Graphics g, PointF center, int width, int height, float angle, Brush fill, Pen pen)
        {
            using System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddClosedCurve(new PointF[]
            {
                new PointF(center.X - width * 0.42F, center.Y - height * 0.16F),
                new PointF(center.X - width * 0.12F, center.Y - height * 0.50F),
                new PointF(center.X + width * 0.30F, center.Y - height * 0.36F),
                new PointF(center.X + width * 0.44F, center.Y + height * 0.08F),
                new PointF(center.X + width * 0.12F, center.Y + height * 0.46F),
                new PointF(center.X - width * 0.34F, center.Y + height * 0.30F)
            }, 0.55F);

            using System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
            matrix.RotateAt(angle, center);
            path.Transform(matrix);

            g.FillPath(fill, path);
            g.DrawPath(pen, path);
        }

        private PointF GetPhilippineMarkerPoint(Rectangle area)
        {
            string address = (txtMapAddress?.Text ?? "").ToLowerInvariant();

            if (address.Contains("davao") || address.Contains("mindanao") || address.Contains("zamboanga") ||
                address.Contains("cotabato") || address.Contains("butuan") || address.Contains("cagayan de oro"))
            {
                return new PointF(area.Left + area.Width * 0.56F, area.Top + area.Height * 0.73F);
            }

            if (address.Contains("cebu") || address.Contains("iloilo") || address.Contains("bacolod") ||
                address.Contains("bohol") || address.Contains("leyte") || address.Contains("visayas"))
            {
                return new PointF(area.Left + area.Width * 0.60F, area.Top + area.Height * 0.49F);
            }

            if (address.Contains("manila") || address.Contains("quezon") || address.Contains("makati") ||
                address.Contains("pasig") || address.Contains("baguio") || address.Contains("luzon") ||
                address.Contains("cavite") || address.Contains("laguna") || address.Contains("bulacan"))
            {
                return new PointF(area.Left + area.Width * 0.49F, area.Top + area.Height * 0.22F);
            }

            return new PointF(area.Left + area.Width * 0.56F, area.Top + area.Height * 0.73F);
        }

        private void DrawLocationPin(Graphics g, PointF point)
        {
            using SolidBrush glow = new SolidBrush(Color.FromArgb(76, 109, 250, 210));
            using SolidBrush outer = new SolidBrush(AccentEmerald);
            using SolidBrush inner = new SolidBrush(AccentDeep);
            using Pen ring = new Pen(Color.White, 2);

            g.FillEllipse(glow, point.X - 30, point.Y - 30, 60, 60);
            g.FillEllipse(outer, point.X - 11, point.Y - 11, 22, 22);
            g.FillEllipse(inner, point.X - 5, point.Y - 5, 10, 10);
            g.DrawEllipse(ring, point.X - 11, point.Y - 11, 22, 22);
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