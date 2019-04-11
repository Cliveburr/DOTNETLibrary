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
    }
}