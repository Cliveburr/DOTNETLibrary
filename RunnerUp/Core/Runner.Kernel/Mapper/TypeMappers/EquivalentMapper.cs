
namespace Runner.Kernel.Mapper.TypeMappers
{
    public class EquivalentMapper : ITypeMapper
    {
        public bool IsApplicable(Type fromType, Type toType)
        {
            return fromType.IsEquivalentTo(toType);
        }

        public object? Map(Type fromType, object? from, Type toType)
        {
            return from;
        }
    }
}
