using LearnAOP.AOP.Builder;
using LearnAOP.AOP.Factory;
using LearnAOP.AOP.Helpers;
using LearnAOP.AOP.Lifetime;
using LearnAOP.AOP.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LearnAOP.AOP.Resolver
{
    public delegate string HotLoadAssemblyPathDelegate(Type type);

    public class HotLoadResolver : IResolver
    {
        public HotLoadAssemblyPathDelegate HotLoadAssemblyPath { get; set; }

        public HotLoadResolver(HotLoadAssemblyPathDelegate hotLoadAssemblyPath)
        {
            HotLoadAssemblyPath = hotLoadAssemblyPath;
        }

        public void Execution(object data, NextPipelineDelegate next)
        {
            var context = data as ResolverContext;
            var name = context.ToResolveType.FullName;

            context.ResolvedType = DoHotLoad(context);

            if (context.ResolvedType != null)
            {
                var typeInterface = context.ResolvedType.InterfaceType;
                var typeClass = context.ResolvedType.ClassType;

                var lifetimeAttribute = AttributeHelper.GetOneFromFirstAttribute<LifetimeAttribute>(typeClass, typeInterface);
                if (lifetimeAttribute != null)
                {
                    context.ResolvedLifetime = new StaticFactory<ILifetime>((ILifetime)Activator.CreateInstance(lifetimeAttribute.LifetimeType));
                }

                var builderAttribute = AttributeHelper.GetOneFromFirstAttribute<BuilderAttribute>(typeClass, typeInterface);
                if (builderAttribute != null)
                {
                    context.ResolvedBuilder = new StaticFactory<IBuilder>((IBuilder)Activator.CreateInstance(builderAttribute.BuilderType));
                }

                next(true);
            }
            else
            {
                next(false);
            }
        }

        private ResolvedType DoHotLoad(ResolverContext context)
        {
            var assemblyPath = HotLoadAssemblyPath(context.ToResolveType);

            var assembly = Assembly.LoadFrom(assemblyPath);

            var types = assembly.GetTypes()
                .Where(t => context.ToResolveType.IsAssignableFrom(t) && !t.IsInterface);

            if (!types.Any())
                return null;

            return new ResolvedType
            {
                InterfaceType = context.ToResolveType,
                ClassType = types.First()
            };
        }
    }
}