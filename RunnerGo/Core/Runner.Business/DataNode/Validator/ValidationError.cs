using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.DataStruct.Validator
{
    public class ValidationError
    {
        public required DataTypeProperty Type { get; set; }
        public required string Text { get; set; }
    }
}
