using DIRS21_Demo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIRS21_Demo.Interfaces
{
    public interface IBookingsService
    {
        Task CreateAsync(Booking input);
        Task<ServiceResultEnum> DeleteAsync(string bookingId);
        Task<ServiceResultEnum> DeleteByServiceIdAsync(string serviceId);
        Task<Booking> GetAsync(string bookingId);
        Task<IList<Booking>> GetByDayAsync(BookingRequest request);
        Task<IList<Booking>> GetByServiceAndDayAsync(BookingRequest request);
        Task<IList<Booking>> GetByServiceAsync(BookingRequest request);
        Task<ServiceResultEnum> UpdateAsync(Booking input);
    }
}