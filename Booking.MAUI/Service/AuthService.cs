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
            _httpClient.BaseAddress = new Uri("http://localhost:5133");  
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            try
            {
                Console.WriteLine($"Attempting to log in with username: {loginDto.UserName}");

                var response = await _httpClient.PostAsJsonAsync("api/User/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        Console.WriteLine($"Received token: {loginResponse.Token}");
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
            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");

                // Log the token value for debugging
                Console.WriteLine($"Token fetched from SecureStorage: {token}");

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token not found or is empty.");
                    throw new Exception("Token not found or is empty.");
                }

                return token;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error in AuthService: {ex.Message}");

                // Optionally, show an alert or return a specific error message
                throw new Exception($"An error occurred while getting token: {ex.Message}");
            }
        }

    }


    public class LoginResponse
    {
        public string Token { get; set; }
    }
}
