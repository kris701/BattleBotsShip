using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.DataModels
{
    public interface IShip
    {
        public enum OrientationDirection { None, NS, EW };

        public int Length { get; }
        public OrientationDirection Orientation { get; }
        public Point Location { get; }
    }
}
