using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordTextStore.WPF
{
    public abstract class WindowBase : Window
    {
        public abstract string IdWindow { get; }

        public WindowBase()
        {

        }

        protected override void OnInitialized(EventArgs e)
        {
            var hasWindowConfig = Program.File.Data.Windows.FirstOrDefault(w =>
                w.IdWindow == IdWindow);
            if (hasWindowConfig != null)
            {
                WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                WindowState = hasWindowConfig.WindowState == System.Windows.WindowState.Minimized ? System.Windows.WindowState.Normal : hasWindowConfig.WindowState;
                Top = hasWindowConfig.Top;
                Left = hasWindowConfig.Left;
                Height = hasWindowConfig.Height;
                Width = hasWindowConfig.Width;
            }

            base.OnInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            var windowConfig = Program.File.Data.Windows.FirstOrDefault(w =>
                w.IdWindow == IdWindow);
            if (windowConfig == null)
            {
                windowConfig = new WindowConfigData
                {
                    IdWindow = IdWindow
                };
                Program.File.Data.Windows.Add(windowConfig);
            }

            windowConfig.WindowState = WindowState;
            windowConfig.Top = Top;
            windowConfig.Left = Left;
            windowConfig.Height = Height;
            windowConfig.Width = Width;

            base.OnClosed(e);
        }

        public static T Show<T>() where T : WindowBase
        {
            T showWindow = null;

            foreach (var window in Application.Current.Windows)
            {
                showWindow = window as T;
                if (showWindow != null)
                    break;
            }

            if (showWindow == null)
            {
                showWindow = (T)Activator.CreateInstance(typeof(T));
                showWindow.Show();
            }
            else
            {
                showWindow.Activate();
                if (showWindow.WindowState == WindowState.Minimized)
                    showWindow.WindowState = WindowState.Normal;
            }

            return showWindow;
        }

        public static void CloseAll()
        {
            foreach (var window in Application.Current.Windows)
            {
                var baseWindow = window as WindowBase;
                if (baseWindow != null)
                {
                    baseWindow.Close();
                }
            }
        }
    }
}