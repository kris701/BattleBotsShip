using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipSimulator.Opponents.RandomBased
{
    /// <summary>
    /// When a ship is it, it will check in lines in each direction.
    /// </summary>
    public class LineExplosionOpponent : BaseOpponent
    {
        public override string Name { get; } = "Line Explosion";

        private bool _isCrosshairState = false;
        private Point _lastHit = new Point();
        private int _fireState = 0;
        private int _reach = 1;

        public override void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (!_isCrosshairState)
            {
                Point firePoint = RndTools.GetRndNewPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard.Shots);
                if (opponentBoard.Fire(firePoint) >= IBoardSimulator.HitState.Hit)
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

                opponentBoard.Fire(firePoint);
            }
        }

        private void Reset()
        {
            _isCrosshairState = false;
            _fireState = 0;
            _reach = 1;
        }
    }
}
