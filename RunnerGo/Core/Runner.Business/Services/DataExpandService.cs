using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;
using Runner.Business.Entities.Nodes;
using Runner.Business.Services.NodeTypes;

namespace Runner.Business.Services
{
    public class DataExpandService : DataServiceBase, IDataResolveService
    {
        private readonly DataService _dataService;
        private readonly NodeService _nodeService;
        private readonly ScriptService _scriptService;

        public DataExpandService(Database database, DataService dataService, NodeService nodeService, ScriptService scriptService)
            : base(database)
        {
            _dataService = dataService;
            _nodeService = nodeService;
            _scriptService = scriptService;
        }

        public Task<Node?> ReadNodeLocation(string path)
        {
            return _nodeService.ReadLocation(path);
        }

        public async Task<List<DataProperty>?> ResolveScriptVersionInputProperties(ObjectId scriptId, string version)
        {
            var sts = await _scriptService.ReadVersionByScriptPath(scriptId, version);
            return sts?.ScriptVersion.Input;
        }

        public async Task<List<DataProperty>?> ResolveDataProperties(ObjectId objectId)
        {
            var data = await _dataService.ReadByNodeId(objectId);
            return data?.Properties;
        }

        public Task<string?> ResolveNodePath(ObjectId objectId)
        {
            return _nodeService.BuildPath(objectId);
        }
    }
}
