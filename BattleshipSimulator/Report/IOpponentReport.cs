using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Report
{
    public interface IOpponentReport
    {
        // Set values
        public int Rounds { get; }
        public string Name { get; }
        public int Won { get; }
        public int Shots { get; }
        public int Hits { get; }
        public long ProcessingTime { get; }

        // Calculated values
        public int Lost { get; }
        public double WinRate { get; }
        public double ShotEfficiency { get; }
        public int Score { get; }
    }
}
