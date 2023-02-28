using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTools
{
    /// <summary>
    /// A simple serializable Point
    /// </summary>
    public class Point : IEquatable<object>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point()
        {
            X = 0;
            Y = 0;
        }

        public override bool Equals(object? other)
        {
            if (other is Point point)
            {
                if (this == null && other == null)
                    return true;
                if (other == null)
                    return false;
                if (point.X == X && point.Y == Y)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string? ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
