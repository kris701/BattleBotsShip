using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleshipSimulator.Opponents.RandomBased
{
    /// <summary>
    /// When a ship is it, it will check in lines in each direction.
    /// The twist with this one is if one of the lines out hit something, it will continue from there afterwards
    /// </summary>
    public class ContinousLineExplosionOpponent : IOpponent
    {
        public string Name { get; } = "Continous Line Explosion";

        private List<Point> _targetPoints;
        private int _fireState = 0;
        private int _reach = 1;

        public ContinousLineExplosionOpponent()
        {
            _targetPoints = new List<Point>();
        }

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (_targetPoints.Count == 0)
            {
                Point firePoint = RndTools.GetRndNewPoint(opponentBoard.Board.Width, opponentBoard.Board.Height, opponentBoard.Shots);
                if (opponentBoard.Fire(firePoint) >= IBoardSimulator.HitState.Hit)
                {
                    _fireState = 0;
                    _targetPoints.Add(firePoint);
                    _reach = 1;
                }
            }
            else
            {
                Point firePoint = new Point(-1, -1);

                while (true)
                {
                    if (_fireState == 0)
                        firePoint = new Point(_targetPoints[0].X, _targetPoints[0].Y - _reach);
                    else if (_fireState == 1)
                        firePoint = new Point(_targetPoints[0].X + _reach, _targetPoints[0].Y);
                    else if (_fireState == 2)
                        firePoint = new Point(_targetPoints[0].X, _targetPoints[0].Y + _reach);
                    else if (_fireState == 3)
                        firePoint = new Point(_targetPoints[0].X - _reach, _targetPoints[0].Y);

                    if (BoundTools.IsWithinBounds(opponentBoard.Board.Width, opponentBoard.Board.Height, firePoint))
                    {
                        _reach++;
                        if (!opponentBoard.Shots.Contains(firePoint))
                            break;
                    }
                    else
                    {
                        _fireState++;
                        _reach = 1;
                    }

                    if (_fireState >= 4)
                    {
                        _targetPoints.RemoveAt(0);
                        if (_targetPoints.Count == 0)
                        {
                            Reset();
                            DoMoveOn(opponentBoard);
                            return;
                        }

                        _fireState = 1;
                        _reach = 1;
                    }
                }

                var hitRes = opponentBoard.Fire(firePoint);
                if (hitRes >= IBoardSimulator.HitState.Hit)
                    _targetPoints.Add(firePoint);
            }
        }

        public async Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token)
        {
            await Task.Run(() => DoMoveOn(opponentBoard));
        }

        public void Reset()
        {
            _targetPoints.Clear();
            _fireState = 0;
            _reach = 1;
        }
    }
}
