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
                var fullUrl = "/api/User/login";
                Console.WriteLine($"LoginAsync: Making request to: {_httpClient.BaseAddress}{fullUrl}");
                var response = await _httpClient.PostAsJsonAsync(fullUrl, loginDto);

                Console.WriteLine($"LoginAsync: Response status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        await SecureStorage.SetAsync("jwt_token", loginResponse.Token);
                        Console.WriteLine($"LoginAsync: Token saved successfully, length: {loginResponse.Token.Length}");
                        Console.WriteLine($"LoginAsync: Token starts with: {loginResponse.Token.Substring(0, Math.Min(50, loginResponse.Token.Length))}...");
                        return loginResponse.Token;
                    }
                    else
                    {
                        Console.WriteLine("LoginAsync: Login failed: Token is null or empty.");
                        throw new Exception("Login failed: Token is null or empty.");
                    }
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"LoginAsync: Login failed with status {response.StatusCode}: {errorDetails}");
                    throw new Exception($"Login failed: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoginAsync: Exception occurred: {ex.Message}");
                throw new Exception($"An error occurred during login: {ex.Message}");
            }
        }

        public async Task<AuthResponseDto?> GetAuthenticatedUserAsync()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("GetAuthenticatedUserAsync: Token is null or empty.");
                return null;
            }

            Console.WriteLine($"GetAuthenticatedUserAsync: Token found, length: {token.Length}");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var fullUrl = "/api/User/myBookings";
            Console.WriteLine($"GetAuthenticatedUserAsync: Making request to: {_httpClient.BaseAddress}{fullUrl}");
            var response = await _httpClient.GetAsync(fullUrl);

            Console.WriteLine($"GetAuthenticatedUserAsync: Response status: {response.StatusCode}");

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
                    Console.WriteLine($"GetAuthenticatedUserAsync: Authenticated user: {user.UserId}");
                    return user;
                }
                else
                {
                    Console.WriteLine("GetAuthenticatedUserAsync: No bookings found for user");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetAuthenticatedUserAsync: Error response: {errorContent}");
            }

            Console.WriteLine($"GetAuthenticatedUserAsync: Failed to get authenticated user. Status code: {response.StatusCode}");
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
            var fullUrl = "/api/User/logout";
            var response = await _httpClient.PostAsync(fullUrl, null);

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
                Console.WriteLine("GetMyBookingsAsync: Token is null or empty");
                return null;
            }

            Console.WriteLine($"GetMyBookingsAsync: Token found, length: {token.Length}");
            Console.WriteLine($"GetMyBookingsAsync: Token starts with: {token.Substring(0, Math.Min(50, token.Length))}...");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var fullUrl = "/api/User/myBookings";
            Console.WriteLine($"GetMyBookingsAsync: Making request to: {_httpClient.BaseAddress}{fullUrl}");
            
            var response = await _httpClient.GetAsync(fullUrl);
            Console.WriteLine($"GetMyBookingsAsync: Response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var bookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
                Console.WriteLine($"GetMyBookingsAsync: Successfully retrieved {bookings?.Count ?? 0} bookings");
                return bookings;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetMyBookingsAsync: Error response: {errorContent}");
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