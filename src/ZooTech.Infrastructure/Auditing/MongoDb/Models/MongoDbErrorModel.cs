using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ZooTech.Infrastructure.Auditing.MongoDb.Models
{
    public class MongoDbErrorModel : MongoDbBaseModel
    {
        [BsonElement("error_information")]
        public AuditErrorInformation? AuditErrorInformation { get; set; }
    
        [BsonElement("stack_trace")]
        public string? StackTrace { get; set; }
    }

    public class AuditErrorInformation
    {
        [BsonElement("type")]
        public string? Type { get; set; }
        
        [BsonElement("message")]
        public string? Message { get; set; }
        
        [BsonElement("source")]
        public string? Source { get; set; }
        
        [BsonElement("method")]
        public string? Method { get; set; }
        
        [BsonElement("target")]
        public string? Inner { get; set; }
        
    }
}