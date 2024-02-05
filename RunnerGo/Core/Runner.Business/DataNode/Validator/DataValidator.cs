using Runner.Business.DataNode.Validator.Types;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.DataNode.Validator
{
    public static class DataValidator
    {
        private static Dictionary<DataTypeEnum, IDataValidator> _validatorCache;

        static DataValidator()
        {
            _validatorCache = new Dictionary<DataTypeEnum, IDataValidator>();
        }

        public static IEnumerable<ValidationError> Validate(DataTypeProperty typeProperty, object? value)
        {
            var validator = GetValidator(typeProperty.Type);
            var validated = validator.ValidateValue(typeProperty, value);
            if (validated is not null)
            {
                yield return validated;
            }
        }

        public static IEnumerable<ValidationError> Validate(List<DataProperty> datas, List<DataTypeProperty> types)
        {
            foreach (var typeProp in types)
            {
                var validator = GetValidator(typeProp.Type);
                var dataValue = datas
                    .FirstOrDefault(dp => dp.Name == typeProp.Name);

                var validated = validator.ValidateValue(typeProp, dataValue);
                if (validated is not null)
                {
                    yield return validated;
                }
            }
        }

        private static IDataValidator GetValidator(DataTypeEnum type)
        {
            if (!_validatorCache.ContainsKey(type))
            {
                _validatorCache[type] = type switch
                {
                    DataTypeEnum.String => new StringValidator(),
                    DataTypeEnum.StringList => new StringListValidator(),
                    _ => throw new RunnerException($"Invalid DataTypeEnum: {type}")
                };
            }
            return _validatorCache[type];
        }
    }
}
