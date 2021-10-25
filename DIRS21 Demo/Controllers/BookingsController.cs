using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IServicesService _servicesService;

        public BookingsController(IBookingsService bookingsService, IServicesService servicesService)
        {
            _bookingsService = bookingsService;
            _servicesService = servicesService;
        }

        // POST: api/Bookings/GetByDay
        [HttpPost]
        [Route("api/[controller]/GetByDayAsync")]
        public async Task<IList<Booking>> GetByDayAsync([FromBody] BookingRequest request)
        {
            HttpContext.Response.StatusCode = 200;
            return await _bookingsService.GetByDayAsync(request);
        }

        // POST: api/Bookings/GetByService
        [HttpPost]
        [Route("api/[controller]/GetByService")]
        public async Task<IList<Booking>> GetByServiceAsync([FromBody] BookingRequest request)
        {
            HttpContext.Response.StatusCode = 200;
            return await _bookingsService.GetByServiceAsync(request);
        }

        // POST: api/Bookings/GetByServiceAndDay
        [HttpPost]
        [Route("api/[controller]/GetByServiceAndDay")]
        public async Task<IList<Booking>> GetByServiceAndDayAsync([FromBody] BookingRequest request)
        {
            HttpContext.Response.StatusCode = 200;
            return await _bookingsService.GetByServiceAndDayAsync(request);
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task PostAsync([FromBody] Booking input)
        {
            HttpContext.Response.StatusCode = 205;
            await CreateBooking(input);
        }

        // POST: api/Bookings/5
        [HttpPost]
        public async Task PostByServiceAsync(string serviceId, [FromBody] Booking input)
        {
            input.serviceId = serviceId;
            await CreateBooking(input);
        }

        // PUT: api/Bookings/5
        [HttpPut]
        public async Task<int?> UpdateAsync([FromBody] Booking input)
        {
            try
            {
                Booking booking = await _bookingsService.GetAsync(input.bookingId);

                input.dbId = booking.dbId;
                input.version = input.version + 1;
                ServiceResultEnum result = await _bookingsService.UpdateAsync(input);

                if (result == ServiceResultEnum.NotFound) { HttpContext.Response.StatusCode = 404; }
                else if (result == ServiceResultEnum.BadRequest) { HttpContext.Response.StatusCode = 400; }
                else if (result == ServiceResultEnum.ResetContent) { HttpContext.Response.StatusCode = 205; }
                else if (result == ServiceResultEnum.InternalServerError) { HttpContext.Response.StatusCode = 500; }

                if (result != ServiceResultEnum.ResetContent)
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

        // DELETE: api/ApiWithActions/5
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

        private async Task CreateBooking(Booking input)
        {
            try
            {
                Service service = await _servicesService.GetAsync(input.serviceId);
                IList<Booking> bookings = await _bookingsService.GetByServiceAndDayAsync(new BookingRequest(input.serviceId, input.date.ToLocalTime()));

                if (service.quantity > bookings.Count)
                {
                    HttpContext.Response.StatusCode = 200;
                    await _bookingsService.CreateAsync(input);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                HttpContext.Response.StatusCode = 500;
            }
        }
    }
}
