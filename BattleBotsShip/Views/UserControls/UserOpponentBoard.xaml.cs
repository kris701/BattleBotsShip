﻿using BattleshipModels;
using BattleshipSimulator;
using BattleshipSimulator.Opponents;
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
        private List<BattleshipTools.Point> _hits = new List<BattleshipTools.Point>();
        private List<BattleshipTools.Point> _shots = new List<BattleshipTools.Point>();
        private List<Canvas> _shipElements = new List<Canvas>();

        public bool IsBoardInitialized { get; set; } = false;
        private UserOpponent? _user;

        public UserOpponentBoard()
        {
            InitializeComponent();
        }

        public void Initialize(IBoardSimulator board, UserOpponent user, bool showShips)
        {
            MainGrid.Children.Clear();
            _user = user;
            _hits = new List<BattleshipTools.Point>();
            _shots = new List<BattleshipTools.Point>();
            _shipElements = new List<Canvas>();

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

            for (int x = 0; x < board.Board.Width; x++)
            {
                for (int y = 0; y < board.Board.Height; y++)
                {
                    var button = new Button()
                    {
                        Margin = new Thickness(0),
                        Opacity = 0.1
                    };
                    Panel.SetZIndex(button, 9999999);
                    button.Click += FireOn_Click;
                    Grid.SetRow(button, y);
                    Grid.SetColumn(button, x);

                    MainGrid.Children.Add(button);
                }
            }

            CheckShots(board.Shots, _shots, Brushes.Gray);
            CheckShots(board.Hits, _hits, Brushes.Black);

            foreach (var ship in board.Board.Ships)
            {
                var color = Brushes.Blue;
                if (board.LostShips.Contains(ship))
                    color = Brushes.Red;

                var canvas = GetShipElement(color);
                if (showShips)
                    canvas.Visibility = Visibility.Visible;
                else
                    canvas.Visibility = Visibility.Hidden;
                Canvas.SetZIndex(canvas, 999999);

                if (ship.Orientation == IShip.OrientationDirection.EW)
                {
                    Grid.SetColumnSpan(canvas, ship.Length);
                    Grid.SetRow(canvas, (int)ship.Location.Y);
                    Grid.SetColumn(canvas, (int)ship.Location.X);
                }
                else if (ship.Orientation == IShip.OrientationDirection.NS)
                {
                    Grid.SetRowSpan(canvas, ship.Length);
                    Grid.SetRow(canvas, (int)ship.Location.Y);
                    Grid.SetColumn(canvas, (int)ship.Location.X);
                }

                MainGrid.Children.Add(canvas);
                _shipElements.Add(canvas);
            }

            IsBoardInitialized = true;
        }

        public void Update(IBoardSimulator board)
        {
            if (!IsInitialized)
                throw new Exception("Visual board was not initialized!");

            CheckShots(board.Shots, _shots, Brushes.Gray);
            CheckShots(board.Hits, _hits, Brushes.Black);

            int shipIndex = 0;
            foreach (var ship in board.Board.Ships)
            {
                var color = Brushes.Blue;
                if (board.LostShips.Contains(ship))
                {
                    color = Brushes.Red;
                }

                if (_shipElements[shipIndex].Background != color)
                    _shipElements[shipIndex].Background = color;

                if (color == Brushes.Red)
                    _shipElements[shipIndex].Visibility = Visibility.Visible;

                shipIndex++;
            }
        }

        private void CheckShots(List<BattleshipTools.Point> shots, List<BattleshipTools.Point> uiShots, Brush color)
        {
            foreach (var hitPoint in shots)
            {
                if (!uiShots.Contains(hitPoint))
                {
                    uiShots.Add(hitPoint);
                    var canvas = new Canvas()
                    {
                        Background = color
                    };
                    Grid.SetRow(canvas, (int)hitPoint.Y);
                    Grid.SetColumn(canvas, (int)hitPoint.X);

                    MainGrid.Children.Add(canvas);
                }
            }
        }

        private Canvas GetShipElement(Brush color)
        {
            return new Canvas()
            {
                Background = color,
                Margin = new Thickness(5)
            };
        }



        public void _Update(IBoardSimulator board, UserOpponent opponent, bool showShips = true)
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
