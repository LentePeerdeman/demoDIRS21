using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DIRS21_Demo.Models
{
    public class Service
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string dbId { get; set; }
        public string serviceId { get; set; }
        public int version { get; set; }

        public string name { get; set; }
        public string desc { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
    }
}