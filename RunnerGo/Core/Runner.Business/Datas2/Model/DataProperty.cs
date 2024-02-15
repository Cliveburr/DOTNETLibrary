
namespace Runner.Business.Datas2.Model
{
    public class DataProperty
    {
        public required string Name { get; set; }
        public required DataTypeEnum Type { get; set; }
        public bool IsRequired { get; set; }
        public DataValue? Value { get; set; }
    }
}
