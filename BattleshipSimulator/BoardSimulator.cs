using BattleshipModels;
using BattleshipTools;
using static BattleshipSimulator.IBoardSimulator;

namespace BattleshipSimulator
{
    public class BoardSimulator : IBoardSimulator
    {
        public List<Point> Shots { get; }
        public List<Point> Hits { get; }

        public IBoard Board { get; }

        public bool HaveLost { get { return LostShips.Count >= Board.Ships.Count; } }
        public List<IShip> LostShips { get; }

        private Dictionary<IShip, int> _shipHits;

        public BoardSimulator(IBoard board)
        {
            Board = board;
            Shots = new List<Point>();
            Hits = new List<Point>();
            LostShips = new List<IShip>();
            _shipHits = new Dictionary<IShip, int>();
        }

        public HitState Fire(Point location)
        {
            Shots.Add(location);
            var hitPositions = Board.GetHitPositions();
            if (hitPositions.ContainsKey(location) && !Hits.Contains(location))
            {
                var ship = hitPositions[location];
                if (_shipHits.ContainsKey(ship))
                    _shipHits[ship]++;
                else
                    _shipHits.Add(ship, 1);
                Hits.Add(location);
                if (_shipHits[ship] == ship.Length)
                {
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
