using PasswordStore.Config;
using System;
using System.Windows;

namespace PasswordStore.WPF.About
{
    public partial class AboutWindow : Window
    {
        private AboutContext _context;

        public AboutWindow()
        {
            InitializeComponent();

            SetContext();
        }

        private void SetContext()
        {
            var context = new AboutContext
            {
                DontShowAboutAnymore = ConfigFile.Data.DontShowAboutAnymore
            };

            _context = context;
            DataContext = _context;
        }

        private void SaveContext()
        {
            ConfigFile.Data.DontShowAboutAnymore = _context.DontShowAboutAnymore;

            ConfigFile.Save();
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void chDontShowAnymore_CheckedUnchecked(object sender, RoutedEventArgs e)
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