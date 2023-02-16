using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipAIs.RandomBased
{
    public class RandomShotsOpponent : IOpponent
    {
        public void DoMoveOn(BoardModel opponentBoard)
        {
            opponentBoard.Fire(RndTools.GetRndNewPoint(opponentBoard.Width, opponentBoard.Height, opponentBoard.Shots));
        }

        public void Reset()
        {
            
        }
    }
}
