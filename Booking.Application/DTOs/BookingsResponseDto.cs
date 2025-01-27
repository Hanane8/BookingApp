using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.App.DTOs
{
    public class BookingsResponseDto
    {
        public List<BookingDto> Bookings { get; set; }
        public Guid UserId { get; set; }
    }
}
