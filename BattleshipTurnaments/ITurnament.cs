﻿using BattleshipModels;
using BattleshipSimulator;
using BattleshipTurnaments.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTurnaments
{
    public interface ITurnament
    {
        public IReport RunTurnament(int rounds, List<string> opponents, List<IBoard> boardOptions);
        public Task<IReport> RunTurnamentAsync(int rounds, List<string> opponents, List<IBoard> boardOptions, CancellationToken cancellationToken);
    }
}
