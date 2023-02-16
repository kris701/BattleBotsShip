﻿using BattleBotsShip.Views;
using System.Windows;

namespace BattleBotsShip
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SingleBattlesButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewGrid.Children.Clear();
            MainViewGrid.Children.Add(new SingleBattlesView());
        }

        private void TurnamentsButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewGrid.Children.Clear();
        }
    }
}
