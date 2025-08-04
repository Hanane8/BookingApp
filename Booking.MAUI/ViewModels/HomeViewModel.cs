using Booking.App.DTOs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using System;
using Booking.MAUI.Service;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Booking.MAUI.ViewModels
{
    public class HomeViewModel : BindableObject
    {
        private readonly ConcertService _concertService;
        private readonly PerformanceService _performanceService;
        private readonly BookingHttpService _bookingService;
        private readonly AuthService _authService;
        public ObservableCollection<ConcertDto> Concerts { get; set; } = new ObservableCollection<ConcertDto>();
        public ObservableCollection<PerformanceDto> Performances { get; set; } = new ObservableCollection<PerformanceDto>();


        private ConcertDto _selectedConcert;
        public ConcertDto SelectedConcert
        {
            get => _selectedConcert;
            set
            {
                _selectedConcert = value;
                OnPropertyChanged();
                LoadPerformances();
            }
        }

        private PerformanceDto _selectedPerformance;
        public PerformanceDto SelectedPerformance
        {
            get => _selectedPerformance;
            set
            {
                _selectedPerformance = value;
                OnPropertyChanged();
            }
        }

        public ICommand BookPerformanceCommand { get; }

        public HomeViewModel(ConcertService concertService, PerformanceService performanceService, BookingHttpService bookingService, AuthService authService)
        {
            _concertService = concertService;
            _performanceService = performanceService;
            _bookingService = bookingService;
            _authService = authService;
            Concerts = new ObservableCollection<ConcertDto>();
            Performances = new ObservableCollection<PerformanceDto>();
            BookPerformanceCommand = new Command(async () => await BookPerformanceAsync());
            LoadConcertsAsync();
        }

        public async Task LoadConcertsAsync()
        {
            try
            {
                var concerts = await _concertService.GetAllConcertsAsync();
                Concerts.Clear();
                foreach (var concert in concerts)
                {
                    Concerts.Add(concert);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading concerts: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load concerts: {ex.Message}", "OK");
            }
        }


        // Load Performances for a selected concert
        private async void LoadPerformances()
        {
            if (SelectedConcert == null) return;

            var performances = await _performanceService.GetPerformancesByConcertIdAsync(SelectedConcert.Id);
            Performances.Clear();
            foreach (var performance in performances)
            {
                Performances.Add(performance);
            }
        }

        private async Task BookPerformanceAsync()
        {
            // Get the authenticated HttpClient
            var httpClient = await _authService.GetAuthenticatedHttpClientAsync();
            var token = httpClient.DefaultRequestHeaders.Authorization?.Parameter;

            if (string.IsNullOrEmpty(token))
            {
                // If the token is null or empty, show an error message
                await Application.Current.MainPage.DisplayAlert("Error", "You must be logged in to book a performance.", "OK");
                return;
            }

            // Decode the token into a ClaimsPrincipal
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                // If the token is invalid, show an error message
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid token format.", "OK");
                return;
            }

            var claimsIdentity = new ClaimsIdentity(jsonToken.Claims);
            var currentUser = new ClaimsPrincipal(claimsIdentity);

            // Check if a performance is selected
            if (SelectedPerformance == null)
            {
                // If no performance is selected, do nothing
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a performance to book.", "OK");
                return;
            }
            // Create a BookPerformanceDto object
            var bookPerformanceDto = new BookPerformanceDto
            {
                PerformanceId = SelectedPerformance.Id
            };

            try
            {
                // Call the BookingHttpService method to book the performance
                var bookedPerformance = await _bookingService.BookPerformanceAsync(bookPerformanceDto);

                // Show success message
                await App.Current.MainPage.DisplayAlert("Success", "Performance booked successfully!", "OK");
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the booking process
                await App.Current.MainPage.DisplayAlert("Error", $"Booking failed: {ex.Message}", "OK");
            }
        }
    }
}


