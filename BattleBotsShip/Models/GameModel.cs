using BattleBotsShip.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BattleBotsShip.Models
{
    public class GameModel : IResetable
    {
        public enum WinnerState { None, Attacker, Defender }
        public enum TurnState { None, Attacker, Defender }

        public BoardModel AttackerBoard { get; set; }
        [JsonIgnore]
        public IOpponent AttackerBot { get; }
        public BoardModel DefenderBoard { get; set; }
        [JsonIgnore]
        public IOpponent DefenderBot { get; }
        private TurnState _originalTurn;
        public TurnState Turn { get; internal set; }

        public GameModel(BoardModel attackerBoard, IOpponent attackerBot, BoardModel defenderBoard, IOpponent defenderBot, TurnState turn)
        {
            AttackerBoard = attackerBoard;
            AttackerBot = attackerBot;
            DefenderBoard = defenderBoard;
            DefenderBot = defenderBot;
            Turn = turn;
            _originalTurn = turn;
        }

        public WinnerState Update()
        {
            if (Turn == TurnState.Attacker) {
                AttackerBot.FireOn(DefenderBoard);
                if (DefenderBoard.HaveLost)
                    return WinnerState.Attacker;
                Turn = TurnState.Defender;
            } else
            {
                DefenderBot.FireOn(AttackerBoard);
                if (AttackerBoard.HaveLost)
                    return WinnerState.Defender;
                Turn = TurnState.Attacker;
            }
            return WinnerState.None;
        }

        public void Reset()
        {
            AttackerBoard.Reset();
            AttackerBot.Reset();
            DefenderBoard.Reset();
            DefenderBot.Reset();
            Turn = _originalTurn;
        }
    }
}
