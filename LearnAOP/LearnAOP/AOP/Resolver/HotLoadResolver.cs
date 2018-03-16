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

        private IDictionary<string, ResolvedType> _resolveds;
        
        public HotLoadResolver(HotLoadAssemblyPathDelegate hotLoadAssemblyPath)
        {
            HotLoadAssemblyPath = hotLoadAssemblyPath;
            _resolveds = new Dictionary<string, ResolvedType>();
        }

        public void Execution(object data, NextPipelineDelegate next)
        {
            var context = data as ResolverContext;
            var name = context.ToResolveType.FullName;

            if (_resolveds.ContainsKey(name))
            {
                context.ResolvedType = _resolveds[name];

                next(true);
            }
            else
            {
                context.ResolvedType = DoHotLoad(context);

                if (context.ResolvedType != null)
                {
                    _resolveds[name] = context.ResolvedType;

                    next(true);
                }
                else
                {
                    next(false);
                }
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