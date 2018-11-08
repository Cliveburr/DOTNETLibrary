using Nodus.Core.Client;
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

namespace Nodus.Ide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var nodus = new NodusClient("localhost", 60081);
            nodus.Start();
            nodus.Ping();

            //var file = @"D:\Visual Studio Stuffs\Nodus\Testing.xml";
            //var content = System.IO.File.ReadAllText(file, Encoding.UTF8);
            //var app = nodus.Load(content);
            //var result = nodus.Run(app);

            var result2 = nodus.Run2(@"Scripts\Nodus.Test.Script.exe", "Nodus.Test.Script.Program.Run", 1234);
        }
    }
}
