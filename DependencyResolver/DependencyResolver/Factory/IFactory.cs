using DependencyResolver.Containers;
using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Factory
{
    public interface IFactory
    {
        object Get(Type serviceType, ResolvedType resolvedType, ResolveContext context);
        IEnumerable<object> GetAll(Type serviceType, ResolvedType resolvedType, ResolveContext context);
    }
}