using Runner.Business.Actions.Types;
using Runner.Business.Datas.Control;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public DataReader ComputeActionContextData(int actionId)
        {
            var parents = new List<ActionTypesBase>();
            var reader = new DataReader();

            var (_, actionType) = FindActionAndType(actionId);
            actionType.BuildData(new DataContext(this, parents));
            parents.Reverse();

            foreach (var parent in parents)
            {
                var actionData = parent.Action.Data;
                if (actionData is not null)
                {
                    reader
                        .ApplyDataProperty(actionData);
                }
            }

            return reader;
        }

        //private void BuildParentIdRecursive(List<int> parents, int actionId)
        //{
        //    var action = FindAction(actionId);
        //    if (action is not null)
        //    {
        //        parents.Add(actionId);
        //        if (action.Parent is not null)
        //        {
        //            BuildParentIdRecursive(parents, action.Parent.Value);
        //        }
        //    }
        //}

        //public string? ReadDataStringNotNullRecursive(int actionId, string dataName)
        //{
        //    var action = FindAction(actionId);
        //    if (action is not null)
        //    {
        //        var value = action.Data?
        //            .FirstOrDefault(d => d.Name == dataName)?.Value as string;
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            return value;
        //        }
        //        else
        //        {
        //            if (action.Parent is not null)
        //            {
        //                return ReadDataStringNotNullRecursive(action.Parent.Value, dataName);
        //            }
        //        }
        //    }
        //    return null;
        //}

        
    }
}
