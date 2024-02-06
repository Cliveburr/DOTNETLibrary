using Runner.Business.Datas.Model;

namespace Runner.Business.Datas.Validator.Types
{
    public class StringValidator : IDataValidator
    {
        public ValidationError? ValidateValue(DataTypeProperty type, object? value)
        {
            var valueStr = value as string;
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
