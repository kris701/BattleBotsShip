using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Report
{
    public class OpponentReport : IOpponentReport
    {
        // Set values
        public int Rounds { get; }
        public string Name { get; }
        public int Won { get; }
        public int Shots { get; }
        public int Hits { get; }
        public long ProcessingTime { get; }

        // Calculated values
        public int Lost => Rounds - Won;
        public double WinRate => Math.Round((double)Won / (double)Rounds, 2);
        public double ShotEfficiency => Math.Round((double)Hits / (double)Shots, 2);
        public int Score => (int)(WinRate * 100) * (int)(ShotEfficiency * 100) - GetProcessingTimePenalty(ProcessingTime);

        private int GetProcessingTimePenalty(long processingTime)
        {
            return Math.Min((int)Math.Pow(processingTime, 1.5), 1000);
        }

        public OpponentReport(int rounds, string name, int won, int shots, int hits, long processingTime)
        {
            Rounds = rounds;
            Name = name;
            Won = won;
            Shots = shots;
            Hits = hits;
            ProcessingTime = processingTime;
        }

        public OpponentReport()
        {
            Rounds = 0;
            Name = "";
            Won = 0;
            Shots = 0;
            Hits = 0;
            ProcessingTime = 0;
        }
    }
}
