using DependencyResolver.Builder;
using DependencyResolver.Containers;
using DependencyResolver.Proxy.Interception;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Proxy.Builder
{
    public class ProxyBuilder : IBuilder
    {
        private readonly ConcurrentDictionary<Type, ProxyObject> _proxies;

        public List<IInterceptionQuery> Interceptions { get; }

        public ProxyBuilder()
        {
            _proxies = new ConcurrentDictionary<Type, ProxyObject>();
            Interceptions = new List<IInterceptionQuery>();
        }

        public object Instantiate(Type serviceType, Type implementationType, ResolveContext context, bool cachedBuilder)
        {
            var proxy = cachedBuilder ?
                _proxies.GetOrAdd(implementationType, i => CreateProxy(serviceType, i)) :
                CreateProxy(serviceType, implementationType);

            return proxy.Instantiate(implementationType, context);
        }

        public ProxyObject CreateProxy(Type serviceType, Type implementationType)
        {
            return new ProxyObject(serviceType, implementationType, Interceptions);
        }
    }
}