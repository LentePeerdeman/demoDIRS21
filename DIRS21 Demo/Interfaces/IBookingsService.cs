using DIRS21_Demo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DIRS21_Demo.Interfaces
{
    public interface IBookingsService
    {
        Task<string> CreateAsync(Booking input);
        Task<ServiceResultEnum> DeleteAsync(string bookingId);
        Task<ServiceResultEnum> DeleteByServiceIdAsync(string serviceId);
        Task<Booking> GetAsync(string bookingId);
        Task<IList<Booking>> GetByDayAsync(string day);
        Task<IList<Booking>> GetByServiceAndDateAsync(string serviceId, string date);
        Task<IList<Booking>> GetByServiceAsync(string service);
        Task<ServiceResultEnum> UpdateAsync(Booking input);
    }
}