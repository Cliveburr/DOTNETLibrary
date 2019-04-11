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
    }
}