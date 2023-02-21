using BattleshipAIs;
using BattleshipModels;
using BattleshipSimulator;
using BattleshipValidators;
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
    /// Interaction logic for UserOpponentBoard.xaml
    /// </summary>
    public partial class UserOpponentBoard : UserControl
    {
        private UserOpponent? _user;

        public UserOpponentBoard()
        {
            InitializeComponent();
        }

        public void Update(IBoardSimulator board, UserOpponent opponent, bool showShips = true)
        {
            _user = opponent;

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

            for (int x = 0; x < board.Board.Width; x++)
            {
                for(int y = 0; y < board.Board.Height; y++) 
                {
                    var button = new Button()
                    {
                        Margin = new Thickness(0)
                    };
                    button.Click += FireOn_Click;
                    Grid.SetRow(button, y);
                    Grid.SetColumn(button, x);

                    MainGrid.Children.Add(button);
                }
            }

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

            if (showShips)
            {
                foreach (var ship in board.Board.Ships)
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
                    }
                    else if (ship.Orientation == IShip.OrientationDirection.NS)
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
            else
            {
                foreach (var ship in board.Board.Ships)
                {
                    if (board.LostShips.Contains(ship))
                    {
                        var color = Brushes.Red;

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
                        }
                        else if (ship.Orientation == IShip.OrientationDirection.NS)
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

        private void FireOn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.IsEnabled)
                {
                    if (_user != null)
                    {
                        int x = Grid.GetColumn(button);
                        int y = Grid.GetRow(button);

                        _user.FireAt = new BattleshipTools.Point(x, y);

                        button.IsEnabled = false;
                    }
                }
            }
        }
    }
}
