﻿using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using Runner.Business.Entities;
using Runner.Business.Entities.Agent;
using Runner.Business.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public Task<List<App>> ReadApps()
        {
            Assert.MustNotNull(_userLogged.User, "Not logged!");
            return Node
                .ToListAsync<App>(a => a.Parent == null && a.Type == NodeType.App && a.OwnerId == _userLogged.User.Id);
        }

        public Task<List<NodeBase>> ReadChilds(NodeBase node)
        {
            return Node
                .ToListAsync(a => a.Parent == node.Id);
        }

        public Task<List<T>> ReadChilds<T>(NodeBase node) where T : NodeBase
        {
            return Node
                .ToListAsync<T>(a => a.Parent == node.Id);
        }

        public Task<NodeBase?> ReadChild(NodeBase node, string name)
        {
            return Node
                .FirstOrDefaultAsync(a => a.Parent == node.Id && a.Name == name);
        }

        public Task<T?> ReadChild<T>(NodeBase node, string name) where T: NodeBase
        {
            return Node
                .FirstOrDefaultAsync<T>(a => a.Parent == node.Id && a.Name == name);
        }

        public Task<NodeBase?> ReadLocation(string path)
        {
            var parts = new System.Collections.Queue(path.ToLower()
                .Split("/", StringSplitOptions.RemoveEmptyEntries));
            return ReadLocation(parts);
        }

        public async Task<NodeBase?> ReadLocation(System.Collections.Queue parts)
        {
            if (parts.Count == 0)
            {
                return null;
            }
            return await ReadLocation_Recursive(parts, null);
        }

        private async Task<NodeBase?> ReadLocation_Recursive(System.Collections.Queue parts, NodeBase? parent)
        {
            var name = (string)parts.Dequeue()!;
            var found = await Node
                .FirstOrDefaultAsync(n => n.Name == name && n.Parent == (parent == null ? null : parent.Id));
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
                    return await ReadLocation_Recursive(parts, found);
                }
            }
        }

        public Task<NodeBase?> ReadById(ObjectId nodeId)
        {
            return Node
                .FirstOrDefaultAsync(a => a.Id == nodeId);
        }

        public Task CreateApp(string name)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            return Create(new App
            {
                Name = name,
                Type = NodeType.App,
                OwnerId = _userLogged.User.Id,
                Parent = null
            });
        }

        public Task CreateFolder(string name, NodeBase parent)
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

        public Task CreateAgentPool(string name, NodeBase parent)
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

        public Task CreateFlow(string name, NodeBase parent)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            return Create(new Flow
            {
                Name = name,
                Type = NodeType.Flow,
                Parent = parent.Id,
                AgentPool = "",
                Root = new FlowActionContainer
                {
                    Label = "Root",
                    Actions = new List<FlowAction>
                    {
                        new FlowAction
                        {
                            Label = "Action"
                        }
                    }
                }
            });
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

            var agent = await ReadChild<Agent>(agentPoolNode, machineName);
            if (agent == null)
            {
                agent = new Agent
                {
                    Parent = agentPoolNode.Id,
                    Name = AssertMachineNameToAgentName(machineName),
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
                agent.Name = AssertMachineNameToAgentName(machineName);
                agent.MachineName = machineName;
                agent.RegistredTags = tags;
                agent.Status = AgentStatus.Idle;
                agent.HeartBeat = DateTime.Now;
                await Node.SaveAsync(agent);
            }

            return agent;
        }

        private async Task Create(NodeBase node)
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
    }
}