using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.AssertExtension
{
    public class AssertEnum
    {
        [StackTraceHidden]
        public void In<T>(T @enum, T value, string message, params string[] format) where T : struct
        {
            In(@enum, new[] { value }, message, format);
        }

        [StackTraceHidden]
        public void In<T>(T @enum, T[] values, string message, params string[] format) where T : struct
        {
            if (!values.Contains(@enum))
            {
                throw new RunnerException(message, format);
            }
        }

        [StackTraceHidden]
        public static void MustDefined(Type type, object value, string message, params string[] format)
        {
            Assert.Test(() => Enum.IsDefined(type, value), message, format);
        }
    }
}
