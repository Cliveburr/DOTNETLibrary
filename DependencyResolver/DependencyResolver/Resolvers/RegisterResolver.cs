using DependencyResolver.Builder;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Helpers;
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

        private void Register(Type serviceType, Type implementationType, IFactory factory, IBuilder builder)
        {
            if (factory == null)
            {
                var factoryAttribute = AttributeHelper.GetFirstAttribute<FactoryAttribute>(implementationType, serviceType);
                if (factoryAttribute != null)
                {
                    factory = factoryAttribute.GetFactory;
                }
                else
                {
                    throw new ArgumentNullException($"Factory invalid for ServiceType: \"{serviceType.FullName}\"!");
                }
            }

            if (builder == null)
            {
                var builderAttribute = AttributeHelper.GetFirstAttribute<BuilderAttribute>(implementationType, serviceType);
                if (builderAttribute != null)
                {
                    builder = builderAttribute.GetBuilder;
                }
            }

            var resolved = new ResolvedType
            {
                ImplementationType = implementationType,
                Factory = factory,
                Builder = builder
            };

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

        public void RegisterType<Tservice, Timplementation>(IFactory factory = null, IBuilder builder = null) where Timplementation : Tservice
        {
            var tService = typeof(Tservice);
            var tImplementation = typeof(Timplementation);

            Register(tService, tImplementation, factory, builder);
        }

        public void RegisterType<Tservice>(IFactory factory = null, IBuilder builder = null)
        {
            var tService = typeof(Tservice);

            Register(tService, tService, factory, builder);
        }

        public void RegisterType(Type serviceType, Type implementationType, IFactory factory = null, IBuilder builder = null)
        {
            Register(serviceType, implementationType, factory, builder);
        }

        public void RegisterType(Type implementationType, IFactory factory = null, IBuilder builder = null)
        {
            Register(implementationType, implementationType, factory, builder);
        }
    }
}