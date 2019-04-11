using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyResolver.WebTests.Resolvers
{
    public class NameResolver : IResolver
    {
        public IEnumerable<ResolvedType> TryResolve(Type serviceType, Container container)
        {
            if (serviceType.FullName.StartsWith("DependencyResolver.WebTests.Subjects.I"))
            {
                var name = serviceType.Name;
                if (name.StartsWith("I"))
                {
                    name = name.Substring(1);
                }

                var implementationFullName = serviceType.FullName.Replace(serviceType.Name, name);
                var implementationType = Type.GetType(implementationFullName);

                yield return new ResolvedType
                {
                    Builder = new CommonBuilder(),
                    Factory = new SingletonFactory(),
                    ImplementationType = implementationType
                };
            }
            yield break;
        }
    }
}