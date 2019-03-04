using PasswordStore.Config;
using PasswordStore.HotKeys;
using PasswordStore.Notify;
using PasswordStore.Session;
using System;
using System.Windows;

namespace PasswordStore
{
    public static class Program
    {
        public static Application App { get; private set; }
        public static NotifyController Notify { get; set; }
        public static HotKeysController HotKeys { get; set; }
        public static UserSession Session { get; set; }

        [STAThread]
        static void Main()
        {
            Session = new UserSession();

            ConfigFile.Load();

            CheckDontShowAboutAnymore();

            Notify = new NotifyController();
            Notify.Set();

            HotKeys = new HotKeysController();
            HotKeys.Set();

            CheckForUserFilePath();

            App = new Application();
            App.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            App.Run();
        }

        private static void CheckDontShowAboutAnymore()
        {
            if (!ConfigFile.Data.DontShowAboutAnymore)
            {
                var about = new WPF.About.AboutWindow();
                about.ShowDialog();
            }
        }

        private static void CheckForUserFilePath()
        {
            if (string.IsNullOrEmpty(ConfigFile.Data.UserFilePath))
            {
                WPF.WindowBase.Show<WPF.Configuration.ConfigurationWindow>();
            }
        }

        public static void Close()
        {
            Notify.Close();
            App.Shutdown();
        }

        public static void ErrorHandle(Exception err)
        {
            if (err is StoreException)
            {
                MessageBox.Show(err.Message, "Password Store", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(err.ToString(), "Password Store", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void Warning(string text, params string[] format)
        {
            MessageBox.Show(string.Format(text, format), "Password Store", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool Confirm(string text, params string[] format)
        {
            return MessageBox.Show(string.Format(text, format), "Password Store", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        public static MessageBoxResult Question(string text, params string[] format)
        {
            return MessageBox.Show(string.Format(text, format), "Password Store", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        }
    }
}