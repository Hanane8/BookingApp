using Booking.App.DTOs;

namespace Booking.App.Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(RegisterDto registerDto);
        Task<string> LoginUserAsync(LoginDto loginDto); 

        Task LogoutUserAsync();
    }
}