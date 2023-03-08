using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipSimulator.Opponents.RandomBased
{
    public class RandomShotsOpponent : BaseOpponent
    {
        public override string Name { get; } = "Random";

        public override void DoMoveOn(IBoardSimulator opponentBoard)
        {
            opponentBoard.Fire(RndTools.GetRndNewPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard.Shots));
        }
    }
}
