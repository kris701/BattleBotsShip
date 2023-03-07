using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipToolsTests
{
    [TestClass]
    public class BoundToolsTests
    {
        [TestMethod]
        [DataRow(0,0, 10, 10)]
        [DataRow(1,0, 10, 10)]
        [DataRow(0, 1, 10, 10)]
        [DataRow(1, 1, 10, 10)]

        [DataRow(9, 9, 10, 10)]
        [DataRow(8, 9, 10, 10)]
        [DataRow(9, 8, 10, 10)]
        [DataRow(8, 8, 10, 10)]

        public void IsWithinBounds_True(int x, int y, int width, int height)
        {
            // ARRANGE
            var point = new Point(x,y);

            // ACT
            Assert.IsTrue(BoundTools.IsWithinBounds(width, height, point));
        }

        [TestMethod]
        [DataRow(-1, 0, 10, 10)]
        [DataRow(0, -1, 10, 10)]
        [DataRow(-1, -1, 10, 10)]

        [DataRow(10, 9, 10, 10)]
        [DataRow(9, 10, 10, 10)]
        [DataRow(10, 10, 10, 10)]

        public void IsWithinBounds_False(int x, int y, int width, int height)
        {
            // ARRANGE
            var point = new Point(x, y);

            // ACT
            Assert.IsFalse(BoundTools.IsWithinBounds(width, height, point));
        }
    }
}
