using DependencyResolver.Containers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Builder.Common
{
    public class CommonBuilder : IBuilder
    {
        private readonly ConcurrentDictionary<Type, CommonBuilderConstructor> _constructors;

        public CommonBuilder()
        {
            _constructors = new ConcurrentDictionary<Type, CommonBuilderConstructor>();
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

        private CommonBuilderConstructor GenerateConstructor(Type implementationType)
        {
            return new CommonBuilderConstructor();
        }
    }
}