using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIRS21_Demo.Interfaces;
using DIRS21_Demo.Models;
using MongoDB.Driver;
using Serilog;

namespace DIRS21_Demo.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IMongoCollection<Service> _servicesService;
        private readonly IMongoCollection<Image> _imagesService;
        private readonly IMongoCollection<Booking> _bookingsService;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        #region ServicesServiceConstructor
        public ServicesService(DatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
            _servicesService = _database.GetCollection<Service>(settings.ServicesCollectionName);
            _imagesService = _database.GetCollection<Image>(settings.ImagesCollectionName);
            _bookingsService = _database.GetCollection<Booking>(settings.BookingsCollectionName);
        }
        #endregion

        public async Task<IList<Service>> GetAsync()
        {
            return await _servicesService.Find(s => true).ToListAsync();
        }

        public async Task<Service> GetAsync(string serviceId)
        {
            return await _servicesService.Find(s => s.serviceId == serviceId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// check if already exists, reset dbId and version, then create
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Task<string></returns>
        public async Task<string> CreateAsync(Service input)
        {
            using (IClientSessionHandle session = await _client.StartSessionAsync())
            {
                // Begin transaction
                //session.StartTransaction();

                try
                {
                    if (await _servicesService.Find(session, s => s.serviceId == input.serviceId).FirstOrDefaultAsync() == null)
                    {
                        input.dbId = null;
                        input.version = 0;
                        _servicesService.InsertOne(session, input);
                        //session.CommitTransaction();

                        return input.serviceId;
                    }
                    Log.Information("Could not create service");
                        //session.AbortTransaction();
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
        /// <returns>Task<ServiceResultEnum></returns>
        public async Task<ServiceResultEnum> UpdateAsync(Service input)
        {
            try
            {
                // TODO: MAKE TRANSACTION SAFE
                Service service = await _servicesService.Find(s => s.serviceId == input.serviceId).FirstOrDefaultAsync();
                input.dbId = service.dbId;
                input.version = input.version + 1;

                ReplaceOneResult result = await _servicesService.ReplaceOneAsync(s => s.serviceId == input.serviceId && s.version == input.version - 1, input);

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

        /// <summary>
        /// Delets bookings, images and services based on serviceId
        /// Next checks result and returns an enum with a HTTP status description
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>Task<ServiceResultEnum></returns>
        public async Task<ServiceResultEnum> DeleteAsync(string serviceId)
        {
            using (IClientSessionHandle session = _client.StartSession())
            {
                // Begin transaction
                //session.StartTransaction();

                try
                {
                    var deleteBookingResultTask = _bookingsService.DeleteManyAsync(session, s => s.serviceId == serviceId);
                    var deleteImagesResultTask = _imagesService.DeleteManyAsync(session, s => s.serviceId == serviceId);
                    var deleteServiceResultTask = _servicesService.DeleteOneAsync(session, s => s.serviceId == serviceId);

                    if ((await deleteBookingResultTask).IsAcknowledged && (await deleteImagesResultTask).IsAcknowledged && (await deleteServiceResultTask).IsAcknowledged)
                    {
                        //session.CommitTransaction();
                        return ServiceResultEnum.OK;
                    }
                    else
                    {
                        Log.Error("Could not delete service, rolling back");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Rolling back transaction, error: {0}", ex.Message);
                }

                //session.AbortTransaction();
            }

            return ServiceResultEnum.InternalServerError;
        }
    }
}
