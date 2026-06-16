using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Reminder
{
    public static class ScreenHelper
    {
        public static Rectangle GetVirtualBounds()
        {
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.Bounds.X < minX) minX = s.Bounds.X;
                if (s.Bounds.Y < minY) minY = s.Bounds.Y;
                if (s.Bounds.Right > maxX) maxX = s.Bounds.Right;
                if (s.Bounds.Bottom > maxY) maxY = s.Bounds.Bottom;
            }
            return Rectangle.FromLTRB(minX, minY, maxX, maxY);
        }
        public static Point BottomRightCorner(int formWidth, int formHeight)
        {
            Screen target = Screen.PrimaryScreen;
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.WorkingArea.Right >= target.WorkingArea.Right &&
                    s.WorkingArea.Bottom >= target.WorkingArea.Bottom)
                {
                    target = s;
                }
            }
            int x = target.WorkingArea.Right - formWidth - 10;
            int y = target.WorkingArea.Bottom - formHeight - 10;
            return new Point(Math.Max(target.WorkingArea.Left, x), Math.Max(target.WorkingArea.Top, y));
        }
    }
}
