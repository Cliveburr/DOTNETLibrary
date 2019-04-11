using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using DependencyResolver.Resolvers;

namespace DependencyResolver.Containers
{
    public class Scope : IContainer, IDisposable
    {
        private readonly Container _container;

        public ConcurrentDictionary<Type, object> ScopeInstances { get; }

        public Scope(Container container)
        {
            _container = container;
            ScopeInstances = new ConcurrentDictionary<Type, object>();
        }

        public void Dispose()
        {
            ScopeInstances.Clear();
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type serviceType)
        {
            return Resolve(serviceType, new ResolveContext
            {
                Container = this,
                Chain = new ChainDependency(),
                Bag = new Dictionary<string, object>()
                {
                    {  "ScopeInstances", ScopeInstances }
                }
            });
        }

        public object Resolve(Type serviceType, ResolveContext context)
        {
            return _container.Resolve(serviceType, context);
        }

        public List<ResolvedType> ResolvedTypes(Type serviceType)
        {
            return _container.ResolvedTypes(serviceType);
        }
    }
}