using BattleshipModels;
using BattleshipSimulator;
using BattleshipSimulator.Opponents;
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
        public static IEnumerable<string[]> AllOpponents()
        {
            foreach(var opponent in OpponentBuilder.OpponentOptions())
                yield return new string[] { opponent };
        }

        [TestMethod]
        [DynamicData(nameof(AllOpponents), DynamicDataSourceType.Method)]
        public void Opponent_Property_AllwaysFire_Test(string opponentName)
        {
            // ARRANGE
            var opponent = OpponentBuilder.GetOpponent(opponentName);
            IBoard testBoard = BoardStyles.GetStyleDefinition(BoardStyles.Styles.Basic);
            IBoardSimulator simulator = new BoardSimulator(testBoard);

            // ACT
            int previousShots = 0;
            while (simulator.Shots.Count != (testBoard.Width * testBoard.Height))
            {
                opponent.DoMoveOn(simulator);
                if (simulator.Shots.Count <= previousShots)
                {
                    Assert.Fail($"Opponent ({opponentName}) did not always fire!");
                }
                previousShots++;
            }
        }

        [TestMethod]
        [DynamicData(nameof(AllOpponents), DynamicDataSourceType.Method)]
        public void Opponent_Property_CanAlwaysWin_Test(string opponentName)
        {
            // ARRANGE
            var opponent = OpponentBuilder.GetOpponent(opponentName);
            IBoard testBoard = BoardStyles.GetStyleDefinition(BoardStyles.Styles.Basic);
            IBoardSimulator simulator = new BoardSimulator(testBoard);

            // ACT
            while (simulator.Shots.Count != (testBoard.Width * testBoard.Height))
            {
                opponent.DoMoveOn(simulator);
            }
            if (!simulator.HaveLost)
                Assert.Fail($"Opponent ({opponentName}) did not always win!");
        }
    }
}
