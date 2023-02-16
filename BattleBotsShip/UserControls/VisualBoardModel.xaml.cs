using BattleshipSimulator;
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

namespace BattleBotsShip.UserControls
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

        public void Update(BoardModel board)
        {
            if (MainGrid.RowDefinitions.Count != board.Height)
            {
                MainGrid.RowDefinitions.Clear();
                for (int i = 0; i < board.Height; i++)
                    MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            if (MainGrid.ColumnDefinitions.Count != board.Width)
            {
                MainGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < board.Width; i++)
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

            foreach(var ship in board.Ships)
            {
                var color = Brushes.Blue;
                if (ship.IsSunk)
                    color = Brushes.Red;

                if (ship.Orientation == ShipModel.OrientationDirection.EW)
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
                } else if (ship.Orientation == ShipModel.OrientationDirection.NS)
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
