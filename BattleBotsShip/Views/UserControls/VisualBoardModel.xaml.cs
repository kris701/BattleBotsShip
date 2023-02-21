using BattleshipSimulator;
using BattleshipSimulator.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BattleBotsShip.Views.UserControls
{
    /// <summary>
    /// Interaction logic for VisualBoardModel.xaml
    /// </summary>
    public partial class VisualBoardModel : UserControl
    {
        public VisualBoardModel()
        {
            InitializeComponent();
        }

        public void Update(IBoardSimulator board)
        {
            if (MainGrid.RowDefinitions.Count != board.Board.Height)
            {
                MainGrid.RowDefinitions.Clear();
                for (int i = 0; i < board.Board.Height; i++)
                    MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            if (MainGrid.ColumnDefinitions.Count != board.Board.Width)
            {
                MainGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < board.Board.Width; i++)
                    MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            MainGrid.Children.Clear();
            foreach (var hitPoint in board.Shots)
            {
                var canvas = new Canvas()
                {
                    Background = Brushes.Gray
                };
                Grid.SetRow(canvas, (int)hitPoint.Y);
                Grid.SetColumn(canvas, (int)hitPoint.X);

                MainGrid.Children.Add(canvas);
            }

            foreach (var hitPoint in board.Hits)
            {
                var canvas = new Canvas()
                {
                    Background = Brushes.Black
                };
                Grid.SetRow(canvas, (int)hitPoint.Y);
                Grid.SetColumn(canvas, (int)hitPoint.X);

                MainGrid.Children.Add(canvas);
            }

            foreach(var ship in board.Board.Ships)
            {
                var color = Brushes.Blue;

                if (ship.Orientation == IShip.OrientationDirection.EW)
                {
                    var canvas = new Canvas()
                    {
                        Background = color,
                        Margin = new Thickness(5)
                    };
                    Grid.SetColumnSpan(canvas, ship.Length);
                    Grid.SetRow(canvas, (int)ship.Location.Y);
                    Grid.SetColumn(canvas, (int)ship.Location.X);

                    MainGrid.Children.Add(canvas);
                } else if (ship.Orientation == IShip.OrientationDirection.NS)
                {
                    var canvas = new Canvas()
                    {
                        Background = color,
                        Margin = new Thickness(5)
                    };
                    Grid.SetRowSpan(canvas, ship.Length);
                    Grid.SetRow(canvas, (int)ship.Location.Y);
                    Grid.SetColumn(canvas, (int)ship.Location.X);

                    MainGrid.Children.Add(canvas);
                }
            }
        }
    }
}
