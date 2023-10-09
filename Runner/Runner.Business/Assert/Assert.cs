using Runner.Business.AssertExtension;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Runner
{
    public static class Assert
    {
        private static AssertStrings _strings;

        public static AssertStrings Strings
        {
            get
            {
                if (_strings == null)
                    _strings = new AssertStrings();
                return _strings;
            }
        }

        private static AssertIO _io;

        public static AssertIO IO
        {
            get
            {
                if (_io == null)
                    _io = new AssertIO();
                return _io;
            }
        }

        private static AssertList _list;

        public static AssertList List
        {
            get
            {
                if (_list == null)
                    _list = new AssertList();
                return _list;
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

        [StackTraceHidden]
        public static void EnumMustDefined(Type type, object value, string message, params string[] format)
        {
            Test(() => Enum.IsDefined(type, value), message, format);
        }
    }
}