using BattleshipModels;
using BattleshipSimulator;
using BattleshipSimulator.Opponents;
using BattleshipSimulator.Report;
using BattleshipValidators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for SingleBattlesView.xaml
    /// </summary>
    public partial class SingleBattlesView : UserControl
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public SingleBattlesView()
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

            IBattleshipSimulator simulator = new BattleshipSimulator.BattleshipSimulator(IBattleshipSimulator.BoardSelectionMethod.Random);
            _cts = new CancellationTokenSource();

            var result = await simulator.RunSingleSimulationAsync(
                OpponentBuilder.GetOpponent(AttackerNameCombobox.Text),
                OpponentBuilder.GetOpponent(DefenderNameCombobox.Text),
                BoardSelector.Boards.Values.ToList(),
                BoardSelector.Boards.Values.ToList(),
                (e) => { return UpdateSimulationUI(e, Int32.Parse(RefreshRateTextbox.Text)); },
                _cts.Token
                );
            Report(result);            

            EnableSettings();
        }

        private async Task UpdateSimulationUI(IGameSimulator simulator, int refreshRate)
        {
            VisualAttackerModel.Update(simulator.AttackerBoard);
            VisualDefenderModel.Update(simulator.DefenderBoard);
            await Task.Delay(refreshRate);
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

        private void NumbersOnly_TextChanged(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Report(IRunReport report)
        {
            ResultsGrid.ItemsSource = new List<IRunReport>() { report };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var name in OpponentBuilder.OpponentOptions())
            {
                AttackerNameCombobox.Items.Add(name);
                DefenderNameCombobox.Items.Add(name);
            }

            AttackerNameCombobox.SelectedIndex = 0;
            DefenderNameCombobox.SelectedIndex = 0;
        }
    }
}
