using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Nodes;
using Runner.Business.Security;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Runner.Business.Services.NodeTypes
{
    public class AgentService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public AgentService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }

        public Task<Agent?> ReadByNodeId(ObjectId nodeId)
        {
            return Agent
                .FirstOrDefaultAsync(a => a.NodeId == nodeId);
        }

        public async Task<List<Agent>> ReadAgentsForPool(ObjectId agentPoolNodeId)
        {
            var agentPool = await AgentPool
                .FirstOrDefaultAsync(ap => ap.NodeId == agentPoolNodeId);
            Assert.MustNotNull(agentPool, "Internal - Missing AgentPool of NodId: " + agentPoolNodeId);

            if (agentPool.Enabled)
            {
                var query = from n in Node.AsQueryable()
                            join a in Agent.AsQueryable() on n.NodeId equals a.NodeId
                            where
                                n.ParentId == agentPoolNodeId &&
                                a.Enabled == true &&
                                a.Status == AgentStatus.Idle
                            select a;

                return await query
                    .ToListAsync();
            }
            else
            {
                return new List<Agent>();
            }
        }

        public Task<Agent?> ReadById(ObjectId agentId)
        {
            return Agent
                .FirstOrDefaultAsync(a => a.AgentId == agentId);
        }

        public Task<AgentPool?> ReadAgentPoolByNodeId(ObjectId nodeId)
        {
            return AgentPool
                .FirstOrDefaultAsync(ap => ap.NodeId == nodeId);
        }

        private string AssertMachineNameToAgentName(string machineName)
        {
            var invalidChars = new char[]
            {
                '-', '_', '/', '\\', '?'
            };

            return new string(machineName
                .ToLower()
                .Where(c => !invalidChars.Contains(c)).ToArray());
        }

        public async Task<ObjectId> Register(string agentPoolPath, string machineName, string versionName, List<string> tags)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to register agent!");

            // checar se ter permissão de Agent no agentPoolNode
            var agentPoolNode = await _nodeService.ReadLocation(agentPoolPath);
            Assert.MustNotNull(agentPoolNode, "AgentPoolNode not found! " + agentPoolPath);

            var agentPool = await ReadAgentPoolByNodeId(agentPoolNode.NodeId);
            Assert.MustNotNull(agentPool, "AgentPool not found! " + agentPoolPath);

            Assert.MustTrue(agentPool.Enabled, "AgentPool is not enabled!");

            var assertName = AssertMachineNameToAgentName(machineName);

            var agentNode = await _nodeService.ReadChildByName(agentPoolNode.NodeId, assertName);
            if (agentNode == null)
            {
                agentNode = new Node
                {
                    Type = NodeType.Agent,
                    Name = assertName,
                    ParentId = agentPoolNode.NodeId,
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                };
                await Node
                    .InsertAsync(agentNode);

                await _nodeService.UpdateUtc(agentPoolNode.NodeId);

                var agent = new Agent
                {
                    NodeId = agentNode.NodeId,
                    MachineName = machineName,
                    VersionName = versionName,
                    RegistredTags = tags,
                    Status = AgentStatus.Idle,
                    Enabled = true
                };
                await Agent
                    .InsertAsync(agent);

                return agent.AgentId;
            }
            else
            {
                var agent = await ReadByNodeId(agentNode.NodeId);
                Assert.MustNotNull(agent, "Internal error! CODE Agent");

                if (agentNode.Name != assertName)
                {
                    var nodeUpdate = Builders<Node>.Update
                        .Set(n => n.Name, assertName)
                        .Set(n => n.UpdatedUtc, DateTime.UtcNow);
                    await Node
                        .UpdateAsync(n => n.NodeId == agentNode.NodeId, nodeUpdate);
                }
                else
                {
                    await _nodeService.UpdateUtc(agentNode.NodeId);
                }

                await _nodeService.UpdateUtc(agentPoolNode.NodeId);

                var agentUpdate = Builders<Agent>.Update
                    .Set(a => a.MachineName, machineName)
                    .Set(a => a.VersionName, versionName)
                    .Set(a => a.RegistredTags, tags)
                    .Set(a => a.Status, AgentStatus.Idle);
                await Agent
                    .UpdateAsync(a => a.AgentId == agent.AgentId, agentUpdate);

                return agent.AgentId;
            }
        }

        public async Task UpdateOffline(ObjectId agentId)
        {
            var agent = await ReadById(agentId);
            Assert.MustNotNull(agent, "Internal error! CODE Agent");

            if (agent.Status != AgentStatus.Offline)
            {
                await _nodeService.UpdateUtc(agent.NodeId);

                var agentUpdate = Builders<Agent>.Update
                    .Set(a => a.Status, AgentStatus.Offline);
                await Agent
                    .UpdateAsync(a => a.AgentId == agent.AgentId, agentUpdate);
            }
        }

        public async Task DeleteByNode(Node node)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            //TODO: checar status ou job?

            await Agent
                .DeleteAsync(a => a.NodeId == node.NodeId);

            if (node.ParentId.HasValue)
            {
                await _nodeService.UpdateUtc(node.ParentId.Value);
            }

            await Node
                .DeleteAsync(n => n.NodeId == node.NodeId);
        }
    }
}
