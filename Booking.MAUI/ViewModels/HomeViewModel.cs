using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Booking.App.DTOs;
using Booking.App.Services;
using Microsoft.Maui.Controls;

namespace Booking.MAUI.ViewModels
{
    public class HomeViewModel : BindableObject
    {
        private readonly IBookingService _bookingService;
        public ObservableCollection<ConcertDto> Concerts { get; set; } = new ObservableCollection<ConcertDto>();

        public HomeViewModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
            LoadConcertsAsync();
        }

        private async Task LoadConcertsAsync()
        {
            var concerts = await _bookingService.GetAllConcertsAsync();
            Concerts.Clear();
            foreach (var concert in concerts)
            {
                Concerts.Add(concert);
            }
        }
    }
}