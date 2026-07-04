using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Identity;

namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public class MongoDbAudit : IAppAuditService
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly ITenantContext _tenantContext;
        private readonly ICurrentUserService _currentUserService;

        private readonly ILogger<MongoDbAudit> _logger;

        public MongoDbAudit(
            MongoDbContext mongoDbContext , 
            ITenantContext tenantContext,
            ICurrentUserService currentUserService,
            
            ILogger<MongoDbAudit> logger
        )
        {
            _mongoDbContext = mongoDbContext;
            _tenantContext = tenantContext;
            _currentUserService = currentUserService;

            _logger = logger;
        }

        public async Task SaveLogAsync(AuditModel auditModel)
        {
            try
            {
                var mongoDbCollection = _mongoDbContext.GetCollection<MongoDbAuditModel>();

                var newLog = new MongoDbAuditModel
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantContext.TenantId,
                    TenantCode = _tenantContext.Code,
                    EventType = auditModel.EventType.ToString(),
                    Action = auditModel.Action,
                    User = new AuditUser
                    {
                        Id = _currentUserService.UserId,
                        Name = _currentUserService.UserName
                    },
                    OldValues = auditModel.OldValues,
                    NewValues = auditModel.NewValues,
                    CreatedAt = auditModel.CreatedAt
                };

                await mongoDbCollection.InsertOneAsync(newLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "ZooTechException: Failed to save audit log to MongoDB. Tenant: {TenantId}, Event: {EventType}",
                    _tenantContext.TenantId,
                    auditModel.EventType);
            }

        }
    }
}