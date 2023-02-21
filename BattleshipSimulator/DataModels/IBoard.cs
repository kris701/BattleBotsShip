using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.DataModels
{
    public interface IBoard
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Width { get; }
        public int Height { get; }

        public BoardStyles Style { get; }
        public List<ShipModel> Ships { get; }

        public Dictionary<Point, IShip> HitPositions { get; }
    }
}
