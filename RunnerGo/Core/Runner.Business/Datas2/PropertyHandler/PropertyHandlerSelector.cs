using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.Validation;

namespace Runner.Business.Datas2.PropertyHandler
{
    public static class PropertyHandlerSelector
    {
        private static Dictionary<DataTypeEnum, IPropertyHandler> _cache;

        static PropertyHandlerSelector()
        {
            _cache = new Dictionary<DataTypeEnum, IPropertyHandler>();
        }

        public static IPropertyHandler Get(DataTypeEnum type)
        {
            if (!_cache.ContainsKey(type))
            {
                _cache[type] = type switch
                {
                    DataTypeEnum.String => new StringHandler(),
                    DataTypeEnum.StringList => new StringHandler(),
                    DataTypeEnum.Node => new NodeHandler(),
                    DataTypeEnum.Inherit => new InheritHandler(),
                    DataTypeEnum.Data => new DataHandler(),
                    DataTypeEnum.ScriptVersion => new ScriptVersionHandler(),
                    _ => throw new RunnerException($"Missing PropertyHandler for DataTypeEnum: {type}")
                };
            }
            return _cache[type];
        }
    }
}
