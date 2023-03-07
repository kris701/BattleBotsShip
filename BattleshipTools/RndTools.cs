using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipTools
{
    public static class RndTools
    {
        private static Random _rnd = new Random();

        public static Point GetRndNewPoint(int width, int height, HashSet<Point> shots)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException("Width must be larger than or equal to zero");
            if (height < 0)
                throw new ArgumentOutOfRangeException("Height must be larger than or equal to zero");
            if (shots.Count >= (width * height))
                throw new Exception("Too many shots! Cannot get a new random point within this space!");

            Point firePoint = new Point(_rnd.Next(0, width), _rnd.Next(0, height));
            while (shots.Contains(firePoint))
            {
                firePoint = new Point(_rnd.Next(0, width), _rnd.Next(0, height));
            }
            return firePoint;
        }
    }
}
