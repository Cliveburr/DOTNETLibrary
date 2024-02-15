using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.Object;

namespace Runner.Business.Datas2.PropertyHandler
{
    public class DataHandler : IPropertyHandler
    {
        public async Task Resolve(DataObject obj, DataHandlerItem item, IDataResolveService service)
        {
            if (item.Value?.ObjectIdValue is null)
            {
                return;
            }

            var properties = await service.ResolveDataProperties(item.Value.ObjectIdValue.Value);
            if (properties is not null)
            {
                var dataObj = new DataObject(properties, service);
                await dataObj.Resolve();
                item.Value.DataExpand = dataObj.ToItems();
            }
        }

        public void Merge(DataHandlerItem to, DataHandlerItem from)
        {
            to.IsRequired = from.IsRequired;

            switch (to.Type)
            {
                case DataTypeEnum.Data:
                    {
                        if (from.Value?.ObjectIdValue is not null)
                        {
                            to.Value = new DataValue
                            {
                                ObjectIdValue = from.Value.ObjectIdValue
                            };
                        }
                        break;
                    }
                default:
                    {
                        to.Type = DataTypeEnum.Data;
                        to.Value = from.Value?.Clone();
                        break;
                    }
            }
        }

        public IEnumerable<ValidationError> Validate(DataHandlerItem item)
        {
            var value = item.Value?.ObjectIdValue;
            if (value is null && item.IsRequired)
            {
                yield return new ValidationError
                {
                    Item = item,
                    Text = $"Data property \"{item.Name}\" is required;"
                };
            }
        }
    }
}
