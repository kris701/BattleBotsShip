using BattleshipAIs;
using BattleshipSimulator;
using BattleshipTurnaments;
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
    public partial class TurnamentBattlesView : UserControl
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public TurnamentBattlesView()
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

            DisableSettings();

            var turnament = TurnamentBuilder.GetTurnament(TurnamentStyleCombobox.Text);
            var allOpponents = OpponentBuilder.GetAllOpponents();
            TurnamentProgressBar.Maximum = allOpponents.Count;
            TurnamentProgressBar.Value = 0;

            var result = await turnament.RunTurnamentAsync(
                Int32.Parse(RoundsTextbox.Text),
                allOpponents,
                BoardSelector.Boards.Values.ToList(),
                () => { return UpdateSimulationUI(turnament); },
                _cts.Token
                );

            TurnamentProgressBar.Value = 0;
            EnableSettings();

            WriteReport(result);
        }

        private async Task UpdateSimulationUI(ITurnament turnament)
        {
            TurnamentProgressBar.Maximum = turnament.TotalRuns;
            TurnamentProgressBar.Value = turnament.CurrentRun;
            await Task.Delay(100);
        }

        private void WriteReport(BattleshipTurnaments.Report.IReport report)
        {
            string outStr = $"Results from turnament, after {report.Rounds} rounds:" + Environment.NewLine;

            foreach(var key in report.WinRate.Keys)
            {
                outStr += $"\t{key}: {report.Wins[key]} wins, {report.Losses[key]} looses, {report.WinRate[key]}% win rate" + Environment.NewLine;
            }

            ResultsPanel.Children.Add(new Label()
            {
                Content = outStr
            });
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TurnamentStyleCombobox.Items.Clear();
            foreach (var option in TurnamentBuilder.TurnamentOptions())
                TurnamentStyleCombobox.Items.Add(option);
            TurnamentStyleCombobox.SelectedIndex = 0;
        }

        private void NumbersOnly_TextChanged(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
