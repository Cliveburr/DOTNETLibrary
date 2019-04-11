using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Factory;
using DependencyResolver.Resolvers;
using DependencyResolver.Tests.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyResolver.Tests.Resolvers
{
    [TestClass]
    public class RegisterResolverTest
    {
        [TestMethod]
        public void SingleRegister()
        {
            using (var container = new Container())
            {
                var factory = new TransientFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                resolver.RegisterType<IRootSubject, RootSubject>(factory, builder);

                var subject = container.Resolve<IRootSubject>();
                Assert.AreEqual(typeof(RootSubject), subject.GetType());
            }
        }

        [TestMethod]
        public void MostValueRegister()
        {
            using (var container = new Container())
            {
                var factory = new TransientFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                resolver.RegisterType<IRootSubject, RootSubject>(factory, builder);
                resolver.RegisterType<IRootSubject, RootSubjectAlternative>(factory, builder);

                var subject = container.Resolve<IRootSubject>();
                Assert.AreEqual(typeof(RootSubjectAlternative), subject.GetType());
            }
        }

        [TestMethod]
        public void DirectRegister()
        {
            using (var container = new Container())
            {
                var factory = new TransientFactory();
                var builder = new CommonBuilder();
                var resolver = new RegisterResolver();
                container.Resolvers.Add(resolver);

                resolver.RegisterType<RootSubject>(factory, builder);

                var subject = container.Resolve<RootSubject>();
                Assert.AreEqual(typeof(RootSubject), subject.GetType());
            }
        }
    }
}
