using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.Entities.Authentication;
using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.DataAccess
{
    public abstract class ServiceBase
    {
        public Database Database { get; private set; }
        private Dictionary<string, object> _collections;

        public ServiceBase(Database database)
        {
            Database = database;
            _collections = new Dictionary<string, object>();
        }

        private CollectionAdapter<T> GetCollectionAdapter<T>(string name)
        {
            if (!_collections.ContainsKey(name))
            {
                var collection = Database.Main.GetCollection<T>(name);
                var collectionAdpter = new CollectionAdapter<T>(collection);
                _collections[name] = collectionAdpter;
            }
            return (CollectionAdapter<T>)_collections[name];
        }

        protected CollectionAdapter<User> User { get => GetCollectionAdapter<User>("User"); }
        protected CollectionAdapter<AccessToken> AccessToken { get => GetCollectionAdapter<AccessToken>("AccessToken"); }
        protected CollectionAdapter<Job> Job { get => GetCollectionAdapter<Job>("Job"); }
     
        protected CollectionAdapter<Node> Node { get => GetCollectionAdapter<Node>("Node"); }
        protected CollectionAdapter<App> App { get => GetCollectionAdapter<App>("App"); }
    }
}
