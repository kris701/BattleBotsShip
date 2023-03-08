using BattleshipModels;
using BattleshipTools;

namespace BattleshipModelsTests
{
    [TestClass]
    public class ShipModelTests
    {
        #region Constructor

        [TestMethod]
        [DataRow(5, IShip.OrientationDirection.EW, 10, 5)]
        [DataRow(200, IShip.OrientationDirection.NS, -5134, 1)]
        public void CanSetConstructor(int length, IShip.OrientationDirection direction, int x, int y)
        {
            // ARRANGE
            // ACT
            IShip ship = new ShipModel(length, direction, new Point(x,y));

            // ASSERT
            Assert.AreEqual(length, ship.Length);
            Assert.AreEqual(direction, ship.Orientation);
            Assert.AreEqual(x, ship.Location.X);
            Assert.AreEqual(y, ship.Location.Y);
        }

        #endregion

        #region Tamper Warning

        [TestMethod]
        [DataRow(IShip.OrientationDirection.EW)]
        [DataRow(IShip.OrientationDirection.NS)]
        [DataRow(IShip.OrientationDirection.None)]
        public void OrientationTriggerTamperWarning(IShip.OrientationDirection direction)
        {
            // ARRANGE
            IShip ship = new ShipModel(10, direction, new Point());
            Assert.IsFalse(ship.HaveBeenTamperedWith);

            // ACT
            var getValue = ship.Orientation;

            // ASSERT
            Assert.IsTrue(ship.HaveBeenTamperedWith);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(-1560, 210)]
        [DataRow(0, 1256346)]
        public void LocationTriggerTamperWarning(int x, int y)
        {
            // ARRANGE
            IShip ship = new ShipModel(10, IShip.OrientationDirection.EW, new Point(x, y));
            Assert.IsFalse(ship.HaveBeenTamperedWith);

            // ACT
            var getValue = ship.Location;

            // ASSERT
            Assert.IsTrue(ship.HaveBeenTamperedWith);
        }

        [TestMethod]
        public void LengthDoesntTriggerTamperWarning()
        {
            // ARRANGE
            IShip ship = new ShipModel(10, IShip.OrientationDirection.EW, new Point());
            Assert.IsFalse(ship.HaveBeenTamperedWith);

            // ACT
            var getValue = ship.Length;

            // ASSERT
            Assert.IsFalse(ship.HaveBeenTamperedWith);
        }

        #endregion

        #region IsPointWithin

        [TestMethod]
        [DataRow(5, IShip.OrientationDirection.EW, 10, 5, 10, 5)]
        [DataRow(200, IShip.OrientationDirection.NS, 0, 0, 0, 199)]
        public void IsPointWithin_True(int length, IShip.OrientationDirection direction, int x, int y, int pointx, int pointy)
        {
            // ARRANGE
            IShip ship = new ShipModel(length, direction, new Point(x, y));
            var checkPoint = new Point(pointx, pointy);

            // ACT
            // ASSERT
            Assert.IsTrue(ship.IsPointWithin(checkPoint));
        }


        [TestMethod]
        [DataRow(5, IShip.OrientationDirection.EW, 10, 5, 10, 6)]
        [DataRow(200, IShip.OrientationDirection.NS, 0, 0, 1, 199)]
        public void IsPointWithin_False(int length, IShip.OrientationDirection direction, int x, int y, int pointx, int pointy)
        {
            // ARRANGE
            IShip ship = new ShipModel(length, direction, new Point(x, y));
            var checkPoint = new Point(pointx, pointy);

            // ACT
            // ASSERT
            Assert.IsFalse(ship.IsPointWithin(checkPoint));
        }

        #endregion

        #region Equals

        [TestMethod]
        [DataRow(5, IShip.OrientationDirection.EW, 10, 5)]
        [DataRow(200, IShip.OrientationDirection.NS, -5134, 1)]
        public void Equals_True(int length, IShip.OrientationDirection direction, int x, int y)
        {
            // ARRANGE
            // ACT
            IShip ship1 = new ShipModel(length, direction, new Point(x, y));
            IShip ship2 = new ShipModel(length, direction, new Point(x, y));

            // ASSERT
            Assert.IsTrue(ship1.Equals(ship2));
            Assert.IsTrue(ship2.Equals(ship1));
            Assert.IsTrue(ship1.Equals(ship1));
            Assert.IsTrue(ship2.Equals(ship2));
        }

        [TestMethod]
        public void Equals_False_1()
        {
            // ARRANGE
            // ACT
            IShip ship1 = new ShipModel(9, IShip.OrientationDirection.NS, new Point(10, 10));
            IShip ship2 = new ShipModel(10, IShip.OrientationDirection.NS, new Point(10, 10));

            // ASSERT
            Assert.IsFalse(ship1.Equals(ship2));
            Assert.IsFalse(ship2.Equals(ship1));
        }

        [TestMethod]
        public void Equals_False_2()
        {
            // ARRANGE
            // ACT
            IShip ship1 = new ShipModel(10, IShip.OrientationDirection.EW, new Point(10, 10));
            IShip ship2 = new ShipModel(10, IShip.OrientationDirection.NS, new Point(10, 10));

            // ASSERT
            Assert.IsFalse(ship1.Equals(ship2));
            Assert.IsFalse(ship2.Equals(ship1));
        }

        [TestMethod]
        public void Equals_False_3()
        {
            // ARRANGE
            // ACT
            IShip ship1 = new ShipModel(10, IShip.OrientationDirection.NS, new Point(10, 10));
            IShip ship2 = new ShipModel(10, IShip.OrientationDirection.NS, new Point(10, 9));

            // ASSERT
            Assert.IsFalse(ship1.Equals(ship2));
            Assert.IsFalse(ship2.Equals(ship1));
        }

        [TestMethod]
        public void Equals_False_4()
        {
            // ARRANGE
            // ACT
            IShip ship1 = new ShipModel(10, IShip.OrientationDirection.NS, new Point(10, 10));

            // ASSERT
            Assert.IsFalse(ship1.Equals(null));
        }

        #endregion
    }
}