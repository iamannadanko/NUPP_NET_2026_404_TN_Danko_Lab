using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Library.Infrastructure.Repositories;

namespace Library.Nosql
{
    // Реалізація інтерфейсу IRepository для нереляційної документної бази даних MongoDB (Завдання на +2 бали)
    public class MongoRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(string connectionString = "mongodb://localhost:27017", string databaseName = "LibraryNosqlDb", string? collectionName = null)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            collectionName ??= typeof(T).Name;
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _collection.InsertOneAsync(entity);
        }

        public async Task Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null) throw new InvalidOperationException("Entity must have an Id property");
            var idVal = idProp.GetValue(entity);
            var filter = Builders<T>.Filter.Eq("Id", idVal);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null) throw new InvalidOperationException("Entity must have an Id property");
            var idVal = idProp.GetValue(entity);
            var filter = Builders<T>.Filter.Eq("Id", idVal);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
