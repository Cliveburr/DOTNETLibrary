using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Security;

namespace Runner.Business.Services.NodeTypes
{
    public class ScriptService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public ScriptService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<Script?> ReadById(ObjectId scriptId)
        {
            return Script
                .FirstOrDefaultAsync(sp => sp.ScriptId == scriptId);
        }

        public Task<Script?> ReadByNodeId(ObjectId nodeId)
        {
            return Script
                .FirstOrDefaultAsync(sp => sp.NodeId == nodeId);
        }

        public async Task DeleteByNode(Node node)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            await Script
                .DeleteAsync(a => a.NodeId == node.NodeId);

            if (node.ParentId.HasValue)
            {
                await _nodeService.UpdateUtc(node.ParentId.Value);
            }

            await Node
                .DeleteAsync(n => n.NodeId == node.NodeId);

            //TODO: check if ScriptContent can be deleted?
        }

        public async Task DeleteVersion(Script script, ScriptVersion scriptVersion)
        {
            script.Versions.Remove(scriptVersion);

            await Script
                .ReplaceAsync(s => s.ScriptId == script.ScriptId, script);

            //TODO: check if ScriptContent can be deleted?
        }
    }
}
