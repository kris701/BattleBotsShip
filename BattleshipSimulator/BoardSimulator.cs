using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BattleshipSimulator.DataModels;

namespace BattleshipSimulator
{
    public class BoardSimulator : IBoardSimulator
    {
        public List<Point> Shots { get; }
        public List<Point> Hits { get; }

        public IBoard Board { get; }

        private int _lostShips = 0;
        public bool HaveLost { get { return _lostShips >= Board.Ships.Count; } }

        private Dictionary<IShip, int> _shipHits;

        public BoardSimulator(IBoard board)
        {
            Board = board;
            Shots = new List<Point>();
            Hits = new List<Point>();
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
                    _lostShips++;
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
            _shipHits.Clear();
            _lostShips = 0;
        }
    }
}
