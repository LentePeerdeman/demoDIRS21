using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIRS21_Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DIRS21_Demo.Services;
using DIRS21_Demo.Interfaces;
using Serilog;

namespace DIRS21_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;
        private readonly IImagesService _imagesService;
        private readonly IBookingsService _bookingsService;

        public ServicesController(IServicesService servicesService, IImagesService imagesService, IBookingsService bookingsService)
        {
            _servicesService = servicesService;
            _imagesService = imagesService;
            _bookingsService = bookingsService;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<IEnumerable<Service>> GetAsync()
        {
            HttpContext.Response.StatusCode = 200;
            return await _servicesService.GetAsync();
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<Service> GetAsync(string id)
        {
            HttpContext.Response.StatusCode = 200;
            return await _servicesService.GetAsync(id);
        }

        // POST: api/Services
        [HttpPost]
        public async Task PostAsync([FromBody] Service input)
        {
            HttpContext.Response.StatusCode = 205;
            await _servicesService.CreateAsync(input);
        }

        // PUT: api/Services
        [HttpPut]
        public async Task<int?> UpdateAsync([FromBody] Service input)
        {
            try
            {
                Service service = await _servicesService.GetAsync(input.serviceId);

                input.dbId = service.dbId;
                input.version = input.version + 1;
                ServiceResultEnum result = await _servicesService.UpdateAsync(input);

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
                ServiceResultEnum deleteBookingsResult = await _bookingsService.DeleteByServiceIdAsync(serviceId);
                ServiceResultEnum deleteImagesResult = await _imagesService.DeleteByServiceIdAsync(serviceId);

                if (deleteBookingsResult == ServiceResultEnum.OK && deleteImagesResult == ServiceResultEnum.OK)
                {
                    ServiceResultEnum result = await _servicesService.DeleteAsync(serviceId);

                    if (result == ServiceResultEnum.OK)
                    {
                        Log.Information("Deleted {0}", serviceId);
                        HttpContext.Response.StatusCode = 200;
                    }
                    else
                    {
                        HttpContext.Response.StatusCode = 404;
                    }
                    return;
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
            }

            Log.Error("Returning StatusCode 500");
            HttpContext.Response.StatusCode = 500;
        }
    }
}
