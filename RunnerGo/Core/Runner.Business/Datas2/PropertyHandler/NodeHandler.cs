using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.Object;

namespace Runner.Business.Datas2.PropertyHandler
{
    public class NodeHandler : IPropertyHandler
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
                case DataTypeEnum.Node:
                    {
                        if (from.Value?.ObjectIdValue is not null)
                        {
                            to.Value = new DataValue
                            {
                                ObjectIdValue = from.Value.ObjectIdValue,
                                NodePath = from.Value.NodePath
                            };
                        }
                        break;
                    }
                default:
                    {
                        to.Type = DataTypeEnum.Node;
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
                    Text = $"Node property \"{item.Name}\" is required;"
                };
            }
        }
    }
}
