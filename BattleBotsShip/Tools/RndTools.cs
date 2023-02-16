using BattleBotsShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleBotsShip.Tools
{
    public static class RndTools
    {
        private static Random _rnd = new Random();

        public static Point GetRndNewPoint(BoardModel opponentBoard)
        {
            Point firePoint = new Point(_rnd.Next(0, opponentBoard.Width), _rnd.Next(0, opponentBoard.Height));
            while (opponentBoard.Shots.Contains(firePoint))
            {
                firePoint = new Point(_rnd.Next(0, opponentBoard.Width), _rnd.Next(0, opponentBoard.Height));
            }
            return firePoint;
        }
    }
}
