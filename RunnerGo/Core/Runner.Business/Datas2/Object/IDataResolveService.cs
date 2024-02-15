using MongoDB.Bson;
using Runner.Business.Datas2.Model;

namespace Runner.Business.Datas2.Object
{
    public interface IDataResolveService
    {
        Task<List<DataProperty>?> ResolveScriptVersionInputProperties(ObjectId scriptId, int vesion);
        Task<List<DataProperty>?> ResolveDataProperties(ObjectId objectId);
        Task<string?> ResolveNodePath(ObjectId objectId);
    }
}
