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

            // Location validation
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

            // Style validation
            var styleBoard = BoardStyles.GetStyleDefinition(board.Style);
            if (styleBoard.Width != board.Width)
                return false;
            if (styleBoard.Height != board.Height)
                return false;
            if (styleBoard.Ships.Count != board.Ships.Count)
                return false;

            List<ShipModel> ships = new List<ShipModel>(board.Ships);
            for (int i = 0; i < styleBoard.Ships.Count; i++)
            {
                bool foundAny = false;
                for (int j = 0; j < ships.Count; j++)
                {
                    if (styleBoard.Ships[i].Length == ships[j].Length)
                    {
                        foundAny = true;
                        ships.RemoveAt(j);
                        break;
                    }
                }
                if (!foundAny)
                    return false;
            }

            return true;
        }
    }
}
