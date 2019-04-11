using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ScopeContainer = DependencyResolver.Containers.Scope;

namespace DependencyResolver.Web.Scope
{
    public class DependencyResolverScope : IServiceScope, IServiceProvider
    {
        public IServiceProvider ServiceProvider { get { return this; } }

        private readonly ScopeContainer _scope;

        public DependencyResolverScope(ScopeContainer scope)
        {
            _scope = scope;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
            {
                return this;
            }
            else
            {
                return _scope.Resolve(serviceType);
            }
        }
    }
}