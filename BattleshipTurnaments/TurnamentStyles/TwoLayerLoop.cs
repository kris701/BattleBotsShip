using BattleshipAIs;
using BattleshipModels;
using BattleshipSimulator;
using BattleshipTurnaments.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BattleshipTurnaments.TurnamentStyles
{
    public class TwoLayerLoop : ITurnament
    {
        public IReport RunTurnament(int rounds, List<string> opponents, List<IBoard> boardOptions)
        {
            List<Task<BattleshipSimulator.Report.IReport>> tasks = GenerateTasks(rounds, opponents, boardOptions, new CancellationToken());

            Parallel.ForEach(tasks, task => task.Start());
            Task.WaitAll(tasks.ToArray());

            return GenerateReport(rounds, tasks);
        }

        public async Task<IReport> RunTurnamentAsync(int rounds, List<string> opponents, List<IBoard> boardOptions, CancellationToken cancellationToken)
        {
            List<Task<BattleshipSimulator.Report.IReport>> tasks = GenerateTasks(rounds, opponents, boardOptions, cancellationToken);

            Parallel.ForEach(tasks, task => task.Start());
            await Task.WhenAll(tasks.ToArray());

            return GenerateReport(rounds, tasks);
        }

        private List<Task<BattleshipSimulator.Report.IReport>> GenerateTasks(int rounds, List<string> opponents, List<IBoard> boardOptions, CancellationToken cancellationToken)
        {
            List<Task<BattleshipSimulator.Report.IReport>> tasks = new List<Task<BattleshipSimulator.Report.IReport>>();

            foreach (var opponentA in opponents)
            {
                foreach (var opponentB in opponents)
                {
                    if (opponentA != opponentB)
                    {
                        tasks.Add(new Task<BattleshipSimulator.Report.IReport>(() =>
                        {
                            IBattleshipSimulator simulator = new BattleshipSimulator.BattleshipSimulator(IBattleshipSimulator.BoardSelectionMethod.Random);
                            if (cancellationToken.IsCancellationRequested)
                                return new BattleshipSimulator.Report.Report();
                            return simulator.RunSumulation(
                                            rounds,
                                            OpponentBuilder.GetOpponent(opponentA),
                                            OpponentBuilder.GetOpponent(opponentB),
                                            boardOptions,
                                            boardOptions
                                            );
                        }));
                    }
                }
            }

            return tasks;
        }

        private Report.Report GenerateReport(int rounds, List<Task<BattleshipSimulator.Report.IReport>> tasks)
        {
            Dictionary<string, int> wins = new Dictionary<string, int>();
            Dictionary<string, int> looses = new Dictionary<string, int>();
            Dictionary<string, long> processingTime = new Dictionary<string, long>();
            Dictionary<string, int> totalRounds = new Dictionary<string, int>();

            foreach (var task in tasks)
            {
                AddOrIncrement(processingTime, task.Result.AttackerName, task.Result.AttackerProcessingTime);
                AddOrIncrement(processingTime, task.Result.DefenderName, task.Result.DefenderProcessingTime);

                AddOrIncrement(totalRounds, task.Result.AttackerName, rounds);
                AddOrIncrement(totalRounds, task.Result.DefenderName, rounds);

                AddOrIncrement(wins, task.Result.AttackerName, task.Result.AttackerWon);
                AddOrIncrement(wins, task.Result.DefenderName, task.Result.DefenderWon);

                AddOrIncrement(looses, task.Result.AttackerName, rounds - task.Result.AttackerWon);
                AddOrIncrement(looses, task.Result.DefenderName, rounds - task.Result.DefenderWon);
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
