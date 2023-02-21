using BattleshipModels;
using BattleshipValidators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace BattleBotsShip.Views.UserControls
{
    /// <summary>
    /// Interaction logic for FileSelectorControl.xaml
    /// </summary>
    public partial class FileSelectorControl : UserControl
    {
        public Dictionary<string, IBoard> Boards = new Dictionary<string, IBoard>();

        public FileSelectorControl()
        {
            InitializeComponent();
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateLayoutSelector();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLayoutSelector();
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
                    if (Boards.ContainsKey(fullname))
                    {
                        Boards.Remove(fullname);
                        item.Background = Brushes.Transparent;
                    }
                    else
                    {
                        BoardValidator validator = new BoardValidator();
                        validator.ValidateBoard(GetBoard(fullname));

                        Boards.Add(fullname, GetBoard(fullname));
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
    }
}
