namespace BattleshipSimulator.Report
{
    public interface IRunReport
    {
        public int Rounds { get; }
        public string AttackerName { get; }
        public int AttackerWon { get; }
        public int AttackerLost { get; }
        public double AttackerWinRate { get; }
        public int AttackerShots { get; }
        public int AttackerHits { get; }
        public double AttackerShotEfficiency { get; }
        public long AttackerProcessingTime { get; }

        public string DefenderName { get; }
        public int DefenderWon { get; }
        public int DefenderLost { get; }
        public double DefenderWinRate { get; }
        public long DefenderProcessingTime { get; }
        public int DefenderShots { get; }
        public int DefenderHits { get; }
        public double DefenderShotEfficiency { get; }
    }
}
