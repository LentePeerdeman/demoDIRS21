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
    public class BookingsController : ControllerBase
    {
        private readonly IBookingsService _bookingsService;

        public BookingsController(IBookingsService bookingsService)
        {
            _bookingsService = bookingsService;
        }

        // Get: api/Bookings/5
        [HttpGet("{bookingId}")]
        public async Task<Booking> GetAsync(string bookingId)
        {
            try
            {
                Booking result = await _bookingsService.GetAsync(bookingId);

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

        // Get: api/Bookings/GetByDay/01.01.1900
        [HttpGet]
        [Route("GetByDay/{day}")]
        public async Task<IList<Booking>> GetByDayAsync(string day)
        {
            try
            {
                IList<Booking> result = await _bookingsService.GetByDayAsync(day);

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

        // Get: api/Bookings/GetByService/5
        [HttpGet]
        [Route("GetByService/{serviceId}")]
        public async Task<IList<Booking>> GetByServiceAsync(string serviceId)
        {
            try
            {
                IList<Booking> result = await _bookingsService.GetByServiceAsync(serviceId);

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

        // Get: api/Bookings/GetByServiceAndDay/5/01.01.2021
        [HttpGet]
        [Route("GetByServiceAndDay/{serviceId}/{date}")]
        public async Task<IList<Booking>> GetByServiceAndDayAsync(string serviceId, string date)
        {
            try
            {
                IList<Booking> result = await _bookingsService.GetByServiceAndDateAsync(serviceId, date);

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

        // POST: api/Bookings
        [HttpPost]
        public async Task<string> PostAsync([FromBody] Booking input)
        {
            return await CreateBooking(input);
        }

        // POST: api/Bookings/5
        [HttpPost("{serviceId}")]
        public async Task<string> PostByServiceAsync(string serviceId, [FromBody] Booking input)
        {
            input.serviceId = serviceId;
            return await CreateBooking(input);
        }

        // PUT: api/Bookings/5
        [HttpPut]
        public async Task<int?> UpdateAsync([FromBody] Booking input)
        {
            try
            {
                ServiceResultEnum result = await _bookingsService.UpdateAsync(input);

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
                    Log.Information("Updated {0}", input.bookingId);
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

        // DELETE: api/Bookings/5
        [HttpDelete("{bookingId}")]
        public async Task DeleteAsync(string bookingId)
        {
            try
            {
                ServiceResultEnum result = await _bookingsService.DeleteAsync(bookingId);

                if (result == ServiceResultEnum.OK)
                {
                    Log.Information("Deleted {0}", bookingId);
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
            }

            Log.Error("Returning StatusCode 500");
            HttpContext.Response.StatusCode = 500;
        }

        private async Task<string> CreateBooking(Booking input)
        {
            try
            {
                if (await _bookingsService.CreateAsync(input) != null)
                {
                    // OK
                    HttpContext.Response.StatusCode = 200;
                    return input.bookingId;
                }
                else
                {
                    // Conflict
                    HttpContext.Response.StatusCode = 409;
                    return null;
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
    }
}
