using BattleshipModels;
using BattleshipSimulator.Report;

namespace BattleshipSimulator
{
    public interface IBattleshipSimulator
    {
        public enum BoardSelectionMethod { None, Random, RandomBothSame }

        public BoardSelectionMethod SelectionMethod { get; }
        public IGameSimulator? CurrentGame { get; }

        public IReport RunSumulation(int rounds, IOpponent attacker, IOpponent defender, List<IBoard> boardOptions);
        public Task<IReport> RunSumulationAsync(int rounds, IOpponent attacker, IOpponent defender, List<IBoard> boardOptions, Func<Task>? updateFunc, CancellationToken cancellationToken);
    }
}
