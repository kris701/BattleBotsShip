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
        public static List<IShip> GetInvalidShips(IBoard board)
        {
            List<IShip> invalidShips = new List<IShip>();
            List<Point> occupiedPoints = new List<Point>();

            // Location validation
            foreach (var ship in board.Ships)
            {
                if (!ShipValidator.ValidateShip(board, ship))
                    invalidShips.Add(ship);

                for (int i = 0; i < ship.Length; i++)
                {
                    var newPoint = new Point();
                    if (ship.Orientation == IShip.OrientationDirection.NS)
                        newPoint = new Point(ship.Location.X, ship.Location.Y + i);
                    else if (ship.Orientation == IShip.OrientationDirection.EW)
                        newPoint = new Point(ship.Location.X + i, ship.Location.Y);

                    if (occupiedPoints.Contains(newPoint))
                        if (!invalidShips.Contains(ship))
                            invalidShips.Add(ship);
                    occupiedPoints.Add(newPoint);
                }
            }
            return invalidShips;
        }

        public static bool ValidateBoard(IBoard board)
        {
            if (GetInvalidShips(board).Count > 0)
                return false;

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
