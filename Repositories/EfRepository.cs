using Api.Data;
using Api.Interface;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EfRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public virtual void Update(T entity) => _dbSet.Update(entity);
        public virtual void Delete(T entity) => _dbSet.Remove(entity);
        public virtual async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public virtual IQueryable<T> Query() => _dbSet.AsQueryable();
    }
}