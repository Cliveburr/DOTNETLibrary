using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;

namespace Runner.Business.Datas.PropertyHandler
{
    public class NodeHandler : IPropertyHandler
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
                case DataTypeEnum.Node:
                    {
                        if (from.Value?.ObjectIdValue is not null)
                        {
                            to.Value = new DataValue
                            {
                                NodePath = from.Value.NodePath,
                                ObjectIdValue = from.Value.ObjectIdValue
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
