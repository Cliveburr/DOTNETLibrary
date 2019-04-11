using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Containers
{
    public class ChainDependency
    {
        private readonly List<Type> _chain;

        public ChainDependency()
        {
            _chain = new List<Type>();
        }

        public void CheckCircularDependency(Type serviceType)
        {
            if (_chain.Contains(serviceType))
            {
                throw new InvalidOperationException($"Circular dependency detected on \"{serviceType.FullName}\"");
            }

            _chain.Add(serviceType);
        }

        public void Release(Type serviceType)
        {
            _chain.Remove(serviceType);
        }
    }
}