using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Resolvers;
using DependencyResolver.Tests.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Factory
{
    [TestClass]
    public class GenericFactoryTest
    {
        [TestMethod]
        public void GenericTypeRegister()
        {
            using (var container = new Container())
            {
                var resolver = new RegisterResolver();
                container.Resolvers.Add(new GenericResolver());
                container.Resolvers.Add(resolver);

                var builder = new CommonBuilder();

                var singletonFactory = new SingletonFactory();
                resolver.RegisterType(typeof(IGenericSubject<>), typeof(GenericSubject<>), singletonFactory, builder);

                var genericSubject0 = container.Resolve<IGenericSubject<int>>();
                var genericSubject1 = container.Resolve<IGenericSubject<int>>();
                Assert.AreEqual(genericSubject0.Id, genericSubject1.Id);
            }
        }
    }
}