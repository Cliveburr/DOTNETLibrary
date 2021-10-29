using PasswordStore.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    public partial class DomainEditWindow : WindowBase, IDisposable
    {
        public override ConfigWindowIDEnum ID => ConfigWindowIDEnum.DomainEdit;

        private DomainItemContext _context;
        public static bool IsShow { get; set; }

        public DomainEditWindow(DomainItemContext item)
        {
            InitializeComponent();

            IsShow = false;
            _context = item;
            SetGroup();
            DataContext = _context;
        }

        private void SetGroup()
        {
            _context.GroupList = Program.Session.User.Data.Domains
                .Select(d => d.Group)
                .Distinct()
                .ToList();
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(_context.ActualPassword))
            {
                Program.Warning("Need to select one password!");
                return false;
            }

            return true;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                DialogResult = true;
                Close();
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public void Dispose()
        {
        }

        private void btRemoveHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectds = dgHistory.SelectedItems
                .Cast<DomainItemHistoryContext>();
            if (!selectds.Any())
            {
                return;
            }

           var newCleanList = _context.History
                .Where(h => !selectds.Contains(h));

            _context.History = new ObservableCollection<DomainItemHistoryContext>(newCleanList);
            _context.RaiseNotify("History");
        }

        private void cbShowPasswordText_Change(object sender, RoutedEventArgs e)
        {
            IsShow = IsShow ? false : true;

            var temp = _context.History;
            _context.History = new ObservableCollection<DomainItemHistoryContext>();
            _context.RaiseNotify("History");
            _context.History = temp;
            _context.RaiseNotify("History");
        }

        private void btUniquePassword_Click(object sender, RoutedEventArgs e)
        {
            using (var edit = new DomainEditPasswordWindow())
            {
                if (edit.ShowDialog() ?? false)
                {
                    _context.ActualPassword = edit.Value;
                    _context.History.Add(new DomainItemHistoryContext
                    {
                        Value = _context.ActualPassword,
                        CreatedDateTime = DateTime.Now
                    });
                }
            }
        }

        private void btSubHotkey_Click(object sender, RoutedEventArgs e)
        {
            using (var edit = new DomainSubHotkeyWindow(_context.SubHotkey))
            {
                if (edit.ShowDialog() ?? false)
                {
                    _context.SubHotkey = edit.Value;
                }
            }
        }
    }

    public class PasswordValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as DomainItemHistoryContext;
            if (DomainEditWindow.IsShow)
            {
                return item.Value;
            }
            else
            {
                return new string('*', item.Value?.Length ?? 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}