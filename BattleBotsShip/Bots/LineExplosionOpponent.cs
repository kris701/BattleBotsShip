using BattleBotsShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleBotsShip.Bots
{
    public class LineExplosionOpponent : IOpponent
    {
        private bool _isCrosshairState = false;
        private Point _lastHit;
        private int _fireState = 0;
        private int _reach = 1;

        public void FireOn(BoardModel opponentBoard)
        {
            if (!_isCrosshairState)
            {
                Random rnd = new Random();
                Point firePoint = new Point(rnd.Next(0, opponentBoard.Width), rnd.Next(0, opponentBoard.Height));
                while (opponentBoard.Shots.Contains(firePoint))
                {
                    firePoint = new Point(rnd.Next(0, opponentBoard.Width), rnd.Next(0, opponentBoard.Height));
                }
                if (opponentBoard.IsHit(firePoint))
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
                    {
                        firePoint = new Point(_lastHit.X, _lastHit.Y - _reach);
                        if (IsValid(opponentBoard, firePoint))
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
                    }
                    else if (_fireState == 1)
                    {
                        firePoint = new Point(_lastHit.X + _reach, _lastHit.Y);
                        if (IsValid(opponentBoard, firePoint))
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
                    }
                    else if (_fireState == 2)
                    {
                        firePoint = new Point(_lastHit.X, _lastHit.Y + _reach);
                        if (IsValid(opponentBoard, firePoint))
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
                    }
                    else if (_fireState == 3)
                    {
                        firePoint = new Point(_lastHit.X - _reach, _lastHit.Y);
                        if (IsValid(opponentBoard, firePoint))
                        {
                            _reach++;
                            if (!opponentBoard.Shots.Contains(firePoint))
                                break;
                        }
                        else
                        {
                            Reset();
                            return;
                        }
                    }
                }
                opponentBoard.IsHit(firePoint);
            }
        }

        public void Reset()
        {
            _isCrosshairState = false;
            _fireState = 0;
            _reach = 1;
        }

        private bool IsValid(BoardModel board, Point point)
        {
            if (point.X < 0)
                return false;
            if (point.X >= board.Width)
                return false;
            if (point.Y < 0)
                return false;
            if (point.Y >= board.Height)
                return false;
            return true;
        }
    }
}
