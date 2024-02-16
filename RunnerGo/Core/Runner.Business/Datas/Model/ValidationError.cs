
namespace Runner.Business.Datas.Model
{
    public class ValidationError
    {
        public required DataHandlerItem Item { get; set; }
        public required string Text { get; set; }
    }
}
