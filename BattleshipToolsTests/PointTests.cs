using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipToolsTests
{
    [TestClass]
    public class PointTests
    {
        #region Constructor 

        [TestMethod]
        [DataRow(1,5)]
        [DataRow(153512,-21532)]
        [DataRow(-22153512,-21532)]
        public void Point_Constructor(int x, int y)
        {
            // ARRANGE
            // ACT
            var point = new Point(x,y);

            // ASSERT
            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
        }

        [TestMethod]
        public void Point_Constructor_Empty()
        {
            // ARRANGE
            // ACT
            var point = new Point();

            // ASSERT
            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
        }

        #endregion

        #region Equals

        [TestMethod]
        [DataRow(1,1)]
        [DataRow(10,-1)]
        [DataRow(-10,-1531)]
        public void Point_Equals_True(int x, int y)
        {
            // ARRANGE
            var point1 = new Point(x,y);
            var point2 = new Point(x,y);

            // ASSERT
            Assert.IsTrue(point1.Equals(point2));
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, -1)]
        [DataRow(-10, -1531)]
        public void Point_Equals_False_1(int x, int y)
        {
            // ARRANGE
            var point1 = new Point(x, y);

            // ASSERT
            Assert.IsFalse(point1.Equals(null));
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, -1)]
        [DataRow(-10, -1531)]
        public void Point_Equals_False_2(int x, int y)
        {
            // ARRANGE
            var point1 = new Point(x, y);

            // ASSERT
            Assert.IsFalse(point1.Equals("abc"));
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, -1)]
        [DataRow(-10, -1531)]
        public void Point_Equals_False_3(int x, int y)
        {
            // ARRANGE
            var point1 = new Point(x, y);
            var point2 = new Point(x, y + 1);

            // ASSERT
            Assert.IsFalse(point1.Equals(point2));
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, -1)]
        [DataRow(-10, -1531)]
        public void Point_Equals_False_4(int x, int y)
        {
            // ARRANGE
            var point1 = new Point(x, y);
            var point2 = new Point(x + 1, y);

            // ASSERT
            Assert.IsFalse(point1.Equals(point2));
        }

        #endregion

        #region GetHashCode

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, -1)]
        [DataRow(-10, -1531)]
        public void Point_GetHashCode_Same(int x, int y)
        {
            // ARRANGE
            var point1 = new Point(x, y);
            var point2 = new Point(x, y);

            // ASSERT
            Assert.AreEqual(point1.GetHashCode(), point2.GetHashCode());
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, -1)]
        [DataRow(-10, -1531)]
        public void Point_GetHashCode_NotSame_1(int x, int y)
        {
            // ARRANGE
            var point1 = new Point(x, y);
            var point2 = new Point(x, y + 1);

            // ASSERT
            Assert.AreNotEqual(point1.GetHashCode(), point2.GetHashCode());
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, -1)]
        [DataRow(-10, -1531)]
        public void Point_GetHashCode_NotSame_2(int x, int y)
        {
            // ARRANGE
            var point1 = new Point(x, y);
            var point2 = new Point(x + 1, y);

            // ASSERT
            Assert.AreNotEqual(point1.GetHashCode(), point2.GetHashCode());
        }

        #endregion
    }
}
