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
        private readonly Container _container;
        private readonly TransientFactory _transientFactory;
        private readonly ConstantFactory _constantFactory;
        private readonly ScopeFactory _scopeFactory;
        private readonly SingletonFactory _singletonFactory;
        private readonly ImplementationFactory _implementationFactory;

        public DependencyResolverProvider(IServiceCollection collection)
        {
            _container = new Container();
            var resolver = new RegisterResolver();
            _container.Resolvers.Add(resolver);
            _container.Resolvers.Add(new EnumerableResolver());
            _container.Resolvers.Add(new GenericResolver());

            var builder = new CommonBuilder();
            _transientFactory = new TransientFactory();
            _constantFactory = new ConstantFactory();
            _scopeFactory = new ScopeFactory();
            _singletonFactory = new SingletonFactory();
            _implementationFactory = new ImplementationFactory();

            _constantFactory.Set<IServiceProvider>(this);
            resolver.RegisterType<IServiceProvider>(_constantFactory, null);

            _constantFactory.Set<IServiceScopeFactory>(new DependencyResolverScopeFactory(_container));
            resolver.RegisterType<IServiceScopeFactory>(_constantFactory, null);

            Register(resolver, builder, collection);
        }

        private void Register(RegisterResolver register, IBuilder builder, IServiceCollection collection)
        {
            foreach (var service in collection)
            {
                if (service.ImplementationInstance != null)
                {
                    _constantFactory.Set(service.ServiceType, service.ImplementationInstance);
                    register.RegisterType(service.ServiceType, _constantFactory, null);
                    continue;
                }
                else if (service.ImplementationFactory != null)
                {
                    _implementationFactory.Set(service.ServiceType, (container) =>
                    {
                        return service.ImplementationFactory(this);
                    });
                    register.RegisterType(service.ServiceType, _implementationFactory, null);
                    continue;
                }
                else if (service.ImplementationType != null)
                {
                    switch (service.Lifetime)
                    {
                        case ServiceLifetime.Singleton: register.RegisterType(service.ServiceType, service.ImplementationType, _singletonFactory, builder); break;
                        case ServiceLifetime.Scoped: register.RegisterType(service.ServiceType, service.ImplementationType, _scopeFactory, builder); break;
                        case ServiceLifetime.Transient: register.RegisterType(service.ServiceType, service.ImplementationType, _transientFactory, builder); break;
                    }
                    continue;
                }

                throw new InvalidOperationException("Invalid service descriptor");
            }
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }
    }
}