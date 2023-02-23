using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments.Report
{
    public class RunReport : IRunReport
    {
        public int Rounds { get; set; }
        public Dictionary<string, int> Wins { get; set; }
        public Dictionary<string, int> Losses { get; set; }
        public Dictionary<string, double> WinRate { get; set; }

        public Dictionary<string, long> ProcessingTime { get; set; }

        public RunReport(int rounds, Dictionary<string, int> wins, Dictionary<string, int> losses, Dictionary<string, double> winRate, Dictionary<string, long> processingTime)
        {
            Rounds = rounds;
            Wins = wins;
            Losses = losses;
            WinRate = winRate;
            ProcessingTime = processingTime;
        }
    }
}
