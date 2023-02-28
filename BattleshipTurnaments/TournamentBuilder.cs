using BattleshipTurnaments.TurnamentStyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments
{
    public static class TournamentBuilder
    {
        private static Dictionary<string, Func<ITournament>> _turnaments = new Dictionary<string, Func<ITournament>> {
            { "TowLayerLoop", () => { return new TwoLayerLoop(); } },
        };

        public static List<string> TurnamentOptions() => _turnaments.Keys.ToList();
        public static ITournament GetTurnament(string name) => _turnaments[name]();
    }
}
