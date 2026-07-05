using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
        private readonly IJwtService _jwtService;

        private static readonly HashSet<string> SensitiveKeys =
        [
            "password",
            "passwordhash",
            "token",
            "refreshtoken",
            "accesstoken",
            "jwt_id",
            "jti",
            "last_login_at",
            "secret",
            "apikey",
            "authorization",
            "cookie"
        ];
        private int MaxArrayItems;
        private int MaxStringLength;
        private int MaxDepth;

        private readonly ILogger<MongoDbAudit> _logger;

        public MongoDbAudit(
            MongoDbContext mongoDbContext , 
            ITenantContext tenantContext,
            ICurrentUserService currentUserService,
            IJwtService jwtService,

            IConfiguration config,

            ILogger<MongoDbAudit> logger
        )
        {
            _mongoDbContext = mongoDbContext;
            _tenantContext = tenantContext;
            _currentUserService = currentUserService;
            _jwtService = jwtService;

            _logger = logger;

            MaxArrayItems = config.GetValue<int>("Auditing:MaxArrayItems", 20);
            MaxStringLength = config.GetValue<int>("Auditing:MaxStringLength", 1000);
            MaxDepth = config.GetValue<int>("Auditing:MaxDepth", 3);
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
                    RequestValues =
                        auditModel.RequestValues is null
                            ? null
                            : Normalize(SerializeToBson(auditModel.RequestValues)!),

                    ResponseValues =
                        auditModel.ResponseValues is null
                            ? null
                            : Normalize(SerializeToBson(auditModel.ResponseValues)!),
                    CreatedAt = DateTime.UtcNow
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
    
        private static BsonValue? SerializeToBson(object? value)
        {
            if (value is null)
                return BsonNull.Value;

            var json = JsonSerializer.Serialize(value);

            return BsonSerializer.Deserialize<BsonValue>(json);
        }

        private BsonValue Normalize(
            string key,
            BsonValue value,
            int depth)
        {
            if (IsSensitive(key))
                return "[HIDDEN]";

            return Normalize(value, depth);
        }

        private BsonValue Normalize(BsonValue value, int depth = 0)
        {
            if (depth >= MaxDepth)
                return new BsonDocument
                {
                    ["_truncated"] = "Maximum depth reached"
                };

            if (value.IsBsonDocument)
            {
                var result = new BsonDocument();

                foreach (var element in value.AsBsonDocument)
                {
                    result[element.Name] = Normalize(
                        element.Name,
                        element.Value,
                        depth + 1);
                }

                return result;
            }

            if (value.IsBsonArray)
            {
                return NormalizeArray(
                    value.AsBsonArray,
                    depth + 1);
            }

            if (value.IsString)
            {
                var text = value.AsString;

                if (_jwtService.IsJwt(text) || LooksLikeToken(text))
                    return "[TOKEN HIDDEN]";

                if (text.Length > MaxStringLength)
                    return text[..MaxStringLength] + "...";

                return value;
            }

            return value;
        }

        private BsonArray NormalizeArray(
            BsonArray source,
            int depth)
        {
            var result = new BsonArray();

            foreach (var item in source.Take(MaxArrayItems))
            {
                result.Add(Normalize(item, depth));
            }

            if (source.Count > MaxArrayItems)
            {
                result.Add(new BsonDocument
                {
                    ["_truncated"] = true,
                    ["_remaining"] = source.Count - MaxArrayItems
                });
            }

            return result;
        }
        private static bool IsSensitive(string key)
        {
            key = key.Replace("_", "")
                    .Replace("-", "")
                    .ToLowerInvariant();

            return SensitiveKeys.Any(key.Contains);
        }

        private static bool LooksLikeToken(string value)
        {
            return value.Length > 80 &&
                value.All(c =>
                    char.IsLetterOrDigit(c) ||
                    c == '.' ||
                    c == '-' ||
                    c == '_' ||
                    c == '=');
        }
    }
}