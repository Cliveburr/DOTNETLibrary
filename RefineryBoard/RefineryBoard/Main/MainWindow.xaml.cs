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

namespace RefineryBoard.Main
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Program.Close();
        }

        private void AuctionActivity_Click(object sender, RoutedEventArgs e)
        {
            var actionTab = new Auction.Init.AuctionPage();

            var newFrame = new Frame();
            newFrame.Content = actionTab;

            var newTab = new TabItem();
            newTab.Content = newFrame;
            newTab.Header = "Auction";

            tabActions.Items.Add(newTab);
        }

        private void OLXActivity_Click(object sender, RoutedEventArgs e)
        {
            var actionTab = new OLX.Init.OLXPage();

            var newFrame = new Frame();
            newFrame.Content = actionTab;

            var newTab = new TabItem();
            newTab.Content = newFrame;
            newTab.Header = "OLX";

            tabActions.Items.Add(newTab);
        }
    }
}
