using BattleshipModels;
using BattleshipSimulator.Report;
using System.Diagnostics;
using static BattleshipSimulator.IBattleshipSimulator;

namespace BattleshipSimulator
{
    public class BattleshipSimulator : IBattleshipSimulator
    {
        public BoardSelectionMethod SelectionMethod { get; }
        public IGameSimulator? CurrentGame { get; internal set;  }

        Random _rnd = new Random();

        public BattleshipSimulator(BoardSelectionMethod selectionMethod)
        {
            SelectionMethod = selectionMethod;
        }

        public IReport RunSumulation(int rounds, IOpponent attacker, IOpponent defender, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions)
        {
            var allBoards = new List<IBoard>(attackerBoardOptions);
            allBoards.AddRange(defenderBoardOptions);
            CheckBoardOptions(allBoards);

            var report = new Report.Report(rounds, attacker.Name, 0, 0, defender.Name, 0, 0);

            CurrentGame = GetGame(attacker, defender);

            for (int i = 0; i < rounds; i++)
            {
                var boards = GetBoards(attackerBoardOptions, defenderBoardOptions);

                CurrentGame.AttackerBoard = new BoardSimulator(boards.Item1);
                CurrentGame.DefenderBoard = new BoardSimulator(boards.Item2);
                var res = IGameSimulator.WinnerState.None;
                while (res == IGameSimulator.WinnerState.None)
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    res = CurrentGame.Update();
                    watch.Stop();
                    if (CurrentGame.Turn == IGameSimulator.TurnState.Attacker)
                        report.DefenderProcessingTime += watch.ElapsedMilliseconds;
                    else
                        report.AttackerProcessingTime += watch.ElapsedMilliseconds;
                }

                if (res == IGameSimulator.WinnerState.Attacker)
                    report.AttackerWon++;
                else if (res == IGameSimulator.WinnerState.Defender)
                    report.DefenderWon++;

                CurrentGame.Reset();
            }

            CurrentGame = null;

            return report;
        }

        public async Task<IReport> RunSumulationAsync(int rounds, IOpponent attacker, IOpponent defender, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, Func<Task>? updateFunc, CancellationToken cancellationToken)
        {
            var allBoards = new List<IBoard>(attackerBoardOptions);
            allBoards.AddRange(defenderBoardOptions);
            CheckBoardOptions(allBoards);

            var report = new Report.Report(rounds, attacker.Name, 0, 0, defender.Name, 0, 0);

            CurrentGame = GetGame(attacker, defender);

            for (int i = 0; i < rounds; i++)
            {
                var boards = GetBoards(attackerBoardOptions, defenderBoardOptions);

                CurrentGame.AttackerBoard = new BoardSimulator(boards.Item1);
                CurrentGame.DefenderBoard = new BoardSimulator(boards.Item2);
                var res = IGameSimulator.WinnerState.None;
                while (res == IGameSimulator.WinnerState.None)
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    res = await CurrentGame.UpdateAsync(cancellationToken);
                    watch.Stop();
                    if (CurrentGame.Turn == IGameSimulator.TurnState.Attacker)
                        report.DefenderProcessingTime += watch.ElapsedMilliseconds;
                    else
                        report.AttackerProcessingTime += watch.ElapsedMilliseconds;

                    if (updateFunc != null)
                        await updateFunc();
                    if (cancellationToken.IsCancellationRequested) break;
                }
                if (cancellationToken.IsCancellationRequested) break;

                if (res == IGameSimulator.WinnerState.Attacker)
                    report.AttackerWon++;
                else if (res == IGameSimulator.WinnerState.Defender)
                    report.DefenderWon++;

                CurrentGame.Reset();
            }

            CurrentGame = null;

            return report;
        }

        private IGameSimulator GetGame(IOpponent attacker, IOpponent defender)
        {
            return new GameModel(
                null,
                attacker,
                null,
                defender,
                IGameSimulator.TurnState.Attacker);
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
