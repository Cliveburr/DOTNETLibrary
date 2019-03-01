using System;
using System.Windows.Forms;

namespace PasswordStore.Config
{
    [Serializable()]
    public class ConfigHotKeyData
    {
        public Keys HotKey { get; set; }
        public ConfigHotKeyType Type { get; set; }
        public uint DomainId { get; set; }
    }
}