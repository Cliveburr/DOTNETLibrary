using Microsoft.Win32;
using RefineryBoard.Auction.ImportExport;
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

namespace RefineryBoard.Auction.Init
{
    /// <summary>
    /// Interaction logic for ChooseEdital.xaml
    /// </summary>
    public partial class ChooseEdital : Page
    {
        public ChooseEdital()
        {
            InitializeComponent();
        }

        private void ImportCaixaPDF_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.DefaultExt = "pdf";
            openDialog.Filter = "Portable Document Format (*.pdf)|*.pdf";
            openDialog.Multiselect = false;
            openDialog.CheckFileExists = true;
            openDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (!openDialog.ShowDialog() ?? false)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var lotes = new CaixaPDFReader().Read(openDialog.FileName);

                var pureGold = (from l in lotes
                                where l.IsPureGold
                                select l)
                                .ToList();

                var bestLotes = (from l in pureGold
                                 where l.ValuePerGran >= 40.0
                                 orderby l.ValuePerGran
                                 select l)
                                 .ToList();

                Mouse.OverrideCursor = null;

                var saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "xlsx";
                saveDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                saveDialog.OverwritePrompt = true;
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (!saveDialog.ShowDialog() ?? false)
                    return;

                Mouse.OverrideCursor = Cursors.Wait;

                ExportToXML.Write(saveDialog.FileName, bestLotes);
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}