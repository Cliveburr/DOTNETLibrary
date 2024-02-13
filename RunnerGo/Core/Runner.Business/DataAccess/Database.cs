using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace Runner.Business.DataAccess
{
    public class Database
    {
        public MongoClient Client { get; private set; }
        public IMongoDatabase Main { get; private set; }

        public Database(string? connectionString)
        {
            var cb = new ConnectionString(connectionString);

            Client = new MongoClient(connectionString);
            Main = Client.GetDatabase(cb.DatabaseName);

            var objectSerializer = new ObjectSerializer(x => true);
            BsonSerializer.RegisterSerializer(objectSerializer);
            //CheckUpdates();
        }

        //private void CheckUpdates()
        //{
        //    var collection = Main.GetCollection<Job>("Job");
        //    var indexKeysDefinition = Builders<Job>.IndexKeys.Ascending(j => j.Queued);
        //    collection.Indexes.CreateOneAsync(new CreateIndexModel<Job>(indexKeysDefinition)).Wait();
        //}
    }
}
