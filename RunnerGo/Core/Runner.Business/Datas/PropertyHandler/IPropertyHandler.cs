using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;

namespace Runner.Business.Datas.PropertyHandler
{
    public interface IPropertyHandler
    {
        Task Resolve(DataObject dataObject, DataHandlerItem item, IDataResolveService service, bool isRecursive);
        void Merge(DataHandlerItem to, DataHandlerItem from);
        IEnumerable<ValidationError> Validate(DataHandlerItem item);
    }
}
