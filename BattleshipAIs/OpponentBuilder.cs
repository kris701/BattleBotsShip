﻿using BattleshipAIs.PatternBased;
using BattleshipAIs.ProbabilityBased;
using BattleshipAIs.RandomBased;
using BattleshipSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipAIs
{
    public static class OpponentBuilder
    {
        private static Dictionary<string, IOpponent> _opponents = new Dictionary<string, IOpponent> {
            // Random Based
            { "Random", new RandomShotsOpponent() },
            { "Crosshair", new CrosshairOpponent() },
            { "LineExplosion", new LineExplosionOpponent() },
            { "ConditionalLineExplosion", new ConditionalLineExplosionOpponent() },

            // Probability Based
            //{ "ProbableShots", new ProbableShotsOpponent() },

            // Pattern Based
            { "GridCLE", new GridCLEOpponent() },
        };

        public static List<string> OpponentOptions() => _opponents.Keys.ToList();
        public static IOpponent GetOpponent(string name)
        {
            return _opponents[name];
        }
        public static List<IOpponent> GetAllOpponents() => _opponents.Values.ToList();
    }
}
