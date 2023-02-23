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

        public string DefenderName { get; set; }
        public int DefenderWon { get; set; }
        public long DefenderProcessingTime { get; set; }

        public TaskReport(string attackerName, int attackerWon, long attackerProcessingTime, string defenderName, int defenderWon, long defenderProcessingTime)
        {
            AttackerName = attackerName;
            AttackerWon = attackerWon;
            AttackerProcessingTime = attackerProcessingTime;
            DefenderName = defenderName;
            DefenderWon = defenderWon;
            DefenderProcessingTime = defenderProcessingTime;
        }
    }
}
