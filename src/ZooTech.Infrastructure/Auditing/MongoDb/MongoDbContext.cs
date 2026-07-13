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
            var databaseName = configuration["MongoDb:DatabaseName"];
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = "zootech_audit";
            }
            
            _auditCollectionName = configuration["MongoDb:AuditCollectionName"];
            if (string.IsNullOrWhiteSpace(_auditCollectionName))
            {
                _auditCollectionName = "audit_logs";
            }
        
            _mongoDatabase = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _mongoDatabase.GetCollection<T>(_auditCollectionName);
        }
    }
}