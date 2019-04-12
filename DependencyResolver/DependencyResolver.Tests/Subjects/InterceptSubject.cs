using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    public class InterceptSubject : IInterceptSubject
    {
        public void LogEvent0(string arg1)
        {
            LogEvent1(3, arg1);
        }

        public void LogEvent1(int arg0, string arg1)
        {
            LogEvent2();
        }

        public void LogEvent2()
        {
        }

        public void LogOutEvent(int value)
        {
        }
    }
}