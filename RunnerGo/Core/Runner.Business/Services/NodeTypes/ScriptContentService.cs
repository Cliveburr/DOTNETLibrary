using MongoDB.Bson;
using Runner.Business.DataAccess;
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

        public Task<ScriptContent?> ReadById(ObjectId scriptContentId)
        {
            return ScriptContent
                .FirstOrDefaultAsync(sc => sc.ScriptContentId == scriptContentId);
        }
    }
}
