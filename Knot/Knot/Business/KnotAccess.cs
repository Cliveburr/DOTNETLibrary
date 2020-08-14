using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knot.Business
{
    public partial class KnotAccess
    {
        public MongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public IMongoCollection<Entities.Knot> Knots { get; private set; }
        public string KnotsName { get; private set; }

        public KnotAccess(string connectionString, string database, string knotsCollection)
        {
            ConventionRegistry.Register(
                "Ignore null values",
                new ConventionPack
                {
                    new IgnoreIfDefaultConvention(true)
                },
                t => true);

            Client = new MongoClient(connectionString);

            Database = Client.GetDatabase(database);
            KnotsName = knotsCollection;
            Knots = Database.GetCollection<Entities.Knot>(knotsCollection);
        }

        public Entities.Knot GetRootKnot()
        {
            var filter = Builders<Entities.Knot>.Filter.Eq(c => c.IsRoot, true);
            var find = Knots.FindSync(filter);
            find.MoveNext();
            if (find.Current.Any())
            {
                return find.Current.Single();
            }
            else
            {
                var knot = new Entities.Knot(true);
                Knots.InsertOneAsync(knot);
                return knot;
            }
        }
    }
}