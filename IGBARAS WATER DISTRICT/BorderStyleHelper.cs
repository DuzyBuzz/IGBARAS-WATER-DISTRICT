using System;
using System.Drawing;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT
{
    public static class BorderStyleHelper
    {
        /// <summary>
        /// Draws a 3D border around the control using light and dark edges.
        /// </summary>
        public static void Draw3DBorder(Control control, PaintEventArgs e)
        {
            Color lightColor = SystemColors.ControlLightLight;
            Color darkColor = SystemColors.ControlDarkDark;

            Rectangle rect = control.ClientRectangle;

            using (Pen lightPen = new Pen(lightColor, 2))
            using (Pen darkPen = new Pen(darkColor, 2))
            {
                // Top + Left - Light
                e.Graphics.DrawLine(lightPen, rect.Left, rect.Bottom - 1, rect.Left, rect.Top); // Left
                e.Graphics.DrawLine(lightPen, rect.Left, rect.Top, rect.Right - 1, rect.Top);   // Top

                // Bottom + Right - Dark
                e.Graphics.DrawLine(darkPen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 1); // Right
                e.Graphics.DrawLine(darkPen, rect.Right - 1, rect.Bottom - 1, rect.Left, rect.Bottom - 1); // Bottom
            }
        }
    }
}
