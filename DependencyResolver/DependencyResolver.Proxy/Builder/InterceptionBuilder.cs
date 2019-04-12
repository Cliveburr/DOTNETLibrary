using DependencyResolver.Builder;
using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Helpers;
using DependencyResolver.Proxy.Interception;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyResolver.Proxy.Builder
{
    public class InterceptionBuilder : IBuilder
    {
        private readonly ConcurrentDictionary<Type, IBuilder> _builders;

        private IBuilder _commonBuilder;
        private ProxyBuilder _proxyBuilder;

        public InterceptionBuilder()
        {
            _builders = new ConcurrentDictionary<Type, IBuilder>();
            _proxyBuilder = new ProxyBuilder();
        }

        public List<IInterceptionQuery> Interceptions
        {
            get
            {
                return _proxyBuilder.Interceptions;
            }
        }

        public IBuilder CommonBuilder
        {
            get
            {
                if (_commonBuilder == null)
                {
                    _commonBuilder = new CommonBuilder();
                }
                return _commonBuilder;
            }
            set
            {
                _commonBuilder = value;
            }
        }

        public object Instantiate(Type serviceType, Type implementationType, ResolveContext context, bool cachedBuilder)
        {
            var builder = cachedBuilder ?
                _builders.GetOrAdd(implementationType, i => DefineBuilder(serviceType, i)) :
                DefineBuilder(serviceType, implementationType);

            return builder.Instantiate(serviceType, implementationType, context, cachedBuilder);
        }

        private IBuilder DefineBuilder(Type serviceType, Type implementationType)
        {
            var hasGlobalAttribute = AttributeHelper.GetAttributes<InterceptionAttribute>(implementationType, serviceType)
                .Any();
            if (hasGlobalAttribute)
            {
                return _proxyBuilder;
            }

            foreach(var method in implementationType.GetMethods())
            {
                var hasOnImplementationMethod = AttributeHelper.GetAttributes<InterceptionAttribute>(method)
                    .Any();
                if (hasOnImplementationMethod)
                {
                    return _proxyBuilder;
                }

                if (Interceptions.Any(i => i.IsApply(method)))
                {
                    return _proxyBuilder;
                }
            }

            foreach (var method in serviceType.GetMethods())
            {
                var hasOnServiceMethod = AttributeHelper.GetAttributes<InterceptionAttribute>(method)
                    .Any();
                if (hasOnServiceMethod)
                {
                    return _proxyBuilder;
                }

                if (Interceptions.Any(i => i.IsApply(method)))
                {
                    return _proxyBuilder;
                }
            }

            return CommonBuilder;
        }
    }
}