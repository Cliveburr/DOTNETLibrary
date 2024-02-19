using MongoDB.Bson;
using Runner.Business.Datas.Model;

namespace Runner.Business.Datas.Object
{
    public interface IDataResolveService
    {
        Task<List<DataProperty>?> ResolveScriptVersionInputProperties(ObjectId scriptId, string vesion);
        Task<List<DataProperty>?> ResolveDataProperties(ObjectId objectId);
    }
}
