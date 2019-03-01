using PasswordStore.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace PasswordStore.WPF.Passwords
{
    /// <summary>
    /// Interaction logic for PasswordsWindow.xaml
    /// </summary>
    public partial class PasswordsWindow : Window
    {
        private SetPasswordWindow _setPassword;
        private HistoryWindow _history;
        private AssociationsWindow _associations;

        public PasswordsWindow()
        {
            InitializeComponent();

            //this.DataContext = new PasswordsContext
            //{
            //    Passwords = new ObservableCollection<PasswordsViewModel>(from p in Program.Passwords.Data.Passwords
            //                                                             select new PasswordsViewModel(p.Clone()))
            //};
        }

        private void Save()
        {
            var context = this.DataContext as PasswordsContext;

            //Program.Passwords.Data.Passwords = context.Passwords.Select(p => p.Data).ToList();
            //Program.Passwords.Save(Program.PasswordUsed);
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Save();
                this.Close();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void btSet_Click(object sender, RoutedEventArgs e)
        {
            var password = this.dgPasswords.SelectedItem as PasswordsViewModel;

            if (password == null)
                return;

            this._setPassword = new SetPasswordWindow();
            this._setPassword.SetPasswordClose += _setPassword_SetPasswordClose;
            
            this.frShow.Navigate(this._setPassword);
            this.frShow.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.frShow.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.frShow.Height = 170;
            this.frShow.Width = 330;

            this.dpMainDock.IsEnabled = false;
            this.frShow.Visibility = System.Windows.Visibility.Visible;
        }

        public void _setPassword_SetPasswordClose(bool isOk, string value)
        {
            this.dpMainDock.IsEnabled = true;
            this.frShow.Visibility = System.Windows.Visibility.Hidden;

            var password = this.dgPasswords.SelectedItem as PasswordsViewModel;

            if (password == null)
                return;

            if (isOk)
            {
                //password.Data.SetValue(value);
            }
        }

        private void btAssociations_Click(object sender, RoutedEventArgs e)
        {
            //var password = this.dgPasswords.SelectedItem as PasswordsViewModel;

            //if (password == null)
            //    return;

            //var context = this.DataContext as PasswordsContext;

            //var usedUints = context.Passwords.ToList()
            //    .SelectMany(p => p.Data.Associations)
            //    .Distinct()
            //    .ToList();

            //var associations = AssociationsData.ReadAssociationsNotIn(usedUints);

            //this._associations = new AssociationsWindow(associations, AssociationsData.ReadAssociations(password.Data.Associations));
            //this._associations.AssociationsClose += _associations_AssociationsClose;

            //this.frShow.Navigate(this._associations);
            //this.frShow.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //this.frShow.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            //this.frShow.Height = 350;
            //this.frShow.Width = 400;

            //this.dpMainDock.IsEnabled = false;
            //this.frShow.Visibility = System.Windows.Visibility.Visible;
        }

        //private void _associations_AssociationsClose(bool isOk, List<AssociationsData> value)
        //{
        //    this.dpMainDock.IsEnabled = true;
        //    this.frShow.Visibility = System.Windows.Visibility.Hidden;

        //    var password = this.dgPasswords.SelectedItem as PasswordsViewModel;

        //    if (password == null)
        //        return;

        //    if (isOk)
        //    {
        //        password.SetAssociations(value.Select(v => v.AssociationsId).ToList());
        //    }
        //}

        private void btHistory_Click(object sender, RoutedEventArgs e)
        {
            //var password = this.dgPasswords.SelectedItem as PasswordsViewModel;

            //if (password == null)
            //    return;

            //this._history = new HistoryWindow(password.Data.History);
            //this._history.SetHistoryClose += _history_SetHistoryClose;

            //this.frShow.Navigate(this._history);
            //this.frShow.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //this.frShow.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            //this.frShow.Width = 430;
            //this.frShow.Height = Double.NaN;

            //this.dpMainDock.IsEnabled = false;
            //this.frShow.Visibility = System.Windows.Visibility.Visible;
        }

        //private void _history_SetHistoryClose(bool isOk, List<PasswordHistoryData> value)
        //{
        //    this.dpMainDock.IsEnabled = true;
        //    this.frShow.Visibility = System.Windows.Visibility.Hidden;

        //    var password = this.dgPasswords.SelectedItem as PasswordsViewModel;

        //    if (password == null)
        //        return;

        //    if (isOk)
        //    {
        //        password.Data.History = value;
        //    }
        //}
    }
}