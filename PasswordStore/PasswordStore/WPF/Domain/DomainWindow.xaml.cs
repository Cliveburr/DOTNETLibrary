using PasswordStore.Config;
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
using System.Windows.Shapes;

namespace PasswordStore.WPF.Domain
{
    public partial class DomainWindow : WindowBase
    {
        public override ConfigWindowIDEnum ID => ConfigWindowIDEnum.Domain;

        private DomainContext _context;

        public DomainWindow()
        {
            InitializeComponent();

            SetContext();
        }

        private void SetContext()
        {
            var context = new DomainContext
            {
                Domains = new ObservableCollection<DomainItemContext>(Program.Session.User.Data.Domains
                    .Select(DomainMapper.FromData))
            };

            _context = context;
            DataContext = _context;
        }

        public static void AddNewHistory(UserDomainData data, uint passwordId)
        {
            var password = Program.Session.User.Data.Passwords
                .First(p => p.PasswordId == passwordId);

            data.History.Add(new UserDomainHistoryData
            {
                Value = password.Value,
                CreatedDateTime = DateTime.Now
            });
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
            var newItem = new DomainItemContext();
            using (var edit = new DomainEditWindow(newItem))
            {
                if (edit.ShowDialog() ?? false)
                {
                    newItem.DomainId = ++Program.Session.User.Data.DomainsIndex;
                    var domain = DomainMapper.FromContext(newItem);
                    AddNewHistory(domain, newItem.PasswordId);
                    Program.Session.User.Data.Domains.Add(domain);
                    Program.Session.Save();
                    _context.Domains.Add(newItem);
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
            var selected = dgDomains.SelectedItem as DomainItemContext;
            if (selected == null)
            {
                return;
            }

            var data = Program.Session.User.Data.Domains
                .First(d => d.DomainId == selected.DomainId);

            var context = DomainMapper.FromData(data);

            using (var edit = new DomainEditWindow(context))
            {
                if (edit.ShowDialog() ?? false)
                {
                    data.Alias = context.Alias;
                    data.Group = context.Group;
                    data.Info = context.Info;
                    data.History = context.History
                        .Select(h => DomainMapper.FromHistoryContext(h))
                        .ToList();
                    if (data.PasswordId != context.PasswordId)
                    {
                        data.PasswordId = context.PasswordId;
                        AddNewHistory(data, context.PasswordId);
                    }
                    
                    Program.Session.Save();
                    var index = _context.Domains.IndexOf(selected);
                    _context.Domains[index] = context;
                }
            }
        }

        public static void ShowDomainEditWindow(UserDomainData data)
        {
            var context = DomainMapper.FromData(data);

            using (var edit = new DomainEditWindow(context))
            {
                if (edit.ShowDialog() ?? false)
                {
                    data.Alias = context.Alias;
                    data.Group = context.Group;
                    data.Info = context.Info;
                    data.History = context.History
                        .Select(h => DomainMapper.FromHistoryContext(h))
                        .ToList();
                    if (data.PasswordId != context.PasswordId)
                    {
                        data.PasswordId = context.PasswordId;
                        AddNewHistory(data, context.PasswordId);
                    }

                    Program.Session.Save();
                }
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
            var selected = dgDomains.SelectedItem as DomainItemContext;
            if (selected != null)
            {
                _context.Domains.Remove(selected);

                var domain = Program.Session.User.Data.Domains
                    .First(d => d.DomainId == selected.DomainId);
                Program.Session.User.Data.Domains.Remove(domain);
                Program.Session.Save();
            }
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