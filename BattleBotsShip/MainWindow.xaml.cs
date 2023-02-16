using BattleBotsShip.Bots;
using BattleBotsShip.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RoundsTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                _rounds = Int32.Parse(box.Text);
            }
        }

        private void StartSimulation()
        {
            var game = GetBoard();

            List<GameModel.WinnerState> winners = new List<GameModel.WinnerState>();

            for (int i = 0; i < _rounds; i++)
            {
                var res = game.Update();
                while (res == GameModel.WinnerState.None)
                {
                    res = game.Update();
                }
                winners.Add(res);
                game.Reset();
            }

            int attackerWon = winners.Count(x => x == GameModel.WinnerState.Attacker);
            int defenderWon = winners.Count(x => x == GameModel.WinnerState.Defender);

            ResultsPanel.Children.Add(new Label()
            {
                Content = $"Attackers won {attackerWon} times and defender {defenderWon} times"
            });
        }

        private async Task StartSimulationAsync()
        {
            var game = GetBoard();

            List<GameModel.WinnerState> winners = new List<GameModel.WinnerState>();

            for (int i = 0; i < _rounds; i++)
            {
                var res = game.Update();
                while (res == GameModel.WinnerState.None)
                {
                    res = game.Update();
                    VisualAttackerModel.Update(game.AttackerBoard);
                    VisualDefenderModel.Update(game.DefenderBoard);
                    await Task.Delay(100);
                }
                winners.Add(res);
                game.Reset();
                await Task.Delay(1000);
            }

            int attackerWon = winners.Count(x => x == GameModel.WinnerState.Attacker);
            int defenderWon = winners.Count(x => x == GameModel.WinnerState.Defender);

            ResultsPanel.Children.Add(new Label()
            {
                Content = $"Attackers won {attackerWon} times and defender {defenderWon} times"
            });
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
    }
}
