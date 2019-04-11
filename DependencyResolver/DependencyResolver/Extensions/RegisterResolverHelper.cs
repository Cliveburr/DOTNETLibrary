using DependencyResolver.Builder;
using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Extensions
{
    public class RegisterResolverHelper
    {
        private Container _container;
        private RegisterResolver _resolver;
        private IFactory _factory;
        private IBuilder _builder;

        public RegisterResolverHelper(Container container)
        {
            _container = container;
            _resolver = new RegisterResolver();
            _container.Resolvers.Add(_resolver);
        }

        private void CheckValidFactoryAndBuilder()
        {
            if (_factory == null)
            {
                throw new Exception("Factory is null, call with for some factory before!");
            }
            if (_builder == null)
            {
                throw new Exception("Builder is null, call with for some builder before!");
            }
        }

        public RegisterResolverHelper WithFactory(IFactory factory)
        {
            _factory = factory;
            return this;
        }

        public RegisterResolverHelper WithSingletonFactory()
        {
            _factory = new SingletonFactory();
            return this;
        }

        public RegisterResolverHelper WithGenericBuilder()
        {
            _builder = new CommonBuilder();
            return this;
        }

        public RegisterResolverHelper RegisterType<Tservice, Timplementation>() where Timplementation : Tservice
        {
            CheckValidFactoryAndBuilder();
            _resolver.RegisterType<Tservice, Timplementation>(_factory, _builder);
            return this;
        }
    }
}