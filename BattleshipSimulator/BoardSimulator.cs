using BattleshipModels;
using System.Drawing;

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

        public IBoardSimulator.HitState Fire(Point location)
        {
            Shots.Add(location);
            if (Board.HitPositions.Keys.Contains(location) && !Hits.Contains(location))
            {
                var ship = Board.HitPositions[location];
                if (_shipHits.ContainsKey(ship))
                    _shipHits[ship]++;
                else
                    _shipHits.Add(ship, 1);
                Hits.Add(location);
                if (_shipHits[ship] == ship.Length)
                {
                    LostShips.Add(ship);
                    return IBoardSimulator.HitState.Sunk;
                }
                else
                    return IBoardSimulator.HitState.Hit;
            }
            return IBoardSimulator.HitState.None;
        }

        public void Reset()
        {
            Shots.Clear();
            Hits.Clear();
            LostShips.Clear();
            _shipHits.Clear();
        }
    }
}
