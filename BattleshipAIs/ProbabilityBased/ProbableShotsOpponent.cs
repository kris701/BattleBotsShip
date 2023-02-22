using BattleshipModels;
using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipAIs.ProbabilityBased
{
    public class ProbableShotsOpponent : IOpponent
    {
        public string Name { get; } = "Probable Shots";

        private int _currentTargetSize = int.MaxValue;
        private Point _lastHit = new Point(-1, -1);

        public void DoMoveOn(IBoardSimulator opponentBoard)
        {
            if (_lastHit.X == -1 && _lastHit.Y == -1)
            {
                opponentBoard.Fire(new Point());
                _lastHit = new Point();
                return;
            }

            if (_currentTargetSize == int.MaxValue)
            {
                foreach(var ship in opponentBoard.Board.Ships)
                {
                    if (!opponentBoard.LostShips.Contains(ship))
                        if (ship.Length < _currentTargetSize)
                            _currentTargetSize = ship.Length;
                }
            }

            var newTargetPoint = GetNewTarget(_lastHit, _currentTargetSize, opponentBoard);
            while (opponentBoard.Shots.Contains(newTargetPoint))
                newTargetPoint = GetNewTarget(_lastHit, _currentTargetSize, opponentBoard);

            _lastHit = newTargetPoint;
            var hitResult = opponentBoard.Fire(newTargetPoint);

            if (hitResult == IBoardSimulator.HitState.Sunk)
                _currentTargetSize = int.MaxValue;
        }

        public async Task DoMoveOnAsync(IBoardSimulator opponentBoard, CancellationToken token)
        {
            await Task.Run(() => DoMoveOn(opponentBoard));
        }

        public void Reset()
        {
            _currentTargetSize = int.MaxValue;
            _lastHit = new Point(-1, -1);
        }

        private Point GetNewTarget(Point lastHit, int targetSize, IBoardSimulator board)
        {
            int currentX = lastHit.X + targetSize;
            int currentY = lastHit.Y;

            if (currentX >= board.Board.Width)
            {
                currentX = 0;
                lastHit.Y++;
            }
            if (currentY >= board.Board.Height)
            {
                return RndTools.GetRndNewPoint(board.Board.Width, board.Board.Height, board.Shots);
            }

            return new Point(currentX, currentY);
        }
    }
}
