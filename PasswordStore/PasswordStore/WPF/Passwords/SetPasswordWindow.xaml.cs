using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for SetPasswordWindow.xaml
    /// </summary>
    public partial class SetPasswordWindow : Page
    {
        public delegate void SetPasswordCloseHandle(bool isOk, string value);
        public event SetPasswordCloseHandle SetPasswordClose;

        public SetPasswordWindow()
        {
            InitializeComponent();
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.SetPasswordClose(false, null);
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.txPassword.Password != this.txConfirmPassword.Password)
            {
                Program.Warning("The passwords are not the same!");
                this.SetPasswordClose(false, null);
            }
            else
            {
                this.SetPasswordClose(true, this.txPassword.Password);
            }
        }
    }
}