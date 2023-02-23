using BattleshipModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleshipSimulator.IBattleshipSimulator;

namespace BattleshipSimulator
{
    public static class BoardSelector
    {
        public enum BoardSelectionMethod { None, Random, AttackerOnly, DefenderOnly }
        private static Random _rnd = new Random();

        public static Tuple<IBoard, IBoard> GetBoards(BoardSelectionMethod selectionMethod, List<IBoard> attackerBoard, List<IBoard> defenderBoards)
        {
            switch (selectionMethod)
            {
                case BoardSelectionMethod.Random:
                    return new Tuple<IBoard, IBoard>(attackerBoard[_rnd.Next(0, attackerBoard.Count)], defenderBoards[_rnd.Next(0, defenderBoards.Count)]);
                case BoardSelectionMethod.AttackerOnly:
                    return new Tuple<IBoard, IBoard>(attackerBoard[_rnd.Next(0, attackerBoard.Count)], attackerBoard[_rnd.Next(0, attackerBoard.Count)]);
                case BoardSelectionMethod.DefenderOnly:
                    return new Tuple<IBoard, IBoard>(defenderBoards[_rnd.Next(0, defenderBoards.Count)], defenderBoards[_rnd.Next(0, defenderBoards.Count)]);
            }
            throw new ArgumentException("Invalid board selection method");
        }
    }
}
