using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Runner.Domain.Entities;
using Runner.Infrastructure.DataAccess;
using System.Formats.Asn1;
using System.Linq.Expressions;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Runner.Business.DataAccess
{
    internal abstract class CollectionBase<T>
    {
        protected IMongoCollection<T> _collection;
        public abstract string Name { get; }
        
        //public BsonMemberMap MapIdMember<TMember>(Expression<Func<TClass, TMember>> memberLambda)
        //private IMongoCollection<E> GetCollection<E>() where E: T
        //{
        //    var name = _collection.CollectionNamespace.CollectionName;
        //    return _collection.Database.GetCollection<E>(name);
        //}

        public CollectionBase(MainDatabase database)
        {
            _collection = database.Main.GetCollection<T>(Name);
        }

        public Task<IClientSessionHandle> StartSessionAsync()
        {
            return _collection.Database.Client.StartSessionAsync();
        }

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, FindOptions? options = null)
        {
            return _collection.Find(filter, options)
                .FirstOrDefaultAsync();
        }

        public Task InsertAsync(T doc)
        {
            return _collection.InsertOneAsync(doc);
        }

        public virtual FilterDefinition<T> CreateEntityIdFilter(T doc)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T doc)
        {
            var filter = CreateEntityIdFilter(doc);
            return _collection.ReplaceOneAsync(filter, doc);
        }





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

