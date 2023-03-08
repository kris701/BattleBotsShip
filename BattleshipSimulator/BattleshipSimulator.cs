using BattleshipModels;
using BattleshipSimulator.Opponents;
using BattleshipSimulator.Report;
using BattleshipTools;
using Nanotek.Helpers;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;
using static BattleshipSimulator.BoardSelector;
using static BattleshipSimulator.IBattleshipSimulator;

namespace BattleshipSimulator
{
    public class BattleshipSimulator : IBattleshipSimulator
    {
        public bool RunParallel { get; set; } = true;
        public BoardSelectionMethod SelectionMethod { get; }

        public BattleshipSimulator(BoardSelectionMethod selectionMethod)
        {
            SelectionMethod = selectionMethod;
        }

        public IRunReport RunSimulation(int rounds, string attackerName, string defenderName, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions)
        {
            CheckBoardOptions(attackerBoardOptions, defenderBoardOptions);

            List<Task<TaskReport>> tasks = GenerateTasks(rounds, attackerName, defenderName, attackerBoardOptions, defenderBoardOptions, new CancellationToken());

            TaskHelper.RunTasks(tasks, RunParallel);

            return GenerateRunReport(rounds, tasks);
        }

        public async Task<IRunReport> RunSimulationAsync(int rounds, string attackerName, string defenderName, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, CancellationToken cancellationToken)
        {
            CheckBoardOptions(attackerBoardOptions, defenderBoardOptions);

            List<Task<TaskReport>> tasks = GenerateTasks(rounds, attackerName, defenderName, attackerBoardOptions, defenderBoardOptions, cancellationToken);

            await TaskHelper.RunTasksAsync(tasks, RunParallel, cancellationToken);

            return GenerateRunReport(rounds, tasks);
        }

        private List<Task<TaskReport>> GenerateTasks(int rounds, string attackerName, string defenderName, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, CancellationToken cancellationToken)
        {
            List<Task<TaskReport>> tasks = new List<Task<TaskReport>>();

            for (int i = 0; i < rounds; i++)
            {
                tasks.Add(new Task<TaskReport>(() => {
                    if (cancellationToken.IsCancellationRequested)
                        return new TaskReport();

                    var attacker = OpponentBuilder.GetOpponent(attackerName);
                    var defender = OpponentBuilder.GetOpponent(defenderName);

                    var boards = BoardSelector.GetBoards(SelectionMethod, attackerBoardOptions, defenderBoardOptions);
                    var game = new GameSimulator(
                        new BoardSimulator(boards.Item1),
                        attacker,
                        new BoardSimulator(boards.Item2),
                        defender);
                    game.Initialize();

                    long attackerProcessingTime = 0;
                    long defenderProcessingTime = 0;

                    Stopwatch watch = new Stopwatch();
                    var res = IGameSimulator.WinnerState.None;
                    while (res == IGameSimulator.WinnerState.None)
                    {
                        watch.Restart();
                        res = game.Update();
                        watch.Stop();

                        if (game.Turn == IGameSimulator.TurnState.Attacker)
                            defenderProcessingTime += watch.ElapsedMilliseconds;
                        else
                            attackerProcessingTime += watch.ElapsedMilliseconds;

                        if (cancellationToken.IsCancellationRequested)
                            return new TaskReport(attacker.Name, 0, 0, 0, 0, defender.Name, 0, 0, 0, 0);
                    }

                    if (res == IGameSimulator.WinnerState.Attacker)
                        return new TaskReport(attacker.Name, 1, attackerProcessingTime, game.DefenderBoard.Shots.Count, game.DefenderBoard.Hits.Count, defender.Name, 0, defenderProcessingTime, game.AttackerBoard.Shots.Count, game.AttackerBoard.Hits.Count);
                    else if (res == IGameSimulator.WinnerState.Defender)
                        return new TaskReport(attacker.Name, 0, attackerProcessingTime, game.DefenderBoard.Shots.Count, game.DefenderBoard.Hits.Count, defender.Name, 1, defenderProcessingTime, game.AttackerBoard.Shots.Count, game.AttackerBoard.Hits.Count);

                    return new TaskReport(attacker.Name, 0, 0, 0, 0, defender.Name, 0, 0, 0, 0);
                }));
            }

            return tasks;
        }

