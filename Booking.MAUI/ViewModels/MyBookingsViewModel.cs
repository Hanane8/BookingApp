using Booking.App.Services;
using Booking.App.DTOs;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using Booking.MAUI.Service;
using System;
using Booking.MAUI.Views;

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
            var authenticatedUser = await _authService.GetAuthenticatedUserAsync();

            if (authenticatedUser == null)
            {
                Console.WriteLine("Authenticated user is null.");
                await Application.Current.MainPage.DisplayAlert("Error", "You must be logged in to view your bookings.", "OK");
                return;
            }

            Console.WriteLine($"Authenticated user ID: {authenticatedUser.UserId}");

            var bookings = await _authService.GetMyBookingsAsync();
            if (bookings == null)
            {
                Console.WriteLine("Failed to retrieve bookings.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to retrieve bookings.", "OK");
                return;
            }

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

            var updatePage = new UpdateBookingPage();
            var updateViewModel = new UpdateBookingViewModel(_bookingService);
            updateViewModel.LoadBooking(SelectedBooking);

            updatePage.BindingContext = updateViewModel;

            await Application.Current.MainPage.Navigation.PushAsync(updatePage);
        }

    }
}
