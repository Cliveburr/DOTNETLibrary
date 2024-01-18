
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

        protected CollectionAdapter<Entities.Nodes.Types.App> App { get => GetCollectionAdapter<Entities.Nodes.Types.App>("App"); }
        protected CollectionAdapter<Entities.Nodes.Types.Folder> Folder { get => GetCollectionAdapter<Entities.Nodes.Types.Folder>("Folder"); }
        protected CollectionAdapter<Entities.Nodes.Types.Data> Data { get => GetCollectionAdapter<Entities.Nodes.Types.Data>("Data"); }
        protected CollectionAdapter<Entities.Nodes.Types.DataType> DataType { get => GetCollectionAdapter<Entities.Nodes.Types.DataType>("DataType"); }
    }
}
