using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
