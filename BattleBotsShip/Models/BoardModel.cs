using BattleBotsShip.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace BattleBotsShip.Models
{
    public class BoardModel : IResetable
    {
        public enum HitState { None, Hit, Sunk }

        public int Width { get; }
        public int Height { get; }

        public List<ShipModel> Ships { get; }
        [JsonIgnore]
        public List<Point> Shots { get; }
        [JsonIgnore]
        public List<Point> Hits { get; }
        private int _lostShips = 0;
        [JsonIgnore]
        public bool HaveLost { get { return _lostShips >= Ships.Count; } }

        public BoardModel(List<ShipModel> ships, int width, int height)
        {
            Width = width; 
            Height = height;
            Ships = ships;
            Shots = new List<Point>();
            Hits = new List<Point>();
        }

        public HitState Fire(Point location)
        {
            foreach(var ship in Ships)
            {
                if (!ship.IsSunk)
                {
                    var hitState = ship.IsHit(location);
                    if (hitState != ShipModel.HitState.None)
                    {
                        Hits.Add(location);
                        Shots.Add(location);
                        if (hitState == ShipModel.HitState.Sunk)
                        {
                            _lostShips++;
                            return HitState.Sunk;
                        }
                        return HitState.Hit;
                    }
                }
            }
            Shots.Add(location);
            return HitState.None;
        }

        public void Reset()
        {
            foreach (var ship in Ships)
                ship.Reset();
            Shots.Clear();
            Hits.Clear();
            _lostShips = 0;
        }
    }
}
