using BattleshipSimulator.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BattleshipSimulator
{
    public interface IBoardSimulator : IResetable
    {
        public enum HitState { None, Hit, Sunk }

        public bool HaveLost { get; }
        public List<Point> Shots { get; }
        public List<Point> Hits { get; }

        public IBoard Board { get; }
        
        public HitState Fire(Point location);
    }
}
