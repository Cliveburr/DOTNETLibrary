using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.DataNode.Validator.Types
{
    public class StringListValidator : IDataValidator
    {
        public ValidationError? ValidateValue(DataTypeProperty type, object? value)
        {
            var valueListStr = value as List<string>;
            if (valueListStr is not null)
            {
                if (!valueListStr.Any() && type.IsRequired)
                {
                    return new ValidationError
                    {
                        Type = type,
                        Text = $"String property \"{type.Name}\" is required;"
                    };
                }
            }
            else
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
