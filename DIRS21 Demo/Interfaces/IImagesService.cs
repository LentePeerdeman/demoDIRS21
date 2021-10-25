using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIRS21_Demo.Interfaces
{
    public interface IImagesService
    {
        Task CreateAsync(Image input);
        Task<ServiceResultEnum> DeleteAsync(string imageId);
        Task<ServiceResultEnum> DeleteByServiceIdAsync(string serviceId);
        Task<IList<Image>> GetAsync();
        Task<Image> GetAsync(string id);
        Task<IEnumerable<Image>> GetByServiceAsync(string serviceId);
        Task<ServiceResultEnum> UpdateAsync([FromBody] Image input);
    }
}