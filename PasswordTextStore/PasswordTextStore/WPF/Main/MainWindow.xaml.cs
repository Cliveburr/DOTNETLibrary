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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordTextStore.WPF.Main
{
    public partial class MainWindow : WindowBase
    {
        public override string IdWindow { get { return "MainWindow"; } }

        public MainWindow()
        {
            InitializeComponent();

            //rtbText.Document.LineHeight = 1;

            DataContext = Program.File.Data;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Program.Close();
        }
    }
}