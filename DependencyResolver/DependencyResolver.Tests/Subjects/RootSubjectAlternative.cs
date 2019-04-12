using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    class RootSubjectAlternative : IRootSubject
    {
        private static int _ids;

        public string Name { get; set; }
        public int Id { get; set; }

        public RootSubjectAlternative()
        {
            Id = _ids++;
        }

        public void CallMethod0()
        {
            Console.WriteLine("CallMethod0 Alternative");
        }

        public string CallMethod1()
        {
            Console.WriteLine("CallMethod1 Alternative");

            return "CallMethod1 Alternative done";
        }

        public string CallMethod2(string arg)
        {
            Console.WriteLine("CallMethod2 Alternative");

            return $"CallMethod2 Alternative \"{arg}\" done";
        }
    }
}