using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Proxy.Builder;
using DependencyResolver.Resolvers;
using DependencyResolver.Tests.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Tests.Proxy
{
    [TestClass]
    public class ProxyTests
    {
        [TestMethod]
        public void ProxyRegister()
        {
            using (var container = new Container())
            {
                var factory = new TransientFactory();
                var builder = new ProxyBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                resolver.RegisterType<IRootSubject, RootSubject>(factory, builder);

                var subject = container.Resolve<IRootSubject>();
                Assert.IsNotNull(subject);

                subject.CallMethod0();

                var call1 = subject.CallMethod1();
                Assert.AreEqual(call1, "CallMethod1 done");

                var call2 = subject.CallMethod2("teste");
                Assert.AreEqual(call2, "CallMethod2 \"teste\" done");
            }
        }

        [TestMethod]
        public void ProxyAllMethods()
        {
            using (var container = new Container())
            {
                var factory = new TransientFactory();
                var builder = new InterceptionBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                var logInterception = new LogInterception();
                builder.Interceptions.Add(logInterception);

                resolver.RegisterType<IInterceptSubject, InterceptSubject>(factory, builder);

                var subject = container.Resolve<IInterceptSubject>();
                Assert.IsNotNull(subject);

                subject.LogEvent0("test");
                subject.LogEvent1(3, "novo test");
                subject.LogEvent2();

                var log = string.Join(Environment.NewLine, logInterception.Logs);
                Console.Write(log);
            }
        }
    }
}