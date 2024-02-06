using Runner.Business.Datas.Model;

namespace Runner.Business.Datas.Validator.Types
{
    public interface IDataValidator
    {
        ValidationError? ValidateValue(DataTypeProperty type, object? data);
    }
}
