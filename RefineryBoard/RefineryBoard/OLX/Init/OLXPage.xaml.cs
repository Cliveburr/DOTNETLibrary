using RefineryBoard.Activity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace RefineryBoard.OLX.Init
{
    /// <summary>
    /// Interaction logic for OLXPage.xaml
    /// </summary>
    public partial class OLXPage : ActivityBase
    {
        private LeftPage _leftPage;
        private RightPage _rightPage;

        public OLXPage()
        {
            InitializeComponent();

            InitMe();
        }

        public void InitMe()
        {
            var model = new OLXModel
            {
                Offers = new ObservableCollection<Data.OfferData>(Program.Data.Content.OLXOffers)
            };
            DataContext = model;

            _leftPage = new LeftPage(model);
            frLeft.Navigate(_leftPage);

            _rightPage = new RightPage(model);
            frRight.Navigate(_rightPage);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            var control = new Refresh.RequestControl(OnData);
            var refreshWindow = new Refresh.RefreshWindow(control);
            refreshWindow.ShowDialog();
        }

        private void OnData(List<Data.OfferData> data)
        {
            Dispatcher.Invoke(() =>
            {
                var model = DataContext as OLXModel;

                foreach (var item in data)
                {
                    var has = model.Offers
                        .FirstOrDefault(o => o.Code == item.Code);
                    if (has == null)
                    {
                        model.Offers.Add(item);
                    }
                    else
                    {
                        has.Price = item.Price;
                    }
                }

                Save_Click(null, null);
                model.RaiseNotify("Offers");
            });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var model = DataContext as OLXModel;

            Program.Data.Content.OLXOffers = model.Offers.ToList();

            Program.Data.Save();
        }
    }
}
