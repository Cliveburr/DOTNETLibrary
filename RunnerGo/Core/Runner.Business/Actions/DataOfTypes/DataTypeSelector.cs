﻿using Runner.Business.Datas.Model;

namespace Runner.Business.Actions.DataOfTypes
{
    public static class DataTypeSelector
    {
        public static List<DataHandlerItem> GetFor(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Container => ContainerDataType.Get(),
                ActionType.Parallel => ParallelDataType.Get(),
                ActionType.Script => ScriptDataType.Get(),
                _ => throw new RunnerException("Invalid ActionType: " + actionType)
            };
        }
    }
}
