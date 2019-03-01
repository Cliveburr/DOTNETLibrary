using PasswordStore.Config;
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
                DontShowAboutAnymore = Program.Config.DontShowAboutAnymore
            };

            _context = context;
            DataContext = _context;
        }

        private void SaveContext()
        {
            Program.Config.DontShowAboutAnymore = _context.DontShowAboutAnymore;

            ConfigFile.Save(Program.Config);
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void chDontShowAnymore_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            SaveContext();
        }
    }
}