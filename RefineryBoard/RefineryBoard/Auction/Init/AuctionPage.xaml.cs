using RefineryBoard.Activity;
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

namespace RefineryBoard.Auction.Init
{
    /// <summary>
    /// Interaction logic for ActionPage.xaml
    /// </summary>
    public partial class AuctionPage : ActivityBase
    {
        private Page _leftPage;

        public AuctionPage()
        {
            InitializeComponent();

            InitBlank();
        }

        public void InitBlank()
        {
            var model = new AuctionModel();
            DataContext = model;

            _leftPage = new ChooseEdital();
            frLeft.Navigate(_leftPage);
        }


    }
}
