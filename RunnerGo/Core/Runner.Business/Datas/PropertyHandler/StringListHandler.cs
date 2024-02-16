using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;

namespace Runner.Business.Datas.PropertyHandler
{
    public class StringListHandler : IPropertyHandler
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

            switch (to.Type)
            {
                case DataTypeEnum.StringList:
                    {
                        if (from.Value?.StringListValue is not null)
                        {
                            to.Value = new DataValue
                            {
                                StringListValue = from.Value.StringListValue
                            };
                        }
                        break;
                    }
                default:
                    {
                        to.Type = DataTypeEnum.StringList;
                        to.Value = from.Value?.Clone();
                        break;
                    }
            }
        }

        public IEnumerable<ValidationError> Validate(DataHandlerItem item)
        {
            var value = item.Value?.StringListValue;
            if (value is not null)
            {
                if (!value.Any() && item.IsRequired)
                {
                    yield return new ValidationError
                    {
                        Item = item,
                        Text = $"StringList property \"{item.Name}\" is required;"
                    };
                }
            }
            else
            {
                if (item.IsRequired)
                {
                    yield return new ValidationError
                    {
                        Item = item,
                        Text = $"StringList property \"{item.Name}\" is required;"
                    };
                }
            }
        }
    }
}
