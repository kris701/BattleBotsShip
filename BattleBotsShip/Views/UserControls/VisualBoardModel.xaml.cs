using BattleshipModels;
using BattleshipSimulator;
using BattleshipTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
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
        private List<BattleshipTools.Point> _hits = new List<BattleshipTools.Point>();
        private List<BattleshipTools.Point> _shots = new List<BattleshipTools.Point>();
        private List<Canvas> _shipElements = new List<Canvas>();

        public bool IsBoardInitialized { get; set; } = false;

        public VisualBoardModel()
        {
            InitializeComponent();
        }

        public void Initialize(IBoardSimulator board, string title)
        {
            MainGrid.Children.Clear();
            TitleLabel.Content = title;
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

            CheckShots(board.Shots, _shots, Brushes.Gray);
            CheckShots(board.Hits, _hits, Brushes.Black);

            foreach (var ship in board.Board.Ships)
            {
                var color = Brushes.Blue;
                if (board.LostShips.Contains(ship))
                    color = Brushes.Red;

                var canvas = GetShipElement(color);
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
                    color = Brushes.Red;

                if (_shipElements[shipIndex].Background != color)
                    _shipElements[shipIndex].Background = color;

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
    }
}
