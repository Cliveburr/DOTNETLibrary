using Runner.Business.Data.Types;
using Runner.Business.Data.Validator.Types;
using Runner.Business.Data.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Data.Validator
{
    public static class DataValidator
    {
        private static Dictionary<DataTypeEnum, IDataValidator> _validatorCache;

        static DataValidator()
        {
            _validatorCache = new Dictionary<DataTypeEnum, IDataValidator>();
        }

        public static List<ValidationError> Validate(DataStruct data, DataTypeStruct type)
        {
            return Validate(data.Properties, type.Properties);
        }

        public static List<ValidationError> Validate(List<DataProperty> datas, List<DataTypeProperty> types)
        {
            var errors = new List<ValidationError>();

            foreach (var typeProp in types)
            {
                var validator = GetValidator(typeProp.Type);
                var dataValue = datas
                    .FirstOrDefault(dp => dp.Name == typeProp.Name);

                var validated = validator.ValidateValue(typeProp, dataValue);
                if (validated is not null)
                {
                    errors.Add(validated);
                }
            }

            return errors;
        }

        public static List<ValidationError> Validate(List<DataWithType> datas)
        {
            var dataProps = datas
                .Select(d => d.Data)
                .ToList();

            var types = datas
                .Select(d => d.Type)
                .ToList();

            return Validate(dataProps, types);
        }

        private static IDataValidator GetValidator(DataTypeEnum type)
        {
            if (!_validatorCache.ContainsKey(type))
            {
                _validatorCache[type] = type switch
                {
                    DataTypeEnum.String => new StringValidator(),
                    _ => throw new RunnerException($"Invalid DataTypeEnum: {type}")
                };
            }
            return _validatorCache[type];
        }
    }
}
