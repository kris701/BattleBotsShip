using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipTools
{
    public static class RndTools
    {
        private static Random _rnd = new Random();

        public static Point GetRndNewPoint(int width, int height, List<Point> shots)
        {
            Point firePoint = new Point(_rnd.Next(0, width), _rnd.Next(0, height));
            while (shots.Contains(firePoint))
            {
                firePoint = new Point(_rnd.Next(0, width), _rnd.Next(0, height));
            }
            return firePoint;
        }
    }
}
