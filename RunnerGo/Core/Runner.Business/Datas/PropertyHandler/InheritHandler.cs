using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;

namespace Runner.Business.Datas.PropertyHandler
{
    public class InheritHandler : IPropertyHandler
    {
        public async Task Resolve(DataObject dataObject, DataHandlerItem item, IDataResolveService service, bool isRecursive)
        {
            if (item.Value?.ObjectIdValue is null)
            {
                return;
            }

            var propResolved = await service.ResolveDataProperties(item.Value.ObjectIdValue.Value);
            if (propResolved is null)
            {
                return;
            }

            var dataResolved = propResolved
                .Select(p => new DataHandlerItem(p))
                .ToList();

            dataResolved.ForEach(p =>
            {
                p.AllowModify = false;
            });

            var cleanDataobjectDatas = new DataObject(service);
            foreach (var data in dataResolved)
            {
                data.AllowModify = false;

                var handler = PropertyHandlerSelector.Get(data.Type);
                await handler.Resolve(cleanDataobjectDatas, data, service, isRecursive);
            }

            //var resolvedObj = new DataObject(dataObject.Properties, service);
            dataObject.Merge(cleanDataobjectDatas.Properties);
            dataObject.Merge(dataResolved);
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
