namespace BattleshipSimulator.Report
{
    public class RunReport : IRunReport
    {
        public int Rounds { get; set; }
        public string AttackerName { get; set; }
        public int AttackerWon { get; set; }
        public int AttackerLost => Rounds - AttackerWon;
        public double AttackerWinRate => (double)AttackerWon / (double)Rounds;
        public long AttackerProcessingTime { get; set; }

        public string DefenderName { get; set; }
        public int DefenderWon { get; set; }
        public int DefenderLost => Rounds - DefenderWon;
        public double DefenderWinRate => (double)DefenderWon / (double)Rounds;
        public long DefenderProcessingTime { get; set; }

        public RunReport(int rounds, string attackerName, int attackerWon, long attackerProcessingTime, string defenderName, int defenderWon, long defenderProcessingTime)
        {
            Rounds = rounds;
            AttackerName = attackerName;
            AttackerWon = attackerWon;
            AttackerProcessingTime = attackerProcessingTime;
            DefenderName = defenderName;
            DefenderWon = defenderWon;
            DefenderProcessingTime = defenderProcessingTime;
        }

        public RunReport()
        {
            Rounds = 0;
            AttackerName = "";
            AttackerWon = 0;
            AttackerProcessingTime = 0;
            DefenderName = "";
            DefenderWon = 0;
            DefenderProcessingTime = 0;
        }
    }
}
