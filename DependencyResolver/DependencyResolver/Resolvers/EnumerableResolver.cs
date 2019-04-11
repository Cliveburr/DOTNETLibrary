using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Resolvers;

namespace DependencyResolver.Resolvers
{
    public class EnumerableResolver : IResolver
    {
        private readonly IFactory _enumerableFactory;

        public EnumerableResolver()
        {
            _enumerableFactory = new EnumerableFactory();
        }

        public IEnumerable<ResolvedType> TryResolve(Type serviceType, Container container)
        {
            if (serviceType.IsConstructedGenericType &&
                serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var itemType = serviceType.GenericTypeArguments[0];
                yield return new ResolvedType
                {
                    ImplementationType = itemType,
                    Factory = _enumerableFactory
                };
            }
            yield break;
        }
    }
}