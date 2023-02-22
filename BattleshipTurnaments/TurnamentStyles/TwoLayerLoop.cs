using BattleshipModels;
using BattleshipSimulator;
using BattleshipTurnaments.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments.TurnamentStyles
{
    public class TwoLayerLoop : ITurnament
    {
        public IReport RunTurnament(int rounds, List<IOpponent> opponents, List<IBoard> boardOptions)
        {
            Dictionary<string, int> wins = new Dictionary<string, int>();
            Dictionary<string, int> looses = new Dictionary<string, int>();
            Dictionary<string, long> processingTime = new Dictionary<string, long>();
            Dictionary<string, int> totalRounds = new Dictionary<string, int>();
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

                        AddOrIncrement(processingTime, opponentA.Name, result.AttackerProcessingTime);
                        AddOrIncrement(processingTime, opponentB.Name, result.DefenderProcessingTime);

                        AddOrIncrement(totalRounds, opponentA.Name, rounds);
                        AddOrIncrement(totalRounds, opponentB.Name, rounds);

                        AddOrIncrement(wins, opponentA.Name, result.AttackerWon);
                        AddOrIncrement(wins, opponentB.Name, result.DefenderWon);

                        AddOrIncrement(looses, opponentA.Name, rounds - result.AttackerWon);
                        AddOrIncrement(looses, opponentB.Name, rounds - result.DefenderWon);
                    }
                }
            }

            Dictionary<string, double> winRate = new Dictionary<string, double>();
            foreach (var key in wins.Keys)
            {
                winRate.Add(key, Math.Round(((double)wins[key] / (double)totalRounds[key]) * 100, 2));
            }

            return new Report.Report(rounds, wins, looses, winRate, processingTime);
        }

        public async Task<IReport> RunTurnamentAsync(int rounds, List<IOpponent> opponents, List<IBoard> boardOptions, Func<Task>? updateFunc, CancellationToken cancellationToken)
        {
            Dictionary<string, int> wins = new Dictionary<string, int>();
            Dictionary<string, int> looses = new Dictionary<string, int>();
            Dictionary<string, long> processingTime = new Dictionary<string, long>();
            Dictionary<string, int> totalRounds = new Dictionary<string, int>();
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
                            boardOptions);

                        if (cancellationToken.IsCancellationRequested)
                            return new Report.Report(-1, new Dictionary<string, int>(), new Dictionary<string, int>(), new Dictionary<string, double>(), new Dictionary<string, long>());

                        AddOrIncrement(processingTime, opponentA.Name, result.AttackerProcessingTime);
                        AddOrIncrement(processingTime, opponentB.Name, result.DefenderProcessingTime);

                        AddOrIncrement(totalRounds, opponentA.Name, rounds);
                        AddOrIncrement(totalRounds, opponentB.Name, rounds);

                        AddOrIncrement(wins, opponentA.Name, result.AttackerWon);
                        AddOrIncrement(wins, opponentB.Name, result.DefenderWon);

                        AddOrIncrement(looses, opponentA.Name, rounds - result.AttackerWon);
                        AddOrIncrement(looses, opponentB.Name, rounds - result.DefenderWon);

                        if (updateFunc != null)
                            await updateFunc();
                    }
                }
            }

            Dictionary<string, double> winRate = new Dictionary<string, double>();
            foreach (var key in wins.Keys)
            {
                winRate.Add(key, Math.Round(((double)wins[key] / (double)totalRounds[key]) * 100, 2));
            }

            return new Report.Report(rounds, wins, looses, winRate, processingTime);
        }

        private void AddOrIncrement(Dictionary<string, int> dict, string key, int by = 1)
        {
            if (dict.ContainsKey(key))
                dict[key] += by;
            else
                dict.Add(key, by);
        }

        private void AddOrIncrement(Dictionary<string, long> dict, string key, long by = 1)
        {
            if (dict.ContainsKey(key))
                dict[key] += by;
            else
                dict.Add(key, by);
        }
    }
}
