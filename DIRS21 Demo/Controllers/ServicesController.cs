using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DIRS21_Demo.Interfaces;
using Serilog;

namespace DIRS21_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;

        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<IEnumerable<Service>> GetAsync()
        {
            // OK
            HttpContext.Response.StatusCode = 200;
            return await _servicesService.GetAsync();
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<Service> GetAsync(string id)
        {
            try
            {
                Service result = await _servicesService.GetAsync(id);

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

        // POST: api/Services
        [HttpPost]
        public async Task<string> PostAsync([FromBody] Service input)
        {
            try
            {
                // Calling the service
                string serviceId = await _servicesService.CreateAsync(input);

                if (!string.IsNullOrEmpty(serviceId))
                {
                    // Reset Content (OK)
                    HttpContext.Response.StatusCode = 205;
                    return serviceId;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            Log.Error("Returning StatusCode 500");
            HttpContext.Response.StatusCode = 500;
            return null;
        }

        // PUT: api/Services
        [HttpPut]
        public async Task<int?> UpdateAsync([FromBody] Service input)
        {
            try
            {
                // Calling the service
                ServiceResultEnum result = await _servicesService.UpdateAsync(input);

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
        [HttpDelete("{serviceId}")]
        public async Task DeleteAsync(string serviceId)
        {
            try
            {
                ServiceResultEnum result = await _servicesService.DeleteAsync(serviceId);

                if (result == ServiceResultEnum.OK)
                {
                    Log.Information("Deleted {0}", serviceId);
                    // OK
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
