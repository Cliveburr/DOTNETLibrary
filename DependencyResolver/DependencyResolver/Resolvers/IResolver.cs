using DependencyResolver.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Resolvers
{
    public interface IResolver
    {
        IEnumerable<ResolvedType> TryResolve(Type serviceType, Container container);
    }
}