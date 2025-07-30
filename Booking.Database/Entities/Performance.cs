using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Database.Entities
{
    public class Performance
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public int ConcertId { get; set; }
        public Concert Concert { get; set; }
        public ICollection<Bokning> Boknings { get; set; } = new List<Bokning>();
    }
}

