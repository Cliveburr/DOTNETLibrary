using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;

namespace PasswordStore.Helpers
{
    public static class StartupLink
    {
        public static string File
        {
            get
            {
                var file = Path.GetFileName(typeof(StartupLink).Assembly.Location);
                var linkFile = file + ".lnk";
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), linkFile);
            }
        }

        public static bool IsStartupEnabled
        {
            get
            {
                return System.IO.File.Exists(File);
            }
        }

        public static void EnableStartup()
        {
            var wsh = new IWshShell_Class();
            var shortcut = wsh.CreateShortcut(File) as IWshShortcut;
            shortcut.TargetPath = typeof(StartupLink).Assembly.Location;
            shortcut.Save();
        }

        public static void DisableStartup()
        {
            if (System.IO.File.Exists(File))
            {
                System.IO.File.Delete(File);
            }
        }
    }
}