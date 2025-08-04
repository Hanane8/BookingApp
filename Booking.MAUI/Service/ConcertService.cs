using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Booking.App.DTOs;
using System;

namespace Booking.MAUI.Service
{
    public class ConcertService
    {
        private readonly HttpClient _httpClient;

        public ConcertService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(Constants.BaseUrl);
        }

        public async Task<List<ConcertDto>> GetAllConcertsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Concert");
                
                if (response.IsSuccessStatusCode)
                {
                    var concerts = await response.Content.ReadFromJsonAsync<List<ConcertDto>>();
                    return concerts ?? new List<ConcertDto>();
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get concerts: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting concerts: {ex.Message}");
            }
        }

        public async Task<ConcertDto> GetConcertByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Concert/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var concert = await response.Content.ReadFromJsonAsync<ConcertDto>();
                    return concert ?? throw new Exception("Concert not found");
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get concert: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting concert: {ex.Message}");
            }
        }
    }
} 