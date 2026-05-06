using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EducationSystem
{
    public partial class OversightForm : Form
    {
        private readonly Color Background = ColorTranslator.FromHtml("#F4FAFD");
        private readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        private readonly Color SurfaceLow = ColorTranslator.FromHtml("#EEF5F7");
        private readonly Color SurfaceHigh = ColorTranslator.FromHtml("#DDE4E6");
        private readonly Color Primary = ColorTranslator.FromHtml("#006B55");
        private readonly Color PrimaryContainer = ColorTranslator.FromHtml("#00B894");
        private readonly Color SecondaryContainer = ColorTranslator.FromHtml("#B7EBD7");
        private readonly Color OnSurface = ColorTranslator.FromHtml("#161D1F");
        private readonly Color OnSurfaceVariant = ColorTranslator.FromHtml("#3C4A44");
        private readonly Color Danger = ColorTranslator.FromHtml("#A03F30");
        private readonly Color Muted = ColorTranslator.FromHtml("#8A9A9D");
        private readonly Color InverseSurface = ColorTranslator.FromHtml("#243033");

        private Panel canvas = null!;
        private Label lblTitle = null!;
        private Label lblSubtitle = null!;
        private Label lblFooterNote = null!;

        private readonly List<ModuleSetting> modules = new List<ModuleSetting>();
        private readonly Dictionary<string, Panel> moduleCards = new Dictionary<string, Panel>();
        private readonly Dictionary<string, Panel> iconBoxes = new Dictionary<string, Panel>();
        private readonly Dictionary<string, Label> iconLabels = new Dictionary<string, Label>();
        private readonly Dictionary<string, Panel> toggleTracks = new Dictionary<string, Panel>();
        private readonly Dictionary<string, Panel> toggleKnobs = new Dictionary<string, Panel>();
        private readonly Dictionary<string, Label> statusLabels = new Dictionary<string, Label>();

        private bool isLoading;

        public OversightForm()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                BuildUI();
                LoadModulesFromDatabase();
                Resize += (s, e) => AdjustLayout();
                AdjustLayout();
            }
        }

        private void BuildUI()
        {
            Controls.Clear();

            BackColor = Background;
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Background,
                AutoScroll = true
            };

            lblTitle = new Label
            {
                Text = "System Configuration",
                Font = new Font("Segoe UI", 26F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true
            };

            lblSubtitle = new Label
            {
                Text = "Super Admin controls for platform-wide permissions, monitoring, reports, archive, and maintenance.",
                Font = new Font("Segoe UI", 11.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = true
            };

            lblFooterNote = new Label
            {
                Text = "Changes are saved immediately to the database.",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Primary,
                AutoSize = true
            };

            canvas.Controls.Add(lblTitle);
            canvas.Controls.Add(lblSubtitle);
            canvas.Controls.Add(lblFooterNote);
            Controls.Add(canvas);
        }

        private void LoadModulesFromDatabase()
        {
            modules.Clear();

            try
            {
                isLoading = true;

                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                EnsureSystemModulesSchema(conn);
                InsertDefaultModulesIfMissing(conn);

                const string query = @"
SELECT
    ModuleKey,
    ModuleName,
    Description,
    IconText,
    IsEnabled,
    StatusText,
    DisplayOrder
FROM dbo.SystemModules
WHERE ISNULL(IsDeleted, 0) = 0
ORDER BY DisplayOrder, ModuleName;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string key = Convert.ToString(reader["ModuleKey"]) ?? "";

                    modules.Add(new ModuleSetting
                    {
                        ModuleKey = key,
                        ModuleName = Convert.ToString(reader["ModuleName"]) ?? "",
                        Description = Convert.ToString(reader["Description"]) ?? "",
                        IconText = GetSafeIconText(key, Convert.ToString(reader["IconText"]) ?? ""),
                        IsEnabled = reader["IsEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsEnabled"]),
                        StatusText = Convert.ToString(reader["StatusText"]) ?? "ACTIVE",
                        DisplayOrder = reader["DisplayOrder"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DisplayOrder"])
                    });
                }

                RebuildModuleCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "System modules could not be loaded from the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                isLoading = false;
            }
        }

        private void RebuildModuleCards()
        {
            foreach (Panel card in moduleCards.Values)
                canvas.Controls.Remove(card);

            moduleCards.Clear();
            iconBoxes.Clear();
            iconLabels.Clear();
            toggleTracks.Clear();
            toggleKnobs.Clear();
            statusLabels.Clear();

            foreach (ModuleSetting module in modules)
            {
                Panel card = BuildModuleCard(module);
                moduleCards[module.ModuleKey] = card;
                canvas.Controls.Add(card);
            }

            AdjustLayout();
        }

        private Panel BuildModuleCard(ModuleSetting module)
        {
            Panel card = new Panel
            {
                BackColor = Surface,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = module.ModuleKey,
                Cursor = Cursors.Hand
            };

            Panel iconBox = new Panel
            {
                Name = "IconBox",
                Size = new Size(54, 54),
                Location = new Point(30, 28),
                BackColor = GetIconBack(module)
            };

            Label icon = new Label
            {
                Text = module.IconText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = GetIconFore(module),
                BackColor = Color.Transparent
            };

            iconBox.Controls.Add(icon);

            Label title = new Label
            {
                Name = "ModuleTitle",
                Text = module.ModuleName,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = OnSurface,
                AutoSize = true,
                Location = new Point(30, 108)
            };

            Label description = new Label
            {
                Name = "ModuleDescription",
                Text = module.Description,
                Font = new Font("Segoe UI", 10.5F),
                ForeColor = OnSurfaceVariant,
                AutoSize = false,
                Location = new Point(30, 148),
                Size = new Size(350, 48)
            };

            Panel track = new Panel
            {
                Name = "ToggleTrack",
                Size = new Size(58, 28),
                BackColor = module.IsEnabled ? PrimaryContainer : SurfaceHigh,
                Cursor = Cursors.Hand
            };

            Panel knob = new Panel
            {
                Name = "ToggleKnob",
                Size = new Size(24, 24),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            track.Controls.Add(knob);
            track.Paint += ToggleTrack_Paint;
            knob.Paint += ToggleKnob_Paint;

            Label bullet = new Label
            {
                Name = "StatusBullet",
                Text = "■",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = module.IsEnabled ? PrimaryContainer : Muted,
                AutoSize = true,
                Location = new Point(30, 216)
            };

            Label status = new Label
            {
                Name = "StatusLabel",
                Text = GetStatusText(module),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = module.IsEnabled ? PrimaryContainer : Muted,
                AutoSize = true,
                Location = new Point(54, 216)
            };

            card.Controls.Add(iconBox);
            card.Controls.Add(title);
            card.Controls.Add(description);
            card.Controls.Add(track);
            card.Controls.Add(bullet);
            card.Controls.Add(status);

            iconBoxes[module.ModuleKey] = iconBox;
            iconLabels[module.ModuleKey] = icon;
            toggleTracks[module.ModuleKey] = track;
            toggleKnobs[module.ModuleKey] = knob;
            statusLabels[module.ModuleKey] = status;

            card.Click += (s, e) => ToggleModule(module.ModuleKey);
            iconBox.Click += (s, e) => ToggleModule(module.ModuleKey);
            icon.Click += (s, e) => ToggleModule(module.ModuleKey);
            title.Click += (s, e) => ToggleModule(module.ModuleKey);
            description.Click += (s, e) => ToggleModule(module.ModuleKey);
            track.Click += (s, e) => ToggleModule(module.ModuleKey);
            knob.Click += (s, e) => ToggleModule(module.ModuleKey);

            RefreshModuleCard(module);

            return card;
        }

        private void ToggleModule(string moduleKey)
        {
            if (isLoading)
                return;

            ModuleSetting? module = modules.FirstOrDefault(x => x.ModuleKey == moduleKey);

            if (module == null)
                return;

            bool oldValue = module.IsEnabled;
            string oldStatus = module.StatusText;

            module.IsEnabled = !module.IsEnabled;
            module.StatusText = module.IsEnabled ? "ACTIVE" : "DISABLED";

            bool saved = SaveModuleStatus(module);

            if (!saved)
            {
                module.IsEnabled = oldValue;
                module.StatusText = oldStatus;
            }

            RefreshModuleCard(module);
        }

        private bool SaveModuleStatus(ModuleSetting module)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(DbConfig.ConnectionString);
                conn.Open();

                const string query = @"
UPDATE dbo.SystemModules
SET IsEnabled = @IsEnabled,
    StatusText = @StatusText,
    UpdatedAt = SYSUTCDATETIME(),
    UpdatedBy = @UpdatedBy
WHERE ModuleKey = @ModuleKey;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ModuleKey", module.ModuleKey);
                cmd.Parameters.AddWithValue("@IsEnabled", module.IsEnabled);
                cmd.Parameters.AddWithValue("@StatusText", module.StatusText);
                cmd.Parameters.AddWithValue("@UpdatedBy", GetCurrentAdminName());

                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                    throw new InvalidOperationException("No module row was updated. ModuleKey: " + module.ModuleKey);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Module status could not be saved.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        private void RefreshModuleCard(ModuleSetting module)
        {
            if (!moduleCards.ContainsKey(module.ModuleKey))
                return;

            Panel card = moduleCards[module.ModuleKey];
            Panel track = toggleTracks[module.ModuleKey];
            Panel knob = toggleKnobs[module.ModuleKey];
            Label status = statusLabels[module.ModuleKey];
            Panel iconBox = iconBoxes[module.ModuleKey];
            Label icon = iconLabels[module.ModuleKey];

            card.BackColor = module.IsEnabled ? Surface : SurfaceLow;
            track.BackColor = module.IsEnabled ? PrimaryContainer : SurfaceHigh;
            knob.BackColor = Color.White;
            knob.Location = new Point(module.IsEnabled ? track.Width - knob.Width - 2 : 2, 2);

            iconBox.BackColor = GetIconBack(module);
            icon.ForeColor = GetIconFore(module);
            icon.Text = GetSafeIconText(module.ModuleKey, module.IconText);

            status.Text = GetStatusText(module);
            status.ForeColor = module.IsEnabled ? PrimaryContainer : Muted;

            Label? bullet = card.Controls.Find("StatusBullet", false).FirstOrDefault() as Label;
            if (bullet != null)
                bullet.ForeColor = module.IsEnabled ? PrimaryContainer : Muted;

            Label? title = card.Controls.Find("ModuleTitle", false).FirstOrDefault() as Label;
            if (title != null)
                title.ForeColor = module.IsEnabled ? OnSurface : Muted;

            Label? description = card.Controls.Find("ModuleDescription", false).FirstOrDefault() as Label;
            if (description != null)
                description.ForeColor = module.IsEnabled ? OnSurfaceVariant : Muted;

            track.Invalidate();
            knob.Invalidate();
            card.Invalidate();
        }

        private string GetSafeIconText(string moduleKey, string currentIcon)
        {
            return moduleKey switch
            {
                "client_management" => "CL",
                "access_control" => "AC",
                "circulation_oversight" => "CO",
                "reports_exports" => "RP",
                "archive_recovery" => "AR",
                "maintenance_mode" => "MT",
                _ => string.IsNullOrWhiteSpace(currentIcon) ? "MD" : currentIcon
            };
        }

        private string GetStatusText(ModuleSetting module)
        {
            return module.IsEnabled ? "STATUS: ENABLED" : "STATUS: DISABLED";
        }

        private Color GetIconBack(ModuleSetting module)
        {
            return module.ModuleKey switch
            {
                "client_management" => SecondaryContainer,
                "access_control" => ColorTranslator.FromHtml("#E1F6EE"),
                "circulation_oversight" => SurfaceHigh,
                "reports_exports" => ColorTranslator.FromHtml("#D9F4EC"),
                "archive_recovery" => ColorTranslator.FromHtml("#E1F6EE"),
                "maintenance_mode" => ColorTranslator.FromHtml("#FDE7E2"),
                _ => SurfaceHigh
            };
        }

        private Color GetIconFore(ModuleSetting module)
        {
            return module.ModuleKey switch
            {
                "maintenance_mode" => Danger,
                _ => Primary
            };
        }

        private void ToggleTrack_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel)
                return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using SolidBrush brush = new SolidBrush(panel.BackColor);
            using System.Drawing.Drawing2D.GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), panel.Height / 2);
            e.Graphics.FillPath(brush, path);
        }

        private void ToggleKnob_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel)
                return;

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

        private string GetCurrentAdminName()
        {
            if (!string.IsNullOrWhiteSpace(UserSession.Username))
                return UserSession.Username!;

            if (!string.IsNullOrWhiteSpace(ClientSession.Username))
                return ClientSession.Username!;

            return "Super Admin";
        }

        private void EnsureSystemModulesSchema(SqlConnection conn)
        {
            const string query = @"
IF OBJECT_ID('dbo.SystemModules', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SystemModules
    (
        ModuleID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ModuleKey NVARCHAR(80) NOT NULL UNIQUE,
        ModuleName NVARCHAR(150) NOT NULL,
        Description NVARCHAR(300) NULL,
        IconText NVARCHAR(20) NULL,
        IsEnabled BIT NOT NULL DEFAULT 1,
        StatusText NVARCHAR(50) NOT NULL DEFAULT 'ACTIVE',
        DisplayOrder INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedBy NVARCHAR(150) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
END;

IF COL_LENGTH('dbo.SystemModules', 'ModuleKey') IS NULL
    ALTER TABLE dbo.SystemModules ADD ModuleKey NVARCHAR(80) NULL;

IF COL_LENGTH('dbo.SystemModules', 'ModuleName') IS NULL
    ALTER TABLE dbo.SystemModules ADD ModuleName NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.SystemModules', 'Description') IS NULL
    ALTER TABLE dbo.SystemModules ADD Description NVARCHAR(300) NULL;

IF COL_LENGTH('dbo.SystemModules', 'IconText') IS NULL
    ALTER TABLE dbo.SystemModules ADD IconText NVARCHAR(20) NULL;

IF COL_LENGTH('dbo.SystemModules', 'IsEnabled') IS NULL
    ALTER TABLE dbo.SystemModules ADD IsEnabled BIT NOT NULL CONSTRAINT DF_SystemModules_IsEnabled DEFAULT 1;

IF COL_LENGTH('dbo.SystemModules', 'StatusText') IS NULL
    ALTER TABLE dbo.SystemModules ADD StatusText NVARCHAR(50) NOT NULL CONSTRAINT DF_SystemModules_StatusText DEFAULT 'ACTIVE';

IF COL_LENGTH('dbo.SystemModules', 'DisplayOrder') IS NULL
    ALTER TABLE dbo.SystemModules ADD DisplayOrder INT NOT NULL CONSTRAINT DF_SystemModules_DisplayOrder DEFAULT 0;

IF COL_LENGTH('dbo.SystemModules', 'CreatedAt') IS NULL
    ALTER TABLE dbo.SystemModules ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_SystemModules_CreatedAt DEFAULT SYSUTCDATETIME();

IF COL_LENGTH('dbo.SystemModules', 'UpdatedAt') IS NULL
    ALTER TABLE dbo.SystemModules ADD UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_SystemModules_UpdatedAt DEFAULT SYSUTCDATETIME();

IF COL_LENGTH('dbo.SystemModules', 'UpdatedBy') IS NULL
    ALTER TABLE dbo.SystemModules ADD UpdatedBy NVARCHAR(150) NULL;

IF COL_LENGTH('dbo.SystemModules', 'IsDeleted') IS NULL
    ALTER TABLE dbo.SystemModules ADD IsDeleted BIT NOT NULL CONSTRAINT DF_SystemModules_IsDeleted DEFAULT 0;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void InsertDefaultModulesIfMissing(SqlConnection conn)
        {
            const string deleteOldDefaults = @"
UPDATE dbo.SystemModules
SET IsDeleted = 1,
    UpdatedAt = SYSUTCDATETIME(),
    UpdatedBy = 'System Migration'
WHERE ModuleKey IN
(
    'dark_mode',
    'email_alerts',
    'advanced_search',
    'member_ratings',
    'digital_archive'
);";

            using (SqlCommand cleanupCmd = new SqlCommand(deleteOldDefaults, conn))
                cleanupCmd.ExecuteNonQuery();

            const string query = @"
IF NOT EXISTS (SELECT 1 FROM dbo.SystemModules WHERE ModuleKey = 'client_management')
INSERT INTO dbo.SystemModules (ModuleKey, ModuleName, Description, IconText, IsEnabled, StatusText, DisplayOrder)
VALUES ('client_management', 'Client Management', 'Manage registered school libraries, client access, subscriptions, and status controls.', 'CL', 1, 'ACTIVE', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.SystemModules WHERE ModuleKey = 'access_control')
INSERT INTO dbo.SystemModules (ModuleKey, ModuleName, Description, IconText, IsEnabled, StatusText, DisplayOrder)
VALUES ('access_control', 'Access Control', 'Configure admin, librarian, member, teacher, student, and guest permission rules.', 'AC', 1, 'ACTIVE', 2);

IF NOT EXISTS (SELECT 1 FROM dbo.SystemModules WHERE ModuleKey = 'circulation_oversight')
INSERT INTO dbo.SystemModules (ModuleKey, ModuleName, Description, IconText, IsEnabled, StatusText, DisplayOrder)
VALUES ('circulation_oversight', 'Circulation Oversight', 'Allow Super Admin monitoring of borrowing, returns, and fines across all institutions.', 'CO', 1, 'ACTIVE', 3);

IF NOT EXISTS (SELECT 1 FROM dbo.SystemModules WHERE ModuleKey = 'reports_exports')
INSERT INTO dbo.SystemModules (ModuleKey, ModuleName, Description, IconText, IsEnabled, StatusText, DisplayOrder)
VALUES ('reports_exports', 'Reports & Exports', 'Enable system-wide reports, analytics summaries, and PDF export generation.', 'RP', 1, 'ACTIVE', 4);

IF NOT EXISTS (SELECT 1 FROM dbo.SystemModules WHERE ModuleKey = 'archive_recovery')
INSERT INTO dbo.SystemModules (ModuleKey, ModuleName, Description, IconText, IsEnabled, StatusText, DisplayOrder)
VALUES ('archive_recovery', 'Archive Recovery', 'Allow restore and permanent delete actions for archived records.', 'AR', 1, 'ACTIVE', 5);

IF NOT EXISTS (SELECT 1 FROM dbo.SystemModules WHERE ModuleKey = 'maintenance_mode')
INSERT INTO dbo.SystemModules (ModuleKey, ModuleName, Description, IconText, IsEnabled, StatusText, DisplayOrder)
VALUES ('maintenance_mode', 'Maintenance Mode', 'Temporarily pause client access while system updates or migrations are running.', 'MT', 0, 'DISABLED', 6);

UPDATE dbo.SystemModules
SET ModuleName = CASE ModuleKey
        WHEN 'client_management' THEN 'Client Management'
        WHEN 'access_control' THEN 'Access Control'
        WHEN 'circulation_oversight' THEN 'Circulation Oversight'
        WHEN 'reports_exports' THEN 'Reports & Exports'
        WHEN 'archive_recovery' THEN 'Archive Recovery'
        WHEN 'maintenance_mode' THEN 'Maintenance Mode'
        ELSE ModuleName
    END,
    Description = CASE ModuleKey
        WHEN 'client_management' THEN 'Manage registered school libraries, client access, subscriptions, and status controls.'
        WHEN 'access_control' THEN 'Configure admin, librarian, member, teacher, student, and guest permission rules.'
        WHEN 'circulation_oversight' THEN 'Allow Super Admin monitoring of borrowing, returns, and fines across all institutions.'
        WHEN 'reports_exports' THEN 'Enable system-wide reports, analytics summaries, and PDF export generation.'
        WHEN 'archive_recovery' THEN 'Allow restore and permanent delete actions for archived records.'
        WHEN 'maintenance_mode' THEN 'Temporarily pause client access while system updates or migrations are running.'
        ELSE Description
    END,
    IconText = CASE ModuleKey
        WHEN 'client_management' THEN 'CL'
        WHEN 'access_control' THEN 'AC'
        WHEN 'circulation_oversight' THEN 'CO'
        WHEN 'reports_exports' THEN 'RP'
        WHEN 'archive_recovery' THEN 'AR'
        WHEN 'maintenance_mode' THEN 'MT'
        ELSE IconText
    END,
    DisplayOrder = CASE ModuleKey
        WHEN 'client_management' THEN 1
        WHEN 'access_control' THEN 2
        WHEN 'circulation_oversight' THEN 3
        WHEN 'reports_exports' THEN 4
        WHEN 'archive_recovery' THEN 5
        WHEN 'maintenance_mode' THEN 6
        ELSE DisplayOrder
    END,
    IsDeleted = 0
WHERE ModuleKey IN
(
    'client_management',
    'access_control',
    'circulation_oversight',
    'reports_exports',
    'archive_recovery',
    'maintenance_mode'
);";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        private void AdjustLayout()
        {
            if (canvas == null)
                return;

            int margin = 34;
            int availableWidth = Math.Max(960, ClientSize.Width - (margin * 2));

            lblTitle.Location = new Point(margin, 28);
            lblSubtitle.Location = new Point(margin + 2, 74);

            int top = 126;
            int gap = 28;
            int cardHeight = 248;
            int cardWidth = (availableWidth - (gap * 2)) / 3;

            for (int i = 0; i < modules.Count; i++)
            {
                ModuleSetting module = modules[i];

                if (!moduleCards.TryGetValue(module.ModuleKey, out Panel? card))
                    continue;

                int col = i % 3;
                int row = i / 3;

                card.Bounds = new Rectangle(
                    margin + (col * (cardWidth + gap)),
                    top + (row * (cardHeight + 30)),
                    cardWidth,
                    cardHeight);

                if (toggleTracks.TryGetValue(module.ModuleKey, out Panel? track))
                {
                    track.Location = new Point(card.Width - 86, 28);

                    if (toggleKnobs.TryGetValue(module.ModuleKey, out Panel? knob))
                        knob.Location = new Point(module.IsEnabled ? track.Width - knob.Width - 2 : 2, 2);
                }

                Label? title = card.Controls.Find("ModuleTitle", false).FirstOrDefault() as Label;
                if (title != null)
                    title.MaximumSize = new Size(card.Width - 68, 0);

                Label? description = card.Controls.Find("ModuleDescription", false).FirstOrDefault() as Label;
                if (description != null)
                    description.Size = new Size(card.Width - 68, 52);
            }

            int rows = Math.Max(1, (int)Math.Ceiling(modules.Count / 3.0));
            int footerY = top + (rows * (cardHeight + 30)) + 4;
            lblFooterNote.Location = new Point(margin, footerY);

            canvas.AutoScrollMinSize = new Size(0, footerY + 70);
        }
    }

    public class ModuleSetting
    {
        public string ModuleKey { get; set; } = "";
        public string ModuleName { get; set; } = "";
        public string Description { get; set; } = "";
        public string IconText { get; set; } = "";
        public bool IsEnabled { get; set; }
        public string StatusText { get; set; } = "";
        public int DisplayOrder { get; set; }
    }
}