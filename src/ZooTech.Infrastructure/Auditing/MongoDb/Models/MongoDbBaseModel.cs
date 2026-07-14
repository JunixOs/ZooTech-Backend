using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Auditing.MongoDb.Models
{
    public abstract class MongoDbBaseModel
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [BsonElement("tenant_id")]
        public int TenantId { get; set; }

        [BsonElement("tenant_code")]
        public string TenantCode { get; set; } = default!;

        [BsonElement("event_type")]
        public AuditEventType EventType { get; set; } = default!;

        [BsonElement("action")]
        public string Action { get; set; } = default!;

        [BsonElement("user")]
        public AuditUser User { get; set; } = default!;

        // Aqui iran mas elementos especificos de un log
        // de evento o error

        [BsonElement("registered_at")]
        public DateTime RegisteredAt { get; set; }
    }

    public class AuditUser
    {
        [BsonElement("id")]
        public int? Id { get; set; }

        [BsonElement("username")]
        public string? UserName { get; set; }
    }
}