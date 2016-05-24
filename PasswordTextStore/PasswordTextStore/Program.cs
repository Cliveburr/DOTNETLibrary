using PasswordTextStore.Crypt;
using PasswordTextStore.WPF.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordTextStore
{
    public static class Program
    {
        public static Application App { get; private set; }
        public static string PasswordUsed { get; set; }
        public static CryptFile File { get; set; }

        [STAThread]
        static void Main()
        {
            if (!Crypt.CryptBusiness.OpenFile())
                return;

            var main = new MainWindow();
            main.Show();

            App = new Application();
            App.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            App.Run();
        }

        public static void Close()
        {
            File.Save(PasswordUsed);
            App.Shutdown();
        }

        public static void ErrorHandle(Exception err)
        {
            if (err is StoreException)
                MessageBox.Show(err.Message, "Password Text Store", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(err.ToString(), "Password Text Store", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void Warning(string text, params string[] format)
        {
            MessageBox.Show(string.Format(text, format), "Password Text Store", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool Confirm(string text, params string[] format)
        {
            return MessageBox.Show(string.Format(text, format), "Password Text Store", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        public static MessageBoxResult Question(string text, params string[] format)
        {
            return MessageBox.Show(string.Format(text, format), "Password Text Store", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        }

        public static void ShowWindow(Type window)
        {
            var win = Activator.CreateInstance(window) as Window;
            win.Show();
        }
    }
}