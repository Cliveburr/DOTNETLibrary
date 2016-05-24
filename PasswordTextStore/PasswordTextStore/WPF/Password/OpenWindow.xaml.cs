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

namespace PasswordTextStore.WPF.Password
{
    public partial class OpenWindow : Window
    {
        public string GetPassword { get { return txPassword.Password; } }

        public OpenWindow()
        {
            InitializeComponent();

            txUser.Text = Environment.UserName;

            txPassword.Focus();
        }

        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void txPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}