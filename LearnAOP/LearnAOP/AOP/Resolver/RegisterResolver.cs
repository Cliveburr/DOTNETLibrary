using LearnAOP.AOP.Builder;
using LearnAOP.AOP.Factory;
using LearnAOP.AOP.Helpers;
using LearnAOP.AOP.Lifetime;
using LearnAOP.AOP.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAOP.AOP.Resolver
{
    public class RegisterResolver : IResolver
    {
        private IDictionary<string, RegistredType> _register;

        private class RegistredType : ResolvedType
        {
            public ILifetime PreferedLifetime { get; set; }
            public IBuilder PreferedBuilder { get; set; }
        }

        public RegisterResolver()
        {
            _register = new Dictionary<string, RegistredType>();
        }

        public RegisterResolver RegisterType<Tinterface, Tclass>(ILifetime lifetime = null, IBuilder builder = null)
        {
            var typeInterface = typeof(Tinterface);
            var nameInterface = typeInterface.FullName;
            var typeClass = typeof(Tclass);

            if (_register.ContainsKey(nameInterface))
            {
                throw new Exception($"Already have this type \"{nameInterface}\" registred!");
            }
            else
            {
                var newRecord = new RegistredType
                {
                    InterfaceType = typeInterface,
                    ClassType = typeof(Tclass),
                    PreferedLifetime = lifetime,
                    PreferedBuilder = builder
                };

                if (newRecord.PreferedLifetime == null)
                {
                    var lifetimeAttribute = AttributeHelper.GetOneFromFirstAttribute<LifetimeAttribute>(typeClass, typeInterface);
                    if (lifetimeAttribute != null)
                    {
                        newRecord.PreferedLifetime = (ILifetime)Activator.CreateInstance(lifetimeAttribute.LifetimeType);
                    }
                }

                if (newRecord.PreferedBuilder == null)
                {
                    var builderAttribute = AttributeHelper.GetOneFromFirstAttribute<BuilderAttribute>(typeClass, typeInterface);
                    if (builderAttribute != null)
                    {
                        newRecord.PreferedBuilder = (IBuilder)Activator.CreateInstance(builderAttribute.BuilderType);
                    }
                }

                _register[nameInterface] = newRecord;
            }

            return this;
        }

        public void Execution(object data, NextPipelineDelegate next)
        {
            var context = data as ResolverContext;
            var name = context.ToResolveType.FullName;

            if (_register.ContainsKey(name))
            {
                var record = _register[name];

                if (record.PreferedLifetime != null)
                    context.ResolvedLifetime = new StaticFactory<ILifetime>(record.PreferedLifetime);

                if (record.PreferedBuilder != null)
                    context.ResolvedBuilder = new StaticFactory<IBuilder>(record.PreferedBuilder);

                context.ResolvedType = record;

                next(true);
            }
            else
            {
                next(false);
            }
        }
    }

    public static class RegisterResolverExtension
    {
        public static RegisterResolver SetRegisterResolver(this Container container)
        {
            var registerResolve = new RegisterResolver();
            container.Resolvers.AddAtEnd(registerResolve);
            return registerResolve;
        }
    }
}