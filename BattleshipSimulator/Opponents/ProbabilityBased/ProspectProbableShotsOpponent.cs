using BattleshipModels;
using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents.ProbabilityBased
{
    public class ProspectProbableShotsOpponent : BaseOpponent
    {
        public override string Name { get; } = "Prospect Probable Shots";
        // Some high value, to increase the likelyhood of the AI attempting to shoot a location that has a partially sunk ship
        private int _hitShipWeight = 50;
        private HashSet<Point> _uncoveredPoints = new HashSet<Point>();
        private HashSet<Point> _availableFirePoints = new HashSet<Point>();
        List<IShip> _aliveShips = new List<IShip>();

        private int _currentProspectTargetIndex = 0;
        private List<Point> _prospectTargets = new List<Point>();
        private bool _isInProspectState = true;
        private double _prospectDensity = 0.15;

        public override void Initialize(IBoardSimulator opponentBoard)
        {
            _aliveShips = GetAliveShips(opponentBoard);
            PopulateProspectList(opponentBoard.Board.Width, opponentBoard.Board.Height);
            for (int x = 0; x < opponentBoard.Board.Width; x++)
            {
                for (int y = 0; y < opponentBoard.Board.Height; y++)
                {
                    var point = new Point(x, y);
                    if (!_prospectTargets.Contains(point))
                        _availableFirePoints.Add(point);
                }
            }
            IsInitialized = true;
        }

        public override void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (_isInProspectState)
            {
                opponentBoard.Fire(_prospectTargets[_currentProspectTargetIndex++]);
                if (_currentProspectTargetIndex >= _prospectTargets.Count)
                {
                    _isInProspectState = false;
                    _uncoveredPoints = GetUncoveredHitPoints(opponentBoard);
                }
            }
            else
            {
                var res = opponentBoard.Fire(GetBestPoint(opponentBoard));
                if (res >= IBoardSimulator.HitState.Hit)
                    _uncoveredPoints = GetUncoveredHitPoints(opponentBoard);
                if (res == IBoardSimulator.HitState.Sunk)
                    _aliveShips = GetAliveShips(opponentBoard);
            }
        }

        private void PopulateProspectList(int width, int height)
        {
            int targetPointCount = (int)((width * height) * _prospectDensity);
            int pointEvery = (int)((double)(width * height) / (double)targetPointCount);

            int pointID = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (pointID % pointEvery == 0)
                    {
                        var newPoint = new Point(x, y);
                        if (BoundTools.IsWithinBounds(width, height, newPoint))
                            _prospectTargets.Add(newPoint);
                    }
                    pointID++;
                }
            }
        }

        private Point GetBestPoint(IBoardSimulator opponentBoard)
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

        private int GetProbabilityForPoint(IBoardSimulator opponentBoard, Point point)
        {
            int probability = 0;
            foreach (var ship in _aliveShips)
                probability += TotalShipProbability(point, opponentBoard.Shots, _uncoveredPoints, ship.Length, opponentBoard.Board.Width, opponentBoard.Board.Height);
            return probability;
        }

        private List<IShip> GetAliveShips(IBoardSimulator opponentBoard)
        {
            List<IShip> aliveShips = new List<IShip>();
            foreach (var ship in opponentBoard.Board.Ships)
                if (!opponentBoard.LostShips.Contains(ship))
                    aliveShips.Add(ship);
            return aliveShips;
        }

        private HashSet<Point> GetUncoveredHitPoints(IBoardSimulator opponentBoard)
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

        private int TotalShipProbability(Point location, HashSet<Point> shots, HashSet<Point> hits, int length, int width, int height)
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

        private int HorizontalShipProbability(Point location, int length, HashSet<Point> shots, HashSet<Point> hits)
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

        private int VerticalShipProbability(Point location, int length, HashSet<Point> shots, HashSet<Point> hits)
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
