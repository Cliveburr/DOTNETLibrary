using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.AssertExtension
{
    public class AssertList
    {
        [StackTraceHidden]
        public void MustHaveAny<T>(IEnumerable<T>? list, string message, params string[] format)
        {
            if (list == null || !list.Any())
            {
                throw new RunnerException(message, format);
            }
        }

        [StackTraceHidden]
        public void MustContains<T>(IEnumerable<T> list, T value, string message, params string[] format)
        {
            if (!list.Contains(value))
            {
                throw new RunnerException(message, format);
            }
        }
    }
}