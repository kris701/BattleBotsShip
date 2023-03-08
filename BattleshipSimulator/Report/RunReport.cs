namespace BattleshipSimulator.Report
{
    public class RunReport : IRunReport
    {
        public IOpponentReport AttackerReport { get; }
        public IOpponentReport DefenderReport { get; }

        public RunReport(IOpponentReport attackerReport, IOpponentReport defenderReport)
        {
            AttackerReport = attackerReport;
            DefenderReport = defenderReport;
        }

        public RunReport()
        {
            AttackerReport = new OpponentReport();
            DefenderReport = new OpponentReport();
        }
    }
}
