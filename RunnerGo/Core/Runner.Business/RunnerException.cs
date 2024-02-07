using System;

namespace Runner
{
    public class RunnerException : Exception
    {
        public RunnerException()
            : base()
        {
        }

        public RunnerException(string msg, params string[] format)
            : base(format.Length > 0 ? string.Format(msg, format) : msg)
        {
        }

        public RunnerException(Exception innerException, string msg, params string[] format)
            : base(format.Length > 0 ? string.Format(msg, format) : msg, innerException)
        {
        }
    }
}