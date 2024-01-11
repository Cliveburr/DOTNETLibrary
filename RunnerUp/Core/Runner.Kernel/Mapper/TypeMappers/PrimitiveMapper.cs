
namespace Runner.Kernel.Mapper.TypeMappers
{
    public class PrimitiveMapper : ITypeMapper
    {
        public bool IsApplicable(Type fromType, Type toType)
        {
            return fromType.IsPrimitive && toType.IsPrimitive;
        }

        public object? Map(Type fromType, object? from, Type toType)
        {
            return Convert.ChangeType(from, toType);
        }
    }
}
