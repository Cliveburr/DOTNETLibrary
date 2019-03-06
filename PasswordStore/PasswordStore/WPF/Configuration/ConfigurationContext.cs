using PasswordStore.Session;

namespace PasswordStore.WPF.Configuration
{
    public class ConfigurationContext : ContextBase
    {
        public bool DontShowAboutAnymore { get; set; }
        public string UserFilePath { get; set; }
        public SessionType SessionType { get; set; }
        public int SessionExpireTime { get; set; }
    }
}