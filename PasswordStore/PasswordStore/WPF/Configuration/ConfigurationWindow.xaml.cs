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
                DontShowAboutAnymore = Program.Config.DontShowAboutAnymore,
                UserFilePath = Program.Config.UserFilePath
            };

            _context = context;
            DataContext = _context;
        }

        private void SaveContext()
        {
            var data = new ConfigData
            {
                DontShowAboutAnymore = _context.DontShowAboutAnymore,
                UserFilePath = _context.UserFilePath
            };

            ConfigFile.Save(data);

            Program.Config = data;
        }

        private void btOpenPasswordFile_Click(object sender, RoutedEventArgs e)
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
            _context.UserFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrypStore.data");
            _context.RaiseNotify("UserFilePath");

            SaveContext();

            Program.Session.Clean();
        }

        private void cbDontshow_Checked(object sender, RoutedEventArgs e)
        {
            SaveContext();
        }
    }
}