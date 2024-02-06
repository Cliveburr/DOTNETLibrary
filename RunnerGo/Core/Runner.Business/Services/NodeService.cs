using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Runner.Business.Services
{
    public class NodeService : DataServiceBase
    {
        public NodeService(Database database)
            : base(database)
        {
        }

        public Task<Node?> ReadByNodeId(ObjectId nodeId)
        {
            return Node
                .FirstOrDefaultAsync(n => n.NodeId == nodeId);
        }

        public Task<Node?> ReadLocation(string path)
        {
            var parts = new System.Collections.Queue(path.ToLower()
                .Split("/", StringSplitOptions.RemoveEmptyEntries));
            if (parts.Count == 0)
            {
                return Task.FromResult<Node?>(null);
            }
            return ReadLocation_Recursive(parts, null);
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
                   .FirstOrDefaultAsync(n => n.Name.ToLower() == name && n.ParentId == parentId);
            if (found is not null && found.Type == NodeType.Flow)
            {
                return found;
            }
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

        public Task<Node?> ReadByNameAndParent(string? name, ObjectId? parentId)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Task.FromResult<Node?>(null);
            }
            else
            {
                return Node
                    .FirstOrDefaultAsync(n => n.Name.ToLower().Equals(name.ToLower())
                        && n.ParentId == parentId);
            }
        }

        public Task<List<Node>> ReadChilds(ObjectId? parentId)
        {
            return Node
                .ToListAsync(n => n.ParentId == parentId);
        }

        public Task<Node?> ReadChildByName(ObjectId parentId, string name)
        {
            return Node
                .FirstOrDefaultAsync(n => n.ParentId == parentId && n.Name == name);
        }

        public Task<bool> HasChilds(ObjectId? parentId)
        {
            return Node
                .AnyAsync(n => n.ParentId == parentId);
        }

        public void ValidateName([NotNull] string? name)
        {
            Assert.Strings.MustNotNullOrEmpty(name, "Invalid name of node!");

            Assert.Number.InRange(name.Length, 3, 30, "Invalid name of node!");

            var validateRegex = new Regex($"^[\\w\\-]*$", RegexOptions.IgnoreCase);
            if (!validateRegex.IsMatch(name))
            {
                throw new RunnerException("Invalid name of node!");
            }
        }

        public async Task UpdateUtc(ObjectId nodeId)
        {
            var nodeUpdate = Builders<Node>.Update
                .Set(n => n.UpdatedUtc, DateTime.UtcNow);
            await Node
                .UpdateAsync(n => n.NodeId == nodeId, nodeUpdate);
        }

        public async Task UpdateName(ObjectId nodeId, string name)
        {
            var has = await ReadByNameAndParent(name, nodeId);
            Assert.MustNull(has, "Name already exist!");

            ValidateName(name);

            var nodeUpdate = Builders<Node>.Update
                .Set(n => n.Name, name)
                .Set(n => n.UpdatedUtc, DateTime.UtcNow);
            await Node
                .UpdateAsync(n => n.NodeId == nodeId, nodeUpdate);
        }
    }
}
