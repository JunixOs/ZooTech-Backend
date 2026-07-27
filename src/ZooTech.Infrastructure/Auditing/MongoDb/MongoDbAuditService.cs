using System.Text.Json;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Infrastructure.Auditing.MongoDb.Models;

namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public class MongoDbAuditService : IAppAuditService
    {
        private readonly IMongoDbContextFactory _mongoDbContext;
        private readonly ITenantContext _tenantContext;
        private readonly ICurrentUserService _currentUserService;

        private readonly MongoDbLogNormalizer _mongoDbLogNormalizer;

        private readonly ILogger<MongoDbAuditService> _logger;

        public MongoDbAuditService(
            IMongoDbContextFactory mongoDbContext , 
            ITenantContext tenantContext,
            ICurrentUserService currentUserService,
            MongoDbLogNormalizer mongoDbLogNormalizer,

            ILogger<MongoDbAuditService> logger
        )
        {
            _mongoDbContext = mongoDbContext;
            _tenantContext = tenantContext;
            _currentUserService = currentUserService;

            _mongoDbLogNormalizer = mongoDbLogNormalizer;

            _logger = logger;
        }

        public async Task AuditEventAsync(AuditEventInfo auditEventInfo)
        {
            try
            {
                var mongoDbCollection = _mongoDbContext.GetEventCollection<MongoDbEventModel>();

                var newEventLog = new MongoDbEventModel
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantContext.TenantId,
                    TenantCode = _tenantContext.Code,
                    EventType = auditEventInfo.EventType,
                    Action = auditEventInfo.Action,
                    User = new AuditUser
                    {
                        Id = _currentUserService.UserId,
                        UserName = _currentUserService.UserName
                    },
                    RequestValues =
                        auditEventInfo.RequestValues is null
                            ? null
                            : _mongoDbLogNormalizer.Normalize(SerializeToBson(auditEventInfo.RequestValues)!),

                    ResponseValues =
                        auditEventInfo.ResponseValues is null
                            ? null
                            : _mongoDbLogNormalizer.Normalize(SerializeToBson(auditEventInfo.ResponseValues)!),
                    RegisteredAt = DateTime.UtcNow
                };

                await mongoDbCollection.InsertOneAsync(newEventLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "ZooTechException: Failed to save audit log to MongoDB. Tenant: {TenantId}, Event: {EventType}",
                    _tenantContext.TenantId,
                    auditEventInfo.EventType);
            }

        }

        public async Task AuditErrorAsync(AuditErrorInfo auditErrorInfo)
        {
            try
            {
                var mongoDbCollection = _mongoDbContext.GetErrorCollection<MongoDbErrorModel>();

                var newErrorLogg = new MongoDbErrorModel
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantContext.TenantId,
                    TenantCode = _tenantContext.Code,
                    EventType = auditErrorInfo.EventType,
                    CustomMessage = auditErrorInfo.CustomMessage,
                    User = new AuditUser
                    {
                        Id = _currentUserService.UserId,
                        UserName = _currentUserService.UserName
                    },

                    AuditErrorBaseInformation = new AuditErrorBaseInformation
                    {
                        ExceptionType = auditErrorInfo.ExceptionType,
                        RootExceptionType = auditErrorInfo.RootExceptionType,

                        Message = auditErrorInfo.Message,
                        RootMessage = auditErrorInfo.RootMessage,

                        Source = auditErrorInfo.Source,
                        DeclaringType = auditErrorInfo.DeclaringType,
                        
                        Method = auditErrorInfo.Method,
                        Inner = auditErrorInfo.Inner
                    },
                    AuditErrorAppInformation = auditErrorInfo.AppInformation is null
                        ? null
                        : new AuditErrorAppInformation
                            {
                                ErrorCode = auditErrorInfo.AppInformation.ErrorCode,
                                ScopeName = auditErrorInfo.AppInformation.ScopeName,
                                ModuleName = auditErrorInfo.AppInformation.ModuleName,
                                Details = auditErrorInfo.AppInformation.Details,
                                CompleteErrorCode = auditErrorInfo.AppInformation.CompleteErrorCode
                            },

                    RegisteredAt = DateTime.UtcNow
                };

                await mongoDbCollection.InsertOneAsync(newErrorLogg);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "ZooTechException: Failed to save audit log in MongoDB. Tenant: {TenantId}, Event: {EventType}",
                    _tenantContext.TenantId,
                    auditErrorInfo.EventType);
            }
        }
    
        private static BsonValue? SerializeToBson(object? value)
        {
            if (value is null)
                return BsonNull.Value;

            var json = JsonSerializer.Serialize(value);

            return BsonSerializer.Deserialize<BsonValue>(json);
        }
    }
}