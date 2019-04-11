using DependencyResolver.Builder;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Resolvers
{
    public class RegisterResolver : IResolver
    {
        public Dictionary<Type, List<ResolvedType>> Types { get; }

        public RegisterResolver()
        {
            Types = new Dictionary<Type, List<ResolvedType>>();
        }

        public IEnumerable<ResolvedType> TryResolve(Type serviceType, Container container)
        {
            if (Types.ContainsKey(serviceType))
            {
                return Types[serviceType];
            }
            else
            {
                return new ResolvedType[0];
            }
        }

        private void Register(Type serviceType, ResolvedType resolved)
        {
            if (Types.ContainsKey(serviceType))
            {
                Types[serviceType].Add(resolved);
            }
            else
            {
                Types[serviceType] = new List<ResolvedType>
                {
                    resolved
                };
            }
        }

        public void RegisterType<Tservice, Timplementation>(IFactory factory, IBuilder builder) where Timplementation : Tservice
        {
            var tService = typeof(Tservice);
            var tImplementation = typeof(Timplementation);

            var type = new ResolvedType
            {
                ImplementationType = tImplementation,
                Factory = factory,
                Builder = builder
            };
            Register(tService, type);
        }

        public void RegisterType<Tservice>(IFactory factory, IBuilder builder)
        {
            var tService = typeof(Tservice);

            var type = new ResolvedType
            {
                ImplementationType = tService,
                Factory = factory,
                Builder = builder
            };
            Register(tService, type);
        }

        public void RegisterType(Type serviceType, Type implementationType, IFactory factory, IBuilder builder)
        {
            var type = new ResolvedType
            {
                ImplementationType = implementationType,
                Factory = factory,
                Builder = builder
            };
            Register(serviceType, type);
        }

        public void RegisterType(Type implementationType, IFactory factory, IBuilder builder)
        {
            var type = new ResolvedType
            {
                ImplementationType = implementationType,
                Factory = factory,
                Builder = builder
            };
            Register(implementationType, type);
        }
    }
}