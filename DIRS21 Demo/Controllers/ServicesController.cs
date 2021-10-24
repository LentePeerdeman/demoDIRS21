using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIRS21_Demo.Models;
using DIRS21_Demo.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DIRS21_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _service;

        public ServicesController(IServicesService service)
        {
            _service = service;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<IEnumerable<Service>> GetAsync()
        {
            return await _service.Get();
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<Service> GetAsync(string id)
        {
            return await _service.Get(id);
        }

        // POST: api/Services
        [HttpPost]
        public void Post([FromBody] Service input)
        {
            _service.UpdateAsync(input);
        }

        // PUT: api/Services/5
        [HttpPut("{id}")]
        public async Task<int?> PutAsync(int id, [FromBody] Service input)
        {
            Service service = await _service.Get(input.serviceId);
            if (service.version == input.version)
            {
                input.dbId = service.dbId;
                input.version = input.version + 1;
                ReplaceOneResult result = await _service.UpdateAsync(input);

                if (result.IsAcknowledged)
                {
                    HttpContext.Response.StatusCode = 205;
                    return input.version;
                }

                HttpContext.Response.StatusCode = 409;
                return null;
            }

            HttpContext.Response.StatusCode = 404;
            return null;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task DeleteAsync(string serviceId)
        {
            DeleteResult result = await _service.DeleteAsync(serviceId);

            if (result.IsAcknowledged)
            {
                HttpContext.Response.StatusCode = 200;
                return;
            }

            HttpContext.Response.StatusCode = 404;
            return;
        }
    }
}
