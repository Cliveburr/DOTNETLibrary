using PasswordStore.Config;
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

namespace PasswordStore.WPF.Password
{
    public partial class PasswordEditWindow : WindowBase, IDisposable
    {
        public override ConfigWindowIDEnum ID => ConfigWindowIDEnum.PasswordEdit;

        private PasswordItemContext _context;
        private bool isShow;

        public PasswordEditWindow(PasswordItemContext item)
        {
            InitializeComponent();

            _context = item;
            DataContext = _context;
        }

        private bool Validate()
        {
            var passwords = GetPasswords();
            if (passwords.Item1 != passwords.Item2)
            {
                Program.Warning("The passwords are not the same!");
                return false;
            }

            if (string.IsNullOrEmpty(_context.Value) && string.IsNullOrEmpty(passwords.Item1))
            {
                Program.Warning("Need to set a valid password!");
                return false;
            }
            else
            {
                _context.Value = passwords.Item1;
            }

            return true;
        }

        private Tuple<string, string> GetPasswords()
        {
            if (isShow)
            {
                return new Tuple<string, string>(txPasswordShow.Text, txConfirmPasswordShow.Text);
            }
            else
            {
                return new Tuple<string, string>(txPassword.Password, txConfirmPassword.Password);
            }
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                DialogResult = true;
                Close();
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public void Dispose()
        {
        }

        private void cbShowPasswordText_Change(object sender, RoutedEventArgs e)
        {
            var checkbox = e.Source as CheckBox;
            isShow = checkbox.IsChecked ?? false;
            if (isShow)
            {
                txPassword.Visibility = Visibility.Collapsed;
                txPasswordShow.Visibility = Visibility.Visible;
                txPasswordShow.Text = txPassword.Password;

                txConfirmPassword.Visibility = Visibility.Collapsed;
                txConfirmPasswordShow.Visibility = Visibility.Visible;
                txConfirmPasswordShow.Text = txConfirmPassword.Password;
            }
            else
            {
                txPasswordShow.Visibility = Visibility.Collapsed;
                txPassword.Visibility = Visibility.Visible;
                txPassword.Password = txPasswordShow.Text;

                txConfirmPasswordShow.Visibility = Visibility.Collapsed;
                txConfirmPassword.Visibility = Visibility.Visible;
                txConfirmPassword.Password = txConfirmPasswordShow.Text;
            }
        }
    }
}