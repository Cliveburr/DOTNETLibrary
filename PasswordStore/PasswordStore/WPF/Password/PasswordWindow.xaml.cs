using PasswordStore.Config;
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

namespace PasswordStore.WPF.Password
{
    public partial class PasswordWindow : WindowBase
    {
        public override ConfigWindowIDEnum ID => ConfigWindowIDEnum.Password;

        private PasswordContext _context;

        public PasswordWindow()
        {
            InitializeComponent();

            SetContext();
        }

        private void SetContext()
        {
            var context = new PasswordContext
            {
                Passwords = new ObservableCollection<PasswordItemContext>(Program.Session.User.Data.Passwords
                    .Select(PasswordMapper.FromData))
            };

            _context = context;
            DataContext = _context;
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddAction();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void AddAction()
        { 
            var newItem = new PasswordItemContext();
            using (var edit = new PasswordEditWindow(newItem))
            {
                if (edit.ShowDialog() ?? false)
                {
                    newItem.PasswordId = ++Program.Session.User.Data.PasswordIndex;
                    var password = PasswordMapper.FromContext(newItem);
                    Program.Session.User.Data.Passwords.Add(password);
                    Program.Session.Save();
                    _context.Passwords.Add(newItem);
                }
            }
        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditAction();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void EditAction()
        {
            var selected = dgPasswords.SelectedItem as PasswordItemContext;
            if (selected == null)
            {
                return;
            }

            var data = Program.Session.User.Data.Passwords
                .First(d => d.PasswordId == selected.PasswordId);

            var context = PasswordMapper.FromData(data);

            using (var edit = new PasswordEditWindow(context))
            {
                if (edit.ShowDialog() ?? false)
                {
                    data.Alias = context.Alias;
                    CheckForPasswordChanged(data, context.Value);

                    Program.Session.Save();
                    var index = _context.Passwords.IndexOf(selected);
                    _context.Passwords[index] = context;
                }
            }
        }

        private void CheckForPasswordChanged(PasswordData data, string password)
        {
            if (data.Value != password)
            {
                var affectDomains = Program.Session.User.Data.Domains
                    .Where(d => d.PasswordId == data.PasswordId);

                foreach (var domain in affectDomains)
                {
                    domain.History.Add(new UserDomainHistoryData
                    {
                        Value = password,
                        CreatedDateTime = DateTime.Now
                    });
                }

                data.Value = password;
            }
        }

        private void btRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveAction();
            }
            catch (Exception err)
            {
                Program.ErrorHandle(err);
            }
        }

        private void RemoveAction()
        {
            var selected = dgPasswords.SelectedItem as PasswordItemContext;
            if (selected != null)
            {
                if (CanRemovePassword(selected.PasswordId))
                {
                    var password = Program.Session.User.Data.Passwords
                        .First(d => d.PasswordId == selected.PasswordId);
                    Program.Session.User.Data.Passwords.Remove(password);
                    Program.Session.Save();

                    _context.Passwords.Remove(selected);
                }
            }
        }

        private bool CanRemovePassword(uint passwordId)
        {
            var affectDomains = Program.Session.User.Data.Domains
                .Where(d => d.PasswordId == passwordId);
            if (affectDomains.Any())
            {
                Program.Warning("Can't remove password with domains using!");
                return false;
            }

            return true;
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

        private void WindowBase_Closed(object sender, EventArgs e)
        {
            Program.Session.FreeWindow();
        }
    }
}