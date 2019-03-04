using PasswordStore.Config;
using PasswordStore.Helpers;
using PasswordStore.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordStore.HotKeys
{
    public class HotKeysController
    {
        private List<KeyItem> _items;
        private int _itemsIndex;
        private KeyControl _control;

        public HotKeysController()
        {
            _items = new List<KeyItem>();
            _control = new KeyControl();
            _control.OnHotKey += _control_OnHotKey;
        }

        public void Set()
        {
            Clear();

            RegisterAll();
        }

        private void Clear()
        {
            foreach (var item in _items)
            {
                _control.UnregisterHotKey(item.ID);
            }
            _itemsIndex = 1;
            _items.Clear();
        }

        private void RegisterAll()
        {
            foreach (var data in ConfigFile.Data.HotKeys)
            {
                var item = new KeyItem
                {
                    ID = _itemsIndex++,
                    Data = data
                };
                _items.Add(item);

                _control.RegisterHotKey(data.HotKey, item.ID);
            }
        }

        private void _control_OnHotKey(int ID)
        {
            ObjectFocus.Instance.Get();

            var hotKey = _items
                .FirstOrDefault(i => i.ID == ID);

            if (hotKey == null)
            {
                return;
            }

            switch (hotKey.Data.Type)
            {
                case ConfigHotKeyType.ChosenPasswords: DoChosenPasswords(); break;
            }
        }

        private void DoChosenPasswords()
        {
            Program.Session.CheckOpen(() =>
            {
                WPF.WindowBase.Show<WPF.Selection.SelectionWindow>();
            });
        }
    }
}