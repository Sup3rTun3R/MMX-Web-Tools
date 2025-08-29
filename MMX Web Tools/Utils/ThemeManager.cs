using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MMX_Web_Tools.Utils
{
    public enum AppTheme
    {
        Light,
        Dim,
        MatrixDark
    }

    public static class ThemeManager
    {
        public sealed class ThemePalette
        {
            public Color FormBack { get; set; }
            public Color TextFore { get; set; }
            public Color ControlBack { get; set; }
            public Color ControlBackAlt { get; set; }
            public Color ControlFore { get; set; }
            public Color InputBack { get; set; }
            public Color InputFore { get; set; }
            public Color ButtonBack { get; set; }
            public Color ButtonFore { get; set; }
            public Color ToolBack { get; set; }
            public Color ToolFore { get; set; }
            public Color GridBack { get; set; }
            public Color GridAltBack { get; set; }
            public Color GridFore { get; set; }
            public Color GridHeaderBack { get; set; }
            public Color GridHeaderFore { get; set; }
            public Color GridSelectionBack { get; set; }
            public Color GridSelectionFore { get; set; }
            public Color GridVariantRowBack { get; set; }
            public Font LogFont { get; set; }
            // New: progress colors
            public Color ProgressBack { get; set; }
            public Color ProgressFore { get; set; }
            public Color ProgressBorder { get; set; }
        }

        private static AppTheme _current = AppTheme.Light;
        public static AppTheme CurrentTheme
        {
            get => _current;
            set
            {
                if (_current == value) return;
                _current = value;
                ThemeChanged?.Invoke(_current);
            }
        }

        public static event Action<AppTheme> ThemeChanged;

        public static ThemePalette GetPalette(AppTheme theme)
        {
            switch (theme)
            {
                case AppTheme.Dim:
                    // Darker Dim with stronger two-tone contrast
                    return new ThemePalette
                    {
                        FormBack = Color.FromArgb(56, 60, 68),
                        TextFore = Color.FromArgb(235, 235, 240),
                        ControlBack = Color.FromArgb(54, 58, 66),     // darker primary
                        ControlBackAlt = Color.FromArgb(74, 78, 88),  // lighter alt for contrast
                        ControlFore = Color.FromArgb(235, 235, 240),
                        InputBack = Color.FromArgb(48, 52, 60),
                        InputFore = Color.Gainsboro,
                        ButtonBack = Color.FromArgb(120, 124, 134),   // brighter button for pop
                        ButtonFore = Color.White,
                        ToolBack = Color.FromArgb(74, 78, 88),
                        ToolFore = Color.FromArgb(0, 220, 0),         // green menu text for readability
                        GridBack = Color.FromArgb(48, 52, 60),
                        GridAltBack = Color.FromArgb(58, 62, 70),
                        GridFore = Color.Gainsboro,
                        GridHeaderBack = Color.FromArgb(74, 78, 88),
                        GridHeaderFore = Color.Gainsboro,
                        GridSelectionBack = Color.FromArgb(100, 112, 132),
                        GridSelectionFore = Color.White,
                        GridVariantRowBack = Color.FromArgb(60, 56, 50),
                        LogFont = SystemFonts.DefaultFont,
                        ProgressBack = Color.FromArgb(64, 68, 76),
                        ProgressFore = Color.FromArgb(80, 160, 220),
                        ProgressBorder = Color.FromArgb(90, 94, 104)
                    };
                case AppTheme.MatrixDark:
                    var green = Color.FromArgb(0, 255, 0);
                    return new ThemePalette
                    {
                        // Slightly lighter Matrix for better visibility
                        FormBack = Color.FromArgb(22, 24, 26),
                        TextFore = green,
                        ControlBack = Color.FromArgb(28, 30, 32),
                        ControlBackAlt = Color.FromArgb(38, 40, 42),
                        ControlFore = green,
                        InputBack = Color.FromArgb(26, 28, 30),
                        InputFore = green,
                        ButtonBack = Color.FromArgb(50, 52, 54),      // lighter button shade for contrast
                        ButtonFore = green,
                        ToolBack = Color.FromArgb(34, 36, 38),
                        ToolFore = green,
                        GridBack = Color.FromArgb(26, 28, 30),
                        GridAltBack = Color.FromArgb(32, 34, 36),
                        GridFore = green,
                        GridHeaderBack = Color.FromArgb(42, 44, 46),
                        GridHeaderFore = green,
                        GridSelectionBack = Color.FromArgb(70, 155, 70), // brighter selection
                        GridSelectionFore = Color.Black,
                        GridVariantRowBack = Color.FromArgb(18, 58, 18), // more visible green tint
                        LogFont = new Font("Consolas", 9f),
                        ProgressBack = Color.FromArgb(24, 26, 28),
                        ProgressFore = Color.FromArgb(0, 200, 0),
                        ProgressBorder = Color.FromArgb(50, 60, 50)
                    };
                case AppTheme.Light:
                default:
                    return new ThemePalette
                    {
                        FormBack = SystemColors.Control,
                        TextFore = SystemColors.ControlText,
                        ControlBack = SystemColors.Control,
                        ControlBackAlt = Color.FromArgb(245, 245, 245),
                        ControlFore = SystemColors.ControlText,
                        InputBack = SystemColors.Window,
                        InputFore = SystemColors.WindowText,
                        ButtonBack = SystemColors.Control,
                        ButtonFore = SystemColors.ControlText,
                        ToolBack = SystemColors.Control,
                        ToolFore = SystemColors.ControlText,
                        GridBack = Color.White,
                        GridAltBack = Color.FromArgb(248, 248, 248),
                        GridFore = Color.Black,
                        GridHeaderBack = SystemColors.Control,
                        GridHeaderFore = SystemColors.ControlText,
                        GridSelectionBack = SystemColors.Highlight,
                        GridSelectionFore = SystemColors.HighlightText,
                        GridVariantRowBack = Color.FromArgb(255, 250, 235),
                        LogFont = SystemFonts.DefaultFont,
                        ProgressBack = Color.FromArgb(240, 240, 240),
                        ProgressFore = Color.FromArgb(0, 120, 215),
                        ProgressBorder = Color.FromArgb(200, 200, 200)
                    };
            }
        }

        public static void ApplyTheme(Control root)
        {
            ApplyTheme(root, CurrentTheme);
        }

        public static void ApplyTheme(Control root, AppTheme theme)
        {
            if (root == null) return;
            var palette = GetPalette(theme);
            ApplyRecursive(root, palette);
        }

        private static void ApplyRecursive(Control c, ThemePalette p)
        {
            if (c == null) return;

            // Handle specific types
            if (c is DataGridView dgv)
            {
                ApplyToDataGridView(dgv, p);
            }
            else if (c is MenuStrip ms)
            {
                ApplyToToolStrip(ms, p);
            }
            else if (c is ToolStrip ts)
            {
                ApplyToToolStrip(ts, p);
            }
            else if (c is StatusStrip ss)
            {
                ApplyToToolStrip(ss, p);
            }
            else if (c is TextBoxBase tbb)
            {
                tbb.BackColor = p.InputBack;
                tbb.ForeColor = p.InputFore;
                if (tbb is RichTextBox rtb)
                {
                    rtb.Font = p.LogFont;
                }
            }
            else if (c is Button btn)
            {
                btn.BackColor = p.ButtonBack;
                btn.ForeColor = p.ButtonFore;
            }
            else if (c is TableLayoutPanel tlp)
            {
                tlp.BackColor = p.ControlBackAlt;
                tlp.ForeColor = p.ControlFore;
            }
            else if (c is GroupBox gb)
            {
                gb.BackColor = p.ControlBack;
                gb.ForeColor = p.ControlFore;
            }
            else if (c is SplitContainer sp)
            {
                sp.BackColor = p.ControlBack;
                sp.Panel1.BackColor = p.ControlBack;
                sp.Panel2.BackColor = p.ControlBackAlt; // two-tone split
                sp.ForeColor = p.ControlFore;
            }
            else if (c is NumericUpDown nud)
            {
                nud.BackColor = p.InputBack;
                nud.ForeColor = p.InputFore;
            }
            else if (c is ProgressBar pb)
            {
                // Let custom themed progressbars paint themselves; still set base colors
                pb.BackColor = p.ProgressBack;
                pb.ForeColor = p.ProgressFore;
            }
            else if (c is CheckBox || c is RadioButton || c is Label)
            {
                c.BackColor = p.ControlBack;
                c.ForeColor = p.ControlFore;
            }
            else
            {
                // default container/control
                c.BackColor = p.ControlBack;
                c.ForeColor = p.ControlFore;
            }

            // Special case: ToolStrip and its dropdown items
            if (c is ToolStrip ts2)
            {
                foreach (ToolStripItem item in ts2.Items)
                {
                    ApplyToToolStripItemRecursive(item, p);
                }
            }

            // Recurse children
            foreach (Control child in c.Controls)
            {
                ApplyRecursive(child, p);
            }
        }

        private static void ApplyToToolStripItemRecursive(ToolStripItem item, ThemePalette p)
        {
            if (item == null) return;
            item.ForeColor = p.ToolFore;
            item.BackColor = p.ToolBack;
            if (item is ToolStripDropDownItem ddi && ddi.HasDropDownItems)
            {
                ddi.DropDown.BackColor = p.ToolBack;
                foreach (ToolStripItem child in ddi.DropDownItems)
                {
                    ApplyToToolStripItemRecursive(child, p);
                }
            }
        }

        private static void ApplyToDataGridView(DataGridView dgv, ThemePalette p)
        {
            dgv.BackgroundColor = p.GridBack;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = p.GridHeaderBack;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = p.GridHeaderFore;
            dgv.DefaultCellStyle.BackColor = p.GridBack;
            dgv.DefaultCellStyle.ForeColor = p.GridFore;
            dgv.DefaultCellStyle.SelectionBackColor = p.GridSelectionBack;
            dgv.DefaultCellStyle.SelectionForeColor = p.GridSelectionFore;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = p.GridAltBack;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = p.GridFore;
            dgv.GridColor = p.GridAltBack;
        }

        private static void ApplyToToolStrip(ToolStrip ts, ThemePalette p)
        {
            ts.BackColor = p.ToolBack;
            ts.ForeColor = p.ToolFore;
            // Apply renderer with flat colors
            ts.Renderer = new ToolStripProfessionalRenderer(new FlatColorTable(p));
        }

        private sealed class FlatColorTable : ProfessionalColorTable
        {
            private readonly ThemePalette _p;
            public FlatColorTable(ThemePalette p) { _p = p; }
            public override Color ToolStripGradientBegin => _p.ToolBack;
            public override Color ToolStripGradientMiddle => _p.ToolBack;
            public override Color ToolStripGradientEnd => _p.ToolBack;
            public override Color MenuStripGradientBegin => _p.ToolBack;
            public override Color MenuStripGradientEnd => _p.ToolBack;
            public override Color StatusStripGradientBegin => _p.ToolBack;
            public override Color StatusStripGradientEnd => _p.ToolBack;
            public override Color ImageMarginGradientBegin => _p.ToolBack;
            public override Color ImageMarginGradientMiddle => _p.ToolBack;
            public override Color ImageMarginGradientEnd => _p.ToolBack;
            public override Color MenuItemSelected => _p.GridSelectionBack;
            public override Color MenuItemSelectedGradientBegin => _p.GridSelectionBack;
            public override Color MenuItemSelectedGradientEnd => _p.GridSelectionBack;
            public override Color MenuItemBorder => _p.GridSelectionBack;
            public override Color ToolStripDropDownBackground => _p.ToolBack;
            public override Color MenuItemPressedGradientBegin => _p.ToolBack;
            public override Color MenuItemPressedGradientEnd => _p.ToolBack;
            public override Color MenuItemPressedGradientMiddle => _p.ToolBack;
        }
    }
}
