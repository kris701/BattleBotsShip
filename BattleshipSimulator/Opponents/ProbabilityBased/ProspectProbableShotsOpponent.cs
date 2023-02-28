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
    public class ProspectProbableShotsOpponent : IOpponent
    {
        public string Name { get; } = "Prospect Probable Shots";
        // Some high value, to increase the likelyhood of the AI attempting to shoot a location that has a partially sunk ship
        private int _hitShipWeight = 50;

        private int _currentProspectTargetIndex = 0;
        private List<Point> _prospectTargets = new List<Point>();
        private bool _isInProspectState = true;
        private double _prospectDensity = 0.15;

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (_isInProspectState)
            {
                if (_prospectTargets.Count == 0)
                    PopulateProspectList(opponentBoard.Board.Width, opponentBoard.Board.Height);

                opponentBoard.Fire(_prospectTargets[_currentProspectTargetIndex++]);
                if (_currentProspectTargetIndex >= _prospectTargets.Count)
                    _isInProspectState = false;
            }
            else
                opponentBoard.Fire(GetBestPoint(opponentBoard));
        }

        public async Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token)
        {
            await Task.Run(() => DoMoveOn(opponentBoard));
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

        private ProbabilityPoint GetBestPoint(IBoardSimulator opponentBoard)
        {
            HashSet<Point> looseHits = GetUncoveredHitPoints(opponentBoard);

            ProbabilityPoint currentBestPoint = new ProbabilityPoint(-1, -1, 0);
            for (int x = 0; x < opponentBoard.Board.Width; x++)
            {
                for (int y = 0; y < opponentBoard.Board.Height; y++)
                {
                    Point newPoint = new Point(x, y);
                    if (!opponentBoard.Shots.Contains(newPoint))
                    {
                        int probability = 0;

                        foreach (var ship in opponentBoard.Board.Ships)
                            if (!opponentBoard.LostShips.Contains(ship))
                                probability += TotalShipProbability(newPoint, opponentBoard.Shots, looseHits, ship.Length, opponentBoard.Board.Width, opponentBoard.Board.Height);

                        if (probability > currentBestPoint.Probability)
                            currentBestPoint = new ProbabilityPoint(x, y, probability);
                    }
                }
            }

            return currentBestPoint;
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
