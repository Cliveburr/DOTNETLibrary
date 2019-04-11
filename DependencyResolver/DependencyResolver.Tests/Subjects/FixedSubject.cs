using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    public class FixedSubject
    {
        private static int _ids;

        public string Name { get; set; }
        public int Id { get; set; }
        public ILevelSubject Level { get; set; }

        public FixedSubject(ILevelSubject level)
        {
            Id = _ids++;
            Level = level;
        }
    }
}