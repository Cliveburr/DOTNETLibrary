using Runner.Business.Datas.Model;

namespace Runner.Business.Actions.DataOfTypes
{
    public static class ParallelDataType
    {
        public static List<DataHandlerItem> Get()
        {
            return [
                new DataHandlerItem { Name = "AgentPool", Type = DataTypeEnum.Node, AllowModify = false },
                new DataHandlerItem { Name = "Tags", Type = DataTypeEnum.StringList, AllowModify = false }
            ];
        }
    }
}
