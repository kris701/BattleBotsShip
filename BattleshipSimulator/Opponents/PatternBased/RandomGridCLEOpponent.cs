using BattleshipSimulator;
using BattleshipTools;
using Nanotek.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents.PatternBased
{
    /// <summary>
    /// Grid Conditional Line Explosion
    /// Based on the GridCLEOpponent, but shoots at random points in the grid instead of from an end.
    /// </summary>
    public class RandomGridCLEOpponent : IOpponent
    {
        public string Name { get; } = "(Random Grid) Conditional Line Explosion";

        private bool _isCrosshairState = false;
        private Point _lastHit = new Point(0, 0);
        private int _fireState = 0;
        private int _reach = 1;

        private List<Point> _gridPoints = new List<Point>();
        private int _currentGridIndex = 0;

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (_gridPoints.Count == 0)
                GenerateGridPoints(opponentBoard.Board.Width, opponentBoard.Board.Height);

            if (!_isCrosshairState)
            {
                Point firePoint = GetGridPoint(opponentBoard);
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
                        DoMoveOn(opponentBoard);
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

        private void GenerateGridPoints(int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = (y % 2); x < width; x += 2)
                {
                    var newPoint = new Point(x, y);
                    if (BoundTools.IsWithinBounds(width, height, newPoint))
                        _gridPoints.Add(newPoint);
                }
            }

            ListHelper.Shuffle(_gridPoints);
        }

        private Point GetGridPoint(IBoardSimulator opponentBoard)
        {
            if (_currentGridIndex >= _gridPoints.Count)
                return RndTools.GetRndNewPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard.Shots);
            else
            {
                while (opponentBoard.Shots.Contains(_gridPoints[_currentGridIndex]))
                {
                    _currentGridIndex++;
                    if (_currentGridIndex >= _gridPoints.Count)
                        return RndTools.GetRndNewPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard.Shots);
                }
                return _gridPoints[_currentGridIndex++];
            }
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
