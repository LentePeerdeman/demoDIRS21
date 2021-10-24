using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIRS21_Demo.Interfaces
{
    public interface IServicesService
    {
        Task<IList<Service>> Get();
        Task<Service> Get(string id);
        Task Create([FromBody] Service input);
        Task<ReplaceOneResult> UpdateAsync([FromBody] Service input);
        Task<DeleteResult> DeleteAsync(string serviceId);
    }
}