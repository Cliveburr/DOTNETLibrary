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
    public class ImplementationFactoryTest
    {
        [TestMethod]
        public void ImplementationRegister()
        {
            using (var container = new Container())
            {
                var factory = new ImplementationFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                factory.Set<IRootSubject>(c =>
                {
                    return new RootSubject { Name = "feito agora" };
                });
                resolver.RegisterType<IRootSubject, RootSubject>(factory, builder);

                var subject = container.Resolve<IRootSubject>();
                Assert.AreEqual(subject.Name, "feito agora");
            }
        }
    }
}