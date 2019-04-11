using DependencyResolver.Containers;
using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyResolver.Factory
{
    public class ImplementationFactory : IFactory
    {
        private Dictionary<Type, List<Func<IContainer, object>>> Implementations { get; }

        public ImplementationFactory()
        {
            Implementations = new Dictionary<Type, List<Func<IContainer, object>>>();
        }

        public object Get(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            var implementations = Implementations[serviceType];
            return implementations[implementations.Count - 1](context.Container);
        }

        public IEnumerable<object> GetAll(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            var implementations = Implementations[serviceType];
            return implementations
                .Select(i => i(context.Container))
                .ToArray();
        }

        public void Set<T>(Func<IContainer, object> implementation)
        {
            Set(typeof(T), implementation);
        }

        public void Set(Type serviceType, Func<IContainer, object> implementation)
        {
            if (Implementations.ContainsKey(serviceType))
            {
                Implementations[serviceType].Add(implementation);
            }
            else
            {
                Implementations[serviceType] = new List<Func<IContainer, object>>
                {
                    implementation
                };
            }
        }
    }
}