using Runner.Business.AssertExtension;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Runner
{
    public static class Assert
    {
        private static AssertStrings? _strings;

        public static AssertStrings Strings
        {
            get
            {
                if (_strings == null)
                    _strings = new AssertStrings();
                return _strings;
            }
        }

        private static AssertIO? _io;

        public static AssertIO IO
        {
            get
            {
                if (_io == null)
                    _io = new AssertIO();
                return _io;
            }
        }

        private static AssertEnumerable? _enumerable;

        public static AssertEnumerable Enumerable
        {
            get
            {
                if (_enumerable == null)
                    _enumerable = new AssertEnumerable();
                return _enumerable;
            }
        }

        private static AssertEnum? _enum;

        public static AssertEnum Enum
        {
            get
            {
                if (_enum == null)
                    _enum = new AssertEnum();
                return _enum;
            }
        }

        private static AssertNumber? _number;

        public static AssertNumber Number
        {
            get
            {
                if (_number == null)
                    _number = new AssertNumber();
                return _number;
            }
        }

        [StackTraceHidden]
        public static void Test(Func<bool> test, string message, params string[] format)
        {
            if (!test())
            {
                throw new RunnerException(message, format);
            }
        }

        [StackTraceHidden]
        public static void MustTrue(bool test, string message, params string[] format)
        {
            Test(() => test, message, format);
        }

        [StackTraceHidden]
        public static void MustFalse(bool test, string message, params string[] format)
        {
            Test(() => !test, message, format);
        }

        [StackTraceHidden]
        public static void MustNull<T>(T? obj, string message, params string[] format)
        {
            if (obj is not null)
            {
                throw new RunnerException(message, format);
            }
        }

        [StackTraceHidden]
        public static void MustNotNull<T>([NotNull] T? obj, string message, params string[] format)
        {
            if (obj is null)
            {
                throw new RunnerException(message, format);
            }
        }
    }
}