using DependencyResolver.Containers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ScopeContainer = DependencyResolver.Containers.Scope;

namespace DependencyResolver.Web.Scope
{
    public class DependencyResolverScopeFactory : IServiceScopeFactory
    {
        private readonly Container _container;

        public DependencyResolverScopeFactory(Container container)
        {
            _container = container;
        }

        public IServiceScope CreateScope()
        {
            return new DependencyResolverScope(new ScopeContainer(_container));
        }
    }
}