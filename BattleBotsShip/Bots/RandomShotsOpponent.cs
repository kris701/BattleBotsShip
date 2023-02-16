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
    public class RandomShotsOpponent : IOpponent
    {
        public void DoMoveOn(BoardModel opponentBoard)
        {
            opponentBoard.Fire(RndTools.GetRndNewPoint(opponentBoard));
        }

        public void Reset()
        {
            
        }
    }
}
