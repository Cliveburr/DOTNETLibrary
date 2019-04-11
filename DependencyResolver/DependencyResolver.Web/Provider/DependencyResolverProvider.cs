using DependencyResolver.Builder;
using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Resolvers;
using DependencyResolver.Web.Scope;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Web.Provider
{
    public class DependencyResolverProvider : IServiceProvider
    {
        public Container Container { get; }
        public TransientFactory TransientFactory { get; }
        public ConstantFactory ConstantFactory { get; }
        public ScopeFactory ScopeFactory { get; }
        public SingletonFactory SingletonFactory { get; }
        public ImplementationFactory ImplementationFactory { get; }

        public DependencyResolverProvider(IServiceCollection collection)
        {
            Container = new Container();
            var resolver = new RegisterResolver();
            Container.Resolvers.Add(resolver);
            Container.Resolvers.Add(new EnumerableResolver());
            Container.Resolvers.Add(new GenericResolver());

            var builder = new CommonBuilder();
            TransientFactory = new TransientFactory();
            ConstantFactory = new ConstantFactory();
            ScopeFactory = new ScopeFactory();
            SingletonFactory = new SingletonFactory();
            ImplementationFactory = new ImplementationFactory();

            ConstantFactory.Set<IServiceProvider>(this);
            resolver.RegisterType<IServiceProvider>(ConstantFactory, null);

            ConstantFactory.Set<IServiceScopeFactory>(new DependencyResolverScopeFactory(Container));
            resolver.RegisterType<IServiceScopeFactory>(ConstantFactory, null);

            Register(resolver, builder, collection);
        }

        private void Register(RegisterResolver register, IBuilder builder, IServiceCollection collection)
        {
            foreach (var service in collection)
            {
                if (service.ImplementationInstance != null)
                {
                    ConstantFactory.Set(service.ServiceType, service.ImplementationInstance);
                    register.RegisterType(service.ServiceType, ConstantFactory, null);
                    continue;
                }
                else if (service.ImplementationFactory != null)
                {
                    ImplementationFactory.Set(service.ServiceType, (container) =>
                    {
                        return service.ImplementationFactory(this);
                    });
                    register.RegisterType(service.ServiceType, ImplementationFactory, null);
                    continue;
                }
                else if (service.ImplementationType != null)
                {
                    switch (service.Lifetime)
                    {
                        case ServiceLifetime.Singleton: register.RegisterType(service.ServiceType, service.ImplementationType, SingletonFactory, builder); break;
                        case ServiceLifetime.Scoped: register.RegisterType(service.ServiceType, service.ImplementationType, ScopeFactory, builder); break;
                        case ServiceLifetime.Transient: register.RegisterType(service.ServiceType, service.ImplementationType, TransientFactory, builder); break;
                    }
                    continue;
                }

                throw new InvalidOperationException("Invalid service descriptor");
            }
        }

        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }
    }
}