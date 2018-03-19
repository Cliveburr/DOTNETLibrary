using System.Windows;
using System.Windows.Input;

namespace PasswordTextStore.WPF.Password
{
    public partial class OpenWindow : Window
    {
        public string GetPassword { get { return txPassword.Password; } }

        public OpenWindow()
        {
            InitializeComponent();

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