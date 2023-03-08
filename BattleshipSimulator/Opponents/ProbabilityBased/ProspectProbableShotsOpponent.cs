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
        List<IShip> _aliveShips = new List<IShip>();

        private int _currentProspectTargetIndex = 0;
        private List<Point> _prospectTargets = new List<Point>();
        private bool _isInProspectState = true;
        private double _prospectDensity = 0.15;

        public override void Initialize(IBoardSimulator opponentBoard)
        {
            _aliveShips = GetAliveShips(opponentBoard);
            IsInitialized = true;
        }

        public override void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (_isInProspectState)
            {
                if (_prospectTargets.Count == 0)
                    PopulateProspectList(opponentBoard.Board.Width, opponentBoard.Board.Height);

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
            Point currentBestPoint = new Point(-1, -1);
            int bestProbability = 0;
            for (int x = 0; x < opponentBoard.Board.Width; x++)
            {
                for (int y = 0; y < opponentBoard.Board.Height; y++)
                {
                    Point newPoint = new Point(x, y);
                    if (!opponentBoard.Shots.Contains(newPoint))
                    {
                        int probability = 0;

                        foreach (var ship in _aliveShips)
                            probability += TotalShipProbability(newPoint, opponentBoard.Shots, _uncoveredPoints, ship.Length, opponentBoard.Board.Width, opponentBoard.Board.Height);

                        if (probability > bestProbability)
                        {
                            bestProbability = probability;
                            currentBestPoint.X = x;
                            currentBestPoint.Y = y;
                        }
                    }
                }
            }

            return currentBestPoint;
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

            for (int xOffset = 0; xOffset < length; xOffset++)
            {
                var newLoc = new Point(location.X - xOffset, location.Y);
                if (newLoc.X >= 0 && newLoc.X + length <= width)
                    probability += HorizontalShipProbability(newLoc, length, shots, hits);
            }
            for (int yOffset = 0; yOffset < length; yOffset++)
            {
                var newLoc = new Point(location.X, location.Y - yOffset);
                if (newLoc.Y >= 0 && newLoc.Y + length <= height)
                    probability += VerticalShipProbability(newLoc, length, shots, hits);
            }

            return probability;
        }

        private int HorizontalShipProbability(Point location, int length, HashSet<Point> shots, HashSet<Point> hits)
        {
            int horizontalProbability = 1;
            for (int x = location.X; x < location.X + length; x++)
            {
                Point checkPoint = new Point(x, location.Y);
                bool isHit = hits.Contains(checkPoint);
                if (isHit)
                    horizontalProbability *= _hitShipWeight;
                else if (shots.Contains(checkPoint))
                {
                    horizontalProbability = 0;
                    break;
                }
            }
            return horizontalProbability;
        }

        private int VerticalShipProbability(Point location, int length, HashSet<Point> shots, HashSet<Point> hits)
        {
            int verticalProbability = 1;
            for (int y = location.Y; y < location.Y + length; y++)
            {
                Point checkPoint = new Point(location.X, y);
                bool isHit = hits.Contains(checkPoint);
                if (isHit)
                    verticalProbability *= _hitShipWeight;
                else if (shots.Contains(checkPoint))
                {
                    verticalProbability = 0;
                    break;
                }
            }
            return verticalProbability;
        }
    }
}
