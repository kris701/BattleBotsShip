using BattleshipModels;
using BattleshipSimulator.Report;
using static BattleshipSimulator.BoardSelector;

namespace BattleshipSimulator
{
    public interface IBattleshipSimulator
    {
        public bool RunParallel { get; set; }
        public BoardSelectionMethod SelectionMethod { get; }
        
        public IRunReport RunSimulation(int rounds, string attackerName, string defenderName, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions);
        public Task<IRunReport> RunSimulationAsync(int rounds, string attackerName, string defenderName, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, CancellationToken cancellationToken);

        public Task<IRunReport> RunSingleSimulationAsync(IOpponent attacker, IOpponent defender, List<IBoard> attackerBoardOptions, List<IBoard> defenderBoardOptions, Func<IGameSimulator, Task>? updateFunc, CancellationToken cancellationToken);
    }
}
