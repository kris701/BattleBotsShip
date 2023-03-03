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
using System.Windows.Controls.Primitives;
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
                IsExpanded = true,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Lucida Console")
            };
            if (Directory.Exists("BoardLayouts"))
            {
                DirectoryInfo info = new DirectoryInfo("BoardLayouts");
                AddOptions(root, info);
                LayoutSelector.Items.Add(root);
            }
        }

        private void AddOptions(TreeViewItem parentItem, DirectoryInfo directory)
        {
            var newItem = new TreeViewItem()
            {
                Header = directory.Name,
                IsExpanded = true,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Lucida Console")
            };
            foreach (var subDir in directory.EnumerateDirectories())
            {
                AddOptions(newItem, subDir);
            }
            foreach (var file in directory.EnumerateFiles())
            {
                var fileItem = new CheckBox()
                {
                    Content = file.Name,
                    Tag = file.FullName,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Lucida Console")
                };
                fileItem.Checked += Board_Checked;
                fileItem.Unchecked += Board_Unchecked;
                newItem.Items.Add(fileItem);
            }
            parentItem.Items.Add(newItem);
        }

        private void Board_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox item)
            {
                if (item.Tag is string fullname)
                {
                    if (!Boards.ContainsKey(fullname))
                    {
                        if (!BoardValidator.ValidateBoard(GetBoard(fullname)))
                        {
                            MessageBox.Show("Invalid board!");
                            item.IsChecked = false;
                            return;
                        }

                        Boards.Add(fullname, GetBoard(fullname));
                    }
                }
            }
        }

        private void Board_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox item)
            {
                if (item.Tag is string fullname)
                {
                    if (Boards.ContainsKey(fullname))
                    {
                        Boards.Remove(fullname);
                    }
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

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in LayoutSelector.Items)
            {
                if (item is TreeViewItem treeItem)
                    SetSelfAndChildrenTo(treeItem, true);
            }
        }

        private void UnSelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in LayoutSelector.Items)
            {
                if (item is TreeViewItem treeItem)
                    SetSelfAndChildrenTo(treeItem, false);
            }
        }

        private void SetSelfAndChildrenTo(TreeViewItem parentItem, bool value)
        {
            foreach(var item in parentItem.Items)
            {
                if (item is TreeViewItem treeItem)
                    SetSelfAndChildrenTo(treeItem, value);
                else if (item is CheckBox box)
                {
                    box.IsChecked = value;
                }
            }
        }
    }
}
