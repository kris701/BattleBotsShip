using BattleshipModels;
using BattleshipSimulator;
using BattleshipTournaments.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTournaments
{
    public interface ITournament
    {
        public delegate void OpponentBattleOverHandler(string opponentA, string opponentB);
        public event OpponentBattleOverHandler? OnOpponentBattleOver;

        public bool RunParallel { get; set; }

        public IRunReport RunTournament(int rounds, List<string> opponents, List<IBoard> boardOptions);
        public Task<IRunReport> RunTournamentAsync(int rounds, List<string> opponents, List<IBoard> boardOptions, CancellationToken cancellationToken);

        public int GetExpectedRounds(List<string> opponents);
    }
}
