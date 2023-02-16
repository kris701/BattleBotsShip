using BattleBotsShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleBotsShip.Bots
{
    internal class RandomShotsOpponent : IOpponent
    {
        public void FireOn(BoardModel opponentBoard)
        {
            Random rnd = new Random();
            Point firePoint = new Point(rnd.Next(0, opponentBoard.Width), rnd.Next(0, opponentBoard.Height));
            while (opponentBoard.Shots.Contains(firePoint))
            {
                firePoint = new Point(rnd.Next(0, opponentBoard.Width), rnd.Next(0, opponentBoard.Height));
            }
            opponentBoard.IsHit(firePoint);
        }

        public void Reset()
        {
            
        }
    }
}
