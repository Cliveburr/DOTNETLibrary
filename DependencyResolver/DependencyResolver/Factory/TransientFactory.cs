using DependencyResolver.Containers;
using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Factory
{
    public class TransientFactory : IFactory
    {
        public object Get(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            return resolvedType.Builder.Instantiate(serviceType, resolvedType.ImplementationType, context, true);
        }

        public IEnumerable<object> GetAll(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            return new List<object> { resolvedType.Builder.Instantiate(serviceType, resolvedType.ImplementationType, context, true) };
        }
    }
}