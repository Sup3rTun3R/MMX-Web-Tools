using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MMX_Web_Tools.Utils;

namespace MMX_Web_Tools.Controls
{
    // Owner-drawn ProgressBar that respects ThemeManager palette and works in dark themes.
    public class ThemedProgressBar : ProgressBar
    {
        public ThemedProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(200, 16);
            ThemeManager.ThemeChanged += _ => Invalidate();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            // ensure themed colors
            var p = ThemeManager.GetPalette(ThemeManager.CurrentTheme);
            BackColor = p.ProgressBack;
            ForeColor = p.ProgressFore;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var p = ThemeManager.GetPalette(ThemeManager.CurrentTheme);
            e.Graphics.Clear(p.ProgressBack);

            // draw border
            using (var borderPen = new Pen(p.ProgressBorder))
            {
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                e.Graphics.DrawRectangle(borderPen, rect);
            }

            // draw fill
            float percent = Maximum > Minimum ? (float)(Value - Minimum) / (Maximum - Minimum) : 0f;
            int fillWidth = Math.Max(0, (int)((Width - 2) * percent));
            var fillRect = new Rectangle(1, 1, fillWidth, Height - 2);
            using (var brush = new SolidBrush(p.ProgressFore))
            {
                e.Graphics.FillRectangle(brush, fillRect);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
