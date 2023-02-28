using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipModels
{
    public class ShipModel : IShip
    {
        public int Length { get; }
        public IShip.OrientationDirection Orientation { get; }
        public Point Location { get; }

        [JsonConstructor]
        public ShipModel(int length, IShip.OrientationDirection orientation, Point location)
        {
            Length = length;
            Orientation = orientation;
            Location = location;
        }

        public ShipModel(int length)
        {
            Length = length;
            Orientation = IShip.OrientationDirection.EW;
            Location = new Point(0,0);
        }

        public bool IsPointWithin(Point point)
        {
            if (Orientation == IShip.OrientationDirection.EW)
            {
                if (point.Y != Location.Y)
                    return false;
                if (point.X >= Location.X && point.X < Location.X + Length)
                    return true;
            }
            else if (Orientation == IShip.OrientationDirection.NS)
            {
                if (point.X != Location.X)
                    return false;
                if (point.Y >= Location.Y && point.Y < Location.Y + Length)
                    return true;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null && this == null)
                return true;
            if (obj is ShipModel ship)
            {
                return  ship.Length == Length &&
                        ship.Orientation == Orientation &&
                        ship.Location.Equals(Location);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Length, Orientation, Location);
        }
    }
}
