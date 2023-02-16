using BattleBotsShip.Models;
using BattleBotsShip.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleBotsShip.Bots
{
    /// <summary>
    /// A bot that what it hits something, it will try the adjacent points around it in the following rounds
    /// </summary>
    public class CrosshairOpponent : IOpponent
    {
        private bool _isCrosshairState = false;
        private Point _lastHit;
        private int _fireState = 0;

        public void DoMoveOn(BoardModel opponentBoard)
        {
            if (!_isCrosshairState)
            {
                Point firePoint = RndTools.GetRndNewPoint(opponentBoard);
                if (opponentBoard.Fire(firePoint) >= BoardModel.HitState.Hit)
                {
                    _lastHit = firePoint;
                    _fireState = 0;
                    _isCrosshairState = true;
                }
            }
            else
            {
                Point firePoint;

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

                    if (BoundTools.IsWithinBounds(opponentBoard, firePoint))
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
