using BattleshipModels;
using BattleshipSimulator;
using BattleshipTools;
using Nanotek.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents.ProbabilityBased
{
    public class ProbableShotsOpponent : BaseOpponent
    {
        public override string Name { get; } = "Probable Shots";
        // Some high value, to increase the likelyhood of the AI attempting to shoot a location that has a partially sunk ship
        private int _hitShipWeight = 50;
        internal HashSet<Point> _uncoveredPoints = new HashSet<Point>();
        internal HashSet<Point> _availableFirePoints = new HashSet<Point>();
        internal Dictionary<int,int> _aliveShipOfLength = new Dictionary<int, int>();

        public override void Initialize(IBoardSimulator opponentBoard)
        {
            _aliveShipOfLength = GetAliveShipsLengths(opponentBoard);
            for (int x = 0; x < opponentBoard.Board.Width; x++)
                for (int y = 0; y < opponentBoard.Board.Height; y++)
                    _availableFirePoints.Add(new Point(x,y));
            IsInitialized = true;
        }

        public override void DoMoveOn(IBoardSimulator opponentBoard)
        {
            var res = opponentBoard.Fire(GetBestPoint(opponentBoard));
            if (res >= IBoardSimulator.HitState.Hit)
                _uncoveredPoints = GetUncoveredHitPoints(opponentBoard);
            if (res == IBoardSimulator.HitState.Sunk)
                _aliveShipOfLength = GetAliveShipsLengths(opponentBoard);
        }

        internal Point GetBestPoint(IBoardSimulator opponentBoard)
        {
            Point currentBestPoint = _availableFirePoints.First();
            int bestProbability = GetProbabilityForPoint(opponentBoard, currentBestPoint);
            foreach (var point in _availableFirePoints)
            {
                int probability = GetProbabilityForPoint(opponentBoard, point);
                if (probability > bestProbability)
                {
                    bestProbability = probability;
                    currentBestPoint = point;
                }
            }
            _availableFirePoints.Remove(currentBestPoint);

            return currentBestPoint;
        }

        internal int GetProbabilityForPoint(IBoardSimulator opponentBoard, Point point)
        {
            int probability = 0;
            foreach (var shipIndex in _aliveShipOfLength.Keys)
                probability += _aliveShipOfLength[shipIndex] * TotalShipProbability(point, opponentBoard.Shots, _uncoveredPoints, shipIndex, opponentBoard.Board.Width, opponentBoard.Board.Height);
            return probability;
        }

        internal HashSet<Point> GetUncoveredHitPoints(IBoardSimulator opponentBoard)
        {
            HashSet<Point> uncoveredPoints = new HashSet<Point>();
            foreach (var hit in opponentBoard.Hits)
            {
                bool isCovered = false;
                foreach (var deadShip in opponentBoard.LostShips)
                {
                    if (deadShip.IsPointWithin(hit))
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

        internal Dictionary<int, int> GetAliveShipsLengths(IBoardSimulator opponentBoard)
        {
            Dictionary<int, int> aliveShipLengths = new Dictionary<int, int>();
            foreach (var ship in opponentBoard.Board.Ships)
                if (!opponentBoard.LostShips.Contains(ship))
                    DictionaryHelper.AddOrIncrement(aliveShipLengths, ship.Length, 1);
            return aliveShipLengths;
        }

        internal int TotalShipProbability(Point location, HashSet<Point> shots, HashSet<Point> hits, int length, int width, int height)
        {
            int probability = 0;
            bool foundAnyHorizontal = false;
            for (int x = location.X - (length - 1); x <= location.X; x++)
            {
                if (x >= 0 && x + length <= width)
                {
                    var newLoc = new Point(x, location.Y);
                    var res = HorizontalShipProbability(newLoc, length, shots, hits);
                    if (res == 0)
                    {
                        if (foundAnyHorizontal)
                            break;
                    }
                    else
                        foundAnyHorizontal = true;
                    probability += res;
                }
            }
            bool foundAnyVerticxal = false;
            for (int y = location.Y - (length - 1); y <= location.Y; y++)
            {
                if (y >= 0 && y + length <= height)
                {
                    var newLoc = new Point(location.X, y);
                    var res = VerticalShipProbability(newLoc, length, shots, hits);
                    if (res == 0)
                    {
                        if (foundAnyVerticxal)
                            break;
                    }
                    else
                        foundAnyVerticxal = true;
                    probability += res;
                }
            }

            return probability;
        }

        internal int HorizontalShipProbability(Point location, int length, HashSet<Point> shots, HashSet<Point> hits)
        {
            int horizontalProbability = 1;
            Point checkPoint = new Point(-1, location.Y);
            for (int x = location.X; x < location.X + length; x++)
            {
                checkPoint.X = x;
                if (shots.Contains(checkPoint))
                {
                    if (hits.Contains(checkPoint))
                        horizontalProbability *= _hitShipWeight;
                    else return 0;
                }
            }
            return horizontalProbability;
        }

        internal int VerticalShipProbability(Point location, int length, HashSet<Point> shots, HashSet<Point> hits)
        {
            int verticalProbability = 1;
            Point checkPoint = new Point(location.X, -1);
            for (int y = location.Y; y < location.Y + length; y++)
            {
                checkPoint.Y = y;
                if (shots.Contains(checkPoint))
                {
                    if (hits.Contains(checkPoint))
                        verticalProbability *= _hitShipWeight;
                    else return 0;
                }
            }
            return verticalProbability;
        }
    }
}
