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

            DataContext = Program.File.Data;

            if (Program.File.Data.Texts.Any())
            {
                tabTexts.SelectedIndex = 0;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Program.Close();
        }

        private void RefreshTexts()
        {
            DataContext = null;
            DataContext = Program.File.Data;
        }

        private void barNew_Click(object sender, RoutedEventArgs e)
        {
            var newText = new Crypt.CryptText { Name = "New", Text = "" };

            Program.File.Data.Texts.Add(newText);

            RefreshTexts();
        }

        private void barDelete_Click(object sender, RoutedEventArgs e)
        {
            var sel = tabTexts.SelectedIndex;

            Program.File.Data.Texts.RemoveAt(sel);

            RefreshTexts();
        }

        private void barRename_Click(object sender, RoutedEventArgs e)
        {
            var sel = tabTexts.SelectedIndex;
            var item = Program.File.Data.Texts[sel];

            var input = new InputBox("Rename File", "Enter with the name for this file:", item.Name);
            input.ShowDialog();

            if (input.DialogResult ?? false)
            {
                item.Name = input.GetText;

                RefreshTexts();
            }
        }

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var tabItem = e.Source as TabItem;
            if (tabItem == null)
                return;

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }


        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            var tabItemTarget = e.Source as TabItem;
            var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

            if (!tabItemTarget.Equals(tabItemSource))
            {
                var target = tabItemTarget.DataContext as Crypt.CryptText;
                var source = tabItemSource.DataContext as Crypt.CryptText;

                var targetIndex = Program.File.Data.Texts.IndexOf(target);
                var sourceIndex = Program.File.Data.Texts.IndexOf(source);

                Program.File.Data.Texts.RemoveAt(sourceIndex);
                Program.File.Data.Texts.Insert(targetIndex, source);

                RefreshTexts();

                //var tabControl = tabItemTarget.Parent as TabControl;
                //int sourceIndex = tabControl.Items.IndexOf(tabItemSource);
                //int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

                //tabControl.Items.Remove(tabItemSource);
                //tabControl.Items.Insert(targetIndex, tabItemSource);

                //tabControl.Items.Remove(tabItemTarget);
                //tabControl.Items.Insert(sourceIndex, tabItemTarget);
            }
        }
    }
}