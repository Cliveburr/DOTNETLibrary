using DependencyResolver.Containers;
using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Factory
{
    public class ConstantFactory : IFactory
    {
        public Dictionary<Type, List<object>> Instances { get; }

        public ConstantFactory()
        {
            Instances = new Dictionary<Type, List<object>>();
        }

        public object Get(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            var objects = Instances[serviceType];
            return objects[objects.Count - 1];
        }

        public IEnumerable<object> GetAll(Type serviceType, ResolvedType resolvedType, ResolveContext context)
        {
            return Instances[serviceType];
        }

        public void Set<T>(T instance)
        {
            Set(typeof(T), instance);
        }

        public void Set(Type serviceType, object instance)
        {
            if (Instances.ContainsKey(serviceType))
            {
                Instances[serviceType].Add(instance);
            }
            else
            {
                Instances[serviceType] = new List<object>
                {
                    instance
                };
            }
        }
    }

}