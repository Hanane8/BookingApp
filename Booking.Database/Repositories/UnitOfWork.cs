using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Database.Database;
using Booking.Database.Entities;

namespace Booking.Database.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookingContext _context;
        

        public UnitOfWork(BookingContext context)
        {
            _context = context;
            ConcertRepository = new Repository<Concert>(context);
            PerformanceRepository = new Repository<Performance>(context);
            BookingRepository = new Repository<Bokning>(context);
            UserRepository = new Repository<User>(context);
        }

        public IRepository<Concert> ConcertRepository { get; }
        public IRepository<Performance> PerformanceRepository { get; }
        public IRepository<Bokning> BookingRepository { get; }
        public IRepository<User> UserRepository { get; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}

