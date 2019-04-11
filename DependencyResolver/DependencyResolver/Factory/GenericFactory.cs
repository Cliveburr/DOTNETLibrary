using System;
using System.Collections.Generic;
using System.Text;
using DependencyResolver.Containers;
using DependencyResolver.Resolvers;

namespace DependencyResolver.Factory
{
    public class GenericFactory : IFactory
    {
        public object Get(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            var resolveds = context.Container.ResolvedTypes(resolvedType.ImplementationType);
            var mostValidResolved = resolveds?[resolveds.Count - 1];

            var implementationType = mostValidResolved.ImplementationType.MakeGenericType(serviceType.GenericTypeArguments);

            return mostValidResolved.Factory.Get(serviceType, new ResolvedType
            {
                ImplementationType = implementationType,
                Factory = mostValidResolved.Factory,
                Builder = mostValidResolved.Builder
            }, context);
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