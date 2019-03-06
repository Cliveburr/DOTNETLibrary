using PasswordStore.Session;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PasswordStore.Config
{
    [Serializable]
    public class ConfigData
    {
        public bool DontShowAboutAnymore { get; set; }
        public string UserFilePath { get; set; }
        public SessionType SessionType { get; set; }
        public int SessionExpireTime { get; set; }
        public List<ConfigWindowData> Windows { get; set; }
        public List<ConfigHotKeyData> HotKeys { get; set; }

        public void InitializeData()
        {
            Windows = Windows ?? new List<ConfigWindowData>();
            HotKeys = HotKeys ?? new List<ConfigHotKeyData>
            {
                new ConfigHotKeyData
                {
                    HotKey = Keys.Control | Keys.Shift | Keys.H,
                    Type = ConfigHotKeyType.ChosenPasswords
                }
            };
        }
    }
}