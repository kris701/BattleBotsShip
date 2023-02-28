using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTournaments.Report
{
    public interface IRunReport
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
    }
}
