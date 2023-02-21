﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipModels
{
    public class ShipModel : IShip
    {
        public int Length { get; }
        public IShip.OrientationDirection Orientation { get; }
        public Point Location { get; }

        public ShipModel(int length, IShip.OrientationDirection orientation, Point location)
        {
            Length = length;
            Orientation = orientation;
            Location = location;
        }
    }
}