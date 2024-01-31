using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Runner.Business.DataAccess
{
    public class CollectionAdapter<T>
    {
        public string Name { get; }
        public IMongoCollection<T> Collection { get; }

        public CollectionAdapter(IMongoCollection<T> collection, string name)
        {
            Collection = collection;
            Name = name;
        }

        public CollectionAdapter<P> Polymorphic<P>() where P: T
        {
            return new CollectionAdapter<P>(Collection.Database.GetCollection<P>(Name), Name);
        }

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        {
            return Collection.Find(filter, options)
                .FirstOrDefaultAsync();
        }

        public Task<ReplaceOneResult> ReplaceAsync(Expression<Func<T, bool>> filter, T doc, ReplaceOptions? options = null)
        {
            return Collection
                .ReplaceOneAsync(filter, doc, options);
        }

        public Task InsertAsync(T doc)
        {
            return Collection
                .InsertOneAsync(doc);
        }

        public Task UpdateAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, UpdateOptions? options = null)
        {
            return Collection
                .UpdateOneAsync(filter, update, options);
        }

        public IAggregateFluent<T> Aggregate()
        {
            return Collection
                .Aggregate();
        }

        public IMongoQueryable<T> AsQueryable()
        {
            return Collection
                .AsQueryable();
        }

        public Task<List<T>> ToListAsync()
        {
            return Collection.Find(Builders<T>.Filter.Empty)
                .ToListAsync();
        }

        public Task<List<T>> ToListAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        {
            return Collection.Find(filter, options)
                .ToListAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        {
            return Collection.Find(filter, options)
                .AnyAsync();
        }

        public Task DeleteAsync(Expression<Func<T, bool>> filter, DeleteOptions? options = null)
        {
            return Collection
                .DeleteOneAsync(filter, options);
        }

        public Task<List<P>> ProjectToListAsync<P>(Expression<Func<T, bool>> filter, ProjectionDefinition<T, P> projection)
        {
            return Collection
                .Find(filter)
                .Project(projection)
                .ToListAsync();
        }

        //public Task<IClientSessionHandle> StartSessionAsync()
        //{
        //    return _collection.Database.Client.StartSessionAsync();
        //}

        //public IFindFluent<T, T?> Find(Expression<Func<T, bool>> filter, FindOptions? options = null)
        //{
        //    return _collection.Find(filter, options);
        //}
    }
}

#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

