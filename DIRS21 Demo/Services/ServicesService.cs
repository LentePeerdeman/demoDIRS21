using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIRS21_Demo.Interfaces;
using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DIRS21_Demo.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IMongoCollection<Service> _services;

        #region BookingsServiceConstructor
        public ServicesService(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _services = database.GetCollection<Service>(settings.ServicesCollectionName);
        }
        #endregion

        public async Task<IList<Service>> GetAsync()
        {
            return await _services.Find(s => true).ToListAsync();
        }

        public async Task<Service> GetAsync(string id)
        {
            return await _services.Find(s => s.name == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync([FromBody] Service input)
        {
            input.dbId = null;
            await _services.InsertOneAsync(input);
        }

        public async Task<ReplaceOneResult> UpdateAsync([FromBody] Service input)
        {
            return await _services.ReplaceOneAsync(s => s.dbId == input.dbId, input);
        }

        public async Task<DeleteResult> DeleteAsync(string serviceId)
        {
            return await _services.DeleteOneAsync(s => s.serviceId == serviceId);
        }
    }
}
