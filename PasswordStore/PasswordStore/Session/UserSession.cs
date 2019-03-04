using PasswordStore.User;
using PasswordStore.WPF.Password;
using System;

namespace PasswordStore.Session
{
    public class UserSession
    {
        public string MainPassword { get; set; }
        public UserFile User { get; private set; }

        private DateTime _sessionOpened;
        private uint _windowUsing;

        public void Clean()
        {
            _sessionOpened = new DateTime();
            User = null;
            MainPassword = null;
        }

        public void CheckOpen(Action callBack)
        {
            if (User == null)
            {
                Open(callBack);
            }
            else
            {
                _windowUsing++;
                callBack();
            }
        }

        private void Open(Action callBack)
        {
            using (var open = new OpenPasswordWindow())
            {
                if (open.ShowDialog() ?? false)
                {
                    _sessionOpened = DateTime.Now;

                    MainPassword = open.MainPassword;
                    User = open.User;

                    _windowUsing++;
                    callBack();
                }
            }
        }

        public void FreeWindow()
        {
            _windowUsing--;
        }

        public void Save()
        {
            User.Save(MainPassword);
        }
    }
}