using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    public interface IInterceptSubject
    {
        void LogEvent0(string arg1);
        void LogEvent1(int arg0, string arg1);
        void LogEvent2();
    }
}