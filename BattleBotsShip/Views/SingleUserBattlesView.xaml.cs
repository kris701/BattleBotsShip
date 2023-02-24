
using BattleshipSimulator.Report;
using BattleshipSimulator;
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
using BattleBotsShip.Views.UserControls;
using BattleshipSimulator.Opponents;
using static BattleshipSimulator.BoardSelector;

namespace BattleBotsShip.Views
{
    /// <summary>
    /// Interaction logic for SingleUserBattlesView.xaml
    /// </summary>
    public partial class SingleUserBattlesView : UserControl
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private UserOpponent _user = new UserOpponent();

        public SingleUserBattlesView()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserBoardSelector.Boards.Count != 1)
            {
                MessageBox.Show("Select only one board as your own!");
                return;
            }
            if (AttackerBoardSelector.Boards.Count <= 0)
            {
                MessageBox.Show("Select at least one board for the attacker!");
                return;
            }

            DisableSettings();
            VisualAttackerModel.IsEnabled = true;

            IBattleshipSimulator simulator = new BattleshipSimulator.BattleshipSimulator(BoardSelectionMethod.Random);
            _cts = new CancellationTokenSource();

            var result = await simulator.RunSingleSimulationAsync(
                OpponentBuilder.GetOpponent(AttackerNameCombobox.Text),
                _user,
                AttackerBoardSelector.Boards.Values.ToList(),
                UserBoardSelector.Boards.Values.ToList(),
                (e) => { return UpdateSimulationUI(e); },
                _cts.Token
                );
            Report(result);

            VisualAttackerModel.IsBoardInitialized = false;
            VisualDefenderModel.IsBoardInitialized = false;

            EnableSettings();
            VisualAttackerModel.IsEnabled = false;
        }

        private async Task UpdateSimulationUI(IGameSimulator simulator)
        {
            if (!VisualAttackerModel.IsBoardInitialized)
                VisualAttackerModel.Initialize(simulator.AttackerBoard, _user, ShowOpponentCheckbox.IsChecked == true);
            if (!VisualDefenderModel.IsBoardInitialized)
                VisualDefenderModel.Initialize(simulator.DefenderBoard);

            VisualAttackerModel.Update(simulator.AttackerBoard);
            VisualDefenderModel.Update(simulator.DefenderBoard);
            await Task.Delay(1);
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
            DisablableGridTwo.IsEnabled = false;
            DisablableSettings.IsEnabled = false;
        }

        private void EnableSettings()
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            BoardSelectorGrid.IsEnabled = true;
            DisablableGridTwo.IsEnabled = true;
            DisablableSettings.IsEnabled = true;
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var name in OpponentBuilder.OpponentOptions())
            {
                AttackerNameCombobox.Items.Add(name);
            }

            AttackerNameCombobox.SelectedIndex = 0;
        }
    }
}
