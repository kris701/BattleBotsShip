namespace BattleshipSimulator
{
    public interface IOpponent : IResetable
    {
        public string Name { get; }
        public void DoMoveOn(IBoardSimulator opponentBoard);
    }
}
