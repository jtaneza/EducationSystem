using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class SettingsForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color SurfaceLowest = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#E2E9EC");
        private readonly Color SurfaceVariant = ColorTranslator.FromHtml("#DDE4E6");

        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color PrimaryFixed = ColorTranslator.FromHtml("#6DFAD2");

        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");

        private readonly Color Tertiary = ColorTranslator.FromHtml("#A03F30");
        private readonly Color TertiaryContainer = ColorTranslator.FromHtml("#F7816D");
        private readonly Color ErrorContainer = ColorTranslator.FromHtml("#FFDAD6");

        private readonly List<ModuleCard> moduleCards = new();

        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Panel canvasPanel = null!;

        public SettingsForm()
        {
            InitializeComponent();
            BuildModulesUI();
        }

        private void BuildModulesUI()
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

            lblTitle = new Label
            {
                Text = "System Modules",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubtitle = new Label
            {
                Text = "Enable or disable features across the entire platform.",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            canvasPanel.Controls.Add(lblTitle);
            canvasPanel.Controls.Add(lblSubtitle);

            moduleCards.Add(CreateModuleCard(
                "Dark Mode Theme",
                "Allow users to switch to a darker interface for comfort.",
                "🌙",
                ColorTranslator.FromHtml("#0E1A44"),
                PrimaryFixed,
                true,
                "STATUS: ACTIVE",
                Color.FromArgb(30, 180, 110),
                true));

            moduleCards.Add(CreateModuleCard(
                "Email Alerts",
                "Send automatic messages for due dates and new books.",
                "✉",
                Color.FromArgb(220, 245, 235),
                Primary,
                true,
                "STATUS: ACTIVE",
                Color.FromArgb(30, 180, 110),
                false));

            moduleCards.Add(CreateModuleCard(
                "Advanced Search",
                "Enable complex filters for title, author, and ISBN.",
                "⌕",
                SurfaceHigh,
                OnSurface,
                true,
                "STATUS: ACTIVE",
                Color.FromArgb(30, 180, 110),
                false));

            moduleCards.Add(CreateModuleCard(
                "Maintenance Mode",
                "Pause user access for system updates.",
                "⚙",
                Color.FromArgb(255, 242, 238),
                Tertiary,
                false,
                "STATUS: DISABLED",
                Color.FromArgb(140, 150, 160),
                false));

            moduleCards.Add(CreateModuleCard(
                "Member Ratings",
                "Let students rate and review books they have read.",
                "★",
                Color.FromArgb(210, 245, 232),
                Primary,
                true,
                "STATUS: ACTIVE",
                Color.FromArgb(30, 180, 110),
                false));

            moduleCards.Add(CreateModuleCard(
                "Digital Archive",
                "Enable the storage and recovery of old records.",
                "🗃",
                Color.FromArgb(230, 250, 242),
                Color.FromArgb(25, 140, 95),
                false,
                "STATUS: PENDING CONFIG",
                Color.FromArgb(120, 130, 150),
                false));

            foreach (var card in moduleCards)
                canvasPanel.Controls.Add(card.Container);

            Resize += SettingsForm_Resize;
            AdjustLayout();
        }

        private ModuleCard CreateModuleCard(
            string title,
            string description,
            string iconText,
            Color iconBack,
            Color iconFore,
            bool enabled,
            string statusText,
            Color statusFore,
            bool neonGlow)
        {
            Panel card = new Panel
            {
                BackColor = SurfaceLowest,
                BorderStyle = BorderStyle.FixedSingle
            };

            if (neonGlow)
                card.Paint += (s, e) =>
                {
                    using Pen p = new Pen(Color.FromArgb(25, 109, 250, 210), 1);
                    e.Graphics.DrawRectangle(p, 0, 0, card.Width - 1, card.Height - 1);
                };

            Panel iconPanel = new Panel
            {
                Size = new Size(52, 52),
                BackColor = iconBack
            };

            Label iconLabel = new Label
            {
                Text = iconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 18F),
                ForeColor = iconFore
            };
            iconPanel.Controls.Add(iconLabel);

            ToggleSwitch toggle = new ToggleSwitch
            {
                IsOn = enabled,
                OnColor = PrimaryContainer,
                OffColor = Color.FromArgb(215, 221, 225),
                KnobColor = Color.White
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            Label descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                ForeColor = OnSurfaceVariant,
                AutoSize = false
            };

            Panel statusDot = new Panel
            {
                Size = new Size(8, 8),
                BackColor = statusFore
            };
            statusDot.Paint += (s, e) =>
            {
                using SolidBrush b = new SolidBrush(statusFore);
                e.Graphics.FillEllipse(b, 0, 0, statusDot.Width - 1, statusDot.Height - 1);
            };

            Label statusLabel = new Label
            {
                Text = statusText,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = statusFore,
                AutoSize = true
            };

            card.Controls.Add(iconPanel);
            card.Controls.Add(toggle);
            card.Controls.Add(titleLabel);
            card.Controls.Add(descLabel);
            card.Controls.Add(statusDot);
            card.Controls.Add(statusLabel);

            return new ModuleCard
            {
                Container = card,
                IconPanel = iconPanel,
                Toggle = toggle,
                TitleLabel = titleLabel,
                DescriptionLabel = descLabel,
                StatusDot = statusDot,
                StatusLabel = statusLabel
            };
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 28;
            int width = ClientSize.Width - (margin * 2);

            lblTitle.Location = new Point(margin, 34);
            lblSubtitle.Location = new Point(margin, 82);

            int top = lblSubtitle.Bottom + 34;

            if (width >= 1120)
            {
                int cardWidth = (width - (gap * 2)) / 3;
                int cardHeight = 278;

                for (int i = 0; i < moduleCards.Count; i++)
                {
                    int row = i / 3;
                    int col = i % 3;

                    int x = margin + (col * (cardWidth + gap));
                    int y = top + (row * (cardHeight + 34));

                    moduleCards[i].Container.Bounds = new Rectangle(x, y, cardWidth, cardHeight);
                    LayoutModuleCard(moduleCards[i]);
                }

                canvasPanel.AutoScrollMinSize = new Size(0, top + (2 * (278 + 34)) + 24);
            }
            else if (width >= 760)
            {
                int cardWidth = (width - gap) / 2;
                int cardHeight = 278;

                for (int i = 0; i < moduleCards.Count; i++)
                {
                    int row = i / 2;
                    int col = i % 2;

                    int x = margin + (col * (cardWidth + gap));
                    int y = top + (row * (cardHeight + 28));

                    moduleCards[i].Container.Bounds = new Rectangle(x, y, cardWidth, cardHeight);
                    LayoutModuleCard(moduleCards[i]);
                }

                canvasPanel.AutoScrollMinSize = new Size(0, top + (3 * (278 + 28)) + 24);
            }
            else
            {
                int cardWidth = width;
                int cardHeight = 278;

                for (int i = 0; i < moduleCards.Count; i++)
                {
                    int y = top + (i * (cardHeight + 24));
                    moduleCards[i].Container.Bounds = new Rectangle(margin, y, cardWidth, cardHeight);
                    LayoutModuleCard(moduleCards[i]);
                }

                canvasPanel.AutoScrollMinSize = new Size(0, top + (moduleCards.Count * (278 + 24)) + 24);
            }
        }

        private void LayoutModuleCard(ModuleCard card)
        {
            card.IconPanel.Location = new Point(34, 34);
            card.Toggle.Location = new Point(card.Container.Width - 96, 34);

            card.TitleLabel.Location = new Point(34, 126);
            card.DescriptionLabel.Location = new Point(34, 168);
            card.DescriptionLabel.Size = new Size(card.Container.Width - 68, 62);

            card.StatusDot.Location = new Point(36, card.Container.Height - 44);
            card.StatusLabel.Location = new Point(card.StatusDot.Right + 10, card.Container.Height - 49);
        }

        private void SettingsForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private sealed class ModuleCard
        {
            public Panel Container = null!;
            public Panel IconPanel = null!;
            public ToggleSwitch Toggle = null!;
            public Label TitleLabel = null!;
            public Label DescriptionLabel = null!;
            public Panel StatusDot = null!;
            public Label StatusLabel = null!;
        }
        public void OpenPasswordSection()
        {
            MessageBox.Show(
                "Password settings are not available in System Modules.",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private sealed class ToggleSwitch : Control
        {
            public bool IsOn { get; set; }
            public Color OnColor { get; set; } = Color.MediumSeaGreen;
            public Color OffColor { get; set; } = Color.LightGray;
            public Color KnobColor { get; set; } = Color.White;

            public ToggleSwitch()
            {
                Size = new Size(62, 32);
                Cursor = Cursors.Hand;
                DoubleBuffered = true;
                Click += (s, e) =>
                {
                    IsOn = !IsOn;
                    Invalidate();
                };
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                Rectangle track = new Rectangle(0, 2, Width - 1, Height - 5);
                using (SolidBrush brush = new SolidBrush(IsOn ? OnColor : OffColor))
                    e.Graphics.FillRoundedRectangle(brush, track, 15);

                int knobX = IsOn ? Width - 30 : 2;
                Rectangle knob = new Rectangle(knobX, 2, 28, 28);

                using (SolidBrush knobBrush = new SolidBrush(KnobColor))
                    e.Graphics.FillEllipse(knobBrush, knob);
            }
        }
    }

    internal static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            using System.Drawing.Drawing2D.GraphicsPath path = new();
            int d = radius * 2;

            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            g.FillPath(brush, path);
        }
    }
}