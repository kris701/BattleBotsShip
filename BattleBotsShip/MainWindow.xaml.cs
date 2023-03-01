using BattleBotsShip.Views;
using System.Windows;

namespace BattleBotsShip
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ChangeView(UIElement newElement)
        {
            MainViewGrid.Children.Clear();
            MainViewGrid.Children.Add(newElement);
            MainMenuGrid.Visibility = Visibility.Hidden;
        }

        private void SingleAIBattlesButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(new SingleBattlesView());
        }

        private void SingleIserBattlesButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(new SingleUserBattlesView());
        }

        private void TournamentBattlesButon_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(new TournamentBattlesView());
        }

        private void BoardDesignerButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(new BoardDesignerView());
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewGrid.Children.Clear();
            MainMenuGrid.Visibility = Visibility.Visible;
        }
    }
}
