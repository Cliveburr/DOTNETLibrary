using LearnAOP.AOP.Builder;
using LearnAOP.AOP.Factory;
using LearnAOP.AOP.Lifetime;
using System;

namespace LearnAOP.AOP.Resolver
{
    public class ResolverContext
    {
        public Container Container { get; set; }
        public Type ToResolveType { get; set; }
        public ResolvedType ResolvedType { get; set; }
        public IFactory<ILifetime> ResolvedLifetime { get; set; }
        public IFactory<IBuilder> ResolvedBuilder { get; set; }
    }
}