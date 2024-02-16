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

        public NodeService NodeService { get { return _nodeService; } }

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

        public async Task<(Script Script, ScriptVersion ScriptVersion)?> ReadVersionByScriptPath(string scriptPath)
        {
            var parts = scriptPath.ToLower()
                .Split("/", StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var version = parts[parts.Count - 1];
            parts.RemoveAt(parts.Count - 1);

            var scriptNode = await _nodeService.ReadLocation(new System.Collections.Queue(parts));
            if (scriptNode is null)
            {
                return null;
            }
            else
            {
                var script = await ReadByNodeId(scriptNode.NodeId);
                Assert.MustNotNull(script, $"Internal - Script not found for NodeId: " + scriptNode.NodeId);

                if (!script.Versions.Any())
                {
                    return null;
                }

                if (version == "*") //todo: improve this
                {
                    var scriptVersion = script.Versions
                        .OrderByDescending(v => v.Version)
                        .First();
                    return (script, scriptVersion);
                }
                else
                {
                    if (int.TryParse(version, out var versionInt))
                    {
                        var scriptVersion = script.Versions
                            .FirstOrDefault(v => v.Version == versionInt);
                        return scriptVersion is null ? null : (script, scriptVersion);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
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
