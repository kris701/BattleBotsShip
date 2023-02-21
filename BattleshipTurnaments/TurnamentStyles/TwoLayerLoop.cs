using BattleshipModels;
using BattleshipSimulator;
using BattleshipTurnaments.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments.TurnamentStyles
{
    public class TwoLayerLoop : ITurnament
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public IReport RunTurnament(int rounds, List<IOpponent> opponents, List<IBoard> boardOptions)
        {
            Dictionary<string, double> winRates = new Dictionary<string, double>();
            Dictionary<string, int> opponentRounds = new Dictionary<string, int>();
            IBattleshipSimulator simulator = new BattleshipSimulator.BattleshipSimulator(IBattleshipSimulator.BoardSelectionMethod.Random);
            
            foreach (var opponentA in opponents)
            {
                foreach (var opponentB in opponents)
                {
                    if (opponentA.Name != opponentB.Name) {
                        var result = simulator.RunSumulation(
                            rounds,
                            opponentA,
                            opponentB,
                            boardOptions
                            );

                        AddOrIncrement(opponentRounds, opponentA.Name);
                        AddOrIncrement(opponentRounds, opponentB.Name);

                        AddOrIncrement(winRates, opponentA.Name, result.AttackerWon);
                        AddOrIncrement(winRates, opponentB.Name, result.DefenderWon);
                    }
                }
            }

            foreach(var key in winRates.Keys)
            {
                winRates[key] = winRates[key] / (double)opponentRounds[key];
            }

            return new Report.Report(rounds, winRates);
        }

        public async Task<IReport> RunTurnamentAsync(int rounds, List<IOpponent> opponents, List<IBoard> boardOptions, Func<Task> updateFunc, CancellationToken cancellationToken)
        {
            Dictionary<string, double> winRates = new Dictionary<string, double>();
            Dictionary<string, int> opponentRounds = new Dictionary<string, int>();
            IBattleshipSimulator simulator = new BattleshipSimulator.BattleshipSimulator(IBattleshipSimulator.BoardSelectionMethod.Random);

            foreach (var opponentA in opponents)
            {
                foreach (var opponentB in opponents)
                {
                    if (opponentA.Name != opponentB.Name)
                    {
                        var result = simulator.RunSumulation(
                            rounds,
                            opponentA,
                            opponentB,
                            boardOptions
                            );

                        if (cancellationToken.IsCancellationRequested)
                            return new Report.Report(-1, new Dictionary<string, double>());

                        AddOrIncrement(opponentRounds, opponentA.Name);
                        AddOrIncrement(opponentRounds, opponentB.Name);

                        AddOrIncrement(winRates, opponentA.Name, result.AttackerWon);
                        AddOrIncrement(winRates, opponentB.Name, result.DefenderWon);
                    }
                }

                await updateFunc();
            }

            foreach (var key in winRates.Keys)
            {
                winRates[key] = winRates[key] / (double)opponentRounds[key];
            }

            return new Report.Report(rounds, winRates);
        }


        private void AddOrIncrement(Dictionary<string, int> dict, string key, int by = 1)
        {
            if (dict.ContainsKey(key))
                dict[key] += by;
            else
                dict.Add(key, by);
        }

        private void AddOrIncrement(Dictionary<string, double> dict, string key, double by = 1)
        {
            if (dict.ContainsKey(key))
                dict[key] += by;
            else
                dict.Add(key, by);
        }
    }
}
