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
        public Dictionary<string, double> WinRates { get; set; }
    }
}
