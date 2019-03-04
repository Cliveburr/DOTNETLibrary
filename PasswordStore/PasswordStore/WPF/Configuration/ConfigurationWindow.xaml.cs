using PasswordStore.Config;
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

            SetContext();
        }

        private void SetContext()
        {
            var context = new ConfigurationContext
            {
                DontShowAboutAnymore = ConfigFile.Data.DontShowAboutAnymore,
                UserFilePath = ConfigFile.Data.UserFilePath
            };

            _context = context;
            DataContext = _context;
        }

        private void SaveContext()
        {
            ConfigFile.Data.DontShowAboutAnymore = _context.DontShowAboutAnymore;
            ConfigFile.Data.UserFilePath = _context.UserFilePath;

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
            _context.UserFilePath = @".\CrypStore.data";
            _context.RaiseNotify("UserFilePath");

            SaveContext();

            Program.Session.Clean();
        }

        private void cbDontshow_Checked(object sender, RoutedEventArgs e)
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
    }
}