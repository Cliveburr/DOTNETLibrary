
namespace Runner.Kernel.Mapper.TypeMappers
{
    public class CastStringMapper : ITypeMapper
    {
        public bool IsApplicable(Type fromType, Type toType)
        {
            return fromType.FullName == "System.String" || toType.FullName == "System.String";
        }

        public object? Map(Type fromType, object? from, Type toType)
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
                    if (string.IsNullOrEmpty(from as string))
                    {
                        switch (toType.FullName)
                        {
                            case "System.Byte": return default(Byte);
                            case "System.Int16": return default(Int16);
                            case "System.UInt16": return default(UInt16);
                            case "System.Int32": return default(Int32);
                            case "System.UInt32": return default(UInt32);
                            case "System.Int64": return default(Int64);
                            case "System.UInt64": return default(UInt64);
                            case "System.Single": return default(Single);
                            case "System.Guid": return default(Guid);
                            default: throw new Exception("CastStringMapper not full dev!");
                        }
                    }
                    else
                    {
                        var fromStr = from.ToString() ?? "";
                        switch (toType.FullName)
                        {
                            case "System.Byte": return Byte.Parse(fromStr);
                            case "System.Int16": return Int16.Parse(fromStr);
                            case "System.UInt16": return UInt16.Parse(fromStr);
                            case "System.Int32": return Int32.Parse(fromStr);
                            case "System.UInt32": return UInt32.Parse(fromStr);
                            case "System.Int64": return Int64.Parse(fromStr);
                            case "System.UInt64": return UInt64.Parse(fromStr);
                            case "System.Single": return Single.Parse(fromStr);
                            case "System.Guid": return Guid.Parse(fromStr);
                            default: throw new Exception("CastStringMapper not full dev!");
                        }
                    }
                }
            }
        }
    }
}
