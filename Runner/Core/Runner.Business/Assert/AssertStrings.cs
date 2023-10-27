using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.AssertExtension
{
    public class AssertStrings
    {
        [StackTraceHidden]
        public void MustNotNullOrEmpty([NotNull] string? text, string message, params string[] format)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new RunnerException(message, format);
            }
        }

        [StackTraceHidden]
        public void MustEqual([NotNull] string? text, string equal, string message, params string[] format)
        {
            if (text != equal)
            {
                throw new RunnerException(message, format);
            }
        }

        [StackTraceHidden]
        public void NotNullAndMax([NotNull] string? text, int max, string message, params string[] format)
        {
            if (text == null)
            {
                throw new RunnerException(message, format);
            }
            if (text.Length > max)
            {
                throw new RunnerException(message, format);
            }
        }

        [StackTraceHidden]
        public void NotNullAndRange([NotNull] string? text, int min, int max, string message, params string[] format)
        {
            if (text == null)
            {
                throw new RunnerException(message, format);
            }
            if (text.Length < min || text.Length > max)
            {
                throw new RunnerException(message, format);
            }
        }
    }
}