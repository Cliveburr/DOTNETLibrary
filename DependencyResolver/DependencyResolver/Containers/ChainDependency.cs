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

        public void CheckCircularDependency(Type type)
        {
            if (_chain.Contains(type))
            {
                throw new InvalidOperationException($"Circular dependency detected on \"{type.FullName}\"");
            }

            _chain.Add(type);
        }

        public void Release(Type type)
        {
            _chain.Remove(type);
        }
    }
}