using PasswordStore.User;
using System;
using System.Windows;
using System.Windows.Input;

namespace PasswordStore.WPF.Passwords
{
    public partial class OpenPasswordsWindow : Window, IDisposable
    {
        public string MainPassword { get; private set; }
        public UserData User { get; private set; }

        public OpenPasswordsWindow()
        {
            InitializeComponent();

            txPassword.Focus();
        }

        private void Open()
        {
            var file = UserFile.Open(Program.Config.UserFilePath, txPassword.Password);
            if (file == null)
            {
                Program.Warning("Permission denied!");
            }
            else
            {
                MainPassword = txPassword.Password;
                User = file.Data;

                DialogResult = true;
            }

            Close();
        }

        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Open();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void txPassword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    Open();
                }
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        public void Dispose()
        {
            MainPassword = string.Empty;
            User = null;
        }
    }
}
