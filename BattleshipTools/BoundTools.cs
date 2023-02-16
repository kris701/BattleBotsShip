using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipTools
{
    public static class BoundTools
    {
        public static bool IsWithinBounds(int width, int height, Point point)
        {
            if (point.X < 0)
                return false;
            if (point.X >= width)
                return false;
            if (point.Y < 0)
                return false;
            if (point.Y >= height)
                return false;
            return true;
        }
    }
}
