using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public partial class OversightForm : Form
    {
        private Label lblTitle = null!;
        private Label lblSubInfo = null!;

        private Panel mainPanel = null!;

        private GroupBox grpUserPermissions = null!;
        private CheckBox chkAdminManageLibrarians = null!;
        private CheckBox chkLibrarianManageMembers = null!;
        private CheckBox chkMemberViewOwnStatus = null!;

        private GroupBox grpFineSettings = null!;
        private Label lblFinePerDay = null!;
        private NumericUpDown numFinePerDay = null!;
        private Label lblMaxFine = null!;
        private NumericUpDown numMaxFine = null!;

        private GroupBox grpBorrowingLimits = null!;
        private Label lblStudentLimit = null!;
        private NumericUpDown numStudentLimit = null!;
        private Label lblTeacherLimit = null!;
        private NumericUpDown numTeacherLimit = null!;
        private Label lblBorrowDays = null!;
        private NumericUpDown numBorrowDays = null!;

        private GroupBox grpCirculationRules = null!;
        private CheckBox chkAllowRenewal = null!;
        private Label lblRenewalLimit = null!;
        private NumericUpDown numRenewalLimit = null!;
        private CheckBox chkBlockOverdueBorrowing = null!;

        private GroupBox grpDataMaintenance = null!;
        private Button btnBackup = null!;
        private Button btnClearLogs = null!;
        private Button btnResetDemo = null!;

        private Button btnSave = null!;

        public OversightForm()
        {
            InitializeComponent();
            BuildUI();
            Resize += OversightForm_Resize;
        }

        private void BuildUI()
        {
            BackColor = Color.Snow;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            AutoScroll = true;

            lblTitle = new Label
            {
                Text = "System Configuration",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.Maroon,
                AutoSize = true,
                Location = new Point(24, 20)
            };

            lblSubInfo = new Label
            {
                Text = "Manage permissions, fine settings, borrowing limits, circulation rules, and maintenance tools.",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(26, 52)
            };

            mainPanel = new Panel
            {
                BackColor = Color.Snow,
                Location = new Point(24, 85),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = false
            };

            BuildUserPermissionsGroup();
            BuildFineSettingsGroup();
            BuildBorrowingLimitsGroup();
            BuildCirculationRulesGroup();
            BuildDataMaintenanceGroup();
            BuildSaveButton();

            mainPanel.Controls.Add(grpUserPermissions);
            mainPanel.Controls.Add(grpFineSettings);
            mainPanel.Controls.Add(grpBorrowingLimits);
            mainPanel.Controls.Add(grpCirculationRules);
            mainPanel.Controls.Add(grpDataMaintenance);
            mainPanel.Controls.Add(btnSave);

            Controls.Add(lblTitle);
            Controls.Add(lblSubInfo);
            Controls.Add(mainPanel);

            ApplyResponsiveLayout();
        }

        private void OversightForm_Resize(object? sender, EventArgs e)
        {
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            int margin = 24;
            int topY = 85;
            int contentWidth = Math.Max(760, ClientSize.Width - (margin * 2));

            mainPanel.Location = new Point(margin, topY);
            mainPanel.Size = new Size(contentWidth, 760);

            bool singleColumn = contentWidth < 950;

            int gap = 20;
            int groupHeightSmall = 150;
            int groupHeightMedium = 180;

            if (!singleColumn)
            {
                int halfWidth = (contentWidth - gap) / 2;

                grpUserPermissions.Location = new Point(0, 0);
                grpUserPermissions.Size = new Size(halfWidth, groupHeightSmall);

                grpFineSettings.Location = new Point(halfWidth + gap, 0);
                grpFineSettings.Size = new Size(halfWidth, groupHeightSmall);

                grpBorrowingLimits.Location = new Point(0, groupHeightSmall + gap);
                grpBorrowingLimits.Size = new Size(halfWidth, groupHeightMedium);

                grpCirculationRules.Location = new Point(halfWidth + gap, groupHeightSmall + gap);
                grpCirculationRules.Size = new Size(halfWidth, groupHeightMedium);

                grpDataMaintenance.Location = new Point(0, groupHeightSmall + groupHeightMedium + (gap * 2));
                grpDataMaintenance.Size = new Size(contentWidth, 140);

                btnSave.Location = new Point(contentWidth - btnSave.Width, grpDataMaintenance.Bottom + 18);
            }
            else
            {
                int fullWidth = contentWidth;
                int y = 0;

                grpUserPermissions.Location = new Point(0, y);
                grpUserPermissions.Size = new Size(fullWidth, groupHeightSmall);
                y += grpUserPermissions.Height + gap;

                grpFineSettings.Location = new Point(0, y);
                grpFineSettings.Size = new Size(fullWidth, groupHeightSmall);
                y += grpFineSettings.Height + gap;

                grpBorrowingLimits.Location = new Point(0, y);
                grpBorrowingLimits.Size = new Size(fullWidth, groupHeightMedium);
                y += grpBorrowingLimits.Height + gap;

                grpCirculationRules.Location = new Point(0, y);
                grpCirculationRules.Size = new Size(fullWidth, groupHeightMedium);
                y += grpCirculationRules.Height + gap;

                grpDataMaintenance.Location = new Point(0, y);
                grpDataMaintenance.Size = new Size(fullWidth, 160);
                y += grpDataMaintenance.Height + gap;

                btnSave.Location = new Point(fullWidth - btnSave.Width, y);
            }

            LayoutMaintenanceButtons();
        }

        private void LayoutMaintenanceButtons()
        {
            int padding = 20;
            int gap = 16;
            int availableWidth = grpDataMaintenance.ClientSize.Width - (padding * 2);

            if (availableWidth >= 520)
            {
                btnBackup.Location = new Point(padding, 45);
                btnClearLogs.Location = new Point(btnBackup.Right + gap, 45);
                btnResetDemo.Location = new Point(btnClearLogs.Right + gap, 45);
            }
            else
            {
                btnBackup.Location = new Point(padding, 35);
                btnClearLogs.Location = new Point(padding, btnBackup.Bottom + 12);
                btnResetDemo.Location = new Point(padding, btnClearLogs.Bottom + 12);
            }
        }

        private void BuildUserPermissionsGroup()
        {
            grpUserPermissions = CreateGroupBox("User Permissions");

            chkAdminManageLibrarians = new CheckBox
            {
                Text = "Client/Admin can manage librarians",
                Location = new Point(20, 35),
                AutoSize = true,
                Checked = true
            };

            chkLibrarianManageMembers = new CheckBox
            {
                Text = "Librarian can manage members",
                Location = new Point(20, 65),
                AutoSize = true,
                Checked = true
            };

            chkMemberViewOwnStatus = new CheckBox
            {
                Text = "Member can view own borrowing status only",
                Location = new Point(20, 95),
                AutoSize = true,
                Checked = true
            };

            grpUserPermissions.Controls.Add(chkAdminManageLibrarians);
            grpUserPermissions.Controls.Add(chkLibrarianManageMembers);
            grpUserPermissions.Controls.Add(chkMemberViewOwnStatus);
        }

        private void BuildFineSettingsGroup()
        {
            grpFineSettings = CreateGroupBox("Fine Penalty Settings");

            lblFinePerDay = CreateLabel("Fine per day:", new Point(20, 38));
            numFinePerDay = CreateNumeric(new Point(140, 35), 1, 1000, 5);

            lblMaxFine = CreateLabel("Maximum fine:", new Point(20, 78));
            numMaxFine = CreateNumeric(new Point(140, 75), 1, 10000, 100);

            grpFineSettings.Controls.Add(lblFinePerDay);
            grpFineSettings.Controls.Add(numFinePerDay);
            grpFineSettings.Controls.Add(lblMaxFine);
            grpFineSettings.Controls.Add(numMaxFine);
        }

        private void BuildBorrowingLimitsGroup()
        {
            grpBorrowingLimits = CreateGroupBox("Borrowing Limits");

            lblStudentLimit = CreateLabel("Student max books:", new Point(20, 38));
            numStudentLimit = CreateNumeric(new Point(170, 35), 1, 20, 3);

            lblTeacherLimit = CreateLabel("Teacher max books:", new Point(20, 78));
            numTeacherLimit = CreateNumeric(new Point(170, 75), 1, 20, 5);

            lblBorrowDays = CreateLabel("Borrow duration (days):", new Point(20, 118));
            numBorrowDays = CreateNumeric(new Point(170, 115), 1, 60, 7);

            grpBorrowingLimits.Controls.Add(lblStudentLimit);
            grpBorrowingLimits.Controls.Add(numStudentLimit);
            grpBorrowingLimits.Controls.Add(lblTeacherLimit);
            grpBorrowingLimits.Controls.Add(numTeacherLimit);
            grpBorrowingLimits.Controls.Add(lblBorrowDays);
            grpBorrowingLimits.Controls.Add(numBorrowDays);
        }

        private void BuildCirculationRulesGroup()
        {
            grpCirculationRules = CreateGroupBox("Circulation Rules");

            chkAllowRenewal = new CheckBox
            {
                Text = "Allow book renewal",
                Location = new Point(20, 35),
                AutoSize = true,
                Checked = true
            };

            lblRenewalLimit = CreateLabel("Renewal limit:", new Point(20, 75));
            numRenewalLimit = CreateNumeric(new Point(140, 72), 0, 10, 2);

            chkBlockOverdueBorrowing = new CheckBox
            {
                Text = "Block borrowing when account has overdue items",
                Location = new Point(20, 115),
                AutoSize = true,
                Checked = true
            };

            grpCirculationRules.Controls.Add(chkAllowRenewal);
            grpCirculationRules.Controls.Add(lblRenewalLimit);
            grpCirculationRules.Controls.Add(numRenewalLimit);
            grpCirculationRules.Controls.Add(chkBlockOverdueBorrowing);
        }

        private void BuildDataMaintenanceGroup()
        {
            grpDataMaintenance = CreateGroupBox("Data Maintenance");

            btnBackup = CreateActionButton("Backup Database", new Size(130, 36));
            btnBackup.Click += (s, e) =>
                MessageBox.Show("Database backup process started.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnClearLogs = CreateActionButton("Clear Old Logs", new Size(130, 36));
            btnClearLogs.Click += (s, e) =>
                MessageBox.Show("Old logs cleaned successfully.", "Data Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnResetDemo = CreateActionButton("Reset Demo Data", new Size(130, 36));
            btnResetDemo.Click += (s, e) =>
                MessageBox.Show("Demo data reset completed.", "Data Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            grpDataMaintenance.Controls.Add(btnBackup);
            grpDataMaintenance.Controls.Add(btnClearLogs);
            grpDataMaintenance.Controls.Add(btnResetDemo);
        }

        private void BuildSaveButton()
        {
            btnSave = new Button
            {
                Text = "Save Configuration",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(170, 40),
                BackColor = Color.Maroon,
                ForeColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
        }

        private GroupBox CreateGroupBox(string text)
        {
            return new GroupBox
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.Maroon,
                BackColor = Color.WhiteSmoke
            };
        }

        private Label CreateLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = location
            };
        }

        private NumericUpDown CreateNumeric(Point location, decimal min, decimal max, decimal value)
        {
            return new NumericUpDown
            {
                Location = location,
                Size = new Size(100, 25),
                Minimum = min,
                Maximum = max,
                Value = value,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular)
            };
        }

        private Button CreateActionButton(string text, Size size)
        {
            Button btn = new Button
            {
                Text = text,
                Size = size,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.Maroon,
                ForeColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("System configuration saved successfully.", "Save Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}