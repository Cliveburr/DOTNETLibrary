using Nodus.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dir = System.IO.Directory;
using File = System.IO.File;

namespace Nodus.Core.Helper
{
    public class IO
    {
        public static string RootPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static string PathCombine(params string[] paths)
        {
            var tr = "";

            if (paths.Length > 0)
            {
                tr = paths[0];
                tr = tr.TrimEnd('\\');
            }

            if (paths.Length > 1)
            {
                for (var i = 1; i < paths.Length; i++)
                {
                    tr = tr + "\\" + paths[i].Trim('\\');
                }
            }

            return tr;
        }

        public static bool CreateDirectory(string path)
        {
            if (Dir.Exists(path))
                return true;
            else
                return Dir.CreateDirectory(path)?.Exists ?? false;
        }

        private static bool RemoveDirectories(string path, bool keepRoot, string searhPattern, bool recursive)
        {
            var files = Dir.GetFiles(path, searhPattern, recursive ? SearchOption.AllDirectories: SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    Thread.Sleep(0);
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            var dirs = Dir.GetDirectories(path, searhPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Reverse()
                .ToList();

            if (!keepRoot)
                dirs.Add(path);

            foreach (var dir in dirs)
            {
                try
                {
                    Dir.Delete(dir, false);
                }
                catch
                {
                    Thread.Sleep(0);
                    try
                    {
                        Dir.Delete(dir, false);
                    }
                    catch
                    {
                        Thread.Sleep(0);
                        try
                        {
                            Dir.Delete(dir, false);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool EmptyDirectory(string path, string searhPattern, bool recursive)
        {
            if (!Dir.Exists(path))
            {
                return CreateDirectory(path);
            }
            else
            {
                return RemoveDirectories(path, true, searhPattern, recursive);
            }
        }

        public static bool DeleteDirectory(string path, string searhPattern, bool recursive)
        {
            if (!Dir.Exists(path))
            {
                return true;
            }
            else
            {
                return RemoveDirectories(path, false, searhPattern, recursive);
            }
        }

        public static FileInformation[] ListFileInfo(string path)
        {
            if (File.Exists(path))
            {
                return new FileInformation[] { new FileInformation(path) };
            }
            else
            {
                if (!Dir.Exists(path))
                    return new FileInformation[0];
                else
                    return Dir.GetFiles(path, "*", SearchOption.AllDirectories)
                        .Select(f => new FileInformation(f))
                        .ToArray();
            }
        }

        public static void DeleteFile(string file)
        {
            if (!File.Exists(file))
                return;

            File.SetAttributes(file, FileAttributes.Normal);
            try
            {
                File.Delete(file);
            }
            catch
            {
                Thread.Sleep(0);
                try
                {
                    File.Delete(file);
                }
                catch
                {
                }
            }
        }

        public static void DeleteFiles(string[] files)
        {
            foreach (var file in files)
            {
                DeleteFile(file);
            }
        }

        public static string SpecialVars(string path)
        {
            path = path.Replace("%ROOTPATH%", Helper.IO.RootPath());
            return path;
        }
    }
}