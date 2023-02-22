namespace BattleshipSimulator.Report
{
    public class Report : IReport
    {
        public int Rounds { get; }

        public string AttackerName { get; }
        public int AttackerWon { get; set; }
        public long AttackerProcessingTime { get; set; }

        public string DefenderName { get; }
        public int DefenderWon { get; set; }
        public long DefenderProcessingTime { get; set; }

        public Report(int rounds, string attackerName, int attackerWon, long attackerProcessingTime, string defenderName, int defenderWon, long defenderProcessingTime)
        {
            Rounds = rounds;
            AttackerName = attackerName;
            AttackerWon = attackerWon;
            AttackerProcessingTime = attackerProcessingTime;
            DefenderName = defenderName;
            DefenderWon = defenderWon;
            DefenderProcessingTime = defenderProcessingTime;
        }

        public Report()
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
