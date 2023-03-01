using BattleshipModels;
using BattleshipSimulator;
using BattleshipSimulator.Opponents;
using BattleshipTools;
using BattleshipValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipSimulatorTests.Opponents
{
    [TestClass]
    public class GeneralOpponentTests
    {
        private static readonly int _testBoardsToGenerate = 50;
        public static IEnumerable<object[]> AllOpponents()
        {
            foreach (var opponent in OpponentBuilder.OpponentOptions())
            {
                for (int i = 0; i < _testBoardsToGenerate; i++)
                {
                    yield return new object[] {
                        opponent,
                        GenerateRandomBoard()
                    };
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(AllOpponents), DynamicDataSourceType.Method)]
        public void Opponent_Property_AllwaysFire_Test(string opponentName, IBoard testBoard)
        {
            // ARRANGE
            var opponent = OpponentBuilder.GetOpponent(opponentName);
            IBoardSimulator simulator = new BoardSimulator(testBoard);

            // ACT
            int previousShots = 0;
            while (simulator.Shots.Count != (testBoard.Width * testBoard.Height))
            {
                opponent.DoMoveOn(simulator);
                if (simulator.HaveLost)
                    break;
                if (simulator.Shots.Count != previousShots + 1)
                {
                    Assert.Fail($"Opponent ({opponentName}) did not always fire!");
                }
                previousShots++;
            }
        }

        [TestMethod]
        [DynamicData(nameof(AllOpponents), DynamicDataSourceType.Method)]
        public async Task Opponent_Property_AllwaysFire_Test_Async(string opponentName, IBoard testBoard)
        {
            // ARRANGE
            var opponent = OpponentBuilder.GetOpponent(opponentName);
            IBoardSimulator simulator = new BoardSimulator(testBoard);

            // ACT
            int previousShots = 0;
            while (simulator.Shots.Count != (testBoard.Width * testBoard.Height))
            {
                await opponent.DoMoveOnAsync(simulator, new CancellationToken());
                if (simulator.HaveLost)
                    break;
                if (simulator.Shots.Count != previousShots + 1)
                {
                    Assert.Fail($"Opponent ({opponentName}) did not always fire!");
                }
                previousShots++;
            }
        }

        [TestMethod]
        [DynamicData(nameof(AllOpponents), DynamicDataSourceType.Method)]
        public void Opponent_Property_CanAlwaysWin_Test(string opponentName, IBoard testBoard)
        {
            // ARRANGE
            var opponent = OpponentBuilder.GetOpponent(opponentName);
            IBoardSimulator simulator = new BoardSimulator(testBoard);

            // ACT
            while (simulator.Shots.Count != (testBoard.Width * testBoard.Height))
            {
                opponent.DoMoveOn(simulator);
                if (simulator.HaveLost)
                    break;
            }

            // ASSERT
            if (!simulator.HaveLost)
                Assert.Fail($"Opponent ({opponentName}) did not always win!");
        }

        [TestMethod]
        [DynamicData(nameof(AllOpponents), DynamicDataSourceType.Method)]
        public async Task Opponent_Property_CanAlwaysWin_Test_Async(string opponentName, IBoard testBoard)
        {
            // ARRANGE
            var opponent = OpponentBuilder.GetOpponent(opponentName);
            IBoardSimulator simulator = new BoardSimulator(testBoard);

            // ACT
            while (simulator.Shots.Count != (testBoard.Width * testBoard.Height))
            {
                await opponent.DoMoveOnAsync(simulator, new CancellationToken());
                if (simulator.HaveLost)
                    break;
            }

            // ASSERT
            if (!simulator.HaveLost)
                Assert.Fail($"Opponent ({opponentName}) did not always win!");
        }

        [TestMethod]
        [DynamicData(nameof(AllOpponents), DynamicDataSourceType.Method)]
        public void Opponent_Property_DoesNotTamperWithBoardOrShips_Test(string opponentName, IBoard testBoard)
        {
            // ARRANGE
            var opponent = OpponentBuilder.GetOpponent(opponentName);
            IBoardSimulator simulator = new BoardSimulator(testBoard);
            Assert.IsFalse(testBoard.HaveBeenTamperedWith);
            foreach(var ship in testBoard.Ships)
                Assert.IsFalse(ship.HaveBeenTamperedWith);

            // ACT
            while (simulator.Shots.Count != (testBoard.Width * testBoard.Height))
            {
                opponent.DoMoveOn(simulator);
                if (simulator.HaveLost)
                    break;
            }

            // ASSERT
            Assert.IsFalse(testBoard.HaveBeenTamperedWith);
            foreach (var ship in testBoard.Ships)
                Assert.IsFalse(ship.HaveBeenTamperedWith);
        }

        [TestMethod]
        [DynamicData(nameof(AllOpponents), DynamicDataSourceType.Method)]
        public async Task Opponent_Property_DoesNotTamperWithBoardOrShips_Test_Async(string opponentName, IBoard testBoard)
        {
            // ARRANGE
            var opponent = OpponentBuilder.GetOpponent(opponentName);
            IBoardSimulator simulator = new BoardSimulator(testBoard);
            Assert.IsFalse(testBoard.HaveBeenTamperedWith);
            foreach (var ship in testBoard.Ships)
                Assert.IsFalse(ship.HaveBeenTamperedWith);

            // ACT
            while (simulator.Shots.Count != (testBoard.Width * testBoard.Height))
            {
                await opponent.DoMoveOnAsync(simulator, new CancellationToken());
                if (simulator.HaveLost)
                    break;
            }

            // ASSERT
            Assert.IsFalse(testBoard.HaveBeenTamperedWith);
            foreach (var ship in testBoard.Ships)
                Assert.IsFalse(ship.HaveBeenTamperedWith);
        }

        private static Random rnd = new Random();
        private static IBoard GenerateRandomBoard()
        {
            IBoard boardDefinition = BoardStyles.GetStyleDefinition(BoardStyles.Styles.Basic);
            do
            {
                List<ShipModel> newShips = new List<ShipModel>();
                for (int j = 0; j < boardDefinition.Ships.Count; j++)
                {
                    newShips.Add(new ShipModel(
                        boardDefinition.Ships[j].Length,
                        (IShip.OrientationDirection)rnd.Next((int)IShip.OrientationDirection.NS, (int)IShip.OrientationDirection.EW + 1),
                        new Point(
                            rnd.Next(0, boardDefinition.Width),
                            rnd.Next(0, boardDefinition.Height)
                            )));
                }
                boardDefinition = new BoardModel(
                    newShips,
                    boardDefinition.Width,
                    boardDefinition.Height,
                    boardDefinition.Style,
                    "Random Generated Board",
                    "");
            }
            while (!BoardValidator.ValidateBoard(boardDefinition));
            return boardDefinition;
        }
    }
}
