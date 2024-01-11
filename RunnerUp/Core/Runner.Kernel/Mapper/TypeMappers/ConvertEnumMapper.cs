
namespace Runner.Kernel.Mapper.TypeMappers
{
    public class ConvertEnumMapper : ITypeMapper
    {
        private readonly EasyMapper _context;

        public ConvertEnumMapper(EasyMapper context)
        {
            _context = context;
        }

        public bool IsApplicable(Type fromType, Type toType)
        {
            return fromType.IsEnum || toType.IsEnum;
        }

        public object? Map(Type fromType, object? from, Type toType)
        {
            if (!fromType.IsEnum && toType.IsEnum)
            {
                return MapOtherToEnum(fromType, from, toType);
            }
            else if (fromType.IsEnum && !toType.IsEnum)
            {
                return MapEnumToOther(fromType, from, toType);
            }
            else
            {
                return MapEnumToEnum(fromType, from, toType);
            }
        }

        private object? MapEnumToOther(Type fromType, object? from, Type toType)
        {
            if (from == null)
            {
                return null;
            }
            else
            {
                if (toType.FullName == "System.String")
                {
                    return from.ToString();
                }
                else
                {
                    var fromInt = Convert.ToInt32(from);
                    var mapper = _context.GetMapper(typeof(int), toType);
                    try
                    {
                        var to = mapper.Map(typeof(int), fromInt, toType);
                        return to;
                    }
                    catch (Exception err)
                    {
                        throw new Exception($"Invalid convert enum! FromType: \"{fromType.FullName}\", ToType: \"{toType.FullName}\", Error: {err.Message}");
                    }
                }
            }
        }

        private object? MapOtherToEnum(Type fromType, object? from, Type toType)
        {
            if (from == null)
            {
                return null;
            }
            else
            {
                return Enum.Parse(toType, from.ToString() ?? "");
            }
        }

        private object? MapEnumToEnum(Type fromType, object? from, Type toType)
        {
            if (from == null)
            {
                return null;
            }
            else
            {
                var fromStr = Convert.ToInt32(from).ToString();
                var toValue = Enum.Parse(toType, fromStr);
                return toValue;
            }
        }
    }
}
