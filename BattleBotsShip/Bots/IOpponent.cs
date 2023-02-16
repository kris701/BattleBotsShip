using BattleBotsShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBotsShip.Bots
{
    public interface IOpponent : IResetable
    {
        public void DoMoveOn(BoardModel opponentBoard);
    }
}
