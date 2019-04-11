using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyResolver.Containers;
using DependencyResolver.Resolvers;

namespace DependencyResolver.Factory
{
    public class EnumerableFactory : IFactory
    {
        public object Get(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            var resolvedTypes = context.Container.ResolvedTypes(resolvedType.ImplementationType);

            var values = (resolvedTypes ?? new List<ResolvedType>())
                .SelectMany(r => r.Factory.GetAll(r.ImplementationType, r, context))
                .ToArray();

            var array = Array.CreateInstance(resolvedType.ImplementationType, values.Length);
            for (var i = 0; i < values.Length; i++)
            {
                array.SetValue(values[i], i);
            }
            return array;
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