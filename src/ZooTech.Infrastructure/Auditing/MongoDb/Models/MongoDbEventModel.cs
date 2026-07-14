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
//   "request": {
//     "name": "Old Name"
//   },
// 
//   "response": {
//     "name": "New Name"
//   },
// 
//   "created_at": ISODate("2026-05-28T10:00:00Z")
// }
namespace ZooTech.Infrastructure.Auditing.MongoDb.Models
{
    public class MongoDbEventModel : MongoDbBaseModel
    {
        [BsonElement("request_values")]
        public BsonValue? RequestValues { get; set; }

        [BsonElement("response_values")]
        public BsonValue? ResponseValues { get; set; }
    }
}