﻿using System;
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

namespace MW2_Statistics_Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            lboxPlayers.ItemsSource = Database.GetPlayers();
            if (lboxPlayers.Items.Count > 0)
                lboxPlayers.SelectedIndex = 0;
        }

        private void lboxMatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Player p = (Player)e.AddedItems[0];
            induvidualPlayerView.LoadPlayerInfoInControl(p);
        }
    }
}
