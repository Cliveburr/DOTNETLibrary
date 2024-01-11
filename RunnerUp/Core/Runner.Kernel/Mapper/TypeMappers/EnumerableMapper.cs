using System.Collections;

namespace Runner.Kernel.Mapper.TypeMappers
{
    public class EnumerableMapper : ITypeMapper
    {
        private readonly EasyMapper _context;

        public EnumerableMapper(EasyMapper context)
        {
            _context = context;
        }

        public bool IsApplicable(Type fromType, Type toType)
        {
            var fromIsEnumerable = fromType.GetInterface(nameof(IEnumerable)) != null;
            var toIsEnumerable = toType.GetInterface(nameof(IEnumerable)) != null;

            if (fromIsEnumerable || toIsEnumerable)
            {
                if (!fromIsEnumerable || !toIsEnumerable)
                {
                    throw new Exception("Can't mapper one enumerable to not enumerable!");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public object? Map(Type fromType, object? from, Type toType)
        {
            var fromElementType = fromType.HasElementType ?
                fromType.GetElementType()! :
                fromType.GetGenericArguments().First();

            var toElementType = toType.HasElementType ?
                toType.GetElementType()! :
                toType.GetGenericArguments().First();

            var fromEnumerable = from as IEnumerable;
            if (fromEnumerable == null)
            {
                return null;
            }
            else
            {
                var mapper = _context.GetMapper(fromElementType, toElementType);

                var toListType = typeof(List<>).MakeGenericType(toElementType);
                var toList = (IList)Activator.CreateInstance(toListType)!;

                foreach (var fromValue in fromEnumerable)
                {
                    try
                    {
                        var toValue = mapper.Map(fromElementType, fromValue, toElementType);
                        toList.Add(toValue);
                    }
                    catch (Exception err)
                    {
                        throw new Exception($"Invalid mapper enumerable! FromType: \"{fromType.FullName}\", ToType: \"{toType.FullName}\", Error: {err.Message}");
                    }
                }

                if (toType.HasElementType)
                {
                    var to = Activator.CreateInstance(toType, toList.Count)!;
                    toList.CopyTo((Array)to, 0);
                    return to;
                }
                else
                {
                    return toList;
                }
            }
        }
    }
}
