using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIRS21_Demo.Interfaces;
using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Serilog;

namespace DIRS21_Demo.Services
{
    public class ImagesService : IImagesService
    {
        private readonly IMongoCollection<Service> _servicesService;
        private readonly IMongoCollection<Image> _imagesService;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        #region ImagesServiceConstructor
        public ImagesService(DatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
            _servicesService = _database.GetCollection<Service>(settings.ServicesCollectionName);
            _imagesService = _database.GetCollection<Image>(settings.ImagesCollectionName);
        }
        #endregion

        public async Task<IList<Image>> GetAsync()
        {
            return await _imagesService.Find(s => true).ToListAsync();
        }

        public async Task<Image> GetAsync(string imageId)
        {
            return await _imagesService.Find(s => s.imageId == imageId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Image>> GetByServiceAsync(string serviceId)
        {
            return await _imagesService.Find(s => s.serviceId == serviceId).ToListAsync();
        }

        /// <summary>
        /// check if already exists, reset dbId and version, then create
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Task<string></returns>
        public async Task<string> CreateAsync(Image input)
        {
            input.dbId = null;

            using (IClientSessionHandle session = _client.StartSession())
            {
                // Begin transaction
                //session.StartTransaction();

                try
                {
                    if (await _servicesService.Find(session, s => s.serviceId == input.serviceId).FirstOrDefaultAsync() != null &&
                        await _imagesService.Find(session, s => s.imageId == input.imageId).FirstOrDefaultAsync() == null)
                    {
                        input.dbId = null;
                        input.version = 0;
                        _imagesService.InsertOne(session, input);
                        //session.CommitTransaction();
                        return input.imageId;
                    }
                    else
                    {
                        Log.Information("Could not create image");
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
        /// <returns>Task<ServiceResultEnum></returns>
        public async Task<ServiceResultEnum> UpdateAsync([FromBody] Image input)
        {
            try
            {
                Image image = await _imagesService.Find(s => s.imageId == input.imageId).FirstOrDefaultAsync();
                input.dbId = image.dbId;
                input.version = input.version + 1;

                ReplaceOneResult result = await _imagesService.ReplaceOneAsync(s => s.imageId == input.imageId && s.version == input.version - 1, input);

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
        public async Task<ServiceResultEnum> DeleteAsync(string imageId)
        {
            try
            {
                if ((await _imagesService.DeleteOneAsync(s => s.imageId == imageId)).DeletedCount == 1)
                {
                    return ServiceResultEnum.OK;
                }
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
