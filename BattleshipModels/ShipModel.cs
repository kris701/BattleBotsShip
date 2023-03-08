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
        [JsonIgnore]
        public bool HaveBeenTamperedWith { get; internal set; } = false;

        public int Length { get; }

        internal IShip.OrientationDirection _orientation = IShip.OrientationDirection.None;
        public IShip.OrientationDirection Orientation
        {
            get
            {
                HaveBeenTamperedWith = true;
                return _orientation;
            }
        }

        internal Point _location = new Point(-1, -1);
        public Point Location
        {
            get
            {
                HaveBeenTamperedWith = true;
                return _location;
            }
        }

        public ShipModel(int length, IShip.OrientationDirection orientation, Point location)
        {
            Length = length;
            _orientation = orientation;
            _location = location;
        }

        /// <summary>
        /// Says if a point is within a ship's are or not
        /// Warning: This will trigger the tamper warning
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
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

        public override bool Equals(object? other)
        {
            if (other == null && this == null)
                return true;
            if (other is ShipModel ship)
            {
                return  ship.Length == Length &&
                        ship._orientation == _orientation &&
                        ship._location.Equals(_location);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Length, _orientation, _location);
        }

        internal List<Point> GetHitPoints()
        {
            List<Point> hitPoints = new List<Point>();
            if (_orientation == IShip.OrientationDirection.NS)
            {
                for (int i = 0; i < Length; i++)
                {
                    var newPoint = new Point(_location.X, _location.Y + i);
                    hitPoints.Add(newPoint);
                }
            }
            else if (_orientation == IShip.OrientationDirection.EW)
            {
                for (int i = 0; i < Length; i++)
                {
                    var newPoint = new Point(_location.X + i, _location.Y);
                    hitPoints.Add(newPoint);
                }
            }
            return hitPoints;
        }

        public object Clone()
        {
            return new ShipModel(
                Length,
                _orientation,
                _location
                );
        }
    }
}
