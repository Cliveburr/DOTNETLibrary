
namespace Runner.Kernel.Mapper.TypeMappers
{
    public class ObjectToObjectMapper : ITypeMapper
    {
        private readonly EasyMapper _context;

        public ObjectToObjectMapper(EasyMapper context)
        {
            _context = context;
        }

        public bool IsApplicable(Type fromType, Type toType)
        {
            return (fromType.IsClass && toType.IsClass) || (fromType.IsValueType && toType.IsValueType);
        }

        public object? Map(Type fromType, object? from, Type toType)
        {
            if (from == null)
            {
                return null;
            }
            else
            {
                var fromProperties = fromType.GetProperties();
                var toProperties = toType.GetProperties();
                var to = Activator.CreateInstance(toType);

                foreach (var fromProperty in fromProperties)
                {
                    var toProperty = toProperties
                        .Where(tp => tp.Name == fromProperty.Name)
                        .FirstOrDefault();
                    if (toProperty == null)
                    {
                        continue;
                    }

                    var fromValue = fromProperty.GetValue(from);
                    var fromPropertyType = fromProperty.PropertyType;
                    var toPropertyType = toProperty.PropertyType;

                    var mapper = _context.GetMapper(fromPropertyType, toPropertyType);
                    try
                    {
                        var toValue = mapper.Map(fromPropertyType, fromValue, toPropertyType);
                        toProperty.SetValue(to, toValue);
                    }
                    catch (Exception err)
                    {
                        throw new Exception($"Invalid mapper! FromType: \"{fromType.FullName}\", Property: \"{fromProperty.Name}\", ToType: \"{toType.FullName}\", Error: {err.Message}");
                    }
                }

                return to;
            }
        }
    }
}
