using DependencyResolver.Web.Provider;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider BuildDependencyResolver(this IServiceCollection collection)
        {
            return new DependencyResolverProvider(collection);
        }
    }
}