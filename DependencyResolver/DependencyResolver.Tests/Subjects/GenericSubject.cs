using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Subjects
{
    public class GenericSubject<T> : IGenericSubject<T>
    {
        private static int _ids;

        public int Id { get; set; }

        public GenericSubject()
        {
            Id = _ids++;
        }
    }
}