using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Runner.Business.DataAccess
{
    public class CollectionAdapter<T> where T : DocumentBase
    {
        private IMongoCollection<T> _collection;

        private IMongoCollection<E> GetCollection<E>() where E: T
        {
            var name = _collection.CollectionNamespace.CollectionName;
            return _collection.Database.GetCollection<E>(name);
        }

        public CollectionAdapter(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public Task<IClientSessionHandle> StartSessionAsync()
        {
            return _collection.Database.Client.StartSessionAsync();
        }

        public IFindFluent<T, T?> Find(Expression<Func<T, bool>> filter, FindOptions? options = null)
        {
            return _collection.Find(filter, options);
        }

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        {
            return _collection.Find(filter, options)
                .FirstOrDefaultAsync();
        }

        public Task<E?> FirstOrDefaultAsync<E>(Expression<Func<E, bool>> filter, FindOptions? options = null) where E : T
        {
            return GetCollection<E>().Find(filter, options)
                .FirstOrDefaultAsync();
        }

        public Task<List<T>> ToListAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        {
            return _collection.Find(filter, options)
                .ToListAsync();
        }

        public Task<List<E>> ToListAsync<E>(Expression<Func<E, bool>> filter, FindOptions? options = null) where E: T
        {
            return GetCollection<E>().Find(filter, options)
                .ToListAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        {
            return _collection.Find(filter, options)
                .AnyAsync();
        }

        public Task CreateAsync(T obj)
        {
            return _collection.InsertOneAsync(obj);
        }

        public Task SaveAsync(T obj)
        {
            return _collection.FindOneAndReplaceAsync(e => e.Id == obj.Id, obj);
        }

        public Task DeleteAsync(T obj)
        {
            return _collection.DeleteOneAsync(e => e.Id == obj.Id);
        }

        public Task UpdateAsync(T obj, UpdateDefinition<T> update, UpdateOptions? options = null)
        {
            return _collection.UpdateOneAsync(e => e.Id == obj.Id, update, options);
        }

        public Task UpdateAsync<E>(E obj, UpdateDefinition<E> update, UpdateOptions? options = null) where E : T
        {
            return GetCollection<E>().UpdateOneAsync(e => e.Id == obj.Id, update, options);
        }

        public Task UpdateAsync<E>(Expression<Func<E, bool>> filter, UpdateDefinition<E> update, UpdateOptions? options = null) where E : T
        {
            return GetCollection<E>().UpdateOneAsync(filter, update, options);
        }

        public Task UpdateAsync<E>(FilterDefinition<E> filter, UpdateDefinition<E> update, UpdateOptions? options = null) where E : T
        {
            return GetCollection<E>().UpdateOneAsync(filter, update, options);
        }

        public Task<T?> ReadByIdAsync(ObjectId id)
        {
            return _collection.Find(e => e.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<List<P>> ProjectToListAsync<E, P>(Expression<Func<E, bool>> filter, ProjectionDefinition<E, P> projection) where E : T
        {
            return GetCollection<E>()
                .Find(filter)
                .Project(projection)
                .ToListAsync();
        }
    }
}

#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

