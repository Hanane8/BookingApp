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

        public async Task<bool> IsUserAuthenticated()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> LoginAsync(LoginRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User/login", dto);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
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
                return null;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/User/me");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }

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

        public void Logout()
        {
            SecureStorage.Remove("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }

    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}