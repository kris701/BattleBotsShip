using BattleshipSimulator;
using BattleshipSimulator.DataModels;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipAIs.RandomBased
{
    /// <summary>
    /// A bot that what it hits something, it will try the adjacent points around it in the following rounds
    /// </summary>
    public class CrosshairOpponent : IOpponent
    {
        private bool _isCrosshairState = false;
        private Point _lastHit;
        private int _fireState = 0;

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (!_isCrosshairState)
            {
                Point firePoint = RndTools.GetRndNewPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard.Shots);
                if (opponentBoard.Fire(firePoint) >= IBoardSimulator.HitState.Hit)
                {
                    _lastHit = firePoint;
                    _fireState = 0;
                    _isCrosshairState = true;
                }
            }
            else
            {
                Point firePoint = new Point(-1, -1);

                while (true)
                {
                    if (_fireState == 0)
                        firePoint = new Point(_lastHit.X, _lastHit.Y - 1);
                    else if (_fireState == 1)
                        firePoint = new Point(_lastHit.X + 1, _lastHit.Y);
                    else if (_fireState == 2)
                        firePoint = new Point(_lastHit.X, _lastHit.Y + 1);
                    else if (_fireState == 3)
                        firePoint = new Point(_lastHit.X - 1, _lastHit.Y);
                    else if (_fireState > 4)
                    {
                        Reset();
                        return;
                    }

                    if (BoundTools.IsWithinBounds(opponentBoard.Board.Width, opponentBoard.Board.Height, firePoint))
                        if (!opponentBoard.Shots.Contains(firePoint))
                            break;

                    _fireState++;
                }

                opponentBoard.Fire(firePoint);
            }
        }

        public void Reset()
        {
            _isCrosshairState = false;
            _fireState = 0;
        }
    }
}
