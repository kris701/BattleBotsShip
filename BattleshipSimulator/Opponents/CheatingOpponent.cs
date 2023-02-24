using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents
{
    public class CheatingOpponent : IOpponent
    {
        public string Name { get; } = "Cheating";
        private List<Point> _hits = new List<Point>();

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            foreach(var fireAt in opponentBoard.Board.GetHitPositions().Keys)
            {
                if (!_hits.Contains(fireAt))
                {
                    opponentBoard.Fire(fireAt);
                    _hits.Add(fireAt);
                    return;
                }
            }
        }

        public async Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token)
        {
            await Task.Run(() => DoMoveOn(opponentBoard));
        }
    }
}
