using BattleshipModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipValidators.Exceptions
{
    public class InvalidBoardException : Exception
    {
        public IBoard Board { get; }
        public InvalidBoardException(string? message, IBoard board) : base(message)
        {
            Board = board;
        }
    }
}
