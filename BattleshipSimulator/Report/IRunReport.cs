namespace BattleshipSimulator.Report
{
    public interface IRunReport
    {
        public int Rounds { get; set; }
        public string AttackerName { get; set; }
        public int AttackerWon { get; set; }
        public int AttackerLost { get; }
        public double AttackerWinRate { get; }
        public long AttackerProcessingTime { get; set; }

        public string DefenderName { get; set; }
        public int DefenderWon { get; set; }
        public int DefenderLost { get; }
        public double DefenderWinRate { get; }
        public long DefenderProcessingTime { get; set; }
    }
}
