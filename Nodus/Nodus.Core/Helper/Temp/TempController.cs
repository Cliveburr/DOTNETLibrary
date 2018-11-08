using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dir = System.IO.Directory;

namespace Nodus.Core.Helper.Temp
{
    public class TempController
    {
        static TempController()
        {
            ClearTempFolders();
        }

        public static TempFolder GetTempFolder()
        {
            var root = IO.RootPath();
            int index = 0;
            while (Dir.Exists(IO.PathCombine(root, "Temp_" + index.ToString())))
                index++;

            string folder = IO.PathCombine(root, "Temp_" + index.ToString());
            IO.CreateDirectory(folder);

            return new TempFolder(folder);
        }

        public static void ClearTempFolders()
        {
            var root = IO.RootPath();
            var folders = Dir.GetDirectories(root)
                .Where(d => System.IO.Path.GetFileName(d).StartsWith("Temp_"))
                .ToList();

            foreach (var folder in folders)
            {
                IO.DeleteDirectory(folder, "*", true);
            }
        }
    }

}