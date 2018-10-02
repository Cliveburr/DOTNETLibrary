using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RefineryBoard
{
    public static class Program
    {
        public static Application App { get; private set; }
        public static Main.MainWindow MainWindow { get; private set; }
        public static Data.AppData Data { get; private set; }

        [STAThread]
        public static void Main()
        {
            Data = RefineryBoard.Data.AppData.Open();

            MainWindow = new Main.MainWindow();
            MainWindow.Show();

            App = new Application();
            App.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            App.Run();
        }

        public static void Close()
        {
            App.Shutdown();
        }

        public static void ErrorHandle(Exception err)
        {
            if (err is RefineryException)
                MessageBox.Show(err.Message, "Refinery Board", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(err.ToString(), "Refinery Board", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
