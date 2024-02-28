using Runner.Business.Datas.Model;

namespace Runner.Business.Actions.DataOfTypes
{
    public static class ParentRunDataType
    {
        public static List<DataHandlerItem> Get()
        {
            return [
                new DataHandlerItem { Name = "Flow", Type = DataTypeEnum.Node, IsRequired = true, AllowModify = false }
            ];
        }
    }
}
