using BattleshipModels;
using BattleshipValidators;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;

namespace BattleBotsShip.Views
{
    /// <summary>
    /// Interaction logic for BoardDesignerView.xaml
    /// </summary>
    public partial class BoardDesignerView : UserControl
    {
        private bool _isLoaded = false;
        private IBoard? _currentStyle;

        public BoardDesignerView()
        {
            InitializeComponent();
        }

        private void NumbersOnly_TextChanged(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StyleCombobox.Items.Clear();
            foreach (var option in Enum.GetNames(typeof(BoardStyles.Styles)).Skip(1))
                StyleCombobox.Items.Add(option);
            StyleCombobox.SelectedIndex = 0;

            GenerateGrid();

            _isLoaded = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStyle != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Json files (*.json)|*.json";
                if (saveFileDialog.ShowDialog() == true)
                {
                    _currentStyle.Name = saveFileDialog.SafeFileName.Replace(".json","");
                    string data = JsonSerializer.Serialize(_currentStyle);
                    File.WriteAllText(saveFileDialog.FileName, data);
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                _currentStyle = GetBoard(openFileDialog.FileName);
                UpdateField();
            }
        }

        private IBoard GetBoard(string file)
        {
            var text = File.ReadAllText(file);
            var model = JsonSerializer.Deserialize<BoardModel>(text);
            if (model == null)
                throw new ArgumentNullException("Invalid board!");
            return model;
        }

        private void GenerateGrid()
        {
            _currentStyle = BoardStyles.GetStyleDefinition((BoardStyles.Styles)Enum.ToObject(typeof(BoardStyles.Styles), (StyleCombobox.SelectedIndex + 1)));

            UpdateField();
        }

        private void UpdateField()
        {
            if (_currentStyle != null)
            {
                SetGridRows(DesignGrid, _currentStyle.Height);
                SetGridColumns(DesignGrid, _currentStyle.Width);

                // Make Tiles
                DesignGrid.Children.Clear();
                for (int x = 0; x < _currentStyle.Width; x++)
                {
                    for (int y = 0; y < _currentStyle.Height; y++)
                    {
                        Canvas tile = new Canvas();
                        tile.Background = Brushes.LightGray;
                        tile.AllowDrop = true;
                        tile.Tag = new Point(x, y);
                        tile.Drop += Tile_Drop;

                        Grid.SetRow(tile, y);
                        Grid.SetColumn(tile, x);
                        DesignGrid.Children.Add(tile);
                    }
                }

                // Place Ships
                var invalidShips = BoardValidator.GetInvalidShips(_currentStyle);
                int shipNumber = 0;
                foreach (var ship in _currentStyle.Ships)
                {
                    Canvas newShip = new Canvas()
                    {
                        Background = Brushes.Red,
                        Margin = new Thickness(5),
                        Tag = shipNumber
                    };
                    newShip.MouseMove += (o, e) => {
                        if (o is Canvas canvas)
                        {
                            if (canvas.Tag is int shipIndex)
                            {
                                if (e.LeftButton == MouseButtonState.Pressed)
                                {
                                    DragDrop.DoDragDrop(canvas, shipIndex, DragDropEffects.Move);
                                }
                            }
                        }
                    };
                    newShip.MouseRightButtonDown += RotateCanvasClick_Drop;

                    if (ship.Orientation == IShip.OrientationDirection.NS)
                    {
                        Grid.SetColumn(newShip, ship.Location.X);
                        Grid.SetColumnSpan(newShip, 1);
                        Grid.SetRow(newShip, ship.Location.Y);
                        Grid.SetRowSpan(newShip, ship.Length);
                    }
                    else if (ship.Orientation == IShip.OrientationDirection.EW)
                    {
                        Grid.SetColumn(newShip, ship.Location.X);
                        Grid.SetColumnSpan(newShip, ship.Length);
                        Grid.SetRow(newShip, ship.Location.Y);
                        Grid.SetRowSpan(newShip, 1);
                    }

                    if (invalidShips.Contains(ship))
                        newShip.Background = Brushes.Red;
                    else
                        newShip.Background = Brushes.Blue;

                    DesignGrid.Children.Add(newShip);
                    shipNumber++;
                }
            }
        }

        private void Tile_Drop(object sender, DragEventArgs e)
        {
            if (sender is Canvas tile)
            {
                if (tile.Tag is Point loc)
                {
                    if (_currentStyle != null)
                    {
                        int currentShipIndex = (int)e.Data.GetData(typeof(int));
                        var newLoc = new BattleshipTools.Point((int)loc.X, (int)loc.Y);
                        _currentStyle.Ships[currentShipIndex] = new ShipModel(
                            _currentStyle.Ships[currentShipIndex].Length,
                            _currentStyle.Ships[currentShipIndex].Orientation,
                            newLoc
                            );

                        UpdateField();
                    }
                }
            }
        }

        private void RotateCanvasClick_Drop(object sender, MouseEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                if (_currentStyle != null)
                {
                    if (canvas.Tag is int currentShipIndex)
                    {
                        var newOrientation = IShip.OrientationDirection.None;
                        if (_currentStyle.Ships[currentShipIndex].Orientation == IShip.OrientationDirection.EW)
                            newOrientation = IShip.OrientationDirection.NS;
                        else
                            newOrientation = IShip.OrientationDirection.EW;
                        _currentStyle.Ships[currentShipIndex] = new ShipModel(
                            _currentStyle.Ships[currentShipIndex].Length,
                            newOrientation,
                            _currentStyle.Ships[currentShipIndex].Location
                            );

                        UpdateField();
                    }
                }
            }
        }

        private void SetGridColumns(Grid grid, int to)
        {
            if (grid.ColumnDefinitions.Count != to)
            {
                grid.ColumnDefinitions.Clear();
                for (int i = 0; i < to; i++)
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void SetGridRows(Grid grid, int to)
        {
            if (grid.RowDefinitions.Count != to)
            {
                grid.RowDefinitions.Clear();
                for (int i = 0; i < to; i++)
                    grid.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void StyleCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded)
                GenerateGrid();
        }
    }
}
