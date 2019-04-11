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
    public class ConstantFactoryTest
    {
        [TestMethod]
        public void ConstantRegister()
        {
            var obj = new RootSubject
            {
                Name = "Testing"
            };

            using (var container = new Container())
            {
                var factory = new ConstantFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                factory.Set<IRootSubject>(obj);
                resolver.RegisterType<IRootSubject, RootSubject>(factory, builder);

                var subject = container.Resolve<IRootSubject>();
                Assert.AreEqual(obj.Name, subject.Name);
            }
        }
    }
}