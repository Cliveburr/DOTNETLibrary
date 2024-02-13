using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.DataAccess.Attributes;
using System.Reflection;

namespace Runner.Business.DataAccess
{
    public class CollectionConfigurations
    {
        public readonly Database _database;
        public readonly IMongoCollection<Entities.Configurations> _collection;

        public CollectionConfigurations(Database database)
        {
            _database = database;
            _collection = database.Main.GetCollection<Entities.Configurations>("__Configurations");
        }

        public void Configure()
        {
            _ = Task.Run(ConfigureAsync);
        }

        private async Task ConfigureAsync()
        {
            await CheckConfiguration("EnsureIndexes_1", EnsureIndexes);
        }

        private async Task CheckConfiguration(string configurationName, Func<Task> execute)
        {
            var has = await _collection
                .Find(c => c.Name == configurationName)
                .FirstOrDefaultAsync();
            if (has is null)
            {
                await execute();
                await _collection
                    .InsertOneAsync(new Entities.Configurations
                    {
                        Name = configurationName,
                        CreatedUtc = DateTime.UtcNow
                    });
            }
        }

        private bool HasIndex(string collectionName, string indexName)
        {
            var collection = _database.Main.GetCollection<BsonDocument>(collectionName);
            var indexes = collection.Indexes.List().ToList();
            var indexNames = indexes
                .SelectMany(index => index.Elements)
                .Where(element => element.Name == "name")
                .Select(name => name.Value.ToString());
            return indexNames.Contains(indexName);
        }

        private async Task CreateIndex(string collectionName, string fieldName, bool ascending)
        {
            var collection = _database.Main.GetCollection<BsonDocument>(collectionName);

            var indexKeysDefinitionBuilder = Builders<BsonDocument>.IndexKeys;
            IndexKeysDefinition<BsonDocument>? indexKeysDefinition = null;

            if (ascending)
            {
                indexKeysDefinition = indexKeysDefinitionBuilder
                    .Ascending(fieldName);
            }
            else
            {
                indexKeysDefinition = indexKeysDefinitionBuilder
                    .Descending(fieldName);
            }

            var createIndexModel = new CreateIndexModel<BsonDocument>(indexKeysDefinition);

            _ = await collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private async Task EnsureIndexesByAttributes(params Type[] types)
        {
            foreach (var type in types)
            {
                var databaseDefAttr = type.GetCustomAttribute<DatabaseDefAttribute>();
                if (databaseDefAttr is null)
                {
                    return;
                }

                var collectionName = databaseDefAttr.CollectionName ?? type.Name;

                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var indexDefAttr = property.GetCustomAttribute<IndexDefAttribute>();
                    if (indexDefAttr is null)
                    {
                        continue;
                    }

                    await CreateIndex(collectionName, property.Name, indexDefAttr.Ascending);
                }
            }
        }

        private async Task EnsureIndexes()
        {
            await EnsureIndexesByAttributes(
                typeof(Business.Entities.Nodes.Node),
                typeof(Business.Entities.Identity.User),
                typeof(Business.Entities.Identity.UserHome),
                typeof(Business.Entities.Security.AccessToken),

                typeof(Business.Entities.Job.Job),
                typeof(Business.Entities.AgentVersion.AgentVersion),

                typeof(Business.Entities.Nodes.Types.App),
                typeof(Business.Entities.Nodes.Types.Folder),
                typeof(Business.Entities.Nodes.Types.Data),
                typeof(Business.Entities.Nodes.Types.DataType),
                typeof(Business.Entities.Nodes.Types.Agent),
                typeof(Business.Entities.Nodes.Types.AgentPool),
                typeof(Business.Entities.Nodes.Types.Script),
                typeof(Business.Entities.Nodes.Types.ScriptPackage),
                typeof(Business.Entities.Nodes.Types.ScriptContent)
            );
        }
    }
}
