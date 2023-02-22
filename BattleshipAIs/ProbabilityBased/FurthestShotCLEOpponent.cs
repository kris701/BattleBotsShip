using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipAIs.ProbabilityBased
{
    /// <summary>
    /// Furthest Shot Conditional Line Explosion
    /// Based on the Conditional Line Explosion opponent, but instead of random shots it will shoot at a point that is 
    /// </summary>
    public class FurthestShotCLEOpponent : IOpponent
    {
        public string Name { get; } = "Furthest Shot Conditional Line Explosion";

        private bool _isCrosshairState = false;
        private Point _lastHit = new Point(0, 0);
        private int _fireState = 0;
        private int _reach = 1;

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (!_isCrosshairState)
            {
                Point firePoint = GetFurthestPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard);
                if (opponentBoard.Fire(firePoint) == IBoardSimulator.HitState.Hit)
                {
                    _lastHit = firePoint;
                    _fireState = 0;
                    _isCrosshairState = true;
                    _reach = 1;
                }
            }
            else
            {
                Point firePoint = new Point(-1, -1);

                while (true)
                {
                    if (_fireState == 0)
                        firePoint = new Point(_lastHit.X, _lastHit.Y - _reach);
                    else if (_fireState == 1)
                        firePoint = new Point(_lastHit.X + _reach, _lastHit.Y);
                    else if (_fireState == 2)
                        firePoint = new Point(_lastHit.X, _lastHit.Y + _reach);
                    else if (_fireState == 3)
                        firePoint = new Point(_lastHit.X - _reach, _lastHit.Y);

                    if (BoundTools.IsWithinBounds(opponentBoard.Board.Width, opponentBoard.Board.Height, firePoint))
                    {
                        _reach++;
                        if (!opponentBoard.Shots.Contains(firePoint))
                            break;
                        else if (!opponentBoard.Hits.Contains(firePoint))
                        {
                            _fireState++;
                            _reach = 1;
                        }
                    }
                    else
                    {
                        _fireState++;
                        _reach = 1;
                    }

                    if (_fireState >= 4)
                    {
                        Reset();
                        return;
                    }
                }
                var hitRes = opponentBoard.Fire(firePoint);
                if (hitRes == IBoardSimulator.HitState.Sunk)
                {
                    Reset();
                }
                else if (hitRes == IBoardSimulator.HitState.None)
                {
                    _fireState++;
                    _reach = 1;
                }
            }
        }

        private Point GetFurthestPoint(int width, int height, IBoardSimulator opponentBoard)
        {
            Point furthestPoint = new Point();
            Point currentPoint = new Point();
            double biggestDistance = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    currentPoint.X = x;
                    currentPoint.Y = y;
                    if (!opponentBoard.Shots.Contains(currentPoint))
                    {
                        double totalDist = double.MaxValue;
                        foreach(var shot in opponentBoard.Shots)
                        {
                            double dist = GetDistance(currentPoint, shot);
                            if (dist < totalDist)
                                totalDist = dist;
                        }

                        if (totalDist > biggestDistance)
                        {
                            biggestDistance = totalDist;
                            furthestPoint = new Point(currentPoint.X, currentPoint.Y);
                        }
                    }
                }
            }

            return furthestPoint;
        }

        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X,2) + Math.Pow(b.Y - a.Y,2));
        }

        public async Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token)
        {
            await Task.Run(() => DoMoveOn(opponentBoard));
        }

        public void Reset()
        {
            _isCrosshairState = false;
            _fireState = 0;
            _reach = 1;
            _lastHit = new Point();
        }
    }
}
