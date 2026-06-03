using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly string _auditCollectionName;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDb:ConnectionString"] ?? throw new InvalidOperationException("MongoDB ConnectionString no configurado");
            var databaseName = configuration["MongoDb:DatabaseName"] ?? throw new InvalidOperationException("MongoDB DatabaseName no configurado");
            
            _auditCollectionName = configuration["MongoDb:AuditCollectionName"] ?? throw new InvalidOperationException("MongoDB CollectionName no configurado");
        
            var client = new MongoClient(connectionString);
            _mongoDatabase = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _mongoDatabase.GetCollection<T>(_auditCollectionName);
        }
    }
}