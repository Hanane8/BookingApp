using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Booking.App.DTOs;
using System;

namespace Booking.MAUI.Service
{
    public class BookingHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public BookingHttpService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            try
            {
                var authenticatedClient = await _authService.GetAuthenticatedHttpClientAsync();
                var fullUrl = "/api/Booking";
                var response = await authenticatedClient.GetAsync(fullUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var bookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
                    return bookings ?? new List<BookingDto>();
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get bookings: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting bookings: {ex.Message}");
            }
        }

        public async Task<BookingDto> BookPerformanceAsync(BookPerformanceDto bookPerformanceDto)
        {
            try
            {
                var authenticatedClient = await _authService.GetAuthenticatedHttpClientAsync();
                var fullUrl = "/api/Booking";
                var response = await authenticatedClient.PostAsJsonAsync(fullUrl, bookPerformanceDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var booking = await response.Content.ReadFromJsonAsync<BookingDto>();
                    return booking ?? throw new Exception("Failed to create booking");
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to book performance: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while booking performance: {ex.Message}");
            }
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                var authenticatedClient = await _authService.GetAuthenticatedHttpClientAsync();
                var fullUrl = $"/api/Booking/{bookingId}";
                var response = await authenticatedClient.DeleteAsync(fullUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to cancel booking: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while canceling booking: {ex.Message}");
            }
        }

        public async Task<BookingDto> GetBookingByIdAsync(int id)
        {
            try
            {
                var authenticatedClient = await _authService.GetAuthenticatedHttpClientAsync();
                var fullUrl = $"/api/Booking/{id}";
                var response = await authenticatedClient.GetAsync(fullUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var booking = await response.Content.ReadFromJsonAsync<BookingDto>();
                    return booking ?? throw new Exception("Booking not found");
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get booking: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting booking: {ex.Message}");
            }
        }

        public async Task<bool> UpdateBookingAsync(int bookingId, BookingDto updatedBooking)
        {
            try
            {
                var authenticatedClient = await _authService.GetAuthenticatedHttpClientAsync();
                var fullUrl = $"/api/Booking/{bookingId}";
                var response = await authenticatedClient.PutAsJsonAsync(fullUrl, updatedBooking);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to update booking: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating booking: {ex.Message}");
            }
        }
    }
} 