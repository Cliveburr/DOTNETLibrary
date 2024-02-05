using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {

        public List<DataProperty>? ReadActionData(int actionId)
        {
            return FindAction(actionId)?.Data;
        }

        public string? ReadDataStringNotNullRecursive(int actionId, string dataName)
        {
            var action = FindAction(actionId);
            if (action is not null)
            {
                var value = action.Data?
                    .FirstOrDefault(d => d.Name == dataName)?.Value as string;
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
                else
                {
                    if (action.Parent is not null)
                    {
                        return ReadDataStringNotNullRecursive(action.Parent.Value, dataName);
                    }
                }
            }
            return null;
        }

        public string? ReadDataString(int actionId, string dataName)
        {
            return FindAction(actionId)?.Data?
                .FirstOrDefault(d => d.Name == dataName)?.Value as string;
        }

        public List<string>? ReadDataTagString(int actionId, string dataName)
        {
            return FindAction(actionId)?.Data?
                .FirstOrDefault(d => d.Name == dataName)?.Value as List<string>;
        }
    }
}
