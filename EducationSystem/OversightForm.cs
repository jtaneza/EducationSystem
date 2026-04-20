using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class OversightForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color SurfaceLowest = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#E2E9EC");
        private readonly Color SurfaceHighest = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color OutlineVariant = ColorTranslator.FromHtml("#BBCAC3");

        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color OnPrimaryContainer = ColorTranslator.FromHtml("#004233");
        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");

        private Panel contentPanel = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;

        private Panel userAccessPanel = null!;
        private Panel softwarePanel = null!;
        private Panel hardwarePanel = null!;
        private Panel architecturePanel = null!;
        private Panel feesPanel = null!;
        private Panel policyPanel = null!;
        private Panel maintenancePanel = null!;
        private Panel footerActionsPanel = null!;

        private CheckBox chkAdminFull = null!;
        private CheckBox chkStaffRules = null!;
        private CheckBox chkStudentFaculty = null!;
        private CheckBox chkGuestRead = null!;

        private NumericUpDown nudDailyFee = null!;
        private NumericUpDown nudMaxFee = null!;
        private NumericUpDown nudLostBookFee = null!;

        private CheckBox chkAutoRenew = null!;
        private CheckBox chkBlockOutstanding = null!;
        private CheckBox chkCrossCampus = null!;

        private Button btnAdjustFee = null!;
        private Button btnExportConfig = null!;
        private Button btnRestoreBackup = null!;
        private Button btnRunUpdates = null!;
        private Button btnDiscard = null!;
        private Button btnApply = null!;

        private Panel swBox1 = null!;
        private Panel swBox2 = null!;
        private Panel swBox3 = null!;
        private Panel swBox4 = null!;

        private Panel module1 = null!;
        private Panel module2 = null!;
        private Panel module3 = null!;
        private Panel module4 = null!;

        private Label archChip1 = null!;
        private Label archChip2 = null!;
        private Label archChip3 = null!;
        private Label rolesText = null!;

        public OversightForm()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                BuildUI();
                Resize += OversightForm_Resize;
                AdjustLayout();
            }
        }

        private void BuildUI()
        {
            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = true;

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background,
                AutoScroll = true
            };

            lblTitle = new Label
            {
                Text = "System Configuration",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubtitle = new Label
            {
                Text = "Manage core institutional rules and LibraFlow LMS technical protocols.",
                Font = new Font("Segoe UI", 11F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            userAccessPanel = CreateCardPanel();
            softwarePanel = CreateCardPanel();
            hardwarePanel = CreateCardPanel();
            architecturePanel = CreateCardPanel();
            feesPanel = CreateCardPanel();
            policyPanel = CreateCardPanel();
            maintenancePanel = CreateCardPanel();

            footerActionsPanel = new Panel
            {
                BackColor = Background
            };

            BuildUserAccessPanel();
            BuildSoftwarePanel();
            BuildHardwarePanel();
            BuildArchitecturePanel();
            BuildFeesPanel();
            BuildPolicyPanel();
            BuildMaintenancePanel();
            BuildFooterActionsPanel();

            contentPanel.Controls.Add(lblTitle);
            contentPanel.Controls.Add(lblSubtitle);
            contentPanel.Controls.Add(userAccessPanel);
            contentPanel.Controls.Add(softwarePanel);
            contentPanel.Controls.Add(hardwarePanel);
            contentPanel.Controls.Add(architecturePanel);
            contentPanel.Controls.Add(feesPanel);
            contentPanel.Controls.Add(policyPanel);
            contentPanel.Controls.Add(maintenancePanel);
            contentPanel.Controls.Add(footerActionsPanel);

            Controls.Add(contentPanel);
        }

        private Panel CreateCardPanel()
        {
            Panel panel = new Panel
            {
                BackColor = SurfaceLowest,
                BorderStyle = BorderStyle.None
            };

            panel.Paint += (s, e) =>
            {
                using Pen pen = new Pen(Color.FromArgb(38, OutlineVariant), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            };

            return panel;
        }

        private Panel CreateModulePanel()
        {
            Panel panel = new Panel
            {
                BackColor = SurfaceLowest,
                BorderStyle = BorderStyle.None
            };

            panel.Paint += (s, e) =>
            {
                using Pen pen = new Pen(Color.FromArgb(55, OutlineVariant), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            };

            return panel;
        }

        private Panel CreateSoftInfoBox(string title, string value, bool addLeftAccent = false)
        {
            Panel box = new Panel
            {
                BackColor = SurfaceLow,
                BorderStyle = BorderStyle.None
            };

            box.Paint += (s, e) =>
            {
                using Pen pen = new Pen(Color.FromArgb(28, OutlineVariant), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, box.Width - 1, box.Height - 1);

                if (addLeftAccent)
                {
                    using SolidBrush brush = new SolidBrush(Color.FromArgb(40, Primary));
                    e.Graphics.FillRectangle(brush, 0, 0, 4, box.Height);
                }
            };

            Label lblBoxTitle = new Label
            {
                Text = title.ToUpper(),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Primary,
                AutoSize = true,
                Location = new Point(16, 14)
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 11F, addLeftAccent ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(16, 42)
            };

            box.Controls.Add(lblBoxTitle);
            box.Controls.Add(lblValue);
            return box;
        }

        private Label CreateSectionTitle(string text, string icon)
        {
            return new Label
            {
                Text = $"{icon}  {text}",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };
        }

        private Label CreateMiniTitle(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };
        }

        private Label CreateChip(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                BackColor = SecondaryContainer,
                ForeColor = ColorTranslator.FromHtml("#3B6B5C"),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };
        }

        private CheckBox CreateConfigCheckBox(string text, bool isChecked)
        {
            return new CheckBox
            {
                Text = text,
                Checked = isChecked,
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = OnSurfaceVariant,
                BackColor = SurfaceLowest
            };
        }

        private CheckBox CreatePolicyCheckBox(string text, bool isChecked)
        {
            return new CheckBox
            {
                Text = text,
                Checked = isChecked,
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = OnSurfaceVariant,
                BackColor = SurfaceLowest
            };
        }

        private NumericUpDown CreateStyledNumeric(decimal value, decimal increment, int decimals)
        {
            return new NumericUpDown
            {
                DecimalPlaces = decimals,
                Increment = increment,
                Value = value,
                Minimum = 0,
                Maximum = 1000,
                Size = new Size(230, 40),
                Font = new Font("Segoe UI", 11F),
                BackColor = SurfaceHigh,
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = OnSurface
            };
        }

        private void StyleFlatButton(Button btn, Color back, Color fore)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = back;
            btn.ForeColor = fore;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void BuildUserAccessPanel()
        {
            Label title = CreateSectionTitle("User Access", "🛡");

            chkAdminFull = CreateConfigCheckBox("Admin Full Access", true);
            chkStaffRules = CreateConfigCheckBox("Staff can change rules", true);
            chkStudentFaculty = CreateConfigCheckBox("Student/Faculty Access", false);
            chkGuestRead = CreateConfigCheckBox("Guest Access (Read)", false);

            userAccessPanel.Controls.Add(title);
            userAccessPanel.Controls.Add(chkAdminFull);
            userAccessPanel.Controls.Add(chkStaffRules);
            userAccessPanel.Controls.Add(chkStudentFaculty);
            userAccessPanel.Controls.Add(chkGuestRead);

            title.Location = new Point(22, 20);
            chkAdminFull.Location = new Point(22, 70);
            chkStaffRules.Location = new Point(22, 105);
            chkStudentFaculty.Location = new Point(22, 140);
            chkGuestRead.Location = new Point(22, 175);
        }

        private void BuildSoftwarePanel()
        {
            Label title = CreateSectionTitle("Software Requirements", "💻");

            swBox1 = CreateSoftInfoBox("Operating Systems", "Windows, macOS, Linux");
            swBox2 = CreateSoftInfoBox("Browsers", "Chrome, Firefox, Edge, Safari");
            swBox3 = CreateSoftInfoBox("Backend Environment", "C# / ASP.NET Core", true);
            swBox4 = CreateSoftInfoBox("Database Engine", "MonsterASP SQL Database", true);

            softwarePanel.Controls.Add(title);
            softwarePanel.Controls.Add(swBox1);
            softwarePanel.Controls.Add(swBox2);
            softwarePanel.Controls.Add(swBox3);
            softwarePanel.Controls.Add(swBox4);

            title.Location = new Point(22, 20);
        }

        private void BuildHardwarePanel()
        {
            Label title = CreateSectionTitle("Hardware Specs (Min)", "🧠");

            hardwarePanel.Controls.Add(title);
            title.Location = new Point(22, 20);

            AddSpecRow(hardwarePanel, "Processor", "Intel Core i3+", 72);
            AddSpecRow(hardwarePanel, "RAM", "2 GB or more", 112);
            AddSpecRow(hardwarePanel, "Storage", "10 GB or more", 152);
        }

        private void AddSpecRow(Panel parent, string label, string value, int top)
        {
            Label lblLeft = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(22, top)
            };

            Label lblRight = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(175, top)
            };

            Panel line = new Panel
            {
                BackColor = Color.FromArgb(22, OutlineVariant),
                Size = new Size(240, 1),
                Location = new Point(22, top + 28)
            };

            parent.Controls.Add(lblLeft);
            parent.Controls.Add(lblRight);
            parent.Controls.Add(line);
        }

        private void BuildArchitecturePanel()
        {
            Label title = CreateSectionTitle("Architecture & Core Modules", "✳");
            Label lblArch = CreateMiniTitle("SYSTEM ARCHITECTURE");
            Label lblRoles = CreateMiniTitle("USER ROLES");
            Label lblModules = CreateMiniTitle("KEY LIBRAFLOW MODULES");

            archChip1 = CreateChip("CLIENT-SERVER");
            archChip2 = CreateChip("ERP SAAS ARCHITECTURE");
            archChip3 = CreateChip("CLOUD");

            rolesText = new Label
            {
                Text = "Admin, Librarian, Student/Faculty",
                Font = new Font("Segoe UI", 10F),
                ForeColor = OnSurface,
                AutoSize = true
            };

            module1 = CreateModuleBox("OPAC", "CATALOG");
            module2 = CreateModuleBox("Circulation", "LENDING");
            module3 = CreateModuleBox("Members", "MANAGEMENT");
            module4 = CreateModuleBox("Reporting", "MIS/ANALYTICS");

            architecturePanel.Controls.Add(title);
            architecturePanel.Controls.Add(lblArch);
            architecturePanel.Controls.Add(lblRoles);
            architecturePanel.Controls.Add(lblModules);
            architecturePanel.Controls.Add(archChip1);
            architecturePanel.Controls.Add(archChip2);
            architecturePanel.Controls.Add(archChip3);
            architecturePanel.Controls.Add(rolesText);
            architecturePanel.Controls.Add(module1);
            architecturePanel.Controls.Add(module2);
            architecturePanel.Controls.Add(module3);
            architecturePanel.Controls.Add(module4);

            title.Location = new Point(22, 20);
            lblArch.Location = new Point(22, 72);
            lblRoles.Location = new Point(312, 72);
            rolesText.Location = new Point(312, 98);
            lblModules.Location = new Point(22, 164);
        }

        private Panel CreateModuleBox(string title, string subtitle)
        {
            Panel box = CreateModulePanel();

            Label lbl1 = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Primary,
                AutoSize = true,
                Location = new Point(14, 10)
            };

            Label lbl2 = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 8F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(14, 30)
            };

            box.Controls.Add(lbl1);
            box.Controls.Add(lbl2);
            return box;
        }

        private void BuildFeesPanel()
        {
            Label title = CreateSectionTitle("Circulation Fees", "💵");

            Label lblDaily = new Label
            {
                Text = "Daily late fee ($)",
                Font = new Font("Segoe UI", 9F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            Label lblMax = new Label
            {
                Text = "Maximum total fee ($)",
                Font = new Font("Segoe UI", 9F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            Label lblLost = new Label
            {
                Text = "Fee for lost books ($)",
                Font = new Font("Segoe UI", 9F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            nudDailyFee = CreateStyledNumeric(1.50m, 0.25m, 2);
            nudMaxFee = CreateStyledNumeric(25.00m, 1.00m, 2);
            nudLostBookFee = CreateStyledNumeric(10.00m, 1.00m, 2);

            btnAdjustFee = new Button
            {
                Text = "✎  Adjust fee levels",
                AutoSize = true
            };
            StyleFlatButton(btnAdjustFee, SurfaceLowest, Primary);

            feesPanel.Controls.Add(title);
            feesPanel.Controls.Add(lblDaily);
            feesPanel.Controls.Add(lblMax);
            feesPanel.Controls.Add(lblLost);
            feesPanel.Controls.Add(nudDailyFee);
            feesPanel.Controls.Add(nudMaxFee);
            feesPanel.Controls.Add(nudLostBookFee);
            feesPanel.Controls.Add(btnAdjustFee);

            title.Location = new Point(22, 20);

            lblDaily.Location = new Point(22, 74);
            nudDailyFee.Location = new Point(22, 96);

            lblMax.Location = new Point(300, 74);
            nudMaxFee.Location = new Point(300, 96);

            lblLost.Location = new Point(22, 160);
            nudLostBookFee.Location = new Point(22, 182);

            btnAdjustFee.Location = new Point(300, 180);
        }

        private void BuildPolicyPanel()
        {
            Label title = CreateSectionTitle("System Policy", "⇄");

            chkAutoRenew = CreatePolicyCheckBox("Allow automatic renewals", true);
            chkBlockOutstanding = CreatePolicyCheckBox("Block accounts with outstanding fees", false);
            chkCrossCampus = CreatePolicyCheckBox("Cross-campus borrowing enabled", true);

            policyPanel.Controls.Add(title);
            policyPanel.Controls.Add(chkAutoRenew);
            policyPanel.Controls.Add(chkBlockOutstanding);
            policyPanel.Controls.Add(chkCrossCampus);

            title.Location = new Point(22, 20);
            chkAutoRenew.Location = new Point(22, 72);
            chkBlockOutstanding.Location = new Point(22, 116);
            chkCrossCampus.Location = new Point(22, 160);
        }

        private void BuildMaintenancePanel()
        {
            Label icon = new Label
            {
                Text = "🗄",
                Font = new Font("Segoe UI Emoji", 16F),
                ForeColor = Primary,
                AutoSize = true,
                Location = new Point(22, 22)
            };

            Label title = new Label
            {
                Text = "System Maintenance",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(56, 20)
            };

            Label subtitle = new Label
            {
                Text = "Configure automated backup and LibraFlow system synchronization cycles.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true,
                Location = new Point(56, 46)
            };

            btnExportConfig = new Button { Text = "↓  Export Config", Size = new Size(130, 56) };
            btnRestoreBackup = new Button { Text = "☁  Restore Backup", Size = new Size(142, 56) };
            btnRunUpdates = new Button { Text = "⟳  Run Updates", Size = new Size(150, 56) };

            StyleFlatButton(btnExportConfig, SurfaceLow, OnSurface);
            StyleFlatButton(btnRestoreBackup, SurfaceLow, OnSurface);
            StyleFlatButton(btnRunUpdates, Primary, Color.White);

            maintenancePanel.Controls.Add(icon);
            maintenancePanel.Controls.Add(title);
            maintenancePanel.Controls.Add(subtitle);
            maintenancePanel.Controls.Add(btnExportConfig);
            maintenancePanel.Controls.Add(btnRestoreBackup);
            maintenancePanel.Controls.Add(btnRunUpdates);
        }

        private void BuildFooterActionsPanel()
        {
            Panel line = new Panel
            {
                BackColor = Color.FromArgb(24, OutlineVariant),
                Dock = DockStyle.Top,
                Height = 1
            };

            btnDiscard = new Button
            {
                Text = "Discard Changes",
                Size = new Size(170, 48)
            };
            StyleFlatButton(btnDiscard, Background, OnSurfaceVariant);

            btnApply = new Button
            {
                Text = "💾  Apply Configuration",
                Size = new Size(250, 48)
            };
            StyleFlatButton(btnApply, PrimaryContainer, OnPrimaryContainer);

            footerActionsPanel.Controls.Add(line);
            footerActionsPanel.Controls.Add(btnDiscard);
            footerActionsPanel.Controls.Add(btnApply);
        }

        private void OversightForm_Resize(object? sender, EventArgs e)
        {
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int margin = 34;
            int gap = 22;
            int contentWidth = ClientSize.Width - (margin * 2);
            int y = 108;

            lblTitle.Location = new Point(margin, 24);
            lblSubtitle.Location = new Point(margin + 2, 66);

            if (contentWidth >= 1150)
            {
                int leftW = (contentWidth - gap) * 4 / 12;
                int rightW = contentWidth - gap - leftW;

                userAccessPanel.Bounds = new Rectangle(margin, y, leftW, 230);
                softwarePanel.Bounds = new Rectangle(userAccessPanel.Right + gap, y, rightW, 230);

                LayoutSoftwarePanelWide();

                int y2 = userAccessPanel.Bottom + gap;
                hardwarePanel.Bounds = new Rectangle(margin, y2, leftW, 250);
                architecturePanel.Bounds = new Rectangle(hardwarePanel.Right + gap, y2, rightW, 250);

                LayoutArchitectureWide();

                int y3 = hardwarePanel.Bottom + gap;
                int feesW = (contentWidth - gap) * 7 / 12;
                int policyW = contentWidth - gap - feesW;

                feesPanel.Bounds = new Rectangle(margin, y3, feesW, 242);
                policyPanel.Bounds = new Rectangle(feesPanel.Right + gap, y3, policyW, 242);

                LayoutFeesWide();

                int y4 = feesPanel.Bottom + gap;
                maintenancePanel.Bounds = new Rectangle(margin, y4, contentWidth, 102);

                btnExportConfig.Location = new Point(maintenancePanel.Width - 448, 22);
                btnRestoreBackup.Location = new Point(btnExportConfig.Right + 16, 22);
                btnRunUpdates.Location = new Point(btnRestoreBackup.Right + 16, 22);

                int y5 = maintenancePanel.Bottom + 18;
                footerActionsPanel.Bounds = new Rectangle(margin, y5, contentWidth, 92);

                btnApply.Location = new Point(footerActionsPanel.Width - btnApply.Width - 10, 22);
                btnDiscard.Location = new Point(btnApply.Left - btnDiscard.Width - 20, 22);
            }
            else
            {
                int fullW = contentWidth;

                userAccessPanel.Bounds = new Rectangle(margin, y, fullW, 230);

                int y2 = userAccessPanel.Bottom + gap;
                softwarePanel.Bounds = new Rectangle(margin, y2, fullW, 250);
                LayoutSoftwarePanelNarrow();

                int y3 = softwarePanel.Bottom + gap;
                hardwarePanel.Bounds = new Rectangle(margin, y3, fullW, 250);

                int y4 = hardwarePanel.Bottom + gap;
                architecturePanel.Bounds = new Rectangle(margin, y4, fullW, 300);
                LayoutArchitectureNarrow();

                int y5 = architecturePanel.Bottom + gap;
                feesPanel.Bounds = new Rectangle(margin, y5, fullW, 242);
                LayoutFeesNarrow();

                int y6 = feesPanel.Bottom + gap;
                policyPanel.Bounds = new Rectangle(margin, y6, fullW, 242);

                int y7 = policyPanel.Bottom + gap;
                maintenancePanel.Bounds = new Rectangle(margin, y7, fullW, 152);

                btnExportConfig.Location = new Point(22, 78);
                btnRestoreBackup.Location = new Point(btnExportConfig.Right + 16, 78);
                btnRunUpdates.Location = new Point(btnRestoreBackup.Right + 16, 78);

                int y8 = maintenancePanel.Bottom + 18;
                footerActionsPanel.Bounds = new Rectangle(margin, y8, fullW, 92);

                btnApply.Location = new Point(footerActionsPanel.Width - btnApply.Width - 10, 22);
                btnDiscard.Location = new Point(btnApply.Left - btnDiscard.Width - 20, 22);
            }

            contentPanel.AutoScrollMinSize = new Size(0, footerActionsPanel.Bottom + 32);
        }

        private void LayoutSoftwarePanelWide()
        {
            swBox1.Bounds = new Rectangle(22, 64, 252, 76);
            swBox2.Bounds = new Rectangle(300, 64, 252, 76);
            swBox3.Bounds = new Rectangle(22, 160, 252, 76);
            swBox4.Bounds = new Rectangle(300, 160, 252, 76);
        }

        private void LayoutSoftwarePanelNarrow()
        {
            int gap = 18;
            int boxW = (softwarePanel.Width - 22 - 22 - gap) / 2;

            swBox1.Bounds = new Rectangle(22, 64, boxW, 76);
            swBox2.Bounds = new Rectangle(swBox1.Right + gap, 64, boxW, 76);
            swBox3.Bounds = new Rectangle(22, 160, boxW, 76);
            swBox4.Bounds = new Rectangle(swBox3.Right + gap, 160, boxW, 76);
        }

        private void LayoutArchitectureWide()
        {
            archChip1.Location = new Point(22, 98);
            archChip2.Location = new Point(128, 98);
            archChip3.Location = new Point(22, 128);

            rolesText.Location = new Point(312, 98);

            module1.Bounds = new Rectangle(22, 192, 124, 56);
            module2.Bounds = new Rectangle(160, 192, 124, 56);
            module3.Bounds = new Rectangle(298, 192, 124, 56);
            module4.Bounds = new Rectangle(436, 192, 124, 56);
        }

        private void LayoutArchitectureNarrow()
        {
            archChip1.Location = new Point(22, 98);
            archChip2.Location = new Point(128, 98);
            archChip3.Location = new Point(22, 128);

            rolesText.Location = new Point(22, 170);

            int gap = 14;
            int moduleY = 220;
            int moduleW = (architecturePanel.Width - 22 - 22 - (gap * 3)) / 4;

            module1.Bounds = new Rectangle(22, moduleY, moduleW, 56);
            module2.Bounds = new Rectangle(module1.Right + gap, moduleY, moduleW, 56);
            module3.Bounds = new Rectangle(module2.Right + gap, moduleY, moduleW, 56);
            module4.Bounds = new Rectangle(module3.Right + gap, moduleY, moduleW, 56);
        }

        private void LayoutFeesWide()
        {
            nudDailyFee.Location = new Point(22, 96);
            nudMaxFee.Location = new Point(300, 96);
            nudLostBookFee.Location = new Point(22, 182);
            btnAdjustFee.Location = new Point(300, 180);
        }

        private void LayoutFeesNarrow()
        {
            nudDailyFee.Location = new Point(22, 96);
            nudMaxFee.Location = new Point(300, 96);
            nudLostBookFee.Location = new Point(22, 182);
            btnAdjustFee.Location = new Point(300, 180);
        }
    }
}