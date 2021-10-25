using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DIRS21_Demo.Models
{
    public class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string dbId { get; set; }
        public string imageId { get; set; }
        public int version { get; set; }
        public string serviceId { get; set; }

        public string name { get; set; }
        public string url { get; set; }
    }
}