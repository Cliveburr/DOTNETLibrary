using DependencyResolver.Builder;
using DependencyResolver.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Resolvers
{
    public class ResolvedType
    {
        public Type ImplementationType { get; set; }
        public IFactory Factory { get; set; }
        public IBuilder Builder { get; set; }
    }
}