using BattleshipAIs;
using BattleshipSimulator;
using BattleshipValidators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace BattleBotsShip
{
    public partial class MainWindow : Window
    {
        private int _rounds = 1;
        private int _refereshRate = 100;
        private bool _stopSimulation = false;
        private bool _isRunning = false;
        private Dictionary<string, BoardModel> _attackerBoards = new Dictionary<string, BoardModel>();
        private Dictionary<string, BoardModel> _defenderBoards = new Dictionary<string, BoardModel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
                return;
            if (_attackerBoards.Keys.Count == 0 || _defenderBoards.Keys.Count == 0)
            {
                MessageBox.Show("Select at least one board!");
                return;
            }

            DisableSettings();
            _stopSimulation = false;

            _rounds = Int32.Parse(RoundsTextbox.Text);
            _refereshRate = Int32.Parse(RefreshRateTextbox.Text);

            _isRunning = true;

            if (VisualizeCheckbox.IsChecked == true) {
                await StartSimulationAsync();
            } else if (VisualizeCheckbox.IsChecked == false)
            {
                StartSimulation();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            EnableSettings();
            _stopSimulation = true;
        }

        private void DisableSettings()
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            BoardSelectorGrid.IsEnabled = false;
            DisablableSettings.IsEnabled = false;
            DisablableGridTwo.IsEnabled = false;
        }

        private void EnableSettings()
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            BoardSelectorGrid.IsEnabled = true;
            DisablableSettings.IsEnabled = true;
            DisablableGridTwo.IsEnabled = true;
        }

        private void NumbersOnly_TextChanged(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void StartSimulation()
        {
            var game = GetBoard();

            List<GameModel.WinnerState> winners = new List<GameModel.WinnerState>();

            for (int i = 0; i < _rounds; i++)
            {
                game.AttackerBoard = _attackerBoards[GetRandomKey(_attackerBoards)];
                game.DefenderBoard = _defenderBoards[GetRandomKey(_defenderBoards)];
                var res = GameModel.WinnerState.None;
                while (res == GameModel.WinnerState.None)
                {
                    res = game.Update();
                }
                game.Reset();
                winners.Add(res);
            }

            Report(winners);

            EnableSettings();

            _isRunning = false;
        }

        private async Task StartSimulationAsync()
        {
            var game = GetBoard();

            List<GameModel.WinnerState> winners = new List<GameModel.WinnerState>();

            for (int i = 0; i < _rounds; i++)
            {
                game.AttackerBoard = _attackerBoards[GetRandomKey(_attackerBoards)];
                game.DefenderBoard = _defenderBoards[GetRandomKey(_defenderBoards)];
                var res = GameModel.WinnerState.None;
                while (res == GameModel.WinnerState.None)
                {
                    res = game.Update();
                    VisualAttackerModel.Update(game.AttackerBoard);
                    VisualDefenderModel.Update(game.DefenderBoard);
                    await Task.Delay(_refereshRate);
                    if (_stopSimulation)
                        break;
                }
                game.Reset();
                if (_stopSimulation)
                    break;
                winners.Add(res);
            }   

            Report(winners);

            EnableSettings();

            _isRunning = false;
        }

        private GameModel GetBoard()
        {
            return new GameModel(
                null,
                OpponentBuilder.GetOpponent(AttackerNameCombobox.Text),
                null,
                OpponentBuilder.GetOpponent(DefenderNameCombobox.Text),
                GameModel.TurnState.Attacker);
        }

        private void Report(List<GameModel.WinnerState> winners)
        {
            int attackerWon = winners.Count(x => x == GameModel.WinnerState.Attacker);
            int defenderWon = winners.Count(x => x == GameModel.WinnerState.Defender);

            ResultsPanel.Children.Add(new Label()
            {
                Content = $"Attackers won {attackerWon} times and defender {defenderWon} times"
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLayoutSelector();

            foreach(var name in OpponentBuilder.OpponentOptions())
            {
                AttackerNameCombobox.Items.Add(name);
                DefenderNameCombobox.Items.Add(name);
            }

            AttackerNameCombobox.SelectedIndex = 0;
            DefenderNameCombobox.SelectedIndex = 0;
        }

        private void UpdateLayoutSelector()
        {
            LayoutSelector.Items.Clear();
            var root = new TreeViewItem() { 
                Header = "Root",
                IsExpanded = true
            };
            DirectoryInfo info = new DirectoryInfo("BoardLayouts");
            AddOptions(root, info);
            LayoutSelector.Items.Add(root);
        }

        private void AddOptions(TreeViewItem parentItem, DirectoryInfo directory)
        {
            var newItem = new TreeViewItem()
            {
                Header = directory.Name,
                IsExpanded = true
            };
            foreach(var subDir in directory.EnumerateDirectories())
            {
                AddOptions(newItem, subDir);
            }
            foreach (var file in directory.EnumerateFiles())
            {
                var fileItem = new TreeViewItem()
                {
                    Header = file.Name,
                    Tag = file.FullName,
                    IsExpanded = true
                };
                fileItem.MouseDoubleClick += ToggleBoard_Click;
                newItem.Items.Add(fileItem);
            }
            parentItem.Items.Add(newItem);
        }

        private void ToggleBoard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem item) {
                if (item.Tag is string fullname) {
                    if (_attackerBoards.ContainsKey(fullname))
                    {
                        _attackerBoards.Remove(fullname);
                        _defenderBoards.Remove(fullname);
                        item.Background = Brushes.Transparent;
                    }
                    else
                    {
                        BoardValidator validator = new BoardValidator();
                        validator.ValidateBoard(GetBoard(fullname));

                        _attackerBoards.Add(fullname, GetBoard(fullname));
                        _defenderBoards.Add(fullname, GetBoard(fullname));
                        item.Background = Brushes.LightGreen;
                    }
                    item.IsSelected = false;
                }
            }
        }

        private BoardModel GetBoard(string file)
        {
            var text = File.ReadAllText(file);
            var model = JsonSerializer.Deserialize<BoardModel>(text);
            if (model == null)
                throw new ArgumentNullException("Invalid board!");
            return model;
        }

        Random _rnd = new Random();
        private string GetRandomKey(Dictionary<string, BoardModel> dict)
        {
            return dict.Keys.ToList()[_rnd.Next(0,dict.Keys.Count)];
        }
    }
}
