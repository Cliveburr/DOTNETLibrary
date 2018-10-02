using RefineryBoard.OLX.Data;
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

namespace RefineryBoard.OLX.Init
{
    /// <summary>
    /// Interaction logic for LeftPage.xaml
    /// </summary>
    public partial class LeftPage : Page
    {
        public LeftPage(OLXModel model)
        {
            InitializeComponent();

            DataContext = model;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = e.Source as DataGrid;
            var selected = dataGrid.SelectedItem as OfferData;

            var model = DataContext as OLXModel;
            model.Selected = selected;
            model.RaiseNotify("Selected");
        }
    }
}
