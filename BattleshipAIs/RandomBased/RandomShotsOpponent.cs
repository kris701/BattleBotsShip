using BattleshipSimulator;
using BattleshipSimulator.DataModels;
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
        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            opponentBoard.Fire(RndTools.GetRndNewPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard.Shots));
        }

        public void Reset()
        {
            
        }
    }
}
