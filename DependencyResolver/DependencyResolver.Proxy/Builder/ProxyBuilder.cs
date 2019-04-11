using DependencyResolver.Builder;
using DependencyResolver.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Proxy.Builder
{
    public class ProxyBuilder : IBuilder
    {
        public object Instantiate(Type serviceType, Type implementationType, ResolveContext context, bool cachedBuilder)
        {
            throw new NotImplementedException();
        }
    }
}