using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Auditing.MongoDb.Models
{
    public class MongoDbErrorModel : MongoDbBaseModel
    {
        [BsonElement("error_base_information")]
        public AuditErrorBaseInformation? AuditErrorBaseInformation { get; set; }
        
        [BsonElement("error_app_information")]
        public AuditErrorAppInformation? AuditErrorAppInformation { get; set; }
    
        [BsonElement("stack_trace")]
        public string? StackTrace { get; set; }

        [BsonElement("custom_message")]
        public string? CustomMessage { get; set; } = default!;
    }

    public class AuditErrorBaseInformation
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

    public class AuditErrorAppInformation
    {
        public string? ErrorCode { get; set; }
        public ScopeName? ScopeName { get; set; }
        public ModuleName? ModuleName { get; set; }
        public List<string>? Details { get; set; }
        public string? CompleteErrorCode { get; set; }
    }
}