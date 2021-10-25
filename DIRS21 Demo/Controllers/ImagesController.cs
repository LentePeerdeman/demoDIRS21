using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIRS21_Demo.Interfaces;
using DIRS21_Demo.Models;
using DIRS21_Demo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DIRS21_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesService _service;

        public ImagesController(IImagesService service)
        {
            _service = service;
        }

        // GET: api/Images
        [HttpGet]
        public async Task<IEnumerable<Image>> GetAsync()
        {
            return await _service.GetAsync();
        }

        // GET: api/Images/5
        [HttpGet("{id}")]
        public async Task<Image> GetAsync(string id)
        {
            return await _service.GetAsync(id);
        }

        // GET: api/Images/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<Image>> GetByServiceAsync(string serviceId)
        {
            return await _service.GetByServiceAsync(serviceId);
        }

        // POST: api/Images
        [HttpPost]
        public async Task PostAsync([FromBody] Image input)
        {
            await _service.CreateAsync(input);
        }

        // PUT: api/Images/5
        [HttpPut("{id}")]
        public async Task<int?> UpdateAsync([FromBody] Image input)
        {
            try
            {
                Image image = await _service.GetAsync(input.imageId);

                input.dbId = image.dbId;
                input.version = input.version + 1;
                ServiceResultEnum result = await _service.UpdateAsync(input);

                if (result == ServiceResultEnum.NotFound) { HttpContext.Response.StatusCode = 404; }
                else if (result == ServiceResultEnum.BadRequest) { HttpContext.Response.StatusCode = 400; }
                else if (result == ServiceResultEnum.ResetContent) { HttpContext.Response.StatusCode = 205; }
                else if (result == ServiceResultEnum.InternalServerError) { HttpContext.Response.StatusCode = 500; }

                if (result != ServiceResultEnum.ResetContent)
                {
                    Log.Information("Updated {0}", input.serviceId);
                    return input.version;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            HttpContext.Response.StatusCode = 500;
            return 0;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task DeleteAsync(string imageId)
        {
            try
            {
                ServiceResultEnum result = await _service.DeleteAsync(imageId);

                if (result == ServiceResultEnum.OK)
                {
                    Log.Information("Deleted {0}", imageId);
                    HttpContext.Response.StatusCode = 200;
                }
                else
                {
                    HttpContext.Response.StatusCode = 404;
                }
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                HttpContext.Response.StatusCode = 500;
            }
        }
    }
}
