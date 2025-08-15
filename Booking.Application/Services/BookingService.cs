using AutoMapper;
using Booking.App.DTOs;
using Booking.App.Validators;
using Booking.Database.Database;
using Booking.Database.Entities;
using Booking.Database.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Booking.App.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly BokningContext _context;
        private readonly IValidator<BookingDto> _bookingValidator;
        private readonly IValidator<BookPerformanceDto> _bookPerformanceValidator;
        private readonly HttpClient _httpClient;
        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, BokningContext context, IValidator<BookingDto> bookingValidator, IValidator<BookPerformanceDto> bookPerformanceValidator, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _bookingValidator = bookingValidator;
            _bookPerformanceValidator = bookPerformanceValidator;
            _httpClient = httpClient;
        }

        public async Task<BookPerformanceDto> BookPerformanceAsync(BookPerformanceDto bookPerformanceDto, ClaimsPrincipal currentUser)
        {
            if (bookPerformanceDto == null)
            {
                throw new ArgumentNullException(nameof(bookPerformanceDto));
            }
            
            Console.WriteLine($"BookingService: Validating BookPerformanceDto with PerformanceId: {bookPerformanceDto.PerformanceId}");
            ValidationResult validationResult = _bookPerformanceValidator.Validate(bookPerformanceDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                Console.WriteLine($"BookingService: Validation failed: {errors}");
                throw new ValidationException(validationResult.Errors);
            }

            Console.WriteLine($"BookingService: Looking up performance with ID: {bookPerformanceDto.PerformanceId}");
            var performance = await _unitOfWork.PerformanceRepository.GetByIdAsync(bookPerformanceDto.PerformanceId);
            if (performance == null)
            {
                Console.WriteLine($"BookingService: Performance not found with ID: {bookPerformanceDto.PerformanceId}");
                throw new KeyNotFoundException("Performance not found.");
            }

            var userIdString = currentUser?.FindFirstValue(ClaimTypes.NameIdentifier); 
            Console.WriteLine($"BookingService: User ID from claims: {userIdString}");

            if (Guid.TryParse(userIdString, out Guid userId)) 
            {
                Console.WriteLine($"BookingService: Looking up user with ID: {userId}");
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    Console.WriteLine($"BookingService: User not found with ID: {userId}");
                    throw new UnauthorizedAccessException("User not found.");
                }

                Console.WriteLine($"BookingService: Creating booking for user: {user.UserName}");
                var booking = new Bokning
                {
                    PerformanceId = bookPerformanceDto.PerformanceId,
                    CustomerName = user.UserName, 
                    CustomerEmail = user.Email, 
                    BookingDate = DateTime.UtcNow,
                    UserId = user.Id 
                };

                await _unitOfWork.BookingRepository.AddAsync(booking);
                await _unitOfWork.SaveAsync();
                Console.WriteLine($"BookingService: Successfully created booking with ID: {booking.Id}");

                return bookPerformanceDto;
            }
            else
            {
                Console.WriteLine($"BookingService: Invalid UserId format: {userIdString}");
                throw new UnauthorizedAccessException("Invalid UserId format.");
            }
        }        
        
        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        public async Task UpdateBookingAsync(int bookingId, BookingDto updatedBooking)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                throw new Exception("Booking not found.");
            }

            _mapper.Map(updatedBooking, booking);

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveAsync();
        }

        public async Task<BookingDto> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<IEnumerable<ConcertDto>> GetAllConcertsAsync()
        {
            var concerts = await _context.Concerts
                .Include(c => c.Performances)
                .ThenInclude(p => p.Boknings)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ConcertDto>>(concerts);
        }

        public async Task<ConcertDto> GetConcertByIdAsync(int concertId)
        {
            var concert = await _context.Concerts
                .Include(c => c.Performances)
                .ThenInclude(p => p.Boknings)
                .FirstOrDefaultAsync(c => c.Id == concertId);
            return _mapper.Map<ConcertDto>(concert);
        }

        public async Task<ConcertDto> CreateConcertAsync(ConcertDto concertDto)
        {
            var concert = _mapper.Map<Concert>(concertDto);
            await _unitOfWork.ConcertRepository.AddAsync(concert);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ConcertDto>(concert);
        }

        public async Task UpdateConcertAsync(ConcertDto concertDto)
        {
            var concert = _mapper.Map<Concert>(concertDto);
            await _unitOfWork.ConcertRepository.Update(concert);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteConcertAsync(int concertId)
        {
            var concert = await _unitOfWork.ConcertRepository.GetByIdAsync(concertId);
            if (concert != null)
            {
                await _unitOfWork.ConcertRepository.Delete(concert);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<IEnumerable<PerformanceDto>> GetAllPerformancesAsync()
        {
            var performances = await _context.Performances
                .Include(p => p.Concert)
                .Include(p => p.Boknings)
                .ToListAsync();
            return _mapper.Map<IEnumerable<PerformanceDto>>(performances);
        }

        public async Task<PerformanceDto> GetPerformanceByIdAsync(int performanceId)
        {
            var performance = await _context.Performances
                .Include(p => p.Concert)
                .Include(p => p.Boknings)
                .FirstOrDefaultAsync(p => p.Id == performanceId);
            return _mapper.Map<PerformanceDto>(performance);
        }

        public async Task<PerformanceDto> CreatePerformanceAsync(PerformanceDto performanceDto)
        {
            var performance = _mapper.Map<Performance>(performanceDto);
            await _unitOfWork.PerformanceRepository.AddAsync(performance);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<PerformanceDto>(performance);
        }

        public async Task UpdatePerformanceAsync(PerformanceDto performanceDto)
        {
            var performance = _mapper.Map<Performance>(performanceDto);
            await _unitOfWork.PerformanceRepository.Update(performance);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeletePerformanceAsync(int performanceId)
        {
            var performance = await _unitOfWork.PerformanceRepository.GetByIdAsync(performanceId);
            if (performance != null)
            {
                await _unitOfWork.PerformanceRepository.Delete(performance);
                await _unitOfWork.SaveAsync();
            }
        }
        public async Task<IEnumerable<PerformanceDto>> GetPerformancesByConcertIdAsync(int concertId)
        {
            var performances = await _context.Performances
                .Include(p => p.Concert)
                .Include(p => p.Boknings)
                .Where(p => p.ConcertId == concertId) 
                .ToListAsync();

            return _mapper.Map<IEnumerable<PerformanceDto>>(performances);
        }
        public async Task<IEnumerable<BookingDto>> GetBookingsForUserAsync(Guid userId)
        {
            var bookings = await _unitOfWork.BookingRepository.GetAllAsync(
                b => b.UserId == userId,
                include: query => query.Include(b => b.Performance)
                    .ThenInclude(p => p.Concert)
            );
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task CancelBookingAsync(int bookingId, ClaimsPrincipal currentUser = null)
        {
            Console.WriteLine($"BookingService: CancelBookingAsync called for booking ID: {bookingId}");
            
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                Console.WriteLine($"BookingService: Booking not found with ID: {bookingId}");
                throw new KeyNotFoundException("Booking not found.");
            }

            
            if (currentUser != null)
            {
                var userIdString = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"BookingService: User ID from claims: {userIdString}");

                if (Guid.TryParse(userIdString, out Guid userId))
                {
                    if (booking.UserId != userId)
                    {
                        Console.WriteLine($"BookingService: User {userId} is not authorized to cancel booking {bookingId} (owned by user {booking.UserId})");
                        throw new UnauthorizedAccessException("You are not authorized to cancel this booking.");
                    }
                }
                else
                {
                    Console.WriteLine($"BookingService: Invalid UserId format: {userIdString}");
                    throw new UnauthorizedAccessException("Invalid UserId format.");
                }
            }

            Console.WriteLine($"BookingService: Canceling booking ID: {bookingId} for user: {booking.CustomerName}");
            await _unitOfWork.BookingRepository.Delete(booking);
            await _unitOfWork.SaveAsync();
            Console.WriteLine($"BookingService: Successfully canceled booking ID: {bookingId}");
        }

    }
}
