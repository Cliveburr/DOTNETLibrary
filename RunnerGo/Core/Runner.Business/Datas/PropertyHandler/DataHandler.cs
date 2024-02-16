using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;

namespace Runner.Business.Datas.PropertyHandler
{
    public class DataHandler : IPropertyHandler
    {
        public async Task Resolve(DataObject dataObject, DataHandlerItem item, IDataResolveService service, bool isRecursive)
        {
            if (!isRecursive || item.Value?.ObjectIdValue is null)
            {
                return;
            }

            var propResolved = await service.ResolveDataProperties(item.Value.ObjectIdValue.Value);
            if (propResolved is null)
            {
                return;
            }

            var dataObj = new DataObject(propResolved, service);
            await dataObj.Resolve(true);
            item.Value.DataExpand = dataObj.Properties;
        }

        public void Merge(DataHandlerItem to, DataHandlerItem from)
        {
            if (!to.IsRequired)
            {
                to.IsRequired = from.IsRequired;
            }

            switch (to.Type)
            {
                case DataTypeEnum.Data:
                    {
                        if (to.Value?.DataExpand is not null && from.Value?.DataExpand is not null)
                        {
                            var dataObj = new DataObject(to.Value.DataExpand);
                            dataObj.Merge(from.Value.DataExpand);
                            to.Value.DataExpand = dataObj.Properties;
                        }
                        //if (from.Value?.ObjectIdValue is not null)
                        //{
                        //    to.Value = new DataValue
                        //    {
                        //        ObjectIdValue = from.Value.ObjectIdValue
                        //    };
                        //}
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
            if (item.Value?.DataExpand is not null)
            {
                foreach (var data in  item.Value.DataExpand)
                {
                    var handler = PropertyHandlerSelector.Get(data.Type);
                    handler.Validate(data);
                }
            }
        }
    }
}
