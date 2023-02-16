using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace BattleBotsShip.Models
{
    public class ShipModel : IResetable
    {
        public enum HitState { None, Hit, Sunk }
        public enum OrientationDirection { None, NS, EW };
        public int Length { get; }
        public OrientationDirection Orientation { get; }
        public Point Location { get; }
        [JsonIgnore]
        public List<Point> Hits { get; internal set; }
        [JsonIgnore]
        public bool IsSunk { get; internal set; } = false;

        public ShipModel(int length, OrientationDirection orientation, Point location)
        {
            Length = length;
            Orientation = orientation;
            Location = location;
            Hits = new List<Point>();
        }

        public HitState IsHit(Point location)
        {
            if (IsSunk) 
                return HitState.Sunk;

            if (Orientation == OrientationDirection.NS)
            {
                if (Location.X == location.X)
                    if (location.Y >= Location.Y && location.Y < Location.Y + Length)
                        return RegisterHit(location);
            }
            else if (Orientation == OrientationDirection.EW)
            {
                if (Location.Y == location.Y)
                    if (location.X >= Location.X && location.X < Location.X + Length)
                        return RegisterHit(location);
            }

            return HitState.None;
        }

        private HitState RegisterHit(Point location)
        {
            Hits.Add(location);
            if (Hits.Count >= Length)
            {
                IsSunk = true;
                return HitState.Sunk;
            }
            else
            return HitState.Hit;
        }

        public void Reset()
        {
            Hits.Clear();
            IsSunk = false;
        }
    }
}
