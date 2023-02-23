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
        private static Dictionary<string, Func<ITurnament>> _turnaments = new Dictionary<string, Func<ITurnament>> {
            { "TowLayerLoop", () => { return new TwoLayerLoop(); } },
        };

        public static List<string> TurnamentOptions() => _turnaments.Keys.ToList();
        public static ITurnament GetTurnament(string name) => _turnaments[name]();
    }
}
