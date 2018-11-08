using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Helper
{
    public class SevenZip
    {
        public string Executable { get; set; }
        public string WorkingDirectory { get; set; }
        public string File { get; private set; }
        public string Log { get; private set; }
        public TimeSpan LastOperationTime { get; private set; }

        public SevenZip(string file)
        {
            WorkingDirectory = IO.RootPath();
            Executable = IO.PathCombine(WorkingDirectory, "7za.exe");
            File = file;
            Log = File + ".log";
        }

        public void AddFolder(string folder)
        {
            DateTime begin = DateTime.Now;

            if (!System.IO.File.Exists(Executable))
                throw new Exception(string.Format(@"The 7Zip executable not found! ""{0}""", Executable));

            Process process = new Process();
            process.OutputDataReceived += add_process_OutputDataReceived;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = WorkingDirectory;
            process.StartInfo.FileName = Executable;
            process.StartInfo.Arguments = string.Format(@"a -t7z ""{0}"" ""{1}"" -m0=LZMA2 -mmt=on -r -y",
                File,
                folder);
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                if (!Log.Substring(Log.Length - 150).Contains("Everything is Ok"))
                    throw new Exception("7za not return everythink is OK!");
            }
            catch
            {
                throw;
            }
            finally
            {
                DateTime end = DateTime.Now;
                LastOperationTime = end - begin;
            }
        }

        public void AddFiles(string[] files)
        {
            DateTime begin = DateTime.Now;

            if (!System.IO.File.Exists(Executable))
                throw new Exception(string.Format(@"The 7Zip executable not found! ""{0}""", Executable));

            var list = IO.PathCombine(WorkingDirectory, "list.txt");
            System.IO.File.WriteAllLines(list, files);

            Process process = new Process();
            process.OutputDataReceived += add_process_OutputDataReceived;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = WorkingDirectory;
            process.StartInfo.FileName = Executable;
            process.StartInfo.Arguments = string.Format(@"a -t7z ""{0}"" ""@{1}"" -m0=LZMA2 -mmt=on -r -y",
                File,
                list);
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                if (!Log.Substring(Log.Length - 150).Contains("Everything is Ok"))
                    throw new Exception("7za not return everythink is OK!");
            }
            catch
            {
                throw;
            }
            finally
            {
                IO.DeleteFile(list);
                DateTime end = DateTime.Now;
                LastOperationTime = end - begin;
            }
        }

        private void add_process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log += e.Data;
        }

        public void Extract(string folder)
        {
            DateTime begin = DateTime.Now;

            if (!System.IO.File.Exists(Executable))
                throw new Exception(string.Format(@"The 7Zip executable not found! ""{0}""", Executable));

            Process process = new Process();
            process.OutputDataReceived += ext_process_OutputDataReceived;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = WorkingDirectory;
            process.StartInfo.FileName = Executable;
            process.StartInfo.Arguments = string.Format(@"x -o""{0}"" ""{1}"" -mmt=on -r -y",
                folder,
                File);
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                if (!Log.Substring(Log.Length - 150).Contains("Everything is Ok"))
                    throw new Exception("7za not return everythink is OK!");
            }
            catch
            {
                throw;
            }
            finally
            {
                DateTime end = DateTime.Now;
                LastOperationTime = end - begin;
            }
        }

        private void ext_process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log += e.Data;
        }

        public bool Test()
        {
            DateTime begin = DateTime.Now;

            if (!System.IO.File.Exists(Executable))
                throw new Exception(string.Format(@"The 7Zip executable not found! ""{0}""", Executable));

            Process process = new Process();
            process.OutputDataReceived += test_process_OutputDataReceived;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = WorkingDirectory;
            process.StartInfo.FileName = Executable;
            process.StartInfo.Arguments = string.Format(@"t ""{0}"" -mmt=on -r -y",
                File);
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                if (!Log.Substring(Log.Length - 150).Contains("Everything is Ok"))
                    return false;
            }
            catch
            {
                throw;
            }
            finally
            {
                DateTime end = DateTime.Now;
                LastOperationTime = end - begin;
            }

            return true;
        }

        private void test_process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log += e.Data;
        }
    }
}