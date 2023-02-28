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
            Dictionary<string, int> totalRounds = new Dictionary<string, int>();
            Dictionary<string, int> shots = new Dictionary<string, int>();
            Dictionary<string, int> hits = new Dictionary<string, int>();
            Dictionary<string, int> score = new Dictionary<string, int>();
            Dictionary<string, double> shotEfficiency = new Dictionary<string, double>();
            Dictionary<string, double> winRate = new Dictionary<string, double>();

            foreach (var task in tasks)
            {
                DictionaryHelper.AddOrIncrement(totalRounds, task.Result.AttackerName, 1);
                DictionaryHelper.AddOrIncrement(totalRounds, task.Result.DefenderName, 1);

                DictionaryHelper.AddOrIncrement(processingTime, task.Result.AttackerName, task.Result.AttackerProcessingTime);
                DictionaryHelper.AddOrIncrement(processingTime, task.Result.DefenderName, task.Result.DefenderProcessingTime);

                DictionaryHelper.AddOrIncrement(wins, task.Result.AttackerName, task.Result.AttackerWon);
                DictionaryHelper.AddOrIncrement(wins, task.Result.DefenderName, task.Result.DefenderWon);

                DictionaryHelper.AddOrIncrement(looses, task.Result.AttackerName, rounds - task.Result.AttackerWon);
                DictionaryHelper.AddOrIncrement(looses, task.Result.DefenderName, rounds - task.Result.DefenderWon);

                DictionaryHelper.AddOrIncrement(shots, task.Result.AttackerName, task.Result.AttackerShots);
                DictionaryHelper.AddOrIncrement(shots, task.Result.DefenderName, task.Result.DefenderShots);

                DictionaryHelper.AddOrIncrement(hits, task.Result.AttackerName, task.Result.AttackerHits);
                DictionaryHelper.AddOrIncrement(hits, task.Result.DefenderName, task.Result.DefenderHits);

                DictionaryHelper.AddOrIncrement(shotEfficiency, task.Result.AttackerName, task.Result.AttackerShotEfficiency);
                DictionaryHelper.AddOrIncrement(shotEfficiency, task.Result.DefenderName, task.Result.DefenderShotEfficiency);
                
                DictionaryHelper.AddOrIncrement(winRate, task.Result.AttackerName, task.Result.AttackerWinRate);
                DictionaryHelper.AddOrIncrement(winRate, task.Result.DefenderName, task.Result.DefenderWinRate);

                DictionaryHelper.AddOrIncrement(score, task.Result.AttackerName, task.Result.AttackerScore);
                DictionaryHelper.AddOrIncrement(score, task.Result.DefenderName, task.Result.DefenderScore);
            }

            foreach (var key in score.Keys)
            {
                processingTime[key] = processingTime[key] / totalRounds[key];
                wins[key] = wins[key] / totalRounds[key];
                looses[key] = looses[key] / totalRounds[key];
                shots[key] = shots[key] / totalRounds[key];
                hits[key] = hits[key] / totalRounds[key];

                shotEfficiency[key] = Math.Round(shotEfficiency[key] / totalRounds[key], 2);
                winRate[key] = Math.Round(winRate[key] / totalRounds[key], 2);
                score[key] = score[key] / totalRounds[key];
            }

            return new RunReport(rounds, wins, looses, winRate, shots, hits, shotEfficiency, processingTime, score);
        }
    }
}
