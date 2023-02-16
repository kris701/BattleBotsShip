using BattleBotsShip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BattleBotsShip.UserControls
{
    /// <summary>
    /// Interaction logic for VisualBoardModel.xaml
    /// </summary>
    public partial class VisualBoardModel : UserControl
    {
        public VisualBoardModel()
        {
            InitializeComponent();
        }

        public void Update(BoardModel board)
        {
            if (MainGrid.RowDefinitions.Count != board.Height)
            {
                MainGrid.RowDefinitions.Clear();
                for (int i = 0; i < board.Height; i++)
                    MainGrid.RowDefinitions.Add(new RowDefinition());
            }
        }
    }
}
