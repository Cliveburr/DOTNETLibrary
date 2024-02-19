using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;

namespace Runner.Business.Datas.PropertyHandler
{
    public class ScriptVersionHandler : IPropertyHandler
    {
        public async Task Resolve(DataObject dataObject, DataHandlerItem item, IDataResolveService service, bool isRecursive)
        {
            if (item.Value?.ObjectIdValue is null || item.Value?.StringValue is null)
            {
                return;
            }

            var propResolved = await service.ResolveScriptVersionInputProperties(item.Value.ObjectIdValue.Value, item.Value.StringValue);
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
                case DataTypeEnum.ScriptVersion:
                    {
                        if (from.Value?.ObjectIdValue is not null)
                        {
                            to.Value = new DataValue
                            {
                                NodePath = from.Value.NodePath,
                                ObjectIdValue = from.Value.ObjectIdValue,
                                StringValue = from.Value.StringValue
                            };
                        }
                        break;
                    }
                default:
                    {
                        to.Type = DataTypeEnum.ScriptVersion;
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
                    Text = $"ScriptVersion property \"{item.Name}\" is required;"
                };
            }
        }
    }
}
