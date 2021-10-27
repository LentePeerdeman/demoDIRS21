using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIRS21_Demo.Models;
using MongoDB.Driver;
using Serilog;
using DIRS21_Demo.Interfaces;

namespace DIRS21_Demo.Services
{
    public class BookingsService : IBookingsService
    {
        private readonly IMongoCollection<Service> _servicesService;
        private readonly IMongoCollection<Booking> _bookingsService;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        #region BookingsServiceConstructor
        public BookingsService(DatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
            _servicesService = _database.GetCollection<Service>(settings.ServicesCollectionName);
            _bookingsService = _database.GetCollection<Booking>(settings.BookingsCollectionName);
        }
        #endregion

        public async Task<Booking> GetAsync(string bookingId)
        {
            return await _bookingsService.Find(s => s.bookingId == bookingId).FirstOrDefaultAsync();
        }

        public async Task<IList<Booking>> GetByDayAsync(string date)
        {
            return await _bookingsService.Find(s => s.date == date).ToListAsync();
        }

        public async Task<IList<Booking>> GetByServiceAsync(string serviceId)
        {
            return await _bookingsService.Find(s => s.serviceId == serviceId).ToListAsync();
        }

        public async Task<IList<Booking>> GetByServiceAndDateAsync(string serviceId, string date)
        {
            return await _bookingsService.Find(s => s.serviceId == serviceId && s.date == date).ToListAsync();
        }

        /// <summary>
        /// Check whether the service exists and whether the booking does not yes exits
        /// Then reset dbId and Version before insert
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Task<string></returns>
        public async Task<string> CreateAsync(Booking input)
        {
            using (IClientSessionHandle session = _client.StartSession())
            {
                // Begin transaction
                //session.StartTransaction();

                try
                {
                    Service service = await _servicesService.Find(session, s => s.serviceId == input.serviceId).FirstOrDefaultAsync();
                    IList<Booking> bookings = await _bookingsService.Find(session, s => s.serviceId == input.serviceId && s.date == input.date).ToListAsync();

                    if (service.quantity > bookings.Count && await _bookingsService.Find(session, s => s.serviceId == input.serviceId).FirstOrDefaultAsync() == null)
                    {
                        input.dbId = null;
                        input.version = 0;
                        await _bookingsService.InsertOneAsync(session, input);
                        return input.bookingId;
                    }
                    else
                    {
                        Log.Information("Could not create booking");
                        //session.AbortTransaction();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Rolling back transaction, error: {0}", ex.Message);
                    //session.AbortTransaction();
                }
            }

            return null;
        }

        /// <summary>
        /// Checks whether the service does exists, gets the ID and replaces it
        /// Also increases the version number
        /// Next checks result and returns an enum with a HTTP status description
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ServiceResultEnum> UpdateAsync(Booking input)
        {
            try
            {
                Booking booking = await _bookingsService.Find(s => s.bookingId == input.bookingId).FirstOrDefaultAsync();
                input.dbId = booking.dbId;
                input.version = input.version + 1;

                ReplaceOneResult result = await _bookingsService.ReplaceOneAsync(s => s.bookingId == input.bookingId && s.version == input.version -1, input);

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
                DeleteResult result = await _bookingsService.DeleteOneAsync(s => s.bookingId == bookingId);

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
                DeleteResult result = await _bookingsService.DeleteOneAsync(s => s.serviceId == serviceId);

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
