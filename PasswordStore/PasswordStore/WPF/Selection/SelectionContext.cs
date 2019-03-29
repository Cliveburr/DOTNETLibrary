using PasswordStore.Helpers;
using PasswordStore.WPF.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordStore.WPF.Selection
{
    public class SelectionContext : ContextBase
    {
        public ObservableCollection<SelectionGroupContext> AGroups { get; set; }
    }

    public class SelectionGroupContext : ContextBase
    {
        public string Header { get; set; }
        public ObservableCollection<SelectionDomainContext> Items { get; set; }
    }

    public class SelectionDomainContext : ContextBase
    {
        public uint DomainId { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }

        public SimpleClickCommand Clipboard_Click { get { return new SimpleClickCommand(Clipboard_Click_Do); } }
        public void Clipboard_Click_Do()
        {
            Clipboard.SetText(Password);
        }

        public SimpleClickCommand Typing_Click { get { return new SimpleClickCommand(Typing_Click_Do); } }
        public void Typing_Click_Do()
        {
            ObjectFocus.Instance.Set();
            KeySender.SendKeys(Password);
        }

        public SimpleClickCommand OpenEdit_Click { get { return new SimpleClickCommand(OpenEdit_Click_Do); } }
        public void OpenEdit_Click_Do()
        {
            var data = Program.Session.User.Data.Domains
                .First(d => d.DomainId == DomainId);

            DomainWindow.ShowDomainEditWindow(data);
        }
    }
}