using System.Diagnostics;

namespace Build.Scripts.Helpers
{
    public class ProccessHelper
    {
        private string _fileName;
        private string? _workingDirectory;
        private string _arguments;
        private Func<string, Task>? _log;

        public ProccessHelper(string fileName, string workingDirectory, string arguments, Func<string, Task>? log = null)
        {
            _fileName = fileName;
            _workingDirectory = workingDirectory;
            _arguments = arguments;
            _log = log;
        }

        public ProccessHelper(string fileName, string arguments, Func<string, Task>? log = null)
        {
            _fileName = fileName;
            _arguments = arguments;
            _log = log;
        }

        public void Run()
        {
            var process = new Process();
            process.OutputDataReceived += process_OutputDataReceived;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = string.IsNullOrWhiteSpace(_workingDirectory) ?
                Path.GetDirectoryName(_fileName) :
                _workingDirectory;
            process.StartInfo.FileName = _fileName;
            process.StartInfo.Arguments = _arguments;

            _ = _log?.Invoke($"Staring process: {_fileName} {_arguments}");
            var begin = DateTime.Now;

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            var elapsed = DateTime.Now - begin;
            _ = _log?.Invoke($"Finish process in {elapsed.ToString("G")}");

        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _ = _log?.Invoke(e.Data);
            }
        }
    }
}
