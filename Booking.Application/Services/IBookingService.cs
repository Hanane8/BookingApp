using Booking.App.DTOs;
using System.Security.Claims;

namespace Booking.App.Services
{
    public interface IBookingService
    {
        Task<BookPerformanceDto> BookPerformanceAsync(BookPerformanceDto bookPerformanceDto, ClaimsPrincipal currentUser);
        Task CancelBookingAsync(int bookingId);
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<ConcertDto>> GetAllConcertsAsync();
        Task<ConcertDto> GetConcertByIdAsync(int concertId);
        Task<ConcertDto> CreateConcertAsync(ConcertDto concertDto);
        Task UpdateConcertAsync(ConcertDto concertDto);
        Task DeleteConcertAsync(int concertId);
        Task<IEnumerable<PerformanceDto>> GetAllPerformancesAsync();
        Task<PerformanceDto> GetPerformanceByIdAsync(int performanceId);
        Task<PerformanceDto> CreatePerformanceAsync(PerformanceDto performanceDto);
        Task UpdatePerformanceAsync(PerformanceDto performanceDto);
        Task DeletePerformanceAsync(int performanceId);
        Task<IEnumerable<PerformanceDto>> GetPerformancesByConcertIdAsync(int concertId);
        Task<IEnumerable<BookingDto>> GetBookingsForUserAsync(Guid userId);


    }
}