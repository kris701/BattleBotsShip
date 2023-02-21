using BattleshipModels;
using BattleshipSimulator.Report;
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

        public IReport RunSumulation(int rounds, IOpponent attacker, IOpponent defender, List<IBoard> boardOptions)
        {
            CheckBoardOptions(boardOptions);

            var report = new Report.Report(rounds, attacker.Name, 0, defender.Name, 0);

            CurrentGame = GetGame(attacker, defender);

            for (int i = 0; i < rounds; i++)
            {
                var boards = GetBoards(boardOptions);

                CurrentGame.AttackerBoard = new BoardSimulator(boards.Item1);
                CurrentGame.DefenderBoard = new BoardSimulator(boards.Item2);
                var res = IGameSimulator.WinnerState.None;
                while (res == IGameSimulator.WinnerState.None)
                {
                    res = CurrentGame.Update();
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

        public async Task<IReport> RunSumulationAsync(int rounds, IOpponent attacker, IOpponent defender, List<IBoard> boardOptions, Func<Task> updateFunc, CancellationToken cancellationToken)
        {
            CheckBoardOptions(boardOptions);

            var report = new Report.Report(rounds, attacker.Name, 0, defender.Name, 0);

            CurrentGame = GetGame(attacker, defender);

            for (int i = 0; i < rounds; i++)
            {
                var boards = GetBoards(boardOptions);

                CurrentGame.AttackerBoard = new BoardSimulator(boards.Item1);
                CurrentGame.DefenderBoard = new BoardSimulator(boards.Item2);
                var res = IGameSimulator.WinnerState.None;
                while (res == IGameSimulator.WinnerState.None)
                {
                    res = CurrentGame.Update();
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

        private Tuple<IBoard, IBoard> GetBoards(List<IBoard> boardOptions)
        {
            switch (SelectionMethod)
            {
                case BoardSelectionMethod.Random:
                    return new Tuple<IBoard, IBoard>(boardOptions[_rnd.Next(0, boardOptions.Count)], boardOptions[_rnd.Next(0, boardOptions.Count)]);
                case BoardSelectionMethod.RandomBothSame:
                    int target = _rnd.Next(0, boardOptions.Count);
                    return new Tuple<IBoard, IBoard>(boardOptions[target], boardOptions[target]);
            }
            throw new ArgumentException("Invalid board selection method");
        }
    }
}
