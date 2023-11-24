using Runner.Business.Data.Types;
using Runner.Business.Data.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Data.Validator.Types
{
    public class StringValidator : IDataValidator
    {
        public ValidationError? ValidateValue(DataTypeProperty type, DataProperty? data)
        {
            var valueStr = data?.Value as string;
            if (string.IsNullOrEmpty(valueStr))
            {
                if (type.IsRequired)
                {
                    return new ValidationError
                    {
                        Type = type,
                        Text = $"String property \"{type.Name}\" is required;"
                    };
                }
            }
            return null;
        }
    }
}
