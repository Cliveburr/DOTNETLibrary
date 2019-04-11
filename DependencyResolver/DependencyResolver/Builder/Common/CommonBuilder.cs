using DependencyResolver.Containers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Builder.Common
{
    public class CommonBuilder : IBuilder
    {
        private readonly ConcurrentDictionary<Type, GenericBuilderConstructor> _constructors;

        public CommonBuilder()
        {
            _constructors = new ConcurrentDictionary<Type, GenericBuilderConstructor>();
        }

        public object Instantiate(Type serviceType, Type implementationType, ResolveContext context, bool cachedBuilder)
        {
            if (cachedBuilder)
            {
                var constructor = _constructors.GetOrAdd(implementationType, GenerateConstructor);
                return constructor.Instantiate(implementationType, context);
            }
            else
            {
                var constructor = GenerateConstructor(null);
                return constructor.Instantiate(implementationType, context);
            }
        }

        private GenericBuilderConstructor GenerateConstructor(Type implementationType)
        {
            return new GenericBuilderConstructor();
        }
    }
}