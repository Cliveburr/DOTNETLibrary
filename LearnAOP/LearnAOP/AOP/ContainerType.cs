using LearnAOP.AOP.Builder;
using LearnAOP.AOP.Lifetime;
using LearnAOP.AOP.Resolver;

namespace LearnAOP.AOP
{
    public class ContainerType
    {
        public uint Index { get; set; }
        public Container Container { get; set; }
        public ResolvedType ResolvedType { get; set; }
        public ILifetime Lifetime { get; set; }
        public IBuilder Builder { get; set; }
    }
}