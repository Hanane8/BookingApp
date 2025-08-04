using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Booking.App.DTOs;
using Microsoft.Maui.Storage;

namespace Booking.MAUI.Service
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var fullUrl = $"{Constants.BaseUrl}/api/User/login";
                var response = await _httpClient.PostAsJsonAsync(fullUrl, loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        await SecureStorage.SetAsync("jwt_token", loginResponse.Token);
                        Console.WriteLine($"Token saved: {loginResponse.Token}");
                        return loginResponse.Token;
                    }
                    else
                    {
                        throw new Exception("Login failed: Token is null or empty.");
                    }
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Login failed: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during login: {ex.Message}");
            }
        }

        public async Task<AuthResponseDto?> GetAuthenticatedUserAsync()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token is null or empty.");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var fullUrl = $"{Constants.BaseUrl}/api/User/myBookings";
            var response = await _httpClient.GetAsync(fullUrl);

            if (response.IsSuccessStatusCode)
            {
                // Assuming the response contains the authenticated user's information
                var bookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
                if (bookings != null && bookings.Any())
                {
                    var user = new AuthResponseDto
                    {
                        UserId = bookings.First().UserId,
                        UserName = "DummyUserName", 
                        Email = "DummyEmail@example.com" 
                    };
                    Console.WriteLine($"Authenticated user: {user.UserId}");
                    return user;
                }
            }

            Console.WriteLine($"Failed to get authenticated user. Status code: {response.StatusCode}");
            return null;
        }


        public async Task<HttpClient> GetAuthenticatedHttpClientAsync()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return _httpClient;
        }

        public async Task<LogoutResponse> LogoutUserAsync()
        {
            var httpClient = await GetAuthenticatedHttpClientAsync();
            var fullUrl = $"{Constants.BaseUrl}/api/User/logout";
            var response = await httpClient.PostAsync(fullUrl, null);

            if (response.IsSuccessStatusCode)
            {
                SecureStorage.Remove("jwt_token");
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return new LogoutResponse { Success = true, Message = "User logged out successfully." };
            }
            else
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                return new LogoutResponse { Success = false, Message = $"Logout failed: {errorDetails}" };
            }
        }
        public async Task<List<BookingDto>> GetMyBookingsAsync()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var httpClient = await GetAuthenticatedHttpClientAsync();
            var fullUrl = $"{Constants.BaseUrl}/api/User/myBookings";
            var response = await httpClient.GetAsync(fullUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<BookingDto>>();
            }

            return null;
        }
    }
    
    public class LoginResponse
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
    public class LogoutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}