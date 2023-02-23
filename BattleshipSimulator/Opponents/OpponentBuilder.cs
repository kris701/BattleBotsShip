using BattleshipSimulator;
using BattleshipSimulator.Opponents.PatternBased;
using BattleshipSimulator.Opponents.ProbabilityBased;
using BattleshipSimulator.Opponents.RandomBased;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents
{
    public static class OpponentBuilder
    {
        private static Dictionary<string, Func<IOpponent>> _opponents = new Dictionary<string, Func<IOpponent>> {
            // Random Based
            { "Random", () => { return new RandomShotsOpponent(); } },
            { "Crosshair", () => { return new CrosshairOpponent(); } },
            { "LineExplosion", () => { return new LineExplosionOpponent(); } },
            { "ContinousLineExplosion", () => { return new ContinousLineExplosionOpponent(); } },
            { "ConditionalLineExplosion", () => { return new ConditionalLineExplosionOpponent(); } },

            // Probability Based
            { "ProbableShots", () => { return new ProbableShotsOpponent(); } },
            { "FurthestShotCLE", () => { return new FurthestShotCLEOpponent(); } },

            // Pattern Based
            { "GridCLE", () => { return new GridCLEOpponent(); } },
        };

        public static List<string> OpponentOptions() => _opponents.Keys.ToList();
        public static List<string> GetAllOpponentNames() => _opponents.Keys.ToList();
        public static IOpponent GetOpponent(string name) => _opponents[name]();
    }
}
