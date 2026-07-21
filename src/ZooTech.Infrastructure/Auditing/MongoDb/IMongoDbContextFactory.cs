using MongoDB.Driver;

namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public interface IMongoDbContextFactory
    {
        public IMongoCollection<T> GetEventCollection<T>();
        public IMongoCollection<T> GetErrorCollection<T>();
    }
}