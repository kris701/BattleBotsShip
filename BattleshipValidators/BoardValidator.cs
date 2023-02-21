using BattleshipModels;
using BattleshipValidators.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipValidators
{
    public class BoardValidator
    {
        public void ValidateBoard(IBoard board)
        {
            List<Point> occupiedPoints = new List<Point>();

            foreach(var ship in board.Ships)
            {
                if (ship.Location.X < 0)
                    throw new InvalidBoardException("Location of ship is outside of the board!", board);
                if (ship.Orientation == IShip.OrientationDirection.EW)
                {
                    if (ship.Location.X + ship.Length > board.Width)
                        throw new InvalidBoardException("Location of ship is outside of the board!", board);
                }
                else if (ship.Location.X >= board.Width)
                    throw new InvalidBoardException("Location of ship is outside of the board!", board);

                if (ship.Location.Y < 0)
                    throw new InvalidBoardException("Location of ship is outside of the board!", board);
                if (ship.Orientation == IShip.OrientationDirection.NS)
                {
                    if (ship.Location.Y + ship.Length > board.Height)
                        throw new InvalidBoardException("Location of ship is outside of the board!", board);
                }
                else if (ship.Location.Y >= board.Height)
                    throw new InvalidBoardException("Location of ship is outside of the board!", board);

                for (int i = 0; i < ship.Length; i++)
                {
                    var newPoint = new Point();
                    if (ship.Orientation == IShip.OrientationDirection.NS)
                        newPoint = new Point(ship.Location.X, ship.Location.Y + i);
                    else if (ship.Orientation == IShip.OrientationDirection.EW)
                        newPoint = new Point(ship.Location.X + i, ship.Location.Y);
                    
                    if (occupiedPoints.Contains(newPoint))
                        throw new InvalidBoardException("Ships are overlapping!", board);
                    occupiedPoints.Add(newPoint);
                }
            }
        }
    }
}
