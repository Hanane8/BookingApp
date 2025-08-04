using Booking.App.DTOs;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Booking.MAUI.Service;
using System;

namespace Booking.MAUI.ViewModels
{
    public class UpdateBookingViewModel : BindableObject
    {
        private readonly BookingHttpService _bookingService;
        public BookingDto Booking { get; set; }

        public ICommand SaveBookingCommand { get; }

        public UpdateBookingViewModel(BookingHttpService bookingService)
        {
            _bookingService = bookingService;
            SaveBookingCommand = new Command(async () => await SaveBookingAsync());
        }

        public async Task SaveBookingAsync()
        {
            if (Booking == null)
                return;

            try
            {
                await _bookingService.UpdateBookingAsync(Booking.Id, Booking);
                await Application.Current.MainPage.DisplayAlert("Success", "Booking updated successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to update booking: {ex.Message}", "OK");
            }

            // Navigate back
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public void LoadBooking(BookingDto booking)
        {
            Booking = booking;
            OnPropertyChanged(nameof(Booking));
        }
    }
}
