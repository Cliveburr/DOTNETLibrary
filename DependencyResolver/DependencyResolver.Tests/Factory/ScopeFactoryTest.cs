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
    public class ScopeFactoryTest
    {
        [TestMethod]
        public void ScopeRegister()
        {
            using (var container = new Container())
            {
                var builder = new CommonBuilder();

                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                var singletonFactory = new SingletonFactory();
                resolver.RegisterType<IRootSubject, RootSubject>(singletonFactory, builder);

                var scopeFactory = new ScopeFactory();
                resolver.RegisterType<ILevelSubject, LevelSubject>(scopeFactory, builder);

                using (var scope0 = new Scope(container))
                {
                    var levelSubject0 = scope0.Resolve<ILevelSubject>();
                    var rootSubject0 = scope0.Resolve<IRootSubject>();

                    using (var scope1 = new Scope(container))
                    {
                        var levelSubject1 = scope1.Resolve<ILevelSubject>();
                        var rootSubject1 = scope1.Resolve<IRootSubject>();

                        using (var scope2 = new Scope(container))
                        {
                            var levelSubject2 = scope2.Resolve<ILevelSubject>();
                            var rootSubject2 = scope2.Resolve<IRootSubject>();

                            Assert.AreEqual(rootSubject0.Id, rootSubject1.Id);
                            Assert.AreEqual(levelSubject0.Root.Id, rootSubject0.Id);
                            Assert.AreEqual(levelSubject1.Root.Id, rootSubject1.Id);

                            Assert.AreNotEqual(levelSubject0.Id, levelSubject1.Id);

                            Assert.AreEqual(rootSubject1.Id, rootSubject2.Id);
                            Assert.AreEqual(levelSubject1.Root.Id, rootSubject1.Id);
                            Assert.AreEqual(levelSubject2.Root.Id, rootSubject2.Id);

                            Assert.AreNotEqual(levelSubject1.Id, levelSubject2.Id);
                        }
                    }
                }
            }
        }
    }
}