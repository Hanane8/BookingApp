using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.App.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int PerformanceId { get; set; }
        public PerformanceDto Performance { get; set; }
        public DateTime BookingDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }
}
