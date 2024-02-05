using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.Actions.DataOfTypes
{
    public static class ContainerDataType
    {
        public static List<DataTypeProperty> Get()
        {
            return new List<DataTypeProperty>
            {
                new DataTypeProperty { Name = "AgentPoolPath", Type = DataTypeEnum.String },
                new DataTypeProperty { Name = "Tags", Type = DataTypeEnum.StringList }
            };
        }
    }
}
