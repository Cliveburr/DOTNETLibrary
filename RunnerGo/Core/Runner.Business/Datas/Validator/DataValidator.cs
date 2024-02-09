using Runner.Business.Datas.Model;
using Runner.Business.Datas.Validator.Types;

namespace Runner.Business.Datas.Validator
{
    public static class DataValidator
    {
        private static Dictionary<DataTypeEnum, IDataValidator> _validatorCache;

        static DataValidator()
        {
            _validatorCache = new Dictionary<DataTypeEnum, IDataValidator>();
        }

        public static IEnumerable<ValidationError> Validate(List<DataFullProperty> dataFullProperties)
        {
            foreach (var dataFullProperty in dataFullProperties)
            {
                var validated = Validate(dataFullProperty);
                if (validated is not null)
                {
                    yield return validated;
                }
            }
        }

        public static ValidationError? Validate(DataTypeProperty typeProperty, object? value)
        {
            var validator = GetValidator(typeProperty.Type);
            return validator.ValidateValue(typeProperty, value);
        }

        public static ValidationError? Validate(DataFullProperty dataFullProperty)
        {
            var validator = GetValidator(dataFullProperty.Type);
            return validator.ValidateValue(dataFullProperty.ToDataTypeProperty(), dataFullProperty.Value);
        }

        public static IDataValidator GetValidator(DataTypeEnum type)
        {
            if (!_validatorCache.ContainsKey(type))
            {
                _validatorCache[type] = type switch
                {
                    DataTypeEnum.String => new StringValidator(),
                    DataTypeEnum.StringList => new StringListValidator(),
                    DataTypeEnum.NodePath => new NodePathValidator(),
                    DataTypeEnum.DataReference => new DataReferenceValidator(),
                    _ => throw new RunnerException($"Invalid DataTypeEnum: {type}")
                };
            }
            return _validatorCache[type];
        }
    }
}
