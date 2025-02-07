using Booking.App.Services;
using Booking.App.DTOs;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Booking.MAUI.ViewModels
{
    public class UpdateBookingViewModel : BindableObject
    {
        private readonly IBookingService _bookingService;
        public BookingDto Booking { get; set; }

        public ICommand SaveBookingCommand { get; }

        public UpdateBookingViewModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
            SaveBookingCommand = new Command(async () => await SaveBookingAsync());
        }

        public async Task SaveBookingAsync()
        {
            if (Booking == null)
                return;

            await _bookingService.UpdateBookingAsync(Booking.Id, Booking);
            await Application.Current.MainPage.DisplayAlert("Success", "Booking updated!", "OK");

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
