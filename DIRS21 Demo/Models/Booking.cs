using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DIRS21_Demo.Models
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string dbId { get; set; }
        public string bookingId { get; set; }
        public int version { get; set; }

        public BsonDateTime date { get; set; }
        public bool paid { get; set; }
    }
}