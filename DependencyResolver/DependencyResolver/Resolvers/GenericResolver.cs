using System;
using System.Collections.Generic;
using System.Text;
using DependencyResolver.Containers;
using DependencyResolver.Factory;

namespace DependencyResolver.Resolvers
{
    public class GenericResolver : IResolver
    {
        private readonly IFactory _genericFactory;

        public GenericResolver()
        {
            _genericFactory = new GenericFactory();
        }

        public IEnumerable<ResolvedType> TryResolve(Type serviceType, Container container)
        {
            if (serviceType.IsConstructedGenericType)
            {
                var genericType = serviceType.GetGenericTypeDefinition();
                var resolvedTypes = container.ResolvedTypes(genericType);
                if (resolvedTypes?.Count > 0)
                {
                    yield return new ResolvedType
                    {
                        ImplementationType = genericType,
                        Factory = _genericFactory
                    };
                }
            }
            yield break;
        }
    }
}