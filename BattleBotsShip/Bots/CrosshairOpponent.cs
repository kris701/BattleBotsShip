using BattleBotsShip.Models;
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
                }
            }
            else
            {
                Point firePoint;

                bool canFire = false;
                while (!canFire)
                {
                    if (_fireState == 0)
                    {
                        firePoint = new Point(_lastHit.X, _lastHit.Y - 1);
                        if (IsValid(opponentBoard, firePoint))
                            if (!opponentBoard.Shots.Contains(firePoint))
                                canFire = true;
                    } 
                    else if (_fireState == 1)
                    {
                        firePoint = new Point(_lastHit.X + 1, _lastHit.Y);
                        if (IsValid(opponentBoard, firePoint))
                            if (!opponentBoard.Shots.Contains(firePoint))
                                canFire = true;
                    }
                    else if (_fireState == 2)
                    {
                        firePoint = new Point(_lastHit.X, _lastHit.Y + 1);
                        if (IsValid(opponentBoard, firePoint))
                            if (!opponentBoard.Shots.Contains(firePoint))
                                canFire = true;
                    }
                    else if (_fireState == 3)
                    {
                        firePoint = new Point(_lastHit.X - 1, _lastHit.Y);
                        if (IsValid(opponentBoard, firePoint))
                            if (!opponentBoard.Shots.Contains(firePoint))
                                canFire = true;
                    } 
                    else if (_fireState > 4)
                    {
                        _isCrosshairState = false;
                        _fireState = 0;
                        return;
                    }
                    _fireState++;
                }
                opponentBoard.IsHit(firePoint);
            }
        }

        public void Reset()
        {
            _isCrosshairState = false;
            _fireState = 0;
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
