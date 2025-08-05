using Booking.App.DTOs;
using Booking.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("book")]
        [Authorize]
        public async Task<IActionResult> BookPerformance([FromBody] BookPerformanceDto bookPerformanceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Console.WriteLine($"BookPerformance: Received request for PerformanceId: {bookPerformanceDto?.PerformanceId}");
                Console.WriteLine($"BookPerformance: User claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
                
                var result = await _bookingService.BookPerformanceAsync(bookPerformanceDto, User);
                Console.WriteLine($"BookPerformance: Successfully created booking");
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"BookPerformance: Validation error: {ex.Message}");
                return BadRequest(new { Error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"BookPerformance: Performance not found: {ex.Message}");
                return NotFound(new { Error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"BookPerformance: Unauthorized: {ex.Message}");
                return Unauthorized(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BookPerformance: Unexpected error: {ex.Message}");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _bookingService.CancelBookingAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings()
        {
            var Boknings = await _bookingService.GetAllBookingsAsync();
            return Ok(Boknings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDto updatedBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _bookingService.UpdateBookingAsync(id, updatedBooking);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}
    
