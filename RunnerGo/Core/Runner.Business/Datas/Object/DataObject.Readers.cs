using Runner.Business.Datas.Model;

namespace Runner.Business.Datas.Object
{
    public partial class DataObject
    {
        private DataHandlerItem? ResolveName(string dataName)
        {
            var parts = new Queue<string>(dataName.Split(["."], StringSplitOptions.RemoveEmptyEntries));
            return ResolveName(Properties, parts);
        }

        private DataHandlerItem? ResolveName(List<DataHandlerItem> items, Queue<string> parts)
        {
            if (parts.Count == 0)
            {
                return null;
            }
            
            var propName = parts.Dequeue();

            var prop = items
                .FirstOrDefault(d => d.Name == propName);
            if (prop is null)
            {
                return null;
            }
            
            if (parts.Count == 0)
            {
                return prop;
            }
            else
            {
                if (prop.Type == DataTypeEnum.Data && prop.Value?.DataExpand is not null)
                {
                    return ResolveName(prop.Value.DataExpand, parts);
                }
            }

            return null;
        }

        public string? ReadString(string dataName)
        {
            var prop = ResolveName(dataName);
            if (prop?.Value is not null && prop.Type == DataTypeEnum.String)
            {
                return prop.Value.StringValue;
            }
            return null;
        }

        public string? ReadNodePath(string dataName)
        {
            var prop = ResolveName(dataName);
            if (prop?.Value is not null && prop.Type == DataTypeEnum.Node)
            {
                return prop.Value.StringValue;
            }
            return null;
        }

        public List<string>? ReadStringList(string dataName)
        {
            var prop = ResolveName(dataName);
            if (prop?.Value is not null && prop.Type == DataTypeEnum.StringList)
            {
                return prop.Value.StringListValue;
            }
            return null;
        }
    }
}
