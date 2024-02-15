using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.Object;

namespace Runner.Business.Datas2.PropertyHandler
{
    public class InheritHandler : IPropertyHandler
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
                var datas = properties.Select(p => new DataHandlerItem(p)).ToList();
                obj.Merge(datas);

                foreach (var data in datas)
                {
                    var handler = PropertyHandlerSelector.Get(data.Type);
                    await handler.Resolve(obj, data, service);
                }
            }
        }

        public void Merge(DataHandlerItem to, DataHandlerItem from)
        {
            to.IsRequired = from.IsRequired;

            switch (to.Type)
            {
                case DataTypeEnum.Inherit:
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
                        to.Type = DataTypeEnum.Inherit;
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
                    Text = $"Inherit property \"{item.Name}\" is required;"
                };
            }
        }
    }
}
