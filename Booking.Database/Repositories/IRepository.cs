using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Database.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> FindAsync(Func<T, bool> predicate);

        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task SaveAsync();
    }
}

