using BattleBotsShip.Views.Models;
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
            ResultsGrid.ItemsSource = null;

            DisableSettings();

            var turnament = TurnamentBuilder.GetTurnament(TurnamentStyleCombobox.Text);
            var allOpponents = OpponentBuilder.GetAllOpponentNames();
            _cts = new CancellationTokenSource();

            var result = await turnament.RunTurnamentAsync(
                Int32.Parse(RoundsTextbox.Text),
                allOpponents,
                BoardSelector.Boards.Values.ToList(),
                _cts.Token
                );

            EnableSettings();

            if (!_cts.IsCancellationRequested)
                WriteReport(result);
        }

        private void WriteReport(BattleshipTurnaments.Report.IReport report)
        {
            List<TurnamentReport> reports = new List<TurnamentReport>();
            foreach(var key in report.WinRate.Keys)
            {
                reports.Add(new TurnamentReport(
                    key,
                    report.Wins[key],
                    report.Losses[key],
                    report.WinRate[key],
                    report.ProcessingTime[key]
                    ));
            }

            List<TurnamentReport> SortedList = reports.OrderByDescending(o => o.Winrate).ToList();

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
