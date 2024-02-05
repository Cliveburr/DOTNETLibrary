using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.DataNode.Validator.Types
{
    public interface IDataValidator
    {
        ValidationError? ValidateValue(DataTypeProperty type, object? data);
    }
}
