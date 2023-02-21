using BattleshipTurnaments.TurnamentStyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments
{
    public static class TurnamentBuilder
    {
        private static Dictionary<string, ITurnament> _turnaments = new Dictionary<string, ITurnament> {
            { "TowLayerLoop", new TwoLayerLoop() },
        };

        public static List<string> TurnamentOptions() => _turnaments.Keys.ToList();
        public static ITurnament GetTurnament(string name)
        {
            return _turnaments[name];
        }
    }
}
