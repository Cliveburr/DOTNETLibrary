using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Resolvers;
using DependencyResolver.Tests.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Builder
{
    [TestClass]
    public class CommonBuilderTest
    {
        [TestMethod]
        public void OneDeepRegister()
        {
            using (var container = new Container())
            {
                var factory = new TransientFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                resolver.RegisterType<IRootSubject, RootSubject>(factory, builder);
                resolver.RegisterType<ILevelSubject, LevelSubject>(factory, builder);

                var subject0 = container.Resolve<ILevelSubject>();
                var subject1 = container.Resolve<ILevelSubject>();
                Assert.AreNotEqual(subject0.Id, subject1.Id);
                Assert.AreNotEqual(subject0.Root.Id, subject1.Root.Id);
            }
        }


        [TestMethod]
        public void TwoDeepRegister()
        {
            using (var container = new Container())
            {
                var factory = new TransientFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                resolver.RegisterType<IRootSubject, RootSubject>(factory, builder);
                resolver.RegisterType<ILevelSubject, LevelSubject>(factory, builder);
                resolver.RegisterType<FixedSubject>(factory, builder);

                var subject0 = container.Resolve<FixedSubject>();
                var subject1 = container.Resolve<FixedSubject>();
                Assert.AreNotEqual(subject0.Id, subject1.Id);
                Assert.AreNotEqual(subject0.Level.Id, subject1.Level.Id);
                Assert.AreNotEqual(subject0.Level.Root.Id, subject1.Level.Root.Id);
            }
        }
    }
}