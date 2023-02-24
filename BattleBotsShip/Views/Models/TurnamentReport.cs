﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBotsShip.Views.Models
{
    public class TurnamentReport
    {
        public string AI { get; set; }
        public int Won { get; set; }
        public int Lost { get; set; }
        public double Winrate { get; set; }
        public long ProcessingTime { get; set; }
        public int Shots { get; set; }
        public int Hits { get; set; }
        public double ShotEfficiency { get; set; }

        public TurnamentReport(string aI, int won, int lost, double winrate, long processingTime, int shots, int hits, double shotEfficiency)
        {
            AI = aI;
            Won = won;
            Lost = lost;
            Winrate = winrate;
            ProcessingTime = processingTime;
            Shots = shots;
            Hits = hits;
            ShotEfficiency = shotEfficiency;
        }
    }
}
