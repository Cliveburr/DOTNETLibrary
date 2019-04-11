using DependencyResolver.Containers;
using DependencyResolver.Extensions;
using DependencyResolver.Tests.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Extensions
{
    [TestClass]
    public class SimpleRegisterExtensionsTests
    {
        [TestMethod]
        public void SingletonRegister()
        {
            using (var container = new Container())
            {
                container
                    .WithRegisterResolver()
                    .WithSingletonFactory()
                    .WithGenericBuilder()
                    .RegisterType<IRootSubject, RootSubject>();

                var subject = container.Resolve<IRootSubject>();
                Assert.AreEqual(typeof(RootSubject), subject.GetType());
            }
        }
    }
}