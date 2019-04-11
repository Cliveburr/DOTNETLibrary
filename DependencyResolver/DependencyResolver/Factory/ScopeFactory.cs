using DependencyResolver.Containers;
using DependencyResolver.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Factory
{
    public class ScopeFactory : IFactory
    {
        public object Get(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            if (context.Bag.ContainsKey("ScopeInstances"))
            {
                var scopeInstances = context.Bag["ScopeInstances"] as ConcurrentDictionary<Type, object>;

                return scopeInstances.GetOrAdd(serviceType, s =>
                {
                    return resolvedType.Builder.Instantiate(resolvedType.ImplementationType, context, false);
                });
            }
            else
            {
                throw new InvalidOperationException("Invalid resolve scoped service with out scope container!");
            }
        }

        public IEnumerable<object> GetAll(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            return new object[]
            {
                Get(serviceType, resolvedType, context)
            };
        }
    }
}