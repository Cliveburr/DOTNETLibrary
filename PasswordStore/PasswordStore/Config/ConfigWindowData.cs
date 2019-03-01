using System;

namespace PasswordStore.Config
{
    [Serializable()]
    public class ConfigWindowData
    {
        public ConfigWindowIDEnum ID { get; set; }
        public System.Windows.WindowState WindowState { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
    }
}