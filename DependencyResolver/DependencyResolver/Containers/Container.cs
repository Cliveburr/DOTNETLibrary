using DependencyResolver.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Containers
{
    public class Container : IContainer, IDisposable
    {
        private readonly ConcurrentDictionary<Type, List<ResolvedType>> _resolveds;

        public List<IResolver> Resolvers { get; }

        public Container()
        {
            _resolveds = new ConcurrentDictionary<Type, List<ResolvedType>>();
            Resolvers = new List<IResolver>();
        }

        public void Dispose()
        {
            _resolveds.Clear();
            Resolvers.Clear();
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
            });
        }

        public object Resolve(Type serviceType, ResolveContext context)
        {
            var resolveds = ResolvedTypes(serviceType);
            var mostValidResolved = resolveds?[resolveds.Count - 1];
            return mostValidResolved?.Factory.Get(serviceType, mostValidResolved, context);
        }

        public List<ResolvedType> ResolvedTypes(Type serviceType)
        {
            return _resolveds.GetOrAdd(serviceType, AddResolvers);
        }

        private List<ResolvedType> AddResolvers(Type serviceType)
        {
            var resolveds = new List<ResolvedType>();
            foreach (var resolve in Resolvers)
            {
                resolveds.AddRange(resolve.TryResolve(serviceType, this));
            }
            if (resolveds.Count == 0)
            {
                return null;
            }
            else
            {
                return resolveds;
            }
        }
    }
}