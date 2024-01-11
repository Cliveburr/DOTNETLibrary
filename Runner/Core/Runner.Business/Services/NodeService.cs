using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.Authentication;
using Runner.Business.Data.Value;
using Runner.Business.DataAccess;
using Runner.Business.Entities;
using Runner.Business.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Runner.Business.Entities.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace Runner.Business.Services
{
    public class NodeService : ServiceBase
    {
        private UserLogged _userLogged;

        public NodeService(Database database, UserLogged userLogged)
            : base(database)
        {
            _userLogged = userLogged;
        }

        public Task<Node?> ReadLocation(string path)
        {
            var parts = new System.Collections.Queue(path
                .Split("/", StringSplitOptions.RemoveEmptyEntries));
            return ReadLocation(parts);
        }

        public async Task<Node?> ReadLocation(System.Collections.Queue parts)
        {
            if (parts.Count == 0)
            {
                return null;
            }
            return await ReadLocation_Recursive(parts, null);
        }

        private async Task<Node?> ReadLocation_Recursive(System.Collections.Queue parts, ObjectId? parentId)
        {
            var name = parts.Dequeue() as string;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var found = await Node
                   .FirstOrDefaultAsync(n => n.Name == name.ToLower() && n.ParentId == parentId);
            //Node? found = null;
            //if (parent == null)
            //{
            //    found = await Node
            //        .FirstOrDefaultAsync(n => n.Name == name && n.Parent == null);
            //}
            //else
            //{
            //    switch (parent.Type)
            //    {
            //        case NodeType.Flow:
            //            if (ObjectId.TryParse(name, out ObjectId runId))
            //            {
            //                found = await Node
            //                    .FirstOrDefaultAsync(n => n.NodeId == runId && n.Parent == parent.Id);
            //            }
            //            break;
            //        default:
            //            found = await Node
            //                .FirstOrDefaultAsync(n => n.Name == name && n.Parent == parent.Id);
            //            break;
            //    }
            //}
            if (found == null)
            {
                return null;
            }
            else
            {
                if (parts.Count == 0)
                {
                    return found;
                }
                else
                {
                    return await ReadLocation_Recursive(parts, found.NodeId);
                }
            }
        }

        public Task<List<Node>> ReadChilds(Node node)
        {
            return Node
                .ToListAsync(a => a.ParentId == node.NodeId);
        }












        public Task<List<T>> ReadChilds<T>(Node node) where T : Node
        {
            return Node
                .ToListAsync<T>(a => a.Parent == node.Id);
        }

        public Task<Node?> ReadChild(Node node, string name)
        {
            return Node
                .FirstOrDefaultAsync(a => a.Parent == node.Id && a.Name == name);
        }

        public Task<T?> ReadChild<T>(Node node, string name) where T: Node
        {
            return Node
                .FirstOrDefaultAsync<T>(a => a.Parent == node.Id && a.Name == name);
        }

        public Task<Node?> ReadById(ObjectId nodeId)
        {
            return Node
                .FirstOrDefaultAsync(a => a.Id == nodeId);
        }



        public Task CreateFolder(string name, Node parent)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            return Create(new Folder
            {
                Name = name,
                Type = NodeType.Folder,
                Parent = parent.Id
            });
        }

        public Task CreateAgentPool(string name, Node parent)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            return Create(new AgentPool
            {
                Name = name,
                Type = NodeType.AgentPool,
                Parent = parent.Id,
                Enabled = true
            });
        }

        public Task CreateFlow(string name, Node parent)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            return Create(new Flow
            {
                Name = name,
                Type = NodeType.Flow,
                Parent = parent.Id,
                Root = new FlowAction
                {
                    Label = "Root",
                    Type = Actions.ActionType.Script,
                    Childs = new List<FlowAction>
                    {
                        new FlowAction
                        {
                            Label = "Action"
                        }
                    }
                }
            });
        }

        public Task UpdateFlow(Flow flow)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            return Node.SaveAsync(flow);
        }

        public Task CreateDataType(string name, Node parent)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create data type!");

            // checar se ter permissão de Create no parent

            return Create(new DataType
            {
                Name = name,
                Type = NodeType.DataType,
                Parent = parent.Id,
                Struct = new Data.Types.DataTypeStruct
                {
                    Properties = new List<Data.Types.DataTypeProperty>
                    {
                    }
                }
            });
        }

        public Task UpdateDataType(DataType datatype)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            return Node.SaveAsync(datatype);
        }

        public Task CreateData(string name, Node parent)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create data!");

            // checar se ter permissão de Create no parent

            return Create(new Entities.Node.Data
            {
                Name = name,
                Type = NodeType.Data,
                Parent = parent.Id,
                Struct = new Data.Value.DataStruct
                {
                    Properties = new List<DataProperty>
                    {
                    }
                }
            });
        }

        public Task UpdateData(Entities.Node.Data data)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            return Node.SaveAsync(data);
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

        public async Task<Agent> RegisterAgent(string agentPool, string machineName, List<string> tags)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to register agent!");

            var agentPoolNode = await ReadLocation(agentPool) as AgentPool;
            Assert.MustNotNull(agentPoolNode, "AgentPool not found! " + agentPool);

            // checar se ter permissão de Agent no agentPoolNode

            Assert.MustTrue(agentPoolNode.Enabled, "AgentPool is not enabled!");

            var assertName = AssertMachineNameToAgentName(machineName);

            var agent = await ReadChild<Agent>(agentPoolNode, assertName);
            if (agent == null)
            {
                agent = new Agent
                {
                    Parent = agentPoolNode.Id,
                    Name = assertName,
                    MachineName = machineName,
                    Type = NodeType.Agent,
                    RegistredTags = tags,
                    Status = AgentStatus.Idle,
                    HeartBeat = DateTime.Now,
                    Enabled = true
                };
                await Node.CreateAsync(agent);
            }
            else
            {
                agent.RegistredTags = tags;
                agent.Status = AgentStatus.Idle;
                agent.HeartBeat = DateTime.Now;
                await Node.SaveAsync(agent);
            }

            return agent;
        }

        private async Task Create(Node node)
        {
            Assert.Strings.NotNullAndRange(node.Name, 3, 20, "Name size invalid 3 < name < 20");

            var has = await Node
                .AnyAsync(n => n.Parent == node.Parent && n.Name.ToLower() == node.Name.ToLower());
            Assert.MustFalse(has, "Name taken!");

            await Node.CreateAsync(node);
        }

        public async Task AgentHeartbeat(ObjectId id)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to register agent!");

            // checar se ter permissão de Agent

            var agent = await Node
                .FirstOrDefaultAsync<Agent>(n => n.Id == id);
            Assert.MustNotNull(agent, "Agent not found! " + id);

            var update = Builders<Agent>.Update
                .Set(a => a.HeartBeat, DateTime.Now);

            await Node.UpdateAsync(agent, update);
        }

        public async Task UpdateAgentOffline(ObjectId id)
        {
            var agent = await Node
                .FirstOrDefaultAsync<Agent>(n => n.Id == id);
            if (agent != null)
            {
                var update = Builders<Agent>.Update
                    .Set(a => a.Status, AgentStatus.Offline);

                await Node.UpdateAsync(agent, update);
            }
        }

        public async Task Delete(Node node)
        {
            //using (var session = await Node.StartSessionAsync())
            //{
            //    try
            //    {
            //        session.StartTransaction();
                    await DeleteRecursive(node);
            //        await session.CommitTransactionAsync();
            //    }
            //    catch
            //    {
            //        await session.AbortTransactionAsync();
            //        throw;
            //    }
            //}
        }

        private async Task DeleteRecursive(Node node)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to delete node!");

            // checar se ter permissão para deletar
            // checa condições especiais dependendo do tipo do node

            var childs = await ReadChilds(node);
            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    await DeleteRecursive(child);
                }
            }

            await Node.DeleteAsync(node);
        }
    }
}
