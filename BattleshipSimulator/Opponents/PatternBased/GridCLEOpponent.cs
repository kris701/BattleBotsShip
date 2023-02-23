using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents.PatternBased
{
    /// <summary>
    /// Grid Conditional Line Explosion
    /// Based on the Conditional Line Explosion opponent, but instead of random shots it fire in a grid pattern
    /// </summary>
    public class GridCLEOpponent : IOpponent
    {
        public string Name { get; } = "(Grid) Conditional Line Explosion";

        private bool _isCrosshairState = false;
        private Point _lastHit = new Point(0, 0);
        private int _fireState = 0;
        private int _reach = 1;

        private Point _currentGridLoc = new Point(0, 0);
        private int _offset = 1;

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (!_isCrosshairState)
            {
                Point firePoint = GetGridPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard);
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

        private Point GetGridPoint(int width, int height, IBoardSimulator opponentBoard)
        {
            Point newTarget = new Point(_currentGridLoc.X, _currentGridLoc.Y);
            while (opponentBoard.Shots.Contains(newTarget))
            {
                int newX = newTarget.X + 2;
                int newY = newTarget.Y;

                if (newX >= width)
                {
                    newY++;
                    newX = _offset;
                    if (_offset == 1)
                        _offset = 0;
                    else
                        _offset = 1;
                }
                if (newY >= height)
                {
                    return new Point(0, 0);
                }
                newTarget.X = newX;
                newTarget.Y = newY;
            }
            _currentGridLoc = newTarget;
            return newTarget;
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
            _offset = 1;
            _currentGridLoc = new Point();
            _lastHit = new Point();
        }
    }
}
