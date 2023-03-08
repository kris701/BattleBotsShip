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
        private bool _haveBeenTamperedWith = false;
        [JsonIgnore]
        public bool HaveBeenTamperedWith { get {
                if (_haveBeenTamperedWith)
                    return true;

                foreach (var ship in Ships)
                {
                    if (ship.HaveBeenTamperedWith)
                    {
                        _haveBeenTamperedWith = true;
                        return true;
                    }
                }
                return false;
            } 
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public int Width { get; }
        public int Height { get; }

        public BoardStyles.Styles Style { get; }

        public List<ShipModel> Ships { get; }

        private Dictionary<Point, IShip> _hitPositions;

        [JsonConstructor]
        public BoardModel(List<ShipModel> ships, int width, int height, BoardStyles.Styles style, string name, string description)
        {
            Width = width;
            Height = height;
            Style = style;
            Name = name;
            Description = description;

            _hitPositions = new Dictionary<Point, IShip>();
            Ships = ships;

            GenerateHitPositions();
        }

        private void GenerateHitPositions()
        {
            _hitPositions.Clear();
            foreach (var ship in Ships)
            {
                var shipHitPoints = ship.GetHitPoints();
                foreach(var point in shipHitPoints)
                    if (!_hitPositions.ContainsKey(point))
                        _hitPositions.Add(point, ship);
            }
        }

        public IShip? GetHit(Point location)
        {
            if (_hitPositions.ContainsKey(location))
                return _hitPositions[location];
            return null;
        }
    }
}
