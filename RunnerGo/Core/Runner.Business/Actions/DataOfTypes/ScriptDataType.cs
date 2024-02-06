using Runner.Business.Datas.Model;

namespace Runner.Business.Actions.DataOfTypes
{
    public static class ScriptDataType
    {
        public static List<DataTypeProperty> Get()
        {
            return new List<DataTypeProperty>
            {
                new DataTypeProperty { Name = "AgentPoolPath", Type = DataTypeEnum.String },
                new DataTypeProperty { Name = "Tags", Type = DataTypeEnum.StringList },
                new DataTypeProperty { Name = "ScriptPath", Type = DataTypeEnum.String },
            };
        }
    }
}
