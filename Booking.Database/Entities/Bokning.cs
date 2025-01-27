using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Database.Entities
{
    public class Bokning
    {
        public int Id { get; set; }
        public int PerformanceId { get; set; }
        public Performance Performance { get; set; }
        public Guid? UserId { get; set; } 
        public User User { get; set; }    
        public DateTime BookingDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }

}

