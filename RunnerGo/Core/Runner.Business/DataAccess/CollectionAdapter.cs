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
            return Collection.InsertOneAsync(doc);
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



        //public Task<IClientSessionHandle> StartSessionAsync()
        //{
        //    return _collection.Database.Client.StartSessionAsync();
        //}

        //public IFindFluent<T, T?> Find(Expression<Func<T, bool>> filter, FindOptions? options = null)
        //{
        //    return _collection.Find(filter, options);
        //}


        //public Task<E?> FirstOrDefaultAsync<E>(Expression<Func<E, bool>> filter, FindOptions? options = null) where E : T
        //{
        //    return GetCollection<E>().Find(filter, options)
        //        .FirstOrDefaultAsync();
        //}

        //public Task<List<T>> ToListAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        //{
        //    return _collection.Find(filter, options)
        //        .ToListAsync();
        //}

        //public Task<List<E>> ToListAsync<E>(Expression<Func<E, bool>> filter, FindOptions? options = null) where E: T
        //{
        //    return GetCollection<E>().Find(filter, options)
        //        .ToListAsync();
        //}

        //public Task<bool> AnyAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        //{
        //    return _collection.Find(filter, options)
        //        .AnyAsync();
        //}

        //public Task CreateAsync(T obj)
        //{
        //    return _collection.InsertOneAsync(obj);
        //}

        //public Task SaveAsync(T obj)
        //{
        //    return _collection.FindOneAndReplaceAsync(e => e.Id == obj.Id, obj);
        //}

        //public Task DeleteAsync(T obj)
        //{
        //    return _collection.DeleteOneAsync(e => e.Id == obj.Id);
        //}

        //public Task UpdateAsync(T obj, UpdateDefinition<T> update, UpdateOptions? options = null)
        //{
        //    return _collection.UpdateOneAsync(e => e.Id == obj.Id, update, options);
        //}

        //public Task UpdateAsync<E>(E obj, UpdateDefinition<E> update, UpdateOptions? options = null) where E : T
        //{
        //    return GetCollection<E>().UpdateOneAsync(e => e.Id == obj.Id, update, options);
        //}

        //public Task UpdateAsync<E>(Expression<Func<E, bool>> filter, UpdateDefinition<E> update, UpdateOptions? options = null) where E : T
        //{
        //    return GetCollection<E>().UpdateOneAsync(filter, update, options);
        //}

        //public Task UpdateAsync<E>(FilterDefinition<E> filter, UpdateDefinition<E> update, UpdateOptions? options = null) where E : T
        //{
        //    return GetCollection<E>().UpdateOneAsync(filter, update, options);
        //}

        //public Task<T?> ReadByIdAsync(ObjectId id)
        //{
        //    return _collection.Find(e => e.Id == id)
        //        .FirstOrDefaultAsync();
        //}

        //public Task<List<P>> ProjectToListAsync<E, P>(Expression<Func<E, bool>> filter, ProjectionDefinition<E, P> projection) where E : T
        //{
        //    return GetCollection<E>()
        //        .Find(filter)
        //        .Project(projection)
        //        .ToListAsync();
        //}
    }
}

#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

