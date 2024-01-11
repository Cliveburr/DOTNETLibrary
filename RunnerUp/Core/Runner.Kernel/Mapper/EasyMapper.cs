using Runner.Kernel.Mapper.TypeMappers;

namespace Runner.Kernel.Mapper
{
    public class EasyMapper
    {
        public List<ITypeMapper> Mappers { get; set; }
        private Dictionary<string, ITypeMapper> _cache;

        public EasyMapper()
        {
            _cache = new Dictionary<string, ITypeMapper>();
            Mappers = new List<ITypeMapper>
            {
                new EquivalentMapper(),
                new PrimitiveMapper(),
                new ConvertEnumMapper(this),
                new CastStringMapper(),
                new EnumerableMapper(this),
                new ObjectToObjectMapper(this)
            };
        }

        public void CleanCache()
        {
            _cache.Clear();
        }

        public To MapTo<To>(object from)
        {
            var toType = typeof(To);
            var fromType = from.GetType();
            var to = MapTo(fromType, from, toType);
            if (to == null)
            {
                throw new Exception("Map to null invalid!");
            }
            else
            {
                return (To)to;
            }
        }

        public To? MapTo<From, To>(From? from)
        {
            var toType = typeof(To);
            var fromType = typeof(From);
            var to = MapTo(fromType, from, toType);
            if (to == null)
            {
                return default;
            }
            else
            {
                return (To)to;
            }
        }

        public object? MapTo(Type fromType, object? from, Type toType)
        {
            var mapper = GetMapper(fromType, toType);
            try
            {
                return mapper.Map(fromType, from, toType);
            }
            catch (Exception err)
            {
                if (err.Message.StartsWith("Invalid mapper"))
                {
                    throw;
                }
                else
                {
                    throw new Exception($"Invalid mapper! FromType: \"{fromType.FullName}\", ToType: \"{toType.FullName}\", Error: {err.Message}");
                }
            }
        }

        public ITypeMapper GetMapper(Type fromType, Type toType)
        {
            //TODO: implement decorator

            var cacheKey = $"{fromType.FullName}:{toType.FullName}";
            if (_cache.ContainsKey(cacheKey))
            {
                return _cache[cacheKey];
            }
            else
            {
                var mapper = Mappers
                    .FirstOrDefault(m => m.IsApplicable(fromType, toType));
                if (mapper != null)
                {
                    _cache[cacheKey] = mapper;
                    return mapper;
                }

                throw new NotImplementedException($"Not Implemented FromType=\"{fromType.FullName}\" ToType=\"{toType.FullName}\"");
            }
        }
    }
}
