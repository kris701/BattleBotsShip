using BattleshipModels;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipValidators
{
    public static class ShipValidator
    {
        public static bool ValidateShip(IBoard board, IShip ship)
        {
            if (ship.Location.X < 0)
                return false;
            if (ship.Orientation == IShip.OrientationDirection.EW)
            {
                if (ship.Location.X + ship.Length > board.Width)
                    return false;
            }
            else if (ship.Location.X >= board.Width)
                return false;

            if (ship.Location.Y < 0)
                return false;
            if (ship.Orientation == IShip.OrientationDirection.NS)
            {
                if (ship.Location.Y + ship.Length > board.Height)
                    return false;
            }
            else if (ship.Location.Y >= board.Height)
                return false;

            return true;
        }
    }
}
