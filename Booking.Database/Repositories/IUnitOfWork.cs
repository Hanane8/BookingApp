using Booking.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Database.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Concert> ConcertRepository { get; }
        IRepository<Performance> PerformanceRepository { get; }
        IRepository<Bokning> BookingRepository { get; }
        IRepository<User> UserRepository { get; }
        Task SaveAsync();
    }
}

