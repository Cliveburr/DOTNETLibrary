using PasswordStore.Config;
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
using System.Windows.Shapes;

namespace PasswordStore.WPF.Domain
{
    public partial class DomainSubHotkeyWindow : WindowBase, IDisposable
    {
        public override ConfigWindowIDEnum ID => ConfigWindowIDEnum.DomainSubHotkey;

        public DomainSubHotkeyWindow(string subHotKey)
        {
            InitializeComponent();

            DataContext = new DomainSubhotkeyContext
            {
                SubHotKey = subHotKey
            };
        }

        public string Value
        {
            get
            {
                return (DataContext as DomainSubhotkeyContext).SubHotKey;
            }
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public void Dispose()
        {
        }

        private void SetSubHotkey(string key)
        {
            var context = DataContext as DomainSubhotkeyContext;
            context.SubHotKey = key;
            context.RaiseNotify("SubHotKey");
        }

        private void WindowBase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SetSubHotkey(string.Empty);
            }
            else
            {
                var key = e.Key.ToString();
                var has = Program.Session.User.Data.Domains
                    .Any(d => d.SubHotkey == key);
                if (has)
                {
                    Program.Warning("Sub HotKey already binded!");
                }
                else
                {
                    SetSubHotkey(key);
                }
            }
        }
    }
}