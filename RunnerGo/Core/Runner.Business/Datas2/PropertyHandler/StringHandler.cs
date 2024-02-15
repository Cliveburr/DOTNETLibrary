using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.Object;

namespace Runner.Business.Datas2.PropertyHandler
{
    public class StringHandler : IPropertyHandler
    {
        public Task Resolve(DataObject obj, DataHandlerItem item, IDataResolveService service)
        {
            return Task.CompletedTask;
        }

        public void Merge(DataHandlerItem to, DataHandlerItem from)
        {
            to.IsRequired = from.IsRequired;

            switch (to.Type)
            {
                case DataTypeEnum.String:
                    {
                        if (from.Value?.StringValue is not null)
                        {
                            to.Value = new DataValue
                            {
                                StringValue = from.Value.StringValue
                            };
                        }
                        break;
                    }
                default:
                    {
                        to.Type = DataTypeEnum.String;
                        to.Value = from.Value?.Clone();
                        break;
                    }
            }
        }

        public IEnumerable<ValidationError> Validate(DataHandlerItem item)
        {
            var value = item.Value?.StringValue;
            if (string.IsNullOrEmpty(value))
            {
                if (item.IsRequired)
                {
                    yield return new ValidationError
                    {
                        Item = item,
                        Text = $"String property \"{item.Name}\" is required;"
                    };
                }
            }
        }
    }
}
