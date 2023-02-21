using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments.Report
{
    public interface IReport
    {
        public int Rounds { get; set; }
        public Dictionary<string, int> Wins { get; set; }
        public Dictionary<string, int> Losses { get; set; }
        public Dictionary<string, double> WinRate { get; set; }
    }
}
