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
            var errors = new List<ValidationError>();

            foreach (var typeProp in type.Properties)
            {
                var validator = GetValidator(typeProp.Type);
                var dataValue = data.Properties
                    .FirstOrDefault(dp => dp.Name == typeProp.Name);

                var validated = validator.ValidateValue(typeProp, dataValue);
                if (validated is not null)
                {
                    errors.Add(validated);
                }
            }

            return errors;
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
