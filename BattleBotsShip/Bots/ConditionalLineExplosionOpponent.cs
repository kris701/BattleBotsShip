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
    /// When a ship is it, it will check in lines in each direction, until it hits an empty space.
    /// This basically means it will always remove a ship, if it hits it.
    /// </summary>
    public class ConditionalLineExplosionOpponent : IOpponent
    {
        private bool _isCrosshairState = false;
        private Point _lastHit;
        private int _fireState = 0;
        private int _reach = 1;

        public void FireOn(BoardModel opponentBoard)
        {
            if (!_isCrosshairState)
            {
                Point firePoint = RndTools.GetRndNewPoint(opponentBoard);
                if (opponentBoard.IsHit(firePoint) == BoardModel.HitState.Hit)
                {
                    _lastHit = firePoint;
                    _fireState = 0;
                    _isCrosshairState = true;
                    _reach = 1;
                }
            }
            else
            {
                Point firePoint;

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

                    if (BoundTools.IsWithinBounds(opponentBoard, firePoint))
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
                var hitRes = opponentBoard.IsHit(firePoint);
                if (hitRes == BoardModel.HitState.Sunk)
                {
                    Reset();
                } 
                else if (hitRes == BoardModel.HitState.None) {
                    _fireState++;
                    _reach = 1;
                }
            }
        }

        public void Reset()
        {
            _isCrosshairState = false;
            _fireState = 0;
            _reach = 1;
        }
    }
}
