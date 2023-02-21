using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleshipSimulator.DataModels;

namespace BattleshipSimulator
{
    public interface IOpponent : IResetable
    {
        public void DoMoveOn(IBoardSimulator opponentBoard);
    }
}
