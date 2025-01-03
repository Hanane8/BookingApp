using Booking.App.DTOs;
using Booking.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;

        public UserController(IUserService userService, IBookingService bookingService)
        {
            _userService = userService;
            _bookingService = bookingService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterDto registerDto)
        {
            try
            {
                var token = await _userService.RegisterUserAsync(registerDto);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            try
            {
                var token = await _userService.LoginUserAsync(loginDto);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [HttpGet("myBookings")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found.");
                }

                var bookings = await _bookingService.GetBookingsForUserAsync(Guid.Parse(userId));
                if (bookings == null || !bookings.Any())
                {
                    return NotFound("No bookings found for the logged-in user.");
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching bookings: {ex.Message}");
            }
        }




        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutUser()
        {
            try
            {
                await _userService.LogoutUserAsync();
                return Ok(new { Message = "User logged out successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Logout failed: {ex.Message}" });
            }
        }
    }
}
