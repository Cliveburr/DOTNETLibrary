using Runner.Business.DataAccess.Attributes;
using System.Reflection;

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

        private CollectionAdapter<T> GetCollectionAdapter<T>()
        {
            var type = typeof(T);
            var databaseDefAttr = type.GetCustomAttribute<DatabaseDefAttribute>();
            if (databaseDefAttr is null)
            {
                throw new Exception("Internal - Missing DatabaseDef attribute on class: " + type.FullName);
            }

            var collectionName = databaseDefAttr.CollectionName ?? type.Name;

            if (!_collections.ContainsKey(collectionName))
            {
                var collection = Database.Main.GetCollection<T>(collectionName);
                var collectionAdpter = new CollectionAdapter<T>(collection, collectionName);
                _collections[collectionName] = collectionAdpter;
            }
            return (CollectionAdapter<T>)_collections[collectionName];
        }

        protected CollectionAdapter<Business.Entities.Nodes.Node> Node { get => GetCollectionAdapter<Business.Entities.Nodes.Node>(); }
        protected CollectionAdapter<Business.Entities.Identity.User> User { get => GetCollectionAdapter<Business.Entities.Identity.User>(); }
        protected CollectionAdapter<Business.Entities.Identity.UserHome> UserHome { get => GetCollectionAdapter<Business.Entities.Identity.UserHome>(); }
        protected CollectionAdapter<Business.Entities.Security.AccessToken> AccessToken { get => GetCollectionAdapter<Business.Entities.Security.AccessToken>(); }

        protected CollectionAdapter<Business.Entities.Job.Job> Job { get => GetCollectionAdapter<Business.Entities.Job.Job>(); }
        protected CollectionAdapter<Business.Entities.AgentVersion.AgentVersion> AgentVersion { get => GetCollectionAdapter<Business.Entities.AgentVersion.AgentVersion>(); }

        protected CollectionAdapter<Business.Entities.Nodes.Types.App> App { get => GetCollectionAdapter<Business.Entities.Nodes.Types.App>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.Folder> Folder { get => GetCollectionAdapter<Business.Entities.Nodes.Types.Folder>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.Data> Data { get => GetCollectionAdapter<Business.Entities.Nodes.Types.Data>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.AgentPool> AgentPool { get => GetCollectionAdapter<Business.Entities.Nodes.Types.AgentPool>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.Agent> Agent { get => GetCollectionAdapter<Business.Entities.Nodes.Types.Agent>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.ScriptPackage> ScriptPackage { get => GetCollectionAdapter<Business.Entities.Nodes.Types.ScriptPackage>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.Script> Script { get => GetCollectionAdapter<Business.Entities.Nodes.Types.Script>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.ScriptContent> ScriptContent { get => GetCollectionAdapter<Business.Entities.Nodes.Types.ScriptContent>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.Flow> Flow { get => GetCollectionAdapter<Business.Entities.Nodes.Types.Flow>(); }
        protected CollectionAdapter<Business.Entities.Nodes.Types.Run> Run { get => GetCollectionAdapter<Business.Entities.Nodes.Types.Run>(); }
    }
}
