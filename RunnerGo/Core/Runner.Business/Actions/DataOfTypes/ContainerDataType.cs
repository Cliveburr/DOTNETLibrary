using Runner.Business.Datas.Model;

namespace Runner.Business.Actions.DataOfTypes
{
    public static class ContainerDataType
    {
        public static List<DataHandlerItem> Get()
        {
            return [
                new DataHandlerItem { Name = "AgentPool", Type = DataTypeEnum.Node, AllowDelete = false },
                new DataHandlerItem { Name = "Tags", Type = DataTypeEnum.StringList, AllowDelete = false }
            ];
        }
    }
}
