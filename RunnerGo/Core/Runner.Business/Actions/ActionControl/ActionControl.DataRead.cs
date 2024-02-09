using Runner.Business.Actions.Types;
using Runner.Business.Datas.Control;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public DataReader ComputeActionContextData(int actionId)
        {
            var parents = new List<ActionTypesBase>();

            var (_, actionType) = FindActionAndType(actionId);
            actionType.BuildData(new DataContext(this, parents));
            parents.Reverse();

            var reader = new DataReader();
            if (EntityRun.Input is not null)
            {
                reader.ApplyDataProperty(EntityRun.Input);
            }

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
    }
}
