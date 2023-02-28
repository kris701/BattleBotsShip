using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBotsShip.Views.Models
{
    public class TurnamentReport
    {
        public string AI { get; set; }
        public double AvgScore { get; set; }
        public double AvgWinrate { get; set; }
        public double AvgShotEfficiency { get; set; }
        public long AvgProcessingTime { get; set; }
        public int AvgWon { get; set; }
        public int AvgLost { get; set; }
        public int AvgShots { get; set; }
        public int AvgHits { get; set; }

        public TurnamentReport(string aI, int won, int lost, double winrate, long processingTime, int shots, int hits, double shotEfficiency, double score)
        {
            AI = aI;
            AvgWon = won;
            AvgLost = lost;
            AvgWinrate = winrate;
            AvgProcessingTime = processingTime;
            AvgShots = shots;
            AvgHits = hits;
            AvgShotEfficiency = shotEfficiency;
            AvgScore = score;
        }
    }
}
