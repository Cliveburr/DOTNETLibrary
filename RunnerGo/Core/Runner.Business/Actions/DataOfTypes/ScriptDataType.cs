﻿using Runner.Business.Datas.Model;

namespace Runner.Business.Actions.DataOfTypes
{
    public static class ScriptDataType
    {
        public static List<DataTypeProperty> Get()
        {
            return new List<DataTypeProperty>
            {
                new DataTypeProperty { Name = "ScriptPath", Type = DataTypeEnum.NodePath, IsRequired = true },
                new DataTypeProperty { Name = "AgentPoolPath", Type = DataTypeEnum.NodePath },
                new DataTypeProperty { Name = "Tags", Type = DataTypeEnum.StringList },
            };
        }
    }
}