        private RunReport GenerateRunReport(int rounds, List<Task<TaskReport>> tasks)
        {
            List<TaskReport> results = new List<TaskReport>();
            foreach (var task in tasks)
                results.Add(task.Result);
            return GenerateRunReport(rounds, results);
        }

        private RunReport GenerateRunReport(int rounds, List<TaskReport> results)
        {
            if (results.Count == 0)
                throw new ArgumentException("There must be at least one TaskReport!");
            string attackerName = results[0].AttackerName;
            int attackerWon = 0;
            int attackerShots = 0;
            int attackerHits = 0;
            long attackerProcessingPower = 0;
            string defenderName = results[0].DefenderName;
            int defenderWon = 0;
            int defenderShots = 0;
            int defenderHits = 0;
            long defenderProcessingPower = 0;

            foreach (var result in results)
            {
                attackerWon += result.AttackerWon;
                attackerShots += result.AttackerShots;
                attackerHits += result.AttackerHits;
                attackerProcessingPower += result.AttackerProcessingTime;
                defenderWon += result.DefenderWon;
                defenderShots += result.DefenderShots;
                defenderHits += result.DefenderHits;
                defenderProcessingPower += result.DefenderProcessingTime;
            }

            return new RunReport(
                new OpponentReport(rounds, attackerName, attackerWon, attackerShots, attackerHits, attackerProcessingPower),
                new OpponentReport(rounds, defenderName, defenderWon, defenderShots, defenderHits, defenderProcessingPower)
                );
        }

        public async Task<IRunReport> RunSingleSimulationAsync(IOpponent attacker, IOpponent defender, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, Func<IGameSimulator, Task>? updateFunc, CancellationToken cancellationToken)
        {
            CheckBoardOptions(attackerBoardOptions, defenderBoardOptions);

            var boards = BoardSelector.GetBoards(SelectionMethod, attackerBoardOptions, defenderBoardOptions);
            var game = new GameSimulator(
                new BoardSimulator(boards.Item1),
                attacker,
                new BoardSimulator(boards.Item2),
                defender);
            game.Initialize();

            long attackerProcessingTime = 0;
            long defenderProcessingTime = 0;

            var res = IGameSimulator.WinnerState.None;
            while (res == IGameSimulator.WinnerState.None)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                res = await game.UpdateAsync(cancellationToken);
                watch.Stop();
                if (game.Turn == IGameSimulator.TurnState.Attacker)
                    defenderProcessingTime += watch.ElapsedMilliseconds;
                else
                    attackerProcessingTime += watch.ElapsedMilliseconds;

                if (updateFunc != null)
                    await updateFunc(game);
                if (cancellationToken.IsCancellationRequested) 
                    break;
            }

            if (res == IGameSimulator.WinnerState.Attacker)
                return GenerateRunReport(1, new List<TaskReport>() {
                    new TaskReport(attacker.Name, 1, attackerProcessingTime, game.DefenderBoard.Shots.Count, game.DefenderBoard.Hits.Count, defender.Name, 0, defenderProcessingTime, game.AttackerBoard.Shots.Count, game.AttackerBoard.Hits.Count)
                });
            else if (res == IGameSimulator.WinnerState.Defender)
                return GenerateRunReport(1, new List<TaskReport>() {
                    new TaskReport(attacker.Name, 0, attackerProcessingTime, game.DefenderBoard.Shots.Count, game.DefenderBoard.Hits.Count, defender.Name, 1, defenderProcessingTime, game.AttackerBoard.Shots.Count, game.AttackerBoard.Hits.Count)
                });

            return GenerateRunReport(1, new List<TaskReport>() {
                    new TaskReport(attacker.Name, 0, 0, 0, 0, defender.Name, 0, 0, 0, 0)
                });
        }

        private void CheckBoardOptions(List<IBoard> attackerOptions, List<IBoard> defenderOptions)
        {
            var allBoards = new List<IBoard>(attackerOptions);
            allBoards.AddRange(defenderOptions);
            CheckBoardOptions(allBoards);
        }

        private void CheckBoardOptions(List<IBoard> boardOptions)
        {
            if (boardOptions.Count == 0)
                throw new ArgumentOutOfRangeException("Board options cannot be empty!");
            var initialStyle = boardOptions[0].Style;
            foreach(var boardOption in boardOptions.Skip(1))
            {
                if (boardOption.Style != initialStyle)
                    throw new ArgumentException("All board options must be the same style!");
            }
        }
    }
}
