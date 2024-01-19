using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Helpers
{
    public static class IO
    {
        public static void ClearDirectory(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            foreach (var file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                { 
                    Thread.Sleep(1);
                    file.Delete();
                }
            }
            foreach (var dir in directoryInfo.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch
                {
                    Thread.Sleep(1);
                    dir.Delete(true);
                }
            }
        }
    }
}
