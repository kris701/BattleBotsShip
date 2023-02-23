using BattleshipModels;
using BattleshipTools;

namespace BattleshipSimulator
{
    public interface IBoardSimulator
    {
        public enum HitState { None, Hit, Sunk }

        public bool HaveLost { get; }
        public List<Point> Shots { get; }
        public List<Point> Hits { get; }
        public List<IShip> LostShips { get; }

        public IBoard Board { get; }

        public HitState Fire(Point location);
    }
}
