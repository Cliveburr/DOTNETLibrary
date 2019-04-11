using DependencyResolver.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Builder
{
    public interface IBuilder
    {
        object Instantiate(Type serviceType, Type implementationType, ResolveContext context, bool cachedBuilder);
    }
}