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
        public Dictionary<string, int> AvgWins { get; }
        public Dictionary<string, int> AvgLosses { get; }
        public Dictionary<string, double> AvgWinRate { get; }
        public Dictionary<string, int> AvgShots { get; }
        public Dictionary<string, int> AvgHits { get; }
        public Dictionary<string, double> AvgShotEfficiency { get; }
        public Dictionary<string, long> AvgProcessingTime { get; }
        public Dictionary<string, int> AvgScore { get; }

        public RunReport(int rounds, Dictionary<string, int> wins, Dictionary<string, int> losses, Dictionary<string, double> winRate, Dictionary<string, int> shots, Dictionary<string, int> hits, Dictionary<string, double> shotEfficiency, Dictionary<string, long> processingTime, Dictionary<string, int> score)
        {
            Rounds = rounds;
            AvgWins = wins;
            AvgLosses = losses;
            AvgWinRate = winRate;
            AvgShots = shots;
            AvgHits = hits;
            AvgShotEfficiency = shotEfficiency;
            AvgProcessingTime = processingTime;
            AvgScore = score;
        }
    }
}
