using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents
{
    public class UserOpponent : IOpponent
    {
        public string Name { get; } = "User";
        public Point? FireAt { get; set; }
        
        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            throw new NotImplementedException();
        }

        public async Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token)
        {
            while(FireAt == null)
            {
                await Task.Delay(100);
                if (token.IsCancellationRequested)
                    return;
            }
            opponentBoard.Fire(FireAt);
            FireAt = null;
        }
    }
}
