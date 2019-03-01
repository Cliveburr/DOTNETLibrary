using PasswordStore.User;
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

namespace PasswordStore.WPF.Passwords
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Page
    {
        //public delegate void SetHistoryCloseHandle(bool isOk, List<PasswordHistoryData> value);
        //public event SetHistoryCloseHandle SetHistoryClose;
        //public ObservableCollection<PasswordHistoryData> History { get; set; }

        //public HistoryWindow(List<PasswordHistoryData> history)
        //{
        //    InitializeComponent();

        //    this.txValue.Binding = new Binding
        //    {
        //        Path = new PropertyPath("Value"),
        //        Converter = new HistoryPasswordConverter(this)
        //    };

        //    this.History = new ObservableCollection<PasswordHistoryData>(history);
        //    this.dgHistory.ItemsSource = this.History;
        //}

        private void chShowAsText_Checked(object sender, RoutedEventArgs e)
        {
            this.dgHistory.Items.Refresh();
        }

        public bool ShwoAsText { get { return this.chShowAsText.IsChecked.HasValue ? this.chShowAsText.IsChecked.Value : false; } }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            //this.SetHistoryClose(false, null);
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            //this.SetHistoryClose(true, this.History.ToList());
        }
    }

    public class HistoryPasswordConverter : IValueConverter
    {
        private HistoryWindow _history;

        public HistoryPasswordConverter(HistoryWindow history)
        {
            this._history = history;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var text = value as string;

            if (this._history.ShwoAsText)
                return text;
            else
                return new String('*', text.Length);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}