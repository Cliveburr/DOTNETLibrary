
using Runner.Business.Entities.AgentVersion;

namespace Runner.Business.DataAccess
{
    public abstract class DataServiceBase
    {
        public Database Database { get; private set; }
        private Dictionary<string, object> _collections;

        public DataServiceBase(Database database)
        {
            Database = database;
            _collections = new Dictionary<string, object>();
        }

        private CollectionAdapter<T> GetCollectionAdapter<T>(string name)
        {
            if (!_collections.ContainsKey(name))
            {
                var collection = Database.Main.GetCollection<T>(name);
                var collectionAdpter = new CollectionAdapter<T>(collection, name);
                _collections[name] = collectionAdpter;
            }
            return (CollectionAdapter<T>)_collections[name];
        }

        protected CollectionAdapter<Entities.Nodes.Node> Node { get => GetCollectionAdapter<Entities.Nodes.Node>("Node"); }
        protected CollectionAdapter<Entities.Identity.User> User { get => GetCollectionAdapter<Entities.Identity.User>("User"); }
        protected CollectionAdapter<Entities.Security.AccessToken> AccessToken { get => GetCollectionAdapter<Entities.Security.AccessToken>("AccessToken"); }

        protected CollectionAdapter<Entities.Job.Job> Job { get => GetCollectionAdapter<Entities.Job.Job>("Job"); }
        protected CollectionAdapter<AgentVersion> AgentVersion { get => GetCollectionAdapter<AgentVersion>("AgentVersion"); }

        protected CollectionAdapter<Entities.Nodes.Types.App> App { get => GetCollectionAdapter<Entities.Nodes.Types.App>("App"); }
        protected CollectionAdapter<Entities.Nodes.Types.Folder> Folder { get => GetCollectionAdapter<Entities.Nodes.Types.Folder>("Folder"); }
        protected CollectionAdapter<Entities.Nodes.Types.Data> Data { get => GetCollectionAdapter<Entities.Nodes.Types.Data>("Data"); }
        protected CollectionAdapter<Entities.Nodes.Types.DataType> DataType { get => GetCollectionAdapter<Entities.Nodes.Types.DataType>("DataType"); }
        protected CollectionAdapter<Entities.Nodes.Types.AgentPool> AgentPool { get => GetCollectionAdapter<Entities.Nodes.Types.AgentPool>("AgentPool"); }
        protected CollectionAdapter<Entities.Nodes.Types.Agent> Agent { get => GetCollectionAdapter<Entities.Nodes.Types.Agent>("Agent"); }
        protected CollectionAdapter<Entities.Nodes.Types.ScriptPackage> ScriptPackage { get => GetCollectionAdapter<Entities.Nodes.Types.ScriptPackage>("ScriptPackage"); }
        protected CollectionAdapter<Entities.Nodes.Types.Script> Script { get => GetCollectionAdapter<Entities.Nodes.Types.Script>("Script"); }
        protected CollectionAdapter<Entities.Nodes.Types.ScriptContent> ScriptContent { get => GetCollectionAdapter<Entities.Nodes.Types.ScriptContent>("ScriptContent"); }
    }
}
