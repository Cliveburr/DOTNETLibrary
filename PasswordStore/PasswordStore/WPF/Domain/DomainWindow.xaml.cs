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
                Domains = new ObservableCollection<DomainItemContext>(Program.Session.User.Domains
                    .Select(d =>
                    {
                        var item = new DomainItemContext
                        {
                            DomainId = d.DomainId,
                            Alias = d.Alias,
                            Group = d.Group,
                            Info = d.Info,
                            History = d.History
                                .Select(h => new DomainItemHistoryContext
                                {
                                    Value = h.Value,
                                    CreatedDateTime = h.CreatedDateTime
                                })
                                .ToList()
                        };

                        var password = Program.Session.User.Passwords
                            .FirstOrDefault(p => p.PasswordId == d.PasswordId);
                        if (password != null)
                        {
                            item.PasswordId = password.PasswordId;
                            item.PasswordAlias = password.Alias;
                            item.PasswordValue = password.Value;
                        }

                        return item;
                    }))
            };

            _context = context;
            DataContext = _context;
        }

        private void SaveContext()
        {
            //var data = new ConfigData
            //{
            //    DontShowAboutAnymore = _context.DontShowAboutAnymore,
            //    UserFilePath = _context.UserFilePath
            //};

            //ConfigFile.Save(data);

            //Program.Config = data;
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new DomainItemContext();
            using (var edit = new DomainEditWindow(newItem))
            {
                if (edit.ShowDialog() ?? false)
                {
                    newItem.DomainId = Program.Session.User.DomainsIndex++;

                    _context.Domains.Add(newItem);

                    var domain = new UserDomainData
                    {
                        DomainId = newItem.DomainId,
                        Alias = newItem.Alias,
                        Group = newItem.Group,
                        Info = newItem.Info,
                        PasswordId = newItem.PasswordId,
                        History = newItem.History
                            .Select(h => new UserDomainHistoryData
                            {
                                Value = h.Value,
                                CreatedDateTime = h.CreatedDateTime
                            })
                            .ToList()
                    };
                    Program.Session.User.Domains.Add(domain);
                    Program.Session.Save();
                }
            }
        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgDomains.SelectedItem as DomainItemContext;
            if (selected == null)
            {
                return;
            }

            var cloneItem = new DomainItemContext
            {
                DomainId = selected.DomainId,
                Alias = selected.Alias,
                Group = selected.Group,
                Info = selected.Info,
                PasswordId = selected.PasswordId,
                PasswordAlias = selected.PasswordAlias,
                PasswordValue = selected.PasswordValue,
                History = selected.History
                    .Select(h => new DomainItemHistoryContext
                    {
                        Value = h.Value,
                        CreatedDateTime = h.CreatedDateTime
                    })
                    .ToList()
            };
        }

        private void btRemove_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgDomains.SelectedItem as DomainItemContext;
            if (selected != null)
            {
                _context.Domains.Remove(selected);

                var domain = Program.Session.User.Domains
                    .First(d => d.DomainId == selected.DomainId);
                Program.Session.User.Domains.Remove(domain);
                Program.Session.Save();
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WindowBase_Closed(object sender, EventArgs e)
        {
            Program.Session.FreeWindow();
        }
    }
}
