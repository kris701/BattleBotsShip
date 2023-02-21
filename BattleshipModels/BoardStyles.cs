using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipModels
{
    public static class BoardStyles
    {
        /// <summary>
        /// Basic: 10x10
        ///        1 ship(s) of size 2
        ///        2 ship(s) of size 3
        ///        1 ship(s) of size 4
        ///        1 ship(s) of size 5
        /// </summary>
        public enum Styles { Invalid, Basic }

        public static IBoard GetStyleDefinition(Styles style)
        {
            switch(style)
            {
                case Styles.Basic:
                    return new BoardModel(
                        new List<ShipModel>() {
                            new ShipModel(2),
                            new ShipModel(3),
                            new ShipModel(3),
                            new ShipModel(4),
                            new ShipModel(5)
                        },
                        10,
                        10,
                        Styles.Basic,
                        "Basic",
                        "Desc"
                        );
            }
            throw new ArgumentException("Invalid ship style!");
        }
    }
}
