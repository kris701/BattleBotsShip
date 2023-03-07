using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipToolsTests
{
    [TestClass]
    public class RndToolsTests
    {
        [TestMethod]
        [DataRow(10,10)]
        [DataRow(100,100)]
        [DataRow(1,100)]
        [DataRow(5,1351)]
        public void GetRndNewPoint_CanGetAny(int width, int height)
        {
            // ARRANGE
            HashSet<Point> points = new HashSet<Point>();

            // ACT
            var result = RndTools.GetRndNewPoint(width, height, points);

            // ASSERT
            Assert.IsTrue(BoundTools.IsWithinBounds(width, height, result));
        }

        [TestMethod]
        [DataRow(10, 10)]
        [DataRow(100, 100)]
        [DataRow(1, 100)]
        [DataRow(5, 1351)]
        public void GetRndNewPoint_DoesNotGetOneThatsTaken(int width, int height)
        {
            // ARRANGE
            Random rnd = new Random();
            Point expectedPoint = new Point(rnd.Next(0, width), rnd.Next(0, height));

            HashSet<Point> points = new HashSet<Point>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var point = new Point(x, y);
                    if (!expectedPoint.Equals(point))
                        points.Add(point);
                }
            }
               
            // ACT
            var result = RndTools.GetRndNewPoint(width, height, points);

            // ASSERT
            Assert.AreEqual(expectedPoint, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetRndNewPoint_ThrowsIfWidthLessThanZero()
        {
            // ARRANGE
            HashSet<Point> points = new HashSet<Point>();

            // ACT
            RndTools.GetRndNewPoint(-1, 10, points);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetRndNewPoint_ThrowsIfHeightLessThanZero()
        {
            // ARRANGE
            HashSet<Point> points = new HashSet<Point>();

            // ACT
            RndTools.GetRndNewPoint(10, -1, points);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        [DataRow(10,10)]
        [DataRow(10,1)]
        [DataRow(1,1)]
        public void GetRndNewPoint_ThrowsIfNoMoreShotsPossible_1(int width, int height)
        {
            // ARRANGE
            HashSet<Point> points = new HashSet<Point>();
            for (int i = 0; i < width * height; i++)
                points.Add(new Point(0, i));

            // ACT
            RndTools.GetRndNewPoint(width, height, points);
        }
    }
}
