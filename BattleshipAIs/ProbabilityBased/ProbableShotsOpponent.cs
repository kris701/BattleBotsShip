using BattleshipModels;
using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipAIs.ProbabilityBased
{
    public class ProbableShotsOpponent : IOpponent
    {
        public string Name { get; } = "Probable Shots";
        // Some high value, to increase the likelyhood of the AI attempting to shoot a location that has a partially sunk ship
        private int _hitShipWeight = 50;

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            List<Point> looseHits = GetUncoveredHitPoints(opponentBoard);

            List<ProbabilityPoint> points = new List<ProbabilityPoint>();
            for (int x = 0; x < opponentBoard.Board.Width; x++)
            {
                for (int y = 0; y < opponentBoard.Board.Height; y++)
                {
                    Point newPoint = new Point(x, y);
                    if (!opponentBoard.Shots.Contains(newPoint))
                    {
                        int probability = 0;

                        foreach(var ship in opponentBoard.Board.Ships)
                            if (!opponentBoard.LostShips.Contains(ship))
                                probability += TotalShipProbability(newPoint, opponentBoard.Shots, looseHits, ship.Length, opponentBoard.Board.Width, opponentBoard.Board.Height);

                        points.Add(new ProbabilityPoint(x, y, probability));
                    }
                }
            }

            var sortedPoints = points.OrderByDescending(x => x.Probability).ToList();
            var bestPoint = sortedPoints.First();
            opponentBoard.Fire(bestPoint);
        }

        public async Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token)
        {
            await Task.Run(() => DoMoveOn(opponentBoard));
        }

        public void Reset()
        {
        }

        private List<Point> GetUncoveredHitPoints(IBoardSimulator opponentBoard)
        {
            List<Point> uncoveredPoints = new List<Point>();
            foreach (var hit in opponentBoard.Hits)
            {
                bool isCovered = false;
                foreach (var deadShip in opponentBoard.LostShips)
                {
                    if (IsPointWithinShip(hit, deadShip))
                    {
                        isCovered = true;
                        break;
                    }
                }
                if (!isCovered)
                    uncoveredPoints.Add(hit);
            }
            return uncoveredPoints;
        }

        private int TotalShipProbability(Point location, List<Point> shots, List<Point> hits, int length, int width, int height)
        {
            int probability = 0;

            for(int xOffset = 0; xOffset < length; xOffset++)
            {
                var newLoc = new Point(location.X - xOffset, location.Y);
                if (newLoc.X >= 0 && newLoc.X + length <= width)
                    probability += ShipProbability(newLoc, length, shots, hits);
            }
            for (int yOffset = 0; yOffset < length; yOffset++)
            {
                var newLoc = new Point(location.X, location.Y - yOffset);
                if (newLoc.Y >= 0 && newLoc.Y + length <= height)
                    probability += ShipProbability(newLoc, length, shots, hits);
            }

            return probability;
        }

        private int ShipProbability(Point location, int length, List<Point> shots, List<Point> hits)
        {
            int horizontalProbability = 1;
            for (int x = location.X; x < location.X + length; x++)
            {
                Point checkPoint = new Point(x, location.Y);
                if (hits.Contains(checkPoint))
                    horizontalProbability *= _hitShipWeight;
                if (shots.Contains(checkPoint) && !hits.Contains(checkPoint))
                {
                    horizontalProbability = 0;
                    break;
                }
            }
            int verticalProbability = 1;
            for (int y = location.Y; y < location.Y + length; y++)
            {
                Point checkPoint = new Point(location.X, y);
                if (hits.Contains(checkPoint))
                    verticalProbability *= _hitShipWeight;
                if (shots.Contains(checkPoint) && !hits.Contains(checkPoint))
                {
                    verticalProbability = 0;
                    break;
                }
            }
            return horizontalProbability + verticalProbability;
        }

        private bool IsPointWithinShip(Point target, IShip ship)
        {
            if (ship.Orientation == IShip.OrientationDirection.EW)
            {
                if (target.Y != ship.Location.Y)
                    return false;
                if (target.X >= ship.Location.X && target.X < ship.Location.X + ship.Length)
                    return true;
            } else if (ship.Orientation == IShip.OrientationDirection.NS)
            {
                if (target.X != ship.Location.X)
                    return false;
                if (target.Y >= ship.Location.Y && target.Y < ship.Location.Y + ship.Length)
                    return true;
            }
            return false;
        }

        internal class ProbabilityPoint : Point
        {
            public int Probability { get; set; }

            public ProbabilityPoint(int x, int y, int prob) : base(x, y)
            {
                Probability = prob;
            }
        }
    }
}
