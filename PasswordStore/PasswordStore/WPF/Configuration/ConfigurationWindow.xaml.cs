using PasswordStore.Config;
using PasswordStore.Helpers;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace PasswordStore.WPF.Configuration
{
    public partial class ConfigurationWindow : WindowBase
    {
        public override ConfigWindowIDEnum ID => ConfigWindowIDEnum.Configuration;

        private ConfigurationContext _context;

        public ConfigurationWindow()
        {
            InitializeComponent();

            chStartup.IsChecked = StartupLink.IsStartupEnabled;

            SetContext();
        }

        private void SetContext()
        {
            var context = new ConfigurationContext
            {
                DontShowAboutAnymore = ConfigFile.Data.DontShowAboutAnymore,
                UserFilePath = ConfigFile.Data.UserFilePath,
                SessionType = ConfigFile.Data.SessionType,
                SessionExpireTime = ConfigFile.Data.SessionExpireTime
            };

            _context = context;
            DataContext = _context;

            tbSessionTime.IsEnabled = _context.SessionType == Session.SessionType.Timer;
        }

        private void SaveContext()
        {
            ConfigFile.Data.DontShowAboutAnymore = _context.DontShowAboutAnymore;
            ConfigFile.Data.UserFilePath = _context.UserFilePath;
            ConfigFile.Data.SessionType = _context.SessionType;
            ConfigFile.Data.SessionExpireTime = _context.SessionExpireTime;

            ConfigFile.Save();
        }

        private void btOpenPasswordFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenPasswordFileAction();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void OpenPasswordFileAction()
        { 
            using (var open = new OpenFileDialog())
            {
                open.CheckFileExists = false;
                open.AddExtension = true;
                open.Multiselect = false;
                open.Filter = "Passwords files (*.data)|*.data";
                open.DefaultExt = "data";

                if (string.IsNullOrEmpty(_context.UserFilePath))
                {
                    open.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    open.InitialDirectory = Path.GetDirectoryName(_context.UserFilePath);
                    open.FileName = Path.GetFileName(_context.UserFilePath);
                }

                if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _context.UserFilePath = open.FileName;
                    _context.RaiseNotify("UserFilePath");

                    SaveContext();

                    Program.Session.Clean();
                }
            }
        }

        private void btDefaultPasswordFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DefaultPasswordAction();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void DefaultPasswordAction()
        {
            _context.UserFilePath = @".\PasswordStore.data";
            _context.RaiseNotify("UserFilePath");

            SaveContext();

            Program.Session.Clean();
        }

        private void rbSessionType_Click(object sender, RoutedEventArgs e)
        {
            tbSessionTime.IsEnabled = false;
        }

        private void rbSessionTypeTimer_Click(object sender, RoutedEventArgs e)
        {
            tbSessionTime.IsEnabled = true;
        }

        private void WindowBase_Closed(object sender, EventArgs e)
        {
            try
            {
                SaveContext();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            StartupLink.EnableStartup();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            StartupLink.DisableStartup();
        }
    }
}