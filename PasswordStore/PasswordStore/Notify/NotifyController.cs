using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordStore.Notify
{
    public class NotifyController
    {
        public NotifyIcon Notify { get; private set; }
        public ContextMenu Menu { get; private set; }

        public void Set()
        {
            Notify = new NotifyIcon();
            Notify.Icon = Properties.Resources.CrypStoreKey;

            Menu = new ContextMenu();

            var mnuConfiguration = new MenuItem("&Configuration");
            mnuConfiguration.Click += mnuConfiguration_Click;
            Menu.MenuItems.Add(mnuConfiguration);

            Menu.MenuItems.Add("-");

            var mnuDomains = new MenuItem("&Domains");
            mnuDomains.Click += mnuDomains_Click;
            Menu.MenuItems.Add(mnuDomains);

            var mnuPasswords = new MenuItem("&Passwords");
            mnuPasswords.Click += mnuPasswords_Click;
            Menu.MenuItems.Add(mnuPasswords);

            Menu.MenuItems.Add("-");

            var mnuExit = new MenuItem("E&xit");
            mnuExit.Click += mnuExit_Click;
            Menu.MenuItems.Add(mnuExit);

            Notify.ContextMenu = Menu;
            Notify.Visible = true;
        }

        private void mnuDomains_Click(object sender, EventArgs e)
        {
            Program.Session.CheckOpen(() =>
            {
                WPF.WindowBase.Show<WPF.Domain.DomainWindow>();
            });
        }

        private void mnuConfiguration_Click(object sender, EventArgs e)
        {
            WPF.WindowBase.Show<WPF.Configuration.ConfigurationWindow>();
        }

        private void mnuPasswords_Click(object sender, EventArgs e)
        {
            Program.Session.CheckOpen(() =>
            {
                WPF.WindowBase.Show<WPF.Password.PasswordWindow>();
            });
        }

        public void Close()
        {
            Notify.Visible = false;
            Notify.Dispose();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Program.Close();
        }
    }
}