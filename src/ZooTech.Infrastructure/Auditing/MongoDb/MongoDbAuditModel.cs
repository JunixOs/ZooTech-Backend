using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


// Recibe los datos del documento
// {
//   "_id": UUID,
// 
//   "tenant_id": 1,
//   "tenant_code": "TENANT01",
// 
//   "event_type": "DOMAIN",
// 
//   "action": "UPDATE",
// 
//   "user": {
//     "id": 10,
//     "name": "admin"
//   },
// 
//   "old_values": {
//     "name": "Old Name"
//   },
// 
//   "new_values": {
//     "name": "New Name"
//   },
// 
//   "created_at": ISODate("2026-05-28T10:00:00Z")
// }
namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public class MongoDbAuditModel
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("tenant_id")]
        public int TenantId { get; set; }

        [BsonElement("tenant_code")]
        public string TenantCode { get; set; } = default!;

        [BsonElement("event_type")]
        public string EventType { get; set; } = default!;

        [BsonElement("action")]
        public string Action { get; set; } = default!;

        [BsonElement("user")]
        public AuditUser User { get; set; } = default!;

        [BsonElement("old_values")]
        public Dictionary<string, object>? OldValues { get; set; }

        [BsonElement("new_values")]
        public Dictionary<string, object>? NewValues { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class AuditUser
    {
        [BsonElement("id")]
        public int? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }
    }
}