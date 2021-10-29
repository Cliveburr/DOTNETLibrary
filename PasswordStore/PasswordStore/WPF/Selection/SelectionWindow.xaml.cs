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

namespace PasswordStore.WPF.Selection
{
    public partial class SelectionWindow : WindowBase
    {
        public override ConfigWindowIDEnum ID => ConfigWindowIDEnum.Selection;

        private SelectionContext _context;

        public SelectionWindow()
        {
            InitializeComponent();

            SetContext();
        }

        private void SetContext()
        {
            var context = new SelectionContext
            {
                Groups = GetAGroupByGroups()
            };

            _context = context;
            DataContext = _context;
        }

        private ObservableCollection<SelectionGroupContext> GetAGroupByGroups()
        {
            var list = Program.Session.User.Data.GroupOrder
                .Select(g =>
                {
                    var items = Program.Session.User.Data.Domains
                        .Where(d => d.Group == g)
                        .Select(MapperDomainToContext);

                    if (items.Any())
                    {
                        return new SelectionGroupContext
                        {
                            Header = g,
                            Items = new ObservableCollection<SelectionDomainContext>(items)
                        };
                    }
                    else
                    {
                        return null;
                    }
                })
                .Where(g => g != null)
                .ToList();

            var listedGroups = list
                .Select(g => g.Header)
                .ToArray();

            list.AddRange(Program.Session.User.Data.Domains
                .GroupBy(d => d.Group)
                .Where(g => !listedGroups.Contains(g.Key))
                .Select(g => new SelectionGroupContext
                {
                    Header = g.Key,
                    Items = new ObservableCollection<SelectionDomainContext>(g.Select(MapperDomainToContext))
                }));

            return new ObservableCollection<SelectionGroupContext>(list);
        }

        private SelectionDomainContext MapperDomainToContext(UserDomainData data)
        {
            return new SelectionDomainContext
            {
                DomainId = data.DomainId,
                Alias = data.Alias,
                Password = data.ActualPassword,
                URL = data.URL,
                Login = data.Login,
                SubHotkey = data.SubHotkey
            };
        }

        private void ActionButtons_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WindowBase_Closed(object sender, EventArgs e)
        {
            Program.Session.FreeWindow();
        }

        private void WindowBase_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key.ToString();
            var has = _context.Groups
                .SelectMany(g => g.Items)
                .FirstOrDefault(i => i.SubHotkey == key);
            if (has != null)
            {
                has.Clipboard_Click_Do();
                Close();
            }
        }

        private void btGroupOrderRaise_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)e.Source;
            var group = (SelectionGroupContext)button.DataContext;

            var order = _context.Groups
                .Select(g => g.Header)
                .ToList();

            var index = order.IndexOf(group.Header);
            if (index > 0)
            {
                order.RemoveAt(index);
                index--;
                order.Insert(index, group.Header);

                Program.Session.User.Data.GroupOrder = order;
                Program.Session.Save();

                _context.Groups = GetAGroupByGroups();
                _context.RaiseNotify("Groups");
            }
        }

        private void btGroupOrderDown_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)e.Source;
            var group = (SelectionGroupContext)button.DataContext;

            var order = _context.Groups
                .Select(g => g.Header)
                .ToList();

            var index = order.IndexOf(group.Header);
            if (index < _context.Groups.Count - 1)
            {
                order.RemoveAt(index);
                index++;
                order.Insert(index, group.Header);

                Program.Session.User.Data.GroupOrder = order;
                Program.Session.Save();

                _context.Groups = GetAGroupByGroups();
                _context.RaiseNotify("Groups");
            }
        }
    }
}