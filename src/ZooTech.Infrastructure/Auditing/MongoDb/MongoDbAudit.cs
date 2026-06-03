using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public class MongoDbAudit : IAppAuditService
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly ITenantContext _tenantContext;

        public MongoDbAudit(
            MongoDbContext mongoDbContext , 
            ITenantContext tenantContext
        )
        {
            _mongoDbContext = mongoDbContext;
            _tenantContext = tenantContext;
        }

        public async Task SavingChangesAsync(AuditModel auditModel)
        {
            try
            {

                var mongoDbCollection = _mongoDbContext.GetCollection<MongoDbAuditModel>();

                var newLog = new MongoDbAuditModel
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantContext.TenantId,
                    TenantCode = _tenantContext.Code,
                    EventType = auditModel.EventType,
                    Action = auditModel.Action,
                    User = new AuditUser
                    {
                        Id = auditModel.UserId,
                        Name = auditModel.UserName
                    },
                    OldValues = auditModel.OldValues,
                    NewValues = auditModel.NewValues,
                    CreatedAt = auditModel.CreatedAt
                };

                await mongoDbCollection.InsertOneAsync(newLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}