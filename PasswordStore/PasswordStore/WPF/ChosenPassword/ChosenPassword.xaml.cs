using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PasswordStore.WPF.ChosenPassword
{
    /// <summary>
    /// Interaction logic for ChosenPassword.xaml
    /// </summary>
    public partial class ChosenPassword : Window
    {
        public ChosenPassword()
        {
            InitializeComponent();

            this.Topmost = true;

            //var context = new ChosenPasswordContext
            //{
            //    Items = (from p in Program.Passwords.Data.Passwords
            //             select new ChosenPasswordItem(p, Item_Click)).ToList()
            //};

            //this.DataContext = context;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;

            var strechHeight = this.Height + this.svWorkArea.ExtentHeight - this.svWorkArea.ViewportHeight;

            if (strechHeight < desktopWorkingArea.Height)
            {
                this.Height = strechHeight;
                this.Top = desktopWorkingArea.Height - this.Height;
            }
            else
            {
                this.Top = 0;
                this.Height = desktopWorkingArea.Height;
            }

            this.Left = desktopWorkingArea.Width - this.Width - 6;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}