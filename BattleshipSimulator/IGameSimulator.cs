namespace BattleshipSimulator
{
    public interface IGameSimulator
    {
        public enum WinnerState { None, Attacker, Defender }
        public enum TurnState { None, Attacker, Defender }

        public IBoardSimulator AttackerBoard { get; set; }
        public IOpponent AttackerOpponent { get; }
        public IBoardSimulator DefenderBoard { get; set; }
        public IOpponent DefenderOpponent { get; }

        public TurnState Turn { get; }

        public WinnerState Update();
        public Task<WinnerState> UpdateAsync(CancellationToken token);
    }
}
