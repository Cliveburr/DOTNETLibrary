using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using Runner.Business.Entities;
using Runner.Business.Entities.Agent;

namespace Runner.Business.DataAccess
{
    public class Database
    {
        public MongoClient Client { get; private set; }
        public IMongoDatabase Main { get; private set; }

        public Database(string connectionString, string mainDatabaseName)
        {
            RegisterClass();
            Client = new MongoClient(connectionString);
            Main = Client.GetDatabase(mainDatabaseName);
            //CheckUpdates();
        }

        private void RegisterClass()
        {
            BsonClassMap.RegisterClassMap<App>();
            BsonClassMap.RegisterClassMap<Folder>();
            BsonClassMap.RegisterClassMap<AgentPool>();
            BsonClassMap.RegisterClassMap<Agent>();
            BsonClassMap.RegisterClassMap<Flow>();
            BsonClassMap.RegisterClassMap<Run>();
        }

        private void CheckUpdates()
        {
            var collection = Main.GetCollection<Job>("Job");
            var indexKeysDefinition = Builders<Job>.IndexKeys.Ascending(j => j.Queued);
            collection.Indexes.CreateOneAsync(new CreateIndexModel<Job>(indexKeysDefinition)).Wait();
        }
    }
}
