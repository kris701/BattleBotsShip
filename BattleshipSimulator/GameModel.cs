namespace BattleshipSimulator
{
    public class GameModel : IGameSimulator
    {
        public IBoardSimulator AttackerBoard { get; set; }
        public IOpponent AttackerBot { get; }
        public IBoardSimulator DefenderBoard { get; set; }
        public IOpponent DefenderBot { get; }
        private IGameSimulator.TurnState _originalTurn;
        public IGameSimulator.TurnState Turn { get; internal set; }

        public GameModel(IBoardSimulator attackerBoard, IOpponent attackerBot, IBoardSimulator defenderBoard, IOpponent defenderBot, IGameSimulator.TurnState turn)
        {
            AttackerBoard = attackerBoard;
            AttackerBot = attackerBot;
            DefenderBoard = defenderBoard;
            DefenderBot = defenderBot;
            Turn = turn;
            _originalTurn = turn;
        }

        public IGameSimulator.WinnerState Update()
        {
            if (Turn == IGameSimulator.TurnState.Attacker)
            {
                AttackerBot.DoMoveOn(DefenderBoard);
                if (DefenderBoard.HaveLost)
                    return IGameSimulator.WinnerState.Attacker;
                Turn = IGameSimulator.TurnState.Defender;
            }
            else
            {
                DefenderBot.DoMoveOn(AttackerBoard);
                if (AttackerBoard.HaveLost)
                    return IGameSimulator.WinnerState.Defender;
                Turn = IGameSimulator.TurnState.Attacker;
            }
            return IGameSimulator.WinnerState.None;
        }

        public async Task<IGameSimulator.WinnerState> UpdateAsync(CancellationToken token)
        {
            if (Turn == IGameSimulator.TurnState.Attacker)
            {
                await AttackerBot.DoMoveOnAsync(DefenderBoard, token);
                if (DefenderBoard.HaveLost)
                    return IGameSimulator.WinnerState.Attacker;
                Turn = IGameSimulator.TurnState.Defender;
            }
            else
            {
                await DefenderBot.DoMoveOnAsync(AttackerBoard, token);
                if (AttackerBoard.HaveLost)
                    return IGameSimulator.WinnerState.Defender;
                Turn = IGameSimulator.TurnState.Attacker;
            }
            return IGameSimulator.WinnerState.None;
        }
    }
}
