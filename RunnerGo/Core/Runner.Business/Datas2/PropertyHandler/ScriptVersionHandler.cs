using Microsoft.AspNetCore.Http.Features;
using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.Object;

namespace Runner.Business.Datas2.PropertyHandler
{
    public class ScriptVersionHandler : IPropertyHandler
    {
        public async Task Resolve(DataObject obj, DataHandlerItem item, IDataResolveService service)
        {
            if (item.Value?.ObjectIdValue is null || item.Value?.IntValue is null)
            {
                return;
            }

            var properties = await service.ResolveScriptVersionInputProperties(item.Value.ObjectIdValue.Value, item.Value.IntValue.Value);
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
                case DataTypeEnum.ScriptVersion:
                    {
                        if (from.Value?.ObjectIdValue is not null)
                        {
                            to.Value = new DataValue
                            {
                                ObjectIdValue = from.Value.ObjectIdValue,
                                IntValue = from.Value.IntValue
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
