using BattleshipModels;
using BattleshipTools;

namespace BattleshipSimulator
{
    public interface IBoardSimulator
    {
        public enum HitState { None, Hit, Sunk }

        public bool HaveLost { get; }
        public HashSet<Point> Shots { get; }
        public HashSet<Point> Hits { get; }
        public HashSet<IShip> LostShips { get; }

        public IBoard Board { get; }

        public HitState Fire(Point location);
    }
}
