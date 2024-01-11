using System.Diagnostics;

namespace Runner.Business.AssertExtension
{
    public class AssertIO
    {
        [StackTraceHidden]
        public void MustDirectoryExist(string path, string message, params string[] format)
        {
            if (!Directory.Exists(path))
            {
                throw new RunnerException(message, format);
            }
        }

        [StackTraceHidden]
        public void MustFileExist(string path, string message, params string[] format)
        {
            if (!File.Exists(path))
            {
                throw new RunnerException(message, format);
            }
        }
    }
}