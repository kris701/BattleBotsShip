using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments.Report
{
    public class Report : IReport
    {
        public int Rounds { get; set; }
        public Dictionary<string, int> Wins { get; set; }
        public Dictionary<string, int> Losses { get; set; }
        public Dictionary<string, double> WinRate { get; set; }

        public Report(int rounds, Dictionary<string, int> wins, Dictionary<string, int> losses, Dictionary<string, double> winRate)
        {
            Rounds = rounds;
            Wins = wins;
            Losses = losses;
            WinRate = winRate;
        }
    }
}
