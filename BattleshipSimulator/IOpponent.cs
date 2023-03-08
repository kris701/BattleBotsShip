namespace BattleshipSimulator
{
    public interface IOpponent
    {
        public string Name { get; }
        public bool IsInitialized { get; }

        public void Initialize(IBoardSimulator opponentBoard);
        public void DoMoveOn(IBoardSimulator opponentBoard);
        public Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token);
    }
}
