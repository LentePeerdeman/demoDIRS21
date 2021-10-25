using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DIRS21_Demo.Models
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string dbId { get; set; }
        public string bookingId { get; set; }
        public int version { get; set; }
        public string serviceId { get; set; }

        public string name { get; set; }
        public BsonDateTime date { get; set; }
        public bool paid { get; set; }
    }

    public class BookingRequest
    {
        public BookingRequest(string serviceId, DateTime date)
        {
            this.serviceId = serviceId;
            this.date = date;
        }

        public string serviceId { get; set; }
        public DateTime date { get; set; }
    }
}