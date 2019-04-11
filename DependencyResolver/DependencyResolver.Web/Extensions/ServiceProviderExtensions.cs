using DependencyResolver.Resolvers;
using DependencyResolver.Web.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Web.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider SetResolver(this IServiceProvider provider, IResolver resolver)
        {
            var dependencyProvider = provider as DependencyResolverProvider;
            if (dependencyProvider == null)
            {
                throw new InvalidOperationException();
            }

            dependencyProvider.Container.Resolvers.Add(resolver);

            return provider;
        }
    }
}