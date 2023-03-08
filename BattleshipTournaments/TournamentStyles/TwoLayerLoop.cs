using BattleshipModels;
using BattleshipSimulator;
using BattleshipTools;
using BattleshipTournaments.Report;
using Nanotek.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BattleshipSimulator.BoardSelector;
using static BattleshipTournaments.ITournament;

namespace BattleshipTournaments.TournamentStyles
{
    public class TwoLayerLoop : ITournament
    {
        public event OpponentBattleOverHandler? OnOpponentBattleOver;

        public bool RunParallel { get; set; } = true;

        public IRunReport RunTournament(int rounds, List<string> opponents, List<IBoard> boardOptions)
        {
            List<Task<BattleshipSimulator.Report.IRunReport>> tasks = GenerateTasks(rounds, opponents, boardOptions, new CancellationToken());

            TaskHelper.RunTasks(tasks, RunParallel);

            return GenerateReport(rounds, tasks);
        }

        public async Task<IRunReport> RunTournamentAsync(int rounds, List<string> opponents, List<IBoard> boardOptions, CancellationToken cancellationToken)
        {
            List<Task<BattleshipSimulator.Report.IRunReport>> tasks = GenerateTasks(rounds, opponents, boardOptions, cancellationToken);

            await TaskHelper.RunTasksAsync(tasks, RunParallel, cancellationToken);

            return GenerateReport(rounds, tasks);
        }

        public int GetExpectedRounds(List<string> opponents)
        {
            int maximum = 0;
            int skip = 1;
            foreach (var opponentA in opponents)
            {
                foreach (var opponentB in opponents.Skip(skip))
                    maximum++;
                skip++;
            }
            return maximum;
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
                        IBattleshipSimulator simulator = new BattleshipSimulator.BattleshipSimulator(BoardSelectionMethod.Random);
                        if (cancellationToken.IsCancellationRequested)
                            return new BattleshipSimulator.Report.RunReport();
                        var result = simulator.RunSimulation(
                                        rounds,
                                        opponentA,
                                        opponentB,
                                        boardOptions,
                                        boardOptions
                                        );
                        if (OnOpponentBattleOver != null)
                            if (!cancellationToken.IsCancellationRequested)
                                OnOpponentBattleOver.Invoke(opponentA, opponentB);
                        return result;
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
            Dictionary<string, int> shots = new Dictionary<string, int>();
            Dictionary<string, int> hits = new Dictionary<string, int>();
            Dictionary<string, int> score = new Dictionary<string, int>();
            Dictionary<string, double> shotEfficiency = new Dictionary<string, double>();
            Dictionary<string, double> winRate = new Dictionary<string, double>();

            foreach (var task in tasks)
            {
                DictionaryHelper.AddOrIncrement(processingTime, task.Result.AttackerReport.Name, task.Result.AttackerReport.ProcessingTime);
                DictionaryHelper.AddOrIncrement(processingTime, task.Result.DefenderReport.Name, task.Result.DefenderReport.ProcessingTime);

                DictionaryHelper.AddOrIncrement(wins, task.Result.AttackerReport.Name, task.Result.AttackerReport.Won);
                DictionaryHelper.AddOrIncrement(wins, task.Result.DefenderReport.Name, task.Result.DefenderReport.Won);

                DictionaryHelper.AddOrIncrement(looses, task.Result.AttackerReport.Name, task.Result.AttackerReport.Lost);
                DictionaryHelper.AddOrIncrement(looses, task.Result.DefenderReport.Name, task.Result.DefenderReport.Lost);

                DictionaryHelper.AddOrIncrement(shots, task.Result.AttackerReport.Name, task.Result.AttackerReport.Shots);
                DictionaryHelper.AddOrIncrement(shots, task.Result.DefenderReport.Name, task.Result.DefenderReport.Shots);

                DictionaryHelper.AddOrIncrement(hits, task.Result.AttackerReport.Name, task.Result.AttackerReport.Hits);
                DictionaryHelper.AddOrIncrement(hits, task.Result.DefenderReport.Name, task.Result.DefenderReport.Hits);

                DictionaryHelper.AddOrIncrement(shotEfficiency, task.Result.AttackerReport.Name, task.Result.AttackerReport.ShotEfficiency);
                DictionaryHelper.AddOrIncrement(shotEfficiency, task.Result.DefenderReport.Name, task.Result.DefenderReport.ShotEfficiency);
                
                DictionaryHelper.AddOrIncrement(winRate, task.Result.AttackerReport.Name, task.Result.AttackerReport.WinRate);
                DictionaryHelper.AddOrIncrement(winRate, task.Result.DefenderReport.Name, task.Result.DefenderReport.WinRate);

                DictionaryHelper.AddOrIncrement(score, task.Result.AttackerReport.Name, task.Result.AttackerReport.Score);
                DictionaryHelper.AddOrIncrement(score, task.Result.DefenderReport.Name, task.Result.DefenderReport.Score);
            }

            foreach (var key in score.Keys)
            {
                processingTime[key] = processingTime[key] / (rounds * score.Keys.Count);
                shots[key] = shots[key] / (rounds * score.Keys.Count);
                hits[key] = hits[key] / (rounds * score.Keys.Count);

                wins[key] = wins[key] / score.Keys.Count;
                looses[key] = looses[key] / score.Keys.Count;
                shotEfficiency[key] = Math.Round(shotEfficiency[key] / score.Keys.Count, 2);
                winRate[key] = Math.Round(winRate[key] / score.Keys.Count, 2);
                score[key] = score[key] / score.Keys.Count;
            }

            return new RunReport(rounds, wins, looses, winRate, shots, hits, shotEfficiency, processingTime, score);
        }
    }
}
