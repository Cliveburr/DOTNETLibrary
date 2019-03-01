using PasswordStore.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordStore.WPF
{
    public abstract class WindowBase : Window
    {
        public abstract ConfigWindowIDEnum ID { get; }

        protected override void OnInitialized(EventArgs e)
        {
            var hasWindowConfig = Program.Config.Windows
                ?.FirstOrDefault(w => w.ID == ID);
            if (hasWindowConfig != null)
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                WindowState = hasWindowConfig.WindowState == WindowState.Minimized ? WindowState.Normal : hasWindowConfig.WindowState;
                Top = hasWindowConfig.Top;
                Left = hasWindowConfig.Left;
                Height = hasWindowConfig.Height;
                Width = hasWindowConfig.Width;
            }

            base.OnInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            var windowConfig = Program.Config.Windows
                ?.FirstOrDefault(w => w.ID == ID);
            if (windowConfig == null)
            {
                windowConfig = new ConfigWindowData();
                Program.Config.Windows.Add(windowConfig);
            }
            windowConfig.WindowState = WindowState;
            windowConfig.Top = Top;
            windowConfig.Left = Left;
            windowConfig.Height = Height;
            windowConfig.Width = Width;
            ConfigFile.Save(Program.Config);

            base.OnClosed(e);
        }

        public static T Show<T>() where T : WindowBase
        {
            T showWindow = null;

            if (Application.Current != null)
            {
                foreach (var window in Application.Current.Windows)
                {
                    showWindow = window as T;
                    if (showWindow != null)
                    {
                        break;
                    }
                }
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
                {
                    showWindow.WindowState = WindowState.Normal;
                }
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