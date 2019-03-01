using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordStore.WPF.Configuration
{
    public class ConfigurationContext : ContextBase
    {
        public bool DontShowAboutAnymore { get; set; }
        public string UserFilePath { get; set; }
    }
}