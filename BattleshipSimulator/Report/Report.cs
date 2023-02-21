namespace BattleshipSimulator.Report
{
    public class Report : IReport
    {
        public int Rounds { get; }

        public string AttackerName { get; }
        public int AttackerWon { get; set; }

        public string DefenderName { get; }
        public int DefenderWon { get; set; }

        public Report(int rounds, string attackerName, int attackerWon, string defenderName, int defenderWon)
        {
            Rounds = rounds;
            AttackerName = attackerName;
            AttackerWon = attackerWon;
            DefenderName = defenderName;
            DefenderWon = defenderWon;
        }
    }
}
