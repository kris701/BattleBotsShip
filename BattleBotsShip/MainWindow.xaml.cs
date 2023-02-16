using BattleBotsShip.Bots;
using BattleBotsShip.Models;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            _stopSimulation = false;

            _rounds = Int32.Parse(RoundsTextbox.Text);
            _refereshRate = Int32.Parse(RefreshRateTextbox.Text);

            if (VisualizeCheckbox.IsChecked == true) {
                await StartSimulationAsync();
            } else if (VisualizeCheckbox.IsChecked == false)
            {
                StartSimulation();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            _stopSimulation = true;
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
                var res = GameModel.WinnerState.None;
                while (res == GameModel.WinnerState.None)
                {
                    res = game.Update();
                }
                winners.Add(res);
                game.Reset();
            }

            Report(winners);
        }

        private async Task StartSimulationAsync()
        {
            var game = GetBoard();

            List<GameModel.WinnerState> winners = new List<GameModel.WinnerState>();

            for (int i = 0; i < _rounds; i++)
            {
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
                if (_stopSimulation)
                    break;
                winners.Add(res);
                game.Reset();
            }

            Report(winners);
        }

        private GameModel GetBoard()
        {
            BoardModel board1 = JsonSerializer.Deserialize<BoardModel>(File.ReadAllText("board1.json"));
            BoardModel board2 = JsonSerializer.Deserialize<BoardModel>(File.ReadAllText("board2.json"));
            return new GameModel(
                board1,
                new RandomShotsOpponent(),
                board2,
                new RandomShotsOpponent(),
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
    }
}
