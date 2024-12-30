using Booking.App.DTOs;
using Booking.App.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public PerformanceController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPerformances()
        {
            var performances = await _bookingService.GetAllPerformancesAsync();
            return Ok(performances);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerformanceById(int id)
        {
            var performance = await _bookingService.GetPerformanceByIdAsync(id);
            if (performance == null)
            {
                return NotFound();
            }
            return Ok(performance);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerformance([FromBody] PerformanceDto performanceDto)
        {
            var performance = await _bookingService.CreatePerformanceAsync(performanceDto);
            return CreatedAtAction(nameof(GetPerformanceById), new { id = performance.Id }, performance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerformance(int id, [FromBody] PerformanceDto performanceDto)
        {
            if (id != performanceDto.Id)
            {
                return BadRequest();
            }

            await _bookingService.UpdatePerformanceAsync(performanceDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerformance(int id)
        {
            await _bookingService.DeletePerformanceAsync(id);
            return NoContent();
        }
    }
}