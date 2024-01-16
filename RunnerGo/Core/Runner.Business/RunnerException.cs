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
            : base(string.Format(msg, format))
        {
        }

        public RunnerException(Exception innerException, string msg, params string[] format)
            : base(string.Format(msg, format), innerException)
        {
        }
    }
}