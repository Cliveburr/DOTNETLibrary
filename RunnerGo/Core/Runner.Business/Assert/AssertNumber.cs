using System.Diagnostics;

namespace Runner.Business.AssertExtension
{
    public class AssertNumber
    {
        [StackTraceHidden]
        public void InRange(int value, int min, int max, string message, params string[] format)
        {
            if (value < min || value > max)
            {
                throw new RunnerException(message, format);
            }
        }
    }
}
