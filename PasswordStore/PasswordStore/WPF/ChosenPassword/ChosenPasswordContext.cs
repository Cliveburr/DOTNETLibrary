using PasswordStore.Helpers;
using PasswordStore.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordStore.WPF.ChosenPassword
{
    public class ChosenPasswordContext
    {
        public List<ChosenPasswordItem> Items { get; set; }
    }

    public class ChosenPasswordItem
    {
        private PasswordData _data;
        private EventHandler _click;

        public string Alias { get { return this._data.Alias; } }

        //public string Associations
        //{
        //    get
        //    {
        //        return string.Join(", ", (from a in AssociationsData.ReadAssociations(this._data.Associations)
        //                                  select a.Name));
        //    }
        //}

        public ChosenPasswordItem(PasswordData data, EventHandler click)
        {
            this._data = data;
            this._click = click;
        }

        public SimpleClickCommand Clipboard_Click { get { return new SimpleClickCommand(Clipboard_Click_Do); } }
        public void Clipboard_Click_Do()
        {
            Clipboard.SetText(this._data.Value);
            this._click(this._data, null);
        }

        public SimpleClickCommand Typing_Click { get { return new SimpleClickCommand(Typing_Click_Do); } }
        public void Typing_Click_Do()
        {
            ObjectFocus.Instance.Set();
            KeySender.SendKeys(this._data.Value);
            this._click(this._data, null);
        }
    }
}