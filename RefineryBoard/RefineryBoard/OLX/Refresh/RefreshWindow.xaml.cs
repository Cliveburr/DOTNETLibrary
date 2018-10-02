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
using System.Windows.Shapes;

namespace RefineryBoard.OLX.Refresh
{
    /// <summary>
    /// Interaction logic for RefreshWindow.xaml
    /// </summary>
    public partial class RefreshWindow : Window
    {
        private RequestControl _control;
        private bool _isRunning;

        public RefreshWindow(RequestControl control)
        {
            InitializeComponent();

            _control = control;
            tbPage.Text = RequestEnginer.Page.ToString();
            DataContext = Program.Data;
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
            {
                _control.Cancel = true;
                _isRunning = false;
            }
            else
            {
                _control.Cancel = false;
                _control.OnPage = Page =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        tbPage.Text = Page.ToString();
                    });
                };
                RequestEnginer.Page = uint.Parse(tbPage.Text);
                _isRunning = true;
                Task.Run(() => RequestEnginer.Get(_control));
            }
        }
    }
}
