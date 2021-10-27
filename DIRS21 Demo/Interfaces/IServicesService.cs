using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIRS21_Demo.Interfaces
{
    public interface IServicesService
    {
        Task<string> CreateAsync(Service input);
        Task<ServiceResultEnum> DeleteAsync(string serviceId);
        Task<IList<Service>> GetAsync();
        Task<Service> GetAsync(string id);
        Task<ServiceResultEnum> UpdateAsync(Service input);
    }
}