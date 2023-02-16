using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBotsShip.Bots
{
    public static class OpponentBuilder
    {
        private static Dictionary<string, IOpponent> _opponents = new Dictionary<string, IOpponent> {
            { "Random", new RandomShotsOpponent() },
            { "Crosshair", new CrosshairOpponent() },
            { "LineExplosion", new LineExplosionOpponent() },
            { "ConditionalLineExplosion", new ConditionalLineExplosionOpponent() },
        };

        public static List<string> OpponentOptions() => _opponents.Keys.ToList();
        public static IOpponent GetOpponent(string name)
        {
            return _opponents[name];
        }
    }
}
