using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipModels
{
    public interface IShip : ICloneable, IEquatable<object>
    {
        public bool HaveBeenTamperedWith { get; }

        public enum OrientationDirection { None, NS, EW };

        public int Length { get; }
        public OrientationDirection Orientation { get; }
        public Point Location { get; }

        public bool IsPointWithin(Point point);
    }
}
