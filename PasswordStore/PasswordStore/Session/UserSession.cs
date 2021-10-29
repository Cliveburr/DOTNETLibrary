using PasswordStore.Config;
using PasswordStore.User;
using PasswordStore.WPF.Password;
using System;

namespace PasswordStore.Session
{
    public class UserSession
    {
        public string MainPassword { get; set; }
        public UserFile User { get; private set; }

        private DateTime _sessionLiveDate;
        private uint _windowUsing;

        public void Clean()
        {
            _sessionLiveDate = new DateTime();
            User = null;
            MainPassword = null;
        }

        public void CheckOpen(Action callBack)
        {
            CheckSessionExpire();

            if (MainPassword == null)
            {
                Open(callBack);
            }
            else
            {
                if (User == null)
                {
                    User = UserFile.Open(MainPassword);
                }

                SetNewSessionAccess(callBack);
            }
        }

        private void Open(Action callBack)
        {
            using (var open = new OpenPasswordWindow())
            {
                if (open.ShowDialog() ?? false)
                {
                    MainPassword = open.MainPassword;
                    User = open.User;

                    SetNewSessionAccess(callBack);
                }
            }
        }

        private void CheckSessionExpire()
        {
            if (ConfigFile.Data.SessionType == SessionType.Always)
            {
                if (_windowUsing == 0)
                {
                    Clean();
                }
            }
            else if (ConfigFile.Data.SessionType == SessionType.Timer)
            {
                var expireDate = _sessionLiveDate.AddMinutes(ConfigFile.Data.SessionExpireTime);
                if (DateTime.Now > expireDate)
                {
                    Clean();
                }
            }
        }

        private void SetNewSessionAccess(Action callBack)
        {
            _sessionLiveDate = DateTime.Now;
            _windowUsing++;
            callBack();
        }

        public void FreeWindow()
        {
            _windowUsing--;
            if (_windowUsing == 0)
            {
                User = null;
            }
        }

        public void Save()
        {
            User.Save(MainPassword);
        }
    }
}