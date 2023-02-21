using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BattleshipSimulator
{
    public interface IGameSimulator : IResetable
    {
        public enum WinnerState { None, Attacker, Defender }
        public enum TurnState { None, Attacker, Defender }

        public IBoardSimulator AttackerBoard { get; set; }
        public IOpponent AttackerBot { get; }
        public IBoardSimulator DefenderBoard { get; set; }
        public IOpponent DefenderBot { get; }

        public TurnState Turn { get; }

        public WinnerState Update();
    }
}
