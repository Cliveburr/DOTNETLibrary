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
        public static MainWindow MainWindow { get; set; }

        [STAThread]
        static void Main()
        {
            App = new Application();
            App.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            if (CryptBusiness.OpenFile())
            {
                MainWindow = new MainWindow();
                MainWindow.Show();

                App.Exit += App_Exit;
                App.Run();
            }
        }

        private static void App_Exit(object sender, ExitEventArgs e)
        {
            File.Save(PasswordUsed);
        }

        private static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is StoreException)
            {
                var storeException = e.Exception as StoreException;

                Warning(storeException.Message);

                e.Handled = !storeException.Terminate;
            }
            else
            {
                ErrorHandle(e.Exception);
                e.Handled = false;
            }
        }

        public static void Close()
        {
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