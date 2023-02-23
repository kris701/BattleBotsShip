using BattleshipTools;
using System;
using System.Collections.Generic;
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

        public BoardStyles.Styles Style { get; }
        public List<ShipModel> Ships { get; }

        private Dictionary<Point, IShip> _hitPositions = new Dictionary<Point, IShip>();
        public Dictionary<Point, IShip> GetHitPositions() => _hitPositions;

        public BoardModel(List<ShipModel> ships, int width, int height, BoardStyles.Styles style, string name, string description)
        {
            Width = width;
            Height = height;
            Ships = ships;
            Style = style;
            Name = name;
            Description = description;

            GenerateHitPositions();
        }

        private void GenerateHitPositions()
        {
            _hitPositions.Clear();
            foreach (var ship in Ships)
            {
                if (ship.Orientation == IShip.OrientationDirection.NS)
                {
                    for (int i = 0; i < ship.Length; i++)
                    {
                        var newPoint = new Point(ship.Location.X, ship.Location.Y + i);
                        if (!_hitPositions.ContainsKey(newPoint))
                            _hitPositions.Add(newPoint, ship);
                    }
                }
                else if (ship.Orientation == IShip.OrientationDirection.EW)
                {
                    for (int i = 0; i < ship.Length; i++)
                    {
                        var newPoint = new Point(ship.Location.X + i, ship.Location.Y);
                        if (!_hitPositions.ContainsKey(newPoint))
                            _hitPositions.Add(newPoint, ship);
                    }
                }
            }
        }
    }
}
