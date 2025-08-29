using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace MMX_Web_Tools.Utils
{
    public static class IconFactory
    {
        public static Bitmap CreateCircularIcon(string glyph, Color background, Color foreground, int size = 20)
        {
            if (string.IsNullOrEmpty(glyph)) glyph = "?";
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.Clear(Color.Transparent);

                // Background circle
                using (var brush = new SolidBrush(background))
                using (var pen = new Pen(Darken(background, 0.2f)))
                {
                    var rect = new Rectangle(1, 1, size - 2, size - 2);
                    g.FillEllipse(brush, rect);
                    g.DrawEllipse(pen, rect);
                }

                // Determine font size dynamically
                float baseFont = size * 0.6f;
                using (var font = new Font("Segoe UI", baseFont, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var fore = new SolidBrush(foreground))
                {
                    g.DrawString(glyph, font, fore, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bmp;
        }

        private static Color Darken(Color c, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            int r = (int)(c.R * (1f - amount));
            int g = (int)(c.G * (1f - amount));
            int b = (int)(c.B * (1f - amount));
            return Color.FromArgb(c.A, r, g, b);
        }
    }
}
