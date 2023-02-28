using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTournaments.Report
{
    public class RunReport : IRunReport
    {
        public int Rounds { get; }
        public Dictionary<string, int> Wins { get; }
        public Dictionary<string, int> Losses { get; }
        public Dictionary<string, double> WinRate { get; }
        public Dictionary<string, int> Shots { get; }
        public Dictionary<string, int> Hits { get; }
        public Dictionary<string, double> ShotEfficiency { get; }

        public Dictionary<string, long> ProcessingTime { get; }

        public RunReport(int rounds, Dictionary<string, int> wins, Dictionary<string, int> losses, Dictionary<string, double> winRate, Dictionary<string, int> shots, Dictionary<string, int> hits, Dictionary<string, double> shotEfficiency, Dictionary<string, long> processingTime)
        {
            Rounds = rounds;
            Wins = wins;
            Losses = losses;
            WinRate = winRate;
            Shots = shots;
            Hits = hits;
            ShotEfficiency = shotEfficiency;
            ProcessingTime = processingTime;
        }
    }
}
