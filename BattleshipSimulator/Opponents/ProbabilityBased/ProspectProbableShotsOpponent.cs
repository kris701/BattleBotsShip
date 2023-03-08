using BattleshipModels;
using BattleshipSimulator;
using BattleshipTools;
using Nanotek.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulator.Opponents.ProbabilityBased
{
    public class ProspectProbableShotsOpponent : ProbableShotsOpponent
    {
        public override string Name { get; } = "Prospect Probable Shots";
        // Some high value, to increase the likelyhood of the AI attempting to shoot a location that has a partially sunk ship

        private int _currentProspectTargetIndex = 0;
        private List<Point> _prospectTargets = new List<Point>();
        private bool _isInProspectState = true;
        private double _prospectDensity = 0.15;

        public override void Initialize(IBoardSimulator opponentBoard)
        {
            _aliveShipOfLength = GetAliveShipsLengths(opponentBoard);
            PopulateProspectList(opponentBoard.Board.Width, opponentBoard.Board.Height);
            for (int x = 0; x < opponentBoard.Board.Width; x++)
            {
                for (int y = 0; y < opponentBoard.Board.Height; y++)
                {
                    var point = new Point(x, y);
                    if (!_prospectTargets.Contains(point))
                        _availableFirePoints.Add(point);
                }
            }
            IsInitialized = true;
        }

        public override void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (_isInProspectState)
            {
                opponentBoard.Fire(_prospectTargets[_currentProspectTargetIndex++]);
                if (_currentProspectTargetIndex >= _prospectTargets.Count)
                {
                    _isInProspectState = false;
                    _uncoveredPoints = GetUncoveredHitPoints(opponentBoard);
                }
            }
            else
            {
                var res = opponentBoard.Fire(GetBestPoint(opponentBoard));
                if (res >= IBoardSimulator.HitState.Hit)
                    _uncoveredPoints = GetUncoveredHitPoints(opponentBoard);
                if (res == IBoardSimulator.HitState.Sunk)
                    _aliveShipOfLength = GetAliveShipsLengths(opponentBoard);
            }
        }

        private void PopulateProspectList(int width, int height)
        {
            int targetPointCount = (int)((width * height) * _prospectDensity);
            int pointEvery = (int)((double)(width * height) / (double)targetPointCount);

            int pointID = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (pointID % pointEvery == 0)
                    {
                        var newPoint = new Point(x, y);
                        if (BoundTools.IsWithinBounds(width, height, newPoint))
                            _prospectTargets.Add(newPoint);
                    }
                    pointID++;
                }
            }
        }
    }
}
