using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ZooTech.Infrastructure.Exceptions;

namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly string _auditCollectionName;

        public MongoDbContext(
            MongoClient client,
            IConfiguration configuration
        )
        {
            var databaseName = configuration["MongoDb:DatabaseName"] ?? throw new UndefinedConfigurationValue(
                message: "Missing configuration: MongoDb:DatabaseName"
            );
            
            _auditCollectionName = configuration["MongoDb:AuditCollectionName"] ?? throw new UndefinedConfigurationValue(
                message: "Missing configuration: MongoDb:AuditCollectionName"
            );
        
            _mongoDatabase = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _mongoDatabase.GetCollection<T>(_auditCollectionName);
        }
    }
}