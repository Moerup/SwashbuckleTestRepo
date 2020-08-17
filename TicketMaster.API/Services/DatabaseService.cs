using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TicketMaster.API.Interfaces;
using TicketMaster.API.Models;

namespace TicketMaster.API.Services
{
    public class DatabaseService<T> where T : IBaseModel
    {
        private static MongoClient _client = new MongoClient(Environment.GetEnvironmentVariable("ConnectionString"));
        private IMongoDatabase _database = _client.GetDatabase("TicketMaster");
        private static readonly string _collectionModelName = typeof(T).UnderlyingSystemType.Name + "s";

        public IMongoQueryable<T> GetCollection()
        {
            var collection = _database.GetCollection<T>(_collectionModelName).AsQueryable();

            return collection;
        }

        public async Task SaveItem(T model)
        {
            var collection = _database.GetCollection<T>(_collectionModelName);
            await collection.InsertOneAsync(model);
        }

        public async Task UpdateItem(T model)
        {
            var collection = _database.GetCollection<T>(_collectionModelName);

            await collection.FindOneAndReplaceAsync(x => x.Id == model.Id, model);
        }

        // Doesn't work for ItemType yet
        public async Task SaveItemAndLinkToConfigurationItem(T model, string configurationItemId)
        {
            await SaveItem(model);
            var collection = _database.GetCollection<ConfigurationItem>("ConfigurationItem");
            var filter = Builders<ConfigurationItem>.Filter.Eq("Id", configurationItemId);

            if (model is Product)
            {
                var update = Builders<ConfigurationItem>.Update.Set(x => x.ProductIds[-1], model.Id);
                await collection.FindOneAndUpdateAsync(filter, update);
            }
            if (model is Field)
            {
                var update = Builders<ConfigurationItem>.Update.Set(x => x.FieldIds[-1], model.Id);
                await collection.FindOneAndUpdateAsync(filter, update);
            }
        }

        public async Task DeleteItem(string id)
        {
            var collection = _database.GetCollection<T>(_collectionModelName);
            await collection.FindOneAndDeleteAsync(x => x.Id == id);
        }
    }
}
