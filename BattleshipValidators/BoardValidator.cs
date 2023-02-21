using BattleshipModels;
using BattleshipTools;
using BattleshipValidators.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipValidators
{
    public static class BoardValidator
    {
        public static bool ValidateBoard(IBoard board)
        {
            List<Point> occupiedPoints = new List<Point>();

            foreach(var ship in board.Ships)
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

                for (int i = 0; i < ship.Length; i++)
                {
                    var newPoint = new Point();
                    if (ship.Orientation == IShip.OrientationDirection.NS)
                        newPoint = new Point(ship.Location.X, ship.Location.Y + i);
                    else if (ship.Orientation == IShip.OrientationDirection.EW)
                        newPoint = new Point(ship.Location.X + i, ship.Location.Y);

                    if (occupiedPoints.Contains(newPoint))
                        return false;
                    occupiedPoints.Add(newPoint);
                }
            }
            return true;
        }
    }
}
