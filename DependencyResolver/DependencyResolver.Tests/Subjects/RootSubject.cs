using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    public class RootSubject : IRootSubject
    {
        private static int _ids;

        public string Name { get; set; }
        public int Id { get; set; }

        public RootSubject()
        {
            Id = _ids++;
        }

        public void CallMethod0()
        {
            Console.WriteLine("CallMethod0");
        }

        public string CallMethod1()
        {
            Console.WriteLine("CallMethod1");

            return "CallMethod1 done";
        }

        public string CallMethod2(string arg)
        {
            Console.WriteLine("CallMethod2");

            return $"CallMethod2 \"{arg}\" done";
        }
    }
}