using BattleshipModels;
using BattleshipTools;
using Nanotek.Helpers;
using static BattleshipSimulator.IBoardSimulator;

namespace BattleshipSimulator
{
    public class BoardSimulator : IBoardSimulator
    {
        public HashSet<Point> Shots { get; }
        public HashSet<Point> Hits { get; }

        public IBoard Board { get; }

        public bool HaveLost { get { return LostShips.Count >= Board.Ships.Count; } }
        public HashSet<IShip> LostShips { get; }

        private Dictionary<IShip, int> _shipHits;

        public BoardSimulator(IBoard board)
        {
            Board = board;
            Shots = new HashSet<Point>();
            Hits = new HashSet<Point>();
            LostShips = new HashSet<IShip>();
            _shipHits = new Dictionary<IShip, int>();
        }

        public HitState Fire(Point location)
        {
            Shots.Add(location);
            var hitShip = Board.GetHit(location);
            if (hitShip != null && !Hits.Contains(location))
            {
                DictionaryHelper.AddOrIncrement(_shipHits, hitShip, 1);
                Hits.Add(location);
                if (_shipHits[hitShip] == hitShip.Length)
                {
                    if (hitShip.Clone() is IShip ship)
                        LostShips.Add(ship);
                    return HitState.Sunk;
                }
                else
                    return HitState.Hit;
            }
            return HitState.None;
        }
    }
}
