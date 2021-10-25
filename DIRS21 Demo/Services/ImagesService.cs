﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Serilog;

namespace DIRS21_Demo.Services
{
    public class ImagesService : IImagesService
    {
        private readonly IMongoCollection<Image> _service;

        #region ImagesServiceConstructor
        public ImagesService(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _service = database.GetCollection<Image>(settings.ImagesCollectionName);
        }
        #endregion

        public async Task<IList<Image>> GetAsync()
        {
            return await _service.Find(s => true).ToListAsync();
        }

        public async Task<Image> GetAsync(string id)
        {
            return await _service.Find(s => s.name == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Image>> GetByServiceAsync(string serviceId)
        {
            return await _service.Find(s => s.serviceId == serviceId).ToListAsync();
        }

        public async Task CreateAsync(Image input)
        {
            input.dbId = null;
            await _service.InsertOneAsync(input);
        }

        public async Task<ServiceResultEnum> UpdateAsync([FromBody] Image input)
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

        public async Task<ServiceResultEnum> DeleteAsync(string imageId)
        {
            try
            {
                DeleteResult result = await _service.DeleteOneAsync(s => s.imageId == imageId);

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
