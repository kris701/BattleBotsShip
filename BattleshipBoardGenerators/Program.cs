using BattleshipModels;
using BattleshipTools;
using BattleshipValidators;
using System;
using System.Text.Json;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        // Generate simple random boards
        static void Main(string[] args)
        {
            Random rnd = new Random();
            BoardStyles.Styles targetStyle = BoardStyles.Styles.Basic;
            for (int i = 0; i < 100; i++)
            {
                IBoard boardDefinition = BoardStyles.GetStyleDefinition(targetStyle);
                while (!BoardValidator.ValidateBoard(boardDefinition))
                {
                    List<ShipModel> newShips = new List<ShipModel>();
                    for (int j = 0; j < boardDefinition.Ships.Count; j++)
                    {
                        newShips.Add(new ShipModel(
                            boardDefinition.Ships[j].Length,
                            (IShip.OrientationDirection)rnd.Next((int)IShip.OrientationDirection.NS, (int)IShip.OrientationDirection.EW),
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

                if (!Directory.Exists("Boards"))
                    Directory.CreateDirectory("Boards");

                string name = $"Boards/Random-{rnd.Next(0, 999999)}.json";
                while (File.Exists(name))
                    name = $"Boards/Random-{rnd.Next(0, 999999)}.json";
                string data = JsonSerializer.Serialize(boardDefinition);
                File.WriteAllText(name, data);
            }
        }
    }
}