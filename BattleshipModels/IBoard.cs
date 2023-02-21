using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipModels
{
    public interface IBoard
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Width { get; }
        public int Height { get; }

        public BoardStyles.Styles Style { get; }
        public List<ShipModel> Ships { get; }

        public Dictionary<Point, IShip> GetHitPositions();
    }
}
