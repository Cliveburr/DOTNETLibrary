using Runner.Business.Actions.Types;
using Runner.Business.Datas.Object;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public DataObject ComputeActionContextData(int actionId, IDataResolveService service)
        {
            var parents = new List<ActionTypesBase>();

            var (_, actionType) = FindActionAndType(actionId);
            actionType.BuildData(new DataContext(this, parents));
            parents.Reverse();

            var reader = new DataObject(service);
            if (EntityRun.Input is not null)
            {
                reader.Merge(EntityRun.Input);
            }

            foreach (var parent in parents)
            {
                var actionData = parent.Action.Data;
                if (actionData is not null)
                {
                    reader.Merge(actionData);
                }
            }

            return reader;
        }
    }
}
