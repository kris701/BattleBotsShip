using BattleshipModels;
using BattleshipSimulator.Opponents;
using BattleshipSimulator.Report;
using BattleshipTools;
using System.Diagnostics;
using System.Xml.Linq;
using static BattleshipSimulator.IBattleshipSimulator;

namespace BattleshipSimulator
{
    public class BattleshipSimulator : IBattleshipSimulator
    {
        public BoardSelectionMethod SelectionMethod { get; }

        private Random _rnd = new Random();

        public BattleshipSimulator(BoardSelectionMethod selectionMethod)
        {
            SelectionMethod = selectionMethod;
        }

        public IRunReport RunSimulation(int rounds, string attackerName, string defenderName, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions)
        {
            CheckBoardOptions(attackerBoardOptions, defenderBoardOptions);

            List<Task<TaskReport>> tasks = GenerateTasks(rounds, attackerName, defenderName, attackerBoardOptions, defenderBoardOptions, new CancellationToken());

            Parallel.ForEach(tasks, task => task.Start());
            Task.WaitAll(tasks.ToArray());

            return GenerateRunReport(rounds, tasks);
        }

        public async Task<IRunReport> RunSimulationAsync(int rounds, string attackerName, string defenderName, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, CancellationToken cancellationToken)
        {
            CheckBoardOptions(attackerBoardOptions, defenderBoardOptions);

            List<Task<TaskReport>> tasks = GenerateTasks(rounds, attackerName, defenderName, attackerBoardOptions, defenderBoardOptions, cancellationToken);

            Parallel.ForEach(tasks, task => task.Start());
            await Task.WhenAll(tasks.ToArray());

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

                    var boards = GetBoards(attackerBoardOptions, defenderBoardOptions);
                    var game = new GameSimulator(
                        new BoardSimulator(boards.Item1),
                        attacker,
                        new BoardSimulator(boards.Item2),
                        defender,
                        IGameSimulator.TurnState.Attacker);

                    long attackerProcessingTime = 0;
                    long defenderProcessingTime = 0;

                    var res = IGameSimulator.WinnerState.None;
                    while (res == IGameSimulator.WinnerState.None)
                    {
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        res = game.Update();
                        watch.Stop();

                        if (game.Turn == IGameSimulator.TurnState.Attacker)
                            defenderProcessingTime += watch.ElapsedMilliseconds;
                        else
                            attackerProcessingTime += watch.ElapsedMilliseconds;

                        if (cancellationToken.IsCancellationRequested)
                            return new TaskReport(attacker.Name, 0, 0, defender.Name, 0, 0);
                    }

                    if (res == IGameSimulator.WinnerState.Attacker)
                        return new TaskReport(attacker.Name, 1, attackerProcessingTime, defender.Name, 0, defenderProcessingTime);
                    else if (res == IGameSimulator.WinnerState.Defender)
                        return new TaskReport(attacker.Name, 0, attackerProcessingTime, defender.Name, 1, defenderProcessingTime);

                    return new TaskReport(attacker.Name, 0, 0, defender.Name, 0, 0);
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
            long attackerProcessingPower = 0;
            string defenderName = results[0].DefenderName;
            int defenderWon = 0;
            long defenderProcessingPower = 0;

            foreach (var result in results)
            {
                attackerWon += result.AttackerWon;
                attackerProcessingPower += result.AttackerProcessingTime;
                defenderWon += result.DefenderWon;
                defenderProcessingPower += result.DefenderProcessingTime;
            }

            return new RunReport(rounds, attackerName, attackerWon, attackerProcessingPower, defenderName, defenderWon, defenderProcessingPower);
        }

        public async Task<IRunReport> RunSingleSimulationAsync(IOpponent attacker, IOpponent defender, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, Func<IGameSimulator, Task>? updateFunc, CancellationToken cancellationToken)
        {
            CheckBoardOptions(attackerBoardOptions, defenderBoardOptions);

            var boards = GetBoards(attackerBoardOptions, defenderBoardOptions);
            var game = new GameSimulator(
                new BoardSimulator(boards.Item1),
                attacker,
                new BoardSimulator(boards.Item2),
                defender,
                IGameSimulator.TurnState.Attacker);

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
                    new TaskReport(attacker.Name, 1, attackerProcessingTime, defender.Name, 0, defenderProcessingTime)
                });
            else if (res == IGameSimulator.WinnerState.Defender)
                return GenerateRunReport(1, new List<TaskReport>() {
                    new TaskReport(attacker.Name, 0, attackerProcessingTime, defender.Name, 1, defenderProcessingTime)
                });

            throw new Exception("Impossible win state occured???");
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

        private Tuple<IBoard, IBoard> GetBoards(List<IBoard> attackerBoard, List<IBoard> defenderBoards)
        {
            switch (SelectionMethod)
            {
                case BoardSelectionMethod.Random:
                    return new Tuple<IBoard, IBoard>(attackerBoard[_rnd.Next(0, attackerBoard.Count)], defenderBoards[_rnd.Next(0, defenderBoards.Count)]);
                case BoardSelectionMethod.AttackerOnly:
                    return new Tuple<IBoard, IBoard>(attackerBoard[_rnd.Next(0, attackerBoard.Count)], attackerBoard[_rnd.Next(0, attackerBoard.Count)]);
                case BoardSelectionMethod.DefenderOnly:
                    return new Tuple<IBoard, IBoard>(defenderBoards[_rnd.Next(0, defenderBoards.Count)], defenderBoards[_rnd.Next(0, defenderBoards.Count)]);
            }
            throw new ArgumentException("Invalid board selection method");
        }
    }
}
