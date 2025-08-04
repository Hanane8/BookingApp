using Booking.App.DTOs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using System;
using Booking.MAUI.Service;

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
                OnPropertyChanged(nameof(IsConcertSelected));
                LoadPerformances();
            }
        }

        public bool IsConcertSelected => SelectedConcert != null;

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
            // Check if user is authenticated
            var isAuthenticated = await _authService.IsUserAuthenticated();
            if (!isAuthenticated)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You must be logged in to book a performance.", "OK");
                return;
            }

            // Check if a performance is selected
            if (SelectedPerformance == null)
            {
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
                
                // Optionally refresh the performances list
                LoadPerformances();
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the booking process
                await App.Current.MainPage.DisplayAlert("Error", $"Booking failed: {ex.Message}", "OK");
            }
        }
    }
}


