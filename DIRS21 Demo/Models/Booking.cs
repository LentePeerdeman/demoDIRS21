using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DIRS21_Demo.Models
{
    internal class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        public string reservationId { get; set; }
        public BsonDateTime date { get; set; }
        public bool paid { get; set; }
    }
}