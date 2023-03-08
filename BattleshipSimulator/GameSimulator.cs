using static BattleshipSimulator.IGameSimulator;

namespace BattleshipSimulator
{
    public class GameSimulator : IGameSimulator
    {
        public bool IsInitialized { get; internal set; } = false;

        public IBoardSimulator AttackerBoard { get; set; }
        public IOpponent AttackerOpponent { get; }
        public IBoardSimulator DefenderBoard { get; set; }
        public IOpponent DefenderOpponent { get; }
        public TurnState Turn { get; internal set; }

        public GameSimulator(IBoardSimulator attackerBoard, IOpponent attackerOpponent, IBoardSimulator defenderBoard, IOpponent defenderOpponent)
        {
            AttackerBoard = attackerBoard;
            AttackerOpponent = attackerOpponent;
            DefenderBoard = defenderBoard;
            DefenderOpponent = defenderOpponent;
            Turn = TurnState.Attacker;
        }

        public void Initialize()
        {
            if (!AttackerOpponent.IsInitialized)
                AttackerOpponent.Initialize(DefenderBoard);
            if (!DefenderOpponent.IsInitialized)
                DefenderOpponent.Initialize(AttackerBoard);
            IsInitialized = true;
        }

        public WinnerState Update()
        {
            if (!IsInitialized)
                throw new Exception("The game simulator have not been initialized!");

            if (Turn == TurnState.Attacker)
            {
                AttackerOpponent.DoMoveOn(DefenderBoard);
                if (DefenderBoard.HaveLost)
                    return WinnerState.Attacker;
                Turn = TurnState.Defender;
            }
            else
            {
                DefenderOpponent.DoMoveOn(AttackerBoard);
                if (AttackerBoard.HaveLost)
                    return WinnerState.Defender;
                Turn = TurnState.Attacker;
            }
            return WinnerState.None;
        }

        public async Task<WinnerState> UpdateAsync(CancellationToken token)
        {
            if (!IsInitialized)
                throw new Exception("The game simulator have not been initialized!");

            if (Turn == TurnState.Attacker)
            {
                await AttackerOpponent.DoMoveOnAsync(DefenderBoard, token);
                if (DefenderBoard.HaveLost)
                    return WinnerState.Attacker;
                Turn = TurnState.Defender;
            }
            else
            {
                await DefenderOpponent.DoMoveOnAsync(AttackerBoard, token);
                if (AttackerBoard.HaveLost)
                    return WinnerState.Defender;
                Turn = TurnState.Attacker;
            }
            return WinnerState.None;
        }
    }
}
