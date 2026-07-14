using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ZooTech.Infrastructure.Exceptions;

namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public class MongoDbContextFactory
    {        
        private readonly IMongoDatabase _mongoDatabase;
        private readonly string _auditEventCollectionName;
        private readonly string _auditErrorCollectionName;

        public MongoDbContextFactory(
            MongoClient client,
            IConfiguration configuration
        )
        {
            var databaseName = configuration["MongoDb:DatabaseName"] ?? throw new UndefinedConfigurationValue(
                message: "Missing configuration: MongoDb:DatabaseName"
            );
            
            _auditEventCollectionName = configuration["MongoDb:EventLogsCollectionName"] ?? throw new UndefinedConfigurationValue(
                message: "Missing configuration: MongoDb:AuditCollectionName"
            );

            _auditErrorCollectionName = configuration["MongoDb:ErrorLogsCollectionName"] ?? throw new UndefinedConfigurationValue(
                message: "Missing configuration: MongoDb:ErrorLogsCollectionName"
            );
        
            _mongoDatabase = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetEventCollection<T>()
        {
            return _mongoDatabase.GetCollection<T>(_auditEventCollectionName);
        }

        public IMongoCollection<T> GetErrorCollection<T>()
        {
            return _mongoDatabase.GetCollection<T>(_auditErrorCollectionName);
        }
    }
}