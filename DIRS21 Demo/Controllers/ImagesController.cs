using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIRS21_Demo.Interfaces;
using DIRS21_Demo.Models;
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
        [HttpGet("{serviceId}")]
        public async Task<IEnumerable<Image>> GetByServiceAsync(string serviceId)
        {
            try
            {
                IEnumerable<Image> result = await _service.GetByServiceAsync(serviceId);

                if (result == null)
                {
                    // Not found
                    HttpContext.Response.StatusCode = 404;
                }
                else
                {
                    // OK
                    HttpContext.Response.StatusCode = 200;
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            Log.Error("Returning StatusCode 500");
            HttpContext.Response.StatusCode = 500;
            return null;
        }

        // GET: api/Images/byId/5
        [HttpGet]
        [Route("byId/{imageId}")]
        public async Task<Image> GetByIdAsync(string imageId)
        {
            try
            {
                Image result = await _service.GetAsync(imageId);

                if (result == null)
                {
                    // Not found
                    HttpContext.Response.StatusCode = 404;
                }
                else
                {
                    // OK
                    HttpContext.Response.StatusCode = 200;
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            Log.Error("Returning StatusCode 500");
            HttpContext.Response.StatusCode = 500;
            return null;
        }

        // POST: api/Images
        [HttpPost]
        public async Task<string> PostAsync([FromBody] Image input)
        {
            string serviceId = await _service.CreateAsync(input);

            if (!string.IsNullOrEmpty(serviceId))
            {
                // Reset Content (OK)
                HttpContext.Response.StatusCode = 205;
                return serviceId;
            }

            Log.Error("Returning StatusCode 500");
            HttpContext.Response.StatusCode = 500;
            return null;
        }

        // PUT: api/Images
        [HttpPut]
        public async Task<int?> UpdateAsync([FromBody] Image input)
        {
            try
            {
                ServiceResultEnum result = await _service.UpdateAsync(input);

                // Not found
                if (result == ServiceResultEnum.NotFound) { HttpContext.Response.StatusCode = 404; }
                // Bad Request
                else if (result == ServiceResultEnum.BadRequest) { HttpContext.Response.StatusCode = 400; }
                // Reset Content (OK)
                else if (result == ServiceResultEnum.ResetContent) { HttpContext.Response.StatusCode = 205; }
                // Internal Server Error
                else if (result == ServiceResultEnum.InternalServerError) { HttpContext.Response.StatusCode = 500; }

                if (result == ServiceResultEnum.ResetContent)
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

            Log.Error("Returning StatusCode 500");
            HttpContext.Response.StatusCode = 500;
            return 0;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{imageId}")]
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
                    // Not found
                    HttpContext.Response.StatusCode = 404;
                }
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            Log.Error("Returning StatusCode 500");
            HttpContext.Response.StatusCode = 500;
        }
    }
}
