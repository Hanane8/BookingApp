using Booking.App.Services;
using Booking.App.DTOs;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using Booking.MAUI.Service;

namespace Booking.MAUI.ViewModels
{
    public class MyBookingsViewModel : BindableObject
    {
        private readonly IBookingService _bookingService;
        private readonly AuthService _authService;

        public ObservableCollection<BookingDto> Bookings { get; set; } = new ObservableCollection<BookingDto>();
        public BookingDto SelectedBooking { get; set; }

        public ICommand CancelBookingCommand { get; }
        public ICommand UpdateBookingCommand { get; }

        public MyBookingsViewModel(IBookingService bookingService, AuthService authService)
        {
            _bookingService = bookingService;
            _authService = authService;

            CancelBookingCommand = new Command(async () => await CancelBookingAsync());
            UpdateBookingCommand = new Command(async () => await UpdateBookingAsync());

            LoadBookingsAsync();
        }

        // Load Bookings for the current user
        public async Task LoadBookingsAsync()
        {
            var token = await _authService.GetTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You must be logged in to view your bookings.", "OK");
                return;
            }
            if (!Guid.TryParse(token, out Guid userId))
            {
                // Handle invalid GUID format
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid user ID format.", "OK");
                return;
            }

            var bookings = await _bookingService.GetBookingsForUserAsync(userId);
            Bookings.Clear();
            foreach (var booking in bookings)
            {
                Bookings.Add(booking);
            }

        }

        private async Task CancelBookingAsync()
        {
            if (SelectedBooking == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a booking to cancel.", "OK");
                return;
            }

            var confirmation = await Application.Current.MainPage.DisplayAlert("Confirm", "Are you sure you want to cancel this booking?", "Yes", "No");

            if (confirmation)
            {
                await _bookingService.CancelBookingAsync(SelectedBooking.Id);
                await Application.Current.MainPage.DisplayAlert("Success", "Booking canceled successfully!", "OK");

                // Reload the bookings
                await LoadBookingsAsync();
            }
        }

        private async Task UpdateBookingAsync()
        {
            if (SelectedBooking == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a booking to update.", "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Update", "Update functionality is not yet implemented.", "OK");
        }
    }
}
