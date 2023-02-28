namespace BattleshipSimulator.Report
{
    public class RunReport : IRunReport
    {
        public int Rounds { get; }
        public string AttackerName { get; }
        public int AttackerWon { get; }
        public int AttackerLost => Rounds - AttackerWon;
        public double AttackerWinRate => (double)AttackerWon / (double)Rounds;
        public long AttackerProcessingTime { get; }
        public int AttackerShots { get; }
        public int AttackerHits { get; }
        public double AttackerShotEfficiency => (double)AttackerHits / (double)AttackerShots;

        public int AttackerScore => (int)(AttackerWinRate * 100) * (int)(AttackerShotEfficiency * 100) - GetProcessingTimePenalty(AttackerProcessingTime);

        public string DefenderName { get; }
        public int DefenderWon { get; }
        public int DefenderLost => Rounds - DefenderWon;
        public double DefenderWinRate => (double)DefenderWon / (double)Rounds;
        public long DefenderProcessingTime { get; }
        public int DefenderShots { get; }
        public int DefenderHits { get; }
        public double DefenderShotEfficiency => (double)DefenderHits / (double)DefenderShots;

        public int DefenderScore => (int)(DefenderWinRate * 100) * (int)(DefenderShotEfficiency * 100) - GetProcessingTimePenalty(DefenderProcessingTime);

        private int GetProcessingTimePenalty(long processingTime)
        {
            return Math.Min((int)Math.Pow(processingTime, 1.2), 1000);
        }

        public RunReport(int rounds, string attackerName, int attackerWon, long attackerProcessingTime, int attackerShots, int attackerHits, string defenderName, int defenderWon, long defenderProcessingTime, int defenderShots, int defenderHits)
        {
            Rounds = rounds;
            AttackerName = attackerName;
            AttackerWon = attackerWon;
            AttackerProcessingTime = attackerProcessingTime;
            AttackerShots = attackerShots;
            AttackerHits = attackerHits;
            DefenderName = defenderName;
            DefenderWon = defenderWon;
            DefenderProcessingTime = defenderProcessingTime;
            DefenderShots = defenderShots;
            DefenderHits = defenderHits;
        }

        public RunReport()
        {
            Rounds = 0;
            AttackerName = "";
            AttackerWon = 0;
            AttackerProcessingTime = 0;
            AttackerShots = 0;
            AttackerHits = 0;
            DefenderName = "";
            DefenderWon = 0;
            DefenderProcessingTime = 0;
            DefenderShots = 0;
            DefenderHits = 0;
        }
    }
}
