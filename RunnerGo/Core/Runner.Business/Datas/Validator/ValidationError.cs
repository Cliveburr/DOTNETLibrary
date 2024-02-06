using Runner.Business.Datas.Model;

namespace Runner.Business.Datas.Validator
{
    public class ValidationError
    {
        public required DataTypeProperty Type { get; set; }
        public required string Text { get; set; }
    }
}
