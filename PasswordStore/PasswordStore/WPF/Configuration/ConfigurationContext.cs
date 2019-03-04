namespace PasswordStore.WPF.Configuration
{
    public class ConfigurationContext : ContextBase
    {
        public bool DontShowAboutAnymore { get; set; }
        public string UserFilePath { get; set; }
    }
}