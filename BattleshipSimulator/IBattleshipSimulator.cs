using BattleshipModels;
using BattleshipSimulator.Report;

namespace BattleshipSimulator
{
    public interface IBattleshipSimulator
    {
        public enum BoardSelectionMethod { None, Random, AttackerOnly, DefenderOnly }

        public BoardSelectionMethod SelectionMethod { get; }
        public IGameSimulator? CurrentGame { get; }

        public IReport RunSumulation(int rounds, IOpponent attacker, IOpponent defender, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions);
        public Task<IReport> RunSumulationAsync(int rounds, IOpponent attacker, IOpponent defender, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, Func<Task>? updateFunc, CancellationToken cancellationToken);
    }
}
