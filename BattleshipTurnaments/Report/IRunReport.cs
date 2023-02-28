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
        public Dictionary<string, int> Wins { get; }
        public Dictionary<string, int> Losses { get; }
        public Dictionary<string, double> WinRate { get; }
        public Dictionary<string, int> Shots { get; }
        public Dictionary<string, int> Hits { get; }
        public Dictionary<string, double> ShotEfficiency { get; }
        public Dictionary<string, long> ProcessingTime { get; }
    }
}
