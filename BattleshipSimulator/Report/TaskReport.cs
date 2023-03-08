using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Report
{
    internal class TaskReport
    {
        public string AttackerName { get; set; }
        public int AttackerWon { get; set; }
        public long AttackerProcessingTime { get; set; }
        public int AttackerShots { get; set; }
        public int AttackerHits { get; set; }

        public string DefenderName { get; set; }
        public int DefenderWon { get; set; }
        public long DefenderProcessingTime { get; set; }
        public int DefenderShots { get; set; }
        public int DefenderHits { get; set; }

        public TaskReport(string attackerName, int attackerWon, long attackerProcessingTime, int attackerShots, int attackerHits, string defenderName, int defenderWon, long defenderProcessingTime, int defenderShots, int defenderHits)
        {
            AttackerName = attackerName;
            AttackerWon = attackerWon;
            AttackerProcessingTime = attackerProcessingTime;
            AttackerShots = attackerShots;
            AttackerHits = attackerHits;
            DefenderName = defenderName;
            DefenderWon = defenderWon;
            DefenderProcessingTime = defenderProcessingTime;
            DefenderShots = defenderShots;
            DefenderHits = defenderHits;
        }

        public TaskReport()
        {
            AttackerName = "";
            AttackerWon = 0;
            AttackerProcessingTime = 0;
            AttackerShots = 0;
            AttackerHits = 0;
            DefenderName = "";
            DefenderWon = 0;
            DefenderProcessingTime = 0;
            DefenderShots = 0;
            AttackerHits = 0;
        }
    }
}
