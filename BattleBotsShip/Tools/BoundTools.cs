using BattleBotsShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleBotsShip.Tools
{
    public static class BoundTools
    {
        public static bool IsWithinBounds(BoardModel board, Point point)
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
