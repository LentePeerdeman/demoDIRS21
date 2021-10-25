using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIRS21_Demo.Models;
using MongoDB.Driver;
using Serilog;
using MongoDB.Bson;
using DIRS21_Demo.Interfaces;

namespace DIRS21_Demo.Services
{
    public class BookingsService : IBookingsService
    {
        private readonly IMongoCollection<Booking> _service;

        #region BookingsServiceConstructor
        public BookingsService(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _service = database.GetCollection<Booking>(settings.BookingsCollectionName);
        }
        #endregion

        public async Task<Booking> GetAsync(string bookingId)
        {
            return await _service.Find(s => s.bookingId == bookingId).FirstOrDefaultAsync();
        }

        public async Task<IList<Booking>> GetByDayAsync(BookingRequest request)
        {
            return await _service.Find(s => s.date == new BsonDateTime(request.date)).ToListAsync();
        }

        public async Task<IList<Booking>> GetByServiceAsync(BookingRequest request)
        {
            return await _service.Find(s => s.serviceId == request.serviceId).ToListAsync();
        }

        public async Task<IList<Booking>> GetByServiceAndDayAsync(BookingRequest request)
        {
            return await _service.Find(s => s.serviceId == request.serviceId && s.date == new BsonDateTime(request.date)).ToListAsync();
        }

        public async Task CreateAsync(Booking input)
        {
            input.dbId = null;
            await _service.InsertOneAsync(input);
        }

        public async Task<ServiceResultEnum> UpdateAsync(Booking input)
        {
            try
            {
                ReplaceOneResult result = await _service.ReplaceOneAsync(s => s.dbId == input.dbId && s.version == input.version, input);

                if (result.MatchedCount < 1) { return ServiceResultEnum.NotFound; }
                else if (result.MatchedCount != 1) { return ServiceResultEnum.BadRequest; }
                else if (result.ModifiedCount != 1) { return ServiceResultEnum.InternalServerError; }

                return ServiceResultEnum.ResetContent;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return ServiceResultEnum.InternalServerError;
        }

        public async Task<ServiceResultEnum> DeleteAsync(string bookingId)
        {
            try
            {
                DeleteResult result = await _service.DeleteOneAsync(s => s.bookingId == bookingId);

                if (result.DeletedCount == 1) { return ServiceResultEnum.OK; }
                return ServiceResultEnum.NotFound;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return ServiceResultEnum.InternalServerError;
        }

        public async Task<ServiceResultEnum> DeleteByServiceIdAsync(string serviceId)
        {
            try
            {
                DeleteResult result = await _service.DeleteOneAsync(s => s.serviceId == serviceId);

                if (result.DeletedCount == 1) { return ServiceResultEnum.OK; }
                return ServiceResultEnum.NotFound;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return ServiceResultEnum.InternalServerError;
        }
    }
}
