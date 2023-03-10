using BattleBotsShip.Views.Models;
using BattleshipSimulator;
using BattleshipSimulator.Opponents;
using BattleshipTournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace BattleBotsShip.Views
{
    /// <summary>
    /// Interaction logic for TurnamentBattlesView.xaml
    /// </summary>
    public partial class TournamentBattlesView : UserControl
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public TournamentBattlesView()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (BoardSelector.Boards.Count == 0)
            {
                MessageBox.Show("Select at least one board!");
                return;
            }
            ResultsGrid.ItemsSource = null;

            DisableSettings();

            var tournament = TournamentBuilder.GetTurnament(TurnamentStyleCombobox.Text);
            int rounds = Int32.Parse(RoundsTextbox.Text);

            List<string> opponents = GetChoosenOpponents();

            StatusProgressbar.Maximum = tournament.GetExpectedRounds(opponents);
            StatusProgressbar.Value = 0;
            tournament.OnOpponentBattleOver += UpdateUI;

            _cts = new CancellationTokenSource();

            var result = await tournament.RunTournamentAsync(
                rounds,
                opponents,
                BoardSelector.Boards.Values.ToList(),
                _cts.Token
                );

            EnableSettings();

            if (!_cts.IsCancellationRequested)
                WriteReport(result);
        }

        private List<string> GetChoosenOpponents()
        {
            List<string> opponents = new List<string>();
            foreach (var child in OpponentsCombobox.Items)
            {
                if (child is CheckBox checkbox)
                {
                    if (checkbox.IsChecked == true)
                        if (checkbox.Content is string opponent)
                            opponents.Add(opponent);
                }
            }
            return opponents;
        }

        private void UpdateUI(string opponentA, string opponentB)
        {
            StatusLabel.Dispatcher.Invoke(new Action(() => {
                StatusLabel.Content = $"Opponent '{opponentA}' finished its battle against '{opponentB}'";
                StatusLabel.ToolTip = StatusLabel.Content;
            }));
            StatusProgressbar.Dispatcher.Invoke(new Action(() => {
                StatusProgressbar.Value++;
            }));
        }

        private void WriteReport(BattleshipTournaments.Report.IRunReport report)
        {
            List<TurnamentReport> reports = new List<TurnamentReport>();
            foreach(var key in report.AvgWinRate.Keys)
            {
                reports.Add(new TurnamentReport(
                    key,
                    report.AvgWins[key],
                    report.AvgLosses[key],
                    report.AvgWinRate[key],
                    report.AvgProcessingTime[key],
                    report.AvgShots[key],
                    report.AvgHits[key],
                    report.AvgShotEfficiency[key],
                    report.AvgScore[key]
                    ));
            }

            List<TurnamentReport> SortedList = reports.OrderByDescending(o => o.AvgScore).ToList();

            ResultsGrid.ItemsSource = SortedList;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
            EnableSettings();
        }

        private void DisableSettings()
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            BoardSelector.IsEnabled = false;
            DisablableSettings.IsEnabled = false;
            DisablableGridTwo.IsEnabled = false;
        }

        private void EnableSettings()
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            BoardSelector.IsEnabled = true;
            DisablableSettings.IsEnabled = true;
            DisablableGridTwo.IsEnabled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TurnamentStyleCombobox.Items.Clear();
            foreach (var option in TournamentBuilder.TurnamentOptions())
                TurnamentStyleCombobox.Items.Add(option);
            TurnamentStyleCombobox.SelectedIndex = 0;

            OpponentsCombobox.Items.Clear();
            foreach (var opponent in OpponentBuilder.OpponentOptions())
                OpponentsCombobox.Items.Add(new CheckBox()
                {
                    Content = opponent,
                    IsChecked = true,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Foreground = Brushes.White
                });
        }

        private void NumbersOnly_TextChanged(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
