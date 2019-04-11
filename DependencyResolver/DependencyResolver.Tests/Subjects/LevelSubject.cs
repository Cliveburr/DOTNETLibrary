using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    public class LevelSubject : ILevelSubject
    {
        private static int _ids;

        public string Name { get; set; }
        public int Id { get; set; }
        public IRootSubject Root { get; set; }

        public LevelSubject(IRootSubject root)
        {
            Id = _ids++;
            Root = root;
        }
    }
}