using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.App.DTOs
{
    public class PerformanceDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public int ConcertId { get; set; }
        public string ConcertTitle { get; set; }
        public ConcertDto Concert { get; set; }
        public int BookingCount { get; set; }
    }
}
