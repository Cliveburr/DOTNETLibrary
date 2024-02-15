using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.Object;
using DataObject = Runner.Business.Datas2.Object.DataObject;

namespace Runner.Business.Datas2.PropertyHandler
{
    public interface IPropertyHandler
    {
        Task Resolve(DataObject obj, DataHandlerItem item, IDataResolveService service);
        void Merge(DataHandlerItem to, DataHandlerItem from);
        IEnumerable<ValidationError> Validate(DataHandlerItem item);
    }
}
