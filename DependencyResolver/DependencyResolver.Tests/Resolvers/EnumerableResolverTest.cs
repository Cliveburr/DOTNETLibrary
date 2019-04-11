using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Resolvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyResolver.Tests.Resolvers
{
    [TestClass]
    public class EnumerableResolverTest
    {
        [TestMethod]
        public void EnumerableTypeRegister()
        {
            using (var container = new Container())
            {
                var constantFactory = new ConstantFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);
                container.Resolvers.Add(new EnumerableResolver());

                constantFactory.Set("test um");
                constantFactory.Set("test dois");
                constantFactory.Set("test tres");

                resolver.RegisterType<string>(constantFactory, builder);

                var list = container.Resolve<IEnumerable<string>>();
                var array = list.ToArray();
                Assert.AreEqual(array.Length, 3);
                Assert.AreEqual(array[0], "test um");
                Assert.AreEqual(array[1], "test dois");
                Assert.AreEqual(array[2], "test tres");
            }
        }
    }
}