using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    public interface IRootSubject
    {
        string Name { get; set; }
        int Id { get; set; }
        void CallMethod0();
        string CallMethod1();
        string CallMethod2(string arg);
    }
}