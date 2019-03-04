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
        private bool _isGroupByPassword;

        public SelectionWindow()
        {
            InitializeComponent();

            SetContext();
        }

        private void SetContext()
        {
            var context = new SelectionContext
            {
                AGroups = GetAGroups()
            };

            _context = context;
            DataContext = _context;
        }

        private ObservableCollection<SelectionGroupContext> GetAGroups()
        {
            if (_isGroupByPassword)
            {
                return null;
            }
            else
            {
                return GetAGroupByGroups();
            }
        }

        private ObservableCollection<SelectionGroupContext> GetAGroupByGroups()
        {
            return new ObservableCollection<SelectionGroupContext>(Program.Session.User.Data.Domains
                .GroupBy(d => d.Group)
                .Select(g => new SelectionGroupContext
                {
                    Header = g.Key,
                    Items = new ObservableCollection<SelectionDomainContext>(
                        g
                            .Select(MapperDomainToContext)
                            .Where(d => d != null))
                }));
        }

        private SelectionDomainContext MapperDomainToContext(UserDomainData data)
        {
            var password = Program.Session.User.Data.Passwords
                .FirstOrDefault(p => p.PasswordId == data.PasswordId);
            if (password == null)
            {
                return null;
            }
            else
            {
                return new SelectionDomainContext
                {
                    DomainId = data.DomainId,
                    Alias = data.Alias,
                    Password = password.Value
                };
            }
        }

        private void ActionButtons_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}