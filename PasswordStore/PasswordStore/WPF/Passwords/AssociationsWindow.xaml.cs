using PasswordStore.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PasswordStore.WPF.Passwords
{
    public partial class AssociationsWindow : Page
    {
        //public delegate void AssociationsCloseHandle(bool isOk, List<AssociationsData> value);
        //public event AssociationsCloseHandle AssociationsClose;
        //public ObservableCollection<AssociationsData> Associations { get; set; }
        //public ObservableCollection<AssociationsData> Selecteds { get; set; }

        //public AssociationsWindow(List<AssociationsData> associations, List<AssociationsData> selecteds)
        //{
        //    InitializeComponent();

        //    this.Associations = new ObservableCollection<AssociationsData>(from a in associations
        //                                                                    select a);
        //    this.lvAssociations.ItemsSource = this.Associations;

        //    this.Selecteds = new ObservableCollection<AssociationsData>(from s in selecteds
        //                                                                 select s);
        //    this.lvSelecteds.ItemsSource = this.Selecteds;
        //}

        private void btSelect_Click(object sender, RoutedEventArgs e)
        {
            //var associations = this.lvAssociations.SelectedItems.Cast<AssociationsData>().ToList();

            //if (associations.Count() == 0)
            //    return;

            //foreach (var t in associations)
            //{
            //    this.Associations.Remove(t);
            //    this.Selecteds.Add(t);
            //}
        }

        private void btSelectAll_Click(object sender, RoutedEventArgs e)
        {
            //var associations = this.lvAssociations.Items.Cast<AssociationsData>().ToList();

            //if (associations.Count() == 0)
            //    return;

            //foreach (var t in associations)
            //{
            //    this.Associations.Remove(t);
            //    this.Selecteds.Add(t);
            //}
        }

        private void btDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            //var selecteds = this.lvSelecteds.Items.Cast<AssociationsData>().ToList();

            //if (selecteds.Count() == 0)
            //    return;

            //foreach (var t in selecteds)
            //{
            //    this.Selecteds.Remove(t);
            //    this.Associations.Add(t);
            //}
        }

        private void btDeselect_Click(object sender, RoutedEventArgs e)
        {
            //var selecteds = this.lvSelecteds.SelectedItems.Cast<AssociationsData>().ToList();

            //if (selecteds.Count() == 0)
            //    return;

            //foreach (var t in selecteds)
            //{
            //    this.Selecteds.Remove(t);
            //    this.Associations.Add(t);
            //}
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            //this.AssociationsClose(false, null);
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            //this.AssociationsClose(true, this.Selecteds.ToList());
        }
    }
}