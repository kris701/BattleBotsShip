using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents
{
    public abstract class BaseOpponent : IOpponent
    {
        public abstract string Name { get; }
        public bool IsInitialized { get; internal set; }

        public virtual void Initialize(IBoardSimulator opponentBoard)
        {
            IsInitialized = true;
        }

        public abstract void DoMoveOn(IBoardSimulator opponentBoard);
        public async virtual Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token)
        {
            await Task.Run(() => DoMoveOn(opponentBoard));
        }
    }
}
