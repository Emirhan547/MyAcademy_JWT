using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Data.Context;

namespace MyAcademy_JWT.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _set;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
            => await _set.FindAsync(id);

        public async Task<List<T>> GetAllAsync()
            => await _set.ToListAsync();

        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}