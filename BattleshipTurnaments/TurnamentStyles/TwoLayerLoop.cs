using BattleshipAIs;
using BattleshipModels;
using BattleshipSimulator;
using BattleshipTools;
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
        public IRunReport RunTurnament(int rounds, List<string> opponents, List<IBoard> boardOptions)
        {
            List<Task<BattleshipSimulator.Report.IRunReport>> tasks = GenerateTasks(rounds, opponents, boardOptions, new CancellationToken());

            Parallel.ForEach(tasks, task => task.Start());
            Task.WaitAll(tasks.ToArray());

            return GenerateReport(rounds, tasks);
        }

        public async Task<IRunReport> RunTurnamentAsync(int rounds, List<string> opponents, List<IBoard> boardOptions, CancellationToken cancellationToken)
        {
            List<Task<BattleshipSimulator.Report.IRunReport>> tasks = GenerateTasks(rounds, opponents, boardOptions, cancellationToken);

            Parallel.ForEach(tasks, task => task.Start());
            await Task.WhenAll(tasks.ToArray());

            return GenerateReport(rounds, tasks);
        }

        private List<Task<BattleshipSimulator.Report.IRunReport>> GenerateTasks(int rounds, List<string> opponents, List<IBoard> boardOptions, CancellationToken cancellationToken)
        {
            List<Task<BattleshipSimulator.Report.IRunReport>> tasks = new List<Task<BattleshipSimulator.Report.IRunReport>>();

            int skip = 1;
            foreach (var opponentA in opponents)
            {
                foreach (var opponentB in opponents.Skip(skip))
                {
                    tasks.Add(new Task<BattleshipSimulator.Report.IRunReport>(() =>
                    {
                        IBattleshipSimulator simulator = new BattleshipSimulator.BattleshipSimulator(IBattleshipSimulator.BoardSelectionMethod.Random);
                        if (cancellationToken.IsCancellationRequested)
                            return new BattleshipSimulator.Report.RunReport();
                        return simulator.RunSimulation(
                                        rounds,
                                        OpponentBuilder.GetOpponent(opponentA),
                                        OpponentBuilder.GetOpponent(opponentB),
                                        boardOptions,
                                        boardOptions
                                        );
                    }));
                }
                skip++;
            }

            return tasks;
        }

        private RunReport GenerateReport(int rounds, List<Task<BattleshipSimulator.Report.IRunReport>> tasks)
        {
            Dictionary<string, int> wins = new Dictionary<string, int>();
            Dictionary<string, int> looses = new Dictionary<string, int>();
            Dictionary<string, long> processingTime = new Dictionary<string, long>();
            Dictionary<string, int> totalRounds = new Dictionary<string, int>();

            foreach (var task in tasks)
            {
                DictionaryHelper.AddOrIncrement(processingTime, task.Result.AttackerName, task.Result.AttackerProcessingTime);
                DictionaryHelper.AddOrIncrement(processingTime, task.Result.DefenderName, task.Result.DefenderProcessingTime);

                DictionaryHelper.AddOrIncrement(totalRounds, task.Result.AttackerName, rounds);
                DictionaryHelper.AddOrIncrement(totalRounds, task.Result.DefenderName, rounds);

                DictionaryHelper.AddOrIncrement(wins, task.Result.AttackerName, task.Result.AttackerWon);
                DictionaryHelper.AddOrIncrement(wins, task.Result.DefenderName, task.Result.DefenderWon);

                DictionaryHelper.AddOrIncrement(looses, task.Result.AttackerName, rounds - task.Result.AttackerWon);
                DictionaryHelper.AddOrIncrement(looses, task.Result.DefenderName, rounds - task.Result.DefenderWon);
            }

            Dictionary<string, double> winRate = new Dictionary<string, double>();
            foreach (var key in wins.Keys)
            {
                winRate.Add(key, Math.Round(((double)wins[key] / (double)totalRounds[key]) * 100, 2));
            }

            return new RunReport(rounds, wins, looses, winRate, processingTime);
        }
    }
}
