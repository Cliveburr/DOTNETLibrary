using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    public interface IGenericSubject<T>
    {
        int Id { get; set; }
    }
}