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
    public class SingletonFactoryTest
    {
        [TestMethod]
        public void SingletonRegister()
        {
            using (var container = new Container())
            {
                var factory = new SingletonFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                resolver.RegisterType<IRootSubject, RootSubject>(factory, builder);

                var subjectA = container.Resolve<IRootSubject>();
                Assert.AreEqual(typeof(RootSubject), subjectA.GetType());

                var subjectB = container.Resolve<IRootSubject>();
                Assert.AreEqual(subjectA.Id, subjectB.Id);
            }
        }
    }
}