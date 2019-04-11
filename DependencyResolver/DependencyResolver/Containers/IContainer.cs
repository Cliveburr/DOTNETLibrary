using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Containers
{
    public interface IContainer
    {
        T Resolve<T>();
        object Resolve(Type serviceType);
        object Resolve(Type serviceType, ResolveContext context);
        List<ResolvedType> ResolvedTypes(Type serviceType);
    }
}