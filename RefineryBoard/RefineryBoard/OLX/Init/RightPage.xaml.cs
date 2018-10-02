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
    /// Interaction logic for RightPage.xaml
    /// </summary>
    public partial class RightPage : Page
    {
        public RightPage(OLXModel model)
        {
            InitializeComponent();

            DataContext = model;
            RefreshValuePerGram_KeyDown(null, null);
        }

        private void RefreshValuePerGram_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e != null && e.Key != Key.Enter)
                    return;

                var model = DataContext as OLXModel;
                if (model.Selected == null)
                    return;

                //decimal price = 0;
                //decimal.TryParse(model.Selected.Price, System.Globalization.NumberStyles.AllowCurrencySymbol | System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
                var price = decimal.Parse(model.Selected.Price, System.Globalization.NumberStyles.AllowCurrencySymbol | System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);

                var weight = decimal.Parse(model.Selected.Weight, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
                if (weight > 0)
                {
                    var vpg = price / weight;
                    tbValuePerGram.Text = vpg.ToString("#0.00");
                }
                else
                {
                    tbValuePerGram.Text = "";
                }
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }
    }
}
