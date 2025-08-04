using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Booking.App.DTOs;
using System;

namespace Booking.MAUI.Service
{
    public class PerformanceService
    {
        private readonly HttpClient _httpClient;

        public PerformanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PerformanceDto>> GetAllPerformancesAsync()
        {
            try
            {
                var fullUrl = "/api/Performance";
                var response = await _httpClient.GetAsync(fullUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var performances = await response.Content.ReadFromJsonAsync<List<PerformanceDto>>();
                    return performances ?? new List<PerformanceDto>();
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get performances: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting performances: {ex.Message}");
            }
        }

        public async Task<List<PerformanceDto>> GetPerformancesByConcertIdAsync(int concertId)
        {
            try
            {
                var fullUrl = $"/api/Performance/concert/{concertId}";
                var response = await _httpClient.GetAsync(fullUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var performances = await response.Content.ReadFromJsonAsync<List<PerformanceDto>>();
                    return performances ?? new List<PerformanceDto>();
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get performances for concert: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting performances for concert: {ex.Message}");
            }
        }

        public async Task<PerformanceDto> GetPerformanceByIdAsync(int id)
        {
            try
            {
                var fullUrl = $"/api/Performance/{id}";
                var response = await _httpClient.GetAsync(fullUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var performance = await response.Content.ReadFromJsonAsync<PerformanceDto>();
                    return performance ?? throw new Exception("Performance not found");
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get performance: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting performance: {ex.Message}");
            }
        }
    }
} 