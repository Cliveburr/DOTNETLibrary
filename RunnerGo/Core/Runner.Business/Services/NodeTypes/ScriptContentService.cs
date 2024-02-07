using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Security;

namespace Runner.Business.Services.NodeTypes
{
    public class ScriptContentService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public ScriptContentService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<ScriptContent?> ReadByIdStr(string scriptContentIdStr)
        {
            if (ObjectId.TryParse(scriptContentIdStr, out var scriptContentId))
            {
                return ReadById(scriptContentId);
            }
            return Task.FromResult<ScriptContent?>(null);
        }

        public Task<ScriptContent?> ReadById(ObjectId scriptContentId)
        {
            return ScriptContent
                .FirstOrDefaultAsync(sc => sc.ScriptContentId == scriptContentId);
        }

        public async Task Delete(ObjectId scriptContentId)
        {
            await ScriptContent
                .DeleteAsync(sc => sc.ScriptContentId == scriptContentId);
        }
    }
}
