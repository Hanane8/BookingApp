using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Database.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Database.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly BookingContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(BookingContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T> FindAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.FirstOrDefault(predicate));
        }
        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public async Task Update(T entity) => _dbSet.Update(entity);
        public async Task Delete(T entity) => _dbSet.Remove(entity);
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

