using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipModels
{
    public class BoardModel : IBoard
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Width { get; }
        public int Height { get; }

        public BoardStyles Style { get; }
        public List<ShipModel> Ships { get; }

        [JsonIgnore]
        public Dictionary<Point, IShip> HitPositions { get; }

        public BoardModel(List<ShipModel> ships, int width, int height, BoardStyles style, string name, string description)
        {
            Width = width;
            Height = height;
            Ships = ships;
            Style = style;
            Name = name;
            Description = description;
            HitPositions = new Dictionary<Point, IShip>();
            foreach(var ship in ships)
            {
                if (ship.Orientation == IShip.OrientationDirection.NS)
                    for (int i = 0; i < ship.Length; i++)
                        HitPositions.Add(new Point(ship.Location.X, ship.Location.Y + i), ship);
                else if (ship.Orientation == IShip.OrientationDirection.EW)
                    for (int i = 0; i < ship.Length; i++)
                        HitPositions.Add(new Point(ship.Location.X + i, ship.Location.Y), ship);
            }
        }
    }
}
