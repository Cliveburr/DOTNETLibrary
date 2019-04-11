using DependencyResolver.Containers;
using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Factory
{
    public class SingletonFactory : IFactory
    {
        public Dictionary<Type, object> Instances { get; }

        public SingletonFactory()
        {
            Instances = new Dictionary<Type, object>();
        }

        public object Get(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            if (!Instances.ContainsKey(serviceType))
            {
                Instances[serviceType] = resolvedType.Builder.Instantiate(resolvedType.ImplementationType, context, false);
            }
            return Instances[serviceType];
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