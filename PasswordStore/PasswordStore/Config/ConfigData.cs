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
        public List<ConfigWindowData> Windows { get; set; } = new List<ConfigWindowData>();
        public List<ConfigHotKeyData> HotKeys { get; set; }

        public ConfigData()
        {
            DefaultHotKeys();
        }

        public void DefaultHotKeys()
        {
            HotKeys = new List<ConfigHotKeyData>
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