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
//using Runner.Business.Entities.Job;
using MongoDB.Driver.Core.Configuration;
using Runner.Domain.Entities.Identity;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using System.Runtime.Serialization;
using Runner.Domain.Entities;
using System.Formats.Asn1;
using System.Linq.Expressions;
using Runner.Domain.Entities.Authentication;

namespace Runner.Infrastructure.DataAccess
{
    public class MainDatabase
    {
        public MongoClient Client { get; private set; }
        public IMongoDatabase Main { get; private set; }

        public MainDatabase(string? connectionString)
        {
            var cb = new ConnectionString(connectionString);
            
            Client = new MongoClient(connectionString);
            Main = Client.GetDatabase(cb.DatabaseName);
            //CheckUpdates();

            BsonSerializer.RegisterSerializer(new Serializer.EntityIdSerializer());

            RegisterClassMap<User>(e => e.UserId);
            RegisterClassMap<AccessToken>(e => e.AccessTokenId);
        }

        private void RegisterClassMap<TClass>(Expression<Func<TClass, EntityId>> memberLambda)
        {
            BsonClassMap.RegisterClassMap<TClass>(e =>
            {
                e.AutoMap();
                e.SetIgnoreExtraElements(true);
                e.MapIdMember(memberLambda);
            });
        }

        //private void CheckUpdates()
        //{
        //    var collection = Main.GetCollection<Job>("Job");
        //    var indexKeysDefinition = Builders<Job>.IndexKeys.Ascending(j => j.Queued);
        //    collection.Indexes.CreateOneAsync(new CreateIndexModel<Job>(indexKeysDefinition)).Wait();
        //}
    }
}
