using Booking.App.DTOs;
using Booking.App.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConcertController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public ConcertController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConcerts()
        {
            var concerts = await _bookingService.GetAllConcertsAsync();
            return Ok(concerts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConcertById(int id)
        {
            var concert = await _bookingService.GetConcertByIdAsync(id);
            if (concert == null)
            {
                return NotFound();
            }
            return Ok(concert);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConcert([FromBody] ConcertDto concertDto)
        {
            var concert = await _bookingService.CreateConcertAsync(concertDto);
            return CreatedAtAction(nameof(GetConcertById), new { id = concert.Id }, concert);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConcert(int id, [FromBody] ConcertDto concertDto)
        {
            if (id != concertDto.Id)
            {
                return BadRequest();
            }

            await _bookingService.UpdateConcertAsync(concertDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConcert(int id)
        {
            await _bookingService.DeleteConcertAsync(id);
            return NoContent();
        }
    }
}