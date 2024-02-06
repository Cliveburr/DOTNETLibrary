
namespace Runner.Business.Datas.Model
{
    public class DataProperty
    {
        public required string Name { get; set; }
        public required DataTypeEnum Type { get; set; }
        public object? Value { get; set; }
    }
}
