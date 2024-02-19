using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;

namespace Runner.Business.Datas.PropertyHandler
{
    public class StringHandler : IPropertyHandler
    {
        public Task Resolve(DataObject dataObject, DataHandlerItem item, IDataResolveService service, bool isRecursive)
        {
            return Task.CompletedTask;
        }

        public void Merge(DataHandlerItem to, DataHandlerItem from)
        {
            if (!to.IsRequired)
            {
                to.IsRequired = from.IsRequired;
            }
            if (to.AllowModify)
            {
                to.AllowModify = from.AllowModify;
            }

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
