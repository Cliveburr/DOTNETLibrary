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
