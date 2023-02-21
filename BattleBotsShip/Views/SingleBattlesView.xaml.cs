using BattleshipAIs;
using BattleshipModels;
using BattleshipSimulator;
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
        CancellationTokenSource _cts = new CancellationTokenSource();
        private Dictionary<string, IBoard> _boards = new Dictionary<string, IBoard>();

        public SingleBattlesView()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_boards.Count == 0)
            {
                MessageBox.Show("Select at least one board!");
                return;
            }

            DisableSettings();

            IBattleshipSimulator simulator = new BattleshipSimulator.BattleshipSimulator(IBattleshipSimulator.BoardSelectionMethod.Random);
            _cts = new CancellationTokenSource();

            if (VisualizeCheckbox.IsChecked == true)
            {
                var result = await simulator.RunSumulationAsync(
                    Int32.Parse(RoundsTextbox.Text),
                    OpponentBuilder.GetOpponent(AttackerNameCombobox.Text),
                    OpponentBuilder.GetOpponent(DefenderNameCombobox.Text),
                    _boards.Values.ToList(),
                    () => { return UpdateSimulationUI(simulator, Int32.Parse(RefreshRateTextbox.Text)); },
                    _cts.Token
                    );
                Report(result);
            }
            else if (VisualizeCheckbox.IsChecked == false)
            {
                var result = simulator.RunSumulation(
                    Int32.Parse(RoundsTextbox.Text),
                    OpponentBuilder.GetOpponent(AttackerNameCombobox.Text),
                    OpponentBuilder.GetOpponent(DefenderNameCombobox.Text),
                    _boards.Values.ToList()
                    );
                Report(result);
            }

            EnableSettings();
        }

        private async Task UpdateSimulationUI(IBattleshipSimulator simulator, int refreshRate)
        {
            if (simulator.CurrentGame != null)
            {
                VisualAttackerModel.Update(simulator.CurrentGame.AttackerBoard);
                VisualDefenderModel.Update(simulator.CurrentGame.DefenderBoard);
            }
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

        private void Report(IReport report)
        {
            ResultsPanel.Children.Add(new Label()
            {
                Content = $"Attacker ({report.AttackerName}) won {report.AttackerWon} times and defender ({report.DefenderName}) {report.DefenderWon} times"
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLayoutSelector();

            foreach (var name in OpponentBuilder.OpponentOptions())
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
            var root = new TreeViewItem()
            {
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
            foreach (var subDir in directory.EnumerateDirectories())
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
            if (sender is TreeViewItem item)
            {
                if (item.Tag is string fullname)
                {
                    if (_boards.ContainsKey(fullname))
                    {
                        _boards.Remove(fullname);
                        item.Background = Brushes.Transparent;
                    }
                    else
                    {
                        BoardValidator validator = new BoardValidator();
                        validator.ValidateBoard(GetBoard(fullname));

                        _boards.Add(fullname, GetBoard(fullname));
                        item.Background = Brushes.LightGreen;
                    }
                    item.IsSelected = false;
                }
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

        Random _rnd = new Random();
        private string GetRandomKey(Dictionary<string, BoardModel> dict)
        {
            return dict.Keys.ToList()[_rnd.Next(0, dict.Keys.Count)];
        }
    }
}
