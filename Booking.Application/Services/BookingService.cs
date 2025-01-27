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
using System.Security.Claims;
using System.Threading.Tasks;

namespace Booking.App.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly BookingContext _context;
        private readonly IValidator<BookingDto> _bookingValidator;
        private readonly HttpClient _httpClient;
        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, BookingContext context, IValidator<BookingDto> bookingValidator, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _bookingValidator = bookingValidator;
            _httpClient = httpClient;
        }

        public async Task<BookPerformanceDto> BookPerformanceAsync(BookPerformanceDto bookPerformanceDto, ClaimsPrincipal currentUser)
        {
            if (bookPerformanceDto == null)
            {
                throw new ArgumentNullException(nameof(bookPerformanceDto));
            }
            var bookingDto = _mapper.Map<BookingDto>(bookPerformanceDto);
            ValidationResult validationResult = _bookingValidator.Validate(bookingDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var performance = await _unitOfWork.PerformanceRepository.GetByIdAsync(bookPerformanceDto.PerformanceId);
            if (performance == null)
            {
                throw new KeyNotFoundException("Performance not found.");
            }

            var userIdString = currentUser?.FindFirstValue(ClaimTypes.NameIdentifier); 

            if (Guid.TryParse(userIdString, out Guid userId)) 
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User not found.");
                }

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

                return bookPerformanceDto;
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid UserId format.");
            }
        }        
        
        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<BookingDto> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<IEnumerable<ConcertDto>> GetAllConcertsAsync()
        {
            var concerts = await _context.Concerts.Include(c => c.Performances).ToListAsync();
            return _mapper.Map<IEnumerable<ConcertDto>>(concerts);
        }

        public async Task<ConcertDto> GetConcertByIdAsync(int concertId)
        {
            var concert = await _context.Concerts.Include(c => c.Performances)
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
            var performances = await _unitOfWork.PerformanceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PerformanceDto>>(performances);
        }

        public async Task<PerformanceDto> GetPerformanceByIdAsync(int performanceId)
        {
            var performance = await _unitOfWork.PerformanceRepository.GetByIdAsync(performanceId);
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

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking != null)
            {
                await _unitOfWork.BookingRepository.Delete(booking);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new Exception("Booking not found.");
            }
        }

    }
}
