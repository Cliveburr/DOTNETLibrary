using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyResolver.WebTests.Subjects
{
    public class NameSubject : INameSubject
    {
        private static int _ids;

        public string Name { get; set; }
        public int Id { get; set; }

        public NameSubject()
        {
            Id = _ids++;
        }
    }
}