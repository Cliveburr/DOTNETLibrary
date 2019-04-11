using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyResolver.WebTests.Subjects
{
    public interface INameSubject
    {
        string Name { get; set; }
        int Id { get; set; }
    }
}