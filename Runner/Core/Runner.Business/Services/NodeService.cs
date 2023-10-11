using MongoDB.Driver;
using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using Runner.Business.Entities;
using Runner.Business.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            return Create(new Folder
            {
                Name = name,
                Type = NodeType.Folder,
                Parent = parent.Id
            });
        }

        private async Task Create(NodeBase node)
        {
            Assert.Strings.NotNullAndRange(node.Name, 3, 20, "Name size invalid 3 < name < 20");

            var has = await Node
                .AnyAsync(n => n.Parent == node.Parent && n.Name == node.Name);
            Assert.MustFalse(has, "Name taken!");

            await Node.CreateAsync(node);
        }
    }
}
