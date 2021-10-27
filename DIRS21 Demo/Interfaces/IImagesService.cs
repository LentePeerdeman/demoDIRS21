using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIRS21_Demo.Interfaces
{
    public interface IImagesService
    {
        Task<string> CreateAsync(Image input);
        Task<ServiceResultEnum> DeleteAsync(string imageId);
        Task<IList<Image>> GetAsync();
        Task<Image> GetAsync(string imageId);
        Task<IEnumerable<Image>> GetByServiceAsync(string serviceId);
        Task<ServiceResultEnum> UpdateAsync([FromBody] Image input);
    }
}