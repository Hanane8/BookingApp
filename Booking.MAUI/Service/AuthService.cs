using System;
using System.Net.Http;
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
            // Replace "localhost" with your machine's IP address "192.168.0.103"
            _httpClient.BaseAddress = new Uri("http://192.168.0.103:5133/api/User/login");  // Your machine's IP address
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            try
            {
                Console.WriteLine($"Attempting to log in with username: {loginDto.UserName}");

                // Skicka inloggningsbegäran till API:t
                var response = await _httpClient.PostAsJsonAsync("api/User/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialisera JSON-svaret till LoginResponse-objektet
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        Console.WriteLine($"Received token: {loginResponse.Token}");

                        // Spara token i SecureStorage
                        await SecureStorage.SetAsync("jwt_token", loginResponse.Token);

                        return loginResponse.Token;
                    }
                    else
                    {
                        throw new Exception("Login failed: Token is null or empty.");
                    }
                }
                else
                {
                    // Logga eventuellt felet för debug-syfte
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {errorDetails}");
                    throw new Exception("Login failed: Invalid credentials.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AuthService: {ex.Message}");
                throw new Exception($"An error occurred during login: {ex.Message}");
            }
        }

        public async Task<string> GetTokenAsync()
        {
            // Hämtar token från SecureStorage
            var token = await SecureStorage.GetAsync("jwt_token");
            return token;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
}
