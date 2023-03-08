namespace BattleshipSimulator.Report
{
    public interface IRunReport
    {
        public IOpponentReport AttackerReport { get; }
        public IOpponentReport DefenderReport { get; }
    }
}
