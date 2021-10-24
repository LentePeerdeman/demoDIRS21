using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DIRS21_Demo.Models
{
    internal class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        public string name { get; set; }
        public string url { get; set; }
    }
}