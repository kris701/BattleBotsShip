using BattleshipModels;
using BattleshipSimulator;
using BattleshipTurnaments.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments
{
    public interface ITurnament
    {
        public IReport RunTurnament(int rounds, List<IOpponent> opponents, List<IBoard> boardOptions);
        public Task<IReport> RunTurnamentAsync(int rounds, List<IOpponent> opponents, List<IBoard> boardOptions, Func<Task>? updateFunc, CancellationToken cancellationToken);
    }
}
