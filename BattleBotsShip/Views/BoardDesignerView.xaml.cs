using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BattleBotsShip.Views
{
    /// <summary>
    /// Interaction logic for BoardDesignerView.xaml
    /// </summary>
    public partial class BoardDesignerView : UserControl
    {
        private bool _isLoaded = false;

        public BoardDesignerView()
        {
            InitializeComponent();
        }

        private void NumbersOnly_TextChanged(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateGrid();
            _isLoaded = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateBoardGrid(object sender, TextChangedEventArgs e)
        {
            if (_isLoaded)
                GenerateGrid();
        }

        private void GenerateGrid()
        {
            if (WidthTextbox.Text == "" || HeightTextbox.Text == "")
                return;
            int width = Int32.Parse(WidthTextbox.Text);
            int height = Int32.Parse(HeightTextbox.Text);
            if (DesignGrid.RowDefinitions.Count != height)
            {
                DesignGrid.RowDefinitions.Clear();
                for (int i = 0; i < height; i++)
                    DesignGrid.RowDefinitions.Add(new RowDefinition());
            }

            if (DesignGrid.ColumnDefinitions.Count != width)
            {
                DesignGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < width; i++)
                    DesignGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
    }
}
