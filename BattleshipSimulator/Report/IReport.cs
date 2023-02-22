﻿namespace BattleshipSimulator.Report
{
    public interface IReport
    {
        public int Rounds { get; }

        public string AttackerName { get; }
        public int AttackerWon { get; }
        public long AttackerProcessingTime { get; }

        public string DefenderName { get; }
        public int DefenderWon { get; }
        public long DefenderProcessingTime { get; }
    }
}
