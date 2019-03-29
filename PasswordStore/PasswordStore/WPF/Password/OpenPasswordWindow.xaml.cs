using PasswordStore.Config;
using PasswordStore.User;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PasswordStore.WPF.Password
{
    public partial class OpenPasswordWindow : Window, IDisposable
    {
        public string MainPassword { get; private set; }
        public UserFile User { get; private set; }

        public OpenPasswordWindow()
        {
            InitializeComponent();

            txPassword.Focus();
        }

        private void Open()
        {
            User = UserFile.Open(GetFilePath(), txPassword.Password);
            if (User == null)
            {
                Program.Warning("Permission denied!");
            }
            else
            {
                MainPassword = txPassword.Password;

                DialogResult = true;
            }

            Close();
        }

        private string GetFilePath()
        {
            if (string.IsNullOrEmpty(ConfigFile.Data.UserFilePath))
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PasswordStore.data");
            }
            else if (ConfigFile.Data.UserFilePath.StartsWith(@".\"))
            {
                var fileName = ConfigFile.Data.UserFilePath.Substring(2);
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            }
            else
            {
                return ConfigFile.Data.UserFilePath;
            }
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